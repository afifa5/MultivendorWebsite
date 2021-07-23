using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace MultivendorWebViewer.Models
{
   

    public partial class ProductNode
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int NodeId { get; set; }

        public virtual Node Node { get; set; }
        public virtual Product Product { get; set; }

    }
}
