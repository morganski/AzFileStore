using System;
using System.Collections.Generic;
using System.IO;

namespace FileStorageSpike.Interfaces
{
    /// <summary>
    /// Interface used to abstract storage of files
    /// </summary>
    public interface IFileContainer
    {
        /// <summary>
        /// Get a file given its filename
        /// </summary>
        /// <param name="filename">The name of the file</param>
        /// <returns>A stream containing the file</returns>
        Stream GetFile(string filename);

        /// <summary>
        /// Store a file
        /// </summary>
        /// <param name="filename">The name of the file</param>
        /// <param name="fileContent">The content of that file</param>
        /// <param name="mimeType">Defines the MIME type of the content</param>
        void StoreFile(string filename, Stream fileContent, string mimeType);

        /// <summary>
        /// Return a collection of filenames from this file store
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetFilenames();
    }
}
