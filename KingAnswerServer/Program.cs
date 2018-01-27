using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KingAnswerServer.Serarchers;
using KingQuestion;
using NetworkSocket;
using NetworkSocket.Http;
using Newtonsoft.Json;

namespace KingAnswerServer
{
    class Program
    {
        private static readonly TcpListener httpServer = new TcpListener();

        static void Main(string[] args)
        { 
            httpServer.Use<HttpMiddleware>();
            httpServer.Start(5544);
            Console.Read();
        }
    }
}
