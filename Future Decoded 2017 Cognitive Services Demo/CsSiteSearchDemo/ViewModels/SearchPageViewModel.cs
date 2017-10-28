using System.Collections.Generic;
using CsSiteSearchDemo.Models;

namespace CsSiteSearchDemo.ViewModels
{
    public class SearchPageViewModel
    {
        public string Query { get; set; }
        public CustomSearchResponse SearchResponse { get; set; }
        public string SessionsCtaTopic { get; set; }
        public string SessionsCtaDay { get; set; }
        public string SessionsCtaLink { get; set; }
        public List<Answer> qnaAnswers;
    }
}