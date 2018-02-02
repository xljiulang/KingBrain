using CsQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KingQuestionProxy.Search
{
    class LiZhiSearchEngine : SearchEngine
    {
        protected override string[] SearchSourceAnswers(string question)
        {
            using (var client = new WebClient())
            {
                var address = $"https://www.sogou.com/web?query={question}";
                var html = client.DownloadData(address);
                CQ cQ = Encoding.UTF8.GetString(html);
                return cQ.Find("#lizhi_long_wrapper .txt-box").Select(item => item.Cq().Text()).ToArray();
            }
        }
    }
}
