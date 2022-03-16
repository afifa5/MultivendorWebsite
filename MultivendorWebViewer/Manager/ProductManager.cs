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
                var product = context.Products.Where(i => ids.Contains(i.Id))
                    .Include(p=>p.ProductImages)
                    .Include(p => p.Name.TextTranslations)
                    .Include(p => p.Description.TextTranslations)
                    .Include(p=> p.ProductSpecifications);
                return product.ToList();
            }
        }
        public Product GetProductById(int id)
        {

            using (var context = new MultivendorModel())
            {
                var product = context.Products.Where(i =>i.Id== id)
                    .Include(p => p.ProductImages)
                    .Include(p => p.Name.TextTranslations)
                    .Include(p => p.Description.TextTranslations)
                    .Include(p => p.ProductSpecifications).FirstOrDefault();
                return product;
            }
        }
        public List<PriceAvailability> GetpriceByproductId(int id)
        {
            using (var context = new MultivendorModel())
            {
                var prices = context.PriceAvailabilities.Where(i => i.ProductId == id);
                return prices.ToList();
            }
        }
        public SpecificationType GetSpecificationType(int id) {
            using (var context = new MultivendorModel())
            {
                var specificationType = context.SpecificationTypes.Where(i =>i.Id == id).Include(p => p.SpecificationTypeText.TextTranslations).FirstOrDefault();
                return specificationType;
            }
        }
        public List<Specification> GetSpecifications(int[] ids)
        {
            using (var context = new MultivendorModel())
            {
                var specificationType = context.Specifications.Where(i => ids.Contains(i.Id)).Include(p => p.SpecificationText.TextTranslations);
                return specificationType.ToList();
            }
        }
    }
}