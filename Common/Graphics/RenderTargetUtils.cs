using Microsoft.Xna.Framework.Graphics;

namespace Pantheon.Common.Graphics;

public static class RenderTargetUtils
{
	public static void EnsureContentsArePreserved(RenderTargetBinding[] bindings)
	{
		foreach (var binding in bindings)
		{
			if (binding.RenderTarget is RenderTarget2D rt)
			{
				rt.RenderTargetUsage = RenderTargetUsage.PreserveContents;
			}
		}
	}
}