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

        public static string forceUpdateUpdaterVersion = "10";

        //Config Inhalte
        public static string[] vars = new string[5];
        public static string managerVersion = "10";
        public static string managerVersionConf = "Nicht gefunden";
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
            try
            {
                int mOnline = Convert.ToInt32(managerVersionOnline);
                int mlocal = Convert.ToInt32(managerVersion);

                if (mOnline > mlocal)
                {
                    log.Items.Add("Stelle sicher, dass eine Internetverbindung hergestellt ist.");
                    ManagerVersionUtil.startAutoUpdate();
                }
            } 
            catch
            {
                btnStartGame.IsEnabled = true;
                btnUpdate.IsEnabled = false;
                btnModSteamUpdate.IsEnabled = false;
                btnNewPath.IsEnabled = true;
                log.Items.Add("Du kannst aktuell kein Update installieren.");
                log.Items.Add("Stelle sicher, dass eine Internetverbindung hergestellt ist.");
            }
        }

        public void btnAktivate()
        {

            if (modSpeicherpfad == "Nicht gefunden")
            {
                btnStartGame.IsEnabled = false;
                btnUpdate.IsEnabled = false;
                btnModSteamUpdate.IsEnabled = false;
                btnNewPath.IsEnabled = true;
                return;
            }

            if (modVersion != modVersionOnline)
            {
                btnStartGame.IsEnabled = false;
                btnUpdate.IsEnabled = true;
                btnModSteamUpdate.IsEnabled = true;
                btnNewPath.IsEnabled = true;
                return;
            }

            if (modVersion == modVersionOnline)
            {
                btnStartGame.IsEnabled = true;
                btnUpdate.IsEnabled = true;
                btnModSteamUpdate.IsEnabled = true;
                btnNewPath.IsEnabled = true;
                return;
            }
        }

        public void installUpdater()
        {
            FileInfo updater = new FileInfo(managerUpdaterPath);
            int fuuv = 0;
            int fvc = 0;
            try
            {
                fuuv = Int32.Parse(forceUpdateUpdaterVersion);
                fvc = Int32.Parse(managerVersionConf);
            }
            catch
            {
                log.Items.Add("Der Updater konnte nicht installiert werden.");
            }

            if (!updater.Exists || fuuv > fvc)
            {
                if (updater.Exists)
                {
                    updater.Delete();
                }
                try
                {
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFile("https://github.com/Play1live/AmongUs_TheOtherRoles_Manager/releases/download/" + managerVersionOnline + "/AutoUpdater.exe", managerUpdaterPath);
                        ConfigUtil c = new ConfigUtil();
                        c.saveConfig();
                        log.Items.Add("Der Updater wurde installiert.");
                    }
                }
                catch
                {
                    log.Items.Add("Der Updater konnte nicht heruntergeladen werden.");
                    log.Items.Add("Stelle sicher, dass eine Internetverbindung verfügbar ist.");
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
            managerVersionConf = temp[0];
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

        private void btnModSteamUpdate_Click(object sender, RoutedEventArgs e)
        {
            ModVersionUtil mversion = new ModVersionUtil();
            saveOnlineVersion(mversion.loadOnlineVersion());

            Directory.Delete(modSpeicherpfad, true);
            mversion.cloneSteamVersion();
            mversion.downloadModfinder();

            mversion.checkForDownload();

            aktualisiereAnzeigen();
            btnAktivate();
            log.Items.Add("Mod wurde heruntergeladen.");
        }
    }
}
