using FileStorageSpike.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileStorageSpike.Services
{
    public class AuditedFileContainer : ISecureFileContainer
    {
        public AuditedFileContainer(ISecureFileContainer wrappedContainer, IAudit audit)
        {
            _wrappedContainer = wrappedContainer;
            _audit = audit;
        }

        public Uri GetSecureFileUri(string filename)
        {
            _audit.RecordOperation(Thread.CurrentPrincipal, "Request URI for file '{0}'", filename);

            return _wrappedContainer.GetSecureFileUri(filename);
        }

        public Stream GetFile(string filename)
        {
            _audit.RecordOperation(Thread.CurrentPrincipal, "Request for file '{0}'", filename);

            return _wrappedContainer.GetFile(filename);
        }

        public void StoreFile(string filename, Stream fileContent, string contentType)
        {
            _audit.RecordOperation(Thread.CurrentPrincipal, "Store file '{0}' of type '{1}'", filename, contentType);
            _wrappedContainer.StoreFile(filename, fileContent, contentType);
        }

        public IEnumerable<string> GetFilenames()
        {
            var filenames = _wrappedContainer.GetFilenames();

            StringBuilder sb = null;

            foreach (var filename in filenames)
            {
                if (null == sb)
                    sb = new StringBuilder(filename);
                else
                    sb.AppendFormat(", {0}", filename);
            }

            _audit.RecordOperation(Thread.CurrentPrincipal, "Filenames requested - these were returned: '{0}'", sb.ToString());

            return filenames;
        }

        public ISecureFileContainer _wrappedContainer;
        public IAudit _audit;
    }
}
