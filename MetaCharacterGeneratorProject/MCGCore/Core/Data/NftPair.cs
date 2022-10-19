namespace MCGCore
{
	public class NftPair
	{
		public string MetadataPath { get; set; }
		public string ImagePath { get; set; }

		public NftPair() {}
		public NftPair(string metadataPath, string imagePath)
		{
			MetadataPath = metadataPath;
			ImagePath = imagePath;
		}
	}
}
