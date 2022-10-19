using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCGCore
{
	public class NotInitializedException : Exception
	{
		public NotInitializedException(string message = null)
			: base(Localization.GetErrorMessage(Localization.NotInitializedException, message)) { }
	}

	public class BindingException : Exception
	{
		public BindingException(string message = null)
			: base(Localization.GetErrorMessage(Localization.BindingException, message)) { }
	}

	public class FileNotFoundedException : Exception
	{
		public FileNotFoundedException(string message = null)
			: base(Localization.GetErrorMessage(Localization.FileNotFoundedException, message)) { }
	}

	public class NotFoundedException : Exception
	{
		public NotFoundedException(string message = null)
			: base(Localization.GetErrorMessage(Localization.NotFoundedException, message)) { }
	}

	public class CacheFailedException : Exception
	{
		public CacheFailedException(string message = null)
			: base(Localization.GetErrorMessage(Localization.CacheFailedException, message)) { }
	}

	public class CacheDeleteFailedException : Exception
	{
		public CacheDeleteFailedException(string message = null)
			: base(Localization.GetErrorMessage(Localization.CacheDeleteFailedException, message)) { }
	}

	public class LimitException : Exception
	{
		public LimitException(string message = null)
			: base(Localization.GetErrorMessage(Localization.LimitException, message)) { }
	}

	public class InvalidIdException : Exception
	{
		public InvalidIdException(string message = null)
			: base(Localization.GetErrorMessage(Localization.InvalidIdException, message)) { }
	}
}
