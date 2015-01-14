using FileStorageSpike.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace FileStorageSpike.Services
{
    public class ConsoleAudit : IAudit
    {
        public void RecordOperation(IPrincipal principal, string stringOrFormat, params object[] args)
        {
            string formatted = string.Format(stringOrFormat, args);

            Console.WriteLine("{0} at {1} : {2}", principal.Identity.Name, DateTime.UtcNow, formatted);
        }
    }
}
