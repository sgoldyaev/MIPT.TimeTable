using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MIPT.BotApi;
using MIPT.Dal;
using Moq;
using NUnit.Framework;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using BindingFlags = System.Reflection.BindingFlags;

namespace MIPT.UnitTests
{
    public class TelegramBotTests
    {
        private readonly Mock<ITelegramBotClient> bot = new Mock<ITelegramBotClient>();
        private IHost host = null!;
        private BotService service = null!;
        
        [SetUp]
        public void Setup()
        {
            this.host = new HostBuilder()
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder.AddInMemoryCollection(new Dictionary<string, string>()
                    {
                        {"Logging:LogLevel:Default","Information"},
                        {"Logging:LogLevel:Microsoft","Warning"},
                        {"Logging:LogLevel:Microsoft.Hosting.Lifetime","Information"},
                    })
                    .Build();
                })
                .ConfigureServices((context, collection) =>
                {
                    collection.AddDbContext<TimeTableDb>((provider, builder) => builder.UseInMemoryDatabase("inMemory"));
                    collection.AddTransient<ITelegramBotClient>(provider => bot.Object);
                    collection.AddTransient<BotService>();
                })
                .Build();

            this.service = this.host.Services.GetService<BotService>();
        }

        [TearDown]
        public void TearDown()
        {
            this.bot.Reset();
        }

        [Test(Description = "Verify if bot.StartReceiving is called when service start")]
        public async Task StartReceivingOnStartAsync()
        {
            await this.service.StartAsync(CancellationToken.None);

            this.bot.Verify(bot =>
                bot.StartReceiving(
                    It.IsAny<UpdateType[]>(), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);
        }

        [Test(Description = "Verify if bot.StopReceiving is called when service stop")]
        public async Task StopReceivingOnStopAsync()
        {
            await this.service.StopAsync(CancellationToken.None);

            this.bot.Verify(bot =>
                    bot.StopReceiving(), 
                Times.Once);
        }

        [Test(Description = "Verify response on /start command")]
        public async Task ResponseToUserOnStartCommand()
        {
            var args = CreateMessage(new Message {Text = "/start", Chat = new Chat{Id = 99}});

            this.bot.Raise(bot => bot.OnMessage += null, args);

            this.bot.Verify(bot =>
                    bot.SendTextMessageAsync(
                        It.Is<ChatId>(chatId => chatId.Identifier == new ChatId(99).Identifier), 
                        It.Is<string>(response => response == "Available commands:\n" +
                            "/start - start bot\n" +
                            "/groups - list of all groups\n" +
                            "/subjects - list of all subjects\n" + 
                            "/tables - list of all schedules."), 
                        It.IsAny<ParseMode>(),
                        It.IsAny<bool>(),
                        It.IsAny<bool>(),
                        It.IsAny<int>(),
                        It.IsAny<IReplyMarkup>(),
                        It.IsAny<CancellationToken>())
                ,Times.Once);
        }

        /// <summary>
        /// helper method to instantiate MessageEventArgs via reflection, since it has not public constructor
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private static MessageEventArgs CreateMessage(Message message)
        {
            return (MessageEventArgs)Activator.CreateInstance(
                type: typeof(MessageEventArgs), 
                bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic,
                binder: null,
                args: new []{message},
                culture: null,
                activationAttributes: null);
        }
    }
}