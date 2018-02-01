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
    /// 表示要存储到db的问题和答案
    /// </summary>
    [Table("quizAnswer")]
    public class QuizAnswer
    {
        /// <summary>
        /// 问题
        /// </summary>
        [Key]
        [Column("quiz")]
        [Required]
        [StringLength(100)]
        public string Quiz { get; set; }

        /// <summary>
        /// 选项卡json
        /// </summary>
        [Column("optionsJson")]
        [Required]
        [StringLength(100)]
        public string OptionsJson { get; set; }

        /// <summary>
        /// 答案
        /// </summary>
        [Column("answer")]
        [Required]
        [StringLength(20)]
        public string Answer { get; set; }
    }
}
