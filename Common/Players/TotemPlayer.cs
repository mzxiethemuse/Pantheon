using Terraria.ModLoader;

namespace Pantheon.Common.Players;

public class TotemPlayer : ModPlayer
{
	
	
	public int ExtraTotemDuration = 0;
	public float TotemDurationIncrease = 0;
	
	

	public override void ResetEffects()
	{
		ExtraTotemDuration = 0;
		TotemDurationIncrease = 0;
		base.ResetEffects();
	}

	

	
}