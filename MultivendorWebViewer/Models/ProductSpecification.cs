using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace MultivendorWebViewer.Models
{
    

    [Table("ProductSpecification")]
    public partial class ProductSpecification
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int SpecificationId { get; set; }

        public virtual Product Product { get; set; }

        public virtual Specification Specification { get; set; }
    }
}
