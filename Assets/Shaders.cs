using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Pantheon.Assets;

public class Shaders : ILoadable
{
	public void Load(Mod mod)
	{
		if (Main.netMode != NetmodeID.Server)
		{
			GaussianBlur.Value.Parameters["kernel"].SetValue(new float[]
			{
				1, 2, 1,
				2, 4, 2,
				1, 2, 1
			});
		}
	}

	public void Unload()
	{
	}
	
	
	
	/// <summary>
	/// Requires uShaderSpecificData x and y to be set. X corrlelates to the amount of waves, Y corelates to the intensity and allat. '1' Y-unit is one hundreth of the texture size.
	/// </summary>
	public static readonly Asset<Effect> Wave = ModContent.Request<Effect>(AssetDirectory.Shaders + "Wavey", AssetRequestMode.ImmediateLoad);
	public static readonly Asset<Effect> Scanline = ModContent.Request<Effect>(AssetDirectory.Shaders + "Scanline", AssetRequestMode.ImmediateLoad);
	public static readonly Asset<Effect> Shine = ModContent.Request<Effect>(AssetDirectory.Shaders + "Shine", AssetRequestMode.ImmediateLoad);

	/// <summary>
	/// has one parameter; float threshold
	/// </summary>
	public static readonly Asset<Effect> Threshold = ModContent.Request<Effect>(AssetDirectory.Shaders + "Threshold", AssetRequestMode.ImmediateLoad);

	/// <summary>
	/// parameters Color and uIntensity.
	/// </summary>
	public static readonly Asset<Effect> Burst = ModContent.Request<Effect>(AssetDirectory.Shaders + "Explosion", AssetRequestMode.ImmediateLoad);

	public static readonly Asset<Effect> ItemCooldown = ModContent.Request<Effect>(AssetDirectory.Shaders + "ItemCooldownShader", AssetRequestMode.ImmediateLoad);
	public static readonly Asset<Effect> ManaOpticDisplay = ModContent.Request<Effect>(AssetDirectory.Shaders + "ScaleThing", AssetRequestMode.ImmediateLoad);

	/// <summary>
	/// takes 2 inputs : float[9] kernel, float distance (100-300)
	/// </summary>
	public static readonly Asset<Effect> GaussianBlur = ModContent.Request<Effect>(AssetDirectory.Shaders + "GaussianBlur", AssetRequestMode.ImmediateLoad);

	/// <summary>
	/// this ones a doozy
	/// takes in: float uTime, float4 colorA, float4 colorB
	/// the texture is used as the noise/displacement/etc
	/// draw it with a noise texture if you wanna get anywhere
	/// </summary>
	public static readonly Asset<Effect> Fire = ModContent.Request<Effect>(AssetDirectory.Shaders + "FireQuad", AssetRequestMode.ImmediateLoad);

	public static void SetUpFireShader(Color a, Color b, float alpha, float speed = 0.02f)
	{
		Fire.Value.Parameters["colorA"].SetValue(a.ToVector4());
		Fire.Value.Parameters["colorB"].SetValue(b.ToVector4());
		Fire.Value.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * speed);
		Fire.Value.Parameters["alpha"].SetValue(alpha);
	}
	
	/// <summary>
	/// parameters: brightness, threshold (0-1), quality (int), amount (float)
	/// </summary>
	// public static readonly Asset<Effect> ShittyBloom = ModContent.Request<Effect>(AssetDirectory.Shaders + "DogshitBloom", AssetRequestMode.ImmediateLoad);

}

