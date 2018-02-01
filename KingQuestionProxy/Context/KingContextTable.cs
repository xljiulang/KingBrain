using System;
using System.Collections.Generic;
using System.Threading;

namespace KingQuestionProxy
{
    /// <summary>
    /// 提供http数据上下文存储与查询
    /// </summary>
    static class KingContextTable
    {
        /// <summary>
        /// 同步锁
        /// </summary>
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
                var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1d));
                cts.Token.Register(() => Remove(context));
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="context"></param>
        private static void Remove(KingContext context)
        {
            lock (syncRoot)
            {
                list.Remove(context);
            }
        }

        /// <summary>
        /// 通过请求体内容获取获取http数据上下文
        /// </summary>
        /// <param name="kingRequest">请求体内容获</param>
        /// <returns></returns>
        public static KingContext TakeByRequest(KingRequest kingRequest)
        {
            lock (syncRoot)
            {
                var index = list.FindIndex(item => item.KingRequest.Equals(kingRequest));
                if (index < 0)
                {
                    return null;
                }

                var context = list[index];
                list.RemoveAt(index);
                return context;
            }
        }
    }
}
