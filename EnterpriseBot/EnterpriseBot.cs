// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EnterpriseBot.Service;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

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
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>                        
        public EnterpriseBot()
        {
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
            // Handle Message activity type, which is the main activity type for shown within a conversational interface
            // Message activities may contain text, speech, interactive cards, and binary or unknown attachments.
            // see https://aka.ms/about-bot-activity-message to learn more about the message and other activity types
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                ActivityType type = DispatchActivity(turnContext.Activity.Text);
                switch (type)
                {
                    case (ActivityType.Table):
                        {
                            // Echo back to the user whatever they typed.
                            List<string> messages = TableService.GetReplyMessage(turnContext.Activity.Text);
                            List<IMessageActivity> resourceResponses = new List<IMessageActivity>();
                            foreach (string message in messages)
                            {
                                IMessageActivity messageActivity = Activity.CreateMessageActivity();
                                messageActivity.Text = message;
                                messageActivity.TextFormat = "plain";
                                messageActivity.Locale = "en-Us";
                                resourceResponses.Add(messageActivity);
                            }
                            await turnContext.SendActivitiesAsync(resourceResponses.ToArray());
                            break;
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
