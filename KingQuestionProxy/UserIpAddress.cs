using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingQuestionProxy
{
    /// <summary>
    /// 表示用户和ip
    /// </summary>
    public class UserIpAddress
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// ip地址
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{User} {IpAddress}";
        }
    }
}
