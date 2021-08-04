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
    public class NodeViewModel : Node
    {
        public NodeViewModel(ApplicationRequestContext requestContext)
        {
            ApplicationRequestContext = requestContext;
        }
        public NodeViewModel(Node node,ApplicationRequestContext requestContext)
        {
            ApplicationRequestContext = requestContext;
            Id = node.Id;
            Identity = node.Identity;
            NameId = node.NameId;
            Name = node.Name;
            Description = node.Description;
            NodeImages = node.NodeImages;

            

        }
        //All Atributes
        public ApplicationRequestContext ApplicationRequestContext { get; set; }
        public string TranslatedName { get { return Name.GetTranslation(ApplicationRequestContext.SelectedCulture); } }
        public string TranslatedDescription { get { return Description.GetTranslation(ApplicationRequestContext.SelectedCulture); } }
    }
}