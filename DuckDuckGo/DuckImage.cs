using Newtonsoft.Json;

namespace DuckDuckGo
{
	public class DuckImage
	{
		[JsonProperty("height")]
		public long Height { get; set; }

		[JsonProperty("image")]
		public string Image { get; set; }

		[JsonProperty("url")]
		public string Url { get; set; }

		[JsonProperty("thumbnail")]
		public string Thumbnail { get; set; }

		[JsonProperty("title")]
		public string Title { get; set; }

		[JsonProperty("width")]
		public long Width { get; set; }
	}
}