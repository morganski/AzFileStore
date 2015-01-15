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
    /// <summary>
    /// This class is used to store documents and retrieve information about those documents
    /// </summary>
    public class DocumentContainer : IDocumentContainer
    {
        /// <summary>
        /// Create the container
        /// </summary>
        /// <param name="containerName">The name of the container (each container has a unique name and zero or more documents)</param>
        /// <param name="resolveFileContainer">Function that resolves a file container</param>
        /// <param name="filenameGenerator">Interface used to generate a filename based on data provided</param>
        /// <param name="documentAuditor">Service which records events when documents are accessed</param>
        public DocumentContainer(string containerName, Func<string, ISecureFileContainer> resolveFileContainer, IFilenameGenerator filenameGenerator, IDocumentAuditor documentAuditor)
        {
            _fileContainer = resolveFileContainer(containerName);
            _filenameGenerator = filenameGenerator;
            _documentAuditor = documentAuditor;
        }

        public string StoreDocument(Stream fileContent, string mimeType, string contentType)
        {
            // Generate the filename for the content
            var filename = _filenameGenerator.GenerateFilename(mimeType, contentType);

            // Pop the file into storage
            _fileContainer.StoreFile(filename, fileContent, mimeType);

            // And record the fact that we've stored the file etc
            _documentAuditor.RecordEvent(Thread.CurrentPrincipal.Identity, EventType.DocumentStored, "Stored document with generated filename of '{0}' of type '{1}' with mime type '{2}'", filename, contentType, mimeType);

            return filename;
        }

        public IEnumerable<string> GetDocumentNames()
        {
            _documentAuditor.RecordEvent(Thread.CurrentPrincipal.Identity, EventType.DocumentNamesRequested, "All documents were requested");
            return _fileContainer.GetFilenames();
        }

        public Uri GetDocumentUri(string filename)
        {
            var uri = _fileContainer.GetSecureFileUri(filename);

            _documentAuditor.RecordEvent(Thread.CurrentPrincipal.Identity, EventType.DocumentUriRequested, "Document '{0}' URI requested and returned as '{1}'", filename, uri.ToString());

            return uri;
        }

        private ISecureFileContainer _fileContainer;
        private IFilenameGenerator _filenameGenerator;
        private IDocumentAuditor _documentAuditor;
    }
}
