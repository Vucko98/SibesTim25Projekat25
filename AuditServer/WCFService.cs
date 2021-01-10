using AuditSecurityManager;
using Common;
using Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AuditServer
{
    public class WCFService : IZabeleziDogadjaje
    {
        public static object resourceLock = new object();

        public void ZabeleziDogadjaje(string[] dogadjaji)
        {
            CustomPrincipal principal = (CustomPrincipal)Thread.CurrentPrincipal;
            string userName = Formatter.ParseName(principal.Identity.Name);

            if (Thread.CurrentPrincipal.IsInRole("ZabeleziDogadjaje"))
            {
                lock (resourceLock)
                {
                    using (StreamWriter file = new StreamWriter("txtInterLog.txt", true))
                    {
                        foreach (string dogadjaj in dogadjaji)
                        {
                            file.WriteLine(dogadjaj);                            
                        }                            
                    }
                }
            }
            else
            {
                throw new FaultException("User " + userName + " try to call ZabeleziDogadjaje method. ZabeleziDogadjaje method need ZabeleziDogadjaje permission.");
            }
        }
    }
}
