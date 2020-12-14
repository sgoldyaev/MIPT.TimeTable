using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using MIPT.Dal;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MIPT.BotApi.Handlers
{
    public class SubjectsHandler : CommandHandler
    {
        public SubjectsHandler(IServiceScopeFactory factory, ITelegramBotClient bot)
            : base(factory, bot, "/subjects")
        {
        }

        protected override string Response(Message message)
        {
            var response = new StringBuilder();
            response.AppendLine("Id, Title");
            
            using (var scope = base.Factory.CreateScope())
            using (var context = scope.ServiceProvider.GetService<TimeTableDb>())
            {
                var query = context.Subject
                    .ToArray();
                
                foreach (var subj in query)
                {
                    response.AppendFormat("#{0} {1}", subj.Id, subj.Title);
                }
            }

            return response.ToString();
        }
    }
}