using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Linq;

namespace P2P_VDR_App
{
    public class P2PServer
    {
        private TcpListener _listener;
        private readonly int _port;
        private readonly string _saveDirectory;

        public P2PServer(int port, string saveDirectory)
        {
            _port = port;
            _saveDirectory = saveDirectory;

            // Ensure the directory exists and log its creation
            if (!Directory.Exists(_saveDirectory))
            {
                Directory.CreateDirectory(_saveDirectory);
                Console.WriteLine($"Directory created: {_saveDirectory}");
            }
            else
            {
                Console.WriteLine($"Directory already exists: {_saveDirectory}");
            }
        }


        public void Start()
        {
            try
            {
                _listener = new TcpListener(IPAddress.Any, _port);
                _listener.Start();
                Console.WriteLine($"Server started on port {_port}.");

                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    TcpClient client = _listener.AcceptTcpClient();
                    Console.WriteLine("Client connected.");

                    using (NetworkStream stream = client.GetStream())
                    using (StreamReader reader = new StreamReader(stream))
                    using (StreamWriter writer = new StreamWriter(stream) { AutoFlush = true })
                    {
                        string command = reader.ReadLine();
                        Console.WriteLine($"Command received: {command}");

                        if (command == "UPLOAD")
                        {
                            ReceiveFile(stream);
                        }
                        else if (command == "LIST")
                        {
                            string[] files = GetFileList();
                            writer.WriteLine(string.Join("|", files));
                        }
                        else if (command == "DELETE")
                        {
                            string fileName = reader.ReadLine();
                            bool success = DeleteFile(fileName);
                            writer.WriteLine(success ? "SUCCESS" : "FAILURE");
                        }
                    }

                    client.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Server error: {ex.Message}");
            }
        }

        private void ReceiveFile(NetworkStream stream)
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                try
                {
                    // Read file name
                    string fileName = reader.ReadString();
                    Console.WriteLine($"Receiving file: {fileName}");

                    // Read file size
                    long fileSize = reader.ReadInt64();
                    Console.WriteLine($"File size: {fileSize} bytes");

                    // Verify save path
                    string filePath = Path.Combine(_saveDirectory, fileName);
                    Console.WriteLine($"File will be saved to: {filePath}");

                    // Save file data
                    using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        byte[] buffer = new byte[4096];
                        long totalBytesRead = 0;
                        int bytesRead;

                        while (totalBytesRead < fileSize && (bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            fs.Write(buffer, 0, bytesRead);
                            totalBytesRead += bytesRead;
                            Console.WriteLine($"Bytes received: {totalBytesRead}/{fileSize}");
                        }
                    }

                    Console.WriteLine($"File saved successfully to: {filePath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while receiving file: {ex.Message}");
                }
            }
        }


        private string[] GetFileList()
        {
            return Directory.Exists(_saveDirectory)
                ? Directory.GetFiles(_saveDirectory).Select(Path.GetFileName).ToArray()
                : Array.Empty<string>();
        }

        private bool DeleteFile(string fileName)
        {
            string filePath = Path.Combine(_saveDirectory, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Console.WriteLine($"File deleted: {fileName}");
                return true;
            }
            return false;
        }
    }
}
