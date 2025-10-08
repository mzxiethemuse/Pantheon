using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pantheon.Common.Templates;

public abstract class Minion : ModProjectile
{
    protected virtual bool BreakTiles => false;
    protected virtual bool DealContactDamage => false;

    public float Timer { get => Projectile.ai[0]; set => Projectile.ai[0] = value; }
    public int target { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = (int)value; }
    public int State { get => (int)Projectile.ai[2]; set => Projectile.ai[2] = (int)value; }

    public bool WhipTagged = false;
    public Player Owner => Main.player[Projectile.owner];

    public NPC NpcTarget => Main.npc[target];


    public override void SetStaticDefaults()
    {
        // This is necessary for right-click targeting
        ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

        Main.projPet[Projectile.type] = true; // Denotes that this projectile is a pet or minion

        ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
        ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
    }

    public override void SetDefaults()
    {
        Projectile.minion = true;
        Projectile.DamageType = DamageClass.Summon;
        Projectile.minionSlots = 1f;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        base.SetDefaults();
    }
    
    // Here you can decide if your minion breaks things like grass or pots
// (in this example false is returned, since having this on true might cause the queen bee larva to break and summon the boss accidently)
    public override bool? CanCutTiles() {
        return BreakTiles;
    }

    public override bool MinionContactDamage() {
        return DealContactDamage;
    }

    public override void AI()
    {
        if (ShouldPerformTargetingLogic()) {
            Timer++;
            if (Owner.dead || !Owner.active) return;
            Projectile.timeLeft = 2;
            if (Owner.HasMinionAttackTargetNPC)
            {
                target = Owner.MinionAttackTargetNPC;
                AttackNPC();
            }
            else
            {
                int t = -1;
                Projectile.Minion_FindTargetInRange(1000, ref t, false);
                target = t;
                if (target == -1)
                {
                    IdleUpdate();
                }
                else
                {
                    AttackNPC();
                }
            }
        }
    }

    public virtual void AttackNPC()
    {
        
    }

    public virtual void OnWhipped(Projectile whip)
    {
        
    }
 
    
    public virtual void IdleUpdate()
    {
        GoToIdlePosition();
        Projectile.friendly = false;
    }

    public virtual bool ShouldPerformTargetingLogic()
    {
        return true;
    }
    public virtual void GoToIdlePosition()
    {
        Fly(GetIdlePosition(), 0.1f, 8, 3);
    }

    public virtual Vector2 GetIdlePosition()
    {
        return IdlePositionLineup(-48f, 10, 40);
    }

    private Vector2 IdlePositionLineup(float yOffset, float xOffsetFlat, float xOffsetMult)
    {
        Vector2 idle = Owner.Center;
        idle.Y += yOffset;
        idle.X += (xOffsetFlat + Projectile.minionPos * xOffsetMult) * -Owner.direction;
        return idle;
    }

    protected void Fly(Vector2 targetPos, float acceleration, float speed, int tolerance)
    {
        if (Projectile.Center.Distance(targetPos) > tolerance)
        {
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.Center.DirectionTo(targetPos) * speed, acceleration);

        }
        else
        {
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Zero, acceleration);

        }
    }

    public void Decelerate(float acceleration)
    {
        Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Zero, acceleration);

    }
}