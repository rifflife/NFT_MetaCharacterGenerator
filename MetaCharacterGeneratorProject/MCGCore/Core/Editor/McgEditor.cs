using System;
using System.Collections.Generic;
using System.IO;

namespace MCGCore
{
	public class McgEditor
	{
		public List<Layer> Layers = new List<Layer>();
		public NftAttributeTable NftAttributeTable { get; } = new NftAttributeTable();
		private McgProjectConfiguration mConfig;
		public Dictionary<IMetadataAttribute, string> LayerDrawExceptionByAttributes = new Dictionary<IMetadataAttribute, string>();

		private int mLastNewLayerIndex = 0;
		

		public McgEditor()
		{
			
		}

		public void SetProjectConfiguration(McgProjectConfiguration config)
		{
			checkDirectory(config.ProjectPath);
			checkDirectory(config.ResourcePath);
			checkDirectory(config.CachePath);
			checkDirectory(config.CollectionPath);

			mConfig = config;

			void checkDirectory(string path)
			{
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
			}
		}

		#region Layer

		public Layer FindLayerByname(string layerName)
		{
			return Layers.Find((layer) => layer.Name == layerName);
		}

		public Layer AddLayer(string name = null)
		{
			if (Layers.Find(i => i.Name == name) != null)
			{
				return null;
			}

			if (name.IsCompletelyEmpty())
			{
				name = $"{Localization.LayerName}{mLastNewLayerIndex++}";
			}

			Layer newLayer = new Layer(name);
			Layers.Add(newLayer);

			return newLayer;
		}
		
		#endregion

		#region Layer Element

		/// <summary>DNA Attribute list를 기반으로 포함되는 Layer element들을 찾습니다. 찾을 수 없는 요소가 있거나, DNA의 모든 요소를 찾을 수 없다면 null을 반환합니다.</summary>
		/// <returns>DNA에 기반한 Layer element이거나 null입니다.</returns>
		public List<LayerElement> FindLayerElementsOrNullByDNA(List<IMetadataAttribute> metadataAttributes)
		{
			List<IMetadataAttribute> identityTrait = new List<IMetadataAttribute>();
			List<IMetadataAttribute> additionalTrait = new List<IMetadataAttribute>();

			// The buffer of checked additional triats
			List<IMetadataAttribute> checkAdditionalTrait = new List<IMetadataAttribute>();

			// sort by identity type or not
			foreach (var a in metadataAttributes)
			{
				string traitType = a.trait_type;

				if (NftAttributeTable.IsIdentityTrait(traitType))
				{
					identityTrait.Add(a);
				}
				else
				{
					additionalTrait.Add(a);
				}
			}

			// Find right element
			List<LayerElement> elements = new List<LayerElement>();

			foreach (var layer in Layers)
			{
				var trait = identityTrait.Find((a)=>a.trait_type == layer.IdentityTraitType);
				if (trait == null)
				{
					continue;
				}

				bool cantFindElement = true;
				foreach (var element in layer.LayerElements)
				{
					if ((!element.HasTraitAttribute) || (!identityTrait.Contains(element.IdentityAttribute)))
					{
						continue;
					}

					if (element.HasColorAttribute)
					{
						if (!additionalTrait.Contains(element.AdditionalAttribute))
						{
							continue;
						}

						if (!checkAdditionalTrait.Contains(element.AdditionalAttribute))
						{
							checkAdditionalTrait.Add(element.AdditionalAttribute);
						}
					}

					identityTrait.Remove(element.IdentityAttribute);

					// Add checked additional trait.
					elements.Add(element);
					cantFindElement = false;
					break;
				}

				if (cantFindElement)
				{
					return null;
				}
			}

			if ((checkAdditionalTrait.Count == additionalTrait.Count) && identityTrait.IsEmpty())
			{
				return elements;
			}
			else
			{
				return null;
			}
		}

		public LayerElement FindLayerElementByName(string layerName, string elementName)
		{
			return FindLayerByname(layerName).FindLayerElementByName(elementName);
		}

		public IMetadataAttribute GetTraitAttribute(string layerName, string elementName)
		{
			return FindLayerElementByName(layerName, elementName).IdentityAttribute;
		}

		/// <summary>
		/// 해당 Layer Element의 속성을 설정합니다.
		/// </summary>
		/// <param name="layerName"></param>
		/// <param name="elementName"></param>
		/// <param name="attribute"></param>
		/// <exception cref="BindingException"/>
		public void SetTraitAttribute(string layerName, string elementName, IMetadataAttribute attribute, bool isIdentity)
		{
			Layer layer = FindLayerByname(layerName);

			if (layer == null)
			{
				throw new BindingException("Layer doesn't exist !");
			}

			if (IsValidLayerAttribute(layerName, attribute))
			{
				LayerElement element = FindLayerElementByName(layerName, elementName);

				if (element == null)
				{
					throw new BindingException("Layer Element doesn't exist !");
				}

				DeleteUnusedAttributeFromTalbe();

				// 레이어 엘리먼트에 속성 할당
				element.BindAttribute(attribute, isIdentity);

				// 속성 테이블에 속성 할당
				NftAttributeTable.AddAttributes(attribute, isIdentity);
			}
			else
			{
				throw new BindingException("This attribute already exist at other layer.");
			}
		}

		/// <summary> 해당 속성을 제거합니다. 현재는 고유 속성 혹은 색상 속성 둘 중 하나만 제거합니다. </summary>
		/// <param name="attibute">제거할 속성입니다.</param>
		public void RemoveTraitAttribute(string layerName, string elementName, IMetadataAttribute attibute)
		{
			var element = FindLayerElementByName(layerName, elementName);

			// Layer Element의 바인딩된 속성을 제거합니다.
			if (element != null && element.HasAttribute(attibute))
			{
				element.RemoveAttribute(attibute);
			}
			else
			{
				return;
			}

			// NFT 속성 테이블에서 바인딩 되었던 속성을 제거합니다.
			if (NftAttributeTable.HasAttribute(attibute))
			{
				NftAttributeTable.RemoveAttribute(attibute);
			}
		}

		/// <summary>Layer Element를 생성하고 생성된 Layer Element를 반환합니다.</summary>
		/// <param name="layerName">Layer의 이름입니다.</param>
		/// <param name="elementName">Layer Element의 이름입니다.</param>
		/// <returns>생성된 Layer Element입니다. 생성에 실패한 경우 null을 반환합니다.</returns>
		public LayerElement AddElement(string layerName, string elementName = null)
		{
			return FindLayerByname(layerName).AddElement(elementName);
		}

		/// <summary>Layer Element를 복사합니다.</summary>
		/// <returns>복사된 Layer Element입니다.</returns>
		public LayerElement CopyElement(string srcLayerName, string srcElementName, string desLayerName, string desElementRename = null)
		{
			var srcElement = FindLayerElementByName(srcLayerName, srcElementName);

			if (srcElement == null)
			{
				return null;
			}

			var copiedElement = FindLayerByname(desLayerName).AddElement();
			copiedElement.CopyFrom(srcElement);

			if (!desElementRename.IsCompletelyEmpty())
			{
				copiedElement.Name = desElementRename;
			}

			return copiedElement;
		}

		/// <summary>
		/// 이미지를 프로젝트로 복제하고, 해당 이미지 경로를 Layer Element에 추가합니다.
		/// TODO : 예외처리 필요
		/// </summary>
		public ImagePart AddImageToLayerElement(string layerName, string elementName, string relativeImagePath)
		{
			if (!File.Exists(relativeImagePath))
			{
				return null;
			}

			string fileName = Path.GetFileName(relativeImagePath);
			string copyPath = Path.Combine(mConfig.ResourcePath, fileName);

			try
			{
				if (File.Exists(copyPath))
				{
					//File.Delete(copyPath);
				}
				else
				{
					File.Copy(relativeImagePath, copyPath);
				}

				return FindLayerElementByName(layerName, elementName).AddNewRelativeImagePath(copyPath);
			}
			catch (Exception e)
			{
				StaticConsole.WriteLine(e);
				return null;
			}

		}

		#endregion

		#region Image Part

		public void BindColorToImagePart(string layerName, string elementName, int imagePartIndex, Color32 bindColor)
		{
			FindLayerElementByName(layerName, elementName).BindColor(imagePartIndex, bindColor);
		}

		public void MoveImagePart(string layerName, string elementName, int indexFrom, int indexInsertTo)
		{
			FindLayerElementByName(layerName, elementName).MoveImagePart(indexFrom, indexInsertTo);
		}

		public void RemoveImagePart(string layerName, string elementName, int removeIndex)
		{
			FindLayerElementByName(layerName, elementName).RemoveImagePart(removeIndex);
		}

		#endregion

		#region Exception

		public void AddLayerDrawException(IMetadataAttribute attributes, string drawExceptLayerName)
        {
			if (!LayerDrawExceptionByAttributes.ContainsKey(attributes))
            {
				LayerDrawExceptionByAttributes.Add(attributes, drawExceptLayerName);
            }
        }

		public List<string> GetDrwaExceptionLayerNames(in List<IMetadataAttribute> attributes)
        {
			List<string> exceptionList = new List<string>();

			foreach (var a in attributes)
            {
				if (LayerDrawExceptionByAttributes.ContainsKey(a))
                {
					string exceptionLayerName = LayerDrawExceptionByAttributes[a];

					if (exceptionList.Contains(exceptionLayerName) == false)
                    {
						exceptionList.Add(exceptionLayerName);
                    }
                }
            }

			return exceptionList;
		}

		#endregion

		public void DeleteUnusedAttributeFromTalbe()
		{
			var allAttributesInLayers = getAllAttributesFromLayer();
			NftAttributeTable.DeleteUnusedAttribute(allAttributesInLayers);
		}

		private List<IMetadataAttribute> getAllAttributesFromLayer()
		{
			List<IMetadataAttribute> attributes = new List<IMetadataAttribute>();

			foreach (Layer layer in Layers)
			{
				foreach (LayerElement element in layer.LayerElements)
				{
					var elementAttributes = element.GetAttributes();

					foreach (var a in elementAttributes)
					{
						attributes.AddIfNotExist(a);
					}
				}
			}

			return attributes;
		}

		/// <summary> 해당 속성이 다른 레이어에서 요구하지 않는 고유 속성인지 검사합니다. </summary>
		public bool IsValidLayerAttribute(string layerName, IMetadataAttribute attribute)
		{
			string traitType = attribute.trait_type;

			foreach (Layer layer in Layers)
			{
				if (layer.Name == layerName)
				{
					continue;
				}

				if (traitType == layer.IdentityTraitType)
				{
					return false;
				}
			}

			return true;
		}

		public void DrawLayerHierarchy()
		{
			foreach (Layer layer in Layers)
			{
				StaticConsole.WriteLine(layer);

				foreach (LayerElement element in layer.LayerElements)
				{
					StaticConsole.WriteLine($"\t{element}");

					foreach (ImagePart imagePart in element.ImageParts)
					{
						StaticConsole.WriteLine($"\t\t{imagePart}");
					}
				}

				StaticConsole.WriteLine("");
			}
		}
	}
}
