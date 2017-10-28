using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using CsSiteSearchDemo.Models;
using CsSiteSearchDemo.ViewModels;
using Newtonsoft.Json;

namespace CsSiteSearchDemo.Controllers
{
    public class SearchController : Controller
    {
        public ActionResult Index()
        {
            var viewModel = new SearchPageViewModel();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Index(SearchPageViewModel viewModel)
        {
            var luisResponse = await GetLuisResponse(viewModel.Query);

            if (luisResponse?.topScoringIntent != null && luisResponse.topScoringIntent.intent == "FindSessionsByTopic"
                && luisResponse.topScoringIntent.score > 0.8)
            {
                var dayEntity = luisResponse.entities.FirstOrDefault(e => e.type == "Day");
                var topicEntity = luisResponse.entities.FirstOrDefault(e => e.type == "Topic");

                if (dayEntity != null || topicEntity != null)
                {
                    viewModel.SessionsCtaTopic = topicEntity?.entity;
                    viewModel.SessionsCtaDay = dayEntity?.resolution.values.First();
                    viewModel.SessionsCtaLink = $"https://www.futuredecoded.com/sessions?page=1";

                    if (topicEntity != null)
                    {
                        viewModel.SessionsCtaLink += $"&searchtext={topicEntity.entity}";
                    }
                    if (dayEntity != null)
                    {
                        viewModel.SessionsCtaLink += dayEntity.resolution.values.First() == "Day 1" ? "&timeslotday=2017-10-31" : "&timeslotday=2017-11-01";
                    }
                }
            }

            var qnaResponse = GetQnAResponse(viewModel.Query);

            if (qnaResponse != null && qnaResponse.answers.Any(a => a.score > 60))
            {
                viewModel.qnaAnswers = qnaResponse.answers.Where(a => a.score > 60).ToList();
            }

            viewModel.SearchResponse = await GetCustomSearchResults(viewModel.Query);

            return View(viewModel);
        }

        public async Task<LuisResponse> GetLuisResponse(string query)
        {
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri("https://westus.api.cognitive.microsoft.com/luis/v2.0/");

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to Luis REST service using HttpClient  
                HttpResponseMessage httpResponse = await client.GetAsync($"apps/f2a6213f-14ec-4eb6-bd5d-1027fc5e4358?subscription-key=bfa14f500a6c4f1b95b8e25a8d6dd95a&verbose=true&timezoneOffset=0&q={query}");

                //Checking the response is successful or not which is sent using HttpClient  
                if (httpResponse.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var responseStr = httpResponse.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    return JsonConvert.DeserializeObject<LuisResponse>(responseStr);
                }

                return null;
            }
        }

        public QnAResponse GetQnAResponse(string query)
        {
            string responseString;
            var builder = new UriBuilder($"https://westus.api.cognitive.microsoft.com/qnamaker/v3.0/knowledgebases/8e57ee80-2b48-4cd0-bcc0-6d8cafc020fc/generateAnswer");
            var postBody = $"{{\"question\": \"{query}\", \"top\": \"3\"}}";

            using (var client = new WebClient())
            {
                client.Encoding = System.Text.Encoding.UTF8;
                client.Headers.Add("Ocp-Apim-Subscription-Key", "8f833e867b25443b95d8c23cd367f7ce");
                client.Headers.Add("Content-Type", "application/json");
                responseString = client.UploadString(builder.Uri, postBody);
            }

            try
            {
                var response = JsonConvert.DeserializeObject<QnAResponse>(responseString);
                return response;
            }
            catch
            {
                throw new Exception("Unable to deserialize QnA Maker response string.");
            }
        }

        public async Task<CustomSearchResponse> GetCustomSearchResults(string query)
        {
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri("https://cognitivegblppe.azure-api.net/bingcustomsearch/v5.0/");

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Define authentication header
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "65e65c84fd6049718207d535c2551536");

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage httpResponse = await client.GetAsync($"search?q={query}&customconfig=1066360521&responseFilter=Webpages&mkt=en-US&safesearch=Moderate");

                //Checking the response is successful or not which is sent using HttpClient  
                if (httpResponse.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var responseStr = httpResponse.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    return JsonConvert.DeserializeObject<CustomSearchResponse>(responseStr);
                }

                return null;
            }
        }
    }
}