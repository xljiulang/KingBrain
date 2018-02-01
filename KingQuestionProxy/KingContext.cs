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
    }
}
