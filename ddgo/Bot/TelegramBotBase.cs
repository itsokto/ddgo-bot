using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Telegram.Bot.Types;

namespace ddgo.Bot
{
	public class TelegramBotBase : ActivityHandler
	{
		/// <inheritdoc />
		public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
		{
			await base.OnTurnAsync(turnContext, cancellationToken);

			if (turnContext.Activity.ChannelId == "telegram")
			{
				var update = turnContext.Activity.GetChannelData<Update>();

				await OnUpdateAsync(turnContext, update, cancellationToken);
			}
		}

		protected virtual Task OnUpdateAsync(ITurnContext turnContext, Update update, CancellationToken cancellationToken = default)
		{
			return Task.CompletedTask;
		}
	}
}