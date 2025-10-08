using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pantheon.Common.Utils;

public abstract class ComplexNPC : ModNPC
{
    protected float Acceleration = 0.1f;
    protected float JumpHeight = 7f;
    protected int HitDustAmount = 10;
    protected int HitDustType = DustID.Blood;

    public virtual long CoinValue => 0;

    private int GuardShaderTime = 0;
    
    /// <summary>
    /// Depreciated
    /// </summary>
    protected bool Guarded = false;
    
    
    
    /// <summary>
    /// if the NPC should fall through platforms. this is *reset* to false in PreAI each update.
    /// </summary>
    protected bool FallThroughPlatforms = false;
    protected float State
    {
        get => NPC.ai[0]; set => NPC.ai[0] = value; }
    protected float Timer
    {
        get => NPC.ai[1]; set => NPC.ai[1] = value; }
    
    protected float AI2
    {
        get => NPC.ai[2]; set => NPC.ai[2] = value; }
    protected float AI3
    {
        get => NPC.ai[3]; set => NPC.ai[3] = value; }

    protected Vector2 GoHere;

    public override bool PreAI()
    {
        // if (Guarded)
        // {
        //     GuardShaderTime++;
        // }
        // else
        // {
        //     GuardShaderTime = 0;
        // }
        // Guarded = false;

        FallThroughPlatforms = false;
        return base.PreAI();
    }

    public void Walk(Vector2 desiredPos, float speed, bool tryReachY = false, float xTargetRange = 10)
    {

        // if (tryReachY) FallThroughPlatforms = true;

        // check if npc has reached the target range
        if ((desiredPos * new Vector2(1, 0)).Distance(NPC.Center * new Vector2(1, 0)) < xTargetRange)
        {

            if (tryReachY)
            {
                if (desiredPos.Y < NPC.Center.Y)
                {
                    TryJump(JumpHeight); //, NPC.position.X > desiredPos.X ? -speed : speed);
                }
                else
                {
                    if (desiredPos.Y >= NPC.Center.Y + NPC.height / 2) FallThroughPlatforms = true;

                }
                
            } else NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, 0,
                Acceleration);
        }
        else
        {
            // if the npc is on the ground, move
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.position.X > desiredPos.X ? -speed : speed,
                Acceleration);
            

            // if against a wall
            if (NPC.collideX)
            {
                TryJump(JumpHeight);
                Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed,
                    ref NPC.gfxOffY);
            }

            // fall thru platforms
            if (tryReachY)
            {
                if (desiredPos.Y >= NPC.Center.Y + NPC.height / 2)
                {
                    FallThroughPlatforms = true;
                }
            }
        }
        
    }


    public void Decelerate() => NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, 0,
        Acceleration);
    
    public void Decelerate(float delta) => NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, 0,
        delta);

    public float DistanceOnX(Vector2 a, Vector2 b)
    {
        return MathF.Abs(Diff(a.X, b.X));
    }

    public float Diff(float a, float b)
    {
        return MathF.Abs(b - a);
    }

    public override bool? CanFallThroughPlatforms() => FallThroughPlatforms;

    public void TryJump(float strength, float speed = 0f)
    {
        if (OnGround())
        {
            NPC.velocity.X += speed;
            
            NPC.velocity.Y = -strength;
        }
    }

    public bool OnGround()
    {
        return NPC.collideY && NPC.velocity.Y == 0 && NPC.oldVelocity.Y >= 0;
    }

    public Player GetTarget()
    {
        return Main.player[NPC.target];
    }

    // protected void OnHurt(NPC.HitInfo hitInfo)
    // {
    //     for (int i = 0; i < HitDustAmount; i++)
    //     {
    //         Dust.NewDust(NPC.position, NPC.width, NPC.height, HitDustType);
    //     }
    // }

    // public virtual bool OnParried()
    // {
    //     for (int i = 0; i < HitDustAmount; i++)
    //     {
    //         Dust.NewDust(NPC.position, NPC.width, NPC.height, HitDustType);
    //     }
    //
    //     var ret = Guarded;
    //     Guarded = false;
    //     Main.NewText(ret);
    //     return ret;
    // }

    
    public override void HitEffect(NPC.HitInfo hit)
    {
        // if (Guarded && hit.DamageType != DamageClass.Magic && hit.DamageType != DamageClass.Summon) CombatText.NewText(new Rectangle((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height), Color.LightGoldenrodYellow, "Blocked!");
        for (int i = 0; i < HitDustAmount; i++)
        {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, HitDustType);
        }

        
    }

    public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
    {
        // if (Guarded && modifiers.DamageType != DamageClass.Magic && modifiers.DamageType != DamageClass.Summon)
        // {
        //     modifiers.FinalDamage *= 0;
        //     modifiers.HideCombatText();
        // }
    }
    
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        // if (Guarded)
        // {
        //     Main.spriteBatch.End();
        //     GameShaders.Misc["HarmonyMod:GuardShader"].UseOpacity(GuardShaderTime);
        //     Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, default,
        //         Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        //     GameShaders.Misc["HarmonyMod:GuardShader"].Apply();
        // }

        return true;
    }

    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        // if (Guarded)
        // {
        //     Main.spriteBatch.End();
        //
        //     Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, default,
        //         Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        // }
    }

    public bool StateJustStarted() => Timer == 1;

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Coins(CoinValue, true));
        base.ModifyNPCLoot(npcLoot);
    }
}


public static class ComplexNPCHelper {
    public static ComplexNPC GetComplexNPC(this NPC npc)
    {
        return (ComplexNPC)npc.ModNPC;
    }
}