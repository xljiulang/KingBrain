using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Attributes;
using WebApiClient.Contexts;

namespace KingQuestion
{
    /// <summary>
    /// KingAnswerServer的接口
    /// </summary>
    [HttpHost("http://localhost")]
    public interface IAnswerApi : IDisposable
    {
        /// <summary>
        /// 发送问题到KingAnswerServer
        /// </summary>
        /// <param name="requestUrl">请求的URL</param>
        /// <param name="question">问题内容</param>
        /// <returns></returns>
        [HttpPost("/SendQuestionAsync")]
        ITask<HttpResponseMessage> SendQuestionAsync(string requestUrl, ByteArrayContent question);
    }
}
