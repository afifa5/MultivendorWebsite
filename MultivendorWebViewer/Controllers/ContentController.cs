
using ImageProcessor;
using ImageProcessor.Imaging;
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
    //public class ImageConversionProvider : SingletonBase<ImageConversionProvider>
    //{
    //    public virtual ImageConversion GetConverter(string type, ModelBindingContext bindingContext)
    //    {
    //        switch (type)
    //        {
    //            case "raster": return new RasterImageConversion();
    //            //case "svg": return new SvgImageConversion();
    //            case "scale": return new ScaleImageConversion();
    //            //case "colorize": return new ColorizeImageConversion();
    //        }
    //        return null;
    //    }
    //}
    public class ImageConversionResult
    {
        public FileResult FileResult { get; set; }

        public string SourceFileName { get; set; }

        public Exception Exception { get; set; }
    }

    [ModelBinder(typeof(ImageConversion.ImageConversionModelBinder))]
    public abstract class ImageConversion
    {
        public double? Width { get; set; }

        public double? Height { get; set; }

        public int? SuperSampling { get; set; }

        public bool Clip { get; set; }

        public bool Cover { get; set; }

        public int EntropyCropTreshold { get; set; }

        //public double? MaxWidth { get; set; }

        //public double? MaxHeight { get; set; }

        public virtual void Initialize(ModelBindingContext bindingContext)
        {
            Width = bindingContext.GetValue<double?>("width");
            Height = bindingContext.GetValue<double?>("height");
            SuperSampling = bindingContext.GetValue<int?>("sampling");
            Clip = bindingContext.GetValue<bool?>("clip") ?? false;
            Cover = bindingContext.GetValue<bool?>("cover") ?? false;
            EntropyCropTreshold = bindingContext.GetValue<int?>("entropyCrop") ?? 0;
            //MaxWidth = bindingContext.GetValue<double?>("maxWidth");
            //MaxHeight = bindingContext.GetValue<double?>("maxHeight");
        }

        public abstract Task<ImageConversionResult> GetFileResultAsync(Stream stream, string fileName);

        public class ImageConversionModelBinder : IModelBinder
        {
            public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
            {
                string type = bindingContext.GetValue<string>("converter");

                if (type == null)
                {
                    return null;
                }

                var instance = new ScaleImageConversion(); //ImageConversionProvider.Default.GetConverter(type, bindingContext);
                if (instance != null)
                {
                    instance.Initialize(bindingContext);
                }
                return instance;
            }
        }
    }

    public class RasterImageConversion : ImageConversion
    {
        private ModelBindingContext bindingContext;

        public RasterImageConversion()
        {
        }

        public override Task<ImageConversionResult> GetFileResultAsync(Stream stream, string fileName)
        {
            ImageConversion conversion = new ScaleImageConversion();
            //string extension = Path.GetExtension(fileName).ToLower();
            //if (extension == ".svg" || extension == ".svgz")
            //{
            //    //conversion = new SvgImageConversion();
            //}
            //else
            //{
            //conversion = new ScaleImageConversion();
            //}

            if (bindingContext != null)
            {
                conversion.Initialize(bindingContext);
            }
            else
            {
                conversion.Width = Width;
                conversion.Height = Height;
                conversion.SuperSampling = SuperSampling;
                conversion.Clip = Clip;
                conversion.Cover = Cover;
            }

            return conversion.GetFileResultAsync(stream, fileName);
        }

        public override void Initialize(ModelBindingContext bindingContext)
        {
            this.bindingContext = bindingContext;
        }
    }

    public class ScaleImageConversion : ImageConversion
    {
        public virtual InterpolationMode Interpolation { get { return InterpolationMode.HighQualityBicubic; } }

        public override Task<ImageConversionResult> GetFileResultAsync(Stream stream, string fileName)
        {
            return Task.Run(() =>
            {
                var timer = System.Diagnostics.Stopwatch.StartNew();

                double? width = Width;
                double? height = Height;

                if (width.HasValue == true || height.HasValue == true)
                {
                    //return Scale(stream, fileName);
                    try
                    {
                        int? integerWidth = width.HasValue == true ? (int?)Math.Ceiling(width.Value) : null;
                        int? integerHeight = height.HasValue == true ? (int?)Math.Ceiling(height.Value) : null;
                        return Scale(stream, fileName, integerWidth, integerHeight, Cover, EntropyCropTreshold);
                        
                    }
                    catch (Exception)
                    {
                        //throw;
                        return new ImageConversionResult { FileResult = new FileStreamResult(stream, Mime.GetMimeMapping(fileName)), SourceFileName = fileName };
                    }
                }

                return new ImageConversionResult { FileResult = new FileStreamResult(stream, Mime.GetMimeMapping(fileName)), SourceFileName = fileName };
            });
        }

        //public class EnsureAlphaChannel : IGraphicsProcessor
        //{
        //    public dynamic DynamicParameter { get; set; }

        //    public Dictionary<string, string> Settings { get; set; }

        //    public Image ProcessImage(ImageFactory factory)
        //    {
        //        if (factory.Image.PixelFormat.HasFlag(PixelFormat.Alpha) == false && factory.Image.PixelFormat.HasFlag(PixelFormat.PAlpha) == false)
        //        {
        //            var bitmap = factory.Image as Bitmap ?? new Bitmap(factory.Image);
        //            return bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), bitmap.PixelFormat | PixelFormat.Alpha);
        //        }
        //        return factory.Image;
        //    }
        //}

        public Rectangle GetEntropy(Image image, byte threshold = 128)
        {
            using (var grey = new ImageProcessor.Imaging.Filters.EdgeDetection.ConvolutionFilter(new ImageProcessor.Imaging.Filters.EdgeDetection.SobelEdgeFilter(), true).Process2DFilter(image))
            using (var processedGrey = new ImageProcessor.Imaging.Filters.Binarization.BinaryThreshold(threshold).ProcessFilter(grey))
            {
                return ImageProcessor.Imaging.Helpers.ImageMaths.GetFilteredBoundingRectangle(processedGrey, 0);
            }
        }

        //public Bitmap EntropyCrop(Image image, byte threshold = 128)
        //{
        //    Bitmap newImage = null;
        //    Bitmap grey = null;

        //    try
        //    {
        //        // Detect the edges then strip out middle shades.
        //        grey = new ImageProcessor.Imaging.Filters.EdgeDetection.ConvolutionFilter(new ImageProcessor.Imaging.Filters.EdgeDetection.SobelEdgeFilter(), true).Process2DFilter(image);
        //        grey = new ImageProcessor.Imaging.Filters.Binarization.BinaryThreshold(threshold).ProcessFilter(grey);

        //        // Search for the first white pixels
        //        Rectangle rectangle = ImageProcessor.Imaging.Helpers.ImageMaths.GetFilteredBoundingRectangle(grey, 0);
        //        grey.Dispose();

        //        newImage = new Bitmap(rectangle.Width, rectangle.Height, PixelFormat.Format32bppPArgb);
        //        newImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);
        //        using (var graphics = Graphics.FromImage(newImage))
        //        {
        //            graphics.DrawImage(
        //                image,
        //                new Rectangle(0, 0, rectangle.Width, rectangle.Height),
        //                rectangle.X,
        //                rectangle.Y,
        //                rectangle.Width,
        //                rectangle.Height,
        //                GraphicsUnit.Pixel);
        //        }

        //        // Reassign the image.
        //        image.Dispose();
        //        image = newImage;
        //    }
        //    catch (Exception)
        //    {
        //        if (grey != null) grey.Dispose();

        //        if (newImage != null) newImage.Dispose();
        //    }

        //    return (Bitmap)image;
        //}

        public ImageConversionResult Scale(Stream stream, string fileName)
        {
            var imageFactory = new ImageFactory();
            imageFactory.Load(stream);

            double? width = Width;
            double? height = Height;
            // Need cropping
            if ((width.HasValue == true && imageFactory.Image.Width != width.Value) || (height.HasValue == true && imageFactory.Image.Height != height.Value))
            {
                var targetSize = new Size(width.HasValue == true ? (int)Math.Ceiling(width.Value) : 0, height.HasValue == true ? (int)Math.Ceiling(height.Value) : 0);

                //imageFactory.EntropyCrop();
                //imageFactory.BitDepth(32);
                //imageFactory.BackgroundColor(Color.Red);
                //imageFactory.Resize(new ResizeLayer(targetSize, Cover == true ? ResizeMode.Crop : ResizeMode.Pad));
                //imageFactory.GaussianBlur(20);
                //imageFactory.BitDepth(32);

                var memoryStream = new MemoryStream();
                {
                    imageFactory.Save(memoryStream);

                    return new ImageConversionResult { FileResult = new FileStreamResult(memoryStream, Mime.GetMimeMapping(fileName)), SourceFileName = fileName };
                }
            }
            else
            {
                return new ImageConversionResult { FileResult = new FileStreamResult(stream, Mime.GetMimeMapping(fileName)), SourceFileName = fileName };
            }
        }

        public override void Initialize(ModelBindingContext bindingContext)
        {
            base.Initialize(bindingContext);

            Clip = bindingContext.GetValue<bool>("clip");
            Cover = bindingContext.GetValue<bool>("cover");
            EntropyCropTreshold = bindingContext.GetValue<int?>("entropyCrop") ?? 0;
        }

        public ImageConversionResult Scale(Stream imageStream, string fileName, int? width, int? height, bool cover = false, int? entropyCropTreshold = null, Color? backgroundColor = null, bool repeatBorders = false, InterpolationMode interpolationMode = InterpolationMode.HighQualityBicubic)
        {
            if (cover == false)
            {
                using (var image = System.Drawing.Image.FromStream(imageStream))
                {
                    if (width.HasValue == false)
                    {
                        width = (height.Value * image.Width) / image.Height;
                    }
                    else if (height.HasValue == false)
                    {
                        height = (width.Value * image.Height) / image.Width;
                    }

                    // The same width and height of the image, just return
                    if (width.Value == image.Width && height.Value == image.Height)
                    {
                        if (imageStream.CanSeek == true) imageStream.Seek(0, SeekOrigin.Begin);
                        return new ImageConversionResult { FileResult = new FileStreamResult(imageStream, Mime.GetMimeMapping(fileName)), SourceFileName = fileName };
                    }

                    double factorWidth = (double)width / (double)image.Width;
                    double factorHeight = (double)height / (double)image.Height;
                    double aspectRation = (double)image.Width / (double)image.Height;

                    Rectangle entropyRegion;
                    if (entropyCropTreshold.HasValue == false) entropyCropTreshold = EntropyCropTreshold;
                    if (entropyCropTreshold.HasValue == true)
                    {
                        int timesBigger = Math.Min(image.Width / width.Value, image.Height / height.Value);
                        if (timesBigger > 1) // If image is huge, scale it down before running entropy
                        {
                            using (var entropyImage = new Bitmap(image, new Size(image.Width / timesBigger, image.Height / timesBigger)))
                            {
                                entropyRegion = GetEntropy(entropyImage, threshold: 10);
                                entropyRegion = new Rectangle(entropyRegion.Left * timesBigger, entropyRegion.Top * timesBigger, entropyRegion.Width * timesBigger, entropyRegion.Height * timesBigger);
                            }
                        }
                        else
                        {
                            entropyRegion = GetEntropy(image, threshold: 10);
                        }
                    }
                    else
                    {
                        entropyRegion = new Rectangle(0, 0, image.Width, image.Height);
                    }

                    Size imageDestinationSize;
                    RectangleF imageSourceBounds = new RectangleF(0, 0, image.Width, image.Height);
                    if (factorWidth > factorHeight) // To small width
                    {
                        //double entropyFactorHeight = (double)entropyRegion.Height / (double)image.Height;
                        int missingWidth = image.Width - (int)Math.Floor(image.Width * factorHeight / factorWidth);
                        double neededExtraHeight = missingWidth / aspectRation;
                        double availableExtraTop = entropyRegion.Top;
                        double availableExtraBottom = image.Height - entropyRegion.Bottom;
                        if (availableExtraTop > 0 || availableExtraBottom > 0)
                        {
                            double takenExtraTop = Math.Min(neededExtraHeight * 0.5, availableExtraTop);
                            neededExtraHeight -= takenExtraTop;
                            double takenExtraBottom = Math.Min(neededExtraHeight, availableExtraBottom);
                            neededExtraHeight -= takenExtraBottom;
                            if (neededExtraHeight > 0)
                            {
                                availableExtraTop -= takenExtraTop;
                                availableExtraBottom -= takenExtraBottom;
                                if (availableExtraTop > 0)
                                {
                                    takenExtraTop += Math.Min(availableExtraTop, neededExtraHeight);

                                }
                                else if (availableExtraBottom > 0)
                                {
                                    takenExtraBottom += Math.Min(availableExtraBottom, neededExtraHeight);
                                }
                                else
                                {
                                    // Repeat borders if suitable
                                }
                            }

                            float takenExtraV = (float)(takenExtraTop + takenExtraBottom);

                            imageSourceBounds = new RectangleF(0, (float)takenExtraTop, image.Width, image.Height - takenExtraV);
                            imageDestinationSize = new Size((int)Math.Floor(height.Value * imageSourceBounds.Width / imageSourceBounds.Height), height.Value);
                        }
                        else
                        {
                            imageDestinationSize = new Size((int)Math.Floor(image.Width * factorHeight), height.Value);
                        }
                    }
                    else // To small height
                    {
                        //double entropyFactorHeight = (double)entropyRegion.Height / (double)image.Height;

                        int missingHeight = image.Height - (int)Math.Floor(image.Height * factorWidth / factorHeight);
                        double neededExtraWidth = missingHeight * aspectRation;
                        double availableExtraLeft = entropyRegion.Left;
                        double availableExtraRight = image.Width - entropyRegion.Right;
                        if (availableExtraLeft > 0 || availableExtraRight > 0)
                        {
                            double takenExtraLeft = Math.Min(neededExtraWidth * 0.5, availableExtraLeft);
                            neededExtraWidth -= takenExtraLeft;
                            double takenExtraRight = Math.Min(neededExtraWidth, availableExtraRight);
                            neededExtraWidth -= availableExtraRight;
                            if (neededExtraWidth > 0)
                            {
                                availableExtraLeft -= takenExtraLeft;
                                availableExtraRight -= takenExtraRight;
                                if (availableExtraLeft > 0)
                                {
                                    takenExtraLeft += Math.Min(availableExtraLeft, neededExtraWidth);

                                }
                                else if (availableExtraRight > 0)
                                {
                                    takenExtraRight += Math.Min(availableExtraRight, neededExtraWidth);
                                }
                                else
                                {
                                    // Repeat borders if suitable
                                }
                            }

                            float takenExtraH = (float)(takenExtraLeft + takenExtraRight);

                            imageSourceBounds = new RectangleF((float)takenExtraLeft, 0, image.Width - takenExtraH, image.Height);
                            imageDestinationSize = new Size(width.Value, (int)Math.Floor(width.Value * imageSourceBounds.Height / imageSourceBounds.Width));
                        }
                        else
                        {
                            imageDestinationSize = new Size(width.Value, (int)Math.Floor(image.Height * factorWidth));
                        }
                    }

                    int offsetX = (width.Value - imageDestinationSize.Width) / 2;
                    int offsetY = (height.Value - imageDestinationSize.Height) / 2;

                    var thumbBitmap = new Bitmap(width.Value, height.Value, PixelFormat.Format32bppArgb);

                    var paddingRectangle = Rectangle.Empty;

                    using (var graphics = Graphics.FromImage(thumbBitmap))
                    {
                        graphics.InterpolationMode = interpolationMode;

                        if (backgroundColor.HasValue == true && backgroundColor.Value.IsEmpty == false)
                        {
                            graphics.Clear(backgroundColor.Value);
                        }

                        graphics.DrawImage(image, new Rectangle(offsetX, offsetY, imageDestinationSize.Width, imageDestinationSize.Height), imageSourceBounds, GraphicsUnit.Pixel);
                    }

                    var outStream = new MemoryStream();
                    thumbBitmap.Save(outStream, ImageFormat.Png); // Change to png to allow transparency
                    outStream.Seek(0, SeekOrigin.Begin);
                    return new ImageConversionResult { FileResult = new FileStreamResult(outStream, "image/png"), SourceFileName = fileName };
                }
            }
            else // TODO, solve this our self. Do not trust performance
            {
                var imageFactory = new ImageFactory();
                imageFactory.Load(imageStream);

                // The same width and height of the image, just resize
                if (width.Value == imageFactory.Image.Width && height.Value == imageFactory.Image.Height)
                {
                    if (imageStream.CanSeek == true) imageStream.Seek(0, SeekOrigin.Begin);
                    return new ImageConversionResult { FileResult = new FileStreamResult(imageStream, Mime.GetMimeMapping(fileName)), SourceFileName = fileName };
                }

                imageFactory.Resize(new ResizeLayer(new Size(width ?? 0, height ?? 0), resizeMode: ResizeMode.Crop, upscale: true));
                //imageFactory.BackgroundColor(backgroundColor ?? Color.Transparent);
                var outStream = new MemoryStream();
                outStream.Seek(0, SeekOrigin.Begin);
                imageFactory.Save(outStream);

                return new ImageConversionResult { FileResult = new FileStreamResult(outStream, Mime.GetMimeMapping(fileName)), SourceFileName = fileName };
            }
        }

        //public Bitmap Scale(Image image, Size newSize, bool maintainAspectRatio = true, bool fixedSize = true, bool cover = true, InterpolationMode interpolationMode = InterpolationMode.Default, Color? backgroundColor = null, bool repeatBorders = false)
        //{
        //    //return EntropyCrop(image, 40);

        //    int width = newSize.Width;
        //    int height = newSize.Height;
        //    if (maintainAspectRatio == true)
        //    {
        //        if (width > 0 && height > 0)
        //        {
        //            float factorHeight = ((float)height) / ((float)image.Height);
        //            float factorWidth = ((float)width) / ((float)image.Width);
        //            if (factorHeight != factorWidth)
        //            {
        //                if ((cover == true ? factorHeight < factorWidth : factorHeight > factorWidth))
        //                {
        //                    height = (int)(image.Height * factorWidth);
        //                }
        //                else
        //                {
        //                    width = (int)(image.Width * factorHeight);
        //                }
        //            }
        //        }
        //        else if (width <= 0)
        //        {
        //            width = (int)(height * ((float)image.Width / (float)image.Height));
        //            fixedSize = false;
        //        }
        //        else
        //        {
        //            height = (int)(width * ((float)image.Height / (float)image.Width));
        //            fixedSize = false;
        //        }
        //    }
        //    else
        //    {
        //        if (width == 0) width = height;
        //        if (height == 0) height = width;
        //    }

        //    var bitmap = (fixedSize == true) ? new Bitmap(image, newSize.Width, newSize.Height) : new Bitmap(image, width, height);

        //    Rectangle paddingRectangle = Rectangle.Empty;

        //    using (var graphics = Graphics.FromImage(bitmap))
        //    {
        //        if (backgroundColor.HasValue == true && backgroundColor.Value.IsEmpty == false)
        //        {
        //            graphics.Clear(backgroundColor.Value);
        //        }

        //        graphics.InterpolationMode = interpolationMode;

        //        if (interpolationMode != InterpolationMode.NearestNeighbor && interpolationMode != InterpolationMode.Low)
        //        {
        //            using (var wrapMode = new ImageAttributes())
        //            {
        //                //wrapMode.SetWrapMode(WrapMode.TileFlipXY);

        //                if (fixedSize == true)
        //                {
        //                    // Contain
        //                    int destX = (bitmap.Width - width) / 2;
        //                    int destY = (bitmap.Height - height) / 2;
        //                    graphics.DrawImage(image, new Rectangle(destX, destY, width, height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);

        //                    // Needs cropping
        //                    if (destX > 0 || destY > 0)
        //                    {
        //                        paddingRectangle = new Rectangle(destX, destY, width, height);

        //                    }
        //                    // Cover
        //                    //int x = 0, y = 0, w = image.Width, h = image.Height;
        //                    //if (bitmap.Width > width)
        //                    //{
        //                    //    // Restaurang
        //                    //    float zoomFactor = (float)bitmap.Width / (float)width;
        //                    //    h = (int)Math.Ceiling(h / zoomFactor);
        //                    //    y = (image.Height - h) / 2;
        //                    //}
        //                    //else if (bitmap.Height > height)
        //                    //{
        //                    //    float zoomFactor = (float)bitmap.Height / (float)height;
        //                    //    w = (int)Math.Ceiling(w / zoomFactor);
        //                    //    x = (image.Width - w) / 2;
        //                    //}
        //                    //graphics.DrawImage(image, new Rectangle(0, 0, width, height), x, y, w, h, GraphicsUnit.Pixel, wrapMode);
        //                }
        //                else // Scaled
        //                {
        //                    graphics.DrawImage(image, new Rectangle(0, 0, width, height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (fixedSize == true)
        //            {
        //                graphics.DrawImage(image, (bitmap.Width - width) / 2, (bitmap.Height - height) / 2, width, height);
        //            }
        //            else
        //            {
        //                graphics.DrawImage(image, 0, 0, width, height);
        //            }
        //        }
        //    }


        //    if (paddingRectangle.IsEmpty == false && repeatBorders == false)
        //    {
        //        var timer = Stopwatch.StartNew();

        //        var lockedBitmap = new FastBitmap(bitmap);

        //        Size paddingBitmapSize = Size.Empty;
        //        Rectangle paddingSourceBounds = Rectangle.Empty;
        //        int xPadding = 0;
        //        int xOffset = 0;
        //        int yPadding = 0;
        //        int yOffset = 0;
        //        bool h = true;
        //        int paddingRepeatSize = 0;

        //        Action draw = () =>
        //        {
        //            //using (var paddingBitmap = new Bitmap(paddingBitmapSize.Width, paddingBitmapSize.Height, bitmap.PixelFormat))
        //            //using (var paddingBitmap = bitmap.Clone(paddingSourceBounds, bitmap.PixelFormat))
        //            {

        //                //var imageFactory = new ImageFactory();
        //                //imageFactory.Load(bitmap.Clone(paddingSourceBounds, bitmap.PixelFormat));
        //                //imageFactory.Resize(paddingBitmapSize);
        //                //var paddingBitmap = imageFactory.Image;

        //                var paddingBitmap = Copy(lockedBitmap, paddingSourceBounds, bitmap.PixelFormat);


        //                //using (var graphics = Graphics.FromImage(paddingBitmap))
        //                //{
        //                //    graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
        //                //    graphics.DrawImage(bitmap.Clone(paddingSourceBounds, bitmap.PixelFormat), new Rectangle(Point.Empty, paddingBitmapSize));
        //                //    //graphics.DrawImageUnscaled(image, new Rectangle(Point.Empty, paddingBitmapSize), paddingSourceBounds, GraphicsUnit.Pixel);
        //                //    //graphics.DrawImage(bitmap, new Rectangle(Point.Empty, paddingBitmapSize), paddingSourceBounds, GraphicsUnit.Pixel);
        //                //}

        //                var lockedPaddingBitmap = paddingBitmap;

        //                //using (var paddingBitmapClone = (Bitmap)paddingBitmap.Clone())
        //                //using (var blurredPaddingBitmap = Blur(/* paddingBitmapClone */paddingBitmap, 30)) 
        //                //using (var lockedPaddingBitmap = new FastBitmap(paddingBitmap/*blurredPaddingBitmap*/))
        //                //using (var lockedBlurredPaddingBitmap = new FastBitmap(blurredPaddingBitmap))
        //                {
        //                    for (int y = 0; y < yPadding; y++)
        //                    {
        //                        for (int x = 0; x < xPadding; x++)
        //                        {
        //                            lockedBitmap.SetPixel(h == true ? xOffset - x : x, h == true ? y : yOffset - y, lockedPaddingBitmap.GetPixel(x, y));

        //                            //float unblurredFraction;
        //                            //float blurredFraction;

        //                            //if (pos == false)
        //                            //{
        //                            //    unblurredFraction = h == true ? Math.Max(0, (x / (float)xPadding) * 1.0f) : Math.Max(0, (y / (float)yPadding) * 1.0f);
        //                            //    blurredFraction = 1f - unblurredFraction;
        //                            //}
        //                            //else
        //                            //{
        //                            //    blurredFraction = h == true ? Math.Max(0, (x / (float)xPadding) * 1.0f) : Math.Max(0, (y / (float)yPadding) * 1.0f);
        //                            //    unblurredFraction = 1f - blurredFraction;
        //                            //}

        //                            //var blurredPixel = lockedBlurredPaddingBitmap.GetPixel(x, y);
        //                            //if (blurredFraction < 1)
        //                            //{
        //                            //    var pixel = lockedPaddingBitmap.GetPixel(x, y);

        //                            //    int A = Math.Min(255, (int)(blurredPixel.A * blurredFraction + pixel.A * unblurredFraction));
        //                            //    int R = Math.Min(255, (int)(blurredPixel.R * blurredFraction + pixel.R * unblurredFraction));
        //                            //    int G = Math.Min(255, (int)(blurredPixel.G * blurredFraction + pixel.G * unblurredFraction));
        //                            //    int B = Math.Min(255, (int)(blurredPixel.B * blurredFraction + pixel.B * unblurredFraction));
        //                            //    lockedBitmap.SetPixel(h == true ? xOffset - x : x, h == true ? y : yOffset - y, Color.FromArgb(A, R, G, B));
        //                            //}
        //                            //else
        //                            //{
        //                            //    lockedBitmap.SetPixel(h == true ? xOffset - x : x, h == true ? y : yOffset - y, blurredPixel);
        //                            //}
        //                        }
        //                    }
        //                }
        //            }
        //        };

        //        if (paddingRectangle.X > 0)
        //        {
        //            paddingBitmapSize = new Size(paddingRectangle.X, bitmap.Height);
        //            paddingRepeatSize = Math.Min(Math.Min(8, paddingBitmapSize.Width), bitmap.Width - paddingBitmapSize.Width);
        //            paddingSourceBounds = new Rectangle(paddingRectangle.X, 0, paddingRepeatSize, bitmap.Height);
        //            xPadding = paddingBitmapSize.Width;
        //            xOffset = paddingBitmapSize.Width - 1;
        //            yPadding = paddingBitmapSize.Height;
        //            yOffset = paddingBitmapSize.Height - 1;

        //            draw();

        //            paddingBitmapSize = new Size(bitmap.Width - paddingRectangle.Right, bitmap.Height);
        //            paddingRepeatSize = Math.Min(Math.Min(8, paddingBitmapSize.Width), bitmap.Width - paddingBitmapSize.Width);
        //            paddingSourceBounds = new Rectangle(bitmap.Width - (paddingBitmapSize.Width + paddingRepeatSize), 0, paddingRepeatSize, bitmap.Height);
        //            xPadding = paddingBitmapSize.Width;
        //            xOffset = bitmap.Width - 1;

        //            draw();
        //        }
        //        else if (paddingRectangle.Y > 0)
        //        {
        //            h = false;

        //            paddingBitmapSize = new Size(bitmap.Width, paddingRectangle.Y);
        //            paddingRepeatSize = Math.Min(Math.Min(8, paddingBitmapSize.Height), bitmap.Height - paddingBitmapSize.Height);
        //            paddingSourceBounds = new Rectangle(0, paddingBitmapSize.Height, bitmap.Width, paddingRepeatSize);
        //            xPadding = paddingBitmapSize.Width;
        //            xOffset = paddingBitmapSize.Width - 1;
        //            yPadding = paddingBitmapSize.Height;
        //            yOffset = paddingBitmapSize.Height - 1;

        //            draw();

        //            paddingBitmapSize = new Size(bitmap.Width, bitmap.Height - paddingRectangle.Bottom);
        //            paddingRepeatSize = Math.Min(Math.Min(8, paddingBitmapSize.Height), bitmap.Height - paddingBitmapSize.Height);
        //            paddingSourceBounds = new Rectangle(0, bitmap.Height - (paddingBitmapSize.Height + paddingRepeatSize), bitmap.Width, paddingRepeatSize);
        //            yPadding = paddingBitmapSize.Height;
        //            yOffset = bitmap.Height - 1;

        //            draw();
        //        }

        //        bitmap = lockedBitmap;

        //        long time = timer.GetElapsedMilliseconds();
        //        Log.DebugWriteLine(TraceEventType.Verbose, string.Format("Time to repeat bounds: {0} ms", time));
        //    }

        //    //bitmap = Blur(bitmap, 20);

        //    return bitmap;
        //}

        //public FastBitmap Copy(FastBitmap bitmap, Rectangle region, PixelFormat pixelFormat)
        //{
        //    var regionBitmap = new Bitmap(region.Width, region.Height, pixelFormat);
        //    var lockedRegionBitmap = new FastBitmap(regionBitmap);
        //    for (int y = 0; y < region.Height; y++)
        //    {
        //        for (int x = 0; x < region.Width; x++)
        //        {
        //            lockedRegionBitmap.SetPixel(x, y, bitmap.GetPixel(region.X + x, region.Y + y));
        //        }
        //    }
        //    return lockedRegionBitmap;
        //}

        //public Bitmap Blur(Bitmap image, int size)
        //{
        //    GaussianLayer gaussianLayer = new GaussianLayer(size);

        //    var convolution = new Convolution(gaussianLayer.Sigma) { Threshold = gaussianLayer.Threshold };

        //    double[,] kernel = convolution.CreateGuassianBlurFilter(gaussianLayer.Size);

        //    return convolution.ProcessKernel(image, kernel, false);
        //}

    }


    [SessionState(SessionStateBehavior.Disabled)]
    public class ContentController : BaseController
    {
        [OutputCache(Duration = 3600, Location = OutputCacheLocation.Client)]
        public virtual async Task<FileResult> Image(int imageId, string fileName = null)
        {
            Response.Cache.SetOmitVaryStar(true);

            try
            {
                if (fileName == null)
                {
                    fileName = ApplicationRequestContext.ImageManager.GetImagesById(imageId).ImageName;
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

        [OutputCache(Duration = 3600, Location = OutputCacheLocation.Client)]
        public virtual async Task<FileResult> Video(int videoId, string fileName = null)
        {
            Response.Cache.SetOmitVaryStar(true);

            try
            {
                if (fileName == null)
                {
                    fileName = ApplicationRequestContext.ImageManager.GetVideoById(videoId).VideoName;
                }

                Stream contentStream = null;

                if (!string.IsNullOrEmpty(fileName))
                {
                    contentStream = await ApplicationRequestContext.ImageManager.GetVideContentStreamByNameAsync(fileName, ApplicationRequestContext);
                }

                if (contentStream == null)
                {
                    return null;
                }

                //var convertedResult = await new ScaleImageConversion().GetFileResultAsync(contentStream, fileName);
                return new FileStreamResult(contentStream, Mime.GetMimeMapping(fileName));
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        [OutputCache(Duration = 3600 * 4, Location = OutputCacheLocation.Any, VaryByParam = "*")]
        public virtual async Task<FileResult> ImageThumbnail(int imageId, int? width, int? height, int? sampling = null, string fileName = null, bool cover = true, int entropyCrop = 0/*, ImageConversion converter = null*/, string source = null)
        {
            Response.Cache.SetOmitVaryStar(true);

            var applicationRequestContext = ApplicationRequestContext.GetContext(HttpContext);

            if (fileName == null)
            {
                fileName = ApplicationRequestContext.ImageManager.GetImagesById(imageId).ImageName;
            }


            /*id missing*/
     

            var imageContentStream = await ApplicationRequestContext.ImageManager.GetImageContentStreamByNameAsync(fileName, ApplicationRequestContext);


            if (imageContentStream == null)
            {

                using (var imageStream = new MemoryStream())
                {
                    var image = new Bitmap(width ?? 1, height ?? 1, PixelFormat.Format32bppArgb);
                    using (var g = Graphics.FromImage(image))
                    {
                        g.Clear(Color.Transparent);
                    }
                    image.Save(imageStream, System.Drawing.Imaging.ImageFormat.Png);

                    return new FileContentResult(imageStream.ToArray(), "image/png");
                }

            }

            var converter = /*converter ??*/ new RasterImageConversion();
            converter.Width = width;
            converter.Height = height;
            converter.SuperSampling = sampling;
            converter.Clip = width.HasValue == false || height.HasValue == false;
            converter.Cover = cover;
            converter.EntropyCropTreshold = entropyCrop;

            try
            {
                //Create cache for thumbnail image
                //if (siteContext.SiteDataCacheManager != null)
                //{
                //    var convertedResult = await converter.GetFileResultAsync(imageContentStream, fileName);

                //    var contentBuffer = await convertedResult.FileResult.GetContentAsBufferAsync();
                //    string contentType = convertedResult.FileResult.ContentType;

                //    HostingEnvironment.QueueBackgroundWorkItem(c =>
                //    {
                //        siteContext.SiteDataCacheManager.CacheThumbnailFile(fileName, contentBuffer, contentType, width ?? -1, height ?? -1, sampling ?? 1);
                //    });

                //    return new FileContentResult(contentBuffer, contentType);
                //}
                //else
                //{
                    var convertedResult = await converter.GetFileResultAsync(imageContentStream, fileName);
                    return convertedResult.FileResult;
                //}
            }
            catch (Exception exception)
            {
                using (var imageStream = new MemoryStream())
                {
                    var image = new Bitmap(width ?? 1, height ?? 1, PixelFormat.Format32bppArgb);
                    using (var g = Graphics.FromImage(image))
                    {
                        g.Clear(Color.Transparent);
                    }
                    image.Save(imageStream, System.Drawing.Imaging.ImageFormat.Png);

                    return new FileContentResult(imageStream.ToArray(), "image/png");
                }
            }

        }


    }


}