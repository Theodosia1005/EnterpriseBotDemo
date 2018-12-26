using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;

namespace EnterpriseBot.Dialogs
{
    public class BookTicketFlow : ComponentDialog
    {
        private const string promptDialogId = "prompt_dialog";

        public BookTicketFlow() : base(nameof(BookTicketFlow))
        {
            var bookticketFlow = new WaterfallStep[]
            {
                GetLocationPromptDepartureTime,
                GetDepartureTimePromptReturnTime,
                GetReturnTimeConfirmTicket,
                AskForBookHotel,
                ShowHotelInfo,
                ConfirmHotel,
                BookHotel,
            };

            AddDialog(new WaterfallDialog(nameof(BookTicketFlow), bookticketFlow));
            AddDialog(new TextPrompt(promptDialogId));

            InitialDialogId = nameof(BookTicketFlow);
        }

        public async Task<DialogTurnResult> GetLocationPromptDepartureTime(WaterfallStepContext sc, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await sc.PromptAsync(promptDialogId, new PromptOptions { Prompt = sc.Context.Activity.CreateReply("When do you leave?") }, cancellationToken);
        }

        public async Task<DialogTurnResult> GetDepartureTimePromptReturnTime(WaterfallStepContext sc, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await sc.PromptAsync(promptDialogId, new PromptOptions { Prompt = sc.Context.Activity.CreateReply("Do you need a return ticket?") }, cancellationToken);
        }

        public async Task<DialogTurnResult> GetReturnTimeConfirmTicket(WaterfallStepContext sc, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await sc.PromptAsync(promptDialogId, new PromptOptions { Prompt = sc.Context.Activity.CreateReply("I will book these tickets for you. Please confirm:") }, cancellationToken);
        }

        public async Task<DialogTurnResult> AskForBookHotel(WaterfallStepContext sc, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await sc.PromptAsync(promptDialogId, new PromptOptions { Prompt = sc.Context.Activity.CreateReply("Do you want to book the hotel?") }, cancellationToken);
        }

        public async Task<DialogTurnResult> ShowHotelInfo(WaterfallStepContext sc, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await sc.PromptAsync(promptDialogId, new PromptOptions { Prompt = sc.Context.Activity.CreateReply("These are some hotels in Seattle:") }, cancellationToken);
        }

        public async Task<DialogTurnResult> ConfirmHotel(WaterfallStepContext sc, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await sc.PromptAsync(promptDialogId, new PromptOptions { Prompt = sc.Context.Activity.CreateReply("Ok. I will book the Hotel 2 from 12/25 to 12/30. Is this fine?") }, cancellationToken);
        }

        public async Task<DialogTurnResult> BookHotel(WaterfallStepContext sc, CancellationToken cancellationToken = default(CancellationToken))
        {
            await sc.Context.SendActivityAsync(sc.Context.Activity.CreateReply("The Hotel has been booked. I’m here if you need any help."), cancellationToken);

            return await sc.EndDialogAsync(true);
        }
    }
}
