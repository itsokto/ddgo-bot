using System;
using System.Net;
using Polly;
using Polly.Fallback;
using Polly.Retry;
using Refit;

namespace DuckDuckGo.Bot.Policies
{
	public static class DuckPolicy
	{
		public static AsyncRetryPolicy<DuckResponse<T>> RetryOnInvalidToken<T>()
		{
			return Policy<DuckResponse<T>>.Handle<ApiException>(ex => ex.StatusCode == HttpStatusCode.Forbidden).RetryAsync(3);
		}

		public static AsyncFallbackPolicy<DuckResponse<T>> FallbackOnInvalidToken<T>()
		{
			return Policy<DuckResponse<T>>.Handle<ApiException>(ex => ex.StatusCode == HttpStatusCode.Forbidden)
										  .FallbackAsync(Activator.CreateInstance<DuckResponse<T>>());
		}
	}
}