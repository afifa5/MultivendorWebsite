using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Routing;
using MultivendorWebViewer.Manager;

namespace MultivendorWebViewer.Common
{
    [NotMapped]
    public class ApplicationRequestContext
    {

        //public ApplicationRequestContext(RequestContext requestContext)
        //{
        //    RequestContext = requestContext;
        //    SelectedCulture = "en-GB";
        //    _CategoryManager = new CategoryManager();
        //}

        //public CategoryManager _CategoryManager { get; set; }

        //public RequestContext RequestContext { get; set; }
        public string SelectedCulture { get; set; }

    }
}