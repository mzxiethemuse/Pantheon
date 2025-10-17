using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Pantheon.Assets;

public class Textures : ILoadable
{
	public void Load(Mod mod)
	{
		
		for (int i = 0; i < 5; i++)
		{
			VFXSmoke[i] = ModContent.Request<Texture2D>(AssetDirectory.Vfx + "smoke_0" + (i + 1).ToString());
		}
		
		for (int i = 0; i < 3; i++)
		{
			VFXWaterBad[i] = ModContent.Request<Texture2D>(AssetDirectory.Vfx + "water_0" + (i + 1).ToString());
		}
	}

	public void Unload()
	{
	}
	
	
	
	

	public static readonly Asset<Texture2D> GenericItem = ModContent.Request<Texture2D>(AssetDirectory.Placeholder + "GenericItem"); 
	public static readonly Asset<Texture2D> VFXExplosion = ModContent.Request<Texture2D>(AssetDirectory.Vfx + "Explosion"); 
	public static readonly Asset<Texture2D> VFXDirt = ModContent.Request<Texture2D>(AssetDirectory.Vfx + "dirt_01");
	public static readonly Asset<Texture2D> Noise = ModContent.Request<Texture2D>(AssetDirectory.Vfx + "NoiseTexture");

	public static Asset<Texture2D>[] VFXSmoke = new Asset<Texture2D>[6];
	public static Asset<Texture2D>[] VFXWaterBad = new Asset<Texture2D>[3];

	
	public static readonly Asset<Texture2D> ManaOpticDisplayBG = ModContent.Request<Texture2D>(AssetDirectory.Vfx + "ManaOpticRectangleOh Yeah"); 
	public static readonly Asset<Texture2D> ManaBankOverlay = ModContent.Request<Texture2D>(AssetDirectory.UI + "ManaBankOverlay"); 

	
	public static readonly Asset<Texture2D> Wishbone = ModContent.Request<Texture2D>("Pantheon/Content/Combat/Mage/Wishbone");
	public static readonly Asset<Texture2D> WishboneOverlay = ModContent.Request<Texture2D>(AssetDirectory.UI + "WishboneOverlay"); 
	public static readonly Asset<Texture2D> ManaStarGlowverlay = ModContent.Request<Texture2D>(AssetDirectory.UI + "ManaStarGlowOverlay"); 
}