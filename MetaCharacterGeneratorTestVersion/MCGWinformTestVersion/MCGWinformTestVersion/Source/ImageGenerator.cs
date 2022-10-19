using QRCoder;
using System.Drawing;
using System.Text;

namespace MCG
{
	static class ImageGenerator
	{
		private const int mPivot = (int)(255 * 1.5f);
		private const float mColorRatio = 0.6f;

		static public Bitmap GetQRCodeBitmap(string hexCode, ColorCode bg, int size, int innerPadding)
		{
			QRCodeGenerator qrCodeGenerator = new QRCodeGenerator();
			QRCodeData qrCodeData = qrCodeGenerator.CreateQrCode(hexCode, QRCodeGenerator.ECCLevel.Q);
			QRCode qrCode = new QRCode(qrCodeData);

			// Set Color
			int sum = bg.R + bg.G + bg.B;
			
			ColorCode inColor;

			if (sum > mPivot)
			{
				inColor = getDarkColor(bg, mColorRatio);
			}
			else
			{
				inColor = getBrightColor(bg, mColorRatio);
			}

			Bitmap qrBitmap = qrCode.GetGraphic(size, inColor.GetColor(), bg.GetColor(), false);
			int padding = innerPadding * 2;
			Bitmap fullBitmap = new Bitmap(qrBitmap.Width + padding, qrBitmap.Height + padding);

			using (Graphics g = Graphics.FromImage(fullBitmap))
			using (Pen p = new Pen(inColor.GetColor()))
			{
				p.Width = (int)(size * 2);
				g.DrawRectangle(p, 0, 0, fullBitmap.Width, fullBitmap.Height);
				g.DrawImage(qrBitmap, innerPadding, innerPadding);
			}

			qrBitmap.Dispose();

			return fullBitmap;
		}

		static public Bitmap GetHexCodeBitmap(string hexCode, ColorCode bg, int fontSize)
		{
			// Set Color
			int sum = bg.R + bg.G + bg.B;

			ColorCode fontColor;

			if (sum > mPivot)
			{
				fontColor = getDarkColor(bg, mColorRatio);
			}
			else
			{
				fontColor = getBrightColor(bg, mColorRatio);
			}

			// Align hex code
			StringBuilder sb = new StringBuilder();

			int index = 0;
			int col = 0;
			foreach (char c in hexCode)
			{
				if (index != 0 && index % 5 == 0)
				{
					col++;

					if (col == 5)
					{
						sb.Append('\n');
					}
					else
					{
						sb.Append(' ');
					}
				}

				sb.Append(c);

				index++;
			}

			hexCode = sb.ToString();

			Bitmap fullBitmap = new Bitmap(1000, 400);

			using (Graphics g = Graphics.FromImage(fullBitmap))
			using (Font f = new Font("돋움체", fontSize, FontStyle.Bold, GraphicsUnit.Pixel))
			using (Brush b = new SolidBrush(fontColor.GetColor()))
			{
				g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
				//g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
				g.DrawString(hexCode, f, b, 0, 0);
			}

			return fullBitmap;
		}

		/// <summary>
		/// 0.0 에 가까울 수록 어둡고 1.0 에 가까울 수록 밝습니다/
		/// </summary>
		static private ColorCode getDarkColor(ColorCode color, float ratio)
		{
			byte r = (byte)(color.R * ratio);
			byte g = (byte)(color.G * ratio);
			byte b = (byte)(color.B * ratio);

			return new ColorCode(r, g, b);
		}

		/// <summary>
		/// 0.0 에 가까울 수록 밝고 1.0 에 가까울 수록 어둡습니다./
		/// </summary>
		static private ColorCode getBrightColor(ColorCode color, float ratio)
		{
			byte r = (byte)(255 - ((255 - color.R) * ratio));
			byte g = (byte)(255 - ((255 - color.G) * ratio));
			byte b = (byte)(255 - ((255 - color.B) * ratio));

			return new ColorCode(r, g, b);
		}
	}
}
