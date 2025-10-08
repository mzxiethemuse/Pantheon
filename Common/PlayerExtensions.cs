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

// the fellas in #mod-dev would KILL ME fer this one!
public static class PantheonPlayerExtensions {
	public static TotemPlayer TotemPlayer(this Player p)
	{
		return p.GetModPlayer<TotemPlayer>();
	}
	
	public static ManaPlayer ManaPlayer(this Player p)
	{
		return p.GetModPlayer<ManaPlayer>();
	}
	
	public static FlamethrowerPlayer FlamethrowerPlayer(this Player p)
	{
		return p.GetModPlayer<FlamethrowerPlayer>();
	}
	

}