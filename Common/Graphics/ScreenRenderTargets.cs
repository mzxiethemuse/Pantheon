using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pantheon.Assets;
using Terraria;
using Terraria.ModLoader;

namespace Pantheon.Common.Graphics;

public class ScreenRenderTargets : ILoadable
{
	RenderTarget2D renderTarget;
	public List<Action> actions;

	public static void AddPixelatedRenderCall(Action action)
	{
		ModContent.GetInstance<ScreenRenderTargets>().actions.Add(action);
	}
	
	public void Load(Mod mod)
	{
		actions = new List<Action>();

		if (!Main.dedServ)
		{
			// adding this makes the rendertargets resize when the resolution is changed
			// rendertargets should be initialized on the main thread :D
			Main.RunOnMainThread(() =>
			{
				Main.OnResolutionChanged += InitRT;
				renderTarget = new RenderTarget2D(Main.instance.GraphicsDevice, (int)(Main.screenWidth * 0.5), (int)(Main.screenHeight * 0.5));
			});
		}
        
		// i dont know why we use "CheckMonoliths", but that's where we hook into to draw things *to* the RT
		On_Main.CheckMonoliths += DrawToRT;
		On_Main.DrawProjectiles += On_Main_DrawProjectiles;
	}

	private void On_Main_DrawProjectiles(On_Main.orig_DrawProjectiles orig, Main self)
	{
		orig.Invoke(self);
		if (renderTarget == null || renderTarget.IsDisposed)
		{
			return;
		}
		Shaders.GaussianBlur.Value.Parameters["distance"].SetValue(800f);
		Main.spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer,
			null, Matrix.Identity);//Main.GameViewMatrix.TransformationMatrix);

		// Lines.Rectangle(new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Red);
		// Main.spriteBatch.Draw(renderTarget, new Vector2(Main.screenWidth, Main.screenHeight) / 2, Color.White);
		// Main.spriteBatch.Draw(renderTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, new Vector2(1 / SizeMultiplier.X, 1 / SizeMultiplier.Y), SpriteEffects.None, 0f);
		Main.spriteBatch.Draw(renderTarget, new Rectangle(0, 0,  Main.screenWidth, Main.screenHeight), Color.White);
		Main.spriteBatch.End();
	}

	private void DrawToRT(On_Main.orig_CheckMonoliths orig)
	{
		orig.Invoke();
		var gd = Main.graphics.GraphicsDevice;
		var oldRTs = gd.GetRenderTargets();
		gd.SetRenderTarget(renderTarget);
		gd.Clear(Color.Transparent);

		foreach (var act in actions)
		{
			act.Invoke();
		}
		// Main.spriteBatch.Draw(fuck, Vector2.Zero, Color.White);

		// Main.spriteBatch.DrawString(FontAssets.MouseText.Value, Main.screenWidth.ToString(), Vector2.Zero, Color.White, 0f, Vector2.Zero, 100f, SpriteEffects.None, 0f);
		actions.Clear();
		// thgis is a dummy thing
		gd.SetRenderTargets(oldRTs);
	}

	private void InitRT(Vector2 vec)
	{
		if (Main.dedServ)
		{
			return;
		}
        
		renderTarget?.Dispose();
		GraphicsDevice gd = Main.instance.GraphicsDevice;
		renderTarget = new RenderTarget2D(gd, (int)(vec.X * 0.5), (int)(vec.Y * 0.5));
	}

	public void Unload()
	{
	}
	
}