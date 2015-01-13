﻿using FileStorageSpike.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var store = new AzureFileStore(ConfigurationManager.ConnectionStrings["azure"].ConnectionString, "sausage");

            string filename = Path.GetTempFileName();

            using (var stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine("Hello there, this is a test");
                    writer.Flush();
                }
            }

            using (var stream = new FileStream(filename, FileMode.Open))
            {
                // Stuff a file into the store...
                store.StoreFile("jim.txt", stream);
            }

            // Get a list of files in the store...
            var filenames = store.GetFilenames();

            // And get a specific file...
            using (var stream = store.GetFile("jim.txt"))
            {
                using (var reader = new StreamReader(stream))
                {
                    var content = reader.ReadToEnd();
                }
            }

        }
    }
}