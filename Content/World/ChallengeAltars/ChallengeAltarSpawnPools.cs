using System.Collections.Generic;
using System.Linq;
using Pantheon.Common.Players;
using Terraria.ID;

namespace Pantheon.Content.World.ChallengeAltars;

/// <summary>
/// If i was a normal person and not a chud this would be defined in like, json or some shit dude
/// FUCK MY STUPID CHUD PUPPY LIFE
/// </summary>
public partial class ChallengeAltarSpawnPools
{
	public static List<NPCChallengeSpawn>? GetPool(VanillaBiome biome, PurityBiome purity, bool underground, bool hell, bool hm)
	{

		int index = (int)biome + (underground ? 10 : 0);
		index = !hell ? index : 666;
		if (hm)
		{
			return (List<NPCChallengeSpawn>)PreHMPools[index].Concat(HMPools[(int)index]);
		}
		else
		{
			return PreHMPools[(int)index];
		}
	}
	
	public static Dictionary<int, List<NPCChallengeSpawn>> PreHMPools =
		new()
		{
			[0] = [
            	new(NPCID.BlueSlime, 20, 10)
            ],
			[10] = [
				new(NPCID.Skeleton, 25, -10),
				new(NPCID.BoneThrowingSkeleton2, 15, 5),
				new(NPCID.CaveBat, 35, 10),
				new(NPCID.RedSlime, 25, 10),
				new(NPCID.Crawdad2, 25, 5),
				new(NPCID.Demon, 0, 42, true),
				new(NPCID.Salamander9, 0, 40, true),
			],
			[1] = [
				new(NPCID.BlueSlime, 20, 10)
			],
			[3] = [
				new(NPCID.JungleSlime, 0, 0),
			],
			[13] = [
				new(NPCID.SpikedJungleSlime, 20, 10),
				new(NPCID.JungleBat, 10, 10),
				new(NPCID.HornetFatty, 25, 15),
			],
			[(int)VanillaBiome.Snow] = [
				new(NPCID.IceSlime, 20, -5),
				new(NPCID.ZombieEskimo, 30, 10),
			],
			[10 + (int)VanillaBiome.Snow] = [
				new(NPCID.SpikedIceSlime, 20, 5),
				new(NPCID.UndeadViking, 30, 20),
				new(NPCID.SnowFlinx, 15, 5),
				new(NPCID.IceBat, 15, 5),
			],
			[(int)VanillaBiome.Desert] = [
				new(NPCID.SandSlime, 5, 10),
				new(NPCID.Vulture, 10, 10),
			],
			[10 + (int)VanillaBiome.Desert] = [
				new(NPCID.FlyingAntlion, 10, 15),
				new(NPCID.WalkingAntlion, 10, 10),
			],
			[666] = [
				new(NPCID.Demon, 10, 15),
				new(NPCID.FireImp, 8, 17),
				new(NPCID.Hellbat, 10, 10)
			]
		}; 
	
	public static Dictionary<int, List<NPCChallengeSpawn>> HMPools =
		new()
		{
			[(int)VanillaBiome.Forest] = [
				new(NPCID.BlueSlime, 20, 10)
			],
			[(int)VanillaBiome.Jungle] = [
				new(NPCID.BlueSlime, 20, 10)
			],
			[(int)VanillaBiome.Snow] = [
				new(NPCID.ArmoredViking, 30, 10),
				new(NPCID.IceElemental, 30, 20),
				new(NPCID.IcyMerman, 15, 5),
			],
			[(int)VanillaBiome.Desert] = [
				new(NPCID.Mummy, 35, 10),
				new(NPCID.DesertGhoul, 25, 10),

				new(NPCID.DesertDjinn, 15, 30)

			],
			[666] = [
				new(NPCID.Demon, 10, 10),
                new(NPCID.FireImp, 25, -10),
                new(NPCID.Lavabat, 10, 14),
				new(NPCID.RedDevil, 4, 8)
			]
		}; 
}