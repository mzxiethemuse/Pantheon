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

	public static SoundStyle ChallengeAltarFinish = new SoundStyle(AssetDirectory.Sound + "ChallengeAltarCongrats");
	public static SoundStyle ChallengeAltarSpawnWave = new SoundStyle(AssetDirectory.Sound + "ChallengeAltarSpawnWave");
	
	
	public static SoundStyle Lobotomy = new SoundStyle(AssetDirectory.Sound + "Lobotomy Sound Effect (DOWNLOAD)");

	
}