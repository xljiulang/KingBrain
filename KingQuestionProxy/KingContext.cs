using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingQuestionProxy
{
    /// <summary>
    /// 表示http数据上下文
    /// </summary>
    public class KingContext
    {
        /// <summary>
        /// 请求内容信息
        /// </summary>
        public KingRequest KingRequest { get; set; }

        /// <summary>
        /// 响应的问题信息
        /// </summary>
        public KingQuestion KingQuestion { get; set; }

        /// <summary>
        /// 获取答案内容
        /// </summary>
        /// <param name="kingAnswer"></param>
        /// <returns></returns>
        public string GetAnswer(KingAnswer kingAnswer)
        {
            var index = kingAnswer.data.answer - 1;
            return this.KingQuestion.data.options[index];
        }
    }
}
