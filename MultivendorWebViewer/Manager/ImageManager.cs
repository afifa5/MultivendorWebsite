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
    public class ImageManager:SingletonBase<ImageManager>
    {

        public List<Image> GetImagesByIds(int[] ids)
        {

            using (var context = new MultivendorModel())
            {
                var images = context.Images.Where(i => ids.Contains(i.Id));
                return images.ToList();
            }
        }
        public Image GetImagesById(int id)
        {

            using (var context = new MultivendorModel())
            {
                var images = context.Images.Where(i => id==i.Id).FirstOrDefault();
                return images;
            }
        }
        public Task<Stream> GetImageContentStreamByNameAsync(string fileName, ApplicationRequestContext requestContext)
        {
            return Task.Run(() =>
            {
                string path = Path.Combine(requestContext.Configuration.SiteProfile.ImageLocation, fileName);

                if (File.Exists(path) == true)
                {
                    return (Stream)new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 65536, FileOptions.Asynchronous | FileOptions.SequentialScan);
                }
                else if (File.Exists(Path.Combine(HttpRuntime.AppDomainAppPath,string.Format( "App_Data\\{0}", fileName))) == true) {

                    return (Stream)new FileStream(Path.Combine(HttpRuntime.AppDomainAppPath, string.Format("App_Data\\{0}", fileName)), FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 65536, FileOptions.Asynchronous | FileOptions.SequentialScan);
                }
                return null;
            });
        }
    }
}