using System.Collections.Generic;

namespace MCGCore
{
	/// 해당 Metadata는 OpenSea의 Metadata 파싱 기능을 기준으로 작성했습니다.
	public class NftMetadata
	{
		/// <summary>NFT의 이미지 URL입니다.</summary>
		public string image { get; set; }
		/// <summary>해당 NFT의 이름입니다.</summary>
		public string name { get; set; }
		/// <summary>속성 리스트입니다.</summary>
		public List<IMetadataAttribute> attribute { get; set; }

		/// <summary>컬렉션의 이름입니다.</summary>
		public string collection_name { get; set; }
		/// <summary>해당 NFT의 고유 DNA입니다.</summary>
		public string dna { get; set; }
		/// <summary>합성된 이미지의 출력 테이블 입니다. 각각의 이미지의 해쉬 값이 출력 순서대로 정렬되어 있습니다.</summary>
		public List<string> draw_table { get; set; }

		public NftMetadata() {}

		public NftMetadata Copy()
        {
			NftMetadata copy = new NftMetadata();
			copy.image = this.image;
			copy.name = this.name;
			copy.attribute = new List<IMetadataAttribute>(attribute);
			copy.collection_name = this.collection_name;
			copy.dna = this.dna;
			copy.draw_table = new List<string>(draw_table);

			return copy;
        }

		public bool HasAttributes(IMetadataAttribute attributes)
        {
			return attribute.Contains(attributes);
        }

        #region Unused Attributes

        /// <summary>해당 NFT와 관련된 외부 URL입니다.</summary>
        //public string external_url { get; set; }
        /// <summary>해당 NFT에 대해 읽을 수 있는 설명입니다. 마크다운 문법으로 작성 가능합니다.</summary>
        //public string description { get; set; }
        /// <summary>OpenSea에서 표현하는 배경 색상입니다. 6자리 16진수 색상 코드입니다.</summary>
        //public string background_color { get; set; }
        /// <summary>Youtube 비디오에 대한 URL입니다.</summary>
        //public string youtube_url { get; set; }
        /// <summary>생성된 프로그램의 이름입니다.</summary>
        //public string generated_program { get; set; }
        /// <summary>저자의 이름입니다.</summary>
        //public string author_name { get; set; }

        #endregion
    }
}