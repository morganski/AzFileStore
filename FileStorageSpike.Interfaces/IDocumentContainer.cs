using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;

namespace FileStorageSpike.Interfaces
{
    /// <summary>
    /// Interface used to access 
    /// </summary>
    public interface IDocumentContainer
    {
        /// <summary>
        /// Store a document
        /// </summary>
        /// <param name="fileContent">The content of that file</param>
        /// <param name="mimeType">Defines the MIME type of the content</param>
        /// <param name="contentType">The user defined content type, such as "Passport", "Bank Statement"</param>
        /// <returns>The name of the file that was created</returns>
        string StoreDocument(Stream fileContent, string mimeType, string contentType);

        /// <summary>
        /// Return a collection of filenames from this file store
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetDocumentNames();

        /// <summary>
        /// Returns a URI that permits read only access to the file for a limited period of time
        /// </summary>
        /// <param name="filename">The filename to access</param>
        /// <returns>A URL for that file</returns>
        Uri GetDocumentUri(string filename);
    }
}
