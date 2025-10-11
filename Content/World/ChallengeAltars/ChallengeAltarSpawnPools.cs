using System.Collections.Generic;
using Pantheon.Common.Players;
using Terraria.ID;

namespace Pantheon.Content.World.ChallengeAltars;

public partial class ChallengeAltarSpawnPools
{
	public static List<NPCChallengeSpawn>? GetPool(VanillaBiome biome, PurityBiome purity, bool underground, bool hm)
	{
		if (underground)
		{
			
		}
		else
		{
			if (hm)
			{
				
			}
			else
			{
				return PreHMPools[(int)biome];
			}
		}
		
		return null;
	}
	
	public static Dictionary<int, List<NPCChallengeSpawn>> PreHMPools =
		new()
		{
			[(int)VanillaBiome.Forest] = [
				new(NPCID.BlueSlime, 20, 10)
			],
			[(int)VanillaBiome.Jungle] = [
				new(NPCID.BlueSlime, 20, 10)
			],
			[(int)VanillaBiome.Snow] = [
				new(NPCID.BlueSlime, 20, 10)
			],
			[(int)VanillaBiome.Desert] = [
				new(NPCID.BlueSlime, 20, 10)
			]
		}; 
}