using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using EnterpriseBot.Service;

namespace EnterpriseBot.Dialogs
{
    public class QnAFlow : ComponentDialog
    {
        private const string qnaDialogId = "table_dialog";

        public QnAFlow() : base(nameof(QnAFlow))
        {
            var qnaFlow = new WaterfallStep[]
            {
                GetQuestion,
            };

            AddDialog(new WaterfallDialog(qnaDialogId, qnaFlow));

            InitialDialogId = qnaDialogId;
        }

        public async Task<DialogTurnResult> GetQuestion(WaterfallStepContext sc, CancellationToken cancellationToken = default(CancellationToken))
        {
            var client = new BertClient();
            var answer = await client.PassageQueryAsync(sc.Context.Activity.Text);
            await sc.Context.SendActivityAsync(sc.Context.Activity.CreateReply(answer));
            return await sc.EndDialogAsync(true);
        }
    }
}
