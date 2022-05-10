using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MultivendorWebViewer.Models
{

  
    public partial class CategoryImage
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public int ImageId { get; set; }

        [ForeignKey("ImageId")]
        public  Image Image { get; set; }

        [ForeignKey("CategoryId")]
        public  Category Category { get; set; }
    }
}
