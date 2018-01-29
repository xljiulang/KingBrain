using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingAnswerClient
{
    /// <summary>
    /// 表示搜索结果信息
    /// </summary>
    public class SearchResult
    {
        /// <summary>
        /// 问题标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 答题选项
        /// </summary>
        public OptionMatchs[] Options { get; set; }

        /// <summary>
        /// 最佳答案
        /// </summary>
        public OptionMatchs Best { get; set; }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine(this.Title);
            var a = 'A';
            foreach (var item in this.Options)
            {
                builder.AppendLine($"{a++} {item}");
            }
            builder.Append("最佳答案：").Append(this.Best == null ? "<NULL>" : ((char)('A' + this.Best.Index)).ToString());
            return builder.ToString();
        }
    }

    /// <summary>
    /// 表示答题选项
    /// </summary>
    public class OptionMatchs
    {
        /// <summary>
        /// 索引
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Options { get; set; }

        /// <summary>
        /// 匹配度
        /// </summary>
        public int Matchs { get; set; }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{this.Options}({this.Matchs})";
        }
    }
}
