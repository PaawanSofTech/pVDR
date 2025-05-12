using System.Windows;

namespace P2P_VDR_App
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnP2P_Click(object sender, RoutedEventArgs e)
        {
            P2PWindow p2pWindow = new P2PWindow();
            p2pWindow.Show();
            this.Close(); // Close MainWindow
        }

        private void btnVDR_Click(object sender, RoutedEventArgs e)
        {
            VDRWindow vdrWindow = new VDRWindow();
            vdrWindow.Show();
            this.Close(); // Close MainWindow
        }
    }
}
