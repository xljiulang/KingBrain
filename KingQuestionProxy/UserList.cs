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

            watcher = new FileSystemWatcher(path, filter);
            watcher.Changed += Watcher_Changed;
            watcher.EnableRaisingEvents = true;

            var users = File.ReadAllText(Path.Combine(path, filter))
                .Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Select(user => new UserIpAddress { User = user });

            userIpAddressList.AddRange(users);
        }

        /// <summary>
        /// userList.txt变化后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                        select item ?? new UserIpAddress { User = u, IpAddress = default(string) };

                var datas = q.ToArray();
                userIpAddressList.Clear();
                userIpAddressList.AddRange(datas);

                foreach (var ui in datas)
                {
                    Console.WriteLine("正在加载用户：" + ui);
                }
            }
        }

        /// <summary>
        /// 获取所有用户和其ip
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 检测客户端ip是否可以使用代理
        /// </summary>
        /// <param name="ipAddress">ip</param>
        /// <returns></returns>
        public static bool IsAcceptIpAddress(string ipAddress)
        {
            lock (syncRoot)
            {
                return userIpAddressList.Any(item => item.IpAddress == ipAddress);
            }
        }

        /// <summary>
        /// 更新用户的ip
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="ipAddress">ip</param>
        /// <returns></returns>
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
