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
