using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmongUsTheOtherRolesManager
{
    class ConfigUtil
    {
        public void saveConfig()
        {
            FileInfo file = new FileInfo(MainWindow.configPath);
            if (!file.Exists)
            {
                MainWindow.addLog("Config konnte nicht gespeichert werden. (Existiert nicht)");
                return;
            }
            try
            {
                using (StreamWriter sw = new StreamWriter(file.FullName))
                {
                    sw.WriteLine("Manager.Version#{0}", MainWindow.managerVersion);
                    sw.WriteLine("Manager.Updater.Path#{0}", MainWindow.managerUpdaterPath);
                    sw.WriteLine("Mod.Version#{0}", MainWindow.modVersion);
                    sw.WriteLine("Mod.Speicherpfad#{0}", MainWindow.modSpeicherpfad);
                    sw.WriteLine("Manager.DateiEXE#{0}", MainWindow.modDateiEXE);
                    sw.Close();
                    MainWindow.addLog("Config wurde geladen.");
                    return;
                }
            }
            catch
            {
                MainWindow.addLog("Config konnte nicht erstellt werden.");
                return;
            }
        }

        public string[] LoadConfig()
        {
            FileInfo file = new FileInfo(MainWindow.configPath);
            if (!file.Exists)
            {
                try
                {
                    // Create a new file     
                    using (StreamWriter sw = file.CreateText())
                    {
                        sw.WriteLine("Manager.Version#{0}", MainWindow.managerVersion);
                        sw.WriteLine("Manager.Updater.Path#{0}", MainWindow.managerUpdaterPath);
                        sw.WriteLine("Mod.Version#{0}", MainWindow.modVersion);
                        sw.WriteLine("Mod.Speicherpfad#{0}", MainWindow.modSpeicherpfad);
                        sw.WriteLine("Manager.DateiEXE#{0}", MainWindow.modDateiEXE);
                        sw.Close();
                        MainWindow.addLog("Config wurde geladen.");
                        return new string[] { MainWindow.managerVersion, MainWindow.managerUpdaterPath, MainWindow.modVersion, MainWindow.modSpeicherpfad, MainWindow.modDateiEXE };
                    }
                }
                catch
                {
                    MainWindow.addLog("Config konnte nicht erstellt werden.");
                    return null;
                }
            }
            else
            {
                try
                {
                    // Read file
                    using (StreamReader sr = new StreamReader(file.FullName))
                    {
                        string z1 = sr.ReadLine().Split('#')[1];
                        string z2 = sr.ReadLine().Split('#')[1];
                        string z3 = sr.ReadLine().Split('#')[1];
                        string z4 = sr.ReadLine().Split('#')[1];
                        string z5 = sr.ReadLine().Split('#')[1];
                        sr.Close();
                        MainWindow.addLog("Config wurde geladen.");
                        return new string[] { z1, z2, z3, z4, z5 };
                    }
                }
                catch
                {
                    MainWindow.addLog("Config konnte nicht geladen werden.");
                    return null;
                }
            }

        }
    }
}
