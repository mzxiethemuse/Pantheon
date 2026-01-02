using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pantheon.Core;
using Terraria;

namespace Pantheon.Common.Graphics;

public static class Trails
{
	
	public static void DrawSoftTrail(Vector2[] positions, float[]? rotations, Func<float, Color> colorFunc,
		Func<float, float> alphaFunc, Func<float, float> scaleFunc, Vector2 offset = default(Vector2))
	{
		var texture = AssetReferences.Assets.Vfx.SoftGlowUnmultiplied.Asset.Value;
		var snapshot = new SpriteBatchSnapshot(Main.spriteBatch);
	    PixelationRenderTarget.AddPixelatedRenderAction(() =>
	    {
	        Main.spriteBatch.BeginFromSnapshot(snapshot with {matrix = SpriteBatchUtils.RescaleMatrix(snapshot.matrix, 0.5f)});
	        for (int i = positions.Length - 1; i > 0; i--)
	        {
		        float progress = (i / (float)positions.Length);
	            float alpha = alphaFunc(progress);
	            Main.spriteBatch.Draw(texture, positions[i] - Main.screenPosition + offset, null, colorFunc(progress) with {A = 0} * alpha, rotations is { } r ? r[i] : 0f, texture.Size() / 2, 0.1f * scaleFunc(progress), SpriteEffects.None, 0f);

	        }
	        
	        Main.spriteBatch.End();
	    });
	}
	
		public static void DrawTextureTrail(Texture2D texture, Vector2[] positions, float[]? rotations, Func<float, Color> colorFunc,
    		Func<float, float> alphaFunc, Func<float, float> scaleFunc, Vector2 offset = default(Vector2))
    	{
    		var snapshot = new SpriteBatchSnapshot(Main.spriteBatch);
    	    PixelationRenderTarget.AddPixelatedRenderAction(() =>
    	    {
    	        Main.spriteBatch.BeginFromSnapshot(snapshot with {matrix = SpriteBatchUtils.RescaleMatrix(snapshot.matrix, 0.5f)});
    	        for (int i = positions.Length - 1; i > 0; i--)
    	        {
    		        float progress = (i / (float)positions.Length);
    	            float alpha = alphaFunc(progress);
    	            Main.spriteBatch.Draw(texture, positions[i] - Main.screenPosition + offset, null, colorFunc(progress) with {A = 0} * alpha, rotations is { } r ? r[i] : 0f, texture.Size() / 2, 0.1f * scaleFunc(progress), SpriteEffects.None, 0f);
    
    	        }
    	        
    	        Main.spriteBatch.End();
    	    });
    	}

	public static void DrawSoftTrail(Vector2[] positions, float[]? rotations, Color color, float scale, Vector2 offset)
	{
		DrawSoftTrail(positions, rotations, progress => color, progress => 1 - progress, progress => scale * (1 - progress), offset);
	}
	
	public static void DrawTextureTrail(Texture2D texture, Vector2[] positions, float[]? rotations, Color color, float scale, Vector2 offset)
	{
		DrawTextureTrail(texture, positions, rotations, progress => color, progress => 1 - progress, progress => scale * (1 - progress), offset);
	}
}