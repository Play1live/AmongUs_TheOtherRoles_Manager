using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO.Compression;
using System.Threading;

namespace AmongUsTheOtherRolesManager
{
    class ModVersionUtil
    {
        public static void deleteOldFiles()
        {
            FileInfo zildic = new FileInfo(MainWindow.modSpeicherpfad+@"\ZIP");
            if (zildic.Exists)
            {
                DeleteFilesAndFoldersRecursively(MainWindow.modSpeicherpfad + @"\ZIP");
            }
            FileInfo modzip = new FileInfo(MainWindow.modSpeicherpfad + @"\Mod.zip");
            if (modzip.Exists)
            {
                modzip.Delete();
            }

        }

        public string loadOnlineVersion()
        {
            try
            {
                string url = "https://github.com/Eisbison/TheOtherRoles/releases";
                WebClient webclient = new WebClient();
                byte[] response = webclient.DownloadData(url);
                string webcode = Encoding.ASCII.GetString(response);

                string temp1 = webcode.Replace("Eisbison/TheOtherRoles/tree/", "|").Split('|')[1];
                string version = temp1.Replace("\" class=", "|").Split('|')[0];

                MainWindow.addLog("Verfügbare Version wurde geladen. " + version);
                return version;
            }
            catch
            {
                MainWindow.addLog("Verfügbare Version konnte nicht geladen werden.");
                return "ERROR";
            }
        }

        public void checkForDownload()
        {
            if (MainWindow.modVersion != MainWindow.modVersionOnline)
            {
                MainWindow.addLog("Datei wird heruntergeladen.");
                download();
            }
            // Force
            else
            {
                MainWindow.addLog("Datei wird heruntergeladen.");
                download();
            }
        }

        public void download()
        {
            try
            {
                WebClient client = new WebClient();
                client.DownloadFile(new Uri("https://github.com/Eisbison/TheOtherRoles/releases/download/" + MainWindow.modVersionOnline + "/TheOtherRoles.zip"), MainWindow.modSpeicherpfad + @"\Mod.zip");
                client.Dispose();
                
                MainWindow.addLog("Datei wurde heruntergeladen.");
            } catch
            {
                MainWindow.addLog("Datei konnte nicht heruntergeladen werden.");
                return;
            }

            
            zipEntpacken();
            DirectoryCopy(MainWindow.modSpeicherpfad+@"\ZIP", MainWindow.modSpeicherpfad, true);
            DeleteFilesAndFoldersRecursively(MainWindow.modSpeicherpfad + @"\ZIP");
            
            MainWindow.modVersion = MainWindow.modVersionOnline;
            ConfigUtil config = new ConfigUtil();
            config.saveConfig();
            MainWindow.addLog("Datei wurde entpackt.");
        }

        private void zipEntpacken()
        {
            try
            {
                ZipFile.ExtractToDirectory(MainWindow.modSpeicherpfad + @"\Mod.zip", MainWindow.modSpeicherpfad + @"\ZIP");
                MainWindow.addLog("Datei wurde entpackt");
            }
            catch
            {
                MainWindow.addLog("Datei konnte nicht entpackt werden");
            }
        }
        

        public void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            try
            {
                // Get the subdirectories for the specified directory.
                DirectoryInfo dir = new DirectoryInfo(sourceDirName);
                DirectoryInfo[] dirs = dir.GetDirectories();

                if (!dir.Exists)
                {
                    throw new DirectoryNotFoundException(
                        "Source directory does not exist or could not be found: "
                        + sourceDirName);
                }

                // If the destination directory doesn't exist, create it.
                if (!Directory.Exists(destDirName))
                {
                    Directory.CreateDirectory(destDirName);
                }

                // Get the files in the directory and copy them to the new location.
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    string temppath = Path.Combine(destDirName, file.Name);
                    file.CopyTo(temppath, true);
                }

                // If copying subdirectories, copy them and their contents to new location.
                if (copySubDirs)
                {
                    foreach (DirectoryInfo subdir in dirs)
                    {
                        string temppath = Path.Combine(destDirName, subdir.Name);
                        DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                    }
                }
            }
            catch
            {
                MainWindow.addLog("Ordner konnte nicht verschoben werden.");
            }
        }

        public static void DeleteFilesAndFoldersRecursively(string target_dir)
        {
            try {
                foreach (string file in Directory.GetFiles(target_dir))
                {
                    File.Delete(file);
                }

                foreach (string subDir in Directory.GetDirectories(target_dir))
                {
                    DeleteFilesAndFoldersRecursively(subDir);
                }

                Thread.Sleep(1); //This makes the difference between whether it works or not. Sleep(0) is not enough.
                Directory.Delete(target_dir);
            }
            catch
            {
                MainWindow.addLog("Alter Ordner konnte nicht gelöscht werden.");
            }
        }

    }

}
