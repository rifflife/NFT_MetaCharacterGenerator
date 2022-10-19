using System.Drawing;

namespace MCG
{
	struct ColorCode
	{
		public byte R;
		public byte G;
		public byte B;

		public ColorCode(byte r, byte g, byte b)
		{
			R = r;
			G = g;
			B = b;
		}
		public ColorCode(string hexCode)
		{
			this = ParseHexCodeToColor(hexCode);
		}
		public ColorCode(ColorCode copy)
		{
			this = copy;
		}

		/// <summary>
		/// 6자리 16진수 RGB 색상 코드를 색상 코드로 변환합니다.
		/// </summary>
		public static ColorCode ParseHexCodeToColor(string colorHexCode)
		{
			if (!IsColorHexCode(colorHexCode))
			{
				return new ColorCode(255, 255, 255);
			}

			byte r = byte.Parse($"{colorHexCode[0]}{colorHexCode[1]}", System.Globalization.NumberStyles.HexNumber);
			byte g = byte.Parse($"{colorHexCode[2]}{colorHexCode[3]}", System.Globalization.NumberStyles.HexNumber);
			byte b = byte.Parse($"{colorHexCode[4]}{colorHexCode[5]}", System.Globalization.NumberStyles.HexNumber);

			return new ColorCode(r, g, b);
		}
		/// <summary>
		/// 해당 색상코드가 유효한지 여부를 반환합니다.
		/// </summary>
		public static bool IsColorHexCode(string colorHexCode)
		{
			colorHexCode = colorHexCode.ToLower();

			if (colorHexCode.Length != 6)
			{
				return false;
			}

			foreach (char c in colorHexCode)
			{
				if ((c < '0' && c > '9') || (c < 'a' && c > 'f'))
				{
					return false;
				}
			}

			return true;
		}
		/// <summary>
		/// .Net의 Color로 변환합니다.
		/// </summary>
		public Color GetColor()
		{
			return Color.FromArgb(R, G, B);
		}
		public override string ToString()
		{
			return $"{R.ToString("X2")}{G.ToString("X2")}{B.ToString("X2")}";
		}
	}
}
