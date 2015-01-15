using FileStorageSpike.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace FileStorageSpike.Services
{
    public class ConsoleDocumentAuditor : IDocumentAuditor
    {
        public void RecordEvent(IIdentity identity, EventType eventType, string stringOrFormat, params string[] args)
        {
            string formatted = string.Format(stringOrFormat, args);

            Console.WriteLine("'{0}' caused a '{1}' event at '{2}' with the following details:\r\n  {3}", identity.Name, eventType, DateTime.UtcNow, formatted);
        }
    }
}
