using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using System.IO;
using NetworkSocket.Http;

namespace KingQuestionProxy
{
    /// <summary>
    /// 提供历史上下文数据查询和保存
    /// </summary>
    static class KingContextTable
    {
        private static readonly object syncRoot = new object();

        private static readonly List<KingContext> list = new List<KingContext>();

        /// <summary>
        /// 添加历史数据
        /// </summary>
        /// <param name="data"></param>
        public static void Add(KingContext data)
        {
            lock (syncRoot)
            {
                list.Add(data);
            }
        }

        /// <summary>
        /// 通过请求体内容获取获取历史数据
        /// </summary>
        /// <param name="request">请求体内容获</param>
        /// <returns></returns>
        public static KingContext GetByRequest(KingRequest request)
        {
            lock (syncRoot)
            {
                return list.FirstOrDefault(item => item.KingRequest.Equals(request));
            }
        }
    }
}
