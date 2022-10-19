using System.Collections.Generic;

namespace MCGCore
{
	public class NftDNA
	{
		public List<IMetadataAttribute> mAttributes { get; set; }

		public NftDNA() { }

		public NftDNA(List<IMetadataAttribute> attributes)
		{
			mAttributes = new List<IMetadataAttribute>(attributes);
		}

		public void AddRange(IEnumerable<IMetadataAttribute> attributes)
		{
			mAttributes.AddRange(attributes);
		}

		public void Add(IMetadataAttribute attribute)
		{
			mAttributes.Add(attribute);
		}
	}
}
