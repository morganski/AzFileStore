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

        public Stream GetFile(string filename)
        {
            var blockBlob = _container.GetBlockBlobReference(filename);
            var stream = new MemoryStream();
            blockBlob.DownloadToStream(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        public void StoreFile(string filename, Stream fileContent)
        {
            var blockBlob = _container.GetBlockBlobReference(filename);
            blockBlob.UploadFromStream(fileContent);
        }

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
