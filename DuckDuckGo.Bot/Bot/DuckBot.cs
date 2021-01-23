using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DuckDuckGo.Bot.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Telegram;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineQueryResults;

namespace DuckDuckGo.Bot.Bot
{
	public class DuckBot : TelegramBot
	{
		private readonly IImagesService _imagesService;
		private readonly UserState _userState;

		public DuckBot(IImagesService imagesService, UserState userState)
		{
			_imagesService = imagesService;
			_userState = userState;
		}

		/// <inheritdoc />
		protected override async Task OnInlineQueryAsync(ITurnContext turnContext, InlineQuery inlineQuery, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrWhiteSpace(inlineQuery.Query))
			{
				return;
			}

			var statePropertyAccessor = _userState.CreateProperty<DuckUserState>(nameof(DuckUserState));
			var user = await statePropertyAccessor.GetAsync(turnContext, () => new DuckUserState(), cancellationToken);

			var duckDuckGoResponse = await _imagesService.GetAsync(inlineQuery.Query, user, cancellationToken);

			var answer = CreateAnswerInlineQuery(inlineQuery, duckDuckGoResponse);

			var reply = turnContext.CreateBotReply(answer);

			user.Vqd = duckDuckGoResponse.Vqd;
			user.Next = duckDuckGoResponse.Next;
			user.Query = inlineQuery.Query;

			await _userState.SaveChangesAsync(turnContext, false, cancellationToken);

			await turnContext.SendActivityAsync(reply, cancellationToken);
		}

		private AnswerInlineQueryRequest CreateAnswerInlineQuery(InlineQuery inlineQuery, DuckDuckGoResponse<DuckImage> response)
		{
			var inlineQueryPhotos = response.Results
				.Take(50)
				.Select((image, i) => new InlineQueryResultPhoto(i.ToString(), image.Image, image.Thumbnail))
				.ToList();

			if (string.IsNullOrWhiteSpace(inlineQuery.Offset))
			{
				inlineQuery.Offset = "0";
			}

			var offset = !string.IsNullOrWhiteSpace(response.Next)
				? (int.Parse(inlineQuery.Offset) + inlineQueryPhotos.Count).ToString()
				: string.Empty;

			return new AnswerInlineQueryRequest(inlineQuery.Id, inlineQueryPhotos) { NextOffset = offset };
		}
	}
}