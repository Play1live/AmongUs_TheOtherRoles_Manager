using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmongUsTheOtherRolesManager
{
    class SpeicherpfadUtil
    {

        public static Boolean crawlpath = false;

        public void start()
        {
            if (crawlpath == true)
            {
                MainWindow.addLog("Die Suche nach dem Speicherpfad läuft bereits.");
            }
            else
            {
                MainWindow.addLog("Speicherpfad wird gesucht.");
                crawlpath = true;
                listDrives();
            }
        }

        private void listDrives()
        {
            MainWindow.addLog("Speicherpfad wird gesucht.");
            // Listet die Festplatten auf
            string[] drives = System.Environment.GetLogicalDrives();

            foreach (string dr in drives)
            {
                System.IO.DriveInfo di = new System.IO.DriveInfo(dr);

                // Festplatten die nicht gelesen werden können werden geskippt.
                if (!di.IsReady)
                {
                    continue;
                }
                System.IO.DirectoryInfo rootDir = di.RootDirectory;

                WalkDirectoryTree(rootDir);
            }
        }

        public string[] blacklist = new string[] { "Windows", "Intel", "WUDownloadCache", "MountUUP", "WindowsApps", "Dell", "WinUpdate", "tmp", "Drivers", "W10UItemp" };

        private void WalkDirectoryTree(System.IO.DirectoryInfo root)
        {
            System.IO.FileInfo[] files = null;
            System.IO.DirectoryInfo[] subDirs = null;

            // First, process all the files directly under this folder
            try
            {
                files = root.GetFiles("*.*");

            }
            catch { }

            if (files != null)
            {
                // Now find all the subdirectories under this directory.
                subDirs = root.GetDirectories();

                foreach (System.IO.DirectoryInfo dirInfo in subDirs)
                {
                    // Skippt die Windows etc. Ordner
                    if (blacklist.Contains(dirInfo.Name))
                    {
                        continue;
                    }

                    // Find Updater.exe
                    if (dirInfo.FullName.Contains("Steam\\steamapps\\common"))
                    {

                        foreach (System.IO.FileInfo fi in files)
                        {
                            if (fi.Name.Equals("Modfinder.exe"))
                            {
                                MainWindow.modSpeicherpfad = fi.DirectoryName;
                                MainWindow.modDateiEXE = "Among Us.exe";

                                MainWindow.addLog("Filename: " + fi.Name);
                                MainWindow.addLog("DirectoryName: " + fi.DirectoryName);
                                MainWindow.addLog("FilenFullNameame: " + fi.FullName);

                                crawlpath = false;
                                return;
                            }
                        }
                    }
                    // Resursive call for each subdirectory.
                    WalkDirectoryTree(dirInfo);
                }
            }
        }
    }
}