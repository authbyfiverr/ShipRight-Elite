using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ShipRight.Extensions;

namespace ShipRight
{
    internal partial class Settings : Form
    {
        private readonly IConfiguration _configuration;

        public Settings(IConfiguration configuration)
        {
            InitializeComponent();
            _configuration = configuration;
            numeric_MouseSpeed.Value = Properties.Settings.Default.MouseSpeed;
            checkBox_Reject.Checked = Properties.Settings.Default.RejectBoards;
            numeric_RejectScore.Value = Properties.Settings.Default.RejectScore;
            numericUpDown_FinishChain.Value = Properties.Settings.Default.ChainFinish;

            if (checkBox_Reject.Checked)
            {
                numeric_RejectScore.Enabled = true;
            }
            else
            {
                numeric_RejectScore.Enabled = false;
            }
        }

        private void button_Save_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
            MainForm.ReloadSettings();
            MainForm.SetLabel(MainForm.Labels.Status, "Settings Saved", Color.Green);
            this.Close();
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            MainForm.SetLabel(MainForm.Labels.Status, "Settings discarded", Color.Orange);
            this.Close();
        }

        private void numeric_MouseSpeed_ValueChanged(object sender, EventArgs e)
            => Properties.Settings.Default.MouseSpeed = (int)numeric_MouseSpeed.Value;

        private void numeric_RejectScore_ValueChanged(object sender, EventArgs e)
            => Properties.Settings.Default.RejectScore = numeric_RejectScore.Value;

        private void checkBox_Reject_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.RejectBoards = checkBox_Reject.Checked;
            if (checkBox_Reject.Checked)
            {
                numeric_RejectScore.Enabled = true;
            }
            else
            {
                numeric_RejectScore.Enabled = false;
            }
        }

        private void numericUpDown_FinishChain_ValueChanged(object sender, EventArgs e)
            => Properties.Settings.Default.ChainFinish = (int)numericUpDown_FinishChain.Value;
    }
}
