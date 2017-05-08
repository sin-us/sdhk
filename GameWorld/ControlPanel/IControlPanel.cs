using System.ServiceModel;

namespace GameWorld.ControlPanel
{
    [ServiceContract]
    public interface IControlPanel
    {
        [OperationContract]
        void SetText(string value);
    }
}
