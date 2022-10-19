//#define FAST_DEBUG

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MCGCore
{
	public static class ImageProcessor
	{
		private const int COLOR_BYTE_COUNT = 4; // For declare stride // BGRA = 4
		private const int INITIAL_DPI = 96; // Origin dpi value of wpf

		/// <summary>
		/// 이미지에 색상을 곱합니다.
		/// </summary>
		/// <param name="sourceImage">원본 이미지입니다.</param>
		/// <param name="color">곱할 색상입니다.</param>
		/// <returns></returns>
		public static BitmapSource MultiplyColor(BitmapSource sourceImage, Color32 color)
		{
			#if FAST_DEBUG
			return sourceImage;
			#endif

			WriteableBitmap wb = new WriteableBitmap(sourceImage);

			int width = wb.PixelWidth;
			int height = wb.PixelHeight;

			var colors = getColor32Array(sourceImage);

			Color32 curColor;

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					curColor = colors[y, x];
					colors[y, x].B = (byte)(curColor.B * color.FB);
					colors[y, x].G = (byte)(curColor.G * color.FG);
					colors[y, x].R = (byte)(curColor.R * color.FR);
				}
			}

			var pixles = getByteArrayFrom(in colors);
			Int32Rect rect = new Int32Rect(0, 0, width, height);
			int imageWidthStride = width * Color32.Stride;

			wb.WritePixels(rect, pixles, imageWidthStride, 0);

			return wb;
		}

		/// <summary>
		/// 이미지를 지정한 경로에 저장합니다.
		/// </summary>
		/// <param name="bitmapSource">저장할 이미지입니다.</param>
		/// <param name="saveFilePath">저장할 이미지 경로입니다.</param>
		/// <exception cref="NotSupportedException"/>
		public static void SaveImage(BitmapSource bitmapSource, string saveFilePath)
		{
			PngBitmapEncoder pngEncoder = new PngBitmapEncoder();
			pngEncoder.Frames.Add(BitmapFrame.Create(bitmapSource));

			using (Stream s = File.Create(saveFilePath))
			{
				pngEncoder.Save(s);
			}
		}

		/// <summary>
		/// 이미지를 지정한 경로에서 불러옵니다.
		/// </summary>
		/// <param name="loadFilePath">이미지 경로입니다.</param>
		/// <returns>비트맵 이미지 경로입니다.</returns>
		/// <exception cref="FileNotFoundedException"/>
		public static BitmapSource LoadBitmap(string loadFilePath)
		{
			try
			{
				return new BitmapImage(new Uri(loadFilePath));
			}
			catch(Exception e)
			{
				throw new FileNotFoundedException("Bitmap file not founded ! Load image failed!");
			}
		}

		/// <summary>
		/// 이미지를 합칩니다.
		/// </summary>
		/// <param name="sourceImages">합칠 이미지 리스트 입니다.</param>
		/// <param name="width">결과물의 너비입니다.</param>
		/// <param name="height">결과물의 높이입니다.</param>
		/// <returns></returns>
		public static BitmapSource MergeImages(in List<BitmapSource> sourceImages, int width, int height)
		{
			Rect drawingSize = new Rect(0, 0, width, height);

			DrawingVisual drawingVisual = new DrawingVisual();

			using (DrawingContext dc = drawingVisual.RenderOpen())
			{
				foreach (var i in sourceImages)
				{
					dc.DrawImage(i, drawingSize);
				}
			}

			RenderTargetBitmap bmp = new RenderTargetBitmap(width, height, INITIAL_DPI, INITIAL_DPI, PixelFormats.Pbgra32);
			bmp.Render(drawingVisual);

			return bmp;
		}

		private static Color32[,] getColor32Array(in BitmapSource bitmapSource)
		{
			int width = bitmapSource.PixelWidth;
			int height = bitmapSource.PixelHeight;

			int stride = width * COLOR_BYTE_COUNT;
			byte[] imageStream = new byte[height * stride];
			bitmapSource.CopyPixels(imageStream, stride, 0);

			Color32[,] colors = new Color32[height, width];

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					colors[y, x] = new Color32(imageStream, x, y, stride);
				}
			}

			return colors;
		}

		private static byte[] getByteArrayFrom(in Color32[,] color)
		{
			int streamIndex = 0;
			int stride = Color32.Stride;

			byte[] byteStream = new byte[color.Length * stride];

			int height = color.GetLength(0);
			int width = color.GetLength(1);

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					byteStream[streamIndex]	    = color[y, x].B;
					byteStream[streamIndex + 1] = color[y, x].G;
					byteStream[streamIndex + 2] = color[y, x].R;
					byteStream[streamIndex + 3] = color[y, x].A;
					streamIndex += stride;
				}
			}

			return byteStream;
		}
	}
}