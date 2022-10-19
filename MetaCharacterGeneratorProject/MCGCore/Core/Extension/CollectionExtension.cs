using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCGCore
{
	public static class CollectionExtension
	{
		public static bool IsEmpty<T>(this ICollection<T> collection)
		{
			return collection.Count <= 0;
		}
	}
}
