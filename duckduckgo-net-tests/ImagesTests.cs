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
			var result = await _duckGoApi.Images("car");
			var result1 = await result.Next();

			Assert.NotNull(result1);
			Assert.NotEmpty(result1.Results);
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