namespace MCGCore
{
	public interface IMetadataAttribute
	{
		string trait_type { get; set; }
		int chance { get; set; }
		byte[] GetHeshBytes();
		string GetHeshString();
		IMetadataAttribute Copy();
	}
}