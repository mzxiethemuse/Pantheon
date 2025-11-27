using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Pantheon.Content.Combat.Melee.Spears;

public static class SpearItemHelpers
{
	public static void HoldStyle(Item item, Player player, Rectangle heldItemFrame)
	{ 
		player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, -0.75f * player.direction);
		player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Quarter, -0.9f * player.direction);
		// item.position = player.position;
		player.itemRotation = -0.25f * player.direction;
		item.scale = 1.5f;
		player.itemLocation += new Vector2(-item.width * 0.45f * player.direction, item.height);
	}

	// beyond this point you reach "holdusegraphicfuckshit gorge"
	public static void HoldItem(Item item, Player player)
	{
		item.noUseGraphic = Main.projectile.Count(projectile =>
			projectile.type == item.shoot && projectile.active) != 0;
	}
}