using CsQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KingQuestionProxy.Search
{
    class ZhidaoSearchEngine : SearchEngineBase
    {
        protected override string[] SearchSourceAnswers(string quiz)
        {
            using (var client = new WebClient())
            {
                var address = $"https://zhidao.baidu.com/search?ct=17&pn=0&tn=ikaslist&rn=10&fr=wwwt&word={quiz}";
                var html = client.DownloadData(address);
                CQ cQ = Encoding.UTF8.GetString(html);
                return cQ.Find(".answer").Select(item => item.Cq().Text()).ToArray();
            }
        }
    }
}
