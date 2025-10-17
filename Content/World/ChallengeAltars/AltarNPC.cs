using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pantheon.Assets;
using Pantheon.Common.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Pantheon.Content.World.ChallengeAltars;

public class AltarNPC : GlobalNPC
{
	public override bool InstancePerEntity => true;

	public bool partOfAltarChallenge;

	public override void Load()
	{
		if (!Main.dedServ)
		{
			// On_Main.DrawNPCs += On_MainOnDrawNPCs;
			ModContent.GetInstance<OutlineRenderTarget>().PreRenderOutlines += OnPreRenderOutlines;
		}
	}

	private void OnPreRenderOutlines()
	{
		foreach (var npc in Main.npc.Where(npc => npc.active && npc.GetGlobalNPC<AltarNPC>().partOfAltarChallenge))
		{
			OutlineRenderTarget.AddAction(new OutlineDrawAction(npc.whoAmI, Main.LocalPlayer.GetModPlayer<ChallengeAltarPlayer>().GetAltarFireSecondaryColor() with { A = 255 }));
		}
	}
	/// <summary>
	/// currently unused
	/// </summary>
	private void On_MainOnDrawNPCs(On_Main.orig_DrawNPCs orig, Main self, bool behindtiles)
	{
		orig.Invoke(self, behindtiles);
		Shaders.AltarNPCOverlay.Value.Parameters["uTime"].SetValue((float)(Main.timeForVisualEffects * 0.05f));
		Shaders.AltarNPCOverlay.Value.Parameters["uColor"].SetValue(Main.LocalPlayer.GetModPlayer<ChallengeAltarPlayer>().GetAltarFireSecondaryColor().ToVector4());

		Main.graphics.GraphicsDevice.Textures[1] = Textures.Noise.Value;
		Main.spriteBatch.End();
		if (!behindtiles)
		{
			foreach (var npc in Main.npc.Where(npc => npc.active && npc.GetGlobalNPC<AltarNPC>().partOfAltarChallenge))
			{
				var t = TextureAssets.Npc[npc.type].Value;
				Shaders.AltarNPCOverlay.Value.Parameters["resolution"].SetValue(t.Size());
				Shaders.AltarNPCOverlay.Value.Parameters["sourceRect"].SetValue(new Vector4(npc.frame.X, npc.frame.Y, npc.frame.Width, npc.frame.Height));
				Shaders.AltarNPCOverlay.Value.Parameters["opacity"].SetValue(0.5f);

				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, Main.Rasterizer, Shaders.AltarNPCOverlay.Value, Main.GameViewMatrix.TransformationMatrix);

				Main.instance.DrawNPC(npc.whoAmI, false);
				// Main.spriteBatch.Draw(
				// 	TextureAssets.Npc[npc.type].Value,
				// 	npc.Center - Main.screenPosition + new Vector2(0, npc.gfxOffY),
				// 	npc.frame,
				// 	Color.Blue,
				// 	npc.rotation,
				// 	npc.frame.Size() / 2,
				// 	npc.scale,
				// 	npc.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
				// 	0f
				// );
				Main.spriteBatch.End();
				// Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, Main.Rasterizer, Shaders.Outline.Value, Main.GameViewMatrix.TransformationMatrix);
				//
				//
				// Main.spriteBatch.Draw(
				// 	TextureAssets.Npc[npc.type].Value,
				// 	npc.Center - Main.screenPosition + new Vector2(0, npc.gfxOffY),
				// 	npc.frame,
				// 	Color.Blue,
				// 	npc.rotation,
				// 	npc.frame.Size() / 2,
				// 	npc.scale,
				// 	npc.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
				// 	0f
				// );
				// Main.spriteBatch.End();
			}
		}
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
	}

	public override void OnKill(NPC npc)
	{
		if (partOfAltarChallenge) ChallengeAltarSystem.Instance.enemiesLeft--;
		base.OnKill(npc);
	}

	public override Color? GetAlpha(NPC npc, Color drawColor)
	{
		if (partOfAltarChallenge)
		{
			npc.SpawnedFromStatue = true;
			if (npc.HasPlayerTarget)
			{
				Player target = Main.player[npc.target];
				return Color.Lerp((target.GetModPlayer<ChallengeAltarPlayer>().GetAltarFirePrimaryColor() * 2f) with { A = drawColor.A}, drawColor, 0.9f);
			}
			return drawColor;
		}
		return base.GetAlpha(npc, drawColor);
	}
}