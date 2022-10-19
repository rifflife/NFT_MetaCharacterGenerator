using System;
using System.Collections.Generic;
using System.IO;

using Excel = Microsoft.Office.Interop.Excel;

namespace MCGCore
{
	public class ExcelReader
	{
		private Dictionary<string, DataCells> mDataCellsBySheetName = new Dictionary<string, DataCells>();
		public string FilePath { get; private set; }

		public ExcelReader(in string excelFilePath)
		{
			string rootPath = Path.GetFullPath(excelFilePath);
			FilePath = rootPath;

			if (!File.Exists(rootPath))
			{
				return;
			}

			Excel.Application excelApplication = null;
			Excel.Workbook workBook = null;
			Excel.Range range = null;

			try
			{
				StaticConsole.WriteLine($"Try open excel file : {rootPath}");
				StaticConsole.WriteLine($"Please wait for open excel process...");
				excelApplication = new Excel.Application();
				workBook = excelApplication.Workbooks.Open(rootPath);

				int workSheetsCount = workBook.Worksheets.Count;

				for (int i = 1; i <= workSheetsCount; i++)
				{
					var workSheet = workBook.Worksheets[i] as Excel.Worksheet;

					range = workSheet.UsedRange;
					mDataCellsBySheetName.Add(workSheet.Name, new DataCells(range));

					releaseObject(range);
					releaseObject(workSheet);
				}
			}
			catch (Exception e)
			{
				StaticConsole.WriteLine("Excel Parse Error!");
				StaticConsole.WriteLine(e);
			}
			finally
			{
				workBook?.Close();
				excelApplication?.Quit();

				releaseObject(excelApplication);
				releaseObject(workBook);

				killExcel();
			}
		}

		public bool TryRead<T>(out T value, string sheetName, int row, int column)
		{
			return mDataCellsBySheetName[sheetName].TryRead(out value, row, column);
		}

		private void releaseObject(object obj)
		{
			try
			{
				if (obj != null)
				{
					System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
					obj = null;
				}
			}
			catch (Exception e)
			{
				obj = null;
				throw e;
			}
			finally
			{
				GC.Collect();
			}
		}

		private void killExcel()
		{
			System.Diagnostics.Process[] PROC = System.Diagnostics.Process.GetProcessesByName("EXCEL");
			foreach (System.Diagnostics.Process PK in PROC)
			{
				if (PK.MainWindowTitle.Length == 0)
				{
					PK.Kill();
				}
			}
		}
	}

	public class DataCells
	{
		private object[,] mDataTalbe;

		public int RowCount { get; private set; }
		public int ColumnCount { get; private set; }

		public DataCells(in Excel.Range range)
		{
			RowCount = range.Rows.Count + 1;
			ColumnCount = range.Columns.Count + 1;

			mDataTalbe = new object[RowCount, ColumnCount];

			for (int row = 1; row < RowCount; row++)
			{
				for (int column = 1; column < ColumnCount; column++)
				{
					Excel.Range currentCell = range[row, column] as Excel.Range;
					mDataTalbe[row, column] = currentCell.Value2;

					bool isMerged = (bool)(currentCell.MergeCells as object);

					// 병합된 Cell이고 값이 없는 경우 바로 윗값을 대입한다.
					if (mDataTalbe[row, column] == null && isMerged && row - 1 > 1)
					{
						mDataTalbe[row, column] = mDataTalbe[row - 1, column];
					}
				}
			}
		}

		public bool TryRead<T>(out T value, int row, int column)
		{
			if (row < 1 || row >= RowCount || column < 1 || column >= ColumnCount)
			{
				value = default(T);
				return false;
			}

			try
			{
				var data = mDataTalbe[row, column];
				if (data == null)
				{
					value = default(T);
					return false;
				}

				value = (T)data;
				return true;
			}
			catch
			{
				value = default(T);
				return false;
			}
		}
	}
}
