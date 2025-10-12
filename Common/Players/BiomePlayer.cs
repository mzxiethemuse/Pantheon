using Terraria.ModLoader;

namespace Pantheon.Common.Players;

public class BiomePlayer : ModPlayer
{
	public bool Underground => Player.ZoneDirtLayerHeight || Player.ZoneRockLayerHeight;
	private VanillaBiome? _biome;
	public VanillaBiome Biome
	{
		// this is like, compeltely superflous but basically it should make it so the player's biome isnt constantly getting if/else'd every frame
		get
		{
			if (_biome == null)
			{
				_biome = GetBiome();
			}
			return (VanillaBiome)_biome;
		}
	}

	public override void PreUpdate()
	{
		_biome = null;
		base.PreUpdate();
	}

	public VanillaBiome GetBiome()
	{
		if (Player.ZoneForest) return VanillaBiome.Forest;
		if (Player.ZoneDesert) return VanillaBiome.Desert;
		if (Player.ZoneJungle) return VanillaBiome.Jungle;
		if (Player.ZoneSnow) return VanillaBiome.Snow;

		return VanillaBiome.None;
	}

	public PurityBiome GetPurity()
	{
		if (Player.ZoneCorrupt) return PurityBiome.Corrupt;
		if (Player.ZoneCrimson) return PurityBiome.Crimson;
		return PurityBiome.Purity;
	}
}


public enum VanillaBiome
{
	None,
	Forest,
	Desert,
	Jungle,
	Snow,
}

public enum PurityBiome
{
	Purity,
	Crimson,
	Corrupt
}