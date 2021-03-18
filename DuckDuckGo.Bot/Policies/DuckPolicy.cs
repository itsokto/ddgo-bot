using System.Net;
using Polly;
using Polly.Retry;
using Refit;

namespace DuckDuckGo.Bot.Policies
{
	public static class DuckPolicy
	{
		public static AsyncRetryPolicy<T> RetryOnInvalidToken<T>()
		{
			return Policy<T>.Handle<ApiException>(ex => ex.StatusCode == HttpStatusCode.Forbidden).RetryAsync(3);
		}
	}
}