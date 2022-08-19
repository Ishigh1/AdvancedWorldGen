namespace AdvancedWorldGen.Helper;

public class OptionsParser
{
	public static void Parse(string jsonText)
	{
		JObject jsonObject;
		try
		{
			jsonObject = JObject.Parse(jsonText);
		}
		catch (JsonReaderException)
		{
			AdvancedWorldGenMod.Instance.Logger.Info($"Error parsing JSON: {jsonText} is not valid JSON");
			return;
		}

		if (jsonObject.TryGetValue("options", out JToken? jsonNode) && jsonNode is JArray optionArray)
		{
			List<string> optionNames = new();
			foreach (JToken? node in optionArray)
				if (node is JValue { Value: string name })
					optionNames.Add(name);

			OptionHelper.Import(optionNames);
		}


		if (jsonObject.TryGetValue("customParams", out jsonNode) && jsonNode is JObject customParams)
		{
			Params @params = OptionHelper.WorldSettings.Params;
			foreach ((string? key, JToken? value) in customParams)
				if (value is JValue jValue && @params.TryGetValue(key, out object? dataValue) && jValue.Value != null && dataValue != null)
				{
					if (dataValue is Enum)
						@params[key] = Enum.Parse(dataValue.GetType(), (string)jValue.Value);
					else
						@params[key] = Convert.ChangeType(jValue.Value, dataValue.GetType());
				}
		}

		if (jsonObject.TryGetValue("legacyParams", out jsonNode) && jsonNode is JObject legacyParams)
		{
			WorldGenConfiguration worldGenConfiguration = AdvancedWorldGenMod.Instance.UiChanger.WorldGenConfigurator!.WorldGenConfiguration;
			foreach (JObject jObject in typeof(WorldGenConfiguration)
				         .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
				         .Select(fieldInfo => (JObject)fieldInfo.GetValue(worldGenConfiguration)!))
				CopyJson(jObject, legacyParams);
		}
	}

	private static void CopyJson(JObject source, JObject target)
	{
		foreach ((string? key, JToken? jToken1) in source)
			if (target.TryGetValue(key, out JToken? jToken2))
				switch (jToken1)
				{
					case JValue jValue1 when jToken2 is JValue jValue2:
						jValue1.Value = jValue2.Value;
						break;
					case JObject jObject1 when jToken2 is JObject jObject2:
						CopyJson(jObject1, jObject2);
						break;
				}
	}

	public static string GetJsonText()
	{
		JObject jsonObject = new();

		JArray optionArray = new();
		foreach (string optionName in OptionHelper.Export())
			optionArray.Add(optionName);
		jsonObject.Add("options", optionArray);

		JObject customParams = new();
		foreach ((string key, object? value) in OptionHelper.WorldSettings.Params)
			if (value is Enum)
				customParams.Add(key, Enum.GetName(value.GetType(), value));
			else
				customParams.Add(key, new JValue(value));

		jsonObject.Add("customParams", customParams);

		JObject legacyParams = new();
		WorldGenConfiguration worldGenConfiguration = AdvancedWorldGenMod.Instance.UiChanger.WorldGenConfigurator!.WorldGenConfiguration;
		foreach (JObject jObject in typeof(WorldGenConfiguration)
			         .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
			         .Select(fieldInfo => (JObject)fieldInfo.GetValue(worldGenConfiguration)!))
		foreach ((string key, JToken? value) in jObject)
			legacyParams.Add(key, value);
		jsonObject.Add("legacyParams", legacyParams);

		return jsonObject.ToString();
	}
}