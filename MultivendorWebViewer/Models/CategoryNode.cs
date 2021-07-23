using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace MultivendorWebViewer.Models
{
 

    public partial class CategoryNode
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }

        public int NodeId { get; set; }

        public virtual Category Category { get; set; }

        public virtual Node Node { get; set; }
    }
}
