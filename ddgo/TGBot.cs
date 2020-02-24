using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineQueryResults;

namespace ddgo
{
	public class TGBot : IBot
	{
		/// <inheritdoc />
		public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = new CancellationToken())
		{
			if (turnContext.Activity.ChannelId == "telegram")
			{
				var inlineQuery = turnContext.Activity.GetChannelData<InlineQuery>();

				var answer =
					new JObject(new JProperty("method", "answerInlineQuery"),
								new JProperty("parameter", new AnswerInlineQueryRequest(inlineQuery.Id, new[]
								{
									new InlineQueryResultPhoto("0",
															   "https://klike.net/uploads/posts/2019-03/1551516106_1.jpg",
															   "https://klike.net/uploads/posts/2019-03/1551516106_1.jpg"),
								})));

				var reply = turnContext.Activity.CreateReply();
				reply.ChannelData = answer;
				await turnContext.SendActivityAsync(reply, cancellationToken);
			}
		}
	}
}