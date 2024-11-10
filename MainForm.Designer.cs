
namespace ShipRight
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            button_Settings = new System.Windows.Forms.Button();
            overlayWorker = new System.ComponentModel.BackgroundWorker();
            lbl_Flag = new System.Windows.Forms.Label();
            groupBox2 = new System.Windows.Forms.GroupBox();
            lbl_LastGameScore = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            lbl_AverageTotal = new System.Windows.Forms.Label();
            label9 = new System.Windows.Forms.Label();
            lbl_BonusScore = new System.Windows.Forms.Label();
            lbl_TotalScore = new System.Windows.Forms.Label();
            label11 = new System.Windows.Forms.Label();
            label10 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            lbl_SolveScore = new System.Windows.Forms.Label();
            lbl_Chain = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            groupBox1 = new System.Windows.Forms.GroupBox();
            radioButton_Auto = new System.Windows.Forms.RadioButton();
            radioButton_Manual = new System.Windows.Forms.RadioButton();
            button_ForceScan = new System.Windows.Forms.Button();
            button_NoSwaps = new System.Windows.Forms.Button();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            lbl_Status = new System.Windows.Forms.Label();
            button_Start = new System.Windows.Forms.Button();
            lblVersion = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            lbl_avgMouseSpeed = new System.Windows.Forms.Label();
            groupBox2.SuspendLayout();
            groupBox1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // backgroundWorker1
            // 
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            // 
            // button_Settings
            // 
            tableLayoutPanel1.SetColumnSpan(button_Settings, 2);
            button_Settings.Dock = System.Windows.Forms.DockStyle.Fill;
            button_Settings.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            button_Settings.Location = new System.Drawing.Point(87, 287);
            button_Settings.Name = "button_Settings";
            button_Settings.Size = new System.Drawing.Size(74, 24);
            button_Settings.TabIndex = 22;
            button_Settings.TabStop = false;
            button_Settings.Text = "Settings";
            button_Settings.UseVisualStyleBackColor = true;
            button_Settings.Click += button_Settings_Click;
            // 
            // overlayWorker
            // 
            overlayWorker.DoWork += overlayWorker_DoWork;
            // 
            // lbl_Flag
            // 
            lbl_Flag.Location = new System.Drawing.Point(85, 19);
            lbl_Flag.Name = "lbl_Flag";
            lbl_Flag.Size = new System.Drawing.Size(46, 19);
            lbl_Flag.TabIndex = 33;
            lbl_Flag.Text = "?/20";
            lbl_Flag.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox2
            // 
            tableLayoutPanel1.SetColumnSpan(groupBox2, 4);
            groupBox2.Controls.Add(lbl_LastGameScore);
            groupBox2.Controls.Add(label7);
            groupBox2.Controls.Add(lbl_AverageTotal);
            groupBox2.Controls.Add(label9);
            groupBox2.Controls.Add(lbl_BonusScore);
            groupBox2.Controls.Add(lbl_TotalScore);
            groupBox2.Controls.Add(label11);
            groupBox2.Controls.Add(label10);
            groupBox2.Controls.Add(label6);
            groupBox2.Controls.Add(label5);
            groupBox2.Controls.Add(lbl_SolveScore);
            groupBox2.Controls.Add(lbl_Chain);
            groupBox2.Controls.Add(lbl_Flag);
            groupBox2.Controls.Add(label8);
            groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBox2.Location = new System.Drawing.Point(3, 113);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new System.Drawing.Size(158, 168);
            groupBox2.TabIndex = 35;
            groupBox2.TabStop = false;
            groupBox2.Text = "Current Details";
            // 
            // lbl_LastGameScore
            // 
            lbl_LastGameScore.Location = new System.Drawing.Point(85, 145);
            lbl_LastGameScore.Name = "lbl_LastGameScore";
            lbl_LastGameScore.Size = new System.Drawing.Size(46, 19);
            lbl_LastGameScore.TabIndex = 50;
            lbl_LastGameScore.Text = "0";
            lbl_LastGameScore.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            label7.Location = new System.Drawing.Point(8, 145);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(76, 19);
            label7.TabIndex = 49;
            label7.Text = "Last Game:";
            label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbl_AverageTotal
            // 
            lbl_AverageTotal.Location = new System.Drawing.Point(85, 70);
            lbl_AverageTotal.Name = "lbl_AverageTotal";
            lbl_AverageTotal.Size = new System.Drawing.Size(46, 19);
            lbl_AverageTotal.TabIndex = 48;
            lbl_AverageTotal.Text = "0";
            lbl_AverageTotal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label9
            // 
            label9.Location = new System.Drawing.Point(8, 70);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(76, 19);
            label9.TabIndex = 47;
            label9.Text = "Avg Score:";
            label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbl_BonusScore
            // 
            lbl_BonusScore.Location = new System.Drawing.Point(85, 112);
            lbl_BonusScore.Name = "lbl_BonusScore";
            lbl_BonusScore.Size = new System.Drawing.Size(46, 19);
            lbl_BonusScore.TabIndex = 44;
            lbl_BonusScore.Text = "0";
            lbl_BonusScore.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_TotalScore
            // 
            lbl_TotalScore.Location = new System.Drawing.Point(85, 53);
            lbl_TotalScore.Name = "lbl_TotalScore";
            lbl_TotalScore.Size = new System.Drawing.Size(46, 19);
            lbl_TotalScore.TabIndex = 46;
            lbl_TotalScore.Text = "0";
            lbl_TotalScore.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label11
            // 
            label11.Location = new System.Drawing.Point(8, 53);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(76, 19);
            label11.TabIndex = 45;
            label11.Text = "Total Score:";
            label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label10
            // 
            label10.Location = new System.Drawing.Point(8, 112);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(76, 19);
            label10.TabIndex = 43;
            label10.Text = "Bonus Score:";
            label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            label6.Location = new System.Drawing.Point(8, 36);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(76, 19);
            label6.TabIndex = 40;
            label6.Text = "Chain:";
            label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            label5.Location = new System.Drawing.Point(8, 19);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(76, 19);
            label5.TabIndex = 39;
            label5.Text = "Move:";
            label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbl_SolveScore
            // 
            lbl_SolveScore.Location = new System.Drawing.Point(85, 95);
            lbl_SolveScore.Name = "lbl_SolveScore";
            lbl_SolveScore.Size = new System.Drawing.Size(46, 19);
            lbl_SolveScore.TabIndex = 42;
            lbl_SolveScore.Text = "0";
            lbl_SolveScore.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_Chain
            // 
            lbl_Chain.Location = new System.Drawing.Point(85, 36);
            lbl_Chain.Name = "lbl_Chain";
            lbl_Chain.Size = new System.Drawing.Size(46, 19);
            lbl_Chain.TabIndex = 36;
            lbl_Chain.Text = "0";
            lbl_Chain.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            label8.Location = new System.Drawing.Point(8, 95);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(76, 19);
            label8.TabIndex = 41;
            label8.Text = "Solve Score:";
            label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox1
            // 
            tableLayoutPanel1.SetColumnSpan(groupBox1, 4);
            groupBox1.Controls.Add(radioButton_Auto);
            groupBox1.Controls.Add(radioButton_Manual);
            groupBox1.Location = new System.Drawing.Point(3, 33);
            groupBox1.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(158, 41);
            groupBox1.TabIndex = 51;
            groupBox1.TabStop = false;
            groupBox1.Text = "Mode";
            // 
            // radioButton_Auto
            // 
            radioButton_Auto.AutoSize = true;
            radioButton_Auto.Location = new System.Drawing.Point(91, 16);
            radioButton_Auto.Margin = new System.Windows.Forms.Padding(3, 3, 10, 3);
            radioButton_Auto.Name = "radioButton_Auto";
            radioButton_Auto.Size = new System.Drawing.Size(51, 19);
            radioButton_Auto.TabIndex = 1;
            radioButton_Auto.TabStop = true;
            radioButton_Auto.Text = "Auto";
            radioButton_Auto.UseVisualStyleBackColor = true;
            radioButton_Auto.CheckedChanged += radioButton_Auto_CheckedChanged;
            // 
            // radioButton_Manual
            // 
            radioButton_Manual.AutoSize = true;
            radioButton_Manual.Location = new System.Drawing.Point(8, 16);
            radioButton_Manual.Name = "radioButton_Manual";
            radioButton_Manual.Size = new System.Drawing.Size(65, 19);
            radioButton_Manual.TabIndex = 0;
            radioButton_Manual.TabStop = true;
            radioButton_Manual.Text = "Manual";
            radioButton_Manual.UseVisualStyleBackColor = true;
            // 
            // button_ForceScan
            // 
            tableLayoutPanel1.SetColumnSpan(button_ForceScan, 2);
            button_ForceScan.Dock = System.Windows.Forms.DockStyle.Fill;
            button_ForceScan.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            button_ForceScan.Location = new System.Drawing.Point(3, 80);
            button_ForceScan.Margin = new System.Windows.Forms.Padding(3, 3, 6, 3);
            button_ForceScan.Name = "button_ForceScan";
            button_ForceScan.Size = new System.Drawing.Size(75, 27);
            button_ForceScan.TabIndex = 36;
            button_ForceScan.TabStop = false;
            button_ForceScan.Text = "Force Solve";
            button_ForceScan.UseVisualStyleBackColor = true;
            button_ForceScan.Click += button_ForceScan_Click;
            // 
            // button_NoSwaps
            // 
            tableLayoutPanel1.SetColumnSpan(button_NoSwaps, 2);
            button_NoSwaps.Dock = System.Windows.Forms.DockStyle.Fill;
            button_NoSwaps.Enabled = false;
            button_NoSwaps.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            button_NoSwaps.Location = new System.Drawing.Point(90, 80);
            button_NoSwaps.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            button_NoSwaps.Name = "button_NoSwaps";
            button_NoSwaps.Size = new System.Drawing.Size(71, 27);
            button_NoSwaps.TabIndex = 37;
            button_NoSwaps.TabStop = false;
            button_NoSwaps.Text = "No Swaps";
            button_NoSwaps.UseVisualStyleBackColor = true;
            button_NoSwaps.Click += button_NoSwaps_Click;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 4;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 49F));
            tableLayoutPanel1.Controls.Add(button_Settings, 2, 5);
            tableLayoutPanel1.Controls.Add(groupBox1, 0, 2);
            tableLayoutPanel1.Controls.Add(groupBox2, 0, 4);
            tableLayoutPanel1.Controls.Add(lbl_Status, 0, 6);
            tableLayoutPanel1.Controls.Add(button_NoSwaps, 2, 3);
            tableLayoutPanel1.Controls.Add(button_Start, 0, 7);
            tableLayoutPanel1.Controls.Add(lblVersion, 3, 8);
            tableLayoutPanel1.Controls.Add(button_ForceScan, 0, 3);
            tableLayoutPanel1.Controls.Add(label1, 0, 8);
            tableLayoutPanel1.Controls.Add(label2, 0, 0);
            tableLayoutPanel1.Controls.Add(label3, 0, 1);
            tableLayoutPanel1.Controls.Add(lbl_avgMouseSpeed, 3, 0);
            tableLayoutPanel1.Location = new System.Drawing.Point(6, 6);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 10;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 18F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 174F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 78F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new System.Drawing.Size(164, 388);
            tableLayoutPanel1.TabIndex = 38;
            // 
            // lbl_Status
            // 
            tableLayoutPanel1.SetColumnSpan(lbl_Status, 4);
            lbl_Status.Dock = System.Windows.Forms.DockStyle.Fill;
            lbl_Status.Location = new System.Drawing.Point(3, 314);
            lbl_Status.Name = "lbl_Status";
            lbl_Status.Padding = new System.Windows.Forms.Padding(1);
            lbl_Status.Size = new System.Drawing.Size(158, 23);
            lbl_Status.TabIndex = 10;
            lbl_Status.Text = "Stopped";
            lbl_Status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button_Start
            // 
            tableLayoutPanel1.SetColumnSpan(button_Start, 4);
            button_Start.Dock = System.Windows.Forms.DockStyle.Fill;
            button_Start.Enabled = false;
            button_Start.Location = new System.Drawing.Point(10, 340);
            button_Start.Margin = new System.Windows.Forms.Padding(10, 3, 10, 3);
            button_Start.Name = "button_Start";
            button_Start.Size = new System.Drawing.Size(144, 26);
            button_Start.TabIndex = 3;
            button_Start.TabStop = false;
            button_Start.Text = "Start (CTRL + R)";
            button_Start.UseVisualStyleBackColor = true;
            button_Start.Click += Start_Click;
            // 
            // lblVersion
            // 
            lblVersion.Location = new System.Drawing.Point(117, 369);
            lblVersion.Name = "lblVersion";
            lblVersion.Size = new System.Drawing.Size(44, 13);
            lblVersion.TabIndex = 9;
            lblVersion.Text = "v1.0.0";
            lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            tableLayoutPanel1.SetColumnSpan(label1, 3);
            label1.Dock = System.Windows.Forms.DockStyle.Fill;
            label1.Location = new System.Drawing.Point(3, 369);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(108, 15);
            label1.TabIndex = 11;
            label1.Text = "Barrelstopper 2023";
            label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            label2.AutoSize = true;
            tableLayoutPanel1.SetColumnSpan(label2, 3);
            label2.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label2.Location = new System.Drawing.Point(6, 1);
            label2.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(105, 15);
            label2.TabIndex = 52;
            label2.Text = "Avg Mouse Speed:";
            // 
            // label3
            // 
            label3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            label3.AutoSize = true;
            tableLayoutPanel1.SetColumnSpan(label3, 4);
            label3.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label3.Location = new System.Drawing.Point(3, 18);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(158, 12);
            label3.TabIndex = 53;
            label3.Text = "Keep between 0.2 - 0.7";
            label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lbl_avgMouseSpeed
            // 
            lbl_avgMouseSpeed.Anchor = System.Windows.Forms.AnchorStyles.None;
            lbl_avgMouseSpeed.AutoSize = true;
            lbl_avgMouseSpeed.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lbl_avgMouseSpeed.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            lbl_avgMouseSpeed.Location = new System.Drawing.Point(117, 1);
            lbl_avgMouseSpeed.Margin = new System.Windows.Forms.Padding(3, 0, 18, 0);
            lbl_avgMouseSpeed.Name = "lbl_avgMouseSpeed";
            lbl_avgMouseSpeed.Size = new System.Drawing.Size(28, 15);
            lbl_avgMouseSpeed.TabIndex = 54;
            lbl_avgMouseSpeed.Text = "0.00";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            ClientSize = new System.Drawing.Size(176, 393);
            Controls.Add(tableLayoutPanel1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MainForm";
            Padding = new System.Windows.Forms.Padding(3);
            Text = "ShipRight Elite";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            groupBox2.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Button button_Settings;
        private System.ComponentModel.BackgroundWorker overlayWorker;
        private System.Windows.Forms.Label lbl_Flag;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lbl_Chain;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lbl_SolveScore;
        private System.Windows.Forms.Label lbl_TotalScore;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label lbl_BonusScore;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lbl_AverageTotal;
        private System.Windows.Forms.Button button_ForceScan;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button button_NoSwaps;
        private System.Windows.Forms.Label lbl_LastGameScore;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lbl_Status;
        internal System.Windows.Forms.Button button_Start;
        internal System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButton_Auto;
        private System.Windows.Forms.RadioButton radioButton_Manual;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbl_avgMouseSpeed;
    }
}

