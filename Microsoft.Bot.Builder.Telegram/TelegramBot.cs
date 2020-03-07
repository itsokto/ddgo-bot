using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Microsoft.Bot.Builder.Telegram
{
	public class TelegramBot : TelegramBotBase
	{
		/// <inheritdoc />
		protected override Task OnUpdateAsync(ITurnContext turnContext, Update update, CancellationToken cancellationToken = default)
		{
			switch (update.Type)
			{
				case UpdateType.InlineQuery:
					return OnInlineQueryAsync(turnContext, update.InlineQuery, cancellationToken);
				default:
					return Task.CompletedTask;
			}
		}

		protected virtual Task OnInlineQueryAsync(ITurnContext turnContext, InlineQuery inlineQuery, CancellationToken cancellationToken = default)
		{
			return Task.CompletedTask;
		}
	}
}