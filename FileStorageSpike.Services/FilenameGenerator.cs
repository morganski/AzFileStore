using FileStorageSpike.Interfaces;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorageSpike.Services
{
    public class FilenameGenerator : IFilenameGenerator
    {
        public FilenameGenerator()
        {
            _mimeMap = new Dictionary<string, string>();
        }

        public string GenerateFilename(string mimeType, string contentType)
        {
            string extension;

            if (!_mimeMap.TryGetValue(mimeType, out extension))
            {
                extension = this.LookupMapping(mimeType);

                _mimeMap.Add(mimeType, extension);
            }

            // Now generate the filename
            return string.Concat(contentType, extension);
        }

        private string LookupMapping(string mimeType)
        {
            string key = string.Format("HKEY_CLASSES_ROOT\\MIME\\Database\\Content Type\\{0}", mimeType);

            var ext = Registry.GetValue(key, "Extension", null);

            if (null == ext)
                throw new Exception("Unknown MIME type");

            return ext.ToString();
        }

        private Dictionary<string, string> _mimeMap;
    }
}
