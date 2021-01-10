using Contracts;
using System;
using System.ServiceModel;
using System.ServiceModel.Security;

namespace AccountManagementService
{
    public class WCFClient : ChannelFactory<IZabeleziDogadjaje>, IZabeleziDogadjaje, IDisposable
    {
        IZabeleziDogadjaje factory;
        public WCFClient(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
        {
            factory = this.CreateChannel();
        }

        public void ZabeleziDogadjaje(string[] dogadjaji)
        {
            try
            {
                factory.ZabeleziDogadjaje(dogadjaji);                
            }
            catch (SecurityAccessDeniedException e)
            {
                Console.WriteLine("Error while trying to ZabeleziDogadjaje. Error message : {0}", e.Message);
            }
            catch (FaultException e)
            {
                Console.WriteLine("Error while trying to ZabeleziDogadjaje. Server message: {0}", e.Message);
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
