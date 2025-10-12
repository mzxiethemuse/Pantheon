using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pantheon.Assets;
using Pantheon.Common;
using Pantheon.Common.Players;
using Pantheon.Common.Utils;
using Pantheon.Content.Dusts;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pantheon.Content.World.ChallengeAltars;

public class ChallengeAltarSystem : ModSystem
{
	/*
	 * When a altar is right clicked:
	 * we tell the ModSystem "hey this is the active altar, start keeping track"
	 * we send a packet that tells the server to start keeping track of the waves + spawn npcs
	 * the server keeps track of the globalnpcs and waves
	 * clients dont need to worry their pretty little heads off as long as they know that the altar is active
	 */
	private bool sendDebugMsgs = true;
	
	public Point activeAltar;
	public short[] activeAltarLoot;
	public Vector2 altarPositionWorld => activeAltar.ToWorldCoordinates(16 * 1.5f, 16 * 2);
	public bool isAltarActive;
	
	public int enemiesLeft;
	private int enemiesInWave;
	private int wavesLeft;
	private int wave;
	private int timeToPreventStuck;
	private int timeUntilNextWave;
	
	public override void PostUpdateEverything()
	{
		base.PostUpdateEverything();
	}

	// Called on client, needs syncing
	public void SetActiveAltar(Point pos)
	{
		if (!isAltarActive)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				// if this comes from a multiplayer client, we need to tell the server to start keeping track
				SendSyncPacket(pos, true);
				activeAltar = pos;
				isAltarActive = true;
				TileEntity.TryGet(new Point16(pos), out ChallengeAltarEntity entity);
				activeAltarLoot = entity.loot;

			} else {
				//otherwise, this is on singleplayer/(server somehow?), and we gotta set the stuff anyways
								TileEntity.TryGet(new Point16(pos), out ChallengeAltarEntity entity);
				activeAltarLoot = entity.loot;
				activeAltar = pos;
				isAltarActive = true;
				SetupAltar();
			}
		}
	}

	public static ChallengeAltarSystem Instance => ModContent.GetInstance<ChallengeAltarSystem>();

	private void SendSyncPacket(Point pos, bool active)
	{
		ModPacket packet = Mod.GetPacket();
		packet.Write((int)PacketTypes.ChallengeAltarSync); 
		packet.Write(active);
		packet.Write(pos.X);
		packet.Write(pos.Y);
		packet.Send();
	}

	public override void PostUpdateWorld()
	{
		if (isAltarActive)
		{
			timeToPreventStuck++;
			if (timeToPreventStuck > (60 * 5))
			{
				if (Main.npc.Count(npc => npc.active && npc.GetGlobalNPC<AltarNPC>().partOfAltarChallenge) == 0)
				{
					enemiesLeft = 0;
					timeToPreventStuck = 0;
				}
			}

			if (enemiesLeft == 0)
			{
				if (timeUntilNextWave == 300)
				{
					// anything that happens on wave start go here
					SoundEngine.PlaySound(Sounds.ChallengeAltarSpawnWave, altarPositionWorld);
				}
				timeToPreventStuck = 0; 
				if (Main.netMode != NetmodeID.Server && Main.timeForVisualEffects % 60 > 0.01f)
				{
					if (wavesLeft == 0)
					{
						for (int i = 0; i < 3; i++)
						{
							Vector2 offset = Main.rand.NextVector2Circular(75, 75);
							Dust.NewDustPerfect(altarPositionWorld + offset,
								DustID.TintableDustLighted, offset.DirectionTo(Vector2.Zero),0, Main.LocalPlayer.GetModPlayer<ChallengeAltarPlayer>().GetAltarFirePrimaryColor(), 0.5f);
						}
					}
					else
					{
						for (int i = 0; i < (GetSpawnCount()); i++)
                        {
                        	Vector2 offset = (new Vector2(-15 * GetSpawnCount(), 0)).RotatedBy(MathF.PI * (i / (float)(GetSpawnCount() - 1)));
                        	Dust.NewDustPerfect(activeAltar.ToWorldCoordinates(16 * 1.5f, 16 * 2) + offset,
                        		DustID.TintableDustLighted, Main.rand.NextVector2Circular(0.25f, 0.25f), 200, Main.LocalPlayer.GetModPlayer<ChallengeAltarPlayer>().GetAltarFirePrimaryColor());
                        }
					}

				}
				
				timeUntilNextWave--;
				if (timeUntilNextWave <= 0)
				{
					wavesLeft--;
					wave++;
					timeUntilNextWave = 300;
					if (wavesLeft < 0)
					{
						SoundEngine.PlaySound(Sounds.ChallengeAltarFinish, altarPositionWorld);

						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							TileEntity.TryGet(new Point16(activeAltar), out ChallengeAltarEntity entity);
							WorldGen.KillTile(activeAltar.X, activeAltar.Y);
							isAltarActive = false;
						}
						if (Main.netMode == NetmodeID.Server)
						{
							NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, activeAltar.X, activeAltar.Y);
							SendSyncPacket(new Point(0, 0), false);
						}

						if (Main.netMode != NetmodeID.Server)
						{
							Burst.SpawnBurstDust(ModContent.DustType<Burst>(), altarPositionWorld, 0.75f, Main.LocalPlayer.GetModPlayer<ChallengeAltarPlayer>().GetAltarFirePrimaryColor(), 8f, 0.9f);
							Burst.SpawnBurstDust(ModContent.DustType<Burst>(), altarPositionWorld, 0.75f, Main.LocalPlayer.GetModPlayer<ChallengeAltarPlayer>().GetAltarFireSecondaryColor(), 8f, 0.9f);
						}
					}
					else
					{
						
						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							SpawnWave();
						}
						if (Main.netMode != NetmodeID.Server) {
							Burst.SpawnBurstDust(ModContent.DustType<Burst>(), altarPositionWorld, 0.85f, Main.LocalPlayer.GetModPlayer<ChallengeAltarPlayer>().GetAltarFirePrimaryColor(), 8f, 0.9f);

							for (int i = 0; i < 64; i++)
							{
								Dust.NewDust(activeAltar.ToWorldCoordinates(0, 0), 16 * 3, 16 * 4,
									DustID.TintableDustLighted,
									0, 0, 100, Main.LocalPlayer.GetModPlayer<ChallengeAltarPlayer>().GetAltarFirePrimaryColor());
							}
						}
					}
				}
			}
			else
			{
				if (enemiesLeft == 1)
				{
					if (Main.netMode != NetmodeID.Server && Main.timeForVisualEffects % 90 > 0.01f)
					{
						// NPC npc = Main.npc.First(npc =>
						// 	npc.active && npc.GetGlobalNPC<AltarNPC>().partOfAltarChallenge);
						// float distance = altarPositionWorld.Distance(npc.Center);
						// Vector2 pos = altarPositionWorld;
						// for (int i = 0; i < distance / 32; i++)
						// {
						// 	pos += altarPositionWorld.DirectionTo(npc.Center) * 16;
						// 	Dust.NewDustPerfect(pos,
						// 		DustID.TintableDustLighted, Main.rand.NextVector2Circular(0.25f, 0.25f), 0, Main.LocalPlayer.GetModPlayer<ChallengeAltarPlayer>().GetAltarFirePrimaryColor(), 0.35f);
						// }
					}
				}
			}
		}
		base.PostUpdateWorld();
	}

	private void SpawnWave()
	{
		
		
		enemiesLeft = GetSpawnCount() - 1;
		Player player = Main.player[Player.FindClosest(activeAltar.ToWorldCoordinates(), 3 * 16, 4 * 16)];
		BiomePlayer biomes = player.GetModPlayer<BiomePlayer>();
		List<short> blacklist = new();
		for (int i = 0; i < enemiesLeft; i++)
		{
			Vector2 offset = (new Vector2(-75, 0)).RotatedBy(MathF.PI * (i / ((float)enemiesLeft - 1)));
			int type = ChallengeAltarSpawnPools.GetRandomFromPool(ChallengeAltarSpawnPools.GetPool(biomes.Biome,
				biomes.GetPurity(), biomes.Underground, Main.hardMode),  ref blacklist);
			var npc = NPC.NewNPCDirect(new EntitySource_Misc("Challenge"), activeAltar.ToWorldCoordinates(24, 32), type);
			npc.GetGlobalNPC<AltarNPC>().partOfAltarChallenge = true;
			npc.position += offset;
		}

		//safeguard
		enemiesLeft = Main.npc.Count(npc => npc.active && npc.GetGlobalNPC<AltarNPC>().partOfAltarChallenge);
	}
	

	/// <summary>
	/// this method assumes that you have already used the BinaryReader to ensure that the first value is equal to PacketTypes.ChallengeAltarSync
	/// </summary>
	public void HandleSyncPacket(BinaryReader reader, int who)
	{
		// theoretically this should only happen on the server
		var active = reader.ReadBoolean();
		Point pos = new Point(reader.ReadInt32(), reader.ReadInt32());
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			if (active)
			{
				if (sendDebugMsgs) Main.NewText("Yo've been synced");

				isAltarActive = true;
				activeAltar = pos;
			}
			else
			{
				if (sendDebugMsgs) Main.NewText("Yo've been synced");
				isAltarActive = false;
			}
		}
		else
		{
			// theoretically this should always be true
			if (active)
			{
				isAltarActive = true;
				activeAltar = pos;
				SetupAltar();
			}
		}
	}

	private void SetupAltar()
	{
		wave = 0;
		wavesLeft = 3;
		enemiesLeft = 0;
		timeUntilNextWave = 300;
	}

	public int GetSpawnCount()
	{
		return 4 + (int)MathF.Floor(wave * (0.75f + (Main.hardMode ? 0.5f : 0)));
	}
	
}


public class AltarNPC : GlobalNPC
{
	public override bool InstancePerEntity => true;

	public bool partOfAltarChallenge;

	public override void Load()
	{
		if (!Main.dedServ)
		{
			On_Main.DrawNPCs += On_MainOnDrawNPCs;
		}
	}

	private void On_MainOnDrawNPCs(On_Main.orig_DrawNPCs orig, Main self, bool behindtiles)
	{
		orig.Invoke(self, behindtiles);
		Shaders.AltarNPCOverlay.Value.Parameters["uTime"].SetValue((float)(Main.timeForVisualEffects * 0.05f));
		Shaders.AltarNPCOverlay.Value.Parameters["uColor"].SetValue(Main.LocalPlayer.GetModPlayer<ChallengeAltarPlayer>().GetAltarFirePrimaryColor().ToVector4());
		Main.graphics.GraphicsDevice.Textures[1] = Textures.Noise.Value;
		Main.spriteBatch.End();
		if (!behindtiles)
		{
			foreach (var npc in Main.npc.Where(npc => npc.active && npc.GetGlobalNPC<AltarNPC>().partOfAltarChallenge))
			{
				var t = TextureAssets.Npc[npc.type].Value;
				Shaders.AltarNPCOverlay.Value.Parameters["resolution"].SetValue(t.Size());
				Shaders.AltarNPCOverlay.Value.Parameters["sourceRect"].SetValue(new Vector4(npc.frame.X, npc.frame.Y, npc.frame.Width, npc.frame.Height));

				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, Main.Rasterizer, Shaders.AltarNPCOverlay.Value, Main.GameViewMatrix.TransformationMatrix);


				Main.spriteBatch.Draw(
					TextureAssets.Npc[npc.type].Value,
					npc.Center - Main.screenPosition + new Vector2(0, npc.gfxOffY),
					npc.frame,
					Color.Blue,
					npc.rotation,
					npc.frame.Size() / 2,
					npc.scale,
					npc.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
					0f
					);
					Main.spriteBatch.End();
			}
		}
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

	}

	public override void OnKill(NPC npc)
	{
		if (partOfAltarChallenge) ChallengeAltarSystem.Instance.enemiesLeft--;
		base.OnKill(npc);
	}

	public override Color? GetAlpha(NPC npc, Color drawColor)
	{
		if (partOfAltarChallenge)
		{
			npc.SpawnedFromStatue = true;
			if (npc.HasPlayerTarget)
			{
				Player target = Main.player[npc.target];
				return Color.Lerp((target.GetModPlayer<ChallengeAltarPlayer>().GetAltarFirePrimaryColor() * 2f) with { A = drawColor.A}, drawColor, 0.9f);
			}
			return drawColor;
		}
		return base.GetAlpha(npc, drawColor);
	}
}