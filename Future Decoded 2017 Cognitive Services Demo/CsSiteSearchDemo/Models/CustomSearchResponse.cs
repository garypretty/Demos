using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CsSiteSearchDemo.Models
{

    public class CustomSearchResponse
    {
        public string _type { get; set; }
        public Instrumentation instrumentation { get; set; }
        public Webpages webPages { get; set; }
    }

    public class Instrumentation
    {
        public string pingUrlBase { get; set; }
        public string pageLoadPingUrl { get; set; }
    }

    public class Webpages
    {
        public string webSearchUrl { get; set; }
        public string webSearchUrlPingSuffix { get; set; }
        public int totalEstimatedMatches { get; set; }
        public Value[] value { get; set; }
    }

    public class Value
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string urlPingSuffix { get; set; }
        public string displayUrl { get; set; }
        public string snippet { get; set; }
        public DateTime dateLastCrawled { get; set; }
    }

}