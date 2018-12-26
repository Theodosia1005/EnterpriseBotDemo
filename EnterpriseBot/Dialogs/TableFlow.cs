using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using EnterpriseBot.Service;
using Microsoft.Bot.Schema;

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
                IMessageActivity messageActivity = Activity.CreateMessageActivity();
                messageActivity.Text = message;
                messageActivity.TextFormat = "plain";
                messageActivity.Locale = "en-Us";
                resourceResponses.Add(messageActivity);
            }
            await sc.Context.SendActivitiesAsync(resourceResponses.ToArray());

            return await sc.EndDialogAsync(true);
        }
    }
}
