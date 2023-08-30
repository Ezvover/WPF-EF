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

        public virtual DbSet<Categories> Categories { get; set; }
        public virtual DbSet<Goods> Goods { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Goods>()
                .HasOptional(e => e.Category1)
                .WithRequired(e => e.Good);
        }
    }
}
