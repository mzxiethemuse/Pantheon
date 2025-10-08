using Pantheon.Common.Templates;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pantheon.Common.Utils;

public class GlobalWhip : GlobalProjectile
{
    public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => ProjectileID.Sets.IsAWhip[entity.type];

    public override void AI(Projectile projectile)
    {
        if (projectile.owner != Main.myPlayer) return;
        foreach (var proj in Main.projectile)
        {
            if (proj.active && proj.owner == projectile.owner && proj.whoAmI != projectile.whoAmI && proj.minion && !ProjectileID.Sets.IsAWhip[proj.type])
            {
                if (proj.Colliding(proj.Hitbox, projectile.Hitbox))
                {
                    if (proj.ModProjectile is SummonerTotem totem)
                    {
                        totem.PreWhipped(projectile);
                    } 
                }
            }
        }
        
        base.AI(projectile);
    }
}