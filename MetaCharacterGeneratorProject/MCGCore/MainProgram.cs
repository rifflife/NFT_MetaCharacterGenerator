using System;
using System.Collections.Generic;
using Newtonsoft.Json;

using MCGCore;
using System.IO;

static class MainProgram
{
	public static void Main()
	{
		string inputEnvironmentPath;
		string folderName;
		string mcgProjectDirectory;

		while (true)
        {
			Console.WriteLine("프로젝트 경로 입력 : ");
			inputEnvironmentPath = Console.ReadLine();
			Console.WriteLine("프로젝트 폴더 입력 : ");
			folderName = Console.ReadLine();

			mcgProjectDirectory = Path.Combine(inputEnvironmentPath, folderName);

			if (Directory.Exists(mcgProjectDirectory))
            {
				EnvironmentPath.McgEnvironmentPath = inputEnvironmentPath;
				EnvironmentPath.McgProjectFolderName = folderName;

				break;
            }

			Console.Clear();
			Console.WriteLine("잘못된 프로젝트 경로입니다.");
			Console.WriteLine("다시 입력해주십시오.");
        }

		ExcelReader reader = new ExcelReader(Path.Combine(EnvironmentPath.McgProjectPath, StaticFileName.ExcelIndex));

		var project = ExcelParser.ParseMcgProject(reader);

		project.Editor.DrawLayerHierarchy();
		project.CheckAndCreateProjectFolders();

		Console.WriteLine();
		Console.WriteLine("Start generate process!");
		Console.WriteLine();

		McgGenerator.TryGenerateNftCollection(project);

		Console.Read();

		//McgProject p;

		//p = TestRoutine_SetProjectManually();
		//p = TestRoutine_loadProject();

		//McgGenerator.TryGenerateNftCollection(p);

		Console.Read();
	}

	public static McgProject TestRoutine_loadProject()
	{
		string projectPath = Path.Combine(@"D:\MCG Test Project");

		McgProject p = new McgProject(projectPath);

		return p;
	}

	//public static McgProject TestRoutine_SetProjectManually()
	//{
	//	string projectPath = Path.Combine(@"D:\MCG Test Project");

	//	McgProject p = new McgProject();

	//	p.ProjectConfiguration.ProjectPath = projectPath;
	//	p.ProjectConfiguration.NftCollectionName = "MyTestNFT";
	//	p.ProjectConfiguration.NftWidth = 1600;
	//	p.ProjectConfiguration.NftHeight = 1600;
	//	p.ProjectConfiguration.ShouldCacheOnMemoryWhenGenerate = false;

	//	p.Editor.SetProjectConfiguration(p.ProjectConfiguration);

	//	#region Set Attribute


	//	#endregion

	//	#region Set Elements

	//	Dictionary<string, string> eyesColor = new Dictionary<string, string>();
	//	eyesColor.Add("Cyan", "8ADDFF");
	//	eyesColor.Add("Red", "EB2741");
	//	eyesColor.Add("Purple", "8531C0");
	//	eyesColor.Add("Pink", "C167A9");

	//	Dictionary<string, string> hairColors = new Dictionary<string, string>();
	//	hairColors.Add("Cyan", "8ADDFF");
	//	hairColors.Add("Red", "EB2741");
	//	hairColors.Add("Purple", "8531C0");
	//	hairColors.Add("Pink", "C167A9");

	//	string layerName;
	//	string preElementName;
	//	string elementName;
	//	string curTraitType;
	//	string curColorType;
	//	Layer layer;


	//	// Background
	//	layerName = "Background";
	//	p.Editor.AddLayer(layerName);

	//	curTraitType = "Background";
	//	p.Editor.NftAttributeTable.AddTraitType(curTraitType, true);

	//	layer = p.Editor.FindLayerByname(layerName);
	//	//layer.AddRequiredTraitType(curTraitType);

	//	elementName = "Background Image";
	//	p.Editor.AddElement(layerName, elementName);
	//	p.Editor.AddImageToLayerElement(layerName, elementName, getAbsoluteResourcePath("static_background_fill_0.png"));
	//	//p.Editor.SetTraitAttribute(layerName, elementName, new StringTraitAttribute(curTraitType, "Basic"), true, false);


	//	// Back Hair
	//	layerName = "Back Hair";
	//	p.Editor.AddLayer(layerName);

	//	curTraitType = "Back Hair";
	//	curColorType = "Hair Color";
	//	p.Editor.NftAttributeTable.AddTraitType(curTraitType, true);
	//	p.Editor.NftAttributeTable.AddTraitType(curColorType, false);

	//	layer = p.Editor.FindLayerByname(layerName);
	//	layer.IdentityTraitType = curTraitType;

	//	for (int i = 0; i < 5; i++)
	//	{
	//		elementName = $"Back Hair {i}";
	//		p.Editor.AddElement(layerName, elementName);
	//		p.Editor.AddImageToLayerElement(layerName, elementName, getAbsoluteResourcePath($"back_hair_fill_{i}.png"));
	//		p.Editor.AddImageToLayerElement(layerName, elementName, getAbsoluteResourcePath($"back_hair_draw_{i}.png"));
	//		p.Editor.SetTraitAttribute(layerName, elementName, new StringTraitAttribute(curTraitType, $"Back Hair {i}"), true);

	//		p.Editor.BindColorToImagePart(layerName, elementName, 0, new Color32("DDD11E"));
	//		p.Editor.SetTraitAttribute(layerName, elementName, new StringTraitAttribute(curColorType, "Orange"), false);

	//		foreach (string colorName in hairColors.Keys)
	//		{
	//			var colorCode = hairColors[colorName];
	//			var copied = p.Editor.CopyElement(layerName, elementName, layerName, $"{elementName} {colorName}");
	//			p.Editor.BindColorToImagePart(layerName, copied.Name, 0, new Color32(colorCode));
	//			p.Editor.SetTraitAttribute(layerName, copied.Name, new StringTraitAttribute(curColorType, colorName), false);
	//		}
	//	}

	//	#region Body
	//	layerName = "Body";
	//	p.Editor.AddLayer(layerName);

	//	curTraitType = "Body Type";
	//	curColorType = "Skin Color";
	//	p.Editor.NftAttributeTable.AddTraitType(curTraitType, true);
	//	p.Editor.NftAttributeTable.AddTraitType(curColorType, false);

	//	layer = p.Editor.FindLayerByname(layerName);
	//	layer.IdentityTraitType = curTraitType;

	//	elementName = "Body 0";
	//	p.Editor.AddElement(layerName, elementName);
	//	p.Editor.AddImageToLayerElement(layerName, elementName, getAbsoluteResourcePath("static_face_fill_0.png"));
	//	p.Editor.AddImageToLayerElement(layerName, elementName, getAbsoluteResourcePath("static_face_draw_0.png"));
	//	p.Editor.AddImageToLayerElement(layerName, elementName, getAbsoluteResourcePath("static_cheek_base_0.png"));
	//	p.Editor.AddImageToLayerElement(layerName, elementName, getAbsoluteResourcePath("static_body_base_0.png"));
	//	p.Editor.BindColorToImagePart(layerName, elementName, 0, new Color32("FFF9EB"));
	//	p.Editor.SetTraitAttribute(layerName, elementName, new StringTraitAttribute(curTraitType, "Type 1"), true);
	//	p.Editor.SetTraitAttribute(layerName, elementName, new StringTraitAttribute(curColorType, "Bright"), false);
	//	preElementName = elementName;

	//	elementName = "Body 1";
	//	p.Editor.CopyElement(layerName, preElementName, layerName, elementName);
	//	p.Editor.BindColorToImagePart(layerName, elementName, 0, new Color32("FFE2BF"));
	//	p.Editor.SetTraitAttribute(layerName, elementName, new StringTraitAttribute(curTraitType, "Type 1"), true);
	//	p.Editor.SetTraitAttribute(layerName, elementName, new StringTraitAttribute(curColorType, "Little Bright"), false);
	//	preElementName = elementName;

	//	elementName = "Body 2";
	//	p.Editor.CopyElement(layerName, preElementName, layerName, elementName);
	//	p.Editor.BindColorToImagePart(layerName, elementName, 0, new Color32("BC9F86"));
	//	p.Editor.SetTraitAttribute(layerName, elementName, new StringTraitAttribute(curTraitType, "Type 1"), true);
	//	p.Editor.SetTraitAttribute(layerName, elementName, new StringTraitAttribute(curColorType, "Brown"), false);
	//	preElementName = elementName;

	//	elementName = "Body 3";
	//	p.Editor.CopyElement(layerName, preElementName, layerName, elementName);
	//	p.Editor.BindColorToImagePart(layerName, elementName, 0, new Color32("8D6652"));
	//	p.Editor.SetTraitAttribute(layerName, elementName, new StringTraitAttribute(curTraitType, "Type 1"), true);
	//	p.Editor.SetTraitAttribute(layerName, elementName, new StringTraitAttribute(curColorType, "Dark"), false);
	//	#endregion

	//	#region Eyes
	//	layerName = "Eyes";
	//	p.Editor.AddLayer(layerName);

	//	curTraitType = "Eyes Type";
	//	curColorType = "Eyes Color";
	//	p.Editor.NftAttributeTable.AddTraitType(curTraitType, true);
	//	p.Editor.NftAttributeTable.AddTraitType(curColorType, false);

	//	layer = p.Editor.FindLayerByname(layerName);
	//	layer.IdentityTraitType = curTraitType;

	//	for (int i = 0; i < 5; i++)
	//	{
	//		elementName = $"Eyes {i}";
	//		p.Editor.AddElement(layerName, elementName);
	//		p.Editor.AddImageToLayerElement(layerName, elementName, getAbsoluteResourcePath($"left_eye_base_{i}.png"));
	//		p.Editor.AddImageToLayerElement(layerName, elementName, getAbsoluteResourcePath($"left_eye_fill_{i}.png"));
	//		p.Editor.AddImageToLayerElement(layerName, elementName, getAbsoluteResourcePath($"left_eye_draw_{i}.png"));
	//		p.Editor.AddImageToLayerElement(layerName, elementName, getAbsoluteResourcePath($"right_eye_base_{i}.png"));
	//		p.Editor.AddImageToLayerElement(layerName, elementName, getAbsoluteResourcePath($"right_eye_fill_{i}.png"));
	//		p.Editor.AddImageToLayerElement(layerName, elementName, getAbsoluteResourcePath($"right_eye_draw_{i}.png"));
	//		p.Editor.SetTraitAttribute(layerName, elementName, new StringTraitAttribute(curTraitType, $"Type {i}"), true);
	//		p.Editor.BindColorToImagePart(layerName, elementName, 1, new Color32("DDD11E"));
	//		p.Editor.BindColorToImagePart(layerName, elementName, 4, new Color32("DDD11E"));
	//		p.Editor.SetTraitAttribute(layerName, elementName, new StringTraitAttribute(curColorType, "Orange"), false);

	//		foreach (string colorName in eyesColor.Keys)
	//		{
	//			var colorCode = eyesColor[colorName];
	//			var copied = p.Editor.CopyElement(layerName, elementName, layerName, $"{elementName} {colorName}");
	//			p.Editor.BindColorToImagePart(layerName, copied.Name, 1, new Color32(colorCode));
	//			p.Editor.BindColorToImagePart(layerName, copied.Name, 4, new Color32(colorCode));
	//			p.Editor.SetTraitAttribute(layerName, copied.Name, new StringTraitAttribute(curColorType, colorName), false);
	//		}
	//	}
	//	#endregion

	//	#region Mouth
	//	layerName = "Mouth";
	//	p.Editor.AddLayer(layerName);

	//	curTraitType = "Mouth";
	//	p.Editor.NftAttributeTable.AddTraitType(curTraitType, true);

	//	layer = p.Editor.FindLayerByname(layerName);
	//	layer.IdentityTraitType = curTraitType;

	//	for (int i = 0; i < 5; i++)
	//	{
	//		elementName = $"Mouth {i}";
	//		p.Editor.AddElement(layerName, elementName);
	//		p.Editor.AddImageToLayerElement(layerName, elementName, getAbsoluteResourcePath($"mouth_base_{i}.png"));
	//		p.Editor.SetTraitAttribute(layerName, elementName, new StringTraitAttribute(curTraitType, $"Mouth Type {i}"), true);
	//	}
	//	#endregion

	//	#region Eyebrow
	//	layerName = "Eyebrow";
	//	p.Editor.AddLayer(layerName);

	//	curTraitType = "Eyebrow";
	//	curColorType = "Hair Color";
	//	p.Editor.NftAttributeTable.AddTraitType(curTraitType, true);
	//	p.Editor.NftAttributeTable.AddTraitType(curColorType, false);

	//	layer = p.Editor.FindLayerByname(layerName);
	//	layer.IdentityTraitType = curTraitType;

	//	for (int i = 0; i < 5; i++)
	//	{
	//		elementName = $"Eyebrow {i}";
	//		p.Editor.AddElement(layerName, elementName);
	//		p.Editor.AddImageToLayerElement(layerName, elementName, getAbsoluteResourcePath($"eyebrow_fill_{i}.png"));
	//		p.Editor.AddImageToLayerElement(layerName, elementName, getAbsoluteResourcePath($"eyebrow_draw_{i}.png"));
	//		p.Editor.SetTraitAttribute(layerName, elementName, new StringTraitAttribute(curTraitType, $"Eyebrow Type {i}"), true);

	//		p.Editor.BindColorToImagePart(layerName, elementName, 0, new Color32("DDD11E"));
	//		p.Editor.SetTraitAttribute(layerName, elementName, new StringTraitAttribute(curColorType, "Orange"), false);

	//		foreach (string colorName in hairColors.Keys)
	//		{
	//			var colorCode = hairColors[colorName];
	//			var copied = p.Editor.CopyElement(layerName, elementName, layerName, $"{elementName} {colorName}");
	//			p.Editor.BindColorToImagePart(layerName, copied.Name, 0, new Color32(colorCode));
	//			p.Editor.SetTraitAttribute(layerName, copied.Name, new StringTraitAttribute(curColorType, colorName), false);
	//		}
	//	}
	//	#endregion

	//	#region Front Hair
	//	layerName = "Front Hair";
	//	p.Editor.AddLayer(layerName);

	//	curTraitType = "Front Hair";
	//	curColorType = "Hair Color";
	//	p.Editor.NftAttributeTable.AddTraitType(curTraitType, true);
	//	p.Editor.NftAttributeTable.AddTraitType(curColorType, false);

	//	layer = p.Editor.FindLayerByname(layerName);
	//	layer.IdentityTraitType = curTraitType;

	//	for (int i = 0; i < 5; i++)
	//	{
	//		elementName = $"Front Hair {i}";
	//		p.Editor.AddElement(layerName, elementName);
	//		p.Editor.AddImageToLayerElement(layerName, elementName, getAbsoluteResourcePath($"front_hair_fill_{i}.png"));
	//		p.Editor.AddImageToLayerElement(layerName, elementName, getAbsoluteResourcePath($"front_hair_draw_{i}.png"));
	//		p.Editor.SetTraitAttribute(layerName, elementName, new StringTraitAttribute(curTraitType, $"Front Hair {i}"), true);

	//		p.Editor.BindColorToImagePart(layerName, elementName, 0, new Color32("DDD11E"));
	//		p.Editor.SetTraitAttribute(layerName, elementName, new StringTraitAttribute(curColorType, "Orange"), false);

	//		foreach (string colorName in hairColors.Keys)
	//		{
	//			var colorCode = hairColors[colorName];
	//			var copied = p.Editor.CopyElement(layerName, elementName, layerName, $"{elementName} {colorName}");
	//			p.Editor.BindColorToImagePart(layerName, copied.Name, 0, new Color32(colorCode));
	//			p.Editor.SetTraitAttribute(layerName, copied.Name, new StringTraitAttribute(curColorType, colorName), false);
	//		}
	//	}
	//	#endregion

	//	#endregion

	//	p.Editor.DrawLayerHierarchy();

	//	p.TrySaveProject();

	//	return p;

	//	//p.ClearUnnecessaryResourcesFiles();
	//}

	public static void TestRoutine_JSON()
	{
		List<IMetadataAttribute> attributes = new List<IMetadataAttribute>()
		{
			{ new StringTraitAttribute("Head", "Triangle") },
			{ new StringTraitAttribute("Head", "Square") },
			{ new IntTraitAttribute("Head", 12) },
			{ new StringTraitAttribute("Head", "ddd") },
		};

		List<IMetadataAttribute> attributes2 = new List<IMetadataAttribute>()
		{
			{ new StringTraitAttribute("Head", "ddd") },
			{ new StringTraitAttribute("Head", "Square") },
			{ new StringTraitAttribute("Head", "Triangle") },
			{ new IntTraitAttribute("Head", 12) },
		};


		string hashCode = HashManager.GetHeshStringOrNull(attributes);
		string hashCode2 = HashManager.GetHeshStringOrNull(attributes2);

		Console.WriteLine(hashCode);
		Console.WriteLine(hashCode2);


		//NftDNA dna = new NftDNA(attributes);

		//string json = JsonHandler.ToJson(dna);

		//var parsed = JsonHandler.ToInstance<NftDNA>(json);

		//Console.WriteLine(JsonHandler.ToJson(parsed));
	}

	private static string getAbsoluteResourcePath(string fileName)
	{
		return $@"D:\StaticResources\KimMyeongseop\CharacterParts\{fileName}";
	}
}