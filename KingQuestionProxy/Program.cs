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
            var ipAddress = from i in NetworkInterface.GetAllNetworkInterfaces()
                            where i.OperationalStatus == OperationalStatus.Up
                            let address = i.GetIPProperties().UnicastAddresses
                            select address.ToArray();

            var okIp = from ip in ipAddress.SelectMany(item => item)
                       where ip.DuplicateAddressDetectionState == DuplicateAddressDetectionState.Preferred
                       where ip.PrefixOrigin == PrefixOrigin.Dhcp || ip.PrefixOrigin == PrefixOrigin.Manual
                       where ip.SuffixOrigin == SuffixOrigin.OriginDhcp || ip.SuffixOrigin == SuffixOrigin.Manual
                       select ip;

            Console.WriteLine(okIp.FirstOrDefault().Address);

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
