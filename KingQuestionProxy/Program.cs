using Topshelf;
using System;
using System.Net.NetworkInformation;
using System.Linq;
using System.Net;
using System.Threading;

namespace KingQuestionProxy
{
    public class Program
    {
        static long current = 0L;
        static long correct = 0L;
        static readonly object syncRoot = new object();

        public static void Main(string[] args)
        {
            var logger = new Logger(Console.Out, new DebugWriter());
            Console.SetOut(logger);
            ServicePointManager.DefaultConnectionLimit = 1024;

            if (args.FirstOrDefault() == "TestSearchEngine")
            {
                TestSearchEngine();
            }

            HostFactory.Run(x =>
            {
                x.Service<FiddlerApp>();
                x.RunAsLocalSystem();

                x.SetDescription("KingQuestionProxy");
                x.SetDisplayName("KingQuestionProxy");
                x.SetServiceName("KingQuestionProxy");
            });
        }

        /// <summary>
        /// 测试搜索引擎
        /// </summary>
        static void TestSearchEngine()
        {
            var ran = new Random();
            var sqlLite = new SqlliteContext();
            var datas = sqlLite.QuizAnswer.ToArray().OrderBy(item => ran.Next()).ToArray();

            datas.AsParallel().ForAll(item =>
            {
                Interlocked.Increment(ref current);
                var question = item.ToKingQuestion();

                var best = Searcher.Search(question);
                if (best?.Option == item.Answer)
                {
                    Interlocked.Increment(ref correct);
                }

                lock (syncRoot)
                {
                    var correctRrate = ((double)correct / (double)current * 100d).ToString("0.00");
                    Console.WriteLine(item.Quiz);
                    Console.WriteLine($"猜测答案：{best?.Option}");
                    Console.WriteLine($"正确答案：{item.Answer}");
                    Console.Title = $"进度：{current}/{datas.Length}  准确率：{correctRrate}%";
                    Console.WriteLine();
                }
            });

        }
    }
}
