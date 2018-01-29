using System;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace KingAnswerClient
{
    class Program
    {
        private static readonly Uri proxyServer = new Uri(ConfigurationManager.AppSettings["proxyServer"]);

        private static KingClient client = new KingClient(proxyServer);

        static void Main(string[] args)
        {
            Console.WriteLine("正在连接到服务器...");
            var state = client.Connect();
            Console.WriteLine($"连接状态：{state}");
            if (state != SocketError.Success)
            {
                return;
            }

            while (true)
            {
                Console.WriteLine("请输入手机使用的IP：");

                IPAddress ip;
                var ipAddress = Console.ReadLine();
                if (IPAddress.TryParse(ipAddress, out ip) == false)
                {
                    Console.WriteLine("IP格式错误");
                    continue;
                }
                else
                {
                    client.BindIpAddress(ip);
                    break;
                }
            }

            Console.WriteLine("现在可以进行手机游戏了");
            Console.WriteLine();

            while (true)
            {
                Console.Read();
            }
        }
    }
}
