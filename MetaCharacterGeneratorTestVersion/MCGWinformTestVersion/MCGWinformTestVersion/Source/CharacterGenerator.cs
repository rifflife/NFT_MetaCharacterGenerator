using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace MCG
{
	class CharacterGenerator
	{
		// Paths
		private string mSourcePath;
		private string mResultPath;

		// Base Bitmap
		private Bitmap mBaseBitmap;

		// Data
		private Dictionary<CharacterPart, List<PartBitmapSet>> mPartBitmapSet;
		private GenerateHexCode mHexCode = new GenerateHexCode();

		// Color Palette
		private ColorPalette mColorPalette;

		public Bitmap BaseBitmap { get { return mBaseBitmap; } }
		public int BitmapWidth { get; }
		public int BitmapHeight { get; }

		public CharacterGenerator(string sourcePath, string resultPath, ColorPalette colorPalette, int bitmapWidth = 1600, int bitmapHeight = 1600)
		{
			// Initialize Bitmap
			BitmapWidth = bitmapWidth;
			BitmapHeight = bitmapHeight;

			mBaseBitmap = new Bitmap(BitmapWidth, BitmapHeight);

			// Initialize Paths
			mSourcePath = sourcePath;
			mResultPath = resultPath;

			// Initialize Resources
			mPartBitmapSet = new Dictionary<CharacterPart, List<PartBitmapSet>>();

			foreach (var type in Enum.GetValues<CharacterPart>())
			{
				mPartBitmapSet.Add(type, new List<PartBitmapSet>());
			}

			mColorPalette = colorPalette;

			initializeResources();
		}

		/// <summary>
		/// Load images and data talbe
		/// </summary>
		private void initializeResources()
		{
			// Read every resource files
			var sourceFiles = Directory.GetFiles(mSourcePath);

			List<PartBitmapInfo> partBitmapSet = new List<PartBitmapInfo>();

			// Parse files
			foreach (var e in sourceFiles)
			{
				if (!PartCodeManager.IsImageFile(e))
				{
					continue;
				}

				partBitmapSet.Add(new PartBitmapInfo(e));
			}

			// Sort Part Bitmap Sets
			foreach (var set in partBitmapSet)
			{
				var currentType = set.PartType;

				if (currentType == CharacterPart.None)
				{
					continue;
				}

				int currentIndex = set.Index;
				int listCount = mPartBitmapSet[currentType].Count;

				for (int i = listCount; i <= currentIndex; i++)
				{
					mPartBitmapSet[currentType].Add(new PartBitmapSet());
				}

				mPartBitmapSet[currentType][currentIndex].AddBitmapInfo(set);
			}
		}
		public void SetByHenerateHexCode(GenerateHexCode hexCode)
		{
			mHexCode = hexCode;
		}
		public void RedrawBitmap(Action callBack)
		{
			Task drawTask = new Task(() =>
			{
				drawPart(CharacterPart.Background);
				drawQrCode();
				drawHexCode();
				drawPart(CharacterPart.BackHair);
				drawPart(CharacterPart.Face);
				drawPart(CharacterPart.Body);
				drawPart(CharacterPart.Cheek);
				drawPart(CharacterPart.Mouth);
				drawPart(CharacterPart.LeftEye);
				drawPart(CharacterPart.RightEye);
				drawPart(CharacterPart.Eyebrow);
				drawPart(CharacterPart.FrontHair);
				callBack();
			});

			drawTask.Start();
		}
		public int GetPartCount(CharacterPart type)
		{
			List<PartBitmapSet> list;

			if (mPartBitmapSet.TryGetValue(type, out list))
			{
				return list.Count;
			}

			return 0;
		}
		public int GetColorCount(CharacterPart type)
		{
			return mColorPalette.GetColorCount(type);
		}
		public void Save()
		{
			string fileName = $"{mHexCode.ToString()}{PartCodeManager.ImageExtension}";
			string filePath = Path.Combine(mResultPath, fileName);
			mBaseBitmap.Save(filePath);
		}
		private void drawPart(CharacterPart part)
		{
			int maxIndex = GetPartCount(part);

			byte index = (byte)(mHexCode.GetIndex(part) % maxIndex);

			var set = mPartBitmapSet[part][index];
			var color = mHexCode.GetColorCode(part);

			set.DrawOnBitmap(mBaseBitmap, color);
		}
		private void drawQrCode()
		{
			ColorCode backgroundColor = mHexCode.GetColorCode(CharacterPart.Background);

			Bitmap qrBitmap = ImageGenerator.GetQRCodeBitmap(mHexCode.ToString(), backgroundColor, 5, 10);

			int outterPadding = 20;

			using (Graphics g = Graphics.FromImage(mBaseBitmap))
			{
				g.DrawImage(qrBitmap, outterPadding, outterPadding);
			}

			qrBitmap.Dispose();
		}
		private void drawHexCode()
		{
			ColorCode backgroundColor = mHexCode.GetColorCode(CharacterPart.Background);

			Bitmap hexCodeBitmap = ImageGenerator.GetHexCodeBitmap(mHexCode.ToString(), backgroundColor, 30);

			int outterPadding = 20;

			using (Graphics g = Graphics.FromImage(mBaseBitmap))
			{
				g.DrawImage(hexCodeBitmap, 215, outterPadding + 10);
			}

			hexCodeBitmap.Dispose();
		}
	}
}