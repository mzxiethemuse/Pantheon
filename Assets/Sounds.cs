using Terraria.Audio;
using Terraria.ModLoader;

namespace Pantheon.Assets;

public class Sounds : ILoadable
{
	public void Load(Mod mod)
	{
	}

	public void Unload()
	{
	}

	public static SoundStyle SeaMinePrime = new SoundStyle(AssetDirectory.Sound + "SeaMinePrime");
	public static SoundStyle DreamcatcherMana = new SoundStyle(AssetDirectory.Sound + "DreamcatcherSoundShit");
	public static SoundStyle ManaBankRestore = new SoundStyle(AssetDirectory.Sound + "ManaBankRestore");

}