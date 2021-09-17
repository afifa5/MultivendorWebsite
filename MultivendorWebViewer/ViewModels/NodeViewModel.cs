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
    public class NodeViewModel 
    {
        public NodeViewModel(Node node, ApplicationRequestContext requestContext)
        {
            ApplicationRequestContext = requestContext;
            Model = node;
        }
        private Node Model { get; set; }
        //All Atributes
        public ApplicationRequestContext ApplicationRequestContext { get; set; }
        public string FormattedName { get { return Model != null && Model.Name != null ? Model.Name.GetTranslation(ApplicationRequestContext.SelectedCulture) : string.Empty; } }
        public string FormattedDescription { get { return Model != null && Model.Description != null ? Model.Description.GetTranslation(ApplicationRequestContext.SelectedCulture) :string.Empty; } }
        public List<ProductViewModel> Products { get; }
    }
}