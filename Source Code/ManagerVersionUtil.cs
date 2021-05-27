using AmongUsTheOtherRolesManager;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace AmongUsModManager_V3
{
    class ManagerVersionUtil
    {
        public static void startAutoUpdate()
        {
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string pid = Process.GetCurrentProcess().Id.ToString();
            string link = "https://github.com/Play1live/AmongUs_TheOtherRoles_Manager/releases/download/" + MainWindow.managerVersionOnline + "/AmongUsModManager.exe";

            Process.Start(@"H:\Programmierung\C#\Visual Studio\AmongUsAutoUpdater\AutoUpdater\bin\Debug\netcoreapp3.1\AutoUpdater.exe", "\"" + path + "\"" + " " + "\"" + fileName + "\"" + " " + "\"" + pid + "\"" + " " + "\"" + link + "\"");
        }


        public static string newManagerVersion()
        {
            try
            {
                string url = "https://github.com/Play1live/AmongUs_TheOtherRoles_Manager/releases";
                WebClient webclient = new WebClient();
                byte[] response = webclient.DownloadData(url);
                string webcode = Encoding.ASCII.GetString(response);

                string temp1 = webcode.Replace("Play1live/AmongUs_TheOtherRoles_Manager/tree/", " |").Split('|')[1];
                string version = temp1.Replace("\" class=", "|").Split('|')[0];

                int onlineV = Int32.Parse(version);
                int managerV = Int32.Parse(MainWindow.managerVersion);

                MainWindow.managerVersionOnline = onlineV+"";

                if (managerV < onlineV)
                {
                    MainWindow.addLog("Eine neue Version des Managers ist verfügbar. (v"+ onlineV + ")");
                    return "Eine neue Version des Managers ist verfügbar. (v" + onlineV + ")";
                }
                else
                {
                    MainWindow.addLog("Manager ist aktuell. (v" + managerV + ")");
                    return "Manager ist aktuell. (v" + managerV + ")";
                }

            }
            catch
            {
                MainWindow.addLog("Version konnte nicht überprüft werden.");
                return "Version konnte nicht überprüft werden.";
            }
        }

    }
}