using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace MultivendorWebViewer.Models
{
    

    [Table("Product")]
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            
            ProductImages = new HashSet<ProductImage>();
            ProductSpecifications = new HashSet<ProductSpecification>();
        }

        public int Id { get; set; }

        [StringLength(50)]
        public string Identity { get; set; }

        public int? NameId { get; set; }

        public int? DescriptionId { get; set; }

        


        public virtual Text Name { get; set; }

        public virtual Text Description { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductImage> ProductImages { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductSpecification> ProductSpecifications { get; set; }
    }
}
