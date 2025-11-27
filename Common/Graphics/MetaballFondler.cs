using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pantheon.Assets;
using Terraria;
using Terraria.ModLoader;

namespace Pantheon.Common.Graphics;

/// <summary>
/// most of this is copied over from ManagedRenderTarget cuz i didnt wanna modify that class too much just to fit the potential of this one
/// </summary>
public abstract class MetaballFondler : ILoadable
{
	protected RenderTarget2D _renderTarget;
	public RenderTarget2D MetaballMask;
	
	protected virtual Color Color => Color.White;
	protected virtual Color OutlineColor => Color.White;
	protected virtual Point Size => new Point(Main.screenWidth, Main.screenHeight);
	
	public void Load(Mod mod)
	{
		if (!Main.dedServ)
		{
			// adding this makes the rendertargets resize when the resolution is changed
			// rendertargets should be initialized on the main thread :D
			Main.RunOnMainThread(() =>
			{
				Main.OnResolutionChanged += InitRT;
				MetaballMask = new RenderTarget2D(Main.instance.GraphicsDevice, Size.X / 2, Size.Y / 2);
				MetaballMask.RenderTargetUsage = RenderTargetUsage.PreserveContents;
				_renderTarget = new RenderTarget2D(Main.instance.GraphicsDevice, Size.X, Size.Y);
				_renderTarget.RenderTargetUsage = RenderTargetUsage.PreserveContents;
			});
		}
        
		// i dont know why we use "CheckMonoliths", but that's where we hook into to draw things *to* the RT
		On_Main.CheckMonoliths += DrawToRT;
		On_Main.DrawProjectiles += DrawRTToScreen;
		// ModContent.GetInstance<OutlineRenderTarget>().RenderToOutlineTarget += OnRenderToOutlineTarget;
	}

	// private void OnRenderToOutlineTarget(Matrix matrix)
	// {
	// 	Main.spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, Main.DefaultSamplerState, default,
 //        	Main.Rasterizer,
 //        	null, Matrix.Identity);
 //        Main.spriteBatch.Draw(MetaballMask, Vector2.Zero, null, OutlineColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);		
 //        
 //        Main.spriteBatch.End();
 //
	// }

	private void DrawRTToScreen(On_Main.orig_DrawProjectiles orig, Main self)
	{

		orig.Invoke(self);
		if (_renderTarget == null || _renderTarget.IsDisposed)
		{
			return;
		}
		

		Effect? shader = null;
		Texture2D? texture2D = null;
		bool shouldDoDefaultDrawing = PreDrawToScreen(ref shader, ref texture2D);
		
		Shaders.Threshold.Value.Parameters["threshold"].SetValue(2f);
		if (shouldDoDefaultDrawing)
		{
			; //Main.GameViewMatrix.TransformationMatrix);

			if (texture2D != null)
			{
				Main.graphics.GraphicsDevice.Textures[1] = texture2D;
				Main.spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.PointWrap, default,
					Main.Rasterizer,
					null, Matrix.Identity);
				Main.spriteBatch.Draw(MetaballMask, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
				Main.spriteBatch.End();

			}
			else
			{
				Main.spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.PointWrap, default,
					Main.Rasterizer,
					shader, Matrix.Identity);
				Main.spriteBatch.Draw(MetaballMask, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color);
				Main.spriteBatch.End();
			}
			Shaders.Outline.Value.Parameters["staticColor"].SetValue(OutlineColor.ToVector4());
			Shaders.Outline.Value.Parameters["textureSize"].SetValue(MetaballMask.Size());
			Main.spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, Main.DefaultSamplerState, default,
				Main.Rasterizer,
				Shaders.Outline.Value, Matrix.Identity);
			Main.spriteBatch.Draw(MetaballMask, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color);
			Main.spriteBatch.End();
		}
	}

	private void DrawToRT(On_Main.orig_CheckMonoliths orig)
	{
		orig.Invoke();
		var gd = Main.graphics.GraphicsDevice;
		var oldRTs = gd.GetRenderTargets();
		gd.SetRenderTarget(_renderTarget);
		gd.Clear(Color.Transparent);

		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
		DrawMetaballsToTarget();
		Main.spriteBatch.End();
		gd.SetRenderTarget(MetaballMask);
		gd.Clear(Color.Transparent);

		Shaders.Threshold.Value.Parameters["threshold"].SetValue(0.8f);
		Main.spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Opaque, Main.DefaultSamplerState, default, Main.Rasterizer, Shaders.Threshold.Value, Matrix.Identity);
		Main.spriteBatch.Draw(_renderTarget, new Rectangle(0, 0, _renderTarget.Width / 2,  _renderTarget.Height / 2), OutlineColor);
		Main.spriteBatch.End();
		gd.SetRenderTargets(oldRTs);

	}

	private void InitRT(Vector2 vec)
	{
		if (Main.dedServ)
		{
			return;
		}
        
		_renderTarget?.Dispose();
		GraphicsDevice gd = Main.instance.GraphicsDevice;
		_renderTarget = new RenderTarget2D(gd, Size.X, Size.Y);
		_renderTarget.RenderTargetUsage = RenderTargetUsage.PreserveContents;
		MetaballMask = new RenderTarget2D(Main.instance.GraphicsDevice, Size.X / 2, Size.Y / 2);
		MetaballMask.RenderTargetUsage = RenderTargetUsage.PreserveContents;
	}

	public void Unload()
	{
		if (!Main.dedServ)
		{
			Main.OnResolutionChanged -= InitRT;
		}
		_renderTarget?.Dispose();
		On_Main.CheckMonoliths -= DrawToRT;
		On_Main.DrawProjectiles -= DrawRTToScreen;
	}

	/// <summary>
	/// Override to set shaders or a source image to apply the metaballs as a mask to.
	/// </summary>
	/// <returns>A bool determining whether to enable the default screen drawing behavior.</returns>
	public virtual bool PreDrawToScreen(ref Effect? effect, ref Texture2D? texture)
	{
		return true;
	}

	public abstract void DrawMetaballsToTarget();

	public abstract void OnLoad();
	public abstract void OnUnload();
}