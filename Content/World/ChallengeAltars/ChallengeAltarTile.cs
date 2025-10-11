using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pantheon.Assets;
using Pantheon.Common.Graphics;
using Pantheon.Common.Utils;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace Pantheon.Content.World.ChallengeAltars;

public class ChallengeAltarTile : ModTile
{
		public static Asset<Texture2D> CrystalTexture;

		public override void Load()
		{
			CrystalTexture = ModContent.Request<Texture2D>("Pantheon/Content/World/ChallengeAltars/ChallengeAltarCrystal");
			base.Load();
		}

		public override void SetStaticDefaults() {

			Main.tileFrameImportant[Type] = true;
			Main.tileLighted[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.HookPostPlaceMyPlayer = ModContent.GetInstance<ChallengeAltarEntity>().Generic_HookPostPlaceMyPlayer;
			TileObjectData.addTile(Type);
			AddMapEntry(new Color(200, 200, 200), Language.GetText("ChallengeAltar"));
			MinPick = 1000;
		}

		public override IEnumerable<Item> GetItemDrops(int i, int j)
		{
			if (TileObjectData.IsTopLeft(i, j) && TileEntity.TryGet(i, j, out ChallengeAltarEntity entity))
			{
				var drops = new Item[entity.loot.Length];
				for (int k = 0; k < entity.loot.Length; k++)
				{
					yield return new Item(entity.loot[k], 1);
					// drops[k].Prefix(-1);
				}

				// return drops;
			}

			foreach (var itemDrop in base.GetItemDrops(i, j)) yield return itemDrop;
		}

		public override bool RightClick(int x, int y)
		{
			var pos = TileObjectData.TopLeft(x, y);
			if (TileEntity.TryGet(pos, out ChallengeAltarEntity entity))
			{
				ChallengeAltarSystem.Instance.SetActiveAltar(pos.ToPoint());
			}
			// Main.NewText("Penis");
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
			if (TileObjectData.IsTopLeft(i, j))
			{
				Vector2 offScreen = Main.drawToScreen ? new Vector2(Main.offScreenRange) : Vector2.Zero;
				Point p = new Point(i, j);
				var player = Main.LocalPlayer.GetModPlayer<ChallengeAltarPlayer>();

				if (ChallengeAltarSystem.Instance.isAltarActive && ChallengeAltarSystem.Instance.activeAltar == new Point(i, j))
				{
					ScreenRenderTargets.AddPixelatedRenderCall(() =>
					{
						var t = Textures.Noise.Value;
						// var color = Lighting.GetColor(i,j, Main.LocalPlayer.GetModPlayer<ChallengeAltarPlayer>().GetAltarFireColor());
						Shaders.SetUpFireShader(player.GetAltarFirePrimaryColor() * 0.5f,
							player.GetAltarFireSecondaryColor() * 1f, 3f, 0.01f);
						Main.spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive, SamplerState.PointWrap,
							DepthStencilState.None, Main.Rasterizer, Shaders.Fire.Value,
							Main.GameViewMatrix.TransformationMatrix);

						//p.ToWorldCoordinates(16+8, 12) - Main.screenPosition + offScreen
						spriteBatch.Draw(t, (p.ToWorldCoordinates(16 + 8, 12) + offScreen - Main.screenPosition) / 2,
							null,
							Color.White * 0.25f, 0f, t.Size() / 2, new Vector2(32, 64) / t.Size() / 2,
							SpriteEffects.FlipVertically, 0f);
						Main.spriteBatch.End();
					});
				}
				else
				{
					TileEntity.TryGet(new Point16(i, j), out ChallengeAltarEntity entity);
					Vector2 pos =
						(p.ToWorldCoordinates(16 + 8, 20 + 2 * MathF.Sin((float)(Main.timeForVisualEffects * 0.02f))) +
							offScreen - Main.screenPosition);
					if (entity != null && entity.loot != null)
					{
						int index = (int)MathF.Floor(((float)Main.timeForVisualEffects * 0.05f) % entity.loot.Length);
						// Main.NewText(entity.loot.Length + ".." + index);
						short itemID = entity.loot[index];
						var t = TextureAssets.Item[itemID].Value;
						float textureHeightToLengthRatio = t.Size().X / t.Size().Y;
						spriteBatch.Draw(t, pos, null, Color.White, 0, t.Size() / 2f, new Vector2(18 * textureHeightToLengthRatio, 18) / t.Size(), SpriteEffects.None, 0f);
					}
					
					int frameY = (int)MathF.Floor((float)((Main.timeForVisualEffects * 0.1f) % 8));
					
					spriteBatch.Draw(CrystalTexture.Value,
						pos,
						new Rectangle(0, 46 * frameY, 28, 46), player.GetAltarFirePrimaryColor() with { A = 10 }, 0f,
						new Vector2(14, 23), Vector2.One, SpriteEffects.None, 0f);


				}
			}
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			var color = Main.LocalPlayer.GetModPlayer<ChallengeAltarPlayer>().GetAltarFireSecondaryColor() * 1f;
			r = color.R / 255f;
			g = color.G / 255f;
			b = color.B / 255f;
			base.ModifyLight(i, j, ref r, ref g, ref b);
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			ModContent.GetInstance<ChallengeAltarEntity>().Kill(i, j);
		}
}

public class ChallengeAltarEntity : ModTileEntity
{
	// What does it do?:
	// When you right click a Challenge Altar, it should activate the modsystem
	// the tile entity should handle:
	// storing the loot,
	public override bool IsTileValidForEntity(int x, int y) => Framing.GetTileSafely(x, y).TileType == ModContent.TileType<ChallengeAltarTile>() && TileObjectData.IsTopLeft(x, y);


	public short[] loot = [ItemID.HermesBoots, ItemID.LifeCrystal, ItemID.BattlePotion];
	
	
	public override void SaveData(TagCompound tag)
	{
		tag.Add("AltarLoot", loot);
		base.SaveData(tag);
	}

	public override void LoadData(TagCompound tag)
	{
		loot = tag.Get<short[]>("AltarLoot");
		base.LoadData(tag);
	}
	
	
}