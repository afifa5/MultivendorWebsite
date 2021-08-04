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
    public class CategoryViewModel : Category
    {
        public CategoryViewModel()
        {
            ApplicationRequestContext=null;
        }
        public CategoryViewModel(ApplicationRequestContext requestContext)
        {
            ApplicationRequestContext = requestContext;
        }
        public CategoryViewModel(Category category, ApplicationRequestContext requestContext)
        {
            ApplicationRequestContext = requestContext;
            Id = category.Id;
            Identity = category.Identity;

            NameId = category.NameId;

            DescriptionId = category.DescriptionId;

            Name = category.Name;
            Description = category.Description;
            CategoryImages = category.CategoryImages;

            CategoryNodes = category.CategoryNodes;




        }
        //All Atributes
        public ApplicationRequestContext ApplicationRequestContext { get; set; }
        public string TranslatedName { get { return Name.GetTranslation(ApplicationRequestContext.SelectedCulture); } }
        public string TranslatedDescription { get { return Description.GetTranslation(ApplicationRequestContext.SelectedCulture); } }

        public List<NodeViewModel> Nodes { get { return GetNodes(); } }


        //All Functions
        public List<NodeViewModel> GetNodes()
        {
            var alllist = new List<NodeViewModel>();
            if (CategoryNodes.Count() > 0)
            {
                foreach(var item in CategoryNodes)
                {
                    alllist.Add(new NodeViewModel(item.Node, ApplicationRequestContext));
                }
            }
            return alllist;
        }
        
        
    }
}