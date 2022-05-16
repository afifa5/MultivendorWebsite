
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MultivendorWebViewer.Helpers
{
    public class CSVFileResult : FileResult
    {
        public CSVFileResult(IEnumerable<IEnumerable<string>> content = null, string separator = ";")
            : base("text/csv")
        {
            Content = content;
            Separator = separator;
        }

        public IEnumerable<IEnumerable<string>> Content { get; set; }

        public Stream GetStream()
        {
            var stream = new MemoryStream();
            CsvUtility.WriteToStream(Content, stream, Separator);
            return stream;
        }

        protected override void WriteFile(HttpResponseBase response)
        {
            throw new NotImplementedException();
        }

        public string Separator { get; set; }

#if NET452
        protected override void WriteFile(HttpResponseBase response)
        {
            CsvUtility.WriteToStream(Content, response.OutputStream, Separator ?? ";");
        }
#endif

#if NET5
        public override Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var executor = context.HttpContext.RequestServices.GetRequiredService<IActionResultExecutor<CSVFileResult>>();
            return executor.ExecuteAsync(context, this);
        }
#endif
    }

#if NET5
    public class CSVFileResultExecutor : FileResultExecutorBase, IActionResultExecutor<CSVFileResult>
    {
        /// <summary>
        /// Initializes a new <see cref="CSVFileResultExecutor"/>.
        /// </summary>
        /// <param name="loggerFactory">The factory used to create loggers.</param>
        public CSVFileResultExecutor(ILoggerFactory loggerFactory)
            : base(CreateLogger<CSVFileResultExecutor>(loggerFactory))
        {
        }

        /// <inheritdoc />
        public virtual async Task ExecuteAsync(ActionContext context, CSVFileResult result)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            // TODO We read full stream and then we write to output stream. That is NOT a good strategy
            using (var stream = result.GetStream())
            {
                //Logger.ExecutingFileResult(result);

                long? fileLength = null;
                
                
                if (stream.CanSeek)
                {
                    fileLength = stream.Length;
                }

                var (range, rangeLength, serveBody) = SetHeadersAndLog(
                    context,
                    result,
                    fileLength,
                    result.EnableRangeProcessing,
                    result.LastModified,
                    result.EntityTag);

                if (!serveBody)
                {
                    return;
                }

                await WriteFileAsync(context, result, range, rangeLength, stream);
            }
        }

        /// <summary>
        /// Write the contents of the FileStreamResult to the response body.
        /// </summary>
        /// <param name="context">The <see cref="ActionContext"/>.</param>
        /// <param name="result">The FileStreamResult to write.</param>
        /// <param name="range">The <see cref="RangeItemHeaderValue"/>.</param>
        /// <param name="rangeLength">The range length.</param>
        protected virtual Task WriteFileAsync(
            ActionContext context,
            CSVFileResult result,
            RangeItemHeaderValue? range,
            long rangeLength,
            Stream stream)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            if (range != null && rangeLength == 0)
            {
                return Task.CompletedTask;
            }

            //if (range != null)
            //{
            //    Logger.WritingRangeToBody();
            //}

            return WriteFileAsync(context.HttpContext, stream, range, rangeLength);
        }
    }
#endif

    public static class CsvUtility
    {
        public static void WriteToStream(IEnumerable<IEnumerable<string>> content, Stream outputStream, string separator = ";")
        {
            if (separator == "44")
                separator = ",";
            if (separator == "9")
                separator = "\t";
            if (separator == "59" || separator == "-1")
                separator = ";";
            using (var streamWriter = new StreamWriter(outputStream))
            {
                Write(content, streamWriter, separator);
            }
        }


        public static void WriteToStream(IEnumerable<IEnumerable<string>> content, Stream outputStream, System.Text.Encoding encoding, string separator = ";")
        {
            if (separator == "44")
                separator = ",";
            if (separator == "9")
                separator = "\t";
            if (separator == "59" || separator == "-1")
                separator = ";";
            using (var streamWriter = new StreamWriter(outputStream, encoding))
            {
                Write(content, streamWriter, separator);
            }
        }

        public static void WriteToFile(string fileName, IEnumerable<IEnumerable<string>> content, string separator = ";")
        {
            if (separator == "44")
                separator = ",";
            if (separator == "9")
                separator = "\t";
            if (separator == "59" || separator == "-1")
                separator = ";";

            using (var fileWriter = new StreamWriter(fileName, false, System.Text.Encoding.Unicode))
            {
                Write(content, fileWriter, separator);
            }
        }

        public static void Write(IEnumerable<IEnumerable<string>> content, TextWriter writer, string separator = ";")
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();

            var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
            {
                NewLine = CsvHelper.Configuration.NewLine.Environment,
                TrimOptions = CsvHelper.Configuration.TrimOptions.Trim,
                Delimiter = separator
            };

            using (var csv = new CsvWriter(writer, config))
            {
                foreach (var line in content)
                {
                    foreach (var field in line)
                    {
                        string formattedFiels = field != null ? field.Replace(Environment.NewLine, " ") : string.Empty;
                        csv.WriteField(formattedFiels);
                    }
                    csv.NextRecord();
                }
            }

            ////string escapedSeparator = string.Concat("\\", separator);
            //foreach (var line in content)
            //{
            //    var l = string.Join(separator, line.Select(w =>
            //    {
            //        if (string.IsNullOrEmpty(w) == true) return string.Empty;
            //        //string escapedWord = w.Replace("\"", "\"\""); // To preserve double-quotes we need to change them to a pair of double-quotes
            //        //escapedWord = w.Replace(separator, escapedSeparator);
            //        string escapedWord = w;
            //        escapedWord = escapedWord.Replace(System.Environment.NewLine, " ");
            //        if (w.ContainsAnyWhiteSpace() == true || w.IndexOf(separator, StringComparison.OrdinalIgnoreCase) != -1)
            //        {
            //            return string.Concat("\"", escapedWord.Trim(), "\"");
            //        }
            //        else
            //        {
            //            escapedWord = w.Replace("\"", "\"\""); // To preserve double-quotes we need to change them to a pair of double-quotes
            //            return escapedWord.Trim();
            //        }
            //    }));
            //    writer.WriteLine(l);
            //}


        }
    }
}
