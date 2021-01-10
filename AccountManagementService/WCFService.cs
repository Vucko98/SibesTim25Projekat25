using AccountManagementServiceHelper;
using AuditSecurityManager;
using Common;
using Contracts;
using System;
using System.ServiceModel;
using System.Threading;

namespace AccountManagementService
{

    public class WCFService : IPayment
    {
        public static AMS zapisivac;        


        public static bool IzaberiVrstuAMSa()
        {
            Console.WriteLine(string.Format("izaberi vrstu ams-a: <1> za Windows Event Log, <2> za XML fajl ili <3> za TXT file"));
            while (true)
            {
                ConsoleKeyInfo c = Console.ReadKey();
                if (c.KeyChar == '1')
                {
                    zapisivac = new AMS_WEL();
                    return true;
                }
                else if (c.KeyChar == '2')
                {
                    zapisivac = new AMS_XML();
                    return false;
                }
                else if (c.KeyChar == '3')
                {
                    zapisivac = new AMS_TXT();
                    return false;
                }
                else
                    Console.WriteLine("Pogresan taster, ponoviti unos: ");
            }            
        }

        public void AddClient(string naziv)
        {
            CustomPrincipal principal = (CustomPrincipal)Thread.CurrentPrincipal;
            string userName = Formatter.ParseName(principal.Identity.Name);
            string poruka = "";

            if (Thread.CurrentPrincipal.IsInRole("AddClient"))
            {
                try
                {
                    if (!Database.korisnici.ContainsKey(naziv))
                    {
                        Korisnik temp = new Korisnik(naziv, 0);
                        Database.korisnici.Add(temp.Naziv, temp);
                        poruka = Audit.DodajKorisnikaSuccess(userName, naziv);
                        zapisivac.Write(poruka);                        
                    }
                    else
                    {
                        poruka = Audit.DodajKorisnikaFailed(userName, naziv, "jer on vec postoji u bazi podataka");
                        zapisivac.Write(poruka);                        
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            else
            {
                try
                {
                    poruka = Audit.DodajKorisnikaFailed(userName, naziv, "jer nije clan odgovarajuce grupe");
                    zapisivac.Write(poruka);                    
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + " <WCFService.AddClient>");
                }                
            }

            throw new FaultException(poruka);
        }

        public void DeleteClient(string naziv)
        {
            CustomPrincipal principal = (CustomPrincipal)Thread.CurrentPrincipal;
            string userName = Formatter.ParseName(principal.Identity.Name);
            string poruka = "";

            if (Thread.CurrentPrincipal.IsInRole("DeleteClient"))
            {
                try
                {
                    if (Database.korisnici.ContainsKey(naziv))
                    {
                        poruka = Audit.ObrisiKorisnikaSuccess(userName, naziv);
                        zapisivac.Write(poruka);
                        Database.korisnici.Remove(naziv);                        
                    }
                    else
                    {
                        poruka = Audit.ObrisiKorisnikaFailed(userName, naziv, "jer nije u bazi podataka");
                        zapisivac.Write(poruka);                        
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            else
            {
                try
                {
                    poruka = Audit.ObrisiKorisnikaFailed(userName, naziv, "jer nije clan odgovarajuce grupe");
                    zapisivac.Write(poruka);                    
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + " <WCFService.Isplata>");
                }                
            }

            throw new FaultException(poruka);
        }

        public void Isplata(int iznos)
        {
            CustomPrincipal principal = (CustomPrincipal)Thread.CurrentPrincipal;
            string userName = Formatter.ParseName(principal.Identity.Name);
            string poruka = "";

            if (Thread.CurrentPrincipal.IsInRole("Isplata"))
            {                
                try
                {
                    if (Database.korisnici.ContainsKey(userName))
                    {
                        if(Database.korisnici[userName].Racun - iznos > 0)
                        {
                            poruka = Audit.IsplataSuccess(userName, iznos);
                            zapisivac.Write(poruka);
                            Database.korisnici[userName].Racun -= iznos;                            
                        }
                        else
                        {
                            poruka = Audit.IsplataFailed(userName, "jer nema dovoljno novca");
                            zapisivac.Write(poruka);                            
                        }
                    }
                    else
                    {
                        poruka = Audit.IsplataFailed(userName, "jer nije u bazi podataka");
                        zapisivac.Write(poruka);                        
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            else
            {
                try
                {
                    poruka = Audit.IsplataFailed(userName, "jer nije clan odgovarajuce grupe");
                    zapisivac.Write(poruka);                    
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + " <WCFService.Isplata>");
                }                
            }

            throw new FaultException(poruka);
        }

        public void Uplata(int iznos)
        {
            
                CustomPrincipal principal = (CustomPrincipal)Thread.CurrentPrincipal;            
                string userName = Formatter.ParseName(principal.Identity.Name);
                string poruka = "";

                if (Thread.CurrentPrincipal.IsInRole("Uplata"))
                {                
                    try
                    {
                        if (Database.korisnici.ContainsKey(userName))
                        {
                            poruka = Audit.UplataSuccess(userName, iznos);
                            zapisivac.Write(poruka);
                            Database.korisnici[userName].Racun += iznos;                            
                        }
                        else
                        {
                            poruka = Audit.UplataFailed(userName, "jer nije u bazi podataka");
                            zapisivac.Write(poruka);                            
                    }                                              
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);                    
                    }
                }
                else
                {
                    try
                    {
                        poruka = Audit.UplataFailed(userName, "jer nije clan odgovarajuce grupe");
                        zapisivac.Write(poruka);                        
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message + " <WCFService.Uplata>");                    
                    }                    
                }

            throw new FaultException(poruka);
        }
    }
}
