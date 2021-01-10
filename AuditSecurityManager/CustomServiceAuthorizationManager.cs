using Common;
using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;

namespace AuditSecurityManager
{
    public class CustomServiceAuthorizationManager : ServiceAuthorizationManager
    {
        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            CustomPrincipal principal = operationContext.ServiceSecurityContext.AuthorizationContext.Properties["Principal"] as CustomPrincipal;            
            return true;
            /*
            bool retValue = principal.IsInRole("ZajednicakaUloga");

            if (!retValue)
            {
                try
                {
                    Audit.AuthorizationFailed(Formatter.ParseName(principal.Identity.Name), OperationContext.Current.IncomingMessageHeaders.Action, "Need Read permission.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return retValue;
            */
        }
    }
}
