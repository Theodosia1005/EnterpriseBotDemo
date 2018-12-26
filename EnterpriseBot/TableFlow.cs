using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;

namespace EnterpriseBot
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
            if (sc.Context.Activity.Text.Contains("parking"))
            {
                await sc.Context.SendActivityAsync(sc.Context.Activity.CreateReply("There are 32 parking places on the basement 1 and 156 parking places on the basement 2."));
            }
            else
            {
                await sc.Context.SendActivityAsync(sc.Context.Activity.CreateReply("Cafeteria has 37% empty seats now."));
            }

            return await sc.EndDialogAsync(true);
        }
    }
}
