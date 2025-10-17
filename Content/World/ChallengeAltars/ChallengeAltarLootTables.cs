using Pantheon.Content.Combat.Mage;
using Pantheon.Content.Combat.Summoner;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pantheon.Content.World.ChallengeAltars;

public static class ChallengeAltarLootTables
{
	public static short[] UniversalPreHMUniqueLoot = [
		ItemID.HermesBoots,
		ItemID.ClimbingClaws,
		ItemID.ShoeSpikes,
		ItemID.ManaCrystal,
		ItemID.Aglet,
		ItemID.PortableStool,
		ItemID.ManaCrystal,
		(short)ModContent.ItemType<ManaSink>(),
		(short)ModContent.ItemType<FocusLens>(),
	];

	public static short[] PreHMDesert =
	[
		ItemID.SandBoots,
		ItemID.SandstorminaBottle,
		ItemID.FlyingCarpet,
		(short)ModContent.ItemType<FocusLens>(),
		(short)ModContent.ItemType<Wishbone>()
	];
	
	public static short[] PreHMSnow =
	[
		ItemID.FlurryBoots,
		ItemID.FlinxFurCoat,
		ItemID.IceSkates,
		ItemID.BlizzardinaBottle,
		(short)ModContent.ItemType<ManaSink>(),
	];

	public static short[] PreHMJungle =
    [
    	ItemID.AnkletoftheWind,
    	ItemID.NaturesGift,
    	ItemID.DontHurtNatureBook,
    	ItemID.FeralClaws,
	    (short)ModContent.ItemType<PeacefulEffigy>(),
		(short)ModContent.ItemType<ManaSink>()
    ];

	public static short[] Potions =
	[
		ItemID.BattlePotion,
		ItemID.HealingPotion,
		ItemID.HunterPotion,
		ItemID.SummoningPotion,
		ItemID.InfernoPotion,
		ItemID.ThornsPotion,
		ItemID.MagicPowerPotion
	];

	public static short[] UniversalHMUniqueLoot =
	[
		ItemID.CrossNecklace,
		ItemID.PhilosophersStone,
		ItemID.FastClock,
		ItemID.TrifoldMap,
	];
}