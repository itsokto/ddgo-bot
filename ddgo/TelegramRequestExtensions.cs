using Newtonsoft.Json.Linq;
using Telegram.Bot.Requests;

namespace ddgo
{
	public static class TelegramRequestExtensions
	{
		public static object ToChannelData<T>(this RequestBase<T> requestBase)
		{
			return new JObject(new JProperty("method", requestBase.MethodName),
							   new JProperty("parameters", JObject.FromObject(requestBase)));
		}
	}
}