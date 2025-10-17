using Terraria;

namespace Pantheon.Content.General.Dusts;

public class GlowDecaySmokeThingFuck : Dirt
{

	public override bool Update(Dust dust)
	{
		if (dust.customData != null) dust.customData = 0;
		
		dust.position += dust.velocity;
		dust.velocity *= 0.95f;
		dust.scale *= 0.92f;
		dust.alpha += 255 / 30;
		dust.rotation += dust.velocity.X * 0.05f;
		if (dust.alpha >= 255)
		{
			dust.active = false;
		}
		return false;
	}
}