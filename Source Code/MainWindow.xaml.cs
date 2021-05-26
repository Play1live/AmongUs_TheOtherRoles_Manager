using AmongUsModManager_V3;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AmongUsTheOtherRolesManager
{

    public partial class MainWindow : Window
    {
        public static string configPath = @"C:\Users\Public\Documents\AmongUs_TheOtherRolesMod_Manager\Config.txt";

        //Config Inhalte
        public static string[] vars = new string[5];
        public static string managerVersion = "7";
        public static string managerUpdaterPath = @"C:\Users\Public\Documents\AmongUs_TheOtherRolesMod_Manager\Updater.exe";
        public static string modVersion = "Nicht gefunden";
        public static string modSpeicherpfad = @"Nicht gefunden";
        public static string modDateiEXE = "Nicht gefunden";

        public static string modVersionOnline = "Nicht geladen";
        public static string managerVersionOnline = "Nicht geladen";

        public static List<string> logList = new List<string>();


        public MainWindow()
        {
            InitializeComponent();

            // Manager wird auf neue Version überprüft
            lblManagerVersion.Content = ManagerVersionUtil.newManagerVersion();

            // load Config
            ConfigUtil configutil = new ConfigUtil();
            saveToConfig(configutil.LoadConfig());

            // Install Updater
            installUpdater();

            // Aktuellste Version laden
            ModVersionUtil modVersionUtil = new ModVersionUtil();
            saveOnlineVersion(modVersionUtil.loadOnlineVersion());

            // Anzeigen aktualisieren
            aktualisiereAnzeigen();

            // Aktualisiert die Buttons
            btnAktivate();

            // Log wird aktualisiert.
            aktualisiereLog();


            // Autoupdater
            int mOnline = Convert.ToInt32(managerVersionOnline);
            int mlocal= Convert.ToInt32(managerVersion);
            if (mOnline > mlocal)
            {
                log.Items.Add("Stelle sicher, dass eine Internetverbindung hergestellt ist.");
                ManagerVersionUtil.startAutoUpdate();
            }
        }

        public void btnAktivate()
        {

            if (modSpeicherpfad == "Nicht gefunden")
            {
                btnStartGame.IsEnabled = false;
                btnUpdate.IsEnabled = false;
                btnNewPath.IsEnabled = true;
                return;
            }

            if (modVersion != modVersionOnline)
            {
                btnStartGame.IsEnabled = false;
                btnUpdate.IsEnabled = true;
                btnNewPath.IsEnabled = true;
                return;
            }

            if (modVersion == modVersionOnline)
            {
                btnStartGame.IsEnabled = true;
                btnUpdate.IsEnabled = true;
                btnNewPath.IsEnabled = true;
                return;
            }
        }

        public void installUpdater()
        {
            FileInfo updater = new FileInfo(managerUpdaterPath);
            if (!updater.Exists)
            {
                try
                {
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFile("https://github.com/Play1live/AmongUs_TheOtherRoles_Manager/releases/download/" + managerVersionOnline + "/AutoUpdater.exe", managerUpdaterPath);
                        log.Items.Add("Der Updater wurde installiert.");
                    }
                }
                catch
                {
                    log.Items.Add("Der Updater konnte nicht heruntergeladen werden.");
                }
            }
        }

        public void aktualisiereLog()
        {
            foreach (string i in logList)
            {
                log.Items.Add(i);
            }
        }

        public void aktualisiereAnzeigen()
        {
            lblVersions.Content = "Verfügbar: " + modVersionOnline + "\nInstalliert: " + modVersion;
            lblPath.Content = modSpeicherpfad;
        }

        public void saveOnlineVersion(string ver)
        {
            modVersionOnline = ver;
        }

        public void saveToConfig(string[] temp)
        {
            modVersion = temp[2];
            modSpeicherpfad = temp[3];
            modDateiEXE = temp[4];
        }

        public static void addLog(string msg)
        {
            logList.Add(msg);
        }

        private void btnStartGame_Click(object sender, RoutedEventArgs e)
        {
            log.Items.Add("Spiel wird gestartet.");
            addLog("Spiel wird gestartet.");
            Process.Start(modSpeicherpfad + @"\" + modDateiEXE);
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            ModVersionUtil.deleteOldFiles();
            ModVersionUtil mversion = new ModVersionUtil();
            saveOnlineVersion(mversion.loadOnlineVersion());

            /*if (modVersionOnline.Equals(modVersion))
            {
                log.Items.Add(modVersionOnline + " ist die aktuellste Version.");
                return;
            }*/

            mversion.checkForDownload();

            aktualisiereAnzeigen();
            btnAktivate();
            log.Items.Add("Mod wurde heruntergeladen.");
        }

        private void btnNewPath_Click(object sender, RoutedEventArgs e)
        {
            addLog("Der Speicherpfad wird gesucht. (Bitte warte!)");
            SpeicherpfadUtil speicherpfad = new SpeicherpfadUtil();
            speicherpfad.start();
            ConfigUtil configUtil = new ConfigUtil();
            configUtil.saveConfig();
            aktualisiereAnzeigen();

            log.Items.Add("Der Speicherpfad wurde aktualisiert.");
            btnUpdate.IsEnabled = true;
        }
    }
}
