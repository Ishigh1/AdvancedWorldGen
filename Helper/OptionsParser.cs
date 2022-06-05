using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AdvancedWorldGen.Base;
using AdvancedWorldGen.CustomSized;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Terraria.WorldBuilding;

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

			ModifiedWorld.Instance.OptionHelper.Import(optionNames);
		}


		if (jsonObject.TryGetValue("customParams", out jsonNode) && jsonNode is JObject customParams)
		{
			Params @params = ModifiedWorld.Instance.OptionHelper.WorldSettings.Params;
			foreach ((string? key, JToken? value) in customParams)
				if (value is JValue jValue && @params.Data.TryGetValue(key, out object? dataValue) && jValue.Value != null && dataValue != null)
					@params.Data[key] = Convert.ChangeType(jValue.Value, dataValue.GetType());
		}

		if (jsonObject.TryGetValue("legacyParams", out jsonNode) && jsonNode is JObject legacyParams)
		{
			WorldGenConfiguration worldGenConfiguration = AdvancedWorldGenMod.Instance.UiChanger.WorldGenConfigurator.WorldGenConfiguration;
			foreach (JObject jObject in typeof(WorldGenConfiguration)
				         .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
				         .Select(fieldInfo => (JObject)fieldInfo.GetValue(worldGenConfiguration)!))
			foreach ((string? key, JToken? value) in legacyParams)
				if (value is JValue jValue && jObject.TryGetValue(key, out JToken? jToken) && jValue.Value != null && jToken is JValue jValue2)
					jValue2.Value = jValue.Value;
		}
	}

	public static string GetJsonText()
	{
		JObject jsonObject = new();

		JArray optionArray = new();
		foreach (string optionName in ModifiedWorld.Instance.OptionHelper.Export())
			optionArray.Add(optionName);
		jsonObject.Add("options", optionArray);

		JObject customParams = new();
		foreach ((string key, object? value) in ModifiedWorld.Instance.OptionHelper.WorldSettings.Params.Data)
			customParams.Add(key, new JValue(value));
		jsonObject.Add("customParams", customParams);

		JObject legacyParams = new();
		WorldGenConfiguration worldGenConfiguration = AdvancedWorldGenMod.Instance.UiChanger.WorldGenConfigurator.WorldGenConfiguration;
		foreach (JObject jObject in typeof(WorldGenConfiguration)
			         .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
			         .Select(fieldInfo => (JObject)fieldInfo.GetValue(worldGenConfiguration)!))
		foreach ((string key, JToken? value) in jObject)
			legacyParams.Add(key, value);
		jsonObject.Add("legacyParams", legacyParams);

		return jsonObject.ToString();
	}
}