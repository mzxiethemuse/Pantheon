using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pantheon.Common;

//Not To Be Confused With Jane (of remover fame)
public class ItemRemover : ModSystem
{
	public static int[] ItemsToRemove =
	[
		ItemID.ManaFlower
	];

	public override void PostAddRecipes()
	{
		for (int i = 0; i < Recipe.numRecipes; i++) {
			Recipe recipe = Main.recipe[i];

			for (int j = 0; j < ItemsToRemove.Length-1; j++)
			{
				if (recipe.HasResult(ItemsToRemove[i])) recipe.DisableRecipe();

			}
		}
		base.PostAddRecipes();
	}
}