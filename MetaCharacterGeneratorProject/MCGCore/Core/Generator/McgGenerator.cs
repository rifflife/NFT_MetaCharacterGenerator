using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Linq;
using System.IO;

namespace MCGCore
{
	public static class McgGenerator
	{
		private static GeneratorConsole mGeneratorConsole = new GeneratorConsole();

		/// <summary>NFT Collection 생성을 시도합니다.</summary>
		/// <param name="project"></param>
		public static bool TryGenerateNftCollection(in McgProject project)
		{
			McgProjectConfiguration config = project.ProjectConfiguration;
			McgEditor editor = project.Editor;
			NftAttributeTable attributeTable = editor.NftAttributeTable;
			NftCollectionData collectionData = project.NftCollectionData;
			
			Dictionary<string, int> elementHashDrawOrderTable;
			McgMemoryResources resources = new McgMemoryResources();
			resources.Initialize(config);

			int generateCount = config.GenerationCount;

			int layerCount = editor.Layers.Count;

			string workName;
			
			#region Check every trait at the layer elements
			workName = "Check every trait at the layer elements";
			mGeneratorConsole.WorkStart(workName);



			mGeneratorConsole.WorkCompleted(workName);
			#endregion

			#region Merge and cache layer elements 
			workName = "Merge and cache layer elements";
			mGeneratorConsole.WorkStart(workName);

			// 병렬처리 필요
			//var workResult = Parallel.ForEach(editor.Layers, (element) =>
			//{

			//});

			foreach (Layer layer in editor.Layers)
			{
				//bool isCompleted = true;

				foreach (LayerElement element in layer.LayerElements)
				{
					if (!element.HasTraitAttribute)
					{
						mGeneratorConsole.UnexpectedError($"Layer element \"{element}\" doesn't have trait attribute !");
						return false;
					}

					// Layer element의 이미지 조합
					BitmapSource mergedImage = element.GetBitmap(config);

					// Layer element의 속성 유효성 검사
					var elementAttributes = element.GetAttributes();
					if (!attributeTable.IsValidAttributes(elementAttributes))
					{
						mGeneratorConsole.UnexpectedError($"Layer element \"{element}\" has invalid ID !");
						return false;
					}

					// Layer element의 고유 해시 생성
					string elementHash = HashManager.GetHeshStringOrNull(elementAttributes);

					// Layer element캐싱
					resources.CacheIamge(elementHash, mergedImage, layer.Name);

					mGeneratorConsole.WriteLine($"Merge images and cache : {element}");
				}
			}

			mGeneratorConsole.WorkCompleted(workName);
			#endregion

			#region Bind draw order
			workName = "Bind draw order";

			elementHashDrawOrderTable = new Dictionary<string, int>();

			for (int i = 0; i < editor.Layers.Count; i++)
			{
				foreach (var element in editor.Layers[i].LayerElements)
				{
					string hashCode = element.GetAttributeHashCodeOrNull();
					
					if (hashCode != null)
					{
						elementHashDrawOrderTable.Add(hashCode, i);
					}
				}
			}

			mGeneratorConsole.WorkStart(workName);
			#endregion

			#region Generate random NFT DNA and metadata
			workName = "Generate random NFT DNA and metadata";
			mGeneratorConsole.WorkStart(workName);

			// Set individual trait type chance
			long numberOfCombinationCases = attributeTable.GetNumberOfCombinationCases();

			if (generateCount >= numberOfCombinationCases)
			{
				mGeneratorConsole.UnexpectedError($"There are not enough layer elements to combine.\nDecrease generation count or add more layer elements.");
				return false;
			}

			// Set seed
			attributeTable.SetSeed(config.Seed);

			for (int i = 0; i < generateCount; i++)
			{
				List<IMetadataAttribute> dnaAttributes = null;

				bool isGenerationFalid = true;

				// Try get unique random DNA
				for (int tryCount = 0; tryCount < 100000; tryCount++)
				{
					dnaAttributes = attributeTable.GetRandomDnaAttributes();

					string dnaString = HashManager.GetHeshStringOrNull(dnaAttributes);
					if (collectionData.HasDNA(dnaString))
					{
						continue;
					}

					// Check is valid DNA
					var findedElements = editor.FindLayerElementsOrNullByDNA(dnaAttributes);

					if (findedElements == null)
					{
						continue;
					}

					// Check is elements image cached
					List<(string hashCode, int layerIndex)> unsortedDrawTable = new List<(string hashCode, int layerIndex)>();

					bool isValid = true;
					foreach (var element in findedElements)
					{
						string hashCode = element.GetAttributeHashCodeOrNull();
						if (!resources.HasImage(hashCode))
						{
							isValid = false;
							break;
						}

						int drawOrder = elementHashDrawOrderTable[hashCode];
						unsortedDrawTable.Add((hashCode, drawOrder));
					}

					if (isValid == false)
					{
						continue;
					}

					var sortedDrawTable = from e in unsortedDrawTable
								orderby e.layerIndex
								select e.hashCode;

					// Check DNA is verify
					string dnaHashString = HashManager.GetHeshStringOrNull(dnaAttributes);

					if (dnaHashString == null)
					{
						mGeneratorConsole.UnexpectedError($"Random DNA generation failed!\nDecrease generation count or add more layer elements.");
						return false;
					}
					
					// Sort element by layer index to draw images right order.
					string nftName = $"# {config.NftCollectionName} {i}";

					NftMetadata nftMetadata = new NftMetadata()
					{
						name = nftName,
						dna = dnaHashString,
						attribute = dnaAttributes,
						collection_name = config.NftCollectionName,
						draw_table = new List<string>(sortedDrawTable),
					};

					collectionData.AddNewCollection(nftMetadata);
					mGeneratorConsole.WriteLine($"\"{nftName}\" : Metadata generated.");
					isGenerationFalid = false;
					break;
				}

				if (isGenerationFalid)
				{
					mGeneratorConsole.UnexpectedError($"Random DNA generation failed!\nDecrease generation count or add more layer elements.");
					return false;
				}
			}

			mGeneratorConsole.WorkCompleted(workName);
			#endregion

			#region Save Metadatas
			workName = "Save Metadatas";

			int index = 0;
			foreach (NftMetadata metadata in collectionData.MetadataList)
			{
				NftMetadata resultMetadata = metadata.Copy();
				var attributes = resultMetadata.attribute;
				for (int i = attributes.Count - 1; i >= 0; i--)
				{
					var a = attributes[i] as StringTraitAttribute;

					// Delete NULL attributes
					if (a != null && a.value == NftAttributeTable.NULL_VALUE)
					{
						attributes.RemoveAt(i);
					}

					// Delete if it's shouldn't show on metadata
					if (attributeTable.ShouldShowOnMetadata(a) == false)
                    {
						attributes.RemoveAt(i);
                    }
				}

				JsonHandler.SaveToJsonToken(resultMetadata, new Uri($"{Path.Combine(config.CollectionPath, index.ToString())}.json"));
				mGeneratorConsole.WriteLine($"\"{resultMetadata.name}\" : Metadata saved.");
				index++;
			}

			mGeneratorConsole.WorkCompleted(workName);
			#endregion

			#region Generate NFT images
			workName = "Generate and save NFT images";
			mGeneratorConsole.WorkStart(workName);

			index = 0;
			foreach (NftMetadata m in collectionData.MetadataList)
			{
				var drawTable = m.draw_table;

				List<BitmapSource> drawImages = new List<BitmapSource>();
				List<string> exceptLayerNames = editor.GetDrwaExceptionLayerNames(m.attribute);

				foreach (string imageHashCode in drawTable)
				{
					// Load cached image from resources table
					var bitmap = resources.LoadCacheImage(imageHashCode);

					// Check if it's exception case
					if (exceptLayerNames.Contains(bitmap.LayerName))
                    {
						continue; // Current layer is excepted layer. So don't draw.
                    }

					drawImages.Add(bitmap.Bitmap);
				}

				var mergedImage = ImageProcessor.MergeImages(drawImages, config.NftWidth, config.NftHeight);
				
				ImageProcessor.SaveImage(mergedImage, $"{Path.Combine(config.CollectionPath, index.ToString())}.png");
				mGeneratorConsole.WriteLine($"\"{m.name}\" : Image saved.");
				index++;
			}

			mGeneratorConsole.WorkCompleted(workName);
            #endregion

            #region Generate NFT CSV Table

			// Get trait type index table for CSV table.
			Dictionary<string, int> traitIndex = attributeTable.GetTraitTypeByIndexTable(1);
			int tableWidth = attributeTable.TraitCount + 1;

            List<List<string>> traitCollectionTable = new List<List<string>>();

			int row = 0;

			foreach (NftMetadata m in collectionData.MetadataList)
            {
				List<string> rowContext = new List<string>();

				for (int i = 0; i < tableWidth; i++)
                {
					rowContext.Add("");
				}

				traitCollectionTable.Add(rowContext);
				traitCollectionTable[row][0] = row.ToString();

				foreach (var a in  m.attribute)
                {
					var stringTrait = a as StringTraitAttribute;

					// Exclude NULL attributes
					if (stringTrait.value == NftAttributeTable.NULL_VALUE)
					{
						continue;
					}

					// Delete if it's shouldn't show on metadata
					if (attributeTable.ShouldShowOnMetadata(stringTrait) == false)
					{
						continue;
					}

					if (traitIndex.TryGetValue(stringTrait.trait_type, out int col))
                    {
						traitCollectionTable[row][col] = stringTrait.value;
                    }
					else
                    {
						traitCollectionTable[row].Add(stringTrait.value);
                    }
                }

				row++;
			}

			string tableName = "Trait Collection Table.csv";
			string traitCollectionTalbe = Path.Combine(config.ProjectPath, tableName);

			using (FileStream fs = new FileStream(traitCollectionTalbe, FileMode.Create))
			using (StreamWriter sw = new StreamWriter(fs))
			{
				// Print Tarit Names
				for (int i = 0; i < tableWidth; i++)
                {
					string traitString = "-";

					foreach (var ts in traitIndex.Keys)
                    {
						if (traitIndex[ts] == i)
                        {
							traitString = ts;
							break;
                        }
                    }

					sw.Write($"{traitString},");
                }

				sw.WriteLine();

				foreach (var nftStream in traitCollectionTable)
                {
					sw.WriteLine(string.Join(",", nftStream));
                }
			}

            #endregion

            return true;
		}

		//public static bool CreateLayerElementImages(in McgProjectConfiguration config, in McgEditor editor)
		//{
		//	// Delete all cache
		//	if (Directory.Exists(config.CachePath))
		//	{
		//		var files = Directory.GetFiles(config.CachePath);
		//		foreach (var filePath in files)
		//		{
		//			File.Delete(filePath);
		//		}
		//	}

		//	// Generate and cache new layer element images
		//	try
		//	{
		//		var layers = editor.Layers;

		//		// Retrieve Layer
		//		for (int layerIndex = 0; layerIndex < layers.Count; layerIndex++)
		//		{
		//			var groups = layers[layerIndex].LayerGroups;

		//			// Retrieve Layer Group
		//			for (int groupIndex = 0; groupIndex < groups.Count; groupIndex++)
		//			{
		//				var elements = groups[groupIndex].LayerElements;

		//				// Retrieve Layer Element
		//				for (int elementIndex = 0; elementIndex < elements.Count; elementIndex++)
		//				{
		//					var imagePaths = elements[elementIndex].Image;
		//					editor.ColorPalettes
		//					var colorNames = elements[elementIndex].ColorName;

		//					// There is no color set
		//					if (colorNames.Count == 0)
		//					{
		//						List<BitmapSource> elementImages = new List<BitmapSource>();

		//						foreach (string path in imagePaths)
		//						{
		//							elementImages.Add(ImageProcessor.LoadBitmap(path));
		//						}

		//						BitmapSource mergedImage = ImageProcessor.MergeImages(elementImages, config.NftWidth, config.NftHeight);

		//						string mergedImageFileName = $"{config.NftCollectionName}_{layerIndex}_{groupIndex}_{elementIndex}.png";
		//						ImageProcessor.SaveImage(mergedImage, Path.Combine(config.CachePath, mergedImageFileName));
		//					}
		//					else
		//					{
		//						// Retrieve Colors
		//						for (int colorIndex = 0; colorIndex < colorNames.Count; colorIndex++)
		//						{

		//						}
		//					}
		//				}
		//			}
		//		}
		//	}
		//	catch(Exception e)
		//	{
		//		StaticConsole.WriteLine(e.Message);
		//		StaticConsole.WriteLine("# Create Layer Element Images Failed !");
		//	}

		//	return true;
		//}
	}
}

