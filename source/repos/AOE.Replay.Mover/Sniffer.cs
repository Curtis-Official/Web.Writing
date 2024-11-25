using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System;
using System.IO;
using System.IO.Compression;

namespace AOE.Replay.Mover
{


    class Sniffer
    {

        public static string downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads";
        public string extractPath = "C:\\Users\\curti\\OneDrive\\Desktop\\ExtractPath";
        public string holdingarea = "C:\\Users\\curti\\OneDrive\\Desktop\\HoldingArea";
        public string destinationDirectory = "C:\\Users\\curti\\Games\\Age of Empires 2 DE\\76561198335390435\\savegame";



        public void Run()
        {
            DateTime today = DateTime.Today;

            // Get files from the Downloads folder created or modified today
            var todayFiles = new DirectoryInfo(downloadsPath)
                .EnumerateFiles()
                .Where(file => file.LastWriteTime.Date == today || file.CreationTime.Date == today)
                .ToList();

            if (todayFiles.Count > 0)
            {
                MoveTodaysFiles(todayFiles);
              
                SendToReplayFolder();
                Clean(holdingarea);
                Clean(extractPath);
            }
            
         
        }

        public void MoveTodaysFiles(List<FileInfo> todayFiles)
        {
            Console.WriteLine("Moving Files");

            if (Directory.Exists(downloadsPath) && Directory.Exists(extractPath))
            {
                foreach (FileInfo FI in todayFiles)
                {
                    string zipExtension = ".zip";
                    string aoeExtension = ".aoe2record";

                    if (FI.Extension.Equals(zipExtension, StringComparison.OrdinalIgnoreCase))
                    {
                        // Construct the full path for the target file in the destination directory
                        //string hold = Path.Combine(holdingarea, FI.Name);
                        Console.WriteLine($"Move Zip File to {holdingarea}");

                      
                        // Move the file to the destination
                        // File.Move(FI., holdingarea);

                        if (!Directory.Exists(holdingarea))
                        {
                            Directory.CreateDirectory(holdingarea);
                        }

                        FI.MoveTo(Path.Combine(holdingarea, FI.Name), true);
                        Extract();
                    }
                    if (FI.Extension.Equals(aoeExtension, StringComparison.OrdinalIgnoreCase))
                    {
                        // Construct the full path for the target file in the destination directory
                        string targetFilePath = Path.Combine(extractPath, FI.Name);
                        Console.WriteLine($"Move aoe2record File to {targetFilePath}");
                        // Move the file to the destination
                        File.Move(FI.FullName, targetFilePath);
                    }
                }
            }
        }


        public void Extract()
        {
            Console.WriteLine("Extracting...");
            string[] files = Directory.GetFiles(holdingarea);
            //string extractionDestination = @"C:\Users\curti\OneDrive\Desktop\New folder (2)";

            //// Create the destination directory if it doesn't exist
            //if (!Directory.Exists(extractionDestination))
            //{
            //    Directory.CreateDirectory(extractionDestination);
            //}

            foreach (string file in files)
            {
                try
                {
                    if (File.Exists(file))
                    {
                        // Extract to the specified destination
                        ZipFile.ExtractToDirectory(file, extractPath);
                        Console.WriteLine($"File extracted: {file}");
                    }
                    else
                    {
                        Console.WriteLine($"Source file does not exist: {file}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error extracting {file}: {ex.Message}");
                }
            }
        }


        public void SendToReplayFolder()
        {
            string[] files = Directory.GetFiles(extractPath);
            string targetExtension = ".aoe2record";


            foreach (string file in files)
            {
                if (Path.GetExtension(file).Equals(targetExtension, StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        // Combine the destination directory with the source file name to get the full destination path
                        string destinationFilePath = Path.Combine(destinationDirectory, Path.GetFileName(file));

                        // Move the file
                        if (!File.Exists(destinationFilePath))
                        {
                            File.Move(file, destinationFilePath);
                        }
                        
   
                       

                        Console.WriteLine($"File moved from {file} to {destinationFilePath}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                    Console.WriteLine(file);
                }
            }
        }

        public void Clean(string path)
        {

            try
            {
                // Create a DirectoryInfo object for the directory
                DirectoryInfo directoryInfo = new DirectoryInfo(path);

                // Get all files in the directory
                FileInfo[] files = directoryInfo.GetFiles();

                // Delete each file
                foreach (FileInfo file in files)
                {
                    try
                    {
                        file.Delete();
                        Console.WriteLine($"File deleted: {file.FullName}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deleting {file.FullName}: {ex.Message}");
                    }
                }

                Console.WriteLine("All files deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

    }
}
