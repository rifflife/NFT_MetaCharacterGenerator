namespace MCGCore
{
	public static class Localization
	{
		public static readonly string UnidentifiedName = "Unidentified Name";
		public static readonly string NotYetIdentified = "Need Identified";

		public static readonly string LayerName = "Layer_";
		public static readonly string LayerElementName = "LayerElement_";

		#region Exception

		public static readonly string McgError = "# MCG Error : ";

		public static readonly string NotInitializedException = "[IdHandlerNotInitialize]";
		public static readonly string BindingException = "[BindingException]";
		public static readonly string FileNotFoundedException = "[FileNotFounded]";
		public static readonly string CacheFailedException = "[CacheFailedException]";
		public static readonly string CacheDeleteFailedException = "[CacheDeleteFailedException]";
		public static readonly string LimitException = "[LimitException]";
		public static readonly string NotFoundedException = "[NotFoundedException]";
		public static readonly string InvalidIdException = "[InvalidIdException]";

		public static string GetErrorMessage(string error, string message)
		{
			return $"{McgError}{error} {message}";
		}

		#endregion
	}
}
