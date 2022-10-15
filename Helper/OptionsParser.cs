namespace AdvancedWorldGen.Helper;

public static class OptionsParser
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

		if (jsonObject.TryGetValue("vanillaParams", out JToken? jsonNode) && jsonNode is JObject vanillaParams)
		{
			JToken? seed = vanillaParams.GetValue("seed");
			if (seed is { Type: JTokenType.String })
			{
				ReflectionAccessor<string> seedAccessor = new FieldAccessor<string>(typeof(UIWorldCreation), "_optionSeed", OptionHelper.WorldSettings.UIWorldCreation);
				seedAccessor.Value = seed.ToString();
				new ReflectionCaller(typeof(UIWorldCreation), "UpdateInputFields", OptionHelper.WorldSettings.UIWorldCreation).Call();
			}

			JToken? evil = vanillaParams.GetValue("evil");
			if (evil is { Type: JTokenType.String })
			{
				ReflectionAccessor<int> evilAccessor = new FieldAccessor<int>(typeof(UIWorldCreation), "_optionEvil", OptionHelper.WorldSettings.UIWorldCreation);
				evilAccessor.Value = int.Parse(evil.Value<string>());
				new ReflectionCaller(typeof(UIWorldCreation), "UpdateSliders", OptionHelper.WorldSettings.UIWorldCreation).Call();
				new ReflectionCaller(typeof(UIWorldCreation), "UpdatePreviewPlate", OptionHelper.WorldSettings.UIWorldCreation).Call();
			}
		}

		if (jsonObject.TryGetValue("options", out jsonNode) && jsonNode is JArray optionArray)
		{
			List<string> optionNames = new();
			foreach (JToken? node in optionArray)
				if (node is JValue { Value: string name })
					optionNames.Add(name);

			OptionHelper.Import(optionNames);
		}


		if (jsonObject.TryGetValue("customParams", out jsonNode) && jsonNode is JObject customParams)
		{
			foreach ((string? key, JToken? value) in customParams)
				if (value is JValue jValue && Params.TryGetValue(key, out object? dataValue) && jValue.Value != null && dataValue != null)
				{
					Params.Set(key,
						dataValue is Enum
							? Enum.Parse(dataValue.GetType(), (string)jValue.Value)
							: Convert.ChangeType(jValue.Value, dataValue.GetType()));
				}
		}

		if (jsonObject.TryGetValue("legacyParams", out jsonNode) && jsonNode is JObject legacyParams)
		{
			WorldGenConfiguration worldGenConfiguration = AdvancedWorldGenMod.Instance.UiChanger.VanillaWorldGenConfigurator!.Configuration;
			foreach (JObject? jObject in typeof(WorldGenConfiguration)
				         .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
				         .Select(fieldInfo => (JObject)fieldInfo.GetValue(worldGenConfiguration)!))
				CopyJson(jObject, legacyParams);
		}

		if (jsonObject.TryGetValue("overhauledParams", out jsonNode) && jsonNode is JObject overhauledParams)
		{
			JObject? worldGenConfiguration = OverhauledWorldGenConfigurator.Root;
			CopyJson(worldGenConfiguration, overhauledParams);
		}
	}

	private static void CopyJson(JObject? source, JObject target)
	{
		if (source is null) return;
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

		JObject vanillaParams = new();
		ReflectionAccessor<string> seedAccessor = new FieldAccessor<string>(typeof(UIWorldCreation), "_optionSeed", OptionHelper.WorldSettings.UIWorldCreation);
		vanillaParams.Add("seed", seedAccessor.Value);
		ReflectionAccessor<int> evilAccessor = new FieldAccessor<int>(typeof(UIWorldCreation), "_optionEvil", OptionHelper.WorldSettings.UIWorldCreation);
		vanillaParams.Add("evil", evilAccessor.Value.ToString());
		jsonObject.Add("vanillaParams", vanillaParams);

		JArray optionArray = new();
		foreach (string optionName in OptionHelper.Export())
			optionArray.Add(optionName);
		jsonObject.Add("options", optionArray);

		JObject customParams = new();
		foreach ((string key, object? value) in Params.Data)
			if (value is Enum)
				customParams.Add(key, Enum.GetName(value.GetType(), value));
			else
				customParams.Add(key, new JValue(value));
		jsonObject.Add("customParams", customParams);

		JObject legacyParams = new();
		WorldGenConfiguration worldGenConfiguration = AdvancedWorldGenMod.Instance.UiChanger.VanillaWorldGenConfigurator!.Configuration;
		foreach (JObject jObject in typeof(WorldGenConfiguration)
			         .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
			         .Select(fieldInfo => (JObject)fieldInfo.GetValue(worldGenConfiguration)!))
		foreach ((string key, JToken? value) in jObject)
			if (value is not JObject jObject2 || jObject2.Count > 0)
				legacyParams.Add(key, value);
		jsonObject.Add("legacyParams", legacyParams);
		
		jsonObject.Add("overhauledParams", OverhauledWorldGenConfigurator.Root);
		
		return jsonObject.ToString();
	}
}