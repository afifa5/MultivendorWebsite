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
using Microsoft.Ajax.Utilities;

namespace MultivendorWebViewer.Manager
{
    public class ProductManager:SingletonBase<ProductManager>
    {
        public List<Product> GetProductByIds(int[] ids)
        {
            var allProduct = GetAllProduct();
            var products = new List<Product>();
            ids.ForEach(i => {
                products.Add(allProduct[i].FirstOrDefault());
            });
            return products;

        }
        public ILookup<int, Product> GetAllProduct() {
           return CacheManager.Default.Get<ILookup<int,Product>>(string.Concat("AllPrduct@", "MultivendorWeb"), CacheLocation.Application, () =>
            {
                using (var context = new MultivendorModelContext(SiteModelDatabaseContextManager.Default.GetConnectionString()))
                {
                    var product = context.Products
                        .Include(p => p.ProductImages)
                         .Include(p => p.ProductVideos)
                        .Include(p => p.Name.TextTranslations)
                        .Include(p => p.Description.TextTranslations)
                        .Include(p => p.ProductSpecifications).ToLookup(t=>t.Id);
                    return product;
                }
            });
        }
        public Product GetProductById(int id)
        {
            var allProduct = GetAllProduct();
           return allProduct[id].FirstOrDefault();
            //using (var context = new MultivendorModel(ServerModelDatabaseContextManager.Default.GetConnectionString()))
            //{
            //    var product = context.Products.Where(i =>i.Id== id)
            //        .Include(p => p.ProductImages)
            //        .Include(p => p.Name.TextTranslations)
            //        .Include(p => p.Description.TextTranslations)
            //        .Include(p => p.ProductSpecifications).FirstOrDefault();
            //    return product;
            //}
        }
        public List<PriceAvailability> GetpriceByproductId(int id)
        {
            using (var context = new MultivendorModelContext(SiteModelDatabaseContextManager.Default.GetConnectionString()))
            {
                var prices = context.PriceAvailabilities.Where(i => i.ProductId == id);
                return prices.ToList();
            }
        }
        public SpecificationType GetSpecificationType(int id) {
            var allSpecificationsType = GetAllSpecificationsType();
           return allSpecificationsType[id].FirstOrDefault();
            //using (var context = new MultivendorModel(ServerModelDatabaseContextManager.Default.GetConnectionString()))
            //{
            //    var specificationType = context.SpecificationTypes.Where(i =>i.Id == id).Include(p => p.SpecificationTypeText.TextTranslations).FirstOrDefault();
            //    return specificationType;
            //}
        }
        public List<Specification> GetSpecifications(int[] ids)
        {
            var allSpecifications = GetAllSpecifications();
            var specifications = new List<Specification>();
            ids.ForEach(p => specifications.Add(allSpecifications[p].FirstOrDefault()));
            return specifications;
            //using (var context = new MultivendorModel(ServerModelDatabaseContextManager.Default.GetConnectionString()))
            //{
            //    var specificationType = context.Specifications.Where(i => ids.Contains(i.Id)).Include(p => p.SpecificationText.TextTranslations);
            //    return specificationType.ToList();
            //}
        }
        public ILookup<int, Specification> GetAllSpecifications()
        {
            return CacheManager.Default.Get<ILookup<int,Specification>>(string.Concat("AllSpecification@", "MultivendorWeb"), CacheLocation.Application, () =>
            {
                using (var context = new MultivendorModelContext(SiteModelDatabaseContextManager.Default.GetConnectionString()))
                {
                    var specifications = context.Specifications.Include(p => p.SpecificationText.TextTranslations).ToLookup(l => l.Id);
                    return specifications;
                }
            });
        }
        public ILookup<int, SpecificationType> GetAllSpecificationsType()
        {
            return CacheManager.Default.Get<ILookup<int, SpecificationType>>(string.Concat("AllSpecificationType@", "MultivendorWeb"), CacheLocation.Application, () =>
            {
                using (var context = new MultivendorModelContext(SiteModelDatabaseContextManager.Default.GetConnectionString()))
                {
                    var specificationsType = context.SpecificationTypes.Include(p => p.SpecificationTypeText.TextTranslations).ToLookup(l => l.Id);
                    return specificationsType;
                }
            });
        }

    }
}