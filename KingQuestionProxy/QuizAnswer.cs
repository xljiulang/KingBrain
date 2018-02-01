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
    /// 表示要存储的问题和答案
    /// </summary>
    [Table("quizAnswer")]
    public class QuizAnswer
    {
        /// <summary>
        /// 问题
        /// </summary>
        [Key]
        [Column("quiz")]
        public string Quiz { get; set; }

        /// <summary>
        /// 答案
        /// </summary>
        [Column("answer")]
        public string Answer { get; set; }
    }
}
