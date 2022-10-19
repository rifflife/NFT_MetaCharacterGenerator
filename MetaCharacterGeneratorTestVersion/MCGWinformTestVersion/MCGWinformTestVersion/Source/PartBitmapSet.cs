using System.Collections.Generic;
using System.Drawing;

namespace MCG
{
	class PartBitmapSet
	{
		private Dictionary<DrawMode, PartBitmapInfo> mPartBitmapDatas;

		public PartBitmapSet()
		{
			mPartBitmapDatas = new Dictionary<DrawMode, PartBitmapInfo>();
		}

		public void AddBitmapInfo(PartBitmapInfo info)
		{
			mPartBitmapDatas.Add(info.DrawType, info);
		}
		public void DrawOnBitmap(Bitmap target, ColorCode color)
		{
			drawByMode(target, DrawMode.Base);
			drawByMode(target, DrawMode.Fill, color);
			drawByMode(target, DrawMode.Draw);
		}

		private void drawByMode(Bitmap target, DrawMode mode)
		{
			if (mode == DrawMode.None)
			{
				return;
			}

			if (mPartBitmapDatas.ContainsKey(mode))
			{
				DirectBitmap curBitmap = mPartBitmapDatas[mode].GetBitmap();
				SimpleShader.DrawBitmap(target, curBitmap);

				curBitmap.Dispose();
			}
		}

		private void drawByMode(Bitmap target, DrawMode mode, ColorCode color)
		{
			if (mode == DrawMode.None)
			{
				return;
			}

			if (mPartBitmapDatas.ContainsKey(mode))
			{
				DirectBitmap curBitmap = mPartBitmapDatas[mode].GetBitmap();
				
				SimpleShader.AddShadowMap(curBitmap, color.GetColor());
				SimpleShader.DrawBitmap(target, curBitmap);

				curBitmap.Dispose();
			}
		}
	}
}