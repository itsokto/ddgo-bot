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
		protected override async Task OnInlineQueryAsync(ITurnContext turnContext, InlineQuery inlineQuery,
														 CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrWhiteSpace(inlineQuery.Query))
			{
				return;
			}

			var statePropertyAccessor = _userState.CreateProperty<DuckUserState>(nameof(DuckUserState));
			var user = await statePropertyAccessor.GetAsync(turnContext, () => new DuckUserState(), cancellationToken);

			var duckResponse = await _imagesService.GetAsync(inlineQuery.Query, user, cancellationToken);

			var answer = CreateAnswerInlineQuery(inlineQuery, duckResponse);

			var reply = turnContext.CreateBotReply(answer);

			user.Vqd = duckResponse.Vqd;
			user.Next = duckResponse.Next;
			user.Query = inlineQuery.Query;

			await _userState.SaveChangesAsync(turnContext, false, cancellationToken);

			await turnContext.SendActivityAsync(reply, cancellationToken);
		}

		private AnswerInlineQueryRequest CreateAnswerInlineQuery(InlineQuery inlineQuery, DuckResponse<DuckImage> response)
		{
			if (response.Results == null || !response.Results.Any() && string.IsNullOrWhiteSpace(response.Next))
			{
				return new AnswerInlineQueryRequest(inlineQuery.Id, Enumerable.Empty<InlineQueryResultBase>())
				{
					NextOffset = string.Empty
				};
			}

			var inlineQueryPhotos = response.Results.Take(50)
											.Select((image, i) => new InlineQueryResultPhoto(i.ToString(), image.Image, image.Thumbnail))
											.ToList();

			var offset = GetOffset(inlineQuery.Offset, inlineQueryPhotos.Count);

			return new AnswerInlineQueryRequest(inlineQuery.Id, inlineQueryPhotos) { NextOffset = offset };
		}

		private string GetOffset(string queryOffset, long count)
		{
			if (string.IsNullOrWhiteSpace(queryOffset))
			{
				queryOffset = "0";
			}

			return long.TryParse(queryOffset, out var offset) ? (offset + count).ToString() : string.Empty;
		}
	}
}