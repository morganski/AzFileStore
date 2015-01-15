using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorageSpike.Interfaces
{
    public interface IFilenameGenerator
    {
        string GenerateFilename(string mimeType, string contentType);
    }
}
