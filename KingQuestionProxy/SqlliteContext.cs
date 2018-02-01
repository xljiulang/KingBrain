using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SQLite;
using System.Data.SQLite.EF6;
using System.Linq;
using System.Reflection;
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
        /// <param name="pooling">pool连接方式</param>
        public SqlliteContext(string dbFile, bool pooling = true)
            : base(CreateConnection(dbFile, pooling), true)
        {
        }

        /// <summary>
        /// 创建连接
        /// </summary>
        /// <param name="dbFile"></param>
        /// <param name="pooling">pool连接方式</param>
        /// <returns></returns>
        private static SQLiteConnection CreateConnection(string dbFile, bool pooling)
        {
            var constring = $"Data Source={dbFile};Pooling={pooling}";
            return new SQLiteConnection(constring);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }
    }
}
