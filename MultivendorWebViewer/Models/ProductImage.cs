using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace MultivendorWebViewer.Models
{
   

    public partial class ProductImage
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int ImageId { get; set; }

        public virtual Image Image { get; set; }

        public virtual Product Product { get; set; }
    }
}
