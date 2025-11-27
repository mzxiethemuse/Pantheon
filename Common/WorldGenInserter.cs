using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Pantheon.Common.Utils;
using Pantheon.Content.Reworks.World;
using Pantheon.Content.World.ChallengeAltars;
using Terraria;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Pantheon.Common;

public class WorldGenInserter : ModSystem
{
	public static List<WorldGenTask> TasksToInsert;

	public override void Load()
	{
		TasksToInsert = new();
		// i (mostly) understand how this works but i took it from https://stackoverflow.com/questions/5411694/get-all-inherited-classes-of-an-abstract-class
		foreach (Type type in Assembly.GetAssembly(typeof(WorldGenTask)).GetTypes().Where(myType => myType is { IsClass: true, IsAbstract: false } && myType.IsSubclassOf(typeof(WorldGenTask))))
		{
			TasksToInsert.Add((WorldGenTask)Activator.CreateInstance(type, [type.Name, 1D]));
		}
		// TasksToInsert.Add(new SurfaceTweaks("Pantheon:SurfaceTweaks", 2));
		base.Load();
	}

	public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
	{
		foreach (var t in TasksToInsert)
		{
			int index = tasks.FindIndex(task => task.Name.Equals(t.PlaceToInsert));
			if (index != -1)
			{
				tasks.Insert(index, t);
			}
		}
		base.ModifyWorldGenTasks(tasks, ref totalWeight);
	}
}