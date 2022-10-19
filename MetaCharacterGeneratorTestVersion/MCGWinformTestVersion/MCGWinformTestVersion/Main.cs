using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace MCG
{
	public partial class Main : Form
	{
		
		private const string mResourcesDirectory = @"..\..\..\..\Resources\KimMyeongseop";
		private const string mResultDirectory = @"..\..\..\..\Result";
		private const string mCharacterPartsFolderName = "CharacterParts";
		private const string mSettingFolderName = @"Setting";
		private const string mMCGWinformTestVersionFolderName = "MCGWinformTestVersion";

		private GenerateHexCode mEditorGenerateHexCode = new GenerateHexCode();

		private CharacterGenerator mCharacterGenerator;
		private ColorPalette mColorPalette;

		private bool mIsRunning = false;

		public Main()
		{
			InitializeComponent();

			// Initialize Generator
			string colorPalettePath = Path.Combine(mResourcesDirectory, mSettingFolderName);
			mColorPalette = new ColorPalette(colorPalettePath);

			string characterPartsSourcePath = Path.Combine(mResourcesDirectory, mCharacterPartsFolderName);
			string generatedCharacterPath = Path.Combine(mResultDirectory, mMCGWinformTestVersionFolderName);
			mCharacterGenerator = new CharacterGenerator(characterPartsSourcePath, generatedCharacterPath, mColorPalette);

			// Background
			AddColorRange(Selector_BackgroundColor, CharacterPart.Background);

			// Skin
			AddColorRange(Selector_FaceColor, CharacterPart.Face);

			// Eyes
			AddIndexRange(Selector_EyeType, CharacterPart.LeftEye);
			AddColorRange(Selector_LeftEyeColor, CharacterPart.LeftEye);
			AddColorRange(Selector_RightEyeColor, CharacterPart.RightEye);

			// Eyebrow
			AddIndexRange(Selector_EyebrowType, CharacterPart.Eyebrow);
			AddColorRange(Selector_EyebrowColor, CharacterPart.Eyebrow);

			// Mouth
			AddIndexRange(Selector_MouthType, CharacterPart.Mouth);

			// Hair
			AddColorRange(Selector_HairColor, CharacterPart.FrontHair);
			AddIndexRange(Selector_FrontHairType, CharacterPart.FrontHair);
			AddIndexRange(Selector_BackHairType, CharacterPart.BackHair);

			// Initialize Hex Code
			GenerateByEditor();
			Redraw();

			void AddColorRange(ComboBox comboBox, CharacterPart type)
			{
				comboBox.Items.AddRange(mColorPalette.GetColors(type));
				comboBox.SelectedIndex = 0;
			}
			void AddIndexRange(ComboBox comboBox, CharacterPart type)
			{
				int index = mCharacterGenerator.GetPartCount(type);

				for (int i = 0; i < index; i++)
				{
					comboBox.Items.Add(i);
				}
				comboBox.SelectedIndex = 0;
			}
		}

		private void FaceCanvas_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.DrawImage(mCharacterGenerator.BaseBitmap, 0, 0, FaceCanvas.Width, FaceCanvas.Height);
		}

		private void GenerateByEditor()
		{
			if (mIsRunning)
			{
				StatusLabel.Text = "현재 생성중입니다.";
				return;
			}

			// Background
			mEditorGenerateHexCode.SetColorCode(CharacterPart.Background, Selector_BackgroundColor.Text);

			// Skin
			mEditorGenerateHexCode.SetColorCode(CharacterPart.Face, Selector_FaceColor.Text);

			// Eyes
			mEditorGenerateHexCode.SetIndex(CharacterPart.LeftEye, Selector_EyeType.Text);
			mEditorGenerateHexCode.SetIndex(CharacterPart.RightEye, Selector_EyeType.Text);
			mEditorGenerateHexCode.SetColorCode(CharacterPart.LeftEye, Selector_LeftEyeColor.Text);
			mEditorGenerateHexCode.SetColorCode(CharacterPart.RightEye, Selector_RightEyeColor.Text);

			// Mouth
			mEditorGenerateHexCode.SetIndex(CharacterPart.Mouth, Selector_MouthType.Text);

			// Eyebrow
			mEditorGenerateHexCode.SetIndex(CharacterPart.Eyebrow, Selector_EyebrowType.Text);
			mEditorGenerateHexCode.SetColorCode(CharacterPart.Eyebrow, Selector_EyebrowColor.Text);
			
			// Hair
			mEditorGenerateHexCode.SetIndex(CharacterPart.FrontHair, Selector_FrontHairType.Text);
			mEditorGenerateHexCode.SetIndex(CharacterPart.BackHair, Selector_BackHairType.Text);
			mEditorGenerateHexCode.SetColorCode(CharacterPart.FrontHair, Selector_HairColor.Text);
			mEditorGenerateHexCode.SetColorCode(CharacterPart.BackHair, Selector_HairColor.Text);
		}
		private void GenerateByHexCode()
		{
			if (mIsRunning)
			{
				StatusLabel.Text = "현재 생성중입니다.";
				return;
			}

			// Background
			Selector_BackgroundColor.Text = mEditorGenerateHexCode.GetColorCode(CharacterPart.Background).ToString();

			// Skin
			Selector_FaceColor.Text = mEditorGenerateHexCode.GetColorCode(CharacterPart.Face).ToString();

			// Eyes
			Selector_EyeType.Text = mEditorGenerateHexCode.GetIndex(CharacterPart.LeftEye).ToString();
			Selector_LeftEyeColor.Text = mEditorGenerateHexCode.GetColorCode(CharacterPart.LeftEye).ToString();
			Selector_RightEyeColor.Text = mEditorGenerateHexCode.GetColorCode(CharacterPart.RightEye).ToString();

			// Mouth
			Selector_MouthType.Text = mEditorGenerateHexCode.GetIndex(CharacterPart.Mouth).ToString();

			// Eyebrow
			Selector_EyebrowType.Text = mEditorGenerateHexCode.GetIndex(CharacterPart.Eyebrow).ToString();
			Selector_EyebrowColor.Text = mEditorGenerateHexCode.GetColorCode(CharacterPart.Eyebrow).ToString();

			// Hair
			Selector_FrontHairType.Text = mEditorGenerateHexCode.GetIndex(CharacterPart.FrontHair).ToString();
			Selector_BackHairType.Text = mEditorGenerateHexCode.GetIndex(CharacterPart.BackHair).ToString();
			Selector_HairColor.Text = mEditorGenerateHexCode.GetColorCode(CharacterPart.FrontHair).ToString();
		}
		private void Redraw()
		{
			if (mIsRunning)
			{
				StatusLabel.Text = "현재 생성중입니다.";
				return;
			}

			mIsRunning = true;

			try
			{
				// 코드 재생성
				mCharacterGenerator.SetByHenerateHexCode(mEditorGenerateHexCode);
				TextBox_GeneratedCode.Text = mEditorGenerateHexCode.ToString();

				// 다시 그리기
				StatusLabel.Text = "생성중...";
				mCharacterGenerator.RedrawBitmap(() =>
				{
					FaceCanvas.Invalidate();
					StatusLabel.Text = "생성 완료.";
					mIsRunning = false;
				});
			}
			catch
			{
				StatusLabel.Text = "생성 실패.";
				mIsRunning = false;
			}
		}

		private void Button_ApplyCustomSetting_Click(object sender, EventArgs e)
		{
			if (mIsRunning)
			{
				StatusLabel.Text = "현재 생성중입니다.";
				return;
			}

			GenerateByEditor();
			Redraw();
		}

		private void Button_ApplyByHexCode_Click(object sender, EventArgs e)
		{
			if (mIsRunning)
			{
				StatusLabel.Text = "현재 생성중입니다.";
				return;
			}

			try
			{
				mEditorGenerateHexCode.SetByHexCode(TextBox_GeneratedCode.Text);
				GenerateByHexCode();
				Redraw();
			}
			catch
			{
				StatusLabel.Text = "생성 실패. 잘못된 코드입니다.";
			}
		}

		private void Button_RandomGenerate_Click(object sender, EventArgs e)
		{
			if (mIsRunning)
			{
				StatusLabel.Text = "현재 생성중입니다.";
				return;
			}

			try
			{
				mEditorGenerateHexCode.SetRandomCodeViaData(mCharacterGenerator, mColorPalette, CheckBox_OddEye.Checked);
				GenerateByHexCode();
				Redraw();
			}
			catch
			{
				StatusLabel.Text = "무작위 생성 실패. 알 수 없는 오류입니다.";
			}
		}

		private void Button_SaveImage_Click(object sender, EventArgs e)
		{
			StatusLabel.Text = "이미지 저장중";
			Task saveTask = new Task(new Action(()=>
			{ 
				mCharacterGenerator.Save();
				StatusLabel.Text = $"이미지 저장 완료. {mResultDirectory}";
			}));

			saveTask.Start();
		}
	}
}
