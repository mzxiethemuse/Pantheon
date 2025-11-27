using Microsoft.Xna.Framework;
using Pantheon.Common.Utils;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Pantheon.Content.Reworks.World;

public class SurfaceTweaks : WorldGenTask
{
	public SurfaceTweaks(string name, double loadWeight) : base(name, loadWeight)
	{
	}

	public override string PlaceToInsert => "Tile Cleanup";

	protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
	{
		WorldHelper.TryAtVariousPointsAlongX(80, 7, 14, i =>
		{
			Point surface = WorldHelper.FindSurfaceTileAtX(i + WorldGen.genRand.Next(-10, 11), 0);
			
			if (surface != new Point(-1, -1))
			{
				int type =  Framing.GetTileSafely(surface).TileType;
				int wall = -1;
				if (type == TileID.Grass)
				{
					wall = WallID.FlowerUnsafe;
				} else if (type == TileID.JungleGrass)
				{
					wall = Main.rand.NextBool() ? WallID.JungleUnsafe2 : WallID.JungleUnsafe;
				}

				if (wall == -1) return false;
				WorldUtils.Gen(surface, new Shapes.Slime(WorldGen.genRand.Next(4, 7)), Actions.Chain([
					new Modifiers.Blotches(3, 0.7D),
					new Modifiers.Expand(1),
					new Modifiers.Offset(0, 3),
					new Modifiers.RadialDither(0.99D, 0.5D),
					new Modifiers.IsEmpty(),
					new Modifiers.Offset(0, 2),
					new Actions.PlaceWall((ushort)wall),
					new Modifiers.Dither(0.95D),
				]));
			}
			return true;
		});
	}
}