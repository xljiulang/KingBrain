using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingQuestionProxy
{
    /// <summary>
    /// 表示数据上下文数据
    /// </summary>
    public class KingContext
    {
        /// <summary>
        /// 请求内容信息
        /// </summary>
        public KingRequest KingRequest { get; set; }

        /// <summary>
        /// 问题信息
        /// </summary>
        public KingQuestionData QuestionData { get; set; }

        /// <summary>
        /// 获取答案内容
        /// </summary>
        /// <param name="kingAnswer"></param>
        /// <returns></returns>
        public string GetAnswer(KingAnswer kingAnswer)
        {
            var index = kingAnswer.data.answer - 1;
            return this.QuestionData.options[index];
        }
    }
}
