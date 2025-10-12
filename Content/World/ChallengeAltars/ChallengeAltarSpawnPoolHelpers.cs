using System.Collections.Generic;
using System.Linq;
using Pantheon.Common.Utils;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Pantheon.Content.World.ChallengeAltars;

public partial class ChallengeAltarSpawnPools : ILoadable
{
	public static ChallengeAltarSpawnPools Instance => ModContent.GetInstance<ChallengeAltarSpawnPools>();
	
	public void Load(Mod mod)
	{
	}

	public void Unload()
	{
		
	}

	public static int GetRandomFromPool(List<NPCChallengeSpawn> spawns, ref List<short> blacklist)
	{
		int type = -1;
		int index = -1;
		var types = new int[spawns.Count];
		var weights = new int[spawns.Count];
		var blacklisted = new bool[spawns.Count];

		for (int i = 0; i < types.Length; i++)
		{
			types[i] = spawns[i].type;
			weights[i] = spawns[i].type;
			blacklisted[i] = spawns[i].limited;
		}
		while (type == -1 || blacklist.Contains((short)type))
		{
			index = Utilities.GetWeightedRandom(weights);
			type = types[index];
		}

		if (blacklisted[index])
		{
			blacklist.Add((short)types[index]);
		}
		return type;
	}

	public static int[] GetTypes(List<NPCChallengeSpawn> spawns)
	{
		var list = new int[spawns.Count];
		for (int i = 0; i < list.Length; i++)
		{
			list[i] = spawns[i].type;
		}
		return list;
	}
	
	public static int[] GetWeights(List<NPCChallengeSpawn> spawns)
	{
		var list = new int[spawns.Count];
		for (int i = 0; i < list.Length; i++)
		{
			list[i] = spawns[i].weight;
		}
		return list;
	}
	
	public static int[] GetWeightWithIncreases(List<NPCChallengeSpawn> spawns, int wave)
	{
		var list = new int[spawns.Count];
		for (int i = 0; i < list.Length; i++)
		{
			list[i] = spawns[i].weight + spawns[i].weightIncreasePerWave * wave;
		}
		return list;
	}

}

public struct NPCChallengeSpawn(int type, int weight, int weightIncreasePerWave, bool limited = false)
{
	public int type = type;
	public int weight = weight;
	public int weightIncreasePerWave = weightIncreasePerWave;
	public bool limited = limited;
}