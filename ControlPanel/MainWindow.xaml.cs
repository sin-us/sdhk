using GameWorld.ControlPanel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            string address = $"net.pipe://localhost/{nameof(GameWorld)}";

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
