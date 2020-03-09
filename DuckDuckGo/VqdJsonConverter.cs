using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DuckDuckGo
{
	public class VqdJsonConverter : JsonConverter
	{
		/// <inheritdoc />
		public override bool CanWrite { get; } = false;

		public override bool CanConvert(Type objectType)
		{
			return objectType.IsAssignableFrom(typeof(string));
		}

		/// <inheritdoc />
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType != JsonToken.StartObject)
			{
				return null;
			}

			var jObject = JObject.Load(reader);

			return jObject.First.First.ToString();
		}
	}
}