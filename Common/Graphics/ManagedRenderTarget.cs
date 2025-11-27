using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pantheon.Assets;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace Pantheon.Common.Graphics;

public abstract class ManagedRenderTarget : ILoadable
{
	protected RenderTarget2D _renderTarget;
	protected virtual Point Size => new Point(Main.screenWidth, Main.screenHeight);
	
	public void Load(Mod mod)
	{
		OnLoad();
		if (!Main.dedServ)
		{
			// adding this makes the rendertargets resize when the resolution is changed
			// rendertargets should be initialized on the main thread :D
			Main.RunOnMainThread(() =>
			{
				Main.OnResolutionChanged += InitRT;
				_renderTarget = new RenderTarget2D(Main.instance.GraphicsDevice, Size.X, Size.Y);
				_renderTarget.RenderTargetUsage = RenderTargetUsage.PreserveContents;
			});
		}
        
		// i dont know why we use "CheckMonoliths", but that's where we hook into to draw things *to* the RT
		On_Main.DrawProjectiles += DrawRTToScreen;
	}

	private void DrawRTToScreen(On_Main.orig_DrawProjectiles orig, Main self)
	{
		orig.Invoke(self);
		if (_renderTarget == null || _renderTarget.IsDisposed)
		{
			return;
		}
		
		DrawToRT();

		Effect? shader = null;
		bool shouldDoDefaultDrawing = PreDrawToScreen(ref shader);
		
		if (shouldDoDefaultDrawing)
		{
			Main.spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, Main.DefaultSamplerState, default,
				Main.Rasterizer,
				shader, Matrix.Identity); //Main.GameViewMatrix.TransformationMatrix);

			Main.spriteBatch.Draw(_renderTarget, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
			Main.spriteBatch.End();
		}
	}

	private void DrawToRT()
	{
		var gd = Main.graphics.GraphicsDevice;
		var oldRTs = gd.GetRenderTargets();
		RenderTargetUtils.EnsureContentsArePreserved(oldRTs);
		gd.SetRenderTarget(_renderTarget);
		gd.Clear(Color.Transparent);

		DrawToTarget();
		
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
	}

	public void Unload()
	{
		OnUnload();
		if (!Main.dedServ)
		{
			Main.OnResolutionChanged -= InitRT;
		}
		_renderTarget?.Dispose();
		On_Main.DrawProjectiles -= DrawRTToScreen;
	}

	/// <summary>
	/// Override to set shaders or perform custom drawing for whatever reason. Default behavior is to draw the target stretched to the screen.
	/// </summary>
	/// <returns>A bool determining whether to enable the default screen drawing behavior.</returns>
	public virtual bool PreDrawToScreen(ref Effect? effect)
	{
		return true;
	}

	public abstract void DrawToTarget();

	public virtual void OnLoad() {}
	public virtual void OnUnload() {}
}