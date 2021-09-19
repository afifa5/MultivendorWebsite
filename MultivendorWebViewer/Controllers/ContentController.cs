
using MultivendorWebViewer.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.SessionState;
using System.Web.UI;

namespace MultivendorWebViewer.Controllers
{
    public class ImageConversionResult
    {
        public FileResult FileResult { get; set; }

        public string SourceFileName { get; set; }

        public Exception Exception { get; set; }
    }

    public class ScaleImageConversion 
    {
        public virtual InterpolationMode Interpolation { get { return InterpolationMode.HighQualityBicubic; } }

        public  Task<ImageConversionResult> GetFileResultAsync(Stream stream, string fileName)
        {
            return Task.Run(() =>
            {
                var timer = System.Diagnostics.Stopwatch.StartNew();

                

                //if (width.HasValue == true || height.HasValue == true)
                //{
                //    //return Scale(stream, fileName);
                //    try
                //    {
                //        int? integerWidth = width.HasValue == true ? (int?)Math.Ceiling(width.Value) : null;
                //        int? integerHeight = height.HasValue == true ? (int?)Math.Ceiling(height.Value) : null;
                //        return Scale(stream, fileName, integerWidth, integerHeight, Cover, EntropyCropTreshold);
                       
                //    }
                //    catch (Exception)
                //    {
                //        //throw;
                //        return new ImageConversionResult { FileResult = new FileStreamResult(stream, Mime.GetMimeMapping(fileName)), SourceFileName = fileName };
                //    }
                //}

                return new ImageConversionResult { FileResult = new FileStreamResult(stream, Mime.GetMimeMapping(fileName)), SourceFileName = fileName };
            });
        }

    }

    [SessionState(SessionStateBehavior.Disabled)]
    public class ContentController : BaseController
    {
        [OutputCache(Duration = 3600, Location = OutputCacheLocation.Client)]
        public virtual async Task<FileResult> Image(int id, string fileName = null)
        {
            Response.Cache.SetOmitVaryStar(true);

            try
            {
                if (fileName == null)
                {
                    fileName = ApplicationRequestContext.ImageManager.GetImagesById(id).ImageName;
                }

                Stream contentStream = null;

                if (!string.IsNullOrEmpty(fileName))
                {
                    contentStream = await ApplicationRequestContext.ImageManager.GetImageContentStreamByNameAsync(fileName, ApplicationRequestContext);
                }

                if (contentStream == null)
                {
                    return null;
                }

                    var convertedResult = await new ScaleImageConversion().GetFileResultAsync(contentStream, fileName);
                    return convertedResult.FileResult;
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        //[OutputCache(Duration = 3600 * 4, Location = OutputCacheLocation.Any, VaryByParam = "*")]
        //public virtual async Task<FileResult> ImageThumbnail(int id, int? width, int? height, int? sampling = null, string fileName = null, bool cover = false, int entropyCrop = 0/*, ImageConversion converter = null*/, string source = null, bool assetImage = false)
        //{
        //    Response.Cache.SetOmitVaryStar(true);

        //    var applicationRequestContext = ApplicationRequestContext.GetContext(HttpContext);
        //    var siteContext = applicationRequestContext.SiteContext;

        //    if (fileName == null)
        //    {
        //        fileName = assetImage ? siteContext.ImageManager.GetAssetImageFileName(id) : siteContext.ImageManager.GetImageFileName(id);
        //    }

        //    if (siteContext.SiteDataCacheManager != null && !string.IsNullOrEmpty(fileName))
        //    {
        //        var cachedFileResult = await siteContext.SiteDataCacheManager.GetCachedThumbnailFileAsync(fileName, width ?? -1, height ?? -1, sampling ?? 1);
        //        if (cachedFileResult != null)
        //        {
        //            return cachedFileResult;
        //        }
        //    }
        //    /*id missing*/
        //    if (id <= 0 && assetImage) {
        //        //fallbackImage
        //        var assetManager = applicationRequestContext.SiteContext.AssetManager;
        //        if (assetManager != null)
        //        {
        //            if (assetManager.DefaultImage != null && !string.IsNullOrEmpty(assetManager.DefaultImage.PersistentIdentiy))
        //            {
        //                fileName = siteContext.ImageManager.GetAssetImageName(assetManager.DefaultImage.PersistentIdentiy);
        //            }
        //            else if (assetManager.DefaultImage != null && !string.IsNullOrEmpty(assetManager.DefaultImage.FileName))
        //            {

        //                fileName = assetManager.DefaultImage.FileName;
        //            }

        //        }
        //    }

        //    var imageContentStream = assetImage ? await siteContext.ImageManager.GetAssetImageContentStreamByNameAsync(fileName) : await siteContext.ImageManager.GetImageContentStreamByNameAsync(fileName);
           
        //    if (imageContentStream == null && assetImage && id > 0)
        //    {
        //        //fallbackImage
        //        var assetManager = applicationRequestContext.SiteContext.AssetManager;
        //        if (assetManager != null)
        //        {
        //            if (assetManager.DefaultImage != null && !string.IsNullOrEmpty(assetManager.DefaultImage.PersistentIdentiy))
        //            {
        //                fileName = siteContext.ImageManager.GetAssetImageName(assetManager.DefaultImage.PersistentIdentiy);
        //                imageContentStream = await siteContext.ImageManager.GetAssetImageContentStreamByNameAsync(fileName);

        //            }
        //            else if (assetManager.DefaultImage != null && !string.IsNullOrEmpty(assetManager.DefaultImage.FileName))
        //            {
        //                imageContentStream = await siteContext.ImageManager.GetAssetImageContentStreamByNameAsync(assetManager.DefaultImage.FileName);

        //            }

        //        }

        //    }

        //    if (imageContentStream == null)
        //    {

        //        using (var imageStream = new MemoryStream())
        //        {
        //            var image = new Bitmap(width ?? 1, height ?? 1, PixelFormat.Format32bppArgb);
        //            using (var g = Graphics.FromImage(image))
        //            {
        //                g.Clear(Color.Transparent);
        //            }
        //            image.Save(imageStream, System.Drawing.Imaging.ImageFormat.Png);

        //            return new FileContentResult(imageStream.ToArray(), "image/png");
        //        }
               
        //    }

        //    var converter = /*converter ??*/ Instance.Create<RasterImageConversion>();
        //    converter.Width = width;
        //    converter.Height = height;
        //    converter.SuperSampling = sampling;
        //    converter.Clip = width.HasValue == false || height.HasValue == false;
        //    converter.Cover = cover;
        //    converter.EntropyCropTreshold = entropyCrop;

        //    try
        //    {
        //        if (siteContext.SiteDataCacheManager != null)
        //        {
        //            var convertedResult = await converter.GetFileResultAsync(imageContentStream, fileName);

        //            var contentBuffer = await convertedResult.FileResult.GetContentAsBufferAsync();
        //            string contentType = convertedResult.FileResult.ContentType;

        //            HostingEnvironment.QueueBackgroundWorkItem(c =>
        //            {
        //                siteContext.SiteDataCacheManager.CacheThumbnailFile(fileName, contentBuffer, contentType, width ?? -1, height ?? -1, sampling ?? 1);
        //            });

        //            return new FileContentResult(contentBuffer, contentType);
        //        }
        //        else
        //        {
        //            var convertedResult = await converter.GetFileResultAsync(imageContentStream, fileName);
        //            return convertedResult.FileResult;
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        using (var imageStream = new MemoryStream())
        //        {
        //            var image = new Bitmap(width ?? 1, height ?? 1, PixelFormat.Format32bppArgb);
        //            using (var g = Graphics.FromImage(image))
        //            {
        //                g.Clear(Color.Transparent);
        //            }
        //            image.Save(imageStream, System.Drawing.Imaging.ImageFormat.Png);

        //            return new FileContentResult(imageStream.ToArray(), "image/png");
        //        }
        //    }

        //}

        //[OutputCache(Duration = 3600, Location = OutputCacheLocation.Client, VaryByParam = "*")]
        //public ActionResult ProfileContent(string imageName)
        //{
        //    string path = ConfigurationManager.GetManager(HttpContext).GetProfileContentsServerPath(id, HttpContext);
        //    if (System.IO.File.Exists(path) == true)
        //    {
        //        return File(path, Mime.GetMimeMapping(path));
        //    }
        //    return new EmptyResult();
        //}

        // Get an asset image
      
    }

   
}