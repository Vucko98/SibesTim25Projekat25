using AccountManagementServiceHelper;
using AuditSecurityManager;
using CertificateManager;
using Common;
using Contracts;
using System;
using System.Collections.Generic;
using System.IdentityModel.Policy;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Threading;

namespace AccountManagementService
{
    public class Program
    {
        static void Main(string[] args)
        {
            string address = "net.tcp://localhost:9999/ams";
            
            NetTcpBinding binding = new NetTcpBinding();
            //binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            
            ServiceHost host = new ServiceHost(typeof(WCFService));
            host.AddServiceEndpoint(typeof(IPayment), binding, address);

            //SetCert(binding, host);                

            Thread nit = new Thread(PosaljiNaAuditObradu);
            try
            {
                WCFService.IzaberiVrstuAMSa();                             
                Console.WriteLine();
                                
                nit.Start(WCFService.zapisivac);
                SetAudit(host);

                host.Open();
                Console.WriteLine("AccountManagementService is started.\nPress <enter> to stop ...");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("[ERROR] {0}", e.Message);
                Console.WriteLine("[StackTrace] {0}", e.StackTrace);
            }
            finally
            {
                host.Close();
                nit.Abort();
            }
        }

        static void SetCert(NetTcpBinding binding, ServiceHost host)
        {            

            ///Custom validation mode enables creation of a custom validator - CustomCertificateValidator
            host.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
            host.Credentials.ClientCertificate.Authentication.CustomCertificateValidator = new ServiceCertValidator();

            ///If CA doesn't have a CRL associated, WCF blocks every client because it cannot be validated
            host.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            /// srvCertCN.SubjectName should be set to the service's username. .NET WindowsIdentity class provides information about Windows user running the given process
            string srvCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

            ///Set appropriate service's certificate on the host. Use CertManager class to obtain the certificate based on the "srvCertCN"
            host.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);
        }

        static void SetAudit(ServiceHost host)
        {
            host.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            host.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });

            host.Authorization.ServiceAuthorizationManager = new CustomServiceAuthorizationManager();

            host.Authorization.PrincipalPermissionMode = PrincipalPermissionMode.Custom;
            List<IAuthorizationPolicy> policies = new List<IAuthorizationPolicy>();
            policies.Add(new CustomAuthorizationPolicy());
            host.Authorization.ExternalAuthorizationPolicies = policies.AsReadOnly();

            //podesavanje AutidBehaviour-a            
            ServiceSecurityAuditBehavior newAudit = new ServiceSecurityAuditBehavior();
            newAudit.AuditLogLocation = AuditLogLocation.Application;
            newAudit.ServiceAuthorizationAuditLevel = AuditLevel.SuccessOrFailure;

            host.Description.Behaviors.Remove<ServiceSecurityAuditBehavior>();
            host.Description.Behaviors.Add(newAudit);
        }

        public static void PosaljiNaAuditObradu(object citac)
        {
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:9998/AuditServer";

            
            using (WCFClient proxy = new WCFClient(binding, new EndpointAddress(new Uri(address))))
            {
                while(true)
                {
                    Thread.Sleep(5000);                    
                    try
                    {
                        proxy.ZabeleziDogadjaje(((AMS)citac).Read());
                    }                   
                    catch (Exception e)
                    {
                        Console.WriteLine("Error message: {0}", e.Message);
                    }
                }                
            }
            
        }        
    }
}

