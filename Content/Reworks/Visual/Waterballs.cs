using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pantheon.Assets;
using Pantheon.Common.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pantheon.Content.Reworks.Visual;

public class Waterballs : MetaballFondler
{

	protected override Color Color => Color.Blue with {A = 49};
	protected override Color OutlineColor => Color.DarkBlue with {A = 150};

	public override void DrawMetaballsToTarget()
	{
		
		foreach (var dust in Main.dust.Where(dust => dust.active && dust.type == DustID.DungeonWater))
		{
			float scale = 0.02f * dust.scale;
			dust.color.A = 0;
			Main.spriteBatch.Draw(Textures.VFXSoftGlow.Value, dust.position - Main.screenPosition, null,
				Color.White with { A = 0 }, 0f, Textures.VFXSoftGlow.Value.Size() / 2, (dust.customData == null ? scale : 0.8f),
				SpriteEffects.None, 0f);
			
		}
	}

	public override void OnLoad()
	{
		
	}

	public override void OnUnload()
	{
		
	}
}

// public class Bloodballs : MetaballFondler
// {
// 	protected override Color Color => (Color.DarkRed * 1.4f) with {A = 155};
// 	protected override Color OutlineColor => Color.DarkRed with {A = 155};
//
// 	public override void DrawMetaballsToTarget()
// 	{
// 		
// 		foreach (var dust in Main.dust.Where(dust => dust.active && dust.type == DustID.Blood))
// 		{
// 			float scale = 0.02f * dust.scale;
// 			dust.scale *= 0.98f;
// 			dust.velocity.Y += 0.75f;
// 			Main.spriteBatch.Draw(Textures.VFXSoftGlow.Value, dust.position - Main.screenPosition, null,
// 				Color.White with { A = 0 }, 0f, Textures.VFXSoftGlow.Value.Size() / 2, (dust.scale * 0.025f),
// 				SpriteEffects.None, 0f);
// 			dust.alpha = 255;
// 		}
// 	}
//
// 	public override void OnLoad()
// 	{
// 		
// 	}
//
// 	public override void OnUnload()
// 	{
// 		
// 	}
// }