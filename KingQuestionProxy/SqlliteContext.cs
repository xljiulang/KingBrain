using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingQuestionProxy
{
    /// <summary>
    /// 表示sqllite数据库上下文
    /// </summary>
    public class SqlliteContext : DbContext
    {
        /// <summary>
        /// db路径
        /// </summary>
        public static readonly string DbFile = "data\\data.db";


        /// <summary>
        /// 问题和答案
        /// </summary>
        public DbSet<QuizAnswer> QuizAnswer { get; set; }


        /// <summary>
        /// sqllite数据库上下文
        /// </summary>
        public SqlliteContext()
            : this(DbFile)
        {
        }

        /// <summary>
        /// sqllite数据库上下文
        /// </summary>
        /// <param name="dbFile">db文件路径</param>
        public SqlliteContext(string dbFile)
            : base($"Data Source={dbFile};Pooling=True")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }
    }
}
