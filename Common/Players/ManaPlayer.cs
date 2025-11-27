using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pantheon.Assets;
using Pantheon.Common.Utils;
using Pantheon.Content.Combat.Mage;
using Pantheon.Content.General.Dusts;
using Pantheon.Content.General.Dusts.Bursts;
using Pantheon.Content.Projectiles;
using ReLogic.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Biomes.CaveHouse;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pantheon.Common.Players;

public class ManaPlayer : ModPlayer
{
	
	
	private RenderTarget2D _manaOpticRT;
	private Vector2 _DetourMomentThisWillProbablyBreakALot;
	public int scratch = 0;
	public int manaRegenTimer;
	public int manaToRegen;
	private int _regenPerSecond;
	public int regenPerSecond {get => _regenPerSecond;
		set =>
			_regenPerSecond = _regenPerSecond == 0 ? value : _regenPerSecond;
		
	}
	private int usedMana;
	
	//this is deadass so bad
	//blame jamz
	public ushort[] manaUsages = new ushort[600];
	
	
	
	
	public int ManaFromPast10Seconds;
	public int ManaFromPast5Seconds;
	
	
	public bool manastorm;
	public bool displayManaRegenTicks;
	public bool displayManaUsage;
	public bool wishbone;
	
	public bool manabank;
	public int storedInBank;
	// Sally has 20 mana stored at the bank with -2%/day interest. She comes back a week later to collect her money. How much money does the bank give her?
	public int bankLossTimer;
	
	public bool athame;
	public bool pincushion;
	public bool hollowRock;

	public int manaBigUnitUsed;
	public bool focuslens;
	public bool nadir;

	public override void Load()
	{
		if (!Main.dedServ)
		{

			// rendertargets should be initialized on the main thread :D
			Main.RunOnMainThread(() =>
			{
				_manaOpticRT = new RenderTarget2D(Main.instance.GraphicsDevice, 125, 100);
			});
			On_Main.CheckMonoliths += DrawToRT;
			On_Main.DrawProjectiles += On_Main_DrawProjectiles;
		}
	}

	

	public override void ResetEffects()
	{
		focuslens = false;
		hollowRock = false;
		pincushion = false;
		athame = false;
		manabank = false;
		nadir = false;
		wishbone = false;
		displayManaUsage = false;
		displayManaRegenTicks = false;
		manastorm = false;
		base.ResetEffects();
	}

	public override void PreUpdate()
	{
		
		_DetourMomentThisWillProbablyBreakALot = Player.Center;
		manaRegenTimer++;
		if (manaToRegen > 0 && manaRegenTimer >= (60 / regenPerSecond))
		{
			Dust.NewDust(Player.position, Player.width, Player.height, DustID.MagicMirror);

			scratch++;
			// Main.NewText("added 1 mana after " + manaRegenTimer + " ticks at " + regenPerSecond);
			// Main.NewText(manaToRegen);
			manaToRegen -= 1;
			Player.statMana += 1;
			manaToRegen = Math.Max(0, manaToRegen);
			manaRegenTimer = 0;
			if (manaToRegen <= 0)
			{
				// Main.NewText("finished replenishing mana after " + scratch + "regen ticks");
				_regenPerSecond = 0;
			}

			Player.netMana = true;
		}

		
		usedMana = Player.statMana;


		
		DoManaBankLogic();
		base.PreUpdate();
	}

	public override void PostUpdateBuffs()
	{
		if (Player.HasBuff<WishboneBuff>())
		{
			Player.GetDamage(DamageClass.Magic) += 0.2f;
			Player.GetAttackSpeed(DamageClass.Magic) += 0.2f;
			Player.manaCost -= 0.1f;
		}
		base.PostUpdateBuffs();
	}

	public override void PostUpdateEquips()
	{
		if (hollowRock && pincushion)
		{
			if (Player.ManaPlayer().pincushion)
			{
				Player.GetDamage(DamageClass.Magic) += 0.35f *
				                                       ((Player.statLifeMax2 - Player.statLife) /
				                                        (float)(Player.statLifeMax2));
			}
		}
		base.PostUpdateEquips();
	}

	public override void PostUpdate()
	{
		Player.manaRegenDelay = MathF.Min(Player.manaRegenDelay, 60);
		// if (manaUsages.Count != 0)
		// {
		// 	Main.NewText(manaUsages[^1]);
		// }
		
		// Circular Buffer ? FUck is that? FUck you
		for (int i = 0; i < 600; i++)
		{
			manaUsages[i] = manaUsages[Math.Min(599, i + 1)];
		}
		manaUsages[599] = (ushort)Math.Max(0, usedMana - Player.statMana) ;

		
		
		ManaFromPast5Seconds = GetManaUsageFromPastTicks(60 * 5);
		ManaFromPast10Seconds = GetManaUsageFromPastTicks(60 * 10);
		
	}

	public override void OnConsumeMana(Item item, int manaConsumed)
	
	{
		if ((Player.statManaMax2 - Player.statMana) / 20 > manaBigUnitUsed)
		{
			Per20ManaUsed();
		}
		if (manabank)
		{
			bankLossTimer /= 2;
			storedInBank += manaConsumed / 2;
		}
		// WISHBONE EFFECT
	
		if (wishbone)
		{
			if (Player.statMana == Player.statManaMax2)
			{
				DoWishboneCrackEffect();

			}
		}
		
		// DREAMCATCHER
		if (manastorm)
		{
			//nested if? more like, Gay butt sex
			if (Player.statMana > ((float)Player.statManaMax2 * 0.75f))
			{
				TrySetManaRegenTicks(40, 20);
			}
		}
		
		manaBigUnitUsed = (Player.statManaMax2 - Player.statMana) / 20;
	}

	private void Per20ManaUsed()
	{
		if (focuslens)
		{
			if (Player == Main.LocalPlayer)
			{
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center,
                	Player.Center.DirectionTo(Main.MouseWorld) * 7f, ModContent.ProjectileType<FocusLightOrb>(), 10, 1f, Player.whoAmI, Main.rand.NextFloat(-2, 2));
			}

		}
	}

	public override void OnMissingMana(Item item, int neededMana)
	{
		if (athame)
		{
			Player.statLife -= neededMana;
			CombatText.NewText(new Rectangle((int)Player.position.X, (int)Player.position.Y, Player.width, Player.height), CombatText.DamagedFriendly, neededMana);
			Player.statMana += neededMana;
			if (Player.statLife <= 0)
			{
				//TODO: change me to NetworkText
				Player.KillMe(PlayerDeathReason.ByCustomReason($"{Player.name} bled out."), 1000, 0, false);
				Player.statLife = 0;
			}
		} else
		{
			if (manabank && storedInBank > neededMana)
			{
				Player.statMana += storedInBank;
				storedInBank = 0;
				SoundEngine.PlaySound(Sounds.ManaBankRestore.WithVolumeScale(0.7f), Player.Center);
				if (wishbone)
				{
					DoWishboneCrackEffect();
				}
			}
		}

	}

	public void DoWishboneCrackEffect()
	{
		SoundEngine.PlaySound(SoundID.DD2_SkeletonHurt, Player.Center);
		SoundEngine.PlaySound(SoundID.DD2_SkeletonDeath, Player.Center);
		Burst.SpawnBurstDust(ModContent.DustType<Burst>(), Player.Center, 0.5f, Color.DarkKhaki * 1.5f, 4f, 0.9f, 3f);

		for (int i = 0; i < 28; i++)
		{
			Vector2 r = Main.rand.NextVector2Circular(2f, 2f);
			Dust d = Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.Bone, r.X, r.Y, 0, default(Color), Main.rand.NextFloat(1f, 2f));
			d.noGravity = true;
			if (Main.rand.NextBool(3))
			{
				Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.TintableDustLighted, r.X * 0.07f, r.Y * 0.07f, 178, Color.Khaki * 0.2f, Main.rand.NextFloat(1f, 2f));
				Dust.NewDustDirect(Player.position, Player.width, Player.height, ModContent.DustType<Dirt>(), r.X * 0.02f, r.Y * 0.02f, 99, Color.DarkKhaki * 0.2f, Main.rand.NextFloat(4f, 6f));
			}
		}
		Player.AddBuff(ModContent.BuffType<WishboneBuff>(), 60 * 2);

		Dust.NewDustPerfect(Player.Center, ModContent.DustType<WishboneCrack>(),
			new Vector2(-2f * Main.rand.NextFloat(0.9f, 1.5f), -8f), 0, Color.White, 1f).customData = 0;
		Dust.NewDustPerfect(Player.Center, ModContent.DustType<WishboneCrack>(),
			new Vector2(2f * Main.rand.NextFloat(0.9f, 1.5f), -8f), 0, Color.White, 1f).customData = 1;
	}

	public override void OnHurt(Player.HurtInfo info)
	{
		if (!info.Cancelled && pincushion)
		{
			Player.statMana = Math.Min(Player.statMana + 10, Player.statManaMax2);
			Player.ManaEffect(10);
			
		}
		base.OnHurt(info);
	}

	void DoManaBankLogic()
	{
		bankLossTimer++;
		if (bankLossTimer >= 30 && manabank && storedInBank > 0)
		{
			storedInBank -= (Player.statMana == Player.statManaMax2 ? 10 : 2);
			if (storedInBank < 0)
			{
				storedInBank = 0;
			}
			bankLossTimer = 0;
		}
	}
	
	public void TrySetManaRegenTicks(int amount, int perSecond)
	{
		if (manaToRegen == 0)
		{
			manaToRegen = amount;
			if (manastorm)
			{
				SoundEngine.PlaySound(Sounds.DreamcatcherMana.WithVolumeScale(0.7f), Player.Center);
				for (int i = 0; i < 7; i++)
				{
					Dust.NewDustDirect(Player.position, Player.width, Player.height, ModContent.DustType<Dirt>(), 2, 2, 0, Color.Blue * 0.2f, Main.rand.NextFloat(4f, 8f));

				}
			}
			Burst.SpawnBurstDust(ModContent.DustType<MagicBurst>(), Player.Center, 0.5f, Color.Blue * 0.8f, 4f, 0.73f, 3f);
			for (int i = 0; i < 40; i++)
			{
				Dust.NewDust(Player.position, Player.width, Player.height, DustID.MagicMirror);
			}
		}

		regenPerSecond = perSecond;
	}

	public int GetManaUsageFromPastTicks(int ticks)
	{
		int mana = 0;
		for (int i = 1; i < ticks; i++)
		{
			mana += manaUsages[^i];
		}
		return mana;
	}

	public void ResetManaUsage()
	{
		manaUsages = new ushort[600];
	}


	private void On_Main_DrawProjectiles(On_Main.orig_DrawProjectiles orig, Main self)
	{
		orig.Invoke(self);
		if (_manaOpticRT == null || _manaOpticRT.IsDisposed || Main.myPlayer == 255)
		{
			return;
		}
		ManaPlayer mana;
		if (Main.LocalPlayer.TryGetModPlayer<ManaPlayer>(out mana) == false) return;
		if (!mana.displayManaUsage && !mana.displayManaRegenTicks)
		{
			return;
		}
		

		Main.spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive, Main.DefaultSamplerState, default, Main.Rasterizer,
			Shaders.ManaOpticDisplay.Value, Matrix.Identity);
		// Lines.Rectangle(new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Red);
		// Main.spriteBatch.Draw(renderTarget, new Vector2(Main.screenWidth, Main.screenHeight) / 2, Color.White);
		// Main.spriteBatch.Draw(renderTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, new Vector2(1 / SizeMultiplier.X, 1 / SizeMultiplier.Y), SpriteEffects.None, 0f);
		Main.spriteBatch.Draw(_manaOpticRT,
			(Main.LocalPlayer.position - Main.screenPosition +
			 new Vector2(26 + Main.LocalPlayer.width / 2, 15 + Main.LocalPlayer.gfxOffY)), null, Color.White, 0f, new Vector2(0, 50), new Vector2(0.95f, 0.85f) * 0.8f, SpriteEffects.None, 0f);

		Main.spriteBatch.End();	
		
		
	}

	// problematic
	private void DrawToRT(On_Main.orig_CheckMonoliths orig)
	{
		orig.Invoke();
		if (_manaOpticRT == null || _manaOpticRT.IsDisposed || Main.myPlayer == 255)
		{
			return;
		}
		ManaPlayer mana;
		if (Main.LocalPlayer.TryGetModPlayer<ManaPlayer>(out mana) == false) return;
		// get the old rendert rgaters
		var gd = Main.graphics.GraphicsDevice;
		var oldRTs = gd.GetRenderTargets();
		
		gd.SetRenderTarget(_manaOpticRT);
		gd.Clear(Color.Transparent);
		Main.spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
		Main.spriteBatch.Draw(Textures.ManaOpticDisplayBG.Value, Vector2.Zero, null, Color.White * 0.35f, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);

		Main.spriteBatch.End();

		Shaders.Scanline.Value.Parameters["uScanlineCount"].SetValue(19);
		Shaders.Scanline.Value.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.01f);
		Shaders.Scanline.Value.Parameters["uColor"].SetValue((Color.CornflowerBlue * 0.25f).ToVector4() );
		
		Main.spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, Shaders.Scanline.Value, Main.GameViewMatrix.TransformationMatrix);
		DebugLines.Rectangle(new Rectangle(0, 0, _manaOpticRT.Width, _manaOpticRT.Height), Color.White);		
		
		Main.spriteBatch.End();

		Main.spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

		// Main.spriteBatch.Draw(Textures.ManaOpticDisplayBG.Value, Vector2.Zero, null, Color.White * 0.35f, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
		//Detours are fucky as hell
		if (mana.displayManaRegenTicks && mana.manaToRegen > 0)
		{
			Main.spriteBatch.DrawString(FontAssets.MouseText.Value, mana.regenPerSecond.ToString() + "/sec",
				new Vector2(20, 15),
				Color.LightBlue * 0.5f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			Main.spriteBatch.DrawString(FontAssets.MouseText.Value,
				(1 + (mana.manaToRegen / Math.Max(mana.regenPerSecond, 1))) + " secs", new Vector2(20, 15 + 22),
				Color.LightBlue * 0.5f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
		}
		if (mana.displayManaUsage)
		{
			Main.spriteBatch.DrawString(FontAssets.MouseText.Value, mana.ManaFromPast10Seconds.ToString() + "/10sec",
				new Vector2(20, 15 + 45),
				Color.LightBlue * 0.5f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
		}
		Main.spriteBatch.End();
		gd.SetRenderTargets(oldRTs);
	}
	
}