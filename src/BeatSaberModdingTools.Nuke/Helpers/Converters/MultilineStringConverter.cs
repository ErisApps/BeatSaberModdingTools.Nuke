using System.Text.Json;
using System.Text.Json.Serialization;

namespace BeatSaberModdingTools.Nuke.Helpers.Converters
{
	internal class MultilineStringConverter : JsonConverter<string>
	{
		public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType == JsonTokenType.StartArray)
			{
				reader.Read();

				var list = new List<string>();
				while (reader.TokenType != JsonTokenType.EndArray)
				{
					var line = reader.GetString();
					if (line != null)
					{
						list.Add(line);
					}

					reader.Read();
				}

				return string.Join("\n", list);
			}

			return reader.GetString();
		}

		public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
		{
			var list = value.Split('\n');
			if (list.Length == 1)
			{
				writer.WriteStringValue(value);
			}
			else
			{
				writer.WriteStartArray();
				foreach (var line in list)
				{
					writer.WriteStringValue(line);
				}

				writer.WriteEndArray();
			}
		}
	}
}