using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Linq;

namespace MCGCore
{
	/// <summary>
	/// 속성에 대한 테이블입니다.
	/// </summary>
	public class NftAttributeTable
	{
		/// <summary>속성 타입 리스트입니다.</summary>
		public List<TraitType> TraitTypes = new List<TraitType>();
		/// <summary>속성 타입을 Key로, 속성을 Value로 하는 테이블 입니다.</summary>
		public Dictionary<string, List<IMetadataAttribute>> AttributeTable = new Dictionary<string, List<IMetadataAttribute>>();
		public int TraitCount => TraitTypes.Count;

		private Dictionary<string, int> mFinalTriatChanceTable;
		private bool IsTraitChanceCalculated => mFinalTriatChanceTable != null;

		private Random mRandom = new Random(100);

		public static readonly string NULL_VALUE = "NULL";

		public void SetSeed(int seed)
        {
			mRandom = new Random(seed);
        }

		public void setFinalTraitChance()
		{
			mFinalTriatChanceTable = new Dictionary<string, int>();

			foreach (TraitType t in TraitTypes)
			{
				var attributeList = AttributeTable[t.Name];

				int chanceSum = 0;

				foreach (var a in attributeList)
				{
					chanceSum += a.chance;
				}

				if (chanceSum < t.ChanceRange)
				{
					chanceSum = t.ChanceRange;
				}

				mFinalTriatChanceTable.Add(t.Name, chanceSum);
			}
		}

		/// <summary>시작 인덱스 부터 시작하는 속성명 테이블입니다.</summary>
		/// <param name="startIndex">시작할 인덱스입니다.</param>
		/// <returns>속성명 테이블입니다.</returns>
		public Dictionary<string, int> GetTraitTypeByIndexTable(int startIndex)
        {
			Dictionary<string, int> indexTable = new Dictionary<string, int>();

			foreach (var t in TraitTypes)
            {
				indexTable.Add(t.Name, startIndex);
				startIndex++;
            }

			return indexTable;
        }

		/// <summary>미리 정의된 확률에 기반해서 Random 속성리스트를 반환받습니다.</summary>
		/// <returns></returns>
		public List<IMetadataAttribute> GetRandomDnaAttributes()
		{
			// Recaluclate if there is no chance table.
			if (IsTraitChanceCalculated == false)
			{
				setFinalTraitChance();
			}

			List<IMetadataAttribute> dna = new List<IMetadataAttribute>();

			foreach (var t in TraitTypes)
			{
				int chanceRange = mFinalTriatChanceTable[t.Name];
				int randomIndex = mRandom.Next(0, chanceRange);
				
				foreach (var a in AttributeTable[t.Name])
				{
					randomIndex -= a.chance;

					if (randomIndex < 0)
					{
						dna.Add(a);
						break;
					}
				}
			}

			return dna;
		}

		public bool ShouldShowOnMetadata(IMetadataAttribute attributes)
        {
			var traitType = FindTraitType(attributes.trait_type);

			if (traitType == null)
            {
				return false;
            }
			else
            {
				return traitType.ShowOnMetadata;
            }
        }

		public long GetNumberOfCombinationCases()
		{
			int possibleCase = 1;

			foreach (var attributes in AttributeTable.Values)
			{
				possibleCase *= attributes.Count;
			}

			return possibleCase;
		}

		#region Trait handling

		public TraitType FindTraitType(string traitType)
		{
			return TraitTypes.Find((t) => t.Name == traitType);
		}

		public bool IsIdentityTrait(string traitType)
		{
			var type = FindTraitType(traitType);

			if (type == null)
			{
				return false;
			}

			return type.IsIdentity;
		}

		public void AddTraitType(string typeName, bool isIdentity, bool showOnMeatadata, int chanceRange = 0)
		{
			if (HasTraitType(typeName))
			{
				return;
			}

			TraitTypes.Add(new TraitType(typeName, isIdentity, showOnMeatadata, chanceRange));

			AttributeTable[typeName] = new List<IMetadataAttribute>();
		}

		public bool HasTraitType(string traitName)
		{
			var instance = TraitTypes.Find((t) => t.Name == traitName);
			return instance != null;
		}

		private bool removeTraitType(string traitName)
		{
			int index = TraitTypes.FindIndex((t) => t.Name == traitName);

			if (index < 0)
			{
				return false;
			}

			TraitTypes.RemoveAt(index);

			return true;
		}

		public void RemoveTraitType(string typeName)
		{
			var deleteIndex = TraitTypes.FindIndex((t) => t.Name == typeName);
			TraitTypes.RemoveAt(deleteIndex);
		}

		public void MoveTraitType(int indexFrom, int indexInsertTo)
		{
			TraitTypes.InsertTo(indexFrom, indexInsertTo);
		}

		#endregion

		/// <summary> 새로운 속성을 추가합니다. </summary>
		/// <param name="attribute">추가할 속성입니다.</param>
		/// <exception cref="BindingException"/>
		/// <exception cref="LimitException"/>
		public void AddAttributes(IMetadataAttribute attribute, bool isIdentity)
		{
			string typeName = attribute.trait_type;

			if (HasTraitType(typeName))
			{
				if (AttributeTable[typeName].Contains(attribute))
				{
					// 중복되는 속성은 나중에 검사한다.
					//throw new BindingException($"\"{attribute}\" attribute already exist!");
				}
				else
				{
					//if (AttributeTable[typeName].IsMaxCount(MaxAttributeCount))
					//{
					//	throw new LimitException("Cannot create attribute anymore.");
					//}

					AttributeTable[typeName].Add(attribute);
				}
			}
			else
			{
				//if (TraitTypes.IsMaxCount(MaxTraitTypeCount))
				//{
				//	throw new LimitException("Cannot create trait type anymore.");
				//}

				TraitTypes.Add(new TraitType(typeName, isIdentity, true));
				AttributeTable.Add(typeName, new List<IMetadataAttribute>() { attribute });
			}
		}

		/// <summary> 해당 속성을 삭제합니다. </summary>
		/// <param name="attribute">삭제할 속성입니다.</param>
		public void RemoveAttribute(IMetadataAttribute attribute)
		{
			if (attribute == null)
			{
				return;
			}

			string typeName = attribute.trait_type;

			if (HasTraitType(typeName))
			{
				AttributeTable[typeName].Remove(attribute);
			}
		}

		/// <summary> 해당 속성 타입을 삭제합니다. </summary>
		/// <param name="traitTypeName">삭제할 속성 타입 이름입니다.</param>
		public void RemoveAttributeType(string traitTypeName)
		{
			if (HasTraitType(traitTypeName))
			{
				if (AttributeTable.ContainsKey(traitTypeName))
				{
					AttributeTable.Remove(traitTypeName);
				}

				removeTraitType(traitTypeName);
			}
		}

		/// <summary> 해당 속성이 존재하는지 검사합니다. </summary>
		/// <param name="attribute">검사할 속성입니다.</param>
		/// <returns>속성이 존재하면 true를, 존재하지 않으면 false를 반환합니다.</returns>
		public bool HasAttribute(IMetadataAttribute attribute)
		{
			if (attribute == null)
			{
				return false;
			}

			if (AttributeTable.TryGetValue(attribute.trait_type, out var list))
			{
				if (list.Contains(attribute))
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}

		public IMetadataAttribute GetAttributeByIndex(int traitIndex, int attributeIndex)
		{
			if (!TraitTypes.IsValidIndex(traitIndex))
			{
				return null;
			}

			string traitKey = TraitTypes[traitIndex].Name;

			if (!AttributeTable.ContainsKey(traitKey))
			{
				return null;
			}

			return AttributeTable[traitKey][attributeIndex];
		}

		public StringTraitAttribute GetAttributeByStringValue(string traitTypeName, string stringTraitValue)
		{
			TraitType traitType = FindTraitType(traitTypeName);

			if (traitType == null)
			{
				return null;
			}

			if (AttributeTable.ContainsKey(traitTypeName))
			{
				var attributes = AttributeTable[traitTypeName];

				foreach (var attribute in attributes)
				{
					if (attribute is StringTraitAttribute)
					{
						var a = attribute as StringTraitAttribute;
						if (a.value == stringTraitValue)
						{
							return a;
						}
					}
				}
			}

			return null;
		}

		public bool IsValidAttributes(IEnumerable<IMetadataAttribute> attributes)
		{
			foreach (var a in attributes)
			{
				if (!HasAttribute(a))
				{
					return false;
				}
			}

			return true;
		}

		public void MoveAttribute(string traitType, int indexFrom, int indexInsertTo)
		{
			if (AttributeTable.ContainsKey(traitType))
			{
				AttributeTable[traitType].InsertTo(indexFrom, indexInsertTo);
			}
		}

		public void DeleteUnusedAttribute(IEnumerable<IMetadataAttribute> usedAttributes)
		{
			List<IMetadataAttribute> unusedAttributes = new List<IMetadataAttribute>();

			foreach (string key in AttributeTable.Keys)
			{
				foreach (var value in AttributeTable[key])
				{
					if (!usedAttributes.Contains(value))
					{
						unusedAttributes.Add(value);
					}
				}
			}

			foreach (var delete in unusedAttributes)
			{
				RemoveAttribute(delete);
			}
		}

		private bool isValidIndex(int traitIndex, int attributeIndex)
		{
			if (traitIndex < TraitTypes.Count)
			{
				if (attributeIndex < AttributeTable[TraitTypes[traitIndex].Name].Count)
				{
					return true;
				}
			}

			return false;
		}

		public class TraitType
		{
			public string Name { get; set; } = null;
			public int ChanceRange { get; set; } = 0;
			public bool IsIdentity { get; set; } = false;
			public bool ShowOnMetadata { get; set; } = true;

			public TraitType() { }
			public TraitType(string name, bool isIdentity, bool showOnMetadata, int chanceRange = 0)
			{
				Name = name;
				IsIdentity = isIdentity;
				ShowOnMetadata = showOnMetadata;
				ChanceRange = chanceRange;
			}

			public override bool Equals(object obj)
			{
				TraitType c = obj as TraitType;

				if (c == null)
				{
					return false;
				}

				return this.Name == c.Name;
			}

			public override int GetHashCode()
			{
				return Name.GetHashCode();
			}
		}

		// 사용되지 않는 기능입니다.

		///// <summary>속성 타입의 최대 개수 바이트입니다.</summary>
		//public int TraitTypeStride { get; }
		///// <summary>속성의 최대 개수 바이트입니다.</summary>
		//public int AttributeStride { get; }

		//public int MaxTraitTypeCount { get; }
		//public int MaxAttributeCount { get; }

		///// <summary>속성간 ID Stride 입니다.</summary>
		//public int IdStride { get; }

		///// <param name="traitTypeStride">속성 타입의 최대 개수 바이트입니다.</param>
		///// <param name="attributeStride">속성의 최대 개수 바이트입니다.</param>
		//public NftAttributeTable(int traitTypeStride, int attributeStride)
		//{
		//	TraitTypeStride = traitTypeStride;
		//	AttributeStride = attributeStride;

		//	IdStride = TraitTypeStride + AttributeStride;

		//	MaxTraitTypeCount = (int)Math.Pow(256, TraitTypeStride);
		//	MaxAttributeCount = (int)Math.Pow(256, AttributeStride);
		//}

		///// <summary>해당 속성의 인덱스를 반환합니다.</summary>
		///// <returns>(속성 타입 인덱스, 속성 인덱스)</returns>
		///// <exception cref="NotFoundedException"/>
		//public (byte, byte) GetAttributeIndex(IMetadataAttribute attribute)
		//{
		//	int traitTypeIndex = TraitTypes.FindIndex((t) => t.Name == attribute.trait_type);
		//	if (traitTypeIndex < 0)
		//	{
		//		throw new NotFoundedException($"Cannot found \"{attribute.trait_type}\" trait type.");
		//	}

		//	int attributeIndex = AttributeTable[attribute.trait_type].FindIndex((a) => a.Equals(attribute));
		//	if (attributeIndex < 0)
		//	{
		//		throw new NotFoundedException($"Cannot found \"{attribute}\"attribute.");
		//	}

		//	return ((byte)traitTypeIndex, (byte)attributeIndex);
		//}
		///// <summary>해당 속성의 ID를 문자열로 반환합니다.</summary>
		///// <returns>속성의 문자열 ID입니다.</returns>
		///// <exception cref="NotFoundedException"/>
		//public string GetID(IMetadataAttribute attribute)
		//{
		//	if (HasAttribute(attribute))
		//	{
		//		return attribute.GetID();
		//	}
		//	else
		//	{
		//		throw new NotFoundedException($"\"{attribute}\" attribute doesn't exist !");
		//	}
		//}

		///// <summary>해당 속성 리스트의 ID를 문자열로 반환합니다. 속성의 순서에 따른 ID를 반환합니다.</summary>
		///// <param name="attributeList"></param>
		///// <returns>속성 리스트의 문자열 ID 입니다.</returns>
		///// <exception cref="NotFoundedException"/>
		//public string GetID(List<IMetadataAttribute> attributeList)
		//{
		//	try
		//	{
		//		StringBuilder sb = new StringBuilder();

		//		foreach (var a in attributeList)
		//		{
		//			string id = GetID(a);
		//			sb.Append(id);
		//		}

		//		return sb.ToString();
		//	}
		//	catch (NotFoundedException e)
		//	{
		//		throw new NotFoundedException($"{e.Message} Attribute match failed !");
		//	}
		//}

		///// <summary>해당 Layer Element의 ID를 문자열로 반환합니다.</summary>
		///// <param name="element">Layer Element입니다.</param>
		///// <returns>Layer Element의 ID입니다.</returns>
		///// <exception cref="NotFoundedException"/>
		//public string GetID(LayerElement element)
		//{
		//	try
		//	{
		//		string traitID = GetID(element.TraitAttribute);
		//		string colorID = "";

		//		if (element.HasColorAttribute)
		//		{
		//			colorID = GetID(element.ColorAttribute);
		//		}

		//		return $"{traitID}{colorID}";
		//	}
		//	catch (NotFoundedException e)
		//	{
		//		throw new NotFoundedException($"{e.Message} Invalid attribute bound to layer element.");
		//	}
		//}

		//public List<IMetadataAttribute> GetAttributes(string id)
		//{
		//	List<IMetadataAttribute> result = new List<IMetadataAttribute>();

		//	int idCount = id.Length;
		//	for (int i = 0; i < idCount; i += IdStride)
		//	{
		//		string curId = id.Substring(i, IdStride);
		//		result.Add(GetAttribute(curId));
		//	}

		//	return result;
		//}

		//public IMetadataAttribute GetAttribute(string id)
		//{
		//	string traitType = id.Substring(0, TraitTypeStride);
		//	string attribute = id.Substring(TraitTypeStride, AttributeStride);

		//	if (!int.TryParse(traitType, NumberStyles.HexNumber, null, out int traitIndex) ||
		//		!int.TryParse(attribute, NumberStyles.HexNumber, null, out int attributeIndex))
		//	{
		//		throw new InvalidIdException($"Cannot parse attribute from id : {id}");
		//	}

		//	var result = GetAttributeByIndex(traitIndex, attributeIndex);

		//	if (result == null)
		//	{
		//		throw new InvalidIdException($"Cannot get attribute from id : {id}");
		//	}

		//	return result;
		//}

		//public int GetAttributeCount(string id)
		//{
		//	return id.Length / IdStride;
		//}

		//public List<string> GetElementIdList(string id)
		//{
		//	List<string> idList = new List<string>();

		//	int idLength = id.Length;
		//	for (int i = 0; i < idLength; i += IdStride)
		//	{
		//		string elementId = id.Substring(i, IdStride);
		//		idList.Add(elementId);
		//	}

		//	return idList;
		//}

		///// <summary>속성 ID를 속성 타입 순서로 정렬합니다.</summary>
		///// <param name="idStream">정렬된 ID 입니다.</param>
		///// <return>정렬된 ID 입니다.</return>
		///// <exception cref="InvalidIdException"/>
		//public string SortID(string idStream)
		//{
		//	if (idStream.Length % IdStride != 0)
		//	{
		//		throw new InvalidIdException("Invalid ID length.");
		//	}

		//	List<(int Trait, string ID)> elementIdList = new List<(int Trait, string ID)>();

		//	int idCount = idStream.Length;
		//	for (int i = 0; i < idCount; i += IdStride)
		//	{
		//		string traitId = idStream.Substring(i, TraitTypeStride);
		//		string attributeId = idStream.Substring(i + TraitTypeStride, AttributeStride);

		//		int.TryParse(traitId, NumberStyles.HexNumber, null, out int tId);

		//		elementIdList.Add((tId, $"{traitId}{attributeId}"));
		//	}

		//	var sortedElement = from e in elementIdList
		//						orderby e.Trait ascending
		//						select e;

		//	StringBuilder sb = new StringBuilder();

		//	foreach (var e in sortedElement)
		//	{
		//		sb.Append(e.ID);
		//	}

		//	return sb.ToString();
		//}

		//public bool IsSorted(string idStream)
		//{
		//	int checker = 0;
		//	int idStreamCount = idStream.Length;
		//	for (int i = 0; i < idStreamCount; i += IdStride)
		//	{
		//		string traitTypeIndexString = idStream.Substring(i, TraitTypeStride);
		//		if (!int.TryParse(traitTypeIndexString, NumberStyles.HexNumber, null, out int traitTypeIndex))
		//		{
		//			return false;
		//		}

		//		if (checker < traitTypeIndex)
		//		{
		//			checker = traitTypeIndex;
		//		}
		//		else
		//		{
		//			return false;
		//		}
		//	}

		//	return true;
		//}

		///// <summary>NFT 속성 테이블에 유효한 ID 인지 검사합니다.</summary>
		///// <param name="id">검사할 ID 입니다.</param>
		///// <return>유효한 ID이면 true를 반환합니다.</return>
		//public bool IsValidID(string id)
		//{
		//	if (id.Length % IdStride != 0)
		//	{
		//		return false;
		//	}

		//	List<string> duplicateCheckList = new List<string>();

		//	int idCount = id.Length;
		//	for (int i = 0; i < idCount; i += IdStride)
		//	{
		//		string traitId = id.Substring(i, TraitTypeStride);
		//		string attributeId = id.Substring(i + TraitTypeStride, AttributeStride);

		//		int.TryParse(traitId, NumberStyles.HexNumber, null, out int tId);
		//		int.TryParse(attributeId, NumberStyles.HexNumber, null, out int aId);

		//		string elementId = $"{traitId}{attributeId}";

		//		if (duplicateCheckList.AddIfNotExist(elementId))
		//		{
		//			// 중복키 발생
		//			return false;
		//		}

		//		if (!isValidIndex(tId, aId))
		//		{
		//			// 존재하지 않는 속성
		//			return false;
		//		}
		//	}

		//	return true;
		//}
	}
}
