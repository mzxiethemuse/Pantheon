using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pantheon.Assets;
using Pantheon.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Pantheon.Content.UI;

public class DrawOverManaBarUISystem : ModSystem
{

	public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
	{
		layers.Insert(
			layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars")) - 1,
			new LegacyGameInterfaceLayer("Pantheon:UnderManaBar", DrawBeforeResourceBars, InterfaceScaleType.UI)
		);
		layers.Insert(
			layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars")) + 1,
			new LegacyGameInterfaceLayer("Pantheon:OverManaBar", DrawAfterResourceBars, InterfaceScaleType.UI)
		);
	}

	static bool DrawBeforeResourceBars()
	{
		if (Main.netMode != NetmodeID.Server)
		{
			Player player = Main.LocalPlayer;
			if (player.ManaPlayer.manabank)
			{
				Color c = Color.White;
				int l = (player.ManaPlayer.storedInBank / 10);
				int m = player.statManaMax / 20;
				for (int i = 0; i < l; i++)
				{
					c = Color.Lerp(Color.White, Color.CornflowerBlue,
						MathF.Sin(0.5f * i + (float)Main.timeForVisualEffects * 0.02f) * 0.5f + 0.5f);
					Main.spriteBatch.Draw(OldTextures.ManaBankOverlay.Value,
						new Vector2(Main.screenWidth - 25 - 16 + 6, 37 + 16 + (i * 7)), new Rectangle(24, 0, 12, 8), c,
						0f, OldTextures.GenericItem.Size() / 2f, 1f, SpriteEffects.None, 0);
					//Main.spriteBatch.Draw(Textures.GenericItem.Value, new Vector2(Main.screenWidth - 25, 37 + (i * 22)), null, Color.White, 0f, Textures.GenericItem.Size() / 2f, 1f, SpriteEffects.None, 0);
				}

				Main.spriteBatch.Draw(OldTextures.ManaBankOverlay.Value,
					new Vector2(Main.screenWidth - 25 - 16 + 6, 37 + 16 + (l * 7)), new Rectangle(24, 10, 12, 8), c, 0f,
					OldTextures.GenericItem.Size() / 2f, 1f, SpriteEffects.None, 0);
				Main.spriteBatch.Draw(OldTextures.ManaBankOverlay.Value, new Vector2(Main.screenWidth - 25 - 16, 37),
					new Rectangle(0, 0, 22, 30), Color.White, 0f, OldTextures.GenericItem.Size() / 2f, 1f,
					SpriteEffects.None, 0);
			}

		}

		return true;

	}

	static bool DrawAfterResourceBars()
	{
		if (Main.netMode != NetmodeID.Server)
		{
			Player player = Main.LocalPlayer;
			int m = player.statManaMax2 / 20 - 1;

			if (player.ManaPlayer.manaToRegen > 0) {
				for (int i = 0; i < m + 1; i++)
				{
					Color c = Color.LightBlue * (1f + 0.5f * MathF.Sin(i + (float)Main.timeForVisualEffects * 0.1f));
					Main.spriteBatch.Draw(OldTextures.ManaStarGlowverlay.Value,
						new Vector2(Main.screenWidth - 23, 39 + (i * 22)), null, c * 0.1f, 0f,
						OldTextures.GenericItem.Size() / 2f, 1f, SpriteEffects.None, 0);
				}
			}
			if (player.ManaPlayer.wishbone && player.statMana == player.statManaMax2)
			{
				Main.spriteBatch.Draw(OldTextures.Wishbone.Value, new Vector2(Main.screenWidth - 23 - 4, 42 + (m * 22)),
					null, Color.White, 0f, OldTextures.GenericItem.Size() / 2f, 1f, SpriteEffects.None, 0);
			}


		}
		return true;
	}
}