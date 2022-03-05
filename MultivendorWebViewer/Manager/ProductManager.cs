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
using System.Threading.Tasks;
using System.IO;
namespace MultivendorWebViewer.Manager
{
    public class ProductManager
    {
        public List<Product> GetProductByIds(int[] ids)
        {

            using (var context = new MultivendorModel())
            {
                var product = context.Products.Where(i => ids.Contains(i.Id)).Include(p=>p.ProductImages);
                return product.ToList();
            }
        }
    }
}