using System.Collections.Generic;

namespace MCGCore
{
	public static class ListExtension
	{
		public static void InsertTo<T>(this List<T> list, int indexFrom, int indexInsertTo)
		{
			// 값이 같으면 아무 행동도 하지 않는다.
			if (indexFrom == indexInsertTo)
			{
				return;
			}

			if (indexFrom < indexInsertTo)
			{
				indexInsertTo--;
			}

			T temp = list[indexFrom];
			list.RemoveAt(indexFrom);
			list.Insert(indexInsertTo, temp);
		}

		public static bool IsValidIndex<T>(this List<T> list, int index)
		{
			return index >= 0 && index < list.Count;
		}

		/// <summary>해당 값이 리스트 내부에 없으면 추가합니다.</summary>
		/// <returns>추가되었으면 true를 반환합니다. 그렇지 않으면 false를 반환합니다.</returns>
		public static bool AddIfNotExist<T>(this List<T> list, T value)
		{
			if (!list.Contains(value))
			{
				list.Add(value);
				return true;
			}

			return false;
		}

		/// <summary>리스트가 최대 개수를 초과했는지 검사합니다.</summary>
		/// <param name="maxCount">최대 개수입니다.</param>
		/// <returns>최대 개수를 초과했다면 true를 반환합니다.</returns>
		public static bool IsMaxCount<T>(this List<T> list, int maxCount)
		{
			return list.Count >= maxCount;
		}
	}
}
