using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using MIPT.Dal;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MIPT.BotApi.Handlers
{
    public class GroupsHandler : CommandHandler
    {
        public GroupsHandler(IServiceScopeFactory factory, ITelegramBotClient bot)
            : base(factory, bot, "/groups")
        {
        }

        protected override string Response(Message message)
        {
            var response = new StringBuilder();
            response.AppendLine("Id, Name");
            
            using (var scope = base.Factory.CreateScope())
            using (var context = scope.ServiceProvider.GetService<TimeTableDb>())
            {
                var query = context.Group
                    .ToArray();
                
                foreach (var gr in query)
                {
                    response.AppendFormat("#{0} {1}", gr.Id, gr.Name);
                }
            }

            return response.ToString();
        }
    }
}