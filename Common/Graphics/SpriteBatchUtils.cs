using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Pantheon.Common.Graphics;

public static class SpriteBatchUtils
{
	public static void BeginFromSnapshot(this SpriteBatch spriteBatch, SpriteBatchSnapshot snapshot)
	{
		spriteBatch.Begin(snapshot.sortMode, snapshot.blendState, snapshot.samplerState, snapshot.depthStencilState, snapshot.rasterizerState, snapshot.effect, snapshot.matrix);
	}

	public static void With(this SpriteBatch spriteBatch, SpriteBatchSnapshot snapshot, Action action)
	{
		spriteBatch.With(snapshot.rasterizerState, snapshot.blendState, snapshot.samplerState, snapshot.sortMode, snapshot.depthStencilState, snapshot.effect, snapshot.matrix, action);
	}

	public static void With(this SpriteBatch spriteBatch, RasterizerState rasterizerState, BlendState blend, SamplerState samplerState,
		SpriteSortMode sortMode, DepthStencilState depthStencilState, Effect? effect, Matrix matrix, Action action)
	{
		bool needsRestart = spriteBatch.beginCalled;
		var snap = new SpriteBatchSnapshot(spriteBatch);
		if (needsRestart)
		{
			spriteBatch.End();
		}
		spriteBatch.Begin(sortMode, blend, samplerState, depthStencilState, rasterizerState, effect, matrix);
		action.Invoke();
		spriteBatch.End();
		if (needsRestart)
		{
			spriteBatch.BeginFromSnapshot(snap);
		}
	}

	public static Matrix RescaleMatrix(Matrix matrix, float factor)
	{
		Matrix m = Main.GameViewMatrix.TransformationMatrix * Matrix.CreateScale(factor, factor, 1f);
		m.Translation *= new Vector3(1 / factor, 1 / factor, 1f);
		return m;
	}
}

public struct SpriteBatchSnapshot(SpriteBatch spriteBatch)
{
	public RasterizerState rasterizerState = spriteBatch.rasterizerState;
	public  SamplerState samplerState = spriteBatch.samplerState;
	public Matrix matrix = spriteBatch.transformMatrix;
	public  SpriteSortMode sortMode = spriteBatch.sortMode;
	public  DepthStencilState depthStencilState = spriteBatch.depthStencilState;
	public  BlendState blendState = spriteBatch.blendState;
	public  Effect? effect = spriteBatch.spriteEffect;
}