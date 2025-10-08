using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Pantheon.Assets;
using Pantheon.Content.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pantheon.Common.Players;

public class SoaringInsigniaReworkPlayer : ModPlayer
{
	public bool soaringInsignia;

	public override void PostUpdateEquips()
	{
		soaringInsignia = Player.empressBrooch;   
		Player.empressBrooch = false;
		base.PostUpdateEquips();
	}

	public override void OnConsumeMana(Item item, int manaConsumed)
	{
		if (soaringInsignia && Player.ManaPlayer().ManaFromPast5Seconds > 100 && !Player.ItemCooldownPlayer().IsItemOnCooldown(ItemID.EmpressFlightBooster))
		{
			// Burst.SpawnBurst(Textures.VFXExplosion, Player.Center, (Color.Pink * 2f) with {A = 60}, 100f, 30, true);
			for (int i = 0; i < 80; i++)
			{
				Dust.NewDust(Player.position, Player.width, Player.height, DustID.LastPrism);
			}
			Player.ItemCooldownPlayer().SetCooldown(ItemID.EmpressFlightBooster, 60 * 8);
			Player.RefreshExtraJumps();
		}
		base.OnConsumeMana(item, manaConsumed);
	}
}

public class SoaringInsignia : GlobalItem
{
	public override bool AppliesToEntity(Item entity, bool lateInstantiation) =>
		entity.type == ItemID.EmpressFlightBooster;

	public override void SetDefaults(Item entity)
	{
		entity.SetNameOverride("Prismatic Idol");
		base.SetDefaults(entity);
	}

	public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
	{
		tooltips.Add(new TooltipLine(Mod, "Pantheon:SoaringInsigniaJump", "Using 100 mana in 5 seconds refreshes extra jumps"));

		base.ModifyTooltips(item, tooltips);
	}
}