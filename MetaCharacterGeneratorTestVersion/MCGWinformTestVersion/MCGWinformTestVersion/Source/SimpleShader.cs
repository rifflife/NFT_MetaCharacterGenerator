using System;
using System.Drawing;
using System.Threading.Tasks;

namespace MCG
{
	static class SimpleShader
	{
		static public void DrawBitmap(Bitmap targetBitmap, DirectBitmap drawImage, int x = 0, int y = 0)
		{
			using (Graphics g = Graphics.FromImage(targetBitmap))
			{
				g.DrawImage(drawImage.Bitmap, x, y);
			}
		}
		static public void AddShadowMap(DirectBitmap shadowMap, Color inColor)
		{
			if (shadowMap == null)
			{
				return;
			}

			int width = shadowMap.Width;
			int height = shadowMap.Height;

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					Color c = shadowMap.GetPixel(x, y);

					int r = addShadowInPixel(c.R, inColor.R);
					int g = addShadowInPixel(c.G, inColor.G);
					int b = addShadowInPixel(c.B, inColor.B);

					shadowMap.SetPixel(x, y, Color.FromArgb(c.A, r, g, b));
				}
			}
		}
		static private int addShadowInPixel(int srcPixel, int shadowPixel)
		{
			return Math.Clamp(srcPixel - (255 - shadowPixel), 0, 255);
		}
		static public void MultiplyColorToBitmap(Bitmap bitmap, Color inColor)
		{
			int width = bitmap.Width;
			int height = bitmap.Height;

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					Color c = bitmap.GetPixel(x, y);
					float ratio = c.R / 255.0f;
					int r = (int)(inColor.R * ratio);
					int g = (int)(inColor.G * ratio);
					int b = (int)(inColor.B * ratio);
					bitmap.SetPixel(x, y, Color.FromArgb(r, g, b));
				}
			}
		}
	}
}
