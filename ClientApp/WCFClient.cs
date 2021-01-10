using CertificateManager;
using Common;
using Contracts;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Security;

namespace ClientApp
{
    public class WCFClient : ChannelFactory<IPayment>, IPayment, IDisposable
	{
		IPayment factory;

		public WCFClient(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
		{
			//SetCredentials();
			factory = this.CreateChannel();
		}

		private void SetCredentials()
        {
			/// cltCertCN.SubjectName should be set to the client's username. .NET WindowsIdentity class provides information about Windows user running the given process
			string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

			this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.Custom;
			this.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator = new ClientCertValidator();
			this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

			/// Set appropriate client's certificate on the channel. Use CertManager class to obtain the certificate based on the "cltCertCN"
			this.Credentials.ClientCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);
		}

		public void DeleteClient(string naziv)
		{
			try
			{
				factory.DeleteClient(naziv);
				Console.WriteLine("DeleteClient allowed.");
			}
			catch (SecurityAccessDeniedException e)
			{
				Console.WriteLine("Error while trying to DeleteClient. Error message: {0}", e.Message);
			}
			catch (FaultException e)
			{
				Console.WriteLine("Server message: {0}", e.Message);
			}
		}

		public void AddClient(string naziv)
		{
			try
			{
				factory.AddClient(naziv);
				Console.WriteLine("AddClient allowed.");
			}
			catch (SecurityAccessDeniedException e)
			{
				Console.WriteLine("Error while trying to AddClient. Error message: {0}", e.Message);
			}
			catch (FaultException e)
			{
				Console.WriteLine("Server message: {0}", e.Message);
			}
		}

		public void Isplata(int iznos)
		{
			try
			{
				factory.Isplata(iznos);				
			}
			catch (SecurityAccessDeniedException e)
			{
				Console.WriteLine("Error while trying to Isplata. Error message: {0}", e.Message);
			}
			catch (FaultException e)
			{
				Console.WriteLine("Server message: {0}", e.Message);
			}
		}

		public void Uplata(int iznos)
		{
			try
			{
				factory.Uplata(iznos);				
			}
			catch (SecurityAccessDeniedException e)
			{
				Console.WriteLine("Error while trying to Uplata. Error message: {0}", e.Message);
			}
			catch (FaultException e)
			{
				Console.WriteLine("Server message: {0}", e.Message);
			}
		}


		public void Dispose()
		{
			if (factory != null)
			{
				factory = null;
			}

			this.Close();
		}
	}
}
