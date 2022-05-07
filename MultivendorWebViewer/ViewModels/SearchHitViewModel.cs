using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MultivendorWebViewer.Models;
using MultivendorWebViewer.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultivendorWebViewer.ViewModels
{
    [NotMapped]
    public class SearchHitViewModel 
    {
        public SearchHitViewModel(string searchTerm) {
            SearchTerm = searchTerm;
        }
        public string SearchTerm { get; set; }
        public IEnumerable<ProductViewModel> Products { get; set; }

        public int? DisplayCount { get; set; }
        public int? TotalCount { get; set; }
    }
}