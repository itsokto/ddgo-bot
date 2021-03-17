using System.Threading;
using System.Threading.Tasks;

namespace DuckDuckGo.Bot.Services
{
	public interface IImagesService
	{
		Task<DuckResponse<DuckImage>> GetAsync(string query, DuckUserState state, CancellationToken cancellationToken = default);
	}
}