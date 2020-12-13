using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MIPT.Common;
using MIPT.Dal;
using Telegram.Bot;

namespace MIPT.BotApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args)
                .UseConsoleLifetime()
                .Build();

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseEnvironment(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"))
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder
                        .SetBasePath(context.HostingEnvironment.ContentRootPath)
                        .AddJsonFile("appsettings.json")
                        .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true)
                        .AddEnvironmentVariables()
                        .Build();
                })
                .ConfigureServices((context, collection) =>
                {
                    var connection = context.Configuration.GetConnectionString("TimeTableDb");
                    collection.Configure<BotSettings>(context.Configuration.GetSection("BotSettings"));
                    collection.AddDbContext<TimeTableDb>((provider, builder) => builder.UseSqlite(connection));
                    collection.AddTransient<ITelegramBotClient>(provider =>
                        new TelegramBotClient(provider.GetService<IOptions<BotSettings>>().Value.Key));
                    collection.AddHostedService<BotService>();
                });
        }
    }
}