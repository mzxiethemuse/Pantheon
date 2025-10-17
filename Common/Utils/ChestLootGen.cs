using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pantheon.Common.Utils;

// W TAKEN FROM EXAMPLE MOD!!
// This class showcases adding additional items to vanilla chests.
// This example simply adds additional items. More complex logic would likely be required for other scenarios.
// If this code is confusing, please learn about "for loops" and the "continue" and "break" keywords: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/jump-statements
public class ChestLootGen : ModSystem
{
	public static void AddLoot(ChestLoot loot)
	{
		ModContent.GetInstance<ChestLootGen>().lootToAdd.Add(loot);
	}
	
	public List<ChestLoot> lootToAdd = new List<ChestLoot>();
	
	public enum ChestType
	{
		Wood = 0,
		Gold = 1,
		GoldLocked = 2,
		Shadow = 3,
		ShadowLocked = 4,
		Barrel = 5,
		JungleShrine = 10,
		Frozen = 11,
		Mossy = 12,
		Sky = 13,
		Spider = 14,
		Lihzahrd = 15,
		Sandstone = 110
		
	}
	// We use PostWorldGen for this because we want to ensure that all chests have been placed before adding items.
	public override void PostWorldGen() {
		foreach (var loot in lootToAdd)
		{
			AddItemsToChests([loot.Type], loot.ChestType, loot.Count, loot.Chance);
		}

	}


	public void AddItemsToChests(int[] ids, ChestType chestType, int count, int chance)
	{
		// I LOVE STEALING FROM EXAMPLE MOD!!
		// I LOVE STEALING FROM EXAMPLE MOD!!
		// I LOVE STEALING FROM EXAMPLE MOD!!
		// I LOVE STEALING FROM EXAMPLE MOD!!
		// I LOVE STEALING FROM EXAMPLE MOD!!
		// I LOVE STEALING FROM EXAMPLE MOD!!
		// I LOVE STEALING FROM EXAMPLE MOD!!// I LOVE STEALING FROM EXAMPLE MOD!!
		// I LOVE STEALING FROM EXAMPLE MOD!!
		// Place some additional items in Frozen Chests:
		// These are the 3 new items we will place.
			
		// This variable will help cycle through the items so that different Frozen Chests get different items
		int choice = 0;
		// Rather than place items in each chest, we'll place up to 6 items (2 of each). 
		int itemsPlaced = 0;
		int maxItems = count;
		// Loop over all the chests
		for (int chestIndex = 0; chestIndex < Main.maxChests; chestIndex++) {
			Chest chest = Main.chest[chestIndex];
			if (chest == null) {
				continue;
			}
			Tile chestTile = Main.tile[chest.x, chest.y];
			// me
			int tile = (int)chestType > 100 ? TileID.Containers2 : TileID.Containers;
			int type = (int)chestType > 100 ? (int)chestType - 100 : (int)chestType; 
			
			// We need to check if the current chest is the Frozen Chest. We need to check that it exists and has the TileType and TileFrameX values corresponding to the Frozen Chest.
			// If you look at the sprite for Chests by extracting Tiles_21.xnb, you'll see that the 12th chest is the Frozen Chest. Since we are counting from 0, this is where 11 comes from. 36 comes from the width of each tile including padding. An alternate approach is to check the wiki and looking for the "Internal Tile ID" section in the infobox: https://terraria.wiki.gg/wiki/Frozen_Chest
			if (chestTile.TileType == tile && chestTile.TileFrameX == type * 36) {
				// We have found a Frozen Chest
				// If we don't want to add one of the items to every Frozen Chest, we can randomly skip this chest with a 33% chance.
				if (chance == 1 || WorldGen.genRand.NextBool(chance))
					continue;
				// Next we need to find the first empty slot for our item
				for (int inventoryIndex = 0; inventoryIndex < Chest.maxItems; inventoryIndex++) {
					if (chest.item[inventoryIndex].type == ItemID.None) {
						// Place the item
						chest.item[inventoryIndex].SetDefaults(ids[choice]);
						// Decide on the next item that will be placed.
						choice = (choice + 1) % ids.Length;
						// Alternate approach: Random instead of cyclical: chest.item[inventoryIndex].SetDefaults(WorldGen.genRand.Next(itemsToPlaceInFrozenChests));
						itemsPlaced++;
						break;
					}
				}
			}
			// Once we've placed as many items as we wanted, break out of the loop
			if (itemsPlaced >= maxItems) {
				break;
			}
		}
	}
}

public struct ChestLoot(ChestLootGen.ChestType chestType, int type, int count, int chance)
{
	public ChestLootGen.ChestType ChestType = chestType;
	public int Type = type;
	public int Count = count;
	public int Chance = chance;
}