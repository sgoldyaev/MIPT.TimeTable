using System.Linq;
using System.Security.Principal;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MIPT.Dal;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MIPT.BotApi.Handlers
{
    public class TablesHandler : CommandHandler
    {
        public TablesHandler(IServiceScopeFactory factory, ITelegramBotClient bot)
            : base(factory, bot, "/tables")
        {
        }

        protected override string Response(Message message)
        {
            var response = new StringBuilder();
            
            using (var scope = base.Factory.CreateScope())
            using (var context = scope.ServiceProvider.GetService<TimeTableDb>())
            {
                var query = context.TimeTable
                    .Include(table => table.GroupRef)
                    .Include(table => table.SubjectRef)
                    .Select(table => new
                    {
                        Group=table.GroupRef.Name,
                        Subject = table.SubjectRef.Title,
                        Start = table.StartAt,
                        Finish = table.FinishAt,
                    })
                    .ToArray();
                
                foreach (var t in query)
                {
                    response.AppendLine($"{t.Start:t}-{t.Finish:t}|{t.Group} {t.Subject} ");
                }
            }

            return response.ToString();
        }
    }
}