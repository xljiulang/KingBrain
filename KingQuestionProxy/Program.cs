using Topshelf;

namespace KingQuestionProxy
{
    public class Program
    {
        public static void Main(string[] args)
        {
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
