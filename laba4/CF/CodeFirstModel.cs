using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace laba4
{
    public partial class CodeFirstModel : DbContext
    {
        public CodeFirstModel()
            : base("name=CodeFirstModel")
        {
        }

        public virtual DbSet<Category1> Categories { get; set; }
        public virtual DbSet<Good1> Goods { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Good1>()
                .HasOptional(e => e.Category1)
                .WithRequired(e => e.Good);
        }
    }
}
