using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MIPT.BotApi.Handlers;
using MIPT.Common;
using MIPT.Dal;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace MIPT.BotApi
{
    public class BotService : IHostedService
    {
        private readonly IServiceScopeFactory factory;
        private readonly ITelegramBotClient bot;
        private readonly CommandHandler[] handlers;
        
        public BotService(ITelegramBotClient bot, IServiceScopeFactory factory)
        {
            this.bot = bot;
            this.factory = factory;

            this.handlers = new CommandHandler[]
            {
                new StartHandler(factory, this.bot),
                new TablesHandler(factory, this.bot),
            };
            
            bot.OnMessage += BotOnOnMessage;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.bot.StartReceiving(allowedUpdates: null, cancellationToken);
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.bot.StopReceiving();

            return Task.CompletedTask;
        }

        private void BotOnOnMessage(object sender, MessageEventArgs e)
        {
            foreach (var handler in this.handlers)
            {
                handler.Handle(e.Message);
            }
        }
    }
}