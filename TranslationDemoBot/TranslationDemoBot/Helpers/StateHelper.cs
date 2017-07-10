using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace TranslationDemoBot.Helpers
{
    public static class StateHelper
    {

        public static async void SetUserLanguageCode(Activity activity, string languageCode)
        {
            try
            {
                StateClient stateClient = activity.GetStateClient();
                BotData conversationData = await stateClient.BotState.GetConversationDataAsync(activity.ChannelId, activity.From.Id);
                conversationData.SetProperty<string>("LanguageCode", languageCode);
                await stateClient.BotState.SetConversationDataAsync(activity.ChannelId, activity.From.Id, conversationData);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<string> GetUserLanguageCode(IActivity activity)
        {
            try
            {
                StateClient stateClient = activity.GetStateClient();
                BotData conversationData = await stateClient.BotState.GetConversationDataAsync(activity.ChannelId, activity.From.Id);

                if (conversationData.Data == null)
                    return null;

                var languageCode = conversationData.GetProperty<string>("LanguageCode");
                return languageCode;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<string> GetUserLanguageCode(IDialogContext context)
        {
            try
            {
                StateClient stateClient = context.Activity.GetStateClient();
                BotData conversationData =
                    await stateClient.BotState.GetConversationDataAsync(context.Activity.ChannelId,
                        context.Activity.From.Id);

                if (conversationData.Data == null)
                    return null;

                var languageCode = conversationData.GetProperty<string>("LanguageCode");
                return languageCode;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}