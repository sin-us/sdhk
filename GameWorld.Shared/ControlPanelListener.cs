using System;
using System.Diagnostics;
using System.ServiceModel;

namespace GameWorld.Shared
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ControlPanelListener : IControlPanel
    {
        public event Action<string> OnSetText;

        public static string Address
        {
            get { return $"net.pipe://localhost/{nameof(GameWorld)}"; }
        }

        public static ControlPanelListener Create()
        {
            var result = new ControlPanelListener();

            ServiceHost serviceHost = new ServiceHost(result);
            NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            serviceHost.AddServiceEndpoint(typeof(IControlPanel), binding, Address);
            serviceHost.Open();

            Debug.WriteLine("ServiceHost running...");

            return result;
        }

        public void SetText(string value)
        {
            OnSetText?.Invoke(value);
        }
    }
}
