using System.ServiceModel;

namespace GameWorld.Shared
{
    [ServiceContract]
    public interface IControlPanel
    {
        [OperationContract]
        void SetText(string value);
    }
}
