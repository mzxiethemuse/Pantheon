using Pantheon.Content.Combat.Summoner;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pantheon.Common;

public class DryadTrades : GlobalNPC
{
	public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.Dryad;

	public override void ModifyShop(NPCShop shop)
	{
		shop.Add<DryadPlushItem>(Condition.DownedEarlygameBoss);

		base.ModifyShop(shop);
	}
}

public class WitchDoctorTrades : GlobalNPC
{
	public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.WitchDoctor;

	public override void ModifyShop(NPCShop shop)
	{
		shop.Add<PeacefulEffigy>(Condition.DownedEarlygameBoss);

		base.ModifyShop(shop);
	}
}