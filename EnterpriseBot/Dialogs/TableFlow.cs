using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using EnterpriseBot.Service;
using Microsoft.Bot.Schema;
using System.IO;
using System.Reflection;

namespace EnterpriseBot.Dialogs
{
    public class TableFlow : ComponentDialog
    {
        private const string tableDialogId = "table_dialog";

        public TableFlow() : base(nameof(TableFlow))
        {
            var tableFlow = new WaterfallStep[]
            {
                GetQuestion,
            };

            AddDialog(new WaterfallDialog(tableDialogId, tableFlow));

            InitialDialogId = tableDialogId;
        }

        public async Task<DialogTurnResult> GetQuestion(WaterfallStepContext sc, CancellationToken cancellationToken = default(CancellationToken))
        {
            List<string> messages = TableService.GetReplyMessage(sc.Context.Activity.Text);
            List<IMessageActivity> resourceResponses = new List<IMessageActivity>();
            foreach (string message in messages)
            {
                if (message.StartsWith("#PIC#"))
                {
                    IMessageActivity picture = Activity.CreateMessageActivity();
                    picture.Text = "Or base on the pridiction, you can go after half an hour.";
                    picture.TextFormat = "plain";
                    picture.Locale = "en-Us";

                    var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    var resDir = Path.Combine(dir, @"Service\Resources\");
                    picture.Attachments = new List<Attachment>();
                    picture.Attachments.Add(new Attachment()
                    {
                        ContentUrl = resDir + "pic.png",
                        ContentType = "image/png",
                        Name = "predict"
                    });
                    resourceResponses.Add(picture);
                }
                else
                {


                    IMessageActivity messageActivity = Activity.CreateMessageActivity();
                    messageActivity.Text = message;
                    messageActivity.TextFormat = "plain";
                    messageActivity.Locale = "en-Us";
                    resourceResponses.Add(messageActivity);
                }
            }
            await sc.Context.SendActivitiesAsync(resourceResponses.ToArray());

            return await sc.EndDialogAsync(true);
        }
    }
}
