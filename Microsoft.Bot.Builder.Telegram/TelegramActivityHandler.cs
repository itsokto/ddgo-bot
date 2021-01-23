using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Microsoft.Bot.Builder.Telegram
{
	public class TelegramActivityHandler : ActivityHandler
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