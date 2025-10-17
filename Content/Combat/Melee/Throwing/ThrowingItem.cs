using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pantheon.Content.Combat.Melee.Throwing;

public class ThrowingItem : GlobalItem
{

    public static readonly int[] throwingWeapons =
    {
        ItemID.Snowball,
        ItemID.Grenade,
        ItemID.Bomb,
        ItemID.PaperAirplaneA,
        ItemID.PaperAirplaneB,
        ItemID.Javelin,
        ItemID.BoneJavelin,
        ItemID.Shuriken,
        ItemID.ThrowingKnife,
        ItemID.ShadowFlameKnife,
        ItemID.MagicDagger,
        ItemID.SpikyBall,
        ItemID.PoisonedKnife,
        ItemID.RottenEgg,
        ItemID.FrostDaggerfish,
        ItemID.WoodenBoomerang,
        ItemID.EnchantedBoomerang,
        ItemID.IceBoomerang,
        ItemID.Trimarang,
        ItemID.Beenade,
        ItemID.MolotovCocktail,
        ItemID.StickyGrenade,
        ItemID.BouncyGrenade,
        ItemID.PartyGirlGrenade,
        ItemID.BoneDagger,
        ItemID.Bone
    };

    // hardcodiful AF
    public static readonly int[] throwingProjectiles =
    {
        ProjectileID.SnowBallFriendly,
        ProjectileID.Grenade,
        ProjectileID.Bomb,
        ProjectileID.PaperAirplaneA,
        ProjectileID.PaperAirplaneB,
        ProjectileID.JavelinFriendly,
        ProjectileID.BoneJavelin,
        ProjectileID.Shuriken,
        ProjectileID.ThrowingKnife,
        ProjectileID.ShadowFlameKnife,
        ProjectileID.MagicDagger,
        ProjectileID.SpikyBall,
        ProjectileID.PoisonedKnife,
        ProjectileID.RottenEgg,
        ProjectileID.FrostDaggerfish,
        ProjectileID.WoodenBoomerang,
        ProjectileID.EnchantedBoomerang,
        ProjectileID.IceBoomerang,
        ProjectileID.Trimarang,
        ProjectileID.Beenade,
        ProjectileID.MolotovCocktail,
        ProjectileID.StickyGrenade,
        ProjectileID.BouncyGrenade,
        ProjectileID.PartyGirlGrenade,
        ProjectileID.BoneDagger,
        ProjectileID.Bone
    };
    
    public override void SetDefaults(Item entity)
    {
        if (throwingWeapons.Contains(entity.type))
        {
            entity.DamageType = DamageClass.Throwing;
            
        }
    }

    public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
    {
        if (item.DamageType == DamageClass.Throwing)
        {
            // terrible code but Fuck you!damage = new StatModifier(item.type, 0f); thank you autocompletedamage = new StatModifier(item.type, 0f);
            damage.Flat += player.GetDamage(DamageClass.Ranged).Flat * 0.7f;
            damage.Flat += player.GetDamage(DamageClass.Melee).Flat * 0.7f;
            damage.Base += player.GetDamage(DamageClass.Ranged).Base * 0.7f;
            damage.Base += player.GetDamage(DamageClass.Melee).Base * 0.7f;
            damage *= MathF.Max(player.GetDamage(DamageClass.Melee).Multiplicative * 0.7f, 1);
            damage *= MathF.Max(player.GetDamage(DamageClass.Melee).Multiplicative * 0.7f, 1);
            damage += (player.GetDamage(DamageClass.Melee).Additive - 1) * 0.7f;
            damage += (player.GetDamage(DamageClass.Ranged).Additive - 1) * 0.7f;
            damage += 0.1f;
        }
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (throwingWeapons.Contains(item.type))
        {
            tooltips.Add(new TooltipLine(Mod, "HarmonyMod:ThrowingTooltippity", "Partially affected by both melee and ranged bonuses"));
        }
    }
}