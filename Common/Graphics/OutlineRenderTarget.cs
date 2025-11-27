using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour;
using Pantheon.Assets;
using Pantheon.Common.Utils;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace Pantheon.Common.Graphics;



public class OutlineRenderTarget : ILoadable
{
	public RenderTarget2D renderTarget;
	List<OutlineDrawAction> actions;
	public event Action PreRenderOutlines;
	public event Action<Matrix> RenderToOutlineTarget;

	public static void AddAction(OutlineDrawAction action)
	{
		ModContent.GetInstance<OutlineRenderTarget>().actions.Add(action);
	}
	
	public void Load(Mod mod)
	{
		actions = new List<OutlineDrawAction>();

		if (!Main.dedServ)
		{
			// adding this makes the rendertargets resize when the resolution is changed
			// rendertargets should be initialized on the main thread :D
			Main.RunOnMainThread(() =>
			{
				Main.OnResolutionChanged += InitRT;
				renderTarget = new RenderTarget2D(Main.instance.GraphicsDevice, Main.screenWidth / 2, Main.screenHeight / 2);
			});
		}
		// i dont know why we use "CheckMonoliths", but that's where we hook into to draw things *to* the RT
		// On_Main.CheckMonoliths += DrawToRT;
		On_Main.DrawNPCs += DrawRTToScreen;
	}

	private void DrawRTToScreen(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
	{
		orig.Invoke(self, behindTiles);
		RenderTargetUtils.EnsureContentsArePreserved(Main.graphics.GraphicsDevice.GetRenderTargets());
		
		Main.spriteBatch.End();
		DrawToRT(null);
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

		if (!behindTiles)
		{
			Main.spriteBatch.End();

			if (renderTarget == null || renderTarget.IsDisposed)
			{
				return;
			}

			Shaders.Outline.Value.Parameters["staticColor"].SetValue(new Vector4(0f, 0f, 0f, 0f));

			Shaders.Outline.Value.Parameters["textureSize"].SetValue(renderTarget.Size());
			Main.spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, Main.DefaultSamplerState, default,
				Main.Rasterizer,
				Shaders.Outline.Value, Matrix.Identity); //Main.GameViewMatrix.TransformationMatrix);

			// Main.spriteBatch.Draw(renderTarget, new Vector2(Main.screenWidth, Main.screenHeight) / 2, Color.White);
			// Main.spriteBatch.Draw(renderTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, new Vector2(1 / SizeMultiplier.X, 1 / SizeMultiplier.Y), SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(renderTarget, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);

			Main.spriteBatch.End();
			// Main.spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, Main.DefaultSamplerState, default,
			// 	Main.Rasterizer,
			// 	null, Matrix.Identity); //Main.GameViewMatrix.TransformationMatrix);
			// Main.spriteBatch.Draw(renderTarget, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
			//
			// Main.spriteBatch.End();
			
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);


		}

	}

	private void DrawToRT(On_Main.orig_CheckMonoliths? orig)
	{
		orig?.Invoke();
		var gd = Main.graphics.GraphicsDevice;
		var oldRTs = gd.GetRenderTargets();
		
		gd.SetRenderTarget(renderTarget);
		gd.Clear(Color.Transparent);

		
		PreRenderOutlines?.Invoke();
		
		Dictionary<Color, List<OutlineDrawAction>> sortedActions = new Dictionary<Color, List<OutlineDrawAction>>();
		
		foreach (var act in actions)
		{
			if (sortedActions.ContainsKey(act.Color))
			{
				sortedActions[act.Color].Add(act);
			}
			else
			{
				sortedActions.Add(act.Color, new List<OutlineDrawAction>() { act });
			}
		}
		
		Matrix matrix = Main.GameViewMatrix.TransformationMatrix * Matrix.CreateScale(0.5f, 0.5f, 1f);
		matrix.Translation *= new Vector3(2f, 2f, 1f);
		foreach (var list in sortedActions)
		{
			Shaders.SolidColor.Value.Parameters["ucolor"].SetValue(list.Key.ToVector4());

			
			
			Main.spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, Main.DefaultSamplerState, default,
				Main.Rasterizer,
				Shaders.SolidColor.Value, matrix); //Main.GameViewMatrix.TransformationMatrix);
			// Lines.Rectangle(new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Red);

			foreach (var action in list.Value)
			{
				if (action.Item is { } item)
				{
					Main.instance.DrawItem(action.Item, action.ItemWhoAMI);
				}
				else if (action.NPCWhoAmI == -1)
				{
					Main.spriteBatch.Draw(action.Texture, (action.Position),
						action.Frame, Color.White, action.Rotation, action.Frame.Size() / 2, action.Scale,
						action.Effect, 0f);
				}
				else
				{
					Main.instance.DrawNPC(action.NPCWhoAmI, false);
				}
			}
			
			Main.spriteBatch.End();
		}
		RenderToOutlineTarget?.Invoke(matrix);

		// idk if this does anything, memorywise
		// sortedActions = null;
		// Main.spriteBatch.Draw(fuck, Vector2.Zero, Color.White);

		// Main.spriteBatch.DrawString(FontAssets.MouseText.Value, Main.screenWidth.ToString(), Vector2.Zero, Color.White, 0f, Vector2.Zero, 100f, SpriteEffects.None, 0f);
		actions.Clear();
		// thgis is a dummy thing
		gd.SetRenderTargets(oldRTs);
	}

	private void InitRT(Vector2 vec)
	{
		if (Main.dedServ)
		{
			return;
		}
        
		renderTarget?.Dispose();
		GraphicsDevice gd = Main.instance.GraphicsDevice;
		renderTarget = new RenderTarget2D(gd, (int)(vec.X * 0.5), (int)(vec.Y * 0.5));
	}

	public void Unload()
	{
		
	}
	
}

public readonly struct OutlineDrawAction
{
	public OutlineDrawAction(Texture2D texture, Vector2 position, float rotation = 0f, Rectangle? frame = null, SpriteEffects effect = SpriteEffects.None, Color color = default, float scale = 1f)
	{
		Position = position;
		Rotation = rotation;
		if (frame != null) Frame = (Rectangle)frame;
		Effect = effect;
		Color = color;
		Scale = scale;
		Texture = texture;
	}

	public OutlineDrawAction(int whoAmI, Color color)
	{
		NPCWhoAmI = whoAmI;
		Color = color;
	}
	
	public OutlineDrawAction(Item item, int whoAmI, Color color)
	{
		Item = item;
		ItemWhoAMI = whoAmI;
		Color = color;
	}

	public Item? Item { get;  } = null;

	public int NPCWhoAmI { get;  }= -1;
	public int ItemWhoAMI { get; } = -1;
	public Vector2 Position { get; }
	public float Rotation { get; }
	public Rectangle Frame { get; }
	public SpriteEffects Effect { get; }
	public Color Color { get; }
	public float Scale { get; }
	public Texture2D Texture { get; }
}