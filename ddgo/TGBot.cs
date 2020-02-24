using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace ddgo
{
	public class TGBot : IBot
	{
		/// <inheritdoc />
		public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = new CancellationToken())
		{
			if (turnContext.Activity.Type == ActivityTypes.Message)
			{
				await turnContext.SendActivityAsync(turnContext.Activity.Text, cancellationToken: cancellationToken);
			}
		}
	}
}