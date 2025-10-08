using Microsoft.Xna.Framework;
using Pantheon.Assets;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Pantheon.Content.Projectiles;

public class Hitbox : ModProjectile
{
    public string name = "Unset hitbox name!";
    public int npcOwner = -1;
    public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs(name);
    public override string Texture => AssetDirectory.Placeholder + "GenericItem";

    public override void SetDefaults() => Projectile.penetrate = -1;

    public override bool PreDraw(ref Color lightColor) => false;

    public static void SpawnHitbox(IEntitySource source, Vector2 center, int halfWidth, int halfHeight, int damage, int time, int sourceIndex = -1, bool friendly = false,
        bool hostile = false)
    {
        if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            var projectile = Projectile.NewProjectileDirect(source, center, Vector2.Zero,
                ModContent.ProjectileType<Hitbox>(), damage, 2f);
            projectile.friendly = friendly;
            projectile.hostile = hostile;
            projectile.timeLeft = time;
            projectile.Size = new Vector2(halfWidth * 2, halfHeight * 2);
            projectile.Center = center;
            // projectile.Name = name;
            // ((Hitbox)projectile.ModProjectile).name = name;
            ((Hitbox)projectile.ModProjectile).npcOwner = sourceIndex;

        }
    }

    public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
    {
        modifiers.DamageSource.SourceNPCIndex = npcOwner;

        modifiers.DamageSource.SourceProjectileType = -1;
        base.ModifyHitPlayer(target, ref modifiers);
    }
}

public class HitboxDisplay : GlobalProjectile
{
    public override bool InstancePerEntity => true;

    public override void ModifyHitPlayer(Projectile projectile, Player target, ref Player.HurtModifiers modifiers)
    {
        base.ModifyHitPlayer(projectile, target, ref modifiers);
    }
}