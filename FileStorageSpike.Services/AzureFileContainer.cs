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
    public class AzureFileContainer : ISecureFileContainer
    {
        public AzureFileContainer(string connectionString, string containerName, TimeSpan fileShareDuration)
        {
            var account = CloudStorageAccount.Parse(connectionString);
            var blobClient = account.CreateCloudBlobClient();
            _container = blobClient.GetContainerReference(containerName);
            _container.CreateIfNotExists();
            _container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Off });
            _fileShareDuration = fileShareDuration;
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
        /// <param name="mimeType">Defines the MIME type of the content</param>
        public void StoreFile(string filename, Stream fileContent, string mimeType)
        {
            var blockBlob = _container.GetBlockBlobReference(filename);
            blockBlob.UploadFromStream(fileContent);
            blockBlob.Properties.ContentType = mimeType;
            blockBlob.SetProperties();
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
                filenames.Add(Path.GetFileName(item.Uri.LocalPath));
            }

            return filenames;
        }

        /// <summary>
        /// Returns a URI that permits read only access to the file for a limited period of time
        /// </summary>
        /// <param name="filename">The filename to access</param>
        /// <returns>A URL for that file</returns>
        public Uri GetSecureFileUri(string filename)
        {
            var blockBlob = _container.GetBlockBlobReference(filename);

            var constraints = new SharedAccessBlobPolicy
            {
                SharedAccessExpiryTime = DateTime.UtcNow.Add(_fileShareDuration),
                Permissions = SharedAccessBlobPermissions.Read
            };

            var token = blockBlob.GetSharedAccessSignature(constraints);

            return new Uri(blockBlob.Uri, token);
        }

        private TimeSpan _fileShareDuration;
        private CloudBlobContainer _container;
    }
}
