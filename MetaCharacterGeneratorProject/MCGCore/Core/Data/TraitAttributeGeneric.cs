using System;
using System.Collections.Generic;
using System.Text;

namespace MCGCore
{
	public static class GlobalTrait
	{
		public const int INITIAL_CHANCE = 1000;
	}

	public class IntTraitAttribute : IMetadataAttribute
	{
		public string trait_type { get; set; }
		public int value { get; set; }
		public int chance { get; set; }

		public IntTraitAttribute() { }
		public IntTraitAttribute(string trait_type, int value, int chance = GlobalTrait.INITIAL_CHANCE)
		{
			this.trait_type = trait_type;
			this.value = value;
			this.chance = chance < 1 ? 1 : chance;
		}
		public IntTraitAttribute(IntTraitAttribute copy)
		{
			this.trait_type = copy.trait_type;
			this.value = copy.value;
		}

		public IMetadataAttribute Copy()
		{
			return new IntTraitAttribute(this);
		}

		public void SetChance(int chance)
		{
			this.chance = chance < 1 ? 1 : chance;
		}

		public byte[] GetHeshBytes()
		{
			var traitBytes = StreamManager.GetBytes(trait_type);
			var valueBytes = StreamManager.GetBytes(value);
			var chanceBytes = StreamManager.GetBytes(chance);

			List<byte> buffer = new List<byte>();

			buffer.AddRange(traitBytes);
			buffer.AddRange(valueBytes);
			buffer.AddRange(chanceBytes);

			return buffer.ToArray();
		}

		public string GetHeshString()
		{
			return HashManager.GetHeshString(GetHeshBytes());
		}

		public override bool Equals(object obj)
		{
			IntTraitAttribute c = obj as IntTraitAttribute;
			if (c == null)
			{
				return false;
			}
			return this.trait_type == c.trait_type && this.value == c.value;
		}

		public override int GetHashCode()
		{
			return trait_type.GetHashCode() + value.GetHashCode();
		}

		public override string ToString() => $"({trait_type} : {value})";
	}

	public class FloatTraitAttribute : IMetadataAttribute
	{
		public string trait_type { get; set; }
		public float value { get; set; }
		public int chance { get; set; }

		public FloatTraitAttribute() { }
		public FloatTraitAttribute(string trait_type, float value, int chance = GlobalTrait.INITIAL_CHANCE)
		{
			this.trait_type = trait_type;
			this.value = value;
			this.chance = chance < 1 ? 1 : chance;
		}
		public FloatTraitAttribute(FloatTraitAttribute copy)
		{
			this.trait_type = copy.trait_type;
			this.value = copy.value;
		}

		public IMetadataAttribute Copy()
		{
			return new FloatTraitAttribute(this);
		}

		public void SetChance(int chance)
		{
			this.chance = chance < 1 ? 1 : chance;
		}
		public byte[] GetHeshBytes()
		{
			var traitBytes = StreamManager.GetBytes(trait_type);
			var valueBytes = StreamManager.GetBytes(value);
			var chanceBytes = StreamManager.GetBytes(chance);

			List<byte> buffer = new List<byte>();

			buffer.AddRange(traitBytes);
			buffer.AddRange(valueBytes);
			buffer.AddRange(chanceBytes);

			return buffer.ToArray();
		}

		public string GetHeshString()
		{
			return HashManager.GetHeshString(GetHeshBytes());
		}

		public override bool Equals(object obj)
		{
			FloatTraitAttribute c = obj as FloatTraitAttribute;
			if (c == null)
			{
				return false;
			}
			return this.trait_type == c.trait_type && this.value == c.value;
		}

		public override int GetHashCode()
		{
			return trait_type.GetHashCode() + value.GetHashCode();
		}

		public override string ToString() => $"({trait_type} : {value})";
	}

	public class StringTraitAttribute : IMetadataAttribute
	{
		public string trait_type { get; set; }
		public string value { get; set; }
		public int chance { get; set; }

		public StringTraitAttribute() { }

		public StringTraitAttribute(string trait_type, string value, int chance = GlobalTrait.INITIAL_CHANCE)
		{
			this.trait_type = trait_type;
			this.value = value;
			this.chance = chance < 1 ? 1 : chance;
		}
		public StringTraitAttribute(StringTraitAttribute copy)
		{
			this.trait_type = copy.trait_type;
			this.value = copy.value;
		}

		public IMetadataAttribute Copy()
		{
			return new StringTraitAttribute(this);
		}

		public void SetChance(int chance)
		{
			this.chance = chance < 1 ? 1 : chance;
		}

		public byte[] GetHeshBytes()
		{
			var traitBytes = StreamManager.GetBytes(trait_type);
			var valueBytes = StreamManager.GetBytes(value);
			var chanceBytes = StreamManager.GetBytes(chance);

			List<byte> buffer = new List<byte>();

			buffer.AddRange(traitBytes);
			buffer.AddRange(valueBytes);
			buffer.AddRange(chanceBytes);

			return buffer.ToArray();
		}

		public string GetHeshString()
		{
			return HashManager.GetHeshString(GetHeshBytes());
		}

		public override bool Equals(object obj)
		{
			StringTraitAttribute c = obj as StringTraitAttribute;
			if (c == null)
			{
				return false;
			}
			return this.trait_type == c.trait_type && this.value == c.value;
		}

		public override int GetHashCode()
		{
			return trait_type.GetHashCode() + value.GetHashCode();
		}

		public override string ToString() => $"({trait_type} : {value})";
	}
}
