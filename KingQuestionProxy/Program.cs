using Topshelf;
using System;
using System.Net.NetworkInformation;
using System.Linq;

namespace KingQuestionProxy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = new Logger(Console.Out, new DebugWriter());
            Console.SetOut(logger);

            //var e = new LiZhiSearchEngine();
            //var f = e.Search(new KingQuestion
            //{
            //    data = new KingQuestionData
            //    {
            //        quiz = "标准钢琴白键有多少个",
            //        options = new[] { "51", "52", "53", "54" }
            //    }
            //});

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
