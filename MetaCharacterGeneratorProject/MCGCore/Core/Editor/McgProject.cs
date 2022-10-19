using System;
using System.Collections.Generic;
using System.IO;

namespace MCGCore
{
	public class McgProject
	{
		public string ProjectPath { get; set; }
		public McgProjectConfiguration ProjectConfiguration { get; set; }
		public McgEditor Editor { get; set; }
		public NftCollectionData NftCollectionData { get; private set; }

		public McgProject()
		{
			ProjectPath = Directory.GetCurrentDirectory();
			ProjectConfiguration = new McgProjectConfiguration(ProjectPath);
			NftCollectionData = new NftCollectionData();
			Editor = new McgEditor();
			Editor.SetProjectConfiguration(ProjectConfiguration);
		}

		public McgProject(string projectPath)
		{
			ProjectPath = projectPath;

			// Load project configuration
			string configPath = Path.Combine(ProjectPath, StaticFileName.ProjectConfiguration);
			ProjectConfiguration = JsonHandler.LoadFromFile<McgProjectConfiguration>(new Uri(configPath)) ?? new McgProjectConfiguration();
			CheckAndCreateProjectFolders();

			// Load MCG editor
			string mcgEditor = Path.Combine(ProjectConfiguration.ProjectPath, StaticFileName.Editor);
			Editor = JsonHandler.LoadFromFile<McgEditor>(new Uri(mcgEditor)) ?? new McgEditor();
			Editor.SetProjectConfiguration(ProjectConfiguration);

			// Load NFT collection data
			string nftCollectionData = Path.Combine(ProjectConfiguration.ProjectPath, StaticFileName.NftCollectionData);
			NftCollectionData = JsonHandler.LoadFromFile<NftCollectionData>(new Uri(nftCollectionData)) ?? new NftCollectionData();

			ClearUnnecessaryResourcesFiles();
		}

		public bool TrySaveProject()
		{
			// Save project configuration
			string configPath = Path.Combine(ProjectConfiguration.ProjectPath, StaticFileName.ProjectConfiguration);
			JsonHandler.SaveToFile(ProjectConfiguration, new Uri(configPath));

			// Save MCG editor
			string mcgEditor = Path.Combine(ProjectConfiguration.ProjectPath, StaticFileName.Editor);
			JsonHandler.SaveToFile(Editor, new Uri(mcgEditor));

			// Save NFT collection data
			string nftCollectionData = Path.Combine(ProjectConfiguration.ProjectPath, StaticFileName.NftCollectionData);
			JsonHandler.SaveToFile(NftCollectionData, new Uri(nftCollectionData));

			return true;
		}

		/// <summary>Project Resources에 불필요한 파일들을 제거합니다.</summary>
		public void ClearUnnecessaryResourcesFiles()
		{
			// Remove resources files
			List<string> programImagePath = new List<string>();
			foreach (Layer layer in Editor.Layers)
			{
				foreach (LayerElement e in layer.LayerElements)
				{
					foreach (ImagePart i in e.ImageParts)
					{
						string imagePath = Path.Combine(ProjectConfiguration.ResourcePath, i.ImageRelativePath);
						programImagePath.Add(imagePath);
					}
				}
			}

			var resourcesFiles = Directory.GetFiles(ProjectConfiguration.ResourcePath);

			foreach (string existFilePath in resourcesFiles)
			{
				if (!programImagePath.Contains(existFilePath))
				{
					File.Delete(existFilePath);
				}
			}

			//// Remove collection
			//var collectionFiles = Directory.GetFiles(ProjectConfiguration.CollectionPath);

			//foreach (string files in collectionFiles)
			//{
			//	string extension = Path.GetExtension(files).ToLower();

			//	if (extension == "json" || extension == "png")
			//	{
			//		File.Delete(files);
			//	}
			//}
		}

		public void CheckAndCreateProjectFolders()
		{
			Directory.CreateDirectory(ProjectConfiguration.CachePath);
			Directory.CreateDirectory(ProjectConfiguration.ResourcePath);
			Directory.CreateDirectory(ProjectConfiguration.CollectionPath);
		}

	}
}
