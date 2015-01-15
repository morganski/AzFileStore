using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace FileStorageSpike.Interfaces
{
    public interface IDocumentAuditor
    {
        void RecordEvent(IIdentity identity, EventType eventType, string stringOrFormat, params string[] args);
    }
}
