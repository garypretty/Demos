using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using TranslationDemoBot.Helpers;

namespace TranslationDemoBot.Extensions
{
    public static class ContextEntensions
    {
        public static async Task PostUsingUserLocaleAsync(this IDialogContext context, string text)
        {
            var userLanguageCode = await StateHelper.GetUserLanguageCode(context);
            if (userLanguageCode != "en")
            {
                text = await TranslationHelper.GetTranslationAsync(text, "en", userLanguageCode);
            }

            await context.PostAsync(text);
        }

        public static async Task<string> ToUserLocaleAsync(this string text, Activity activity)
        {
            var userLanguageCode = await StateHelper.GetUserLanguageCode(activity);
            if (userLanguageCode != "en")
            {
                text = await TranslationHelper.GetTranslationAsync(text, "en", userLanguageCode);
            }

            return text;
        }
    }

}