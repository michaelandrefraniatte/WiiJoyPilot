using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.Numerics;
using Microsoft.Win32.SafeHandles;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.Security.Principal;
using System.Management;
using System.Security.Cryptography;
using wjp;
using System.Windows.Threading;
using System.Reflection;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Security.Policy;

namespace WiiJoyPilot
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    [System.Security.SuppressUnmanagedCodeSecurity]
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            if (!hasAdminRights())
            {
                RunElevated();
                this.Close();
            }
            InitializeComponent();
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            ExtractResourceToFile("MotionInputPairing");
            ExtractResourceToFile("lhidread");
            System.Windows.Application.Current.MainWindow.Loaded += MainWindow_Loaded;
            this.KeyDown += MainWindow_KeyDown;
        }
        private void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            OnKeyDown(e.Key);
        }
        private void OnKeyDown(System.Windows.Input.Key keyData)
        {
            if (keyData == System.Windows.Input.Key.F1)
            {
                const string message = "• Author: Michaël André Franiatte.\n\r\n\r• Contact: michael.franiatte@gmail.com.\n\r\n\r• Publisher: https://github.com/michaelandrefraniatte.\n\r\n\r• Copyrights: All rights reserved, no permissions granted.\n\r\n\r• License: Not open source, not free of charge to use.";
                const string caption = "About";
                MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            if (keyData == System.Windows.Input.Key.Escape)
            {
                this.Close();
            }
        }
        public Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string dllName = args.Name.Contains(",") ? args.Name.Substring(0, args.Name.IndexOf(",")) : args.Name.Replace(".dll", "");
            dllName = dllName.Replace(".", "_");
            if (dllName.EndsWith("_resources"))
                return null;
            System.Resources.ResourceManager rm = new System.Resources.ResourceManager(GetType().Namespace + ".Properties.Resources", Assembly.GetExecutingAssembly());
            byte[] bytes = (byte[])rm.GetObject(dllName);
            return Assembly.Load(bytes);
        }
        public void ExtractResourceToFile(string dllName)
        {
            dllName = dllName.Replace(".", "_");
            System.Resources.ResourceManager rm = new System.Resources.ResourceManager(GetType().Namespace + ".Properties.Resources", Assembly.GetExecutingAssembly());
            byte[] bytes = (byte[])rm.GetObject(dllName);
            dllName = dllName.Replace("_", ".");
            using (FileStream fs = new FileStream(@"C:\Windows\System32\" + dllName + ".dll", FileMode.Create))
            {
                fs.Write(bytes, 0, bytes.Length);
            }
            using (FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + dllName + ".dll", FileMode.Create))
            {
                fs.Write(bytes, 0, bytes.Length);
            }
        }
        public static bool hasAdminRights()
        {
            WindowsPrincipal principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        public static void RunElevated()
        {
            try
            {
                ProcessStartInfo processInfo = new ProcessStartInfo();
                processInfo.Verb = "runas";
                processInfo.FileName = Application.ExecutablePath;
                Process.Start(processInfo);
            }
            catch { }
        }
        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
        private static extern uint TimeBeginPeriod(uint ms);
        [DllImport("winmm.dll", EntryPoint = "timeEndPeriod")]
        private static extern uint TimeEndPeriod(uint ms);
        [DllImport("ntdll.dll", EntryPoint = "NtSetTimerResolution")]
        private static extern void NtSetTimerResolution(uint DesiredResolution, bool SetResolution, ref uint CurrentResolution);
        private static uint CurrentResolution = 0;
        public static int irmode = 1;
        private static bool running;
        public static string filename = "";
        private void bopen_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Filter = "All Files(*.*)|*.*";
            if (op.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filename = op.FileName;
                OpenConfig(filename);
            }
        }
        private void bsave_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SaveConfig(filename);
        }
        private void bsaveas_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "All Files(*.*)|*.*";
            if (filename != "")
                sf.FileName = System.IO.Path.GetFileName(filename);
            if (sf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filename = sf.FileName;
                SaveConfig(filename);
            }
        }
        private void SaveConfig(string path)
        {
            try
            {
                using (StreamWriter file = new StreamWriter(path))
                {
                    file.WriteLine(cbback1.SelectionBoxItem.ToString());
                    file.WriteLine(cbback2.SelectionBoxItem.ToString());
                    file.WriteLine(cbstart1.SelectionBoxItem.ToString());
                    file.WriteLine(cbstart2.SelectionBoxItem.ToString());
                    file.WriteLine(cba1.SelectionBoxItem.ToString());
                    file.WriteLine(cba2.SelectionBoxItem.ToString());
                    file.WriteLine(cbb1.SelectionBoxItem.ToString());
                    file.WriteLine(cbb2.SelectionBoxItem.ToString());
                    file.WriteLine(cbx1.SelectionBoxItem.ToString());
                    file.WriteLine(cbx2.SelectionBoxItem.ToString());
                    file.WriteLine(cby1.SelectionBoxItem.ToString());
                    file.WriteLine(cby2.SelectionBoxItem.ToString());
                    file.WriteLine(cbup1.SelectionBoxItem.ToString());
                    file.WriteLine(cbup2.SelectionBoxItem.ToString());
                    file.WriteLine(cbleft1.SelectionBoxItem.ToString());
                    file.WriteLine(cbleft2.SelectionBoxItem.ToString());
                    file.WriteLine(cbdown1.SelectionBoxItem.ToString());
                    file.WriteLine(cbdown2.SelectionBoxItem.ToString());
                    file.WriteLine(cbright1.SelectionBoxItem.ToString());
                    file.WriteLine(cbright2.SelectionBoxItem.ToString());
                    file.WriteLine(cbrightstick1.SelectionBoxItem.ToString());
                    file.WriteLine(cbrightstick2.SelectionBoxItem.ToString());
                    file.WriteLine(cbleftstick1.SelectionBoxItem.ToString());
                    file.WriteLine(cbleftstick2.SelectionBoxItem.ToString());
                    file.WriteLine(cbrightbumper1.SelectionBoxItem.ToString());
                    file.WriteLine(cbrightbumper2.SelectionBoxItem.ToString());
                    file.WriteLine(cbleftbumper1.SelectionBoxItem.ToString());
                    file.WriteLine(cbleftbumper2.SelectionBoxItem.ToString());
                    file.WriteLine(cbrighttrigger1.SelectionBoxItem.ToString());
                    file.WriteLine(cbrighttrigger2.SelectionBoxItem.ToString());
                    file.WriteLine(cblefttrigger1.SelectionBoxItem.ToString());
                    file.WriteLine(cblefttrigger2.SelectionBoxItem.ToString());
                    file.WriteLine(cbads.IsChecked);
                    file.WriteLine(tbstickxsens.Value);
                    file.WriteLine(tbstickysens.Value);
                    file.WriteLine(cbmove.IsChecked);
                    file.WriteLine(tbirxsens.Value);
                    file.WriteLine(tbirysens.Value);
                    file.WriteLine(tbdzx.Value);
                    file.WriteLine(tbdzy.Value);
                    file.WriteLine(tbviewpower1x.Value);
                    file.WriteLine(tbviewpower2x.Value);
                    file.WriteLine(tbviewpower3x.Value);
                    file.WriteLine(tbviewpower1y.Value);
                    file.WriteLine(tbviewpower2y.Value);
                    file.WriteLine(tbviewpower3y.Value);
                    file.WriteLine(tbcentery.Value);
                    file.WriteLine(cbirmode.IsChecked);
                }
                this.Title = "WiiJoyPilot: " + System.IO.Path.GetFileName(path);
            }
            catch { }
            Apply();
        }
        private void OpenConfig(string path)
        {
            try
            {
                using (StreamReader file = new StreamReader(path))
                {
                    cbback1.SelectedIndex = SelectIndex(file.ReadLine());
                    cbback2.SelectedIndex = SelectIndex(file.ReadLine());
                    cbstart1.SelectedIndex = SelectIndex(file.ReadLine());
                    cbstart2.SelectedIndex = SelectIndex(file.ReadLine());
                    cba1.SelectedIndex = SelectIndex(file.ReadLine());
                    cba2.SelectedIndex = SelectIndex(file.ReadLine());
                    cbb1.SelectedIndex = SelectIndex(file.ReadLine());
                    cbb2.SelectedIndex = SelectIndex(file.ReadLine());
                    cbx1.SelectedIndex = SelectIndex(file.ReadLine());
                    cbx2.SelectedIndex = SelectIndex(file.ReadLine());
                    cby1.SelectedIndex = SelectIndex(file.ReadLine());
                    cby2.SelectedIndex = SelectIndex(file.ReadLine());
                    cbup1.SelectedIndex = SelectIndex(file.ReadLine());
                    cbup2.SelectedIndex = SelectIndex(file.ReadLine());
                    cbleft1.SelectedIndex = SelectIndex(file.ReadLine());
                    cbleft2.SelectedIndex = SelectIndex(file.ReadLine());
                    cbdown1.SelectedIndex = SelectIndex(file.ReadLine());
                    cbdown2.SelectedIndex = SelectIndex(file.ReadLine());
                    cbright1.SelectedIndex = SelectIndex(file.ReadLine());
                    cbright2.SelectedIndex = SelectIndex(file.ReadLine());
                    cbrightstick1.SelectedIndex = SelectIndex(file.ReadLine());
                    cbrightstick2.SelectedIndex = SelectIndex(file.ReadLine());
                    cbleftstick1.SelectedIndex = SelectIndex(file.ReadLine());
                    cbleftstick2.SelectedIndex = SelectIndex(file.ReadLine());
                    cbrightbumper1.SelectedIndex = SelectIndex(file.ReadLine());
                    cbrightbumper2.SelectedIndex = SelectIndex(file.ReadLine());
                    cbleftbumper1.SelectedIndex = SelectIndex(file.ReadLine());
                    cbleftbumper2.SelectedIndex = SelectIndex(file.ReadLine());
                    cbrighttrigger1.SelectedIndex = SelectIndex(file.ReadLine());
                    cbrighttrigger2.SelectedIndex = SelectIndex(file.ReadLine());
                    cblefttrigger1.SelectedIndex = SelectIndex(file.ReadLine());
                    cblefttrigger2.SelectedIndex = SelectIndex(file.ReadLine());
                    cbads.IsChecked = bool.Parse(file.ReadLine());
                    tbstickxsens.Value = Convert.ToInt32(file.ReadLine());
                    tbstickysens.Value = Convert.ToInt32(file.ReadLine());
                    cbmove.IsChecked = bool.Parse(file.ReadLine());
                    tbirxsens.Value = Convert.ToInt32(file.ReadLine());
                    tbirysens.Value = Convert.ToInt32(file.ReadLine());
                    tbdzx.Value = Convert.ToInt32(file.ReadLine());
                    tbdzy.Value = Convert.ToInt32(file.ReadLine());
                    tbviewpower1x.Value = Convert.ToInt32(file.ReadLine());
                    tbviewpower2x.Value = Convert.ToInt32(file.ReadLine());
                    tbviewpower3x.Value = Convert.ToInt32(file.ReadLine());
                    tbviewpower1y.Value = Convert.ToInt32(file.ReadLine());
                    tbviewpower2y.Value = Convert.ToInt32(file.ReadLine());
                    tbviewpower3y.Value = Convert.ToInt32(file.ReadLine());
                    tbcentery.Value = Convert.ToInt32(file.ReadLine());
                    cbirmode.IsChecked = bool.Parse(file.ReadLine());
                }
                this.Title = "WiiJoyPilot: " + System.IO.Path.GetFileName(path);
            }
            catch { }
            Apply();
        }
        private int SelectIndex(string item)
        {
            int index = 0;
            if (item == "")
                index = 0;
            if (item == "JoyconLeftButtonDPAD_DOWN")
                index = 1;
            if (item == "JoyconLeftButtonDPAD_LEFT")
                index = 2;
            if (item == "JoyconLeftButtonDPAD_RIGHT")
                index = 3;
            if (item == "JoyconLeftButtonDPAD_UP")
                index = 4;
            if (item == "JoyconLeftButtonMINUS")
                index = 5;
            if (item == "JoyconLeftButtonACC")
                index = 6;
            if (item == "JoyconLeftButtonSHOULDER_1")
                index = 7;
            if (item == "JoyconLeftButtonSHOULDER_2")
                index = 8;
            if (item == "JoyconLeftButtonCAPTURE")
                index = 9;
            if (item == "WiimoteButtonStateOne")
                index = 10;
            if (item == "WiimoteButtonStateTwo")
                index = 11;
            if (item == "WiimoteButtonStateDown")
                index = 12;
            if (item == "WiimoteButtonStateLeft")
                index = 13;
            if (item == "WiimoteButtonStateRight")
                index = 14;
            if (item == "WiimoteButtonStateUp")
                index = 15;
            if (item == "WiimoteButtonStateHome")
                index = 16;
            if (item == "WiimoteButtonACC")
                index = 17;
            if (item == "WiimoteButtonStateA")
                index = 18;
            if (item == "WiimoteButtonStateB")
                index = 19;
            if (item == "WiimoteButtonStatePlus")
                index = 20;
            if (item == "WiimoteButtonStateMinus")
                index = 21;
            return index;
        }
        private void bapply_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Apply();
        }
        private void Apply()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                wjp.Class1.Apply(cbback1.SelectionBoxItem.ToString(), cbback2.SelectionBoxItem.ToString(), cbstart1.SelectionBoxItem.ToString(), cbstart2.SelectionBoxItem.ToString(), cba1.SelectionBoxItem.ToString(), cba2.SelectionBoxItem.ToString(), cbb1.SelectionBoxItem.ToString(), cbb2.SelectionBoxItem.ToString(), cbx1.SelectionBoxItem.ToString(), cbx2.SelectionBoxItem.ToString(), cby1.SelectionBoxItem.ToString(), cby2.SelectionBoxItem.ToString(), cbup1.SelectionBoxItem.ToString(), cbup2.SelectionBoxItem.ToString(), cbleft1.SelectionBoxItem.ToString(), cbleft2.SelectionBoxItem.ToString(), cbdown1.SelectionBoxItem.ToString(), cbdown2.SelectionBoxItem.ToString(), cbright1.SelectionBoxItem.ToString(), cbright2.SelectionBoxItem.ToString(), cbrightstick1.SelectionBoxItem.ToString(), cbrightstick2.SelectionBoxItem.ToString(), cbleftstick1.SelectionBoxItem.ToString(), cbleftstick2.SelectionBoxItem.ToString(), cbrightbumper1.SelectionBoxItem.ToString(), cbrightbumper2.SelectionBoxItem.ToString(), cbleftbumper1.SelectionBoxItem.ToString(), cbleftbumper2.SelectionBoxItem.ToString(), cbrighttrigger1.SelectionBoxItem.ToString(), cbrighttrigger2.SelectionBoxItem.ToString(), cblefttrigger1.SelectionBoxItem.ToString(), cblefttrigger2.SelectionBoxItem.ToString(), bool.Parse(cbirmode.IsChecked.ToString()) ? 2 : 1, (double)tbcentery.Value, (bool)cbads.IsChecked, (bool)cbmove.IsChecked, (double)tbirxsens.Value / 10f, (double)tbirysens.Value / 10f, (double)tbstickxsens.Value / 10f, (double)tbstickysens.Value / 10f, (double)tbviewpower1x.Value, (double)tbviewpower2x.Value, (double)tbviewpower3x.Value, (double)tbviewpower1y.Value, (double)tbviewpower2y.Value, (double)tbviewpower3y.Value, (double)tbdzx.Value, (double)tbdzy.Value);
            }));
        }
        private void MainWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            TimeBeginPeriod(1);
            NtSetTimerResolution(1, true, ref CurrentResolution);
            if (System.IO.File.Exists("tempsave"))
            {
                using (System.IO.StreamReader file = new System.IO.StreamReader("tempsave"))
                {
                    filename = file.ReadLine();
                }
                if (filename != "" & System.IO.File.Exists(filename))
                {
                    OpenConfig(filename);
                }
                else 
                    Apply();
            }
            else
                Apply();
            System.Windows.Application.Current.MainWindow.Closed += MainWindow_Closed;
            running = true;
            Dispatcher.Invoke(new Action(() =>
            {
                Task.Run(() => wjp.Class1.Start());
                Task.Run(() => taskX());
            }));
        }
        private void taskX()
        {
            while (running)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    try
                    {
                        wjp.Class1.taskB();
                        wjp.Class1.taskX();
                    }
                    catch { }
                }));
                Thread.Sleep(1);
            }
        }
        private void MainWindow_Closed(object sender, EventArgs e)
        {
            try
            {
                if (filename != "")
                {
                    using (StreamWriter createdfile = new StreamWriter("tempsave"))
                    {
                        createdfile.WriteLine(filename);
                    }
                }
                running = false;
                wjp.Class1.Close();
            }
            catch { }
        }
    }
}