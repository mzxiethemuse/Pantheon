using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pantheon.Assets;
using Terraria;
using Terraria.ModLoader;

namespace Pantheon.Common.Graphics;

public class PixelationRenderTarget : ManagedRenderTarget
{
	public List<Action> PixelatededActions;
	public static RenderTarget2D InstanceTarget => ModContent.GetInstance<PixelationRenderTarget>()._renderTarget; 

	protected override Point Size => new Point(Main.screenWidth / 2, Main.screenHeight / 2);

	public static void AddPixelatedRenderAction(Action action)
	{
		ModContent.GetInstance<PixelationRenderTarget>().PixelatededActions.Add(action);
	}

	public override void DrawToTarget()
	{
        foreach (var act in PixelatededActions)
        {
        	act.Invoke();
        }
        // Main.spriteBatch.Draw(fuck, Vector2.Zero, Color.White);

        // Main.spriteBatch.DrawString(FontAssets.MouseText.Value, Main.screenWidth.ToString(), Vector2.Zero, Color.White, 0f, Vector2.Zero, 100f, SpriteEffects.None, 0f);
        PixelatededActions.Clear();
        // thgis is a dummy thing
	}

	public override void OnLoad()
	{
		PixelatededActions = new List<Action>();
	}


	// public override bool PreDrawToScreen(ref Effect? effect)
	// {

	// 	return true;
	// }
	
}