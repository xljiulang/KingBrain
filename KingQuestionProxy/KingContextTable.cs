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
    /// 提供http数据上下文存储与查询
    /// </summary>
    static class KingContextTable
    {
        private static readonly object syncRoot = new object();

        /// <summary>
        /// http数据上下文列表
        /// </summary>
        private static readonly List<KingContext> list = new List<KingContext>();

        /// <summary>
        /// 添加http数据上下文
        /// </summary>
        /// <param name="context">http数据上下文</param>
        public static void Add(KingContext context)
        {
            lock (syncRoot)
            {
                list.Add(context);
            }
        }

        /// <summary>
        /// 通过请求体内容获取获取http数据上下文
        /// </summary>
        /// <param name="kingRequest">请求体内容获</param>
        /// <returns></returns>
        public static KingContext GetByRequest(KingRequest kingRequest)
        {
            lock (syncRoot)
            {
                return list.FirstOrDefault(item => item.KingRequest.Equals(kingRequest));
            }
        }
    }
}
