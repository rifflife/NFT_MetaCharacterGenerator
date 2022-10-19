using System.Windows.Media.Imaging;

namespace MCGCore
{
	public class ImagePart
	{
		public string ImageRelativePath = null;
		public bool HasBindedColor = false;
		public Color32 BindedColor = Color32.White;

		public ImagePart() { }

		public ImagePart(ImagePart copy)
		{
			this.ImageRelativePath = copy.ImageRelativePath;
			this.HasBindedColor = copy.HasBindedColor;
			this.BindedColor = copy.BindedColor;
		}

		public ImagePart(string imageRelativePath)
		{
			ImageRelativePath = imageRelativePath;
		}

		public ImagePart(string imageRelativePath, Color32 blendingColor)
		{
			ImageRelativePath = imageRelativePath;
			BindedColor = blendingColor;
			HasBindedColor = true;
		}

		public ImagePart Copy()
		{
			return new ImagePart(this);
		}

		public void BindColor(Color32 blendingColor)
		{
			BindedColor = blendingColor;
			HasBindedColor = true;
		}

		/// <summary>
		/// 바인딩된 색상이 곱해진 이미지를 반환합니다.
		/// </summary>
		/// <returns>색상이 곱해진 이미지입니다.</returns>
		public BitmapSource GetBitmap()
		{
			var bitmap = ImageProcessor.LoadBitmap(ImageRelativePath);

			if (!HasBindedColor || BindedColor == Color32.White)
			{
				return bitmap;
			}

			return ImageProcessor.MultiplyColor(bitmap, BindedColor);
		}

		public override string ToString()
		{
			return $"Binded Color : {BindedColor} / Relative Path : {ImageRelativePath}";
		}
	}
}
