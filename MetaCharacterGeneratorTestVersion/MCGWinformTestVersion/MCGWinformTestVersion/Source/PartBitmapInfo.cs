using System.Drawing;
using System.IO;

namespace MCG
{
	/// <summary>
	/// 부위별 이미지 정보입니다.
	/// </summary>
	class PartBitmapInfo
	{
		public string FileName { get; }
		public int Index { get; }
		public CharacterPart PartType { get; }
		public DrawMode DrawType { get; }

		public string FilePath { get; }

		public PartBitmapInfo(string filePath)
		{
			this.FilePath = filePath;
			this.FileName = Path.GetFileName(filePath);
			this.Index = PartCodeManager.ParseIndexFromFileName(filePath);
			this.PartType = PartCodeManager.ParseCharacterPartFromFileName(filePath);
			this.DrawType = PartCodeManager.ParseDrawModeFromFileName(FileName);
		}
		/// <summary>
		/// 해당 캐릭터 부위의 비트맵 이미지를 로드합니다. 사용후 메모리 해제가 필요합니다.
		/// </summary>
		public DirectBitmap GetBitmap()
		{
			Image loadedImage = Image.FromFile(FilePath);
			Bitmap bitmap = new Bitmap(loadedImage);
			DirectBitmap db = new DirectBitmap(bitmap.Width, bitmap.Height);
			db.SetBitmap(bitmap);
			loadedImage.Dispose();

			return db;
		}
		public override string ToString()
		{
			return FileName;
		}
	}
}
