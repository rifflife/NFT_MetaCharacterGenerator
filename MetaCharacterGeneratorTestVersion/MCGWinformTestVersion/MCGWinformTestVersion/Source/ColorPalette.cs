using System;
using System.Collections.Generic;
using System.IO;

namespace MCG
{
	class ColorPalette
	{
		private readonly string mColorCodePaletteFileName = "color_palette.txt";
		private string mResourcePath;
		private Dictionary<CharacterPart, List<ColorCode>> mPartPalette;

		public ColorPalette(string resourcePath)
		{
			parsePaletteCode(resourcePath, mColorCodePaletteFileName);
		}

		#region Private

		private void parsePaletteCode(string path, string fileName)
		{
			// Initialize character parts palette
			mPartPalette = new Dictionary<CharacterPart, List<ColorCode>>();

			// Make file stream
			string filePath = Path.Combine(path, fileName);
			StreamReader sr = new StreamReader(new FileStream(filePath, FileMode.Open));

			// FSM
			List<CharacterPart> currentPartTypes = new List<CharacterPart>();

			while (!sr.EndOfStream)
			{
				string data = sr.ReadLine().Trim();

				// Empty
				if (string.IsNullOrEmpty(data))
				{
					continue;
				}

				// Check Properties
				if (data[0] == '[' && data[data.Length - 1] == ']')
				{
					currentPartTypes.Clear();

					string partTypes = data.Substring(1, data.Length - 2);

					var partTypeTokens = partTypes.Split(',', StringSplitOptions.RemoveEmptyEntries);

					foreach (string type in partTypeTokens)
					{
						var curType = Enum.Parse<CharacterPart>(type.Trim());
						currentPartTypes.Add(curType);
					}

					continue;
				}

				var tokens = data.Split(' ', StringSplitOptions.TrimEntries);

				// Parse tokens
				foreach (string token in tokens)
				{
					// Color Hex Code
					if (ColorCode.IsColorHexCode(token))
					{
						ColorCode color = new ColorCode(token);
						addColor(currentPartTypes, color);
						continue;
					}

					// Commment or nothing
					break;
				}
			}

			sr.Close();
		}
		private void addColor(List<CharacterPart> partTypeList, ColorCode color)
		{
			foreach (var type in partTypeList)
			{
				if (!mPartPalette.ContainsKey(type))
				{
					List<ColorCode> newColorPalette = new List<ColorCode>() { color };
					mPartPalette.Add(type, newColorPalette);
				}
				else
				{
					mPartPalette[type].Add(color);
				}
			}
		}

		#endregion

		#region Public

		public object[] GetColors(CharacterPart type)
		{
			if (mPartPalette.ContainsKey(type))
			{
				List<object> colors = new List<object>();

				foreach (var c in mPartPalette[type])
				{
					colors.Add(c);
				}

				return colors.ToArray();
			}
			
			return null;
		}
		/// <summary>
		/// 해당 캐릭터 부위의 색상을 반환받습니다. 인덱스가 넘어서면 모듈러 연산을 수행합니다.
		/// </summary>
		/// <param name="type">부위</param>
		/// <param name="index">색상 인덱스</param>
		/// <returns>색상, 없는 부위인 경우 검정 색상 반환</returns>
		public ColorCode GetColor(CharacterPart type, int index)
		{
			if (mPartPalette.ContainsKey(type))
			{
				index = index % mPartPalette[type].Count;
				return mPartPalette[type][index];
			}

			return new ColorCode();
		}
		public int GetColorCount(CharacterPart type)
		{
			if (mPartPalette.ContainsKey(type))
			{
				return mPartPalette[type].Count;
			}

			return 0;
		}

		#endregion
	}
}