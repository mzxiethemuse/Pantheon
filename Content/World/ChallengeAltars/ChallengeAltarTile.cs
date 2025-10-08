using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pantheon.Assets;
using Pantheon.Common.Graphics;
using Pantheon.Common.Utils;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Pantheon.Content.World.ChallengeAltars;

public class ChallengeAltarTile : ModTile
{
		public override void SetStaticDefaults() {
			Main.tileFrameImportant[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
			TileObjectData.addTile(Type);
			AddMapEntry(new Color(200, 200, 200), Language.GetText("ChallengeAltar"));
			MinPick = 1000;
		}

		public override bool RightClick(int x, int y)
		{
			var pos = new Point(x, y).ToWorldCoordinates();
			Main.NewText("Penis");
			return true;
		}

		public override void MouseOver(int i, int j)
		{
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				Main.hoverItemName = "Challenge";
			}
			base.MouseOver(i, j);
		}

		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
			if (TileObjectData.IsTopLeft(i, j))
			{
				Main.instance.TilesRenderer.AddSpecialPoint(i, j, TileDrawing.TileCounterType.CustomNonSolid);
			}
			return base.PreDraw(i, j, spriteBatch);
		}

		public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Main.spriteBatch.End();
			ScreenRenderTargets.AddPixelatedRenderCall(() => {
				var t = Textures.Noise.Value;
                Shaders.SetUpFireShader(Color.Red, Color.Orange, 4f, 0.01f);
                Main.spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, Main.Rasterizer, Shaders.Fire.Value, Main.GameViewMatrix.TransformationMatrix);
      
                Vector2 offScreen = Main.drawToScreen ? new Vector2(Main.offScreenRange) : Vector2.Zero;
                Point p = new Point(i, j);
                //p.ToWorldCoordinates(16+8, 12) - Main.screenPosition + offScreen
                spriteBatch.Draw(t, Main.MouseScreen/ 2, null, Color.White * 0.25f, 0f, t.Size() / 2, new Vector2(32, 64) / t.Size(), SpriteEffects.FlipVertically, 0f);
                Main.spriteBatch.End();});
			
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

		}
}