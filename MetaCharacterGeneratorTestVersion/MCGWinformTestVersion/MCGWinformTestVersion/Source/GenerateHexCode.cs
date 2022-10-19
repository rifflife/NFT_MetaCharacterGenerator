using System;
using System.Collections.Generic;

namespace MCG
{
	class GenerateHexCode
	{
		public const short ProgramNumber = 20001;
		private Dictionary<CharacterPart, byte> mPartIndices = new Dictionary<CharacterPart, byte>();
		private Dictionary<CharacterPart, ColorCode> mColorCodes = new Dictionary<CharacterPart, ColorCode>();

		public GenerateHexCode() { }
		public GenerateHexCode(string hexCode)
		{
			SetByHexCode(hexCode);
		}
		public void SetByHexCode(string hexCode)
		{
			// Program Number
			int index = 4;

			// Background
			SetColorCode(CharacterPart.Background, parseColorCode(hexCode, ref index));

			// Skin
			SetColorCode(CharacterPart.Face, parseColorCode(hexCode, ref index));

			// Eyes
			byte eyeType = parseByte(hexCode, ref index);
			SetIndex(CharacterPart.LeftEye, eyeType);
			SetIndex(CharacterPart.RightEye, eyeType);
			SetColorCode(CharacterPart.LeftEye, parseColorCode(hexCode, ref index));
			SetColorCode(CharacterPart.RightEye, parseColorCode(hexCode, ref index));

			// Eyebrow
			SetIndex(CharacterPart.Eyebrow, parseByte(hexCode, ref index));
			SetColorCode(CharacterPart.Eyebrow, parseColorCode(hexCode, ref index));

			// Mouth
			SetIndex(CharacterPart.Mouth, parseByte(hexCode, ref index));

			// Hair
			SetIndex(CharacterPart.FrontHair, parseByte(hexCode, ref index));
			SetIndex(CharacterPart.BackHair, parseByte(hexCode, ref index));
			var hairColor = parseColorCode(hexCode, ref index);
			SetColorCode(CharacterPart.FrontHair, hairColor);
			SetColorCode(CharacterPart.BackHair, hairColor);
		}
		public override string ToString()
		{
			return
				$"{ProgramNumber.ToString("X4")}" +
				// Background
				$"{getColorHexCode(CharacterPart.Background)}" +

				// Skin
				$"{getColorHexCode(CharacterPart.Face)}" +

				// Eyes
				$"{getByteHexCode(CharacterPart.LeftEye)}" +
				$"{getColorHexCode(CharacterPart.LeftEye)}" +
				$"{getColorHexCode(CharacterPart.RightEye)}" +

				// Eyebrow
				$"{getByteHexCode(CharacterPart.Eyebrow)}" +
				$"{getColorHexCode(CharacterPart.Eyebrow)}" +

				// Mouth
				$"{getByteHexCode(CharacterPart.Mouth)}" +

				// Hair
				$"{getByteHexCode(CharacterPart.FrontHair)}" +
				$"{getByteHexCode(CharacterPart.BackHair)}" +
				$"{getColorHexCode(CharacterPart.FrontHair)}";
		}
		public byte GetIndex(CharacterPart part)
		{
			byte value;

			if (mPartIndices.TryGetValue(part, out value))
			{
				return value;
			}

			return 0;
		}
		public void SetIndex(CharacterPart part, byte index)
		{
			if (mPartIndices.ContainsKey(part))
			{
				mPartIndices[part] = index;
			}
			else
			{
				mPartIndices.Add(part, index);
			}
		}
		public void SetIndex(CharacterPart part, string hexCode)
		{
			byte index;

			try
			{
				index = byte.Parse(hexCode, System.Globalization.NumberStyles.HexNumber);
			}
			catch
			{
				index = 0;
			}

			SetIndex(part, index);
		}
		public ColorCode GetColorCode(CharacterPart part)
		{
			ColorCode colorCode;

			if (mColorCodes.TryGetValue(part, out colorCode))
			{
				return colorCode;
			}

			return new ColorCode();
		}
		public void SetColorCode(CharacterPart part, ColorCode color)
		{
			if (mColorCodes.ContainsKey(part))
			{
				mColorCodes[part] = color;
			}
			else
			{
				mColorCodes.Add(part, color);
			}
		}
		public void SetColorCode(CharacterPart part, string hexCode)
		{
			SetColorCode(part, new ColorCode(hexCode));
		}
		public void SetRandomCodeViaData(CharacterGenerator characterGenerator, ColorPalette colorPalette, bool makeOddEye)
		{
			Random random = new Random();

			// Background
			SetColorCode(CharacterPart.Background, getRandomColorCode(CharacterPart.Background));

			// Skin
			SetColorCode(CharacterPart.Face, getRandomColorCode(CharacterPart.Face));

			// Eyes
			byte eyeIndex = getRandomIndexCode(CharacterPart.LeftEye);
			SetIndex(CharacterPart.LeftEye, eyeIndex);
			SetIndex(CharacterPart.RightEye, eyeIndex);

			if (makeOddEye)
			{
				// Generater Random Odd Eyes
				SetColorCode(CharacterPart.LeftEye, getRandomColorCode(CharacterPart.LeftEye));
				SetColorCode(CharacterPart.RightEye, getRandomColorCode(CharacterPart.LeftEye));
			}
			else
			{
				ColorCode eyeColor = getRandomColorCode(CharacterPart.LeftEye);
				SetColorCode(CharacterPart.LeftEye, eyeColor);
				SetColorCode(CharacterPart.RightEye, eyeColor);
			}

			// Eyebrow
			SetIndex(CharacterPart.Eyebrow, getRandomIndexCode(CharacterPart.Eyebrow));
			SetColorCode(CharacterPart.Eyebrow, getRandomColorCode(CharacterPart.Eyebrow));

			// Mouth
			SetIndex(CharacterPart.Mouth, getRandomIndexCode(CharacterPart.Mouth));

			// Hair
			ColorCode hairColor = getRandomColorCode(CharacterPart.FrontHair);
			SetIndex(CharacterPart.BackHair, getRandomIndexCode(CharacterPart.BackHair));
			SetIndex(CharacterPart.FrontHair, getRandomIndexCode(CharacterPart.FrontHair));
			SetColorCode(CharacterPart.FrontHair, hairColor);
			SetColorCode(CharacterPart.BackHair, hairColor);

			ColorCode getRandomColorCode(CharacterPart type)
			{
				int colorCount = characterGenerator.GetColorCount(type);
				int randomIndex = random.Next(colorCount);
				return colorPalette.GetColor(type, randomIndex);
			}
			byte getRandomIndexCode(CharacterPart type)
			{
				int typeCount = characterGenerator.GetPartCount(type);
				return (byte)random.Next(typeCount);
			}
		}

		private string getByteHexCode(CharacterPart type)
		{
			return mPartIndices[type].ToString("X2");
		}
		private string getColorHexCode(CharacterPart type)
		{
			return mColorCodes[type].ToString();
		}
		private byte parseByte(string hexCode, ref int parserIndex)
		{
			string byteHex = hexCode.Substring(parserIndex, 2);
			parserIndex += 2;
			return byte.Parse(byteHex, System.Globalization.NumberStyles.HexNumber);
		}
		private ColorCode parseColorCode(string hexCode, ref int parserIndex)
		{
			string colorHex = hexCode.Substring(parserIndex, 6);
			parserIndex += 6;
			return new ColorCode(colorHex);
		}
	}
}
