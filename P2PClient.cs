using System;
using System.IO;
using System.Net.Sockets;

namespace P2P_VDR_App
{
    public class P2PClient
    {
        private readonly string _serverIP;
        private readonly int _serverPort;

        public P2PClient(string serverIP, int serverPort)
        {
            _serverIP = serverIP;
            _serverPort = serverPort;
        }

        public void SendFile(string filePath)
        {
            try
            {
                using (TcpClient client = new TcpClient(_serverIP, _serverPort))
                using (NetworkStream stream = client.GetStream())
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    // Get file info
                    string fileName = Path.GetFileName(filePath);
                    FileInfo fileInfo = new FileInfo(filePath);

                    Console.WriteLine($"Sending file: {fileName}");
                    Console.WriteLine($"File size: {fileInfo.Length} bytes");

                    // Send file name and size
                    writer.Write(fileName);
                    writer.Write(fileInfo.Length);

                    // Send file data
                    using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        byte[] buffer = new byte[4096];
                        int bytesRead;
                        long totalBytesSent = 0;

                        while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            stream.Write(buffer, 0, bytesRead);
                            totalBytesSent += bytesRead;
                            Console.WriteLine($"Bytes sent: {totalBytesSent}/{fileInfo.Length}");
                        }
                    }

                    Console.WriteLine("File sent successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while sending file: {ex.Message}");
            }
        }


        public string[] GetFileList()
        {
            try
            {
                using (TcpClient client = new TcpClient(_serverIP, _serverPort))
                using (NetworkStream stream = client.GetStream())
                using (StreamWriter writer = new StreamWriter(stream) { AutoFlush = true })
                using (StreamReader reader = new StreamReader(stream))
                {
                    writer.WriteLine("LIST");
                    string response = reader.ReadLine();
                    return response.Split('|');
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching file list: {ex.Message}");
                return Array.Empty<string>();
            }
        }

        public bool DeleteFile(string fileName)
        {
            try
            {
                using (TcpClient client = new TcpClient(_serverIP, _serverPort))
                using (NetworkStream stream = client.GetStream())
                using (StreamWriter writer = new StreamWriter(stream) { AutoFlush = true })
                using (StreamReader reader = new StreamReader(stream))
                {
                    writer.WriteLine("DELETE");
                    writer.WriteLine(fileName);
                    string response = reader.ReadLine();
                    return response == "SUCCESS";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting file: {ex.Message}");
                return false;
            }
        }
    }
}
