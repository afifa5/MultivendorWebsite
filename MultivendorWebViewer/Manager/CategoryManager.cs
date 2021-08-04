using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MultivendorWebViewer.ViewModels;
using MultivendorWebViewer.Models;
using MultivendorWebViewer.Common;

namespace MultivendorWebViewer.Manager
{
    public class CategoryManager
    {
        public CategoryViewModel GetCategory(int id, ApplicationRequestContext requestContext)
        {
            using (var context=new MultivendorModel())
            {
                var category = context.Categories.Where(i => i.Id == id).FirstOrDefault();
                if (category != null)
                { 

                    return new CategoryViewModel(category, requestContext);
                }
                else return null;
            }
            
        }
    }
}