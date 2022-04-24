using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MultivendorWebViewer.Models;
using MultivendorWebViewer.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultivendorWebViewer.ViewModels
{
    public class NavigationPathViewModel
    {
        public NavigationPathViewModel( ApplicationRequestContext requestContext)
        {
            ApplicationRequestContext = requestContext;
        }

        public ApplicationRequestContext ApplicationRequestContext { get; set; }
        public string Name { get; set; }

        public string Identity { get; set; }

        public string DisplayText { get; set; }

        public string Url { get; set; }

        public string Icon { get; set; }

        public object HtmlAttributes { get; set; }
    }
   
}