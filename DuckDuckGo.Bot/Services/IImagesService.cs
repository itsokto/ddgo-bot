using System.Threading;
using System.Threading.Tasks;

namespace DuckDuckGo.Bot.Services
{
	public interface IImagesService
	{
		Task<DuckDuckGoResponse<DuckImage>> GetAsync(string query, DuckUserState state, CancellationToken cancellationToken = default);
	}
}