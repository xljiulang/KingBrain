using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingQuestionProxy
{
    public class SqlliteContext : DbContext
    {
        /// <summary>
        /// db路径
        /// </summary>
        public static readonly string DbFile = "data\\data.db";

        public SqlliteContext()
            : this(DbFile)
        {
        }

        public SqlliteContext(string dbFile)
            : base($"Data Source={dbFile};Pooling=True")
        {
        }

        public DbSet<QuizAnswer> QuizAnswer { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }
    }
}
