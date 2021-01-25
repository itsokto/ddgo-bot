using System;
using System.Threading.Tasks;
using Flurl.Http.Testing;
using Xunit;

namespace DuckDuckGo.Tests
{
	public class ApiTests : BaseTest
	{
		private readonly DuckDuckGoApi _duckGoApi;

		public ApiTests()
		{
			_duckGoApi = new DuckDuckGoApi();
		}

		[Fact]
		public async Task GetTokenTest()
		{
			var html = ReadFile("get_token_car.html");

			using var httpTest = new HttpTest();
			httpTest.RespondWith(html);

			var token = await _duckGoApi.GetToken("car");

			Assert.Equal("3-46082209034006461627445001051878587260-103201150309019047502164315555969820387", token);
		}

		[Fact]
		public async Task GetTokenThrowsInvalidOperationExceptionTest()
		{
			var html = ReadFile("get_token_car_without_vqd.html");

			using var httpTest = new HttpTest();
			httpTest.RespondWith(html);

			await Assert.ThrowsAsync<InvalidOperationException>(() => _duckGoApi.GetToken("car"));
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