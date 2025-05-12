using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;

namespace P2P_VDR_App
{
    public partial class P2PWindow : Window
    {
        private ObservableCollection<string> devices;

        public P2PWindow()
        {
            InitializeComponent();
            devices = new ObservableCollection<string>(); // Initialize the ObservableCollection
            deviceList.ItemsSource = devices; // Bind devices to the ListBox
            StartFileReceiver(); // Automatically start listening for files
        }

        private void SendFile(string filePath, string receiverIP, int port = 5000)
        {
            try
            {
                using (TcpClient client = new TcpClient(receiverIP, port))
                using (NetworkStream stream = client.GetStream())
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    string fileName = System.IO.Path.GetFileName(filePath);
                    FileInfo fileInfo = new FileInfo(filePath);

                    // Send file name
                    writer.Write(fileName);

                    // Send file size
                    writer.Write(fileInfo.Length);

                    // Send file data
                    using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        byte[] buffer = new byte[4096];
                        long totalBytesSent = 0;
                        int bytesRead;

                        while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            writer.Write(buffer, 0, bytesRead);
                            totalBytesSent += bytesRead;

                            double progress = (double)totalBytesSent / fileInfo.Length * 100;
                            Log($"Sending '{fileName}': {progress:F2}%");
                        }
                    }

                    Log($"File '{fileName}' sent successfully to {receiverIP}.");
                }
            }
            catch (Exception ex)
            {
                Log($"Error sending file: {ex.Message}");
            }
        }


        private void StartFileReceiver(int port = 5000, string saveDirectory = "ReceivedFiles")
        {
            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }

            Task.Run(() =>
            {
                TcpListener listener = new TcpListener(System.Net.IPAddress.Any, port);
                listener.Start();
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"File receiver started on port {port}. Waiting for incoming files...");
                });

                while (true)
                {
                    using (TcpClient client = listener.AcceptTcpClient())
                    using (NetworkStream stream = client.GetStream())
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        try
                        {
                            // Read file name
                            string fileName = reader.ReadString();

                            // Read file size
                            long fileSize = reader.ReadInt64();

                            // Save file
                            string filePath = Path.Combine(saveDirectory, fileName);
                            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                            {
                                byte[] buffer = new byte[4096];
                                long totalBytesRead = 0;
                                int bytesRead;

                                while (totalBytesRead < fileSize && (bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    fs.Write(buffer, 0, bytesRead);
                                    totalBytesRead += bytesRead;
                                }
                            }

                            // Update UI on file receipt
                            Dispatcher.Invoke(() =>
                            {
                                MessageBox.Show($"File '{fileName}' received and saved to '{saveDirectory}'.");
                            });
                        }
                        catch (Exception ex)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                MessageBox.Show($"Error receiving file: {ex.Message}");
                            });
                        }
                    }
                }
            });
        }

        private void Log(string message)
        {
            Dispatcher.Invoke(() =>
            {
                logTextBox.AppendText($"{DateTime.Now}: {message}\n");
                logTextBox.ScrollToEnd();
            });
        }

        private void btnDiscover_Click(object sender, RoutedEventArgs e)
        {
            // Simulate device discovery
            devices.Clear();
            devices.Add("Device1 (192.168.1.101)");
            devices.Add("Device2 (192.168.1.102)");
            MessageBox.Show("Devices discovered.");
        }

        private void btnSendFile_Click(object sender, RoutedEventArgs e)
        {
            if (deviceList.SelectedItem is string selectedDevice)
            {
                var openFileDialog = new Microsoft.Win32.OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    string filePath = openFileDialog.FileName;

                    // Extract IP address from the selected device
                    string receiverIP = selectedDevice.Split('(')[1].Trim(')');
                    SendFile(filePath, receiverIP);
                }
                else
                {
                    MessageBox.Show("No file selected.");
                }
            }
            else
            {
                MessageBox.Show("Please select a device.");
            }
        }

    }
}
