using System;
using Terraria;
using Terraria.ModLoader;

namespace Pantheon.Common.Players;

// You ain't nothing but a broke Boy Boy Boy Boy
public class MovementPlayer : ModPlayer
{
	private bool wasGrappling = false;
	private bool wasSliding = false;
	public override void ResetEffects()
	{
		Player.extraAccessory = true;
		Player.extraAccessorySlots = Math.Max(2, Player.extraAccessorySlots);
		base.ResetEffects();
	}

	public override void PostUpdateEquips()
	{
		// Player.empressBrooch = false;
		Player.wingTimeMax = Math.Max(30, Player.wingTimeMax / 4);
		
		// if (Player.controlJump && Player.grappling[0])
		base.PostUpdateEquips();
	}

	public override void OnExtraJumpStarted(ExtraJump jump, ref bool playSound)
	{
		if (Player.controlLeft || Player.controlRight)
		{
			Player.velocity.X = MathF.Abs(Player.velocity.X) * 1.25f * (Player.controlRight ? 1 : -1);
		}
		base.OnExtraJumpStarted(jump, ref playSound);
	}

	public override void PreUpdateMovement()
	{
		if (Player.controlJump && wasSliding)
		{
			Player.velocity.X = (Player.controlRight ? 1 : -1) * -3;
			Player.wingTime = 0;
			

		}
		if (Player.controlJump && wasGrappling && Player.grapCount == 0)
		{
			if ((Player.controlLeft || Player.controlRight))
			{
				
				Player.velocity.X = (Player.controlRight ? 1 : -1) * 5;
				Player.velocity.Y = -5;
			}
			else
			{
				Player.velocity.X *= 1.2f;
			}

		}

		wasGrappling = Player.grapCount > 0;
		wasSliding = Player.sliding;
		base.PreUpdateMovement();
	}
	
	public override void ModifyExtraJumpDurationMultiplier(ExtraJump jump, ref float duration) {
		// Make the jump duration last for 2x longer than normal
		if (jump == ExtraJump.SandstormInABottle || jump == ExtraJump.BlizzardInABottle)
			duration *= 0.45f;
	}
}