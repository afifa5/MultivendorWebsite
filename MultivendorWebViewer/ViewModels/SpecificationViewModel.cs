using MultivendorWebViewer.Common;
using MultivendorWebViewer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MultivendorWebViewer.ViewModels
{
    public class SpecificationViewModel
    {
        public SpecificationViewModel(Specification product, ApplicationRequestContext requestContext)
        {
            ApplicationRequestContext = requestContext;
            Model = product;
        }
        private Specification Model { get; set; }
        //All Atributes
        public ApplicationRequestContext ApplicationRequestContext { get; set; }
        public int Id { get { return Model.Id; } }
        public string Identity { get { return Model.Identity; } }
        public SpecificationMode SpecificationMode { get { return Model.SpecificationMode== 0 ? SpecificationMode.Description:(Model.SpecificationMode == 1?SpecificationMode.Highlighted:SpecificationMode.Selection); } }
        public SpecificationTypeViewModel SpecificationType { get { return GetSpecificationType(); } }
        public string FormattedName { get { return Model != null && Model.SpecificationText != null ? Model.SpecificationText.GetTranslation(ApplicationRequestContext.SelectedCulture) : string.Empty; } }

        private SpecificationTypeViewModel GetSpecificationType() {
            var specificationType = ApplicationRequestContext.ProductManager.GetSpecificationType(Model.SpecificationTypeId);
            return new SpecificationTypeViewModel(specificationType, ApplicationRequestContext);
        }
    }
    public enum SpecificationMode
    {
        Description = 0,
        Highlighted = 1,
        Selection = 2,
    }
}