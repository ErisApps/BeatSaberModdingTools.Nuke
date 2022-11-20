using System.Text.Json;
using System.Text.Json.Serialization;

namespace BeatSaberModdingTools.Nuke.Helpers.Converters
{
	internal class NullableUriConverter : JsonConverter<Uri>
	{
		public override Uri? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType == JsonTokenType.Null)
			{
				return null;
			}

			return Uri.TryCreate(reader.GetString(), UriKind.RelativeOrAbsolute, out var parsedUri) ? parsedUri : null;
		}

		public override void Write(Utf8JsonWriter writer, Uri? value, JsonSerializerOptions options)
		{
			if (value == null)
			{
				writer.WriteNullValue();
			}
			else
			{
				writer.WriteStringValue(value.ToString());
			}
		}
	}
}
