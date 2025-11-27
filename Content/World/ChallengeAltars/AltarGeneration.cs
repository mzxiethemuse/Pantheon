using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Pantheon.Common.Utils;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Pantheon.Content.World.ChallengeAltars;

public class AltarGeneration : WorldGenTask
{
	private static List<Point> altars = new List<Point>();
	
	public AltarGeneration(string name, double loadWeight) : base(name, loadWeight)
	{
	}

	public override string PlaceToInsert => "Micro Biomes";

	protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
	{
		
		WorldHelper.TryAtVariousPointsAlongX(30, Main.maxTilesX / 255, Main.maxTilesX / 135, (int x) =>
		{
			int y = (int)MathHelper.Lerp((float)Main.worldSurface, Main.UnderworldLayer, WorldGen.genRand.NextFloat());
			Point pos = new Point(x, y);
			return TryGenerateAltarAtPos(pos);
		});
	}

	public static bool TryGenerateAltarAtPos(Point pos)
	{
		var x = pos.X;
		var y = pos.Y;
		foreach (Point altar in altars)
        {
        	if (pos.ToVector2().Distance(altar.ToVector2()) <= 85f)
        	{
        		return false;
        	}
        }
        // writing out workflow
        /*
         * needs to not contain any: lihzard tiles, dungeon tiles (will do dungeon altars in seperate pass..), hive blocks
         * should go down until it finds a solid position (or fail if it goes into underworld or exceeds amt of tries)
         * if it finds a solid tile first try, then go up until you find a floor
         */

        Tile chosenTile = Framing.GetTileSafely(x, y);
        if (chosenTile.HasTile)
        {
        	// go up until reach air
        	int extraY = 1;
        	while (chosenTile.HasTile)
        	{
        		chosenTile = Framing.GetTileSafely(x, y - extraY);
        		extraY++;
        		if (extraY > 20)
        		{
        			// fail if we take too long
        			return false;
        		}
        	}
        	// this will give us the first tile that is below air
        	y = y - extraY + 1;
        }
        else
        {
        	// go down until reach solid
        	int extraY = 1;
        	while (!chosenTile.HasTile)
        	{
        		chosenTile = Framing.GetTileSafely(x, y + extraY);
        		extraY++;
        		if (extraY > 20)
        		{
        			// fail if we take too long
        			return false;
        		}
        	}
        	// this will give us the first tile that is solid
        	y = y + extraY;
        }
        
        
        // TODO: MAKE SURE ALTARS ONLY SPAWN ON TILES WITH PROPER LOOT CODED IN
        // TODO: ADD SECONDARY PASS TO GENERATE EXTRA ALTARS IN CERTAIN BIOMES?
        
        var invalidTileCount = new Ref<int>(0);			
        WorldUtils.Gen(new Point(x, y), new Shapes.Circle(5), Actions.Chain([
        	new Modifiers.OnlyTiles([TileID.LihzahrdBrick, TileID.BlueDungeonBrick, TileID.GreenDungeonBrick, TileID.PinkDungeonBrick, TileID.Hive]),
        	new Actions.Scanner(invalidTileCount),
        ]));

        if (invalidTileCount.Value == 0)
        {
	        // clear out a nice little hole in the wall
	        
	        
	        ShapeData shape = new ShapeData();
	        WorldUtils.Gen(new Point(x, y - 6), new Shapes.Circle(13, 6), Actions.Chain([
		        new Actions.ClearTile(true).Output(shape),
		        //failsafe
		        new Modifiers.SkipWalls([WallID.Sandstone, WallID.HardenedSand]),
		        new Actions.ClearWall(),
	        ]));
	        // try and make things grassy (if this action even does what i think it does lol
	        WorldUtils.Gen(new Point(x, y - 6), new ModShapes.OuterOutline(shape), new ActionGrass());
	        // place a nice little platform
	        WorldGen.PlaceObject(x, y, (ushort)ModContent.TileType<ChallengeAltarTile>());
	        ModContent.GetInstance<ChallengeAltarEntity>().Place(x - 1, y - 3);
	        // quickly setting loot
	        TileEntity entity;
	        TileEntity.TryGet(x - 1, y - 3, out entity);
	        if (entity is ChallengeAltarEntity altar)
	        {
		        altar.Update();
	        }
            WorldUtils.Gen(new Point(x, y), new Shapes.Mound(3, 4), Actions.Chain([
	            new Actions.PlaceWall(WallID.GrayBrick)
            ]));

        }
        
        altars.Add(pos);
        return true;
	}
	
	
}