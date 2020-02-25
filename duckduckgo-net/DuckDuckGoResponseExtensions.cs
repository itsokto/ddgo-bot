using System.Threading;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;

namespace DuckDuckGo
{
	public static class DuckDuckGoResponseExtensions
	{
		public static bool CanNext<T>(this DuckDuckGoResponse<T> duckDuckGoResponse) where T : class
		{
			return !string.IsNullOrWhiteSpace(duckDuckGoResponse.Next);
		}

		public static Task<DuckDuckGoResponse<T>> Next<T>(this DuckDuckGoResponse<T> duckDuckGoResponse, CancellationToken cancellationToken = default)
			where T : class
		{
			return $"https://duckduckgo.com/{duckDuckGoResponse.Next}".SetQueryParam("vqd", duckDuckGoResponse.Vqd)
																	  .GetJsonAsync<DuckDuckGoResponse<T>>(cancellationToken);
		}
	}
}