using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;

namespace EnterpriseBot
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
            await sc.Context.SendActivityAsync(sc.Context.Activity.CreateReply("200 dollars a day"));

            return await sc.EndDialogAsync(true);
        }
    }
}
