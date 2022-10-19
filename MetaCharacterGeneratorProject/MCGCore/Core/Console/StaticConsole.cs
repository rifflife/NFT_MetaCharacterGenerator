using System;
using System.Diagnostics;

namespace MCGCore
{
	public static class StaticConsole
	{
		private static CLIConsole mCLIConsole = new CLIConsole();

		public static void WriteLine(object context) => mCLIConsole.WriteLine(context);
		public static void WriteLine(string context) => mCLIConsole.WriteLine(context);
		public static void WriteLine(int context) => mCLIConsole.WriteLine(context);
		public static void WriteLine(float context) => mCLIConsole.WriteLine(context);
	}

	public class CLIConsole : IConsole
	{
		public void WriteLine(object context) => Console.WriteLine(context);
		public void WriteLine(string context) => Console.WriteLine(context);
		public void WriteLine(int context) => Console.WriteLine(context);
		public void WriteLine(float context) => Console.WriteLine(context);
	}

	public class WorkerConsole : CLIConsole
	{
		private Stopwatch mStopwatch = new Stopwatch();

		public void WorkCompleted(string workName)
		{
			mStopwatch.Stop();
			WriteLine($"##### {workName} #####");
			WriteLine($"# [{workName}] completed...");
			WriteLine($"# Elapsed time : {mStopwatch.ElapsedMilliseconds}ms");
			WriteLine($"# Work completed time : {DateTime.Now}");
			DrawSeparator();
		}

		public void WorkStart(string workName)
		{
			DrawSeparator();
			WriteLine($"##### {workName} #####");
			WriteLine($"# [{workName}] start...");
			WriteLine($"# Work start time : {DateTime.Now}");
			mStopwatch.Restart();
		}

		public void DrawSeparator()
		{
			WriteLine("------------------------------"); // 30
		}

		public void DrawEmptyLine()
		{
			WriteLine("");
		}
	}

	public class GeneratorConsole : WorkerConsole
	{
		public void UnexpectedError(string message)
		{
			WriteLine($"# [Generator Error] {message}");
			WriteLine($"# Suspended Time {DateTime.Now}");
			DrawSeparator();
		}
	}
	public class ExcelParserConsole : WorkerConsole
	{
		public void UnexpectedError(string message)
		{
			WriteLine($"# [ExcelParser Error] {message}");
			WriteLine($"# Suspended Time {DateTime.Now}");
			DrawSeparator();
		}
	}
}
