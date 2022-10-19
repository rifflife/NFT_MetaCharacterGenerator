using System;
using System.Collections.Generic;

namespace MCG
{
	static class PartCodeManager
	{
		public static readonly string ImageExtension = ".png";

		public static readonly string Fill = "fill";
		public static readonly string Draw = "draw";
		public static readonly string Base = "base";

		public static readonly string Background = "static_background";
		public static readonly string Body = "static_body";
		public static readonly string Cheek = "static_cheek";
		public static readonly string Face = "static_face";

		public static readonly string LeftEye = "left_eye";
		public static readonly string RightEye = "right_eye";
		public static readonly string Eyebrow = "eyebrow";
		public static readonly string Mouth = "mouth";
		public static readonly string FrontHair = "front_hair";
		public static readonly string SideHair = "side_hair";
		public static readonly string BackHair = "back_hair";

		private static Dictionary<string, CharacterPart> mPartMatchTable = new Dictionary<string, CharacterPart>()
		{
			{ Background,      CharacterPart.Background },
			{ Body,      CharacterPart.Body },
			{ Cheek,      CharacterPart.Cheek },
			{ Face,      CharacterPart.Face },

			{ LeftEye,      CharacterPart.LeftEye },
			{ RightEye,     CharacterPart.RightEye },
			{ Eyebrow,      CharacterPart.Eyebrow },
			{ Mouth,        CharacterPart.Mouth },
			{ FrontHair,    CharacterPart.FrontHair },
			{ SideHair,     CharacterPart.SideHair },
			{ BackHair,     CharacterPart.BackHair },
		};
		private static Dictionary<string, DrawMode> mDrawModeMatchTable = new Dictionary<string, DrawMode>()
		{
			{ Base, DrawMode.Base},
			{ Fill, DrawMode.Fill},
			{ Draw, DrawMode.Draw},
		};

		public static bool IsImageFile(string pathFileName)
		{
			return pathFileName.Contains(ImageExtension);
		}
		/// <summary>
		/// 파일 이름에서 인덱스를 파싱합니다.
		/// </summary>
		public static int ParseIndexFromFileName(string pathFileName)
		{
			var fileTokens = pathFileName.Split('_', StringSplitOptions.TrimEntries);
			string indexString = fileTokens[fileTokens.Length - 1].Split('.')[0];
			return int.Parse(indexString);
		}
		/// <summary>
		/// 파일 이름에서 캐릭터 부위 enum을 파싱합니다.
		/// </summary>
		public static CharacterPart ParseCharacterPartFromFileName(string fileName)
		{
			foreach (string part in mPartMatchTable.Keys)
			{
				if (fileName.Contains(part))
				{
					return mPartMatchTable[part];
				}
			}
			return CharacterPart.None;
		}
		/// <summary>
		/// 파일 이름에서 출력 속성을 파싱합니다.
		/// </summary>
		public static DrawMode ParseDrawModeFromFileName(string fileName)
		{
			foreach (string mode in mDrawModeMatchTable.Keys)
			{
				if (fileName.Contains(mode))
				{
					return mDrawModeMatchTable[mode];
				}
			}
			return DrawMode.None;
		}
	}
}
