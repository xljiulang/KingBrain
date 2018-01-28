using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingAnswerClient
{
    public class SearchResult
    {
        public string Title { get; set; }

        public OptionMatchs[] Options { get; set; }

        public OptionMatchs Best { get; set; }

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

    public class OptionMatchs
    {
        public int Index { get; set; }

        public string Options { get; set; }

        public int Matchs { get; set; }

        public override string ToString()
        {
            return $"{this.Options}({this.Matchs})";
        }
    }
}
