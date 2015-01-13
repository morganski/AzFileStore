using FileStorageSpike.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorageSpike.Services
{
    public class AzureFileStore : IFileStore
    {
        public AzureFileStore(string connectionString, string containerName)
        {
            var account = CloudStorageAccount.Parse(connectionString);
            var blobClient = account.CreateCloudBlobClient();
            _container = blobClient.GetContainerReference(containerName);
            _container.CreateIfNotExists();
        }

        /// <summary>
        /// Get a file given its filename
        /// </summary>
        /// <param name="filename">The name of the file</param>
        /// <returns>A stream containing the file</returns>
        public Stream GetFile(string filename)
        {
            var blockBlob = _container.GetBlockBlobReference(filename);
            var stream = new MemoryStream();
            blockBlob.DownloadToStream(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        /// <summary>
        /// Store a file
        /// </summary>
        /// <param name="filename">The name of the file</param>
        /// <param name="fileContent">The content of that file</param>
        public void StoreFile(string filename, Stream fileContent)
        {
            var blockBlob = _container.GetBlockBlobReference(filename);
            blockBlob.UploadFromStream(fileContent);
        }

        /// <summary>
        /// Return a collection of filenames from this file store
        /// </summary>
        /// <returns>A collection of all filenames</returns>
        public IEnumerable<string> GetFilenames()
        {
            List<string> filenames = new List<string>();

            foreach (var item in _container.ListBlobs().OfType<CloudBlockBlob>())
            {
                if (item.Uri.IsFile)
                    filenames.Add(Path.GetFileName(item.Uri.AbsolutePath));
            }

            return filenames;
        }

        private CloudBlobContainer _container;
    }
}
