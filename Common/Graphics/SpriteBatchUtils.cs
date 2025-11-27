using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pantheon.Common.Graphics;

public static class SpriteBatchUtils
{
	public static void BeginFromSnapshot(this SpriteBatch spriteBatch, SpriteBatchSnapshot snapshot)
	{
		spriteBatch.Begin(snapshot.sortMode, snapshot.blendState, snapshot.samplerState, snapshot.depthStencilState, snapshot.rasterizerState, snapshot.effect);
	}
}

public struct SpriteBatchSnapshot(SpriteBatch spriteBatch)
{
	public readonly RasterizerState rasterizerState = spriteBatch.rasterizerState;
	public readonly SamplerState samplerState = spriteBatch.samplerState;
	public Matrix matrix = spriteBatch.transformMatrix;
	public readonly SpriteSortMode sortMode = spriteBatch.sortMode;
	public readonly DepthStencilState depthStencilState = spriteBatch.depthStencilState;
	public readonly BlendState blendState = spriteBatch.blendState;
	public readonly Effect? effect = spriteBatch.spriteEffect;
}