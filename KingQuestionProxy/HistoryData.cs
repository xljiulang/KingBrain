using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingQuestionProxy
{
    /// <summary>
    /// 表示历史数据
    /// </summary>
    public class HistoryData
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
        /// 搜索结果信息
        /// </summary>
        public SearchResult SearchResult { get; set; }
    }
}
