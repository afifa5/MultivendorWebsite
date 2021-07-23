using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace MultivendorWebViewer.Models
{
   

    public partial class SubNode
    {
        public int Id { get; set; }

        public int NodeId { get; set; }

        public int SubNodeId { get; set; }

        public virtual Node Node { get; set; }

        public virtual Node SubNodeItem { get; set; }
    }
}
