using System.Threading.Tasks;
using DuckDuckGo;
using Xunit;

namespace Tests
{
	public class ImagesTests
	{
		private readonly DuckDuckGoApi _duckGoApi;

		public ImagesTests()
		{
			_duckGoApi = new DuckDuckGoApi();
		}

		[Fact]
		public async Task GetTokenTest()
		{
			var token = await _duckGoApi.GetToken("car");

			Assert.NotNull(token);
		}

		[Fact]
		public async Task SearchImagesNextTest()
		{
			var response = await _duckGoApi.Images("car");
			var nextResponse = await _duckGoApi.Next(response);

			Assert.NotNull(nextResponse);
			Assert.NotEmpty(nextResponse.Results);
		}

		[Fact]
		public async Task SearchImagesTest()
		{
			var result = await _duckGoApi.Images("car");

			Assert.NotNull(result);
			Assert.NotEmpty(result.Results);
		}
	}
}