using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;

namespace DuckDuckGo
{
	public class DuckDuckGoApi
	{
		private static readonly Regex VqdRegex = new Regex(@"vqd=\'([\d-]+)\'", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

		public async Task<string> GetToken(string query, CancellationToken cancellationToken = default)
		{
			var response = await "https://duckduckgo.com".SetQueryParam("q", query).GetStringAsync(cancellationToken).ConfigureAwait(false);
			var math = VqdRegex.Match(response);
			if (!math.Success)
			{
				throw new InvalidOperationException("Can't parse vqd!");
			}

			return math.Groups[1].Value;
		}

		public async Task<DuckDuckGoResponse<DuckImage>> Images(string query,
																string vqd = null,
																SearchFilter searchFilter = SearchFilter.Off,
																CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrWhiteSpace(vqd))
			{
				vqd = await GetToken(query, cancellationToken);
			}

			return await "https://duckduckgo.com/i.js".SetQueryParams(new
													  {
														  o = "json",
														  q = query,
														  vqd,
														  f = ",,,",
														  p = (int) searchFilter
													  })
													  .GetJsonAsync<DuckDuckGoResponse<DuckImage>>(cancellationToken);
		}
	}
}