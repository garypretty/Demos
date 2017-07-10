using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using TranslationDemoBot.Services;

namespace TranslationDemoBot.Helpers
{
    public static class TranslationHelper
    {
        public static async Task<string> DetectAndTranslate(Activity activity)
        {
            var currentUserLanguageCode = await StateHelper.GetUserLanguageCode(activity);

            if (activity.Text.Split().Length > 1 || currentUserLanguageCode == null)
            {
                var inputLanguageCode = await DetectLanguageAsync(activity.Text);

                if (currentUserLanguageCode != inputLanguageCode)
                {
                    StateHelper.SetUserLanguageCode(activity, inputLanguageCode);
                }

                if (inputLanguageCode != null && inputLanguageCode.ToLower() != "en")
                {
                    return await GetTranslationAsync(activity.Text, inputLanguageCode, "en");
                }
            }

            return activity.Text;
        }

        public static async Task<string> GetTranslationAsync(string inputText, string inputLocale, string outputLocale)
        {
            var translator = new TranslationService();
            var translation = await translator.TranslateAsync(inputText, inputLocale, outputLocale);
            return translation;
        }

        private static async Task<string> DetectLanguageAsync(string input)
        {
            var translator = new TranslationService();
            var language = await translator.DetectAsync(input);
            return language;
        }
    }
}