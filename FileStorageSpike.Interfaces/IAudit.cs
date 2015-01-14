using System.Security.Principal;

namespace FileStorageSpike.Interfaces
{
    public interface IAudit
    {
        void RecordOperation(IPrincipal principal, string stringOrFormat, params object[] args);
    }
}
