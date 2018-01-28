using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingAnswerServer
{
    public class SearchResult
    {
        const string answerPath = "Answer";

        public string Title { get; set; }

        public OptionMatchs[] Options { get; set; }

        public OptionMatchs Best { get; set; }

        public static SearchResult SearchFromLocal(string title)
        {
            var file = GetFilePath(title);
            if (File.Exists(file) == false)
            {
                return null;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("本答案是从本地缓存获取...");
            Console.ForegroundColor = ConsoleColor.Gray;

            var json = File.ReadAllText(file, Encoding.UTF8);
            return JsonConvert.DeserializeObject<SearchResult>(json);
        }


        private static string GetFilePath(string title)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                title = title.Replace(c.ToString(), null);
            }

            return Path.Combine(answerPath, title + ".txt");
        }


        public void UpdateBestAndSave(int index)
        {
            this.Best = this.Options[index];
            var file = GetFilePath(this.Title);
            Directory.CreateDirectory(answerPath);
            var fileName = Path.Combine(file);
            var json = JsonConvert.SerializeObject(this);
            File.WriteAllText(fileName, json, Encoding.UTF8);
        }

        public override string ToString()
        {
            return this.ToString(true);
        }

        public string ToString(bool withTitle)
        {
            var builder = new StringBuilder();
            if (withTitle == true)
            {
                builder.AppendLine(this.Title);
            }

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
