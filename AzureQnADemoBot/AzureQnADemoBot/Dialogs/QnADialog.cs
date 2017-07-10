using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using QnAMakerDialog;

namespace AzureQnADemoBot.Dialogs
{
    [Serializable]
    [QnAMakerService("YOUR_SUBSCRIPTION_KEY", "YOUR_KNOWLEDGEBASE_ID")]
    public class QnaDialog : QnAMakerDialog<object>
    {
        public override async Task NoMatchHandler(IDialogContext context, string originalQueryText)
        {
            await context.PostAsync($"Sorry, I couldn't find an answer for '{originalQueryText}'.");
            context.Wait(MessageReceived);
        }
    }
}