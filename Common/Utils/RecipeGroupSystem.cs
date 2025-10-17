using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pantheon.Common.Utils;

public class RecipeGroupSystem : ModSystem
{
	public override void AddRecipeGroups()
	{
		RecipeGroup group = new RecipeGroup(() => "Pantheon:PlatinumBar", [ItemID.GoldBar, ItemID.PlatinumBar]);
		RecipeGroup.RegisterGroup("Pantheon:PlatinumBar", group);
		base.AddRecipeGroups();
	}
}