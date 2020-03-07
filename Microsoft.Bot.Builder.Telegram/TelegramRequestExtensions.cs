using Newtonsoft.Json.Linq;
using Telegram.Bot.Requests.Abstractions;

namespace Microsoft.Bot.Builder.Telegram
{
	public static class TelegramRequestExtensions
	{
		public static object ToChannelData<T>(this IRequest<T> requestBase)
		{
			return new JObject(new JProperty("method", requestBase.MethodName),
							   new JProperty("parameters", JObject.FromObject(requestBase)));
		}
	}
}