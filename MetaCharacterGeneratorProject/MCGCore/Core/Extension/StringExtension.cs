namespace MCGCore
{
	public static class StringExtension
	{
		/// <summary> 문자열이 null이거나 비어있거나, 공백으로만 이루어져있는지 여부를 반환합니다. </summary>
		/// <returns>문자열이 null, 공백, 혹은 비어있다면 true를 반환합니다.</returns>
		public static bool IsCompletelyEmpty(this string text)
		{
			return string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text);
		}

		public static bool IsEqualInLowerCase(string lhs, string rhs) => lhs.ToLower() == rhs.ToLower();
	}
}
