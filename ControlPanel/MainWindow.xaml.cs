using GameWorld.Shared;
using System.Diagnostics;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;

namespace ControlPanel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IControlPanel channel;

        public MainWindow()
        {
            string address = ControlPanelListener.Address;

            NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            EndpointAddress ep = new EndpointAddress(address);
            channel = ChannelFactory<IControlPanel>.CreateChannel(binding, ep);

            Debug.WriteLine("Client Connected");

            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            channel.SetText(((TextBox)sender).Text);
        }
    }
}
