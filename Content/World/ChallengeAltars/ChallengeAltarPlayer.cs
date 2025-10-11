using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pantheon.Content.World.ChallengeAltars;

public class ChallengeAltarPlayer : ModPlayer
{
	public Color GetAltarFirePrimaryColor()
	{
		if (Player.ZoneForest) return Color.ForestGreen * 4f;
		if (Player.ZoneUndergroundDesert) return Color.DarkGoldenrod;
		if (Player.ZoneNormalUnderground) return Color.SaddleBrown; 
		if (Player.ZoneCorrupt) return Color.MediumOrchid * 0.5f;
		if (Player.ZoneCrimson) return Color.DarkRed;
		if (Player.ZoneNormalCaverns) return Color.CadetBlue * 0.25f; 

		if (Player.ZoneGlowshroom) return Color.SteelBlue;
		if (Player.ZoneJungle) return Color.DeepPink * 0.25f;
		return Colors.CurrentLiquidColor * 0.5f;;
	}
	
	public Color GetAltarFireSecondaryColor()
	{
		if (Player.ZoneForest) return Color.DarkGreen * 3f;
		if (Player.ZoneUndergroundDesert) return Color.Khaki * 0.5f;
		if (Player.ZoneNormalUnderground) return Color.SandyBrown * 0.5f;
		if (Player.ZoneNormalCaverns) return Color.DarkSlateBlue; 

		if (Player.ZoneJungle) return Color.SeaGreen;
		if (Player.ZoneCorrupt) return Color.MediumPurple * 0.5f;

		return Colors.CurrentLiquidColor * 0.5f;
	}
	
}