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
    public class CategoryManager: SingletonBase<CategoryManager>
    {
        public Category GetCategory(int id, ApplicationRequestContext requestContext)
        {
            
            using (var context= SiteModelDatabaseContextManager.Default.Context())
            {
                var category = context.Categories.Where(i => i.Id == id)
                    .Include(p => p.CategoryNodes)
                    .Include(p=> p.CategoryImages)
                    .Include(p=>p.Name.TextTranslations)
                    .Include(p=>p.Description.TextTranslations).FirstOrDefault();
                if (category != null)
                { 

                    return category;
                }
                else return null;
            }
            
        }

        public List<Node> GetNodesByIds(int[] ids) {
           
            using (var context = SiteModelDatabaseContextManager.Default.Context())
            {
                var nodes = context.Nodes.Where(i => ids.Contains(i.Id))
                    .Include(p => p.Name.TextTranslations)
                    .Include(p => p.NodeImages)
                    .Include(p => p.Description.TextTranslations);
                return nodes.ToList();
            }
        }
        public SubNode GetSubnode(int id)
        {

            using (var context = SiteModelDatabaseContextManager.Default.Context())
            {
                var nodes = context.SubNodes.Where(i => i.SubNodeItemId == id);
                return nodes.FirstOrDefault();
            }
        }
       
        public Node GetNodeById(int id)
        {

            using (var context = SiteModelDatabaseContextManager.Default.Context())
            {
                var node = context.Nodes.Where(i => id== i.Id)
                    .Include(p=>p.NodeImages)
                    .Include(p => p.SubNodes)
                    .Include(p => p.ProductNodes)
                    .Include(p => p.Name.TextTranslations)
                    .Include(p => p.Description.TextTranslations).FirstOrDefault();
                return node;
            }
        }


    }
}