using System.Collections.ObjectModel;
using System.IO;

namespace P2P_VDR_App
{
    public class FileManager
    {
        public ObservableCollection<FileRecord> Files { get; private set; }

        public FileManager()
        {
            Files = new ObservableCollection<FileRecord>();
        }

        public void UploadFile(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            Files.Add(new FileRecord { FileName = fileName, Status = "Uploaded" });
        }

        public bool DownloadFile(string fileName, string destinationPath)
        {
            // Simulate file download (in a real scenario, you'd copy from server)
            try
            {
                string mockSourcePath = Path.Combine(Directory.GetCurrentDirectory(), "MockFiles", fileName);
                if (!File.Exists(mockSourcePath))
                {
                    // Create a mock file for demo purposes
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "MockFiles"));
                    File.WriteAllText(mockSourcePath, $"This is a mock file for {fileName}");
                }

                string destinationFile = Path.Combine(destinationPath, fileName);
                File.Copy(mockSourcePath, destinationFile, true);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
