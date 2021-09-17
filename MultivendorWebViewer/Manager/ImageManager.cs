using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MultivendorWebViewer.ViewModels;
using MultivendorWebViewer.Models;
using MultivendorWebViewer.Common;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;


namespace MultivendorWebViewer.Manager
{
    public class ImageManager
    {

        public List<Image> GetImagesByIds(int[] ids)
        {

            using (var context = new MultivendorModel())
            {
                var images = context.Images.Where(i => ids.Contains(i.Id));
                return images.ToList();
            }
        }
 
    }
}