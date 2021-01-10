using AuditSecurityManager;
using Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Policy;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;

namespace AuditServer
{
    class Program
    {
        static void Main(string[] args)
        {
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:9998/AuditServer";

            ServiceHost host = new ServiceHost(typeof(WCFService));
            host.AddServiceEndpoint(typeof(IZabeleziDogadjaje), binding, address);

            SetAudit(host);
            Thread nit = new Thread(PostaviObavestenja);
            try
            {                               
                nit.Start();
                host.Open();
                Console.WriteLine("AuditServer is started.\nPress <enter> to stop ...");
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

        public static void PostaviObavestenja()
        {

            EventLog customLog = null;
            const string SourceName = "AuditServer";            
            const string LogName = "AMSAuditServer";

            try
            {
                if (!EventLog.SourceExists(SourceName))
                {
                    EventLog.CreateEventSource(SourceName, LogName);
                }
                customLog = new EventLog(LogName, Environment.MachineName, SourceName);
            }
            catch (Exception e)
            {
                customLog = null;
                Console.WriteLine("Error while trying to create log handle. Error = {0}", e.Message);
            }

            while (true)
            {
                Thread.Sleep(10000);
                if (customLog != null)
                {                    
                    foreach (string dogadjaj in Read())
                    {
                        Console.WriteLine(dogadjaj);

                        if(dogadjaj.Contains("je uspesno isplaceno"))
                            customLog.WriteEntry(dogadjaj, EventLogEntryType.Warning);                        
                        else if (dogadjaj.Contains("NEUSPESNIH transakcija"))
                            customLog.WriteEntry(dogadjaj, EventLogEntryType.Error);
                        else if (dogadjaj.Contains("uspesnih transakcija"))
                            customLog.WriteEntry(dogadjaj, EventLogEntryType.Information);
                    }                        
                }
                else
                {
                    throw new ArgumentException("Error while trying to write event to event log.");
                }
            }            
        
        }

        public static string[] Read()
        {
            List<string> tempLines = new List<string>();
            string[] lines;
            lock (WCFService.resourceLock)
            {
                Database.korisnici.Clear();

                lines = File.ReadAllLines("txtInterLog.txt");
                int iznos = 0;

                foreach (string line in lines)
                {
                    if(line.Contains("je uspesno isplaceno"))
                    {
                        string[] tempStr = line.Split(' ');
                        try
                        {
                            iznos = Int32.Parse(tempStr[5]);                            
                        }
                        catch (FormatException e)
                        {
                            Console.WriteLine("greska u pretvaranju stringa u broj: " + e.Message);
                        }
                        if (iznos >= 10)
                            tempLines.Add(line);

                        if(!Database.korisnici.ContainsKey(tempStr[1]))
                        {
                            Record tempRecor = new Record();
                            tempRecor.UspesnihTransakcija = 1;
                            Database.korisnici.Add(tempStr[1], tempRecor);
                        }
                        else
                        {
                            Database.korisnici[tempStr[1]].UspesnihTransakcija += 1;
                        }
                    }  
                    else if(line.Contains("nije dozvoljena isplata"))
                    {
                        string[] tempStr = line.Split(' ');
                        if (!Database.korisnici.ContainsKey(tempStr[1]))
                        {
                            Record tempRecor = new Record();
                            tempRecor.NeuspesnihTransakcija = 1;
                            Database.korisnici.Add(tempStr[1], tempRecor);
                        }
                        else
                        {
                            Database.korisnici[tempStr[1]].NeuspesnihTransakcija += 1;
                        }
                    }
                }
                
                foreach(KeyValuePair<string, Record> korisnik in Database.korisnici)
                {
                    if (korisnik.Value.UspesnihTransakcija >= 1)
                        tempLines.Add(string.Format("Korisnik {0} je izvrsio {1} uspesnih transakcija.", korisnik.Key, korisnik.Value.UspesnihTransakcija));
                    if (korisnik.Value.NeuspesnihTransakcija > 3)
                        tempLines.Add(string.Format("Korisnik {0} je izvrsio {1} NEUSPESNIH transakcija.", korisnik.Key, korisnik.Value.NeuspesnihTransakcija));
                }

                using (StreamWriter file = new StreamWriter("txtInterLog.txt"));
            }
            return tempLines.ToArray();
        }
    }
}
