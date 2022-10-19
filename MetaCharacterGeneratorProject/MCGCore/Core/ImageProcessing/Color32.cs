using System;
using System.Runtime.InteropServices;

namespace MCGCore
{
	/// <summary>
	/// BGRA 32비트 색상 자료형입니다.
	/// </summary>
	[StructLayout(LayoutKind.Explicit)]
	public struct Color32 : IEquatable<Color32>
	{
		[FieldOffset(0)] public UInt32 ColorBGRA;

		[FieldOffset(0)] public byte B;
		[FieldOffset(1)] public byte G;
		[FieldOffset(2)] public byte R;
		[FieldOffset(3)] public byte A;

		public float FB => B / 255F;
		public float FG => G / 255F;
		public float FR => R / 255F;
		public float FA => A / 255F;

		public static Color32 White => new Color32(255, 255, 255);
		public static Color32 Black => new Color32(0, 0, 0);
		public static Color32 Blue => new Color32(255, 0, 0);
		public static Color32 Green => new Color32(0, 255, 0);
		public static Color32 Red => new Color32(0, 0, 255);

		public static readonly int Stride = 4;

		public Color32(int colorBGRA) : this() => ColorBGRA = (UInt32)colorBGRA;

		/// <summary> 0에서 255사이의 값으로 색상을 생성합니다. </summary>
		public Color32(int blue, int green, int red, int alpha = 255) : this()
		{
			B = (byte)MathExtension.Clamp(blue, 0, 255);
			G = (byte)MathExtension.Clamp(green, 0, 255);
			R = (byte)MathExtension.Clamp(red, 0, 255);
			A = (byte)MathExtension.Clamp(alpha, 0, 255);
		}

		/// <summary> 0에서 1사이의 값으로 색상을 생성합니다. </summary>
		public Color32(float blue, float green, float red, float alpha = 1) : this()
		{
			B = (byte)MathExtension.Clamp(blue, 0, 1).Remap((0, 1), (0, 255));
			G = (byte)MathExtension.Clamp(green, 0, 1).Remap((0, 1), (0, 255));
			R = (byte)MathExtension.Clamp(red, 0, 1).Remap((0, 1), (0, 255));
			A = (byte)MathExtension.Clamp(alpha, 0, 1).Remap((0, 1), (0, 255));
		}

		/// <summary>16진수 색상 코드로 초기화 합니다.</summary>
		/// <param name="hexCode">16진수 색상 코드</param>
		/// <param name="isBGR">false인 경우 RGB 순서로 파싱합니다. true인 경우 BGR순서로 파싱합니다.</param>
		public Color32(string hexCode, bool isBGR = true) : this()
		{
			if (hexCode.Length < 6)
			{
				this = White;
			}

			if (hexCode[0] == '#')
			{
				hexCode = hexCode.Substring(1);
			}

			// BGR
			if (isBGR)
			{
				ColorBGRA = (UInt32)int.Parse(hexCode, System.Globalization.NumberStyles.HexNumber);
			}
			// RGB
			else
			{
				string bgrHexCode = $"{hexCode[4]}{hexCode[5]}{hexCode[2]}{hexCode[3]}{hexCode[0]}{hexCode[1]}";
				ColorBGRA = (UInt32)int.Parse(hexCode, System.Globalization.NumberStyles.HexNumber);
			}
		}

		public Color32(in byte[] stream, int x, int y, int stride) : this()
		{
			int index = y * stride + x * 4;
			B = stream[index++];
			G = stream[index++];
			R = stream[index++];
			A = stream[index];
		}

		public Color32(in Color32 copy) : this()
		{
			B = copy.B;
			G = copy.G;
			R = copy.R;
			A = copy.A;
		}

		public static Color32 operator *(Color32 lhs, Color32 rhs)
		{
			return new Color32(lhs.B * rhs.FB, lhs.G * rhs.FG, lhs.R * rhs.FR);
		}

		public static Color32 operator +(Color32 lhs, Color32 rhs)
		{
			return new Color32(
				MathExtension.Clamp(lhs.B + rhs.B, 0, 255),
				MathExtension.Clamp(lhs.B + rhs.B, 0, 255),
				MathExtension.Clamp(lhs.B + rhs.B, 0, 255),
				MathExtension.Clamp(lhs.B + rhs.B, 0, 255));
		}

		public static Color32 operator -(Color32 lhs, Color32 rhs)
		{
			return new Color32(
				MathExtension.Clamp(lhs.B - rhs.B, 0, 255),
				MathExtension.Clamp(lhs.B - rhs.B, 0, 255),
				MathExtension.Clamp(lhs.B - rhs.B, 0, 255),
				MathExtension.Clamp(lhs.B - rhs.B, 0, 255));
		}

		public static bool operator ==(Color32 lhs, Color32 rhs)
		{
			return ((lhs.B == rhs.B) && (lhs.G == rhs.G) && (lhs.R == rhs.R) && (lhs.A == rhs.A));
		}

		public static bool operator !=(Color32 lhs, Color32 rhs)
		{
			return !((lhs.B == rhs.B) && (lhs.G == rhs.G) && (lhs.R == rhs.R) && (lhs.A == rhs.A));
		}

		public bool Equals(Color32 other)
		{
			return (B == other.B && G == other.G && R == other.R && A == other.A);
		}

		public override string ToString() => ColorBGRA.ToString("X");
	}
}