using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pantheon.Assets;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pantheon.Common.Players;

// general catchall for buffs
public class BuffEffectPlayer : ModPlayer
{
	public bool lobotomize = false;
	public bool penis = false;
	private bool penis2 = false;
	private Vector2 cameraOffsetLobotomize = Vector2.Zero;
	private Vector2 gay = Vector2.Zero;
	public float penis3 = 0;
	public override void Load()
	{
		On_Main.DrawProjectiles += (orig, self) =>
		{
			orig(self);
			if (Main.LocalPlayer.GetModPlayer<BuffEffectPlayer>().lobotomize)
			{
				Shaders.GaussianBlur.Value.Parameters["distance"].SetValue(80f);
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState,
					default, Main.Rasterizer, Shaders.GaussianBlur.Value, Main.GameViewMatrix.TransformationMatrix);
				if (Main.LocalPlayer.GetModPlayer<BuffEffectPlayer>().penis3 > 0) Main.spriteBatch.Draw(Textures.Beatles.Value,
					Main.ScreenSize.ToVector2() / 2, null, Color.White, 0f, Textures.Beatles.Size() / 2,
                							new Vector2(1, 0.75f), SpriteEffects.None, 0f);
				for (int i = 0; i < 4; i++)
				{
					if (Main.rand.NextBool(20))
					{
						Vector2 pos = Main.LocalPlayer.Center + Main.rand.NextVector2Circular(2000, 800);
						Main.spriteBatch.Draw(TextureAssets.Npc[Main.rand.Next(TextureAssets.Npc.Length)].Value,
							pos - Main.screenPosition, null, Color.White, 0f, Vector2.Zero,
							Main.rand.NextVector2Square(3, 4), SpriteEffects.None, 0f);
					}
				}

				Main.spriteBatch.End();
			}
		};
		Filters.Scene["Pantheon:Lobotomize"] = new Filter(new ScreenShaderData(Shaders.GaussianBlur2, "FilterMyShader"));
		Filters.Scene["Pantheon:Lobotomize2"] = new Filter(new ScreenShaderData(Shaders.GaussianBlur2, "FilterMyShader"));

		base.Load();
	}

	public override void PostUpdate()
	{
		
		if (lobotomize)
		{
			// we want it to take 3 seconds, so 255 / 3
			penis3 -= (255 / 0.06f) * (1/ 60f);
			penis3 = MathF.Max(penis3, 0);
			for (int i = 0; i < 16; i++)
			{
				float xVel = Player.direction * 7 + Player.velocity.X;
				Dust.NewDust(Player.position, Player.width, Player.height / 3, DustID.Blood, xVel, -5, 0, default, 0.5f);
			}
			if (penis == false || Main.rand.NextBool(200) || (Player.velocity.X == 0 && Player.oldVelocity.X != 0 && penis2 == false))
			{
				penis3 = 255;
				penis2 = false;
				SoundEngine.PlaySound(Sounds.Lobotomy.WithPitchVariance(0.15f));
				// Main.timeForVisualEffects = Main.rand.NextDouble() * 10;
				cameraOffsetLobotomize = Main.rand.NextVector2CircularEdge(3000, 3000);
				
				if (Main.rand.NextBool(9))
				{
					cameraOffsetLobotomize = -Main.screenPosition;
				} else if (Main.rand.NextBool(Main.rand.Next(6, 7)))
				{
					Player.position += cameraOffsetLobotomize * -0.25f;
				}

				if (gay == Vector2.Zero)
				{
					gay = Player.position;
				}
				else
				{
					Player.position = gay;
					gay *= 0;
				} 
			}

			penis = true;
			// lobotomize = false;
			Filters.Scene.Activate("Pantheon:Lobotomize");
			Filters.Scene.Activate("Pantheon:Lobotomize2");
			Player.AddBuff(BuffID.Confused, 60 * 5);
		}
		else
		{
			Filters.Scene.Deactivate("Pantheon:Lobotomize");
			Filters.Scene.Deactivate("Pantheon:Lobotomize2");
		}
		base.PostUpdate();
	}
	
	

	public override void ModifyScreenPosition()
	{
		Main.screenPosition = Main.screenPosition + cameraOffsetLobotomize;
		cameraOffsetLobotomize = Vector2.Lerp(cameraOffsetLobotomize, Vector2.Zero, 0.05f);
		base.ModifyScreenPosition();
	}
}