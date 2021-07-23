using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace MultivendorWebViewer.Models
{
   

    public partial class CategoryImage
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }

        public int ImageId { get; set; }

        public virtual Category Category { get; set; }

        public virtual Image Image { get; set; }
    }
}
