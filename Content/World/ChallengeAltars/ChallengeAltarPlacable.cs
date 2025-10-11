using Pantheon.Assets;
using Terraria.ModLoader;

namespace Pantheon.Content.World.ChallengeAltars;

public class ChallengeAltarPlacable : ModItem
{
	public override string Texture => AssetDirectory.Placeholder + "GenericItem";

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<ChallengeAltarTile>());
		base.SetDefaults();
	}
}