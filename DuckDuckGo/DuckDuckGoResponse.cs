using System.Collections.Generic;
using Newtonsoft.Json;

namespace DuckDuckGo
{
	public class DuckDuckGoResponse<T>
	{
		[JsonConverter(typeof(VqdJsonConverter))]
		[JsonProperty("vqd")]
		public string Vqd { get; set; }

		[JsonProperty("next")]
		public string Next { get; set; }

		[JsonProperty("results")]
		public IList<T> Results { get; set; }

		[JsonProperty("query")]
		public string Query { get; set; }
	}
}