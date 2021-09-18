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
        public int Id { get { return Model.Id; } }
        public string Identity { get { return Model.Identity; } }
        public string FormattedName { get { return Model != null && Model.Name != null ? Model.Name.GetTranslation(ApplicationRequestContext.SelectedCulture) : string.Empty; } }
        public string FormattedDescription { get { return Model != null && Model.Description != null ? Model.Description.GetTranslation(ApplicationRequestContext.SelectedCulture) :string.Empty; } }
        
        public List<ImageViewModel> Images { get { return GetImages(); } }

        public List<NodeViewModel> SubNodes { get { return GetNodes(); } }
        public List<ProductViewModel> Products { get; }
        public List<NodeViewModel> GetNodes()
        {
            var alllist = new List<NodeViewModel>();
            if (Model != null && Model.SubNodes != null && Model.SubNodes.Count() > 0)
            {
                var ids = Model.SubNodes.Select(p => p.Id).ToArray();
                var categoryNodes = ApplicationRequestContext.CategoryManager.GetNodesByIds(ids);
                foreach (var item in categoryNodes)
                {
                    alllist.Add(new NodeViewModel(item, ApplicationRequestContext));
                }
            }
            return alllist;
        }
        public List<ImageViewModel> GetImages()
        {
            var alllist = new List<ImageViewModel>();
            if (Model != null && Model.NodeImages != null && Model.NodeImages.Count() > 0)
            {
                var ids = Model.NodeImages.Select(p => p.Id).ToArray();
                var categoryImages = ApplicationRequestContext.ImageManager.GetImagesByIds(ids);

                foreach (var item in categoryImages)
                {
                    alllist.Add(new ImageViewModel(item, ApplicationRequestContext));
                }
            }
            return alllist;
        }
    }
}