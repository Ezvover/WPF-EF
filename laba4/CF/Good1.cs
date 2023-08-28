namespace laba4
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Good1
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id { get; set; }

        [StringLength(50)]
        public string name { get; set; }

        [StringLength(50)]
        public string desc { get; set; }

        [StringLength(50)]
        public string category { get; set; }

        public int? rate { get; set; }

        public int? price { get; set; }

        public int? amount { get; set; }

        public string other { get; set; }

        [Column(TypeName = "image")]
        public byte[] picture { get; set; }

        public virtual Category1 Category1 { get; set; }
    }
}