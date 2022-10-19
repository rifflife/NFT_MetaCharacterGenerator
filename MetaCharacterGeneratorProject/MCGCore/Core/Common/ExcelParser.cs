using System;
using System.Collections.Generic;
using System.IO;

namespace MCGCore
{
	public static class ExcelParser
	{
		private static ExcelParserConsole mExcelParserConsole = new ExcelParserConsole();

		public static readonly string ProjectSettingSheetName = "Project Setting";
		public static readonly string AttributeSettingSheetName = "Attribute Setting";
		public static readonly string LayerSettingSheetName = "Layer Setting";
		public static readonly string ElementSettingSheetName = "Element Setting";
		public static readonly string TraitChanceSetting = "Trait Chance Setting";
		public static readonly string ExceptionSetting = "Exception Setting";

		private const int MAX_PARSE_TRY_COUNT = 10000;

		private enum ProjectSettingProperty
		{
			ProjectName,
			NftCollectionName,
			NftImageWidth,
			NftImageHeight,
			GenerationCount,
			Seed,
		}
		private static Dictionary<string, ProjectSettingProperty> ProjectSettingPropertyTable = new Dictionary<string, ProjectSettingProperty>()
		{
			{ "Project Name", ProjectSettingProperty.ProjectName },
			{ "NFT Collection Name", ProjectSettingProperty.NftCollectionName },
			{ "NFT Image Width", ProjectSettingProperty.NftImageWidth },
			{ "NFT Image Height", ProjectSettingProperty.NftImageHeight },
			{ "Generation Count", ProjectSettingProperty.GenerationCount },
			{ "Seed", ProjectSettingProperty.Seed },
		};

		public static McgProject ParseMcgProject(in ExcelReader reader)
		{
			McgProject project = new McgProject();

			string workName;

			#region Parse project configuration
			workName = "Parse project configuration";
			mExcelParserConsole.WorkStart(workName);

			try
			{
				parseMcgProjectConfiguration(reader, ref project);
			}
			catch (Exception e)
			{
				mExcelParserConsole.WriteLine(e);
				return null;
			}

			mExcelParserConsole.WorkCompleted(workName);
			#endregion

			#region Parse MCG Editor
			workName = "Parse MCG Editor";
			mExcelParserConsole.WorkStart(workName);

			try
			{
				parseMcgEditor(reader, ref project);
			}
			catch (Exception e)
			{
				mExcelParserConsole.WriteLine(e);
				return null;
			}

			mExcelParserConsole.WorkCompleted(workName);
			#endregion


			return project;
		}

		private static void parseMcgProjectConfiguration(in ExcelReader reader, ref McgProject project)
		{
			string currentSheet = ProjectSettingSheetName;

			McgProjectConfiguration config = project.ProjectConfiguration;

			config.ProjectPath = Directory.GetParent(reader.FilePath).FullName;

			int readRow = 2;

			while(true)
			{
				reader.TryRead(out string propertyName, currentSheet, readRow, 1);

				if (propertyName == null)
				{
					break;
				}

				if (!ProjectSettingPropertyTable.ContainsKey(propertyName))
				{
					readRow++;
					continue;
				}

				switch (ProjectSettingPropertyTable[propertyName])
				{
					case ProjectSettingProperty.ProjectName:
						if (reader.TryRead(out string projectdName, currentSheet, readRow, 2))
						{
							config.ProjectName = projectdName;
						}
						else
						{
							return;
						}
						break;

					case ProjectSettingProperty.NftCollectionName:
						if (reader.TryRead(out string nftCollectionName, currentSheet, readRow, 2))
						{
							config.NftCollectionName = nftCollectionName;
						}
						else
						{
							return;
						}
						break;

					case ProjectSettingProperty.NftImageWidth:
						if (reader.TryRead(out double imageWidth, currentSheet, readRow, 2))
						{
							config.NftWidth = (int)imageWidth;
						}
						break;

					case ProjectSettingProperty.NftImageHeight:
						if (reader.TryRead(out double imageHeight, currentSheet, readRow, 2))
						{
							config.NftHeight = (int)imageHeight;
						}
						break;

					case ProjectSettingProperty.GenerationCount:
						if (reader.TryRead(out double generationCount, currentSheet, readRow, 2))
						{
							config.GenerationCount = (int)generationCount;
						}
						break;
					case ProjectSettingProperty.Seed:
						if (reader.TryRead(out double seed, currentSheet, readRow, 2))
                        {
							config.Seed = (int)seed;
                        }
						break;
				}
				readRow++;
			}

			project.ProjectConfiguration = config;
		}

		private static void parseMcgEditor(ExcelReader reader, ref McgProject project)
		{
			McgEditor editor = project.Editor;
			
			mExcelParserConsole.DrawSeparator();
			mExcelParserConsole.WriteLine("# Parse Attribute Setting");

			for (int row = 3; row < MAX_PARSE_TRY_COUNT; row++)
			{
				if (tryReadString(reader, out string traitTypeName, AttributeSettingSheetName, row, 1))
				{
					if (!reader.TryRead(out bool identity, AttributeSettingSheetName, row, 2))
					{
						mExcelParserConsole.WriteLine($"Worng identity name in sheet : {AttributeSettingSheetName}");
						return;
					}
					if (!reader.TryRead(out bool showOnMetadata, AttributeSettingSheetName, row, 3))
					{
						mExcelParserConsole.WriteLine($"Wrong boolean in sheet : {AttributeSettingSheetName}");
						return;
					}
					if (!reader.TryRead(out double chanceRange, AttributeSettingSheetName, row, 4))
					{
						mExcelParserConsole.WriteLine($"Wrong chance range in sheet : {AttributeSettingSheetName}");
						return;
					}

					editor.NftAttributeTable.AddTraitType(traitTypeName, identity, showOnMetadata, (int)chanceRange);
					mExcelParserConsole.WriteLine($"Add trait type to NFT Attribute Table : {traitTypeName}");
				}
				else
				{
					mExcelParserConsole.WriteLine("# Attribute Setting Parse End");
					break;
				}
			}

			mExcelParserConsole.DrawSeparator();
			mExcelParserConsole.WriteLine("# Parse Layer Setting");

			for (int row = 3; row < MAX_PARSE_TRY_COUNT; row++)
			{
				if (tryReadString(reader, out string layerName, LayerSettingSheetName, row, 1))
				{
					// Add new layer
					Layer currentLayer = editor.AddLayer(layerName);
					if (currentLayer == null)
					{
						mExcelParserConsole.WriteLine($"Layer already exist. Layer : {layerName}");
						return;
					}

					// Add identity trait type
					if (!tryReadString(reader, out string identityTraitTypeName, LayerSettingSheetName, row, 2))
					{
						mExcelParserConsole.WriteLine($"Worng Identity Trait in sheet : {LayerSettingSheetName}");
						return;
					}

					if (!editor.NftAttributeTable.HasTraitType(identityTraitTypeName))
					{
						mExcelParserConsole.WriteLine($"NFT attributes table doesn't have trait type : {identityTraitTypeName}");
						return;
					}

					currentLayer.IdentityTraitType = identityTraitTypeName;

					// Add additional trait type
					if (tryReadString(reader, out string additionalTraitType, LayerSettingSheetName, row, 3))
					{
						if (!editor.NftAttributeTable.HasTraitType(additionalTraitType))
						{
							mExcelParserConsole.WriteLine($"NFT attributes table doesn't have trait type : {additionalTraitType}");
							return;
						}

						currentLayer.AdditionalTraitType = additionalTraitType;
					}

					mExcelParserConsole.WriteLine($"Layer Setted. Layer : {currentLayer}");
				}
				else
				{
					mExcelParserConsole.WriteLine("# Layer Setting Parse End");
					break;
				}
			}

			mExcelParserConsole.DrawSeparator();
			mExcelParserConsole.WriteLine("# Parse Element Setting");

			for (int row = 3; row < MAX_PARSE_TRY_COUNT; row++)
			{
				if (tryReadString(reader, out string layerName, ElementSettingSheetName, row, 1))
				{
					// Set required fields
					bool isInvalid = true;
					isInvalid &= tryReadString(reader, out string layerElementName, ElementSettingSheetName, row, 2);
					isInvalid &= tryReadString(reader, out string identityTraitName, ElementSettingSheetName, row, 3);
					isInvalid &= tryReadString(reader, out string imageFileName, ElementSettingSheetName, row, 4);


					if (isInvalid == false)
					{
						mExcelParserConsole.WriteLine($"Wrong layer element at layer : {layerName}");
						return;
					}

					LayerElement currentLayerElement = editor.AddElement(layerName, layerElementName);

					string currentIdentityTraitName = editor.FindLayerByname(layerName).IdentityTraitType;
					var identityAttribute = new StringTraitAttribute(currentIdentityTraitName, identityTraitName);

					editor.SetTraitAttribute(layerName, layerElementName, identityAttribute, true);

					string currentAdditionalTraitName = editor.FindLayerByname(layerName).AdditionalTraitType;
					// Set additional fields
					if (tryReadString(reader, out string additionalTraitName, ElementSettingSheetName, row, 5))
					{
						var additionalAttribute = new StringTraitAttribute(currentAdditionalTraitName, additionalTraitName);

						editor.SetTraitAttribute(layerName, layerElementName, additionalAttribute, false);
					}
					else
					{
						if (currentAdditionalTraitName != null)
						{
							var nullAdditionalAttribute = new StringTraitAttribute(currentAdditionalTraitName, NftAttributeTable.NULL_VALUE);
							editor.SetTraitAttribute(layerName, layerElementName, nullAdditionalAttribute, false);
						}
					}

					// Add image part
					string imagePath = Path.Combine(project.ProjectConfiguration.ResourcePath, imageFileName);
					ImagePart ImagePart = editor.AddImageToLayerElement(layerName, layerElementName, imagePath);

					// Set color
					if (tryReadString(reader, out string colorCodeString, ElementSettingSheetName, row, 6))
					{
						if (ImagePart == null)
						{
							mExcelParserConsole.WriteLine($"Image file missing : {imageFileName}");
							return;
						}

						ImagePart.BindColor(new Color32(colorCodeString, false));
					}

					if (ImagePart != null)
					{
						mExcelParserConsole.WriteLine($"Image Part Setted. : {ImagePart}");
					}

					if (currentLayerElement != null)
					{
						mExcelParserConsole.WriteLine($"Layer Element Setted. Layer Element: {currentLayerElement}");
					}
				}
				else
				{
					mExcelParserConsole.WriteLine("# Element Setting Parse End");
					break;
				}
			}

			mExcelParserConsole.DrawSeparator();
			mExcelParserConsole.WriteLine("# Parse Trait Chance Setting");

			for (int row = 3; row < MAX_PARSE_TRY_COUNT; row++)
			{
				if (tryReadString(reader, out string traitType, TraitChanceSetting, row, 1))
				{
					if (tryReadString(reader, out string traitValue, TraitChanceSetting, row, 2)
						&& reader.TryRead(out double currentChance, TraitChanceSetting, row, 3))
					{
						int changeChance = (int)currentChance;
						var attribute = editor.NftAttributeTable.GetAttributeByStringValue(traitType, traitValue);

						if (attribute == null)
						{
							mExcelParserConsole.WriteLine($"There is no such thing as [Trait Type : {traitType} / Value : {traitValue}]");
						}
						else
						{
							attribute.SetChance(changeChance);
							mExcelParserConsole.WriteLine($"Set attribute \"{attribute}\" chance to : {changeChance}");
						}
					}
					else
					{
						mExcelParserConsole.WriteLine("Wrong Trait Value or Chance!");
					}
				}
				else
				{
					mExcelParserConsole.WriteLine("# Trait Chance Setting Parse End");
					break;
				}
			}

			mExcelParserConsole.DrawSeparator();
			mExcelParserConsole.WriteLine("# Parse Exception Setting");

			for (int row = 3; row < MAX_PARSE_TRY_COUNT; row++)
            {
				if (tryReadString(reader, out string traitType, ExceptionSetting, row, 1))
                {
					if (tryReadString(reader, out string traitValue, ExceptionSetting, row, 2) &&
						tryReadString(reader, out string exceptLayerName, ExceptionSetting, row, 3))
                    {
						StringTraitAttribute exceptionAttribute = new StringTraitAttribute(traitType, traitValue);
						editor.AddLayerDrawException(exceptionAttribute, exceptLayerName);
					}
                }
				else
                {
					mExcelParserConsole.WriteLine("# Exception Setting Parse End");
					break;
                }
            }
		}

		private static bool tryReadString(in ExcelReader reader, out string value, string sheetName, int row, int column)
		{
			if (reader.TryRead(out value, sheetName, row, column))
			{
				if (value.IsCompletelyEmpty())
				{
					return false;
				}

				return true;
			}

			return false;
		}
	}

}
