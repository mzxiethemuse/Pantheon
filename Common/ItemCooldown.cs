using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pantheon.Assets;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pantheon.Common;

public class ItemCooldownPlayer : ModPlayer
{
	public Dictionary<int, ItemCooldownData> ItemCooldowns;

	public override void Load()
	{
		ItemCooldowns = new Dictionary<int, ItemCooldownData>();
		
	}

	public override void UpdateEquips()
	{
		var toRemove = new List<int>();
		if (ItemCooldowns != null)
		{
			foreach (var pair in ItemCooldowns)
			{
				ItemCooldowns[pair.Key].cooldown++;
				if (ItemCooldowns[pair.Key].cooldown > pair.Value.maxTime)
				{
					toRemove.Add(pair.Key);
				}
			}

			foreach (var id in toRemove)
			{
				ItemCooldowns.Remove(id);
			}
		} else
		{
			ItemCooldowns = new Dictionary<int, ItemCooldownData>();

			// Main.NewText("BIg boner down the lane");
		}
		base.UpdateEquips();
	}

	public void SetCooldown(int type, int cooldown)
	{
		if (ItemCooldowns.ContainsKey(type))
		{
			ItemCooldowns[type] = new ItemCooldownData(cooldown);
		}
		else
		{
			ItemCooldowns.Add(type, new ItemCooldownData(cooldown));
		}
	}

	public bool IsItemOnCooldown(int type) => ItemCooldowns.ContainsKey(type);
}

public static class CooldownExtensions
{
	extension(Player player)
	{
		
		// ReSharper disable once UnusedMember.Global
		public bool IsItemOnCoolDown(int type) => player.ItemCooldownPlayer.ItemCooldowns.ContainsKey(type);
	}
}

public class CooldownGlobalItem : GlobalItem
{
	public override bool CanUseItem(Item item, Player player) =>
		!player.ItemCooldownPlayer.ItemCooldowns.ContainsKey(item.type);
	
	public bool myPlayerHasCooldownFor(int type) => Main.myPlayer != 255 && Main.LocalPlayer.ItemCooldownPlayer.ItemCooldowns != null && Main.LocalPlayer.ItemCooldownPlayer.ItemCooldowns.ContainsKey(type);


	public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame,
		Color drawColor,
		Color itemColor, Vector2 origin, float scale)
	{
		if (myPlayerHasCooldownFor(item.type))
		{
			ItemCooldownData data = Main.LocalPlayer.ItemCooldownPlayer.ItemCooldowns[item.type];
			OldShaders.ItemCooldown.Value.Parameters["uTime"].SetValue(data.cooldown / (float)data.maxTime);
			OldShaders.ItemCooldown.Value.Parameters["uImageSize1"].SetValue(TextureAssets.Item[item.type].Size());
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, OldShaders.ItemCooldown.Value,
				Main.UIScaleMatrix);
		}
		return true;
	}

	public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor,
		Color itemColor, Vector2 origin, float scale)
	{
		if (myPlayerHasCooldownFor(item.type))
		{
			Main.spriteBatch.End();

			Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);
		}
	}
	
	
}

public class ItemCooldownData
{
	public int maxTime;
	public int cooldown = 0;

	public ItemCooldownData(int time)
	{
		maxTime = time;
	}
}