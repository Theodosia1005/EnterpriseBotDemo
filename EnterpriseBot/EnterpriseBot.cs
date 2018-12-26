// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder.Dialogs;
using EnterpriseBot.Dialogs;

namespace EnterpriseBot
{
    /// <summary>
    /// Represents a bot that processes incoming activities.
    /// For each user interaction, an instance of this class is created and the OnTurnAsync method is called.
    /// This is a Transient lifetime service. Transient lifetime services are created
    /// each time they're requested. Objects that are expensive to construct, or have a lifetime
    /// beyond a single turn, should be carefully managed.
    /// For example, the <see cref="MemoryStorage"/> object and associated
    /// <see cref="IStatePropertyAccessor{T}"/> object are created with a singleton lifetime.
    /// </summary>
    /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1"/>
    public class EnterpriseBot : IBot
    {
        private DialogSet dialogs;
        private ConversationState conversationState; 

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>                        
        public EnterpriseBot(ConversationState conversationState )
        {
            dialogs = new DialogSet(conversationState.CreateProperty<DialogState>(nameof(DialogState)));
            dialogs.Add(new TableFlow());
            dialogs.Add(new QnAFlow());
            dialogs.Add(new BookTicketFlow());
        }

        /// <summary>
        /// Every conversation turn calls this method.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        /// <seealso cref="BotStateSet"/>
        /// <seealso cref="ConversationState"/>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                var dc = await dialogs.CreateContextAsync(turnContext);
                var results = await dc.ContinueDialogAsync(cancellationToken);
                if (results.Status == DialogTurnStatus.Empty)
                {
                    ActivityType type = DispatchActivity(turnContext.Activity.Text);
                    switch (type)
                    {
                        case (ActivityType.Table):
                            {
                                await dc.BeginDialogAsync(nameof(TableFlow));
                                break;
                            }
                        case ActivityType.Other:
                            {
                                await dc.BeginDialogAsync(nameof(BookTicketFlow));
                                break;
                            }
                    }
                }
                
            }
        }

        private enum ActivityType
        {
            Flow = 0,
            Table = 1,
            QnA = 2,
            Other = 3
        }

        private ActivityType DispatchActivity(string request)
        {
            if (request.Contains("parking"))
            {
                return ActivityType.Table;
            }
            return ActivityType.Other;
        }
    }
}
