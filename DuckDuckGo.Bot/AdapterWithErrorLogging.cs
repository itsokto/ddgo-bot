using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DuckDuckGo.Bot
{
	public class AdapterWithErrorLogging : BotFrameworkHttpAdapter
	{
		public AdapterWithErrorLogging(IConfiguration configuration, ILogger<BotFrameworkHttpAdapter> logger) : base(configuration, logger)
		{
			OnTurnError = (turnContext, exception) =>
			{
				var message = $"[OnTurnError]: {exception.Message}";

				if (exception is ErrorResponseException responseException)
				{
					var stringBuilder = new StringBuilder();
					stringBuilder.AppendLine(message);
					stringBuilder.AppendLine("[Response]:");
					stringBuilder.AppendLine(responseException.Response.Content);

					logger.LogError(responseException, stringBuilder.ToString());
				}
				else
				{
					logger.LogError(exception, message);
				}

				return Task.CompletedTask;
			};
		}
	}
}