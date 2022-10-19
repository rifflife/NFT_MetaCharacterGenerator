using System.Collections.Generic;

namespace MCGCore
{
	public class NftCollectionData
	{
		public List<NftPair> NftPairList { get; private set; } = new List<NftPair>();

		public List<NftMetadata> MetadataList = new List<NftMetadata>();

		public NftCollectionData() { }

		public bool HasDNA(string dna)
		{
			var data = MetadataList.Find((m)=>m.dna == dna);
			return data == null ? false : true;
		}

		public bool HasCollection(NftMetadata metadata)
		{
			return HasDNA(metadata.dna);
		}

		public bool AddNewCollection(NftMetadata metadata)
		{
			if (HasDNA(metadata.dna))
			{
				return false;
			}

			MetadataList.Add(metadata);

			return true;
		}

	}
}
