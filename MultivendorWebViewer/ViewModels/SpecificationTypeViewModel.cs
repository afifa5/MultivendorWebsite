using MultivendorWebViewer.Common;
using MultivendorWebViewer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MultivendorWebViewer.ViewModels
{
    public class SpecificationTypeViewModel
    {
        public SpecificationTypeViewModel(SpecificationType product, ApplicationRequestContext requestContext)
        {
            ApplicationRequestContext = requestContext;
            Model = product;
        }
        private SpecificationType Model { get; set; }
        //All Atributes
        public ApplicationRequestContext ApplicationRequestContext { get; set; }
        public int Id { get { return Model.Id; } }
        public string Identity { get { return Model.Identity; } }
        public Text SpecificationType => Model.SpecificationTypeText;
        public int? Mode { get { return Model.SpecificationTypeMode; } }
        public string FormattedName { get { return Model != null && Model.SpecificationTypeText != null ? Model.SpecificationTypeText.GetTranslation(ApplicationRequestContext.SelectedCulture) : string.Empty; } }
    }
    public enum SpecificationTypeMode { 
        String = 0,
        Text = 1,
        Number = 2,
    }
}