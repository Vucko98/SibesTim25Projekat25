using CertificateManager;
using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Threading;

namespace ClientApp
{
	public class Program
	{
		static void Main(string[] args)
		{									
			NetTcpBinding binding = new NetTcpBinding();														
			EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:9999/ams"));
			//SetCeret(binding, address);

			try
            {
				using (WCFClient proxy = new WCFClient(binding, address))
				{
					Console.WriteLine("Izabrati 1 za manualni ili 2 za automatski unos: ");

					ConsoleKeyInfo c;
					while (true)
                    {
						c = Console.ReadKey();
						if (c.KeyChar == '1' || c.KeyChar == '2')
							break;
						else
							Console.WriteLine("Pogresan taster, ponoviti unos: ");
					}

					if (c.KeyChar == '2')
					{
						while (true)
						{
							proxy.Uplata(1);
							proxy.Isplata(10);
							//proxy.Uplata(1);
							//proxy.Isplata(1);
							//proxy.Isplata(1);
							//proxy.AddClient("client1");
							//proxy.DeleteClient("client10");
							//proxy.AddClient("client5");
							//proxy.DeleteClient("client5");
							//proxy.DeleteClient("client5");
							//proxy.AddClient("client5");
							//proxy.AddClient("client5");
							Thread.Sleep(2500);
						}
					}

					else
						Meni(proxy);
				}
			}
			catch(Exception e)
            {
				Console.WriteLine();
            }
			finally
            {
				Console.ReadLine();
			}			
		}

		static void SetCeret(NetTcpBinding binding, EndpointAddress address)
        {
			/// Define the expected service certificate. It is required to establish cmmunication using certificates.
			string srvCertCN = "wcfservice";
			/// Use CertManager class to obtain the certificate based on the "srvCertCN" representing the expected service identity.	
			X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, srvCertCN);

			binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
			address = new EndpointAddress(new Uri("net.tcp://localhost:9999/ams"), new X509CertificateEndpointIdentity(srvCert));
		}

		static void Meni(WCFClient proxy)
        {
			Console.WriteLine();
			Console.WriteLine("1 za Uplatu. 2 za Isplatu. 3 za DodavanjeKorisnika. 4 za BrisanjeKorisnika");
			ConsoleKeyInfo c;
			string unos;
			int iznos = 0;
			while (true)
            {
				c = Console.ReadKey();
				if (c.KeyChar == '1')
                {
					Console.Write(" Iznos: ");					
                    while(true)
                    {
						try
						{
							unos = Console.ReadLine();
							iznos = Int32.Parse(unos);
							if (iznos <= 0)
                            {
								Console.WriteLine("Potrebno je uneti broj veci od 0: ");
								continue;
							}
							break;
						}
						catch
						{
							Console.WriteLine("Potrebno je uneti broj: ");	
						}
					}
                    try
                    {						
						proxy.Uplata(iznos);						
					}
                    catch(Exception e)
                    {
						Console.WriteLine(e.Message);
                    }
					
                }					
				else if(c.KeyChar == '2')
                {
					Console.Write(" Iznos: ");					
					while (true)
					{
						try
						{
							unos = Console.ReadLine();
							iznos = Int32.Parse(unos);
							if (iznos <= 0)
							{
								Console.WriteLine("Potrebno je uneti broj veci od 0: ");
								continue;
							}
							break;
						}
						catch
						{
							Console.WriteLine("Potrebno je uneti broj: ");
						}
					}
					try
					{						;
						proxy.Isplata(iznos);						
					}
					catch (Exception e)
					{
						Console.WriteLine(e.Message);
					}
				}
				else if (c.KeyChar == '3')
                {
					Console.Write("Naziv korisnika: ");
					unos = Console.ReadLine();

					try
					{						
						proxy.AddClient(unos);						
					}
					catch (Exception e)
					{
						Console.WriteLine(e.Message);
					}
				}
				else if (c.KeyChar == '4')
                {
					Console.Write("Naziv korisnika: ");
					unos = Console.ReadLine();

					try
					{
						proxy.DeleteClient(unos);						
					}
					catch (Exception e)
					{
						Console.WriteLine(e.Message);
					}
				}
				else
					Console.WriteLine("Pogresan taster, ponoviti unos: ");
			}
        }

	}
}
