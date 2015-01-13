using System;

namespace FileStorageSpike.Interfaces
{
    public interface ISecureFileStore : IFileStore
    {
        /// <summary>
        /// Returns a URI that permits read only access to the file for a limited period of time
        /// </summary>
        /// <param name="filename">The filename to access</param>
        /// <returns>A URL for that file</returns>
        Uri GetSecureFileUri(string filename);
    }
}
