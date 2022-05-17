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
using RefactorThis.GraphDiff;
using MultivendorWebViewer.Server.Models;

namespace MultivendorWebViewer.Manager
{
    public class UserManager:SingletonBase<UserManager>
    {
        public User GetUserByUsername(string userName)
        {
            var allUsers = GetAllUsers();
            return allUsers[userName].FirstOrDefault();

        }
        public User GetUserByUserId(int id)
        {
            using (var context = new ServerModelContext(ServerModelDatabaseContextManager.Default.GetConnectionString()))
            {
                var user = context.Users.Where(p => p.Id == id).FirstOrDefault();
                return user;
            }

        }
        public Customer GetCustomerById(int id)
        {
            var allCustomer = GetAllCustomer();
            return allCustomer[id].FirstOrDefault();

        }
        public ILookup<string, User> GetAllUsers()
        {
            return CacheManager.Default.Get<ILookup<string, User>>(string.Concat("AllUser@", "MultivendorWeb"), CacheLocation.Application, () =>
            {
                using (var context = new ServerModelContext(ServerModelDatabaseContextManager.Default.GetConnectionString()))
                {
                    var allUsers = context.Users.ToLookup(t => t.UserName);
                    return allUsers;
                }
            });
        }
        public ILookup<int, Customer> GetAllCustomer()
        {
            return CacheManager.Default.Get<ILookup<int, Customer>>(string.Concat("AllCustomer@", "MultivendorWeb"), CacheLocation.Application, () =>
            {
                using (var context = new ServerModelContext(ServerModelDatabaseContextManager.Default.GetConnectionString()))
                {
                    var allCustomers = context.Customers.ToLookup(t => t.Id);
                    return allCustomers;
                }
            });
        }
        public virtual User GetCurrentUser(ApplicationRequestContext applicationRequestContext)
        {

            if (applicationRequestContext.SessionData == null) return null;
            var sessionUsers = applicationRequestContext.SessionData.User;
            var siteUser = sessionUsers.GetOrAdd("MultivendorWebUser", user =>
            {
                return new User();
            });

            return siteUser;
        }
        public virtual void SetCurrentOrder(ApplicationRequestContext applicationRequestContext, Order order)
        {
            var sessionOrders = applicationRequestContext.SessionData.Orders;
            if (order != null && order.OrderLines != null && order.OrderLines.Any())
            {
                sessionOrders["MultivendorWeb"] = order;
            }
            else
            {
                ((System.Collections.IDictionary)sessionOrders).Remove("MultivendorWeb");
            }
        }


    }
}