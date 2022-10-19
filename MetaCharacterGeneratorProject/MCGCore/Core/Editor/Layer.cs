using System.Collections.Generic;
using System.Text;

namespace MCGCore
{
	public class Layer
	{
		public string Name { get; set; }

		public string IdentityTraitType { get; set; } = null;
		public string AdditionalTraitType { get; set; } = null;

		public List<LayerElement> LayerElements { get; } = new List<LayerElement>();

		private int mLastLayerElementCount = 0;

		public Layer(string name)
		{
			Name = name;
		}

		/// <summary>Layer Element를 생성하고 생성된 Layer Element를 반환합니다.</summary>
		/// <param name="name">Layer Element의 이름입니다.</param>
		/// <returns>생성된 Layer Element입니다. 생성에 실패한 경우 null을 반환합니다.</returns>
		public LayerElement AddElement(string name = null)
		{
			if (LayerElements.Find(i => i.Name == name) != null)
			{
				return null; // Element already exist
			}

			if (name.IsCompletelyEmpty())
			{
				name = $"{Localization.LayerElementName}{mLastLayerElementCount++}";
			}

			var element = new LayerElement(name);

			LayerElements.Add(element);

			return element;
		}

		public LayerElement FindLayerElementByName(string name)
		{
			return LayerElements.Find(layerGroup => layerGroup.Name == name);
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			var layerName = Name.IsCompletelyEmpty() ? Localization.UnidentifiedName : Name;

			sb.Append("[Layer] ");
			sb.Append(layerName);
			sb.Append(" / Required : ");
			sb.Append(IdentityTraitType);
			sb.Append(" / Additional : ");
			sb.Append(AdditionalTraitType);

			return sb.ToString();
		}
	}
}
