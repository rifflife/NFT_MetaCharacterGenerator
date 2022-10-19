using Newtonsoft.Json;
using System;
using System.IO;

namespace MCGCore
{
	public static class JsonHandler
	{
		public static JsonSerializerSettings LoadOption { get; } = new JsonSerializerSettings()
		{
			TypeNameHandling = TypeNameHandling.Auto,
			NullValueHandling = NullValueHandling.Ignore,
		};
		public static JsonSerializerSettings SaveOption { get; } = new JsonSerializerSettings()
		{
			TypeNameHandling = TypeNameHandling.Auto,
			NullValueHandling = NullValueHandling.Ignore,
			Formatting = Formatting.Indented
		};
		public static JsonSerializerSettings JsonTokenSetting { get; } = new JsonSerializerSettings()
		{
			Formatting = Formatting.Indented,
			NullValueHandling = NullValueHandling.Ignore
		};

		public static void SaveToFile<T>(T instance, Uri fileUri)
		{
			using (FileStream fs = new FileStream(fileUri.OriginalString, FileMode.Create))
			using (StreamWriter sw = new StreamWriter(fs))
			{
				string saveContext = ToJson(instance);
				sw.Write(saveContext);
			}
		}
		public static T LoadFromFile<T>(Uri fileUri)
		{
			string loadContext;

			using (FileStream fs = new FileStream(fileUri.OriginalString, FileMode.Open))
			using (StreamReader sr = new StreamReader(fs))
			{
				loadContext = sr.ReadToEnd();
			}

			return ToInstance<T>(loadContext);
		}
		public static string ToJson<T>(T instance)
		{
			return JsonConvert.SerializeObject(instance, SaveOption);
		}
		public static T ToInstance<T>(string json)
		{
			return JsonConvert.DeserializeObject<T>(json, LoadOption);
		}
		public static string ToJsonToken(object instance)
		{
			return JsonConvert.SerializeObject(instance, JsonTokenSetting);
		}
		public static void SaveToJsonToken(object instance, Uri fileUri)
		{
			using (FileStream fs = new FileStream(fileUri.OriginalString, FileMode.Create))
			using (StreamWriter sw = new StreamWriter(fs))
			{
				string saveContext = ToJsonToken(instance);
				sw.Write(saveContext);
			}
		}
	}
}
