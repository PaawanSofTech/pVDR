using System.Collections.ObjectModel;
using System.Windows;

namespace P2P_VDR_App
{
    public partial class VDRWindow : Window
    {
        private ObservableCollection<string> files;

        public VDRWindow()
        {
            InitializeComponent();
            files = new ObservableCollection<string>();
            fileList.ItemsSource = files;
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Login successful.");
        }

        private void btnViewFiles_Click(object sender, RoutedEventArgs e)
        {
            files.Clear();
            files.Add("File1.txt");
            files.Add("File2.pdf");
            MessageBox.Show("Files loaded.");
        }
    }
}
