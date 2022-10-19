using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;

namespace MCGCore
{
	public class LayerElement
	{
		public string Name { get; set; } = null;
		public IMetadataAttribute IdentityAttribute { get; set; } = null;
		public IMetadataAttribute AdditionalAttribute { get; set; } = null;
		public List<ImagePart> ImageParts { get; set; } = new List<ImagePart>();
		public float Weight { get; set; }

		public bool HasTraitAttribute { get { return IdentityAttribute != null; } }
		public bool HasColorAttribute { get { return AdditionalAttribute != null; } }

		public LayerElement() {}

		public LayerElement(string name)
		{
			Name = name;
			Weight = 1;
		}

		public LayerElement(LayerElement copy)
		{
			CopyFrom(copy);
		}

		public LayerElement Copy()
		{
			return new LayerElement(this);
		}

		public void CopyFrom(LayerElement copy)
		{
			this.Name = copy.Name + "_copy";
			this.IdentityAttribute = copy.IdentityAttribute?.Copy();
			this.AdditionalAttribute = copy.AdditionalAttribute?.Copy();

			foreach (ImagePart i in copy.ImageParts)
			{
				this.ImageParts.Add(i.Copy());
			}

			this.Weight = copy.Weight;
		}

		#region ImagePart

		public void MoveImagePart(int indexFrom, int indexInsertTo)
		{
			ImageParts.InsertTo(indexFrom, indexInsertTo);
		}

		public void RemoveImagePart(int removeIndex)
		{
			ImageParts.RemoveAt(removeIndex);
		}

		#endregion

		#region Color

		public void BindColor(int index, Color32 color)
		{
			if (index < ImageParts.Count)
			{
				ImageParts[index].HasBindedColor = true;
				ImageParts[index].BindedColor =	color;
			}
		}

		public void UnbindColor(int index)
		{
			if (index < ImageParts.Count)
			{
				ImageParts[index].HasBindedColor = false;
				ImageParts[index].BindedColor = Color32.White;
			}
		}

		#endregion

		#region Attribute

		/// <summary>해당 속성이 바인딩 되었는지 여부를 반환합니다.</summary>
		public bool HasAttribute(IMetadataAttribute attribute)
		{
			return IdentityAttribute.Equals(attribute) || AdditionalAttribute.Equals(attribute);
		}

		/// <summary>바인딩된 속성을 제거합니다.</summary>
		public void RemoveAttribute(IMetadataAttribute attribute)
		{
			if (IdentityAttribute.Equals(attribute))
			{
				IdentityAttribute = null;
			}
			else if (AdditionalAttribute.Equals(attribute))
			{
				AdditionalAttribute = null;
			}
		}

		/// <summary>속성을 바인딩합니다.</summary>
		public void BindAttribute(IMetadataAttribute attribute, bool isIdentity)
		{
			if (isIdentity)
			{
				IdentityAttribute = attribute;
			}
			else
			{
				AdditionalAttribute = attribute;
			}
		}

		public List<IMetadataAttribute> GetAttributes()
		{
			var list = new List<IMetadataAttribute>();

			if (HasTraitAttribute)
			{
				list.Add(IdentityAttribute);
			}
			if (HasColorAttribute)
			{
				list.Add(AdditionalAttribute);
			}

			return  list;
		}

		// TODO : Attribute binding not single one, List or Array

		#endregion

		public ImagePart AddNewRelativeImagePath(string relativeImagePath)
		{
			if (ImageParts.Find(i => i.ImageRelativePath == relativeImagePath) != null)
			{
				return null; // Element already exist
			}
			
			var imagePart = new ImagePart(relativeImagePath);

			ImageParts.Add(imagePart);

			return imagePart;
		}

		public BitmapSource GetBitmap(McgProjectConfiguration config)
		{
			List<BitmapSource> imageList = new List<BitmapSource>();

			foreach (var i in ImageParts)
			{
				imageList.Add(i.GetBitmap());
			}

			return ImageProcessor.MergeImages(imageList, config.NftWidth, config.NftHeight);;
		}

		/// <summary>Layer element의 속성 해시값을 반환합니다. 속성값이 없으면 null을 반환합니다.</summary>
		/// <returns>Layer element의 속성 해시값입니다.</returns>
		public string GetAttributeHashCodeOrNull()
		{
			List<IMetadataAttribute> attributes = new List<IMetadataAttribute>();

			if (HasTraitAttribute)
			{
				attributes.Add(IdentityAttribute);
			}
			if (HasColorAttribute)
			{
				attributes.Add(AdditionalAttribute);
			}

			return HashManager.GetHeshStringOrNull(attributes);
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			string elementName = Name.IsCompletelyEmpty() ? Localization.UnidentifiedName : Name;

			sb.Append("[Layer Element] ");
			sb.Append(elementName);
			sb.Append(" / ");
			sb.Append(IdentityAttribute?.ToString());
			sb.Append(AdditionalAttribute?.ToString());

			return sb.ToString();
		}
	}
}