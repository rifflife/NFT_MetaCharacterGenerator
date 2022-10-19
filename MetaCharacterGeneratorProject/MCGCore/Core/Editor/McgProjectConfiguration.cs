using System.IO;

namespace MCGCore
{
	public class McgProjectConfiguration
	{
		public string ProjectName { get; set; }
		public string Version { get; } = ProgramConfiguration.Version;
		public string NftCollectionName { get; set; }
		public string ProjectPath { get; set; }
		public string CollectionPath => Path.Combine(ProjectPath, StaticPath.ColectionPath);
		public string ResourcePath => Path.Combine(ProjectPath, StaticPath.Resources);
		public string CachePath => Path.Combine(ProjectPath, StaticPath.Cache);
		public int Seed { get; set; } = 100;

		public int NftWidth = 1024;
		public int NftHeight = 1024;
		public int GenerationCount = 100;

		public bool ShouldCacheOnMemoryWhenGenerate { get; set; } = false;
		public string ImageFileExtension { get; set; } = ".png";

		public McgProjectConfiguration() {}

		public McgProjectConfiguration(string projectPath)
		{
			ProjectPath = projectPath;
		}
	}
}
