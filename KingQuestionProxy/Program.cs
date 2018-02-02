using Topshelf;
using System;
using System.Net.NetworkInformation;
using System.Linq;

namespace KingQuestionProxy
{
    public class Program
    {
        static long total = 0L;
        static long ok = 0L;
        static object root = new object();
        public static void Main(string[] args)
        {
            var logger = new Logger(Console.Out, new DebugWriter());
            Console.SetOut(logger);


            System.Net.ServicePointManager.DefaultConnectionLimit = 50;

            var sqlLite = new SqlliteContext();
            var ran = new Random();
            var datas = sqlLite.QuizAnswer.ToArray().OrderBy(item => ran.Next()).ToArray();
            datas.AsParallel().ForAll(item =>
            {
                System.Threading.Interlocked.Increment(ref total);
                var question = new KingQuestion
                {
                    data = new KingQuestionData
                    {
                        quiz = item.Quiz,
                        options = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(item.OptionsJson),
                    }
                };
                var best = Searcher.Search(question);
                if (best?.Option == item.Answer)
                {
                    System.Threading.Interlocked.Increment(ref ok);
                }

                lock (root)
                {
                    var cuect = ((double)ok / (double)total * 100d).ToString("0.00");
                    var pecent = ((double)total / (double)datas.Length * 100d).ToString("0.00");
                    Console.WriteLine(item.Quiz);
                    Console.WriteLine($"猜测答案：{best?.Option}");
                    Console.WriteLine($"正确答案：{item.Answer}");
                    Console.Title = $"进度：{pecent}%  准确率：{cuect}%";
                    Console.WriteLine();
                }
            });


            HostFactory.Run(x =>
            {
                x.Service<FiddlerApp>();
                x.RunAsLocalSystem();

                x.SetDescription("KingQuestionProxy");
                x.SetDisplayName("KingQuestionProxy");
                x.SetServiceName("KingQuestionProxy");
            });
        }
    }
}
