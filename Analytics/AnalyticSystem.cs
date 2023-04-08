namespace AdvancedWorldGen.Analytics;

public class AnalyticSystem : ModSystem
{
	public static string DataPath => Path.Combine(AdvancedWorldGenMod.FolderPath, "Id.bin");
	public static AnalyticSystem Instance => ModContent.GetInstance<AnalyticSystem>();
	private long Id;
	private HttpClient HttpClient = null!;

	public override void OnModLoad()
	{
		if (File.Exists(DataPath))
		{
			if (WorldgenSettings.Instance.Analytics)
			{
				using FileStream inputFileStream = new(DataPath, FileMode.Open, FileAccess.Read);
				using BinaryReader binaryReader = new(inputFileStream);
				Id = binaryReader.ReadInt64();
			}
			else
			{
				File.Delete(DataPath);
			}
		}
		else if (WorldgenSettings.Instance.Analytics)
		{
			Id = BitConverter.ToInt64(BitConverter.GetBytes(Main.rand.NextDouble()));
			using FileStream outputFileStream = new(DataPath, FileMode.Create, FileAccess.Write);
			using BinaryWriter binaryWriter = new(outputFileStream);
			binaryWriter.Write(Id);
		}

		if (WorldgenSettings.Instance.Analytics)
		{
			HttpClient = new HttpClient();
		}
	}

	public void SendData(string? log = null)
	{
		if (WorldgenSettings.Instance.Analytics)
		{
			const string url = "http://awg.alwaysdata.net/register.php";
			// Create the request content
			List<KeyValuePair<string, string>> arguments = new()
			{
				new KeyValuePair<string, string>("id", Id.ToString()),
				new KeyValuePair<string, string>("options", OptionsParser.GetJsonText()),
				new KeyValuePair<string, string>("overhauled", WorldgenSettings.Instance.FasterWorldgen.ToInt().ToString()),
				new KeyValuePair<string, string>("version", AdvancedWorldGenMod.Instance.Version.ToString())
			};
			if (log != null)
			{
				arguments.Add(new KeyValuePair<string, string>("log", log));
			}

			if (ModifiedWorld.Instance.Times != null)
			{
				JObject jsonObject = new();
				foreach ((string? key, TimeSpan value) in ModifiedWorld.Instance.Times)
				{
					jsonObject.Add(key, value.Milliseconds);
				}
				arguments.Add(new KeyValuePair<string, string>("time", jsonObject.ToString()));
			}
			
			FormUrlEncodedContent content = new(arguments);

			// Send the POST request without waiting for a response
			HttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, url) { Content = content });
		}
	}
}