namespace ShipRight
{
    partial class Settings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            groupBox2 = new System.Windows.Forms.GroupBox();
            label3 = new System.Windows.Forms.Label();
            numericUpDown_FinishChain = new System.Windows.Forms.NumericUpDown();
            checkBox_Reject = new System.Windows.Forms.CheckBox();
            label2 = new System.Windows.Forms.Label();
            numeric_RejectScore = new System.Windows.Forms.NumericUpDown();
            groupBox1 = new System.Windows.Forms.GroupBox();
            label1 = new System.Windows.Forms.Label();
            numeric_MouseSpeed = new System.Windows.Forms.NumericUpDown();
            button_Save = new System.Windows.Forms.Button();
            button_Cancel = new System.Windows.Forms.Button();
            toolTip1 = new System.Windows.Forms.ToolTip(components);
            tableLayoutPanel1.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_FinishChain).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numeric_RejectScore).BeginInit();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numeric_MouseSpeed).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 4;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            tableLayoutPanel1.Controls.Add(groupBox2, 2, 0);
            tableLayoutPanel1.Controls.Add(groupBox1, 0, 0);
            tableLayoutPanel1.Controls.Add(button_Save, 2, 2);
            tableLayoutPanel1.Controls.Add(button_Cancel, 3, 2);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(4);
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 52.43243F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 47.56757F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new System.Drawing.Size(338, 224);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // groupBox2
            // 
            tableLayoutPanel1.SetColumnSpan(groupBox2, 2);
            groupBox2.Controls.Add(label3);
            groupBox2.Controls.Add(numericUpDown_FinishChain);
            groupBox2.Controls.Add(checkBox_Reject);
            groupBox2.Controls.Add(label2);
            groupBox2.Controls.Add(numeric_RejectScore);
            groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBox2.Location = new System.Drawing.Point(174, 10);
            groupBox2.Margin = new System.Windows.Forms.Padding(6);
            groupBox2.Name = "groupBox2";
            tableLayoutPanel1.SetRowSpan(groupBox2, 2);
            groupBox2.Size = new System.Drawing.Size(154, 173);
            groupBox2.TabIndex = 6;
            groupBox2.TabStop = false;
            groupBox2.Text = "Puzzle Options";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(59, 83);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(88, 15);
            label3.TabIndex = 28;
            label3.Text = "Finish Chain [?]";
            toolTip1.SetToolTip(label3, "This setting determines whether to carry a combo through flag 19 or to re-solve at 19 for a potentially higher score.");
            // 
            // numericUpDown_FinishChain
            // 
            numericUpDown_FinishChain.Location = new System.Drawing.Point(6, 79);
            numericUpDown_FinishChain.Maximum = new decimal(new int[] { 84, 0, 0, 65536 });
            numericUpDown_FinishChain.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown_FinishChain.Name = "numericUpDown_FinishChain";
            numericUpDown_FinishChain.Size = new System.Drawing.Size(47, 23);
            numericUpDown_FinishChain.TabIndex = 27;
            numericUpDown_FinishChain.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            numericUpDown_FinishChain.Value = new decimal(new int[] { 6, 0, 0, 0 });
            numericUpDown_FinishChain.ValueChanged += numericUpDown_FinishChain_ValueChanged;
            // 
            // checkBox_Reject
            // 
            checkBox_Reject.AutoSize = true;
            checkBox_Reject.Location = new System.Drawing.Point(10, 25);
            checkBox_Reject.Name = "checkBox_Reject";
            checkBox_Reject.Size = new System.Drawing.Size(97, 19);
            checkBox_Reject.TabIndex = 26;
            checkBox_Reject.Text = "Reject Boards";
            checkBox_Reject.UseVisualStyleBackColor = true;
            checkBox_Reject.Click += checkBox_Reject_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(59, 54);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(79, 15);
            label2.TabIndex = 25;
            label2.Text = "Dismiss Score";
            // 
            // numeric_RejectScore
            // 
            numeric_RejectScore.DecimalPlaces = 2;
            numeric_RejectScore.Enabled = false;
            numeric_RejectScore.Increment = new decimal(new int[] { 25, 0, 0, 131072 });
            numeric_RejectScore.Location = new System.Drawing.Point(6, 50);
            numeric_RejectScore.Maximum = new decimal(new int[] { 84, 0, 0, 65536 });
            numeric_RejectScore.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numeric_RejectScore.Name = "numeric_RejectScore";
            numeric_RejectScore.Size = new System.Drawing.Size(47, 23);
            numeric_RejectScore.TabIndex = 24;
            numeric_RejectScore.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            numeric_RejectScore.Value = new decimal(new int[] { 6, 0, 0, 0 });
            numeric_RejectScore.ValueChanged += numeric_RejectScore_ValueChanged;
            // 
            // groupBox1
            // 
            tableLayoutPanel1.SetColumnSpan(groupBox1, 2);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(numeric_MouseSpeed);
            groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBox1.Location = new System.Drawing.Point(10, 10);
            groupBox1.Margin = new System.Windows.Forms.Padding(6);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(152, 85);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Preferences";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(59, 22);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(78, 15);
            label1.TabIndex = 25;
            label1.Text = "Mouse Speed";
            // 
            // numeric_MouseSpeed
            // 
            numeric_MouseSpeed.Enabled = false;
            numeric_MouseSpeed.Location = new System.Drawing.Point(6, 18);
            numeric_MouseSpeed.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            numeric_MouseSpeed.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numeric_MouseSpeed.Name = "numeric_MouseSpeed";
            numeric_MouseSpeed.Size = new System.Drawing.Size(47, 23);
            numeric_MouseSpeed.TabIndex = 24;
            numeric_MouseSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            numeric_MouseSpeed.Value = new decimal(new int[] { 5, 0, 0, 0 });
            numeric_MouseSpeed.ValueChanged += numeric_MouseSpeed_ValueChanged;
            // 
            // button_Save
            // 
            button_Save.Location = new System.Drawing.Point(171, 192);
            button_Save.Name = "button_Save";
            button_Save.Size = new System.Drawing.Size(75, 23);
            button_Save.TabIndex = 4;
            button_Save.Text = "Save";
            button_Save.UseVisualStyleBackColor = true;
            button_Save.Click += button_Save_Click;
            // 
            // button_Cancel
            // 
            button_Cancel.Location = new System.Drawing.Point(253, 192);
            button_Cancel.Name = "button_Cancel";
            button_Cancel.Size = new System.Drawing.Size(75, 23);
            button_Cancel.TabIndex = 5;
            button_Cancel.Text = "Cancel";
            button_Cancel.UseVisualStyleBackColor = true;
            button_Cancel.Click += button_Cancel_Click;
            // 
            // Settings
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            ClientSize = new System.Drawing.Size(338, 224);
            Controls.Add(tableLayoutPanel1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            MaximizeBox = false;
            MinimumSize = new System.Drawing.Size(354, 263);
            Name = "Settings";
            Text = "ShipRight Settings";
            TopMost = true;
            tableLayoutPanel1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_FinishChain).EndInit();
            ((System.ComponentModel.ISupportInitialize)numeric_RejectScore).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numeric_MouseSpeed).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button_Save;
        private System.Windows.Forms.Button button_Cancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numeric_MouseSpeed;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkBox_Reject;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numeric_RejectScore;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.NumericUpDown numericUpDown_FinishChain;
    }
}