﻿using CsQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KingQuestionProxy.Search
{
    /// <summary>
    /// 百度搜索工具
    /// </summary>
    public class BaiduSearchEngine : SearchEngine
    {
        /// <summary>
        /// 找class="c-abstract"的标签的文本
        /// 就是原始参数答案
        /// </summary>
        /// <param name="question">问题</param>
        /// <returns></returns>
        protected override string[] SearchSourceAnswers(string question)
        {
            using (var client = new WebClient())
            {
                var address = $"http://www.baidu.com/s?ie=utf-8&wd={question}";
                var html = client.DownloadData(address);
                CQ cQ = Encoding.UTF8.GetString(html);
                return cQ.Find(".c-abstract").Select(item => item.Cq().Text()).ToArray();
            }
        }
    }
}
