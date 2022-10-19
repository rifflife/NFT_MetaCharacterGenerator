using System;
using System.Collections.Generic;

namespace MCGCore
{
	public class AttributeGroup
	{
		public string AttributeName { get; set; }
		public List<string> Attributes;

		public AttributeGroup(string attributeName)
		{
			AttributeName = attributeName;
		}

		public void AddTriat(IMetadataAttribute attributes)
		{
			//Attributes
		}
	}
}
