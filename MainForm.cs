using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.DataFormats;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Button = System.Windows.Forms.Button;
using Label = System.Windows.Forms.Label;
using MethodInvoker = System.Windows.Forms.MethodInvoker;

namespace ShipRight
{
    internal partial class MainForm : Form
    {
        private static MainForm form = null;
        private static Settings settingsForm;

        private readonly IConfiguration _configuration;
        private readonly IRunner _runner;
        private readonly IManualOverlay _manualOverlay;
        private readonly IPuzzler _puzzler;

        private readonly Hotkey _startHotkey;
        private readonly int _startHotkeyHashcode;

        private readonly Hotkey _stopHotkey;
        private readonly int _stopHotkeyHashcode;
        internal string ClientName = "";
        internal string BotVersion = "";


        [DllImport("user32.dll")]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName,
            int nMaxCount);

        public MainForm(IConfiguration configuration, IRunner runner, IManualOverlay manualOverlay, IPuzzler puzzler)
        {
            InitializeComponent();

            _configuration = configuration;
            _manualOverlay = manualOverlay;

            _runner = runner;

            _startHotkey = new Hotkey(Constants.CTRL, Keys.R, this);
            _startHotkey.Register();
            _startHotkeyHashcode = _startHotkey.GetHashCode();

            _stopHotkey = new Hotkey(Constants.CTRL, Keys.S, this);
            _stopHotkey.Register();
            _stopHotkeyHashcode = _stopHotkey.GetHashCode();

            form = this;
            settingsForm = new Settings(_configuration);
            backgroundWorker1.RunWorkerAsync();
            overlayWorker.RunWorkerAsync();
            LoadSettings();

            radioButton_Manual.Checked = !_configuration.Automatic;
            radioButton_Auto.Checked = _configuration.Automatic;

            Debug.WriteLine($"Auto: {_configuration.Automatic}");
            _puzzler = puzzler;
        }

        public static void ReloadSettings()
        {
            form.LoadSettings();
        }

        private void LoadSettings()
        {
            _configuration.Automatic = Properties.Settings.Default.Automatic;
            _configuration.MouseSpeed = Properties.Settings.Default.MouseSpeed;
            _configuration.RejectBoards = Properties.Settings.Default.RejectBoards;
            _configuration.RejectScore = Properties.Settings.Default.RejectScore;
            _configuration.FinishChain = Properties.Settings.Default.ChainFinish;
        }

        public static void Remote_Start_Click()
        {
            form.StartButtonClick();

        }
        private void Start_Click(object sender, EventArgs e)
        {
            StartButtonClick();
        }

        private void StartButtonClick()
        {
            if (!button_Start.Enabled) return;
            if (_runner.IsRunning)
            {
                try
                {
                    radioButton_Auto.Enabled = true;
                    radioButton_Manual.Enabled = true;
                    _runner.Interrupt();
                    _manualOverlay.ClearMove();
                    _puzzler.Reset();
                    _manualOverlay.Pause();
                    button_Start.Text = "Start (CTRL + R)";
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Canceled");
                    //throw;
                }

            }
            else
            {
                if (lbl_Status.Text == "Stopping...") return;
                _puzzler.Reset();
                _runner.Run();
                button_Start.Text = "Stop (CTRL + S)";
                _manualOverlay.UnPause();
                radioButton_Auto.Enabled = false;
                radioButton_Manual.Enabled = false;
            }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 0x0312)
            {
                int id = m.WParam.ToInt32();

                if (id == _startHotkeyHashcode && !_runner.IsRunning || id == _stopHotkeyHashcode && _runner.IsRunning)
                {
                    StartButtonClick();
                }
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _startHotkey.UnRegister();
            _stopHotkey.UnRegister();
            Properties.Settings.Default.WindowSize = this.Size;
            Properties.Settings.Default.WindowLocation = this.Location;
            Properties.Settings.Default.Save();
            try
            {
                _runner.Interrupt();
                _manualOverlay.Stop();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                //throw;
            }

        }


        private bool IsPPWindow()
        {
            foreach (Process p in Process.GetProcesses())
            {
                if (IsPPTitle(p.MainWindowTitle))
                {
                    _configuration.PpWindow = p.MainWindowHandle;
                    _configuration.PpProcess = p;
                    return true;
                }
                else if (p.MainWindowTitle == "Puzzle Pirates" || p.MainWindowTitle == "Puzzle Piraten")
                {
                    if (GetWindowClass(p.MainWindowHandle) == "SunAwtFrame")
                    {
                        _configuration.PpWindow = p.MainWindowHandle;
                        _configuration.PpProcess = p;
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsPPTitle(string Title)
        {
            if ((Title.StartsWith("Puzzle Piraten") && (Title.EndsWith("ocean"))) || (Title.StartsWith("Puzzle Pirates") && (Title.EndsWith("ocean"))))
                return true;
            return false;
        }

        //GetWindowClass
        private static string GetWindowClass(IntPtr Handle)
        {
            StringBuilder lpClassName = new StringBuilder();
            GetClassName(Handle, lpClassName, 100);
            return lpClassName.ToString();
        }

        private bool CheckHandle()
        {
            if (_configuration.PpProcess == null)
                return false;
            else if (_configuration.PpProcess.HasExited)
                return false;


            Match match = new Regex("- (.*?) on").Match(_configuration.PpProcess.MainWindowTitle);
            if (match.Success)
            {
                //Console.WriteLine(clientName);
                //pirate = match.Groups[1].Value + "_";
                ClientName = match.Groups[1].Value;
            }
            return true;
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            System.Threading.Timer timer = null;
            timer = new System.Threading.Timer((g) =>
            {
                if (!CheckHandle())
                {
                    if (IsPPWindow())
                    {
                        SetLabel(Labels.Status, "PP Found", Color.Green);
                        _manualOverlay.CreateWindow();
                        _manualOverlay.Pause();
                        button_Start.Invoke(new MethodInvoker(delegate { button_Start.Enabled = true; }));
                    }
                    else
                    {
                        SetLabel(Labels.Status, "PP Not Found", Color.Red);
                        button_Start.Invoke(new MethodInvoker(delegate { button_Start.Enabled = false; }));
                    }
                }

                timer.Change(2000, Timeout.Infinite);
            }, null, 0, Timeout.Infinite);
        }

        private void UpdateLabel(string msg, string labelName)
        {
            Label label = Controls.Find(labelName, true).FirstOrDefault() as Label;
            if (label.InvokeRequired)
                label.Invoke(new MethodInvoker(delegate
                {
                    label.Text = msg;
                    label.ResetForeColor();
                }));
            else
            {
                label.Text = msg;
                label.ResetForeColor();
            }
        }

        private void UpdateLabel(string msg, string labelName, Color color)
        {
            Label label = Controls.Find(labelName, true).FirstOrDefault() as Label;
            if (label.InvokeRequired)
                label.Invoke(new MethodInvoker(delegate
                {
                    label.Text = msg;
                    label.ForeColor = color;
                }));
            else
            {
                label.Text = msg;
                label.ForeColor = color;
            }
        }


        public static void SetLabel(string labelName, string text)
        {
            form.UpdateLabel(text, labelName);
        }

        public static void SetLabel(string labelName, string text, Color color)
        {
            form.UpdateLabel(text, labelName, color);
        }

        internal static class Labels
        {
            public static readonly string Status = "lbl_Status";
            public static readonly string Window = "lbl_PpWindow";
            public static readonly string Flag = "lbl_Flag";
            public static readonly string Chain = "lbl_Chain";
            public static readonly string SolveScore = "lbl_SolveScore";
            public static readonly string AverageScore = "lbl_AverageTotal";
            public static readonly string TotalScore = "lbl_TotalScore";
            public static readonly string BonusScore = "lbl_BonusScore";
            public static readonly string MouseSpeed = "lbl_avgMouseSpeed";
            public static readonly string LastGame = "lbl_LastGameScore";
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            BotVersion = $"v{fileVersionInfo.ProductVersion}";
            lblVersion.Text = $"v{fileVersionInfo.ProductVersion}";
            if (Properties.Settings.Default.WindowSize != new Size(0, 0))
            {
                this.Size = Properties.Settings.Default.WindowSize;
                this.Location = EnsureVisibleLocation(Properties.Settings.Default.WindowLocation);
            }
        }

        private void button_Settings_Click(object sender, EventArgs e)
        {
            if (_runner.IsRunning)
            {
                try
                {
                    StartButtonClick();
                }
                catch (Exception ev)
                {
                    Debug.WriteLine("Canceled");
                    //throw;
                }
            }
            settingsForm.ShowDialog();
        }

        private void overlayWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            while (!_manualOverlay.WindowCreated)
                Thread.Sleep(500);
            _manualOverlay.Run();
        }

        private async void button_ForceScan_Click(object sender, EventArgs e)
        {

            // Set ForceScan to true and disable the button
            _configuration.ForceScan = true;
            button_ForceScan.Enabled = false;

            // Wait for 5 seconds without blocking the UI thread
            await Task.Delay(5000);

            // Set ForceScan to false and enable the button
            _configuration.ForceScan = false;
            button_ForceScan.Enabled = true;
        }

        private async void button_NoSwaps_Click(object sender, EventArgs e)
        {
            // Set ForceScan to true and disable the button
            button_NoSwaps.Enabled = false;

            // Wait for 5 seconds without blocking the UI thread
            await Task.Delay(1000);

            // Set ForceScan to false and enable the button
            button_NoSwaps.Enabled = true;
        }

        private Point EnsureVisibleLocation(Point location)
        {
            bool isVisible = false;
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.WorkingArea.Contains(location))
                {
                    isVisible = true;
                    break;
                }
            }

            if (!isVisible)
            {
                // Default to the primary screen's working area location
                location = Screen.PrimaryScreen.WorkingArea.Location;
            }

            return location;
        }

        private void radioButton_Auto_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Automatic = radioButton_Auto.Checked;
            Properties.Settings.Default.Save();
            ReloadSettings();
        }
    }
}
