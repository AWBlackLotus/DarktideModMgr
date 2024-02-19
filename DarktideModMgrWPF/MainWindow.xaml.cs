using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Runtime.InteropServices;

namespace DarktideModMgrWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        static string RootPath = Environment.CurrentDirectory;
        string inipath = RootPath + "\\Config.ini",Local, WorkPath, ModCountstr, commandline = "", launcherpath = "\\launcher\\Launcher.exe", dateformat, now;
        string[] ModFolder, FArr;
        string[] ModList = new string[] {}, ModList1 = new string[] {}, ModList0 = new string[] {}, LanguageList = new string[] {};
        Dictionary<string, string> Localtxtd = new Dictionary<string, string>();
        static string[] Localtext = { "Error", "Warning", "Auth", "Info", "OK","Cancel",
            "WrongPath", "NoModFolder", "NoMod", "tMainDes", "lLoaclModsDes", "lPrepareModsDes",
            "tModsDes","bPatchDes", "bStartDes", "WrongModSelect", "bAddAllDes", "bRemoveAllDes", 
            "bSaveDes", "bRefreshDes", "rFolderDes", "rTxtDes", "Writesucc", "gGListDes",
            "gGRadioDes", "gGModDes", "NoModSelected", "SaveTipsTitle", "SaveTips", "tOtherDes",
            "gBakDes", "bRestoreDes", "bRestoreDes", "lBackupTitleDes", "tRestoresucc",
            "llangsettitleDes", "glanguagesDes","langchangedsuccDes","lModFound", "lModCount",
            "lfilterlabel","llastsave","lauthor","lspecialthanks", "lMainpage","reinstall"};
        int ModCount = 0;
        bool isSaved = true, esave = false;
        private void APP_Load(object sender, RoutedEventArgs e)
        {
            Localization();
            IniDetect();
            PathDetect();
            Backupdetect();
            Lastsave();
        }

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);


        public string ConfigRead(string Section, string Key, string inipath)
        {
            StringBuilder temp = new StringBuilder(1024);
            GetPrivateProfileString(Section, Key, "", temp, 1024, inipath);
            return temp.ToString();
        }
        public void ConfigWrite(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, inipath);
        }
        private void Localization()
        {
            languagelist.Items.Clear();
            string[] WPCache = RootPath.Split("\\");
            string[] WPCache1 = WPCache.Take(WPCache.Count() - 1).ToArray();
            string simplefilename = "";
            WorkPath = string.Join("\\", WPCache1);
            Local = ConfigRead("Settings", "Local", inipath);
            if (!File.Exists(RootPath + "\\localization\\" + Local + ".ini"))
            {
                Local = "English";
            }
            try
            {
                LanguageList = Directory.GetFiles(RootPath + "\\localization\\");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "please reinstall.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                Environment.Exit(0);
            }
            try
            {
                foreach (string file in LanguageList)
                {
                    simplefilename = file.Replace(RootPath + "\\localization\\".ToString(), "");
                    simplefilename = simplefilename.Replace(".ini", "");
                    languagelist.Items.Add(simplefilename);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Localtxtd["Warning"], MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            foreach (string Localtxt in Localtext)
            {
                Localtxtd[Localtxt] = ConfigRead("Content", Localtxt, RootPath + "\\localization\\" + Local + ".ini");
            }
            languagelist.Text = Local;
            Mainpage.Header = Localtxtd["tMainDes"];
            Mods.Header = Localtxtd["tModsDes"];
            Other.Header = Localtxtd["tOtherDes"];
            LocalMods.Header = Localtxtd["lLoaclModsDes"];
            PrepareMods.Header = Localtxtd["lPrepareModsDes"];
            buttonAddAll.Content = Localtxtd["bAddAllDes"];
            buttonRemoveAll.Content = Localtxtd["bRemoveAllDes"];
            buttonSave.Content = Localtxtd["bSaveDes"];
            buttonPatchToggle.Content = Localtxtd["bPatchDes"];
            buttonStart.Content = Localtxtd["bStartDes"];
            buttonRefresh.Content = Localtxtd["bRefreshDes"];
            radioFolder.Content = Localtxtd["rFolderDes"];
            radioTxt.Content = Localtxtd["rTxtDes"];
            ListGroup.Header = Localtxtd["gGListDes"];
            RadioGroup.Header = Localtxtd["gGRadioDes"];
            ModGroup.Header = Localtxtd["gGModDes"];
            Backuprestore.Header = Localtxtd["gBakDes"];
            Restore.Content = Localtxtd["bRestoreDes"];
            BackupTitle.Content = Localtxtd["lBackupTitleDes"];
            language.Header = Localtxtd["glanguagesDes"];
            languageselecttitle.Content = Localtxtd["llangsettitleDes"];
            languageconfirm.Content = Localtxtd["OK"];
            lastsavelabel.Content = Localtxtd["llastsave"];
            authorlabel.Content = Localtxtd["lauthor"];
            specialthankslabel.Content = Localtxtd["lspecialthanks"];
            versionlabel.Content = "v" + GetVersion();
            if(ModCount > 0)
            {
                Statuslabel.Content = Localtxtd["lModFound"] + " " + ModCountstr + " " + Localtxtd["lModCount"];
            }
        }
        public static string GetVersion()
        {
            string version = Application.ResourceAssembly.GetName().Version.ToString();
            version = version.Substring(0, version.Length - 2);
            return version;
        }
        private void Hyperlink_blog(object sender, RequestNavigateEventArgs e)
        {
            Process.Start("explorer.exe","http://blog.crymc.cn");
            e.Handled = true;
        }
        private void Hyperlink_nexusmods(object sender, RequestNavigateEventArgs e)
        {
            Process.Start("explorer.exe", "https://www.nexusmods.com/warhammer40kdarktide");
            e.Handled = true;
        }

        private void IniDetect()
        {
            if (!File.Exists(inipath))
            {
                try
                {
                    FileStream FS1 = new FileStream(inipath, FileMode.CreateNew);
                    FS1.Close();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, Localtxtd["Error"], MessageBoxButton.OK, MessageBoxImage.Error);
                    Environment.Exit(0);
                }
                ConfigWrite("Settings", "Local", "English");
                ConfigWrite("Settings", "Dateformat", "yyyy-MM-dd-HH-mm-ss");
            }
        }
        private void PathDetect()
        {
            string DTPath = WorkPath + "\\binaries\\Darktide.exe";
            string LDPath = WorkPath + "\\tools\\dtkit-patch.exe";
            if (!File.Exists(DTPath) || !File.Exists(LDPath))
            {
                MessageBox.Show(Localtxtd["WrongPath"], Localtxtd["Error"], MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }
        }
        private void ModDetect()
        {
            if (isSaved == false)
            {
                MessageBoxButton SaveTipsbuttons = MessageBoxButton.YesNoCancel;
                MessageBoxImage SaveTipicon = MessageBoxImage.Question;
                MessageBoxResult SaveResult = MessageBox.Show(Localtxtd["SaveTips"], Localtxtd["SaveTipsTitle"], SaveTipsbuttons, SaveTipicon);
                if (SaveResult == MessageBoxResult.Yes)
                {
                    esave = true; //此开关为真时不会弹保存成功窗口
                    Savefile();
                    ModDetect();
                }
                else if(SaveResult == MessageBoxResult.No)
                {
                    isSaved = true;
                    ModDetect();
                }
                else
                {
                    return;
                }
            }
            else if(isSaved == true)
            {
                listView.Items.Clear();
                listView1.Items.Clear();
                if (radioFolder.IsChecked == true)
                {
                    string ModPath = WorkPath + "\\mods\\";
                    try
                    {
                        ModFolder = Directory.GetDirectories(ModPath);
                    }
                    catch
                    {
                        try
                        {
                            Directory.CreateDirectory(ModPath);
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show(ex.Message, Localtxtd["Error"], MessageBoxButton.OK, MessageBoxImage.Error);
                            Environment.Exit(0);
                        }
                    }
                    if (ModFolder.Length > 0)
                    {
                        IList ArrModFolder = new List<string>();
                        foreach (string Folder in ModFolder)
                        {
                            FArr = Folder.Split("mods\\");
                            if (FArr[1] != "dmf" && FArr[1] != "base" && FArr[1] != "")
                            {
                                ArrModFolder.Add(FArr[1]);
                            }
                        }
                        foreach (string Folder1 in ArrModFolder)
                        {
                            listView.Items.Add(Folder1);
                        }
                    }
                    else
                    {
                        MessageBox.Show(Localtxtd["NoMod"], Localtxtd["Warning"], MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                else if (radioTxt.IsChecked == true)
                {
                    string RPath = WorkPath + "\\mods\\mod_load_order.txt";
                    string[] ModList;
                    ModList = File.ReadAllLines(RPath);
                    foreach (string Mod in ModList)
                    {
                        if (Mod.Contains("--"))
                        {
                            string nominusMod = Mod.Replace("--",string.Empty);
                            listView.Items.Add(nominusMod);
                        }
                        else
                        {
                            listView1.Items.Add(Mod);
                        }
                    }
                }
            }
            ModCount = listView.Items.Count + listView1.Items.Count;
            try
            {
                ModCountstr = ModCount.ToString();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, Localtxtd["Warning"], MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Statuslabel.Content = Localtxtd["lModFound"] + " " + ModCountstr + " " + Localtxtd["lModCount"];
        }
        private void buttonAddAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (string listedmods in listView.Items) 
            {
                listView1.Items.Add(listedmods);
            }
            listView.Items.Clear();
            Notsaved();
        }
        private void buttonRemoveAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (string listedmods in listView1.Items)
            {
                listView.Items.Add(listedmods);
            }
            listView1.Items.Clear();
        }
        private void buttonRefresh_Click(object sender, RoutedEventArgs e)
        {
            ModDetect();
        }
        private void Savefile()
        {
            string[] mergedlist = new string[0] { }, finallist = new string[0] { };
            string WPath = WorkPath + "\\mods\\mod_load_order.txt";
            if (listView1.Items.Count > 0 && listView.Items.Count == 0)
            {
                Array.Resize(ref ModList, listView1.Items.Count);
                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    ModList[i] = listView1.Items[i].ToString();
                }
                try
                {
                    File.WriteAllLines(WPath, ModList);
                    Saved();
                    Backup();
                    Lastsave();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Localtxtd["Warning"], MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                if (esave == false)
                {
                    MessageBox.Show(Localtxtd["Writesucc"], Localtxtd["Info"], MessageBoxButton.OK, MessageBoxImage.Information);
                }
                esave = false;
            }
            else if(listView1.Items.Count > 0 && listView.Items.Count > 0)
            {
                Array.Resize(ref ModList0, listView.Items.Count);
                Array.Resize(ref ModList, listView1.Items.Count);
                for (int i = 0; i < listView.Items.Count; i++)
                {
                    ModList0[i] = listView.Items[i].ToString();
                    ModList0[i] = ModList0[i].Insert(0, "--");
                }
                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    ModList[i] = listView1.Items[i].ToString();
                }
                Array.Resize(ref mergedlist, listView1.Items.Count + listView.Items.Count);
                mergedlist = ModList0.Concat(ModList).ToArray();
                try
                {
                    File.WriteAllLines(WPath, mergedlist);
                    Saved();
                    Backup();
                    Lastsave();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Localtxtd["Warning"], MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (esave == false)
                {
                    MessageBox.Show(Localtxtd["Writesucc"], Localtxtd["Info"], MessageBoxButton.OK, MessageBoxImage.Information);
                }
                esave = false;
            }
            else
            {
                MessageBox.Show(Localtxtd["NoModSelected"], Localtxtd["Info"], MessageBoxButton.OK, MessageBoxImage.Information);
            }
            Backupdetect();
        }
        private void Lastsave()
        {
            string modorder = WorkPath + "\\mods\\mod_load_order.txt";
            if (!File.Exists(modorder))
            {
                try
                {
                    FileStream FS1 = new FileStream(modorder, FileMode.CreateNew);
                    FS1.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Localtxtd["Error"], MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            DateTime lastModified = File.GetLastWriteTime(modorder);
            lastsavelabelc.Content = lastModified;
        }
        private async void MoveDete(object sender, MouseEventArgs e)
        {
            await Task.Delay(500);
            Detector();
        }
        private void Detector()
        {
            if (listView1.Items.Count > 0)
            {
                Array.Resize(ref ModList1, listView1.Items.Count);
                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    ModList1[i] = listView1.Items[i].ToString();
                }
                if (ModList1.Length == ModList.Length)
                {
                    for (int i = 0; i < ModList.Length; i++)
                    {
                        if (ModList1[i] != ModList[i])
                        {
                            Notsaved();
                            break;
                        }
                    }
                }
                else
                {
                    Notsaved();
                }
            }
            else
            {
                Saved();
            };
        }
        private void Saved()
        {
            isSaved = true;
            Savetips.Content = " ";
        }
        private void Notsaved()
        {
            if(listView1.Items.Count != 0) 
            {
                isSaved = false;
                Savetips.Content = "*";
            }
        }
        private void buttonPatchToggle_Click(object sender, RoutedEventArgs e)
        {
            string databak = WorkPath + "\\bundle\\bundle_database.data.bak", toolpath = WorkPath + "\\tools\\dtkit-patch.exe"; ;
            if(File.Exists(databak))
            {
                commandline = "--unpatch .\\bundle";
            }
            else
            {
                commandline = "--patch .\\bundle";
            }
            using (Process p = new Process())
            {
                p.StartInfo.FileName = toolpath;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = true; 
                try
                {
                    p.Start();
                    p.StandardInput.WriteLine(commandline);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, Localtxtd["Warning"], MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                p.StandardInput.AutoFlush = false;
                p.WaitForExit();
                p.Close();
            }
        }
        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            string launcher = WorkPath + launcherpath;
            try
            {
                Process.Start(launcher);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Localtxtd["Warning"], MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }
        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            esave = false;
            Savefile();
        }
        private void Backup()
        {
            string backupfilename = now + ".txt", backuppath = RootPath + "\\backup\\", backupfullpath = backuppath + backupfilename;
            string txt = WorkPath + "\\mods\\mod_load_order.txt";
            dateformat = ConfigRead("Settings","Dateformat", inipath);
            if (!Directory.Exists(backuppath))
            {
                try
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(backuppath);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, Localtxtd["Warning"], MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            try
            {
                now = DateTime.Now.ToString(dateformat);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, Localtxtd["Warning"], MessageBoxButton.OK, MessageBoxImage.Warning);
                ConfigWrite("Settings", "Dateformat", "yyyy-MM-dd-HH-mm-ss");
                return;
            }
            if (!File.Exists(backupfullpath))
            {
                bool isrewrite = true;
                try
                {
                    File.Copy(txt, backupfullpath, isrewrite);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, Localtxtd["Warning"], MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
        }
        private void Backupdetect()
        {
            Backupflieslist.Items.Clear();
            string[] backupfiles = Directory.GetFiles(RootPath + "\\backup\\");
            string fileresult;
            try
            {
                foreach (string file in backupfiles)
                {
                    fileresult = file.Replace(RootPath + "\\backup\\".ToString(), "");
                    Backupflieslist.Items.Add(fileresult);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, Localtxtd["Warning"], MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void Restore_Click(object sender, RoutedEventArgs e)
        {
            Backuprestorefun();
        }
        private void Backuprestorefun()
        {
            string txtpath,selecttxt = "";
            if(Backupflieslist.SelectedItem != null)
            {
                selecttxt = Backupflieslist.SelectedItem.ToString();
                if (isSaved == false)
                {
                    MessageBoxButton BackupTipsbuttons = MessageBoxButton.YesNoCancel;
                    MessageBoxImage BackupTipicon = MessageBoxImage.Question;
                    MessageBoxResult BackupResult = MessageBox.Show(Localtxtd["SaveTips"], Localtxtd["SaveTipsTitle"], BackupTipsbuttons, BackupTipicon);
                    if (BackupResult == MessageBoxResult.Yes)
                    {
                        esave = true; //此开关为真时不会弹保存成功窗口
                        Savefile();
                        Backuprestorefun();
                    }
                    else if (BackupResult == MessageBoxResult.No)
                    {
                        isSaved = true;
                        Backuprestorefun();
                    }
                    else
                    {
                        return;
                    }
                }
                else if (isSaved == true)
                {
                    if (selecttxt != null)
                    {
                        txtpath = RootPath + "\\backup\\" + selecttxt;
                        try
                        {
                            File.Copy(txtpath, WorkPath + "\\mods\\mod_load_order.txt", true);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, Localtxtd["Warning"], MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        MessageBox.Show(Localtxtd["tRestoresucc"], Localtxtd["Info"], MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }
        private void languageconfirm_Click(object sender, RoutedEventArgs e)
        {
            string newlocal = "";
            if (languagelist.SelectedItem != null)
            {
                newlocal = languagelist.SelectedItem.ToString();
            }
            if (newlocal != Local && newlocal != "")
            {
                Local = newlocal;
                try
                {
                    ConfigWrite("Settings", "Local", Local);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Localtxtd["Warning"], MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                MessageBox.Show(Localtxtd["langchangedsuccDes"], Localtxtd["Info"], MessageBoxButton.OK, MessageBoxImage.Information);
                Localization();
            }
        }
    }
}

