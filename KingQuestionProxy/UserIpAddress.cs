using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingQuestionProxy
{
    /// <summary>
    /// 表示用户和ip
    /// </summary>
    [Table("users")]
    public class UserIpAddress
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Key]
        [Required]
        [StringLength(20)]
        [Column("name")]
        public string Name { get; set; }

        /// <summary>
        /// ip地址
        /// </summary>
        [Required]
        [StringLength(40)]
        [Column("ipAddress")]
        public string IpAddress { get; set; }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Name} {IpAddress}";
        }
    }
}
