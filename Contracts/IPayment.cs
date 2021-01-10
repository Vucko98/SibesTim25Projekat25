using System.ServiceModel;

namespace Contracts
{
    [ServiceContract]
    public interface IPayment
    {
        [OperationContract]
        void AddClient(string naziv);

        [OperationContract]
        void DeleteClient(string naziv);

        [OperationContract]
        void Isplata(int iznos);

        [OperationContract]
        void Uplata(int iznos);
    }
}
