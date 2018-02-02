using CsQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KingQuestionProxy.Search
{
    class BingSearchEngine : SearchEngineBase
    {
        protected override string[] SearchSourceAnswers(string quiz)
        {
            using (var client = new WebClient())
            {
                var address = $"http://cn.bing.com/search?q={quiz}";
                var html = client.DownloadData(address);
                CQ cQ = Encoding.UTF8.GetString(html);
                return cQ.Find(".b_caption>p").Select(item => item.Cq().Text()).ToArray();
            }
        }
    }
}
