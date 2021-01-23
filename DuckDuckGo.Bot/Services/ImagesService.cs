using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DuckDuckGo.Bot.Services
{
	public class ImagesService : IImagesService
	{
		private readonly DuckDuckGoApi _duckDuckGoApi;

		public ImagesService(DuckDuckGoApi duckDuckGoApi)
		{
			_duckDuckGoApi = duckDuckGoApi;
		}

		public async Task<DuckDuckGoResponse<DuckImage>> GetAsync(string query, DuckUserState state,
			CancellationToken cancellationToken = default)
		{
			DuckDuckGoResponse<DuckImage> duckDuckGoResponse;

			if (query != state.Query)
			{
				duckDuckGoResponse = await _duckDuckGoApi.Images(query, cancellationToken: cancellationToken);
			}
			else
			{
				duckDuckGoResponse = new DuckDuckGoResponse<DuckImage>
				{
					Vqd = state.Vqd,
					Next = state.Next
				};

				duckDuckGoResponse = await _duckDuckGoApi.Next(duckDuckGoResponse, cancellationToken);
			}

			// Photo must be in jpeg format.
			// https://core.telegram.org/bots/api#inlinequeryresultphoto
			duckDuckGoResponse.Results = FilterImages(duckDuckGoResponse.Results, ".jpg", ".jpeg").ToList();

			return duckDuckGoResponse;
		}

		private IEnumerable<DuckImage> FilterImages(IEnumerable<DuckImage> source, params string[] extensions)
		{
			return source.Where(img => extensions.Any(ext => ext == Path.GetExtension(img.Image)));
		}
	}
}