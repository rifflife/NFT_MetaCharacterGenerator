using System.IO;

namespace MCGCore
{
	public static class StaticPath
	{
		public static readonly string ColectionPath = "Collection";
		public static readonly string Resources = "Resources";
		public static readonly string Cache = "Result";
		public static readonly string Image = "Image";
	}

	public static class StaticFileName
	{
		public static readonly string ProjectConfiguration = "project_configuration.json";
		public static readonly string Editor = "editor.json";
		public static readonly string NftCollectionData = "nft_collection_data.json";
		public static readonly string ExcelIndex = "Index.xlsx";
	}

	public static class EnvironmentPath
    {
		public static string McgEnvironmentPath = @"..\..\..\..\Documents\";
		public static string McgProjectFolderName = "";
		public static string McgProjectPath => Path.Combine(McgEnvironmentPath, McgProjectFolderName);
    }
}
