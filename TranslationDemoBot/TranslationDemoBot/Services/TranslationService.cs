using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace TranslationDemoBot.Services
{
    public class TranslationService
    {
        private readonly HttpClient _client = new HttpClient();

        public static async Task<String> GetToken()
        {
            var cognitiveServicesTokenIssuerEndpoint = new Uri("https://api.cognitive.microsoft.com/sts/v1.0/issueToken?Subscription-Key=56fdc6bb06bd47ec9aa556273b18e8f5");

            HttpContent requestContent = new StringContent("", Encoding.UTF8, "application/xml");
            requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/xml");

            var client = new HttpClient();
            var cognitiveServicesTokenResponse = await client.PostAsync(cognitiveServicesTokenIssuerEndpoint, requestContent);

            if (!cognitiveServicesTokenResponse.IsSuccessStatusCode)
            {
                var error = await cognitiveServicesTokenResponse.Content.ReadAsAsync<JToken>();
                var errorType = error.Value<string>("error");
                var errorDescription = error.Value<string>("error_description");
                throw new HttpRequestException($"Get token request failed: {errorType} {errorDescription}");
            }

            var token = await cognitiveServicesTokenResponse.Content.ReadAsStringAsync();

            return token;
        }

        public async Task<string> TranslateAsync(string inputText, string inputLocale, string outputLocale)
        {
            var token = await GetToken();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var requestUri = string.Format("http://api.microsofttranslator.com/v2/Http.svc/Translate?text={0}&to={1}",
                System.Net.WebUtility.UrlEncode(inputText),
                outputLocale);

            var rawTranslationResponse = await _client.GetStringAsync(requestUri);
            var parsedTranslationResponse = XDocument.Parse(rawTranslationResponse);
            var translatedText = parsedTranslationResponse.Root?.FirstNode.ToString();
            return translatedText == inputText ? "" : translatedText;
        }

        internal async Task<string> DetectAsync(string input)
        {
            var token = await GetToken();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var requestUri = string.Format("http://api.microsofttranslator.com/v2/Http.svc/Detect?text={0}", 
                System.Net.WebUtility.UrlEncode(input));

            var rawTranslationResponse = await _client.GetStringAsync(requestUri);
            var parsedTranslationResponse = XDocument.Parse(rawTranslationResponse);
            var detectedLocale = parsedTranslationResponse.Root?.FirstNode.ToString();
            return detectedLocale;
        }
    }

}