using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pantheon.Common;
using Pantheon.Content.World.ChallengeAltars;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pantheon
{
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class Pantheon : Mod
	{
		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			var ID = reader.ReadInt32();
			if (ID == (int)PacketTypes.ChallengeAltarSync)
			{
				ChallengeAltarSystem.Instance.HandleSyncPacket(reader, whoAmI);
			} 
			
		}
	}
}
