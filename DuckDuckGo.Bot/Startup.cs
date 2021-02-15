using DuckDuckGo.Bot.Bot;
using DuckDuckGo.Bot.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DuckDuckGo.Bot
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<DuckDuckGoApi>();

			services.AddSingleton<IStorage>(
				new CosmosDbPartitionedStorage(
					new CosmosDbPartitionedStorageOptions
					{
						CosmosDbEndpoint = Configuration.GetValue<string>("CosmosDbEndpoint"),
						AuthKey = Configuration.GetValue<string>("CosmosDbAuthKey"),
						DatabaseId = Configuration.GetValue<string>("CosmosDbDatabaseId"),
						ContainerId = Configuration.GetValue<string>("CosmosDbContainerId")
					}));

			services.AddSingleton<UserState>();

			services.AddSingleton<IImagesService, ImagesService>();

			services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorLogging>();

			services.AddBot<DuckBot>(options =>
			{
				options.CredentialProvider = new ConfigurationCredentialProvider(Configuration);
			});
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseBotFramework();
		}
	}
}