using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DuckDuckGo.Bot.Services
{
	public class ImagesService : IImagesService
	{
		private readonly IDuckApi _duckDuckGoApi;

		public ImagesService(IDuckApi duckDuckGoApi)
		{
			_duckDuckGoApi = duckDuckGoApi;
		}

		public async Task<DuckResponse<DuckImage>> GetAsync(string query, DuckUserState state,
			CancellationToken cancellationToken = default)
		{
			DuckResponse<DuckImage> duckResponse;

			if (query != state.Query)
			{
				duckResponse = await _duckDuckGoApi.GetImagesAsync(query, cancellationToken: cancellationToken);
			}
			else
			{
				duckResponse = new DuckResponse<DuckImage>
				{
					Vqd = state.Vqd,
					Next = state.Next
				};

				duckResponse = await _duckDuckGoApi.NextAsync(duckResponse, cancellationToken);
			}

			// Photo must be in jpeg format.
			// https://core.telegram.org/bots/api#inlinequeryresultphoto
			duckResponse.Results = FilterImages(duckResponse.Results, ".jpg", ".jpeg").ToList();

			return duckResponse;
		}

		private IEnumerable<DuckImage> FilterImages(IEnumerable<DuckImage> source, params string[] extensions)
		{
			return source.Where(img => extensions.Any(ext => ext == Path.GetExtension(img.Image)));
		}
	}
}