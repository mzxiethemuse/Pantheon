using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pantheon.Common;

public class HookChanges : GlobalProjectile
{
	public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) =>
		entity.aiStyle == ProjAIStyleID.Hook;

	public override void SetDefaults(Projectile entity)
	{
		entity.extraUpdates = 3;
		base.SetDefaults(entity);
	}

	public override void OnSpawn(Projectile projectile, IEntitySource source)
	{
		// projectile.velocity *= 25f;
		base.OnSpawn(projectile, source);
	}
	
	
	
}