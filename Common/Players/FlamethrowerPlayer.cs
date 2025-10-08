using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pantheon.Common.Players;

public class FlamethrowerPlayer : ModPlayer
{
	public float FlameSizeIncrease = 0f;

	public override void ResetEffects()
	{
		FlameSizeIncrease = 0f;
		base.ResetEffects();
	}
	
	
}

public class FlamethrowerItems : GlobalItem
{
	public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.useAmmo == AmmoID.Gel;
	

}