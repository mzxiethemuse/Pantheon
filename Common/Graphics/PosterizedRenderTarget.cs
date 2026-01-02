using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pantheon.Assets;
using Terraria;
using Terraria.ModLoader;

namespace Pantheon.Common.Graphics;

public class PosterizedRenderTarget : ManagedRenderTarget
{
	public List<Action> PosterizedActions;

	protected override bool BehindProjectiles => true;
	protected override Point Size => new Point(Main.screenWidth / 2, Main.screenHeight / 2);

	public static void AddPosterizedRenderAction(Action action)
	{
		ModContent.GetInstance<PosterizedRenderTarget>().PosterizedActions.Add(action);
	}

	public override void DrawToTarget()
	{
        foreach (var act in PosterizedActions)
        {
        	act.Invoke();
        }
        // Main.spriteBatch.Draw(fuck, Vector2.Zero, Color.White);

        // Main.spriteBatch.DrawString(FontAssets.MouseText.Value, Main.screenWidth.ToString(), Vector2.Zero, Color.White, 0f, Vector2.Zero, 100f, SpriteEffects.None, 0f);
        PosterizedActions.Clear();
        // thgis is a dummy thing
	}

	public override void OnLoad()
	{
		PosterizedActions = new List<Action>();
	}


	public override bool PreDrawToScreen(ref Effect? effect)
	{
		OldShaders.ColorQuantize.Value.Parameters["levels"].SetValue(9);
		effect = OldShaders.ColorQuantize.Value;

		// Main.spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer,
		// 	Shaders.ColorQuantize.Value, Matrix.Identity);//Main.GameViewMatrix.TransformationMatrix);
		//
		// // Lines.Rectangle(new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Red);
		// // Main.spriteBatch.Draw(renderTarget, new Vector2(Main.screenWidth, Main.screenHeight) / 2, Color.White);
		// // Main.spriteBatch.Draw(renderTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, new Vector2(1 / SizeMultiplier.X, 1 / SizeMultiplier.Y), SpriteEffects.None, 0f);
		// Main.spriteBatch.Draw(_renderTarget, new Rectangle(0, 0,  Main.screenWidth, Main.screenHeight), Color.White);
		// Main.spriteBatch.End();
		return true;
	}
	
}