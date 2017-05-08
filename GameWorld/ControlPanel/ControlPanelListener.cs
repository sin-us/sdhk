using GameWorld.ControlPanel;
using System;
using System.Diagnostics;
using System.ServiceModel;

namespace GameWorld
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ControlPanelListener : IControlPanel
    {
        public event Action<string> OnSetText;

        public static ControlPanelListener Create()
        {
            var result = new ControlPanelListener();

            string address = $"net.pipe://localhost/{nameof(GameWorld)}";

            ServiceHost serviceHost = new ServiceHost(result);
            NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            serviceHost.AddServiceEndpoint(typeof(IControlPanel), binding, address);
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
