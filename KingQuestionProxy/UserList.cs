using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace KingQuestionProxy
{
    /// <summary>
    /// 用户列表
    /// </summary>
    public static class UserList
    {
        private static readonly FileSystemWatcher watcher;

        private static readonly object syncRoot = new object();

        private static readonly List<UserIpAddress> userIpAddressList = new List<UserIpAddress>();

        static UserList()
        {
            var path = "Data";
            var filter = "userList.txt";

            watcher = new FileSystemWatcher();
            watcher.Path = path;
            watcher.Filter = filter;
            watcher.Changed += Watcher_Changed;
            watcher.EnableRaisingEvents = true;
            watcher.IncludeSubdirectories = true;

            watcher.NotifyFilter = NotifyFilters.Attributes
                | NotifyFilters.CreationTime
                | NotifyFilters.DirectoryName
                | NotifyFilters.FileName
                | NotifyFilters.LastAccess
                | NotifyFilters.LastWrite
                | NotifyFilters.Security
                | NotifyFilters.Size;

            var users = File.ReadAllText(Path.Combine(path, filter))
                .Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Select(user => new UserIpAddress { User = user });

            userIpAddressList.AddRange(users);
            foreach (var ui in userIpAddressList)
            {
                Console.WriteLine("正在加载用户：" + ui);
            }
        }

        public static void Init()
        {
        }

        private static void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            lock (syncRoot)
            {
                var users = File.ReadAllText(e.FullPath)
                  .Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                var q = from u in users
                        join ui in userIpAddressList
                        on u equals ui.User
                        into g
                        from item in g.DefaultIfEmpty()
                        select item == null ? new UserIpAddress { User = u, IpAddress = default(string) } : item;

                var datas = q.ToArray();
                userIpAddressList.Clear();
                userIpAddressList.AddRange(datas);

                foreach (var ui in datas)
                {
                    Console.WriteLine("正在加载用户：" + ui);
                }
            }
        }

        public static UserIpAddress[] GetUserIpAddress()
        {
            lock (syncRoot)
            {
                var q = from ui in userIpAddressList
                        where ui.IpAddress != null
                        let ip = Regex.Match(ui.IpAddress, @"\d+\.\d+\.\d+\.\d+").Value
                        select new UserIpAddress { User = ui.User, IpAddress = ip };

                return q.ToArray();
            }
        }

        public static bool IsAcceptIpAddress(string ipAddress)
        {
            lock (syncRoot)
            {
                return userIpAddressList.Any(item => item.IpAddress == ipAddress);
            }
        }

        public static bool UpdateIpAddress(string user, string ipAddress)
        {
            lock (syncRoot)
            {
                var userIp = userIpAddressList.FirstOrDefault(item => item.User == user);
                if (userIp != null)
                {
                    userIp.IpAddress = ipAddress;
                    return true;
                }
                return false;
            }
        }
    }
}
