using System;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Pantheon.Assets;
/// <summary>
/// This class is EGREGIOUS, but we use tml-build AssetReferences now, i dont feel like migrating old stuff, so its marked as deprecated for now
/// </summary>
[Obsolete("We don't use this anymore, use AssetReferences from TML-Build instead... kept around for compat sake", false)]
public class OldSounds : ILoadable
{
	public void Load(Mod mod)
	{
	}

	public void Unload()
	{
	}

	
}