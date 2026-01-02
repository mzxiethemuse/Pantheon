using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pantheon.Common.Players;
using ReLogic.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pantheon.Common;

// as of C#14: the fellas in #mod-dev will not kill me fer this one.
public static class PantheonPlayerExtensions {
	extension(Player p)
	{
		public TotemPlayer TotemPlayer => p.GetModPlayer<TotemPlayer>();

		public ManaPlayer ManaPlayer => p.GetModPlayer<ManaPlayer>();

		public FlamethrowerPlayer FlamethrowerPlayer => p.GetModPlayer<FlamethrowerPlayer>();
		
		public ItemCooldownPlayer ItemCooldownPlayer => p.GetModPlayer<ItemCooldownPlayer>();
	}
}