using Autofac;
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

            var containerFunc = container.Resolve<Func<string, IDocumentContainer>>();

            var store = containerFunc("sausage");

            string filename = Path.GetTempFileName();

            using (var stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine("<HTML><BODY>Clucking bell!</BODY></HTML>");
                    writer.Flush();
                }
            }

            string storedFilename = null;

            using (var stream = new FileStream(filename, FileMode.Open))
            {
                // Stuff a file into the store...
                storedFilename = store.StoreDocument(stream, "text/html", "A random file");
            }

            // Get a list of files in the store...
            var filenames = store.GetDocumentNames();

            // Now get the file URI...
            var uri = store.GetDocumentUri(storedFilename);
        }

        static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<AzureFileContainer>()
                .As<IFileContainer, ISecureFileContainer>()
                .WithParameter("connectionString", ConfigurationManager.ConnectionStrings["azure"].ConnectionString)
                .WithParameter("fileShareDuration", TimeSpan.FromMinutes(1));   // Only set this up for development, would expect a longer duration!

            builder.RegisterType<DocumentContainer>()
                .As<IDocumentContainer>();

            builder.Register<Func<string, ISecureFileContainer>>(cc =>
            {
                var context = cc.Resolve<IComponentContext>();

                return containerName => context.Resolve<ISecureFileContainer>(new NamedParameter("containerName", containerName));
            });

            builder.Register<Func<string, IDocumentContainer>>(cc =>
            {
                var context = cc.Resolve<IComponentContext>();

                return containerName => context.Resolve<IDocumentContainer>(new NamedParameter("containerName", containerName));
            });

            builder.RegisterType<FilenameGenerator>()
                .As<IFilenameGenerator>();

            builder.RegisterType<ConsoleDocumentAuditor>()
                .As<IDocumentAuditor>();

            builder.RegisterType<FilenameGenerator>()
                .As<IFilenameGenerator>();

            return builder.Build();
        }

        const string FILENAME = "jim.htm";
    }


}
