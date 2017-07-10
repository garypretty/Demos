using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using TranslationDemoBot.Extensions;
using TranslationDemoBot.Helpers;

namespace TranslationDemoBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;  
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            // calculate something for us to return
            int length = (activity.Text ?? string.Empty).Length;

            // get the current user language
            var languageCode = await StateHelper.GetUserLanguageCode(activity);

            // reply to the user and tell them which language we detected
            var message = $"You sent {activity.Text} which was detected as {languageCode}";
            await context.PostAsync(message);

            // reply to the user in their language telling them how long their original message was
            var translatedMessageLengthMsg =
                await TranslationHelper.GetTranslationAsync(
                    $"Your original message was '{length}' characters long!", "en", languageCode);
            await context.PostAsync(translatedMessageLengthMsg);

            context.Wait(MessageReceivedAsync);
        }
    }
}