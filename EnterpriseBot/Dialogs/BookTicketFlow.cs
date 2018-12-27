using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Solutions.Extensions;
using System.Collections.Specialized;
using Microsoft.Bot.Solutions.Dialogs;
using Microsoft.Bot.Solutions.Cards;
using EnterpriseBot.AdaptiveCard;

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
            var botResponse = new BotResponse("I will book these tickets for you. Please confirm:", "I will book these tickets for you. Please confirm:");
            var replyToConversation = sc.Context.Activity.CreateReply(botResponse);
            //replyToConversation.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            replyToConversation.Attachments = new List<Attachment>();
            var replyMessage1 = sc.Context.Activity.CreateAdaptiveCardReply(
                botResponse,
                "AdaptiveCard/TicketCard.json",
                new CardData(),
                null,
                new StringDictionary() { { "Location", "Beijing - Seattle" }, { "DepartTime", "12/25 12:15" }, { "ArriveTime", "12/25 6:30" } });
            var replyMessage2 = sc.Context.Activity.CreateAdaptiveCardReply(
                botResponse,
                "AdaptiveCard/TicketCard.json",
                new CardData(),
                null,
                new StringDictionary() { { "Location", "Beijing - Seattle" }, { "DepartTime", "12/30 7:30" }, { "ArriveTime", "12/31 14:45" } });
            replyToConversation.Attachments.Add(replyMessage1.Attachments[0]);
            replyToConversation.Attachments.Add(replyMessage2.Attachments[0]);
            return await sc.PromptAsync(promptDialogId, new PromptOptions { Prompt = replyToConversation }, cancellationToken);
        }

        public async Task<DialogTurnResult> AskForBookHotel(WaterfallStepContext sc, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await sc.PromptAsync(promptDialogId, new PromptOptions { Prompt = sc.Context.Activity.CreateReply("Do you want to book the hotel?") }, cancellationToken);
        }

        public async Task<DialogTurnResult> ShowHotelInfo(WaterfallStepContext sc, CancellationToken cancellationToken = default(CancellationToken))
        {
            var botResponse = new BotResponse("These are some hotels in Seattle:", "These are some hotels in Seattle:");
            var replyToConversation = sc.Context.Activity.CreateReply(botResponse);
            replyToConversation.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            replyToConversation.Attachments = new List<Attachment>();
            var replyMessage1 = sc.Context.Activity.CreateAdaptiveCardReply(
                botResponse,
                "AdaptiveCard/HotelCard.json",
                new CardData(),
                null,
                new StringDictionary() { { "Name", "Hotel 1" }, { "Address", "Street 1" }, { "Price", "$180/day" } });
            var replyMessage2 = sc.Context.Activity.CreateAdaptiveCardReply(
                botResponse,
                "AdaptiveCard/HotelCard.json",
                new CardData(),
                null,
                new StringDictionary() { { "Name", "Hotel 2" }, { "Address", "Street 2" }, { "Price", "$180/day" } });
            var replyMessage3 = sc.Context.Activity.CreateAdaptiveCardReply(
                botResponse,
                "AdaptiveCard/HotelCard.json",
                new CardData(),
                null,
                new StringDictionary() { { "Name", "Hotel 3" }, { "Address", "Street 3" }, { "Price", "$180/day" } });
            replyToConversation.Attachments.Add(replyMessage1.Attachments[0]);
            replyToConversation.Attachments.Add(replyMessage2.Attachments[0]);
            replyToConversation.Attachments.Add(replyMessage3.Attachments[0]);
            return await sc.PromptAsync(promptDialogId, new PromptOptions { Prompt = replyToConversation }, cancellationToken);
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
