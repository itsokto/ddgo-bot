using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DuckDuckGo;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Telegram;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineQueryResults;

namespace ddgo.Bot
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

			DuckDuckGoResponse<DuckImage> duckDuckGoResponse;
			if (inlineQuery.Query == user.Query)
			{
				duckDuckGoResponse = new DuckDuckGoResponse<DuckImage>
				{
					Vqd = user.Vqd,
					Next = user.Next
				};
				duckDuckGoResponse = await _duckDuckGoApi.Next(duckDuckGoResponse, cancellationToken);
			}
			else
			{
				duckDuckGoResponse = await _duckDuckGoApi.Images(inlineQuery.Query, cancellationToken: cancellationToken);
			}

			var images = FilterOnlyJpeg(duckDuckGoResponse.Results);

			var inlineQueryPhotos = images
									.Take(50)
									.Select((image, i) => new InlineQueryResultPhoto(i.ToString(), image.Image, image.Thumbnail))
									.ToList();

			if (string.IsNullOrWhiteSpace(inlineQuery.Offset))
			{
				inlineQuery.Offset = "0";
			}

			var offset = !string.IsNullOrWhiteSpace(duckDuckGoResponse.Next)
				? (int.Parse(inlineQuery.Offset) + inlineQueryPhotos.Count).ToString()
				: string.Empty;

			var answer = new AnswerInlineQueryRequest(inlineQuery.Id, inlineQueryPhotos) { NextOffset = offset }.ToChannelData();

			var reply = turnContext.Activity.CreateReply();
			reply.ChannelData = answer;

			user.Vqd = duckDuckGoResponse.Vqd;
			user.Next = duckDuckGoResponse.Next;
			user.Query = inlineQuery.Query;

			await _userState.SaveChangesAsync(turnContext, false, cancellationToken);

			await turnContext.SendActivityAsync(reply, cancellationToken);
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