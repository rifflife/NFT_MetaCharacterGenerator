using System;
using System.Collections.Generic;
using System.Globalization;

namespace MCGCore
{
	/// <summary>생성된 Layer Element 들의 ID를 저장해 매칭 테이블을 생성합니다. ID 매칭 기능을 제공합니다.</summary>
	public class IdMatchTable
	{
		private NftAttributeTable mNftAttributeTable;
		/// <summary>가장 첫 Index를 기준으로 분류합니다..</summary>
		private List<(string ID, List<string> ElementIdList)> mSortedIdTable = new List<(string ID, List<string> ElementIdList)>();

		public IdMatchTable(NftAttributeTable table)
		{
			mNftAttributeTable = table;
		}

		/// <summary>Layer Element의 ID를 저장합니다.</summary>
		/// <param name="layerElementID">저장할 Layer Element의 ID 입니다.</param>
		public void StoreLayerElementID(string layerElementID)
		{
			// 저장되는 모든 ID는 반드시 정렬되어야 한다.
			if (!mNftAttributeTable.IsSorted(layerElementID))
			{
				layerElementID = mNftAttributeTable.SortID(layerElementID);
			}
			
			if (mSortedIdTable.FindIndex((i) => i.ID == layerElementID) < 0)
			{
				var elementIdList = mNftAttributeTable.GetElementIdList(layerElementID);
				mSortedIdTable.Add((layerElementID, elementIdList));
			}
		}

		/// <summary>ID Stream을 기준으로 매칭되는 Layer Element들의 ID 리스트를 반환합니다. 조합시 사용할 수 있습니다.</summary>
		/// <param name="nftDNA">파싱할 NFT DNA 입니다. Layer Element의 ID 조합으로 이루어져 있어야 합니다.</param>
		/// <returns>Layer Element들의 ID 리스트입니다.</returns>
		public List<string> ParseToIdList(string nftDNA)
		{
			List<string> parsedIdList = new List<string>();

			// 파싱할 ID는 반드시 정렬되어야 한다.
			if (!mNftAttributeTable.IsSorted(nftDNA))
			{
				nftDNA = mNftAttributeTable.SortID(nftDNA);
			}

			int idStride = mNftAttributeTable.IdStride;
			int idStreamCount = nftDNA.Length;

			// 파싱한 ID를 Element별로 분해하기
			List<string> elementIdList = mNftAttributeTable.GetElementIdList(nftDNA);
			List<string> tempMatchedElementList = new List<string>();

			// 분해된 ID를 매칭 시도한다.
			foreach (var i in mSortedIdTable)
			{
				bool isMatched = true;

				tempMatchedElementList.Clear();

				foreach (string elementId in i.ElementIdList)
				{
					if (!elementIdList.Contains(elementId))
					{
						isMatched = false;
						break;
					}

					tempMatchedElementList.Add(elementId);
				}

				if (isMatched)
				{
					foreach (string remove in tempMatchedElementList)
					{
						elementIdList.Remove(remove);
					}
					parsedIdList.Add(i.ID);
				}
			}

			return parsedIdList.Count != 0 ? parsedIdList : null;
		}

		//private int getFirstTraitTypeIndex(string idStream)
		//{
		//	string traitTypeString = idStream.Substring(0, mNftAttributeTable.TraitTypeStride);
		//	if (int.TryParse(traitTypeString, NumberStyles.HexNumber, null, out int result))
		//	{
		//		return result;
		//	}
		//	else
		//	{
		//		return -1;
		//	}
		//}
	}
}
