using System;

namespace MCGCore
{
	public static class MathExtension
	{
		public static int Clamp(int value, int min, int max)
		{
			if (value < min)
			{
				return min;
			}
			else if (value > max)
			{
				return max;
			}

			return value;
		}
		public static float Clamp(float value, float min, float max)
		{
			if (value < min)
			{
				return min;
			}
			else if (value > max)
			{
				return max;
			}

			return value;
		}
		public static byte Clamp(byte value, int min, int max)
		{
			return Clamp(value, min, max);
		}

		public static float Lerp(float start, float end, float t) => start + (end - start) * t;
		public static int Lerp(int start, int end, float t) => (int)(start + (end - start) * t);
		public static byte Lerp(byte start, byte end, float t) => (byte)(start + (end - start) * t);

		public static float Remap(this float value, in (float, float) input, in (float, float) output)
		{
			float t = (value - input.Item1) / (input.Item2 - input.Item1);
			return output.Item1 + (output.Item2 - output.Item1) * t;
		}
		public static int Remap(this int value, in (float, float) input, in (float, float) output)
		{
			float t = (value - input.Item1) / (input.Item2 - input.Item1);
			return (int)(output.Item1 + (output.Item2 - output.Item1) * t);
		}
	}
}
