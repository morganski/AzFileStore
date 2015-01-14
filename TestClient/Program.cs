﻿using Autofac;
using FileStorageSpike.Interfaces;
using FileStorageSpike.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);

            var container = BuildContainer();

            var containerFunc = container.Resolve<Func<string, ISecureFileContainer>>();

            var store = containerFunc("sausage");

            //var store = new AzureFileContainer(ConfigurationManager.ConnectionStrings["azure"].ConnectionString, "sausage");

            string filename = Path.GetTempFileName();

            using (var stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine("<HTML><BODY>Clucking bell!</BODY></HTML>");
                    writer.Flush();
                }
            }

            using (var stream = new FileStream(filename, FileMode.Open))
            {
                // Stuff a file into the store...
                store.StoreFile(FILENAME, stream, "text/html");
            }

            // Get a list of files in the store...
            var filenames = store.GetFilenames();

            // And get a specific file...
            using (var stream = store.GetFile(FILENAME))
            {
                using (var reader = new StreamReader(stream))
                {
                    var content = reader.ReadToEnd();
                }
            }

            var uri = store.GetSecureFileUri(FILENAME);
        }

        static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<AzureFileContainer>()
                .Named<ISecureFileContainer>("wrapped")
                .WithParameter("connectionString", ConfigurationManager.ConnectionStrings["azure"].ConnectionString)
                .WithParameter("fileShareDuration", TimeSpan.FromMinutes(1));   // Only set this up for development, would expect a longer duration!

            builder.RegisterType<AuditedFileContainer>()
                .As<ISecureFileContainer, IFileContainer>();

            builder.RegisterType<ConsoleAudit>()
                .As<IAudit>();

            builder.Register<Func<string, ISecureFileContainer>>(cc =>
            {
                var context = cc.Resolve<IComponentContext>();

                return (containerName) =>
                {
                    var wrapped = context.ResolveNamed<ISecureFileContainer>("wrapped", new NamedParameter("containerName", containerName));
                    return context.Resolve<ISecureFileContainer>(new TypedParameter(typeof(ISecureFileContainer), wrapped));
                };
            });

            return builder.Build();
        }

        const string FILENAME = "jim.htm";
    }


}
