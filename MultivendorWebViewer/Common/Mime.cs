using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MultivendorWebViewer.Common
{
    public static class Mime
    {
        public static string GetMimeMapping(string fileName)
        {
            if (String.IsNullOrEmpty(fileName) == true) return "application/octet-stream";

            try
            {
                string extension = Path.GetExtension(fileName).ToLowerInvariant();
                switch (extension)
                {
                    case ".svg": return "image/svg+xml";
                    case ".svgz": return "image/svg+xml";
                    case ".mht": return "application/x-mimearchive";
                    case ".pvz": return "application/x-pvlite9-ed";
                    default: return MimeMapping.GetMimeMapping(fileName);
                }
            }
            catch
            {
                return MimeMapping.GetMimeMapping(fileName);
            }
        }
    }
}
