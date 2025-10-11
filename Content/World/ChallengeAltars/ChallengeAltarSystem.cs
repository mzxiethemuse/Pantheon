using System.IO;
using Microsoft.Xna.Framework;
using Pantheon.Common;
using Pantheon.Common.Players;
using Pantheon.Common.Utils;
using Terraria;
using Terraria.DataStructures;
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
	public bool isAltarActive;
	
	public int enemiesLeft;
	private int enemiesInWave;
	private int wavesLeft;
	private int wave;
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
			} else {
				//otherwise, this is on singleplayer/(server somehow?), and we gotta set the stuff anyways
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
			if (enemiesLeft == 0)
			{
				timeUntilNextWave--;
				Main.NewText(				timeUntilNextWave);
				if (timeUntilNextWave <= 0)
				{
					wavesLeft--;
					wave++;
					Main.NewText("waves"+ wavesLeft);
					timeUntilNextWave = 300;
					if (wavesLeft < 0)
					{
						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							TileEntity.TryGet(new Point16(activeAltar), out ChallengeAltarEntity entity);
							Main.NewText("Gay");
							WorldGen.KillTile(activeAltar.X, activeAltar.Y);
							isAltarActive = false;
						}
						if (Main.netMode == NetmodeID.Server)
						{
							NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, activeAltar.X, activeAltar.Y);
							SendSyncPacket(new Point(0, 0), false);
						}
					}
					else
					{
						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							SpawnWave();
						}
					}
				}
			}
		}
		base.PostUpdateWorld();
	}

	private void SpawnWave()
	{
		enemiesLeft = 5;
		Player player = Main.player[Player.FindClosest(activeAltar.ToWorldCoordinates(), 3 * 16, 4 * 16)];
		BiomePlayer biomes = player.GetModPlayer<BiomePlayer>();
		for (int i = 0; i < enemiesLeft; i++)
		{
			if (player.ZoneForest)
			{
				int type = ChallengeAltarSpawnPools.GetRandomFromPool(ChallengeAltarSpawnPools.GetPool(biomes.Biome,
					biomes.GetPurity(), biomes.Underground, Main.hardMode));
				var npc = NPC.NewNPCDirect(new EntitySource_Misc("Challenge"), activeAltar.ToWorldCoordinates(), type);
				npc.GetGlobalNPC<AltarNPC>().partOfAltarChallenge = true;
				npc.velocity = Main.rand.NextVector2Circular(5, 5);
				npc.position += Main.rand.NextVector2FromRectangle(new Rectangle(0, 0, 16 * 3, 16 * 4));
			}

		}
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
	
}


public class AltarNPC : GlobalNPC
{
	public override bool InstancePerEntity => true;

	public bool partOfAltarChallenge;
	
	public override void OnKill(NPC npc)
	{
		if (partOfAltarChallenge) ChallengeAltarSystem.Instance.enemiesLeft--;
		base.OnKill(npc);
	}
}