using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Telegram;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineQueryResults;

namespace DuckDuckGo.Bot.Bot
{
	public class DuckBot : TelegramBot
	{
		private readonly DuckDuckGoApi _duckDuckGoApi;

		private readonly UserState _userState;

		public DuckBot(DuckDuckGoApi duckDuckGoApi, UserState userState)
		{
			_duckDuckGoApi = duckDuckGoApi;
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

			var duckDuckGoResponse = await GetImagesAsync(inlineQuery, user, cancellationToken);

			var answer = CreateAnswerInlineQuery(inlineQuery, duckDuckGoResponse);

			var reply = turnContext.CreateBotReply(answer);

			user.Vqd = duckDuckGoResponse.Vqd;
			user.Next = duckDuckGoResponse.Next;
			user.Query = inlineQuery.Query;

			await _userState.SaveChangesAsync(turnContext, false, cancellationToken);

			await turnContext.SendActivityAsync(reply, cancellationToken);
		}

		private Task<DuckDuckGoResponse<DuckImage>> GetImagesAsync(InlineQuery inlineQuery, DuckUserState user, CancellationToken cancellationToken = default)
		{
			if (inlineQuery.Query != user.Query)
			{
				return _duckDuckGoApi.Images(inlineQuery.Query, cancellationToken: cancellationToken);
			}

			var duckDuckGoResponse = new DuckDuckGoResponse<DuckImage>
			{
				Vqd = user.Vqd,
				Next = user.Next
			};

			return _duckDuckGoApi.Next(duckDuckGoResponse, cancellationToken);
		}

		private AnswerInlineQueryRequest CreateAnswerInlineQuery(InlineQuery inlineQuery, DuckDuckGoResponse<DuckImage> response)
		{
			var jpegImages = FilterOnlyJpeg(response.Results);

			var inlineQueryPhotos = jpegImages
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

		private IEnumerable<DuckImage> FilterOnlyJpeg(IEnumerable<DuckImage> source)
		{
			foreach (var image in source)
			{
				var extension = Path.GetExtension(image.Image);

				if (extension == ".jpg" || extension == ".jpeg")
				{
					yield return image;
				}
			}
		}
	}
}