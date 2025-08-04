namespace UT2_LISG_Stats
{
    partial class Form1
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
            tableLayoutPanel1 = new TableLayoutPanel();
            flowLayoutPanel1 = new FlowLayoutPanel();
            tbTower1 = new ReaLTaiizor.Controls.PoisonTextBox();
            poisonDateTime1 = new ReaLTaiizor.Controls.PoisonDateTime();
            poisonDateTime2 = new ReaLTaiizor.Controls.PoisonDateTime();
            btnExecute = new ReaLTaiizor.Controls.PoisonButton();
            cbSelectPoints = new CheckBox();
            cbShowPoints = new CheckBox();
            lbMovingAverage = new ReaLTaiizor.Controls.PoisonLabel();
            tbMovingAverage = new ReaLTaiizor.Controls.PoisonTextBox();
            lbSlope = new ReaLTaiizor.Controls.PoisonLabel();
            tbSlope = new ReaLTaiizor.Controls.PoisonTextBox();
            lbLoess = new ReaLTaiizor.Controls.PoisonLabel();
            tbLoess = new ReaLTaiizor.Controls.PoisonTextBox();
            formsPlot1 = new ScottPlot.WinForms.FormsPlot();
            flowLayoutPanel2 = new FlowLayoutPanel();
            formsPlot2 = new ScottPlot.WinForms.FormsPlot();
            formsPlot3 = new ScottPlot.WinForms.FormsPlot();
            tableLayoutPanel1.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            flowLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.AutoScroll = true;
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(flowLayoutPanel1, 0, 0);
            tableLayoutPanel1.Controls.Add(formsPlot1, 0, 1);
            tableLayoutPanel1.Controls.Add(flowLayoutPanel2, 0, 2);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new Size(1076, 450);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(tbTower1);
            flowLayoutPanel1.Controls.Add(poisonDateTime1);
            flowLayoutPanel1.Controls.Add(poisonDateTime2);
            flowLayoutPanel1.Controls.Add(btnExecute);
            flowLayoutPanel1.Controls.Add(cbSelectPoints);
            flowLayoutPanel1.Controls.Add(cbShowPoints);
            flowLayoutPanel1.Controls.Add(lbMovingAverage);
            flowLayoutPanel1.Controls.Add(tbMovingAverage);
            flowLayoutPanel1.Controls.Add(lbSlope);
            flowLayoutPanel1.Controls.Add(tbSlope);
            flowLayoutPanel1.Controls.Add(lbLoess);
            flowLayoutPanel1.Controls.Add(tbLoess);
            flowLayoutPanel1.Dock = DockStyle.Fill;
            flowLayoutPanel1.Location = new Point(3, 3);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(1070, 34);
            flowLayoutPanel1.TabIndex = 4;
            // 
            // tbTower1
            // 
            // 
            // 
            // 
            tbTower1.CustomButton.Image = null;
            tbTower1.CustomButton.Location = new Point(53, 1);
            tbTower1.CustomButton.Name = "";
            tbTower1.CustomButton.Size = new Size(21, 21);
            tbTower1.CustomButton.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Blue;
            tbTower1.CustomButton.TabIndex = 1;
            tbTower1.CustomButton.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Light;
            tbTower1.CustomButton.UseSelectable = true;
            tbTower1.CustomButton.Visible = false;
            tbTower1.Lines = new string[]
    {
    "tower1"
    };
            tbTower1.Location = new Point(3, 3);
            tbTower1.MaxLength = 32767;
            tbTower1.Name = "tbTower1";
            tbTower1.PasswordChar = '\0';
            tbTower1.ScrollBars = ScrollBars.None;
            tbTower1.SelectedText = "";
            tbTower1.SelectionLength = 0;
            tbTower1.SelectionStart = 0;
            tbTower1.ShortcutsEnabled = true;
            tbTower1.Size = new Size(75, 23);
            tbTower1.TabIndex = 1;
            tbTower1.Text = "tower1";
            tbTower1.UseSelectable = true;
            tbTower1.WaterMarkColor = Color.FromArgb(109, 109, 109);
            tbTower1.WaterMarkFont = new Font("Segoe UI", 12F, FontStyle.Italic, GraphicsUnit.Pixel);
            tbTower1.Click += tbTower1_Click;
            // 
            // poisonDateTime1
            // 
            poisonDateTime1.FontSize = ReaLTaiizor.Extension.Poison.PoisonDateTimeSize.Medium;
            poisonDateTime1.Location = new Point(84, 3);
            poisonDateTime1.MinimumSize = new Size(0, 29);
            poisonDateTime1.Name = "poisonDateTime1";
            poisonDateTime1.Size = new Size(200, 29);
            poisonDateTime1.TabIndex = 2;
            // 
            // poisonDateTime2
            // 
            poisonDateTime2.FontSize = ReaLTaiizor.Extension.Poison.PoisonDateTimeSize.Medium;
            poisonDateTime2.Location = new Point(290, 3);
            poisonDateTime2.MinimumSize = new Size(0, 29);
            poisonDateTime2.Name = "poisonDateTime2";
            poisonDateTime2.Size = new Size(200, 29);
            poisonDateTime2.TabIndex = 3;
            // 
            // btnExecute
            // 
            btnExecute.Location = new Point(496, 3);
            btnExecute.Name = "btnExecute";
            btnExecute.Size = new Size(103, 23);
            btnExecute.TabIndex = 4;
            btnExecute.Text = "Execute";
            btnExecute.UseSelectable = true;
            btnExecute.Click += btnExecute_Click;
            // 
            // cbSelectPoints
            // 
            cbSelectPoints.AutoSize = true;
            cbSelectPoints.Location = new Point(605, 3);
            cbSelectPoints.Name = "cbSelectPoints";
            cbSelectPoints.Size = new Size(93, 19);
            cbSelectPoints.TabIndex = 5;
            cbSelectPoints.Text = "Select Points";
            cbSelectPoints.UseVisualStyleBackColor = true;
            // 
            // cbShowPoints
            // 
            cbShowPoints.AutoSize = true;
            cbShowPoints.Location = new Point(704, 3);
            cbShowPoints.Name = "cbShowPoints";
            cbShowPoints.Size = new Size(91, 19);
            cbShowPoints.TabIndex = 6;
            cbShowPoints.Text = "Show Points";
            cbShowPoints.UseVisualStyleBackColor = true;
            // 
            // lbMovingAverage
            // 
            lbMovingAverage.AutoSize = true;
            lbMovingAverage.Location = new Point(801, 0);
            lbMovingAverage.Name = "lbMovingAverage";
            lbMovingAverage.Size = new Size(61, 19);
            lbMovingAverage.TabIndex = 7;
            lbMovingAverage.Text = "MovAvg:";
            // 
            // tbMovingAverage
            // 
            // 
            // 
            // 
            tbMovingAverage.CustomButton.Image = null;
            tbMovingAverage.CustomButton.Location = new Point(0, 1);
            tbMovingAverage.CustomButton.Name = "";
            tbMovingAverage.CustomButton.Size = new Size(21, 21);
            tbMovingAverage.CustomButton.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Blue;
            tbMovingAverage.CustomButton.TabIndex = 1;
            tbMovingAverage.CustomButton.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Light;
            tbMovingAverage.CustomButton.UseSelectable = true;
            tbMovingAverage.CustomButton.Visible = false;
            tbMovingAverage.Lines = new string[]
    {
    "300"
    };
            tbMovingAverage.Location = new Point(868, 3);
            tbMovingAverage.MaxLength = 32767;
            tbMovingAverage.Name = "tbMovingAverage";
            tbMovingAverage.PasswordChar = '\0';
            tbMovingAverage.ScrollBars = ScrollBars.None;
            tbMovingAverage.SelectedText = "";
            tbMovingAverage.SelectionLength = 0;
            tbMovingAverage.SelectionStart = 0;
            tbMovingAverage.ShortcutsEnabled = true;
            tbMovingAverage.Size = new Size(22, 23);
            tbMovingAverage.TabIndex = 0;
            tbMovingAverage.Text = "300";
            tbMovingAverage.UseSelectable = true;
            tbMovingAverage.WaterMarkColor = Color.FromArgb(109, 109, 109);
            tbMovingAverage.WaterMarkFont = new Font("Segoe UI", 12F, FontStyle.Italic, GraphicsUnit.Pixel);
            // 
            // lbSlope
            // 
            lbSlope.AutoSize = true;
            lbSlope.Location = new Point(896, 0);
            lbSlope.Name = "lbSlope";
            lbSlope.Size = new Size(42, 19);
            lbSlope.TabIndex = 8;
            lbSlope.Text = "Slope";
            // 
            // tbSlope
            // 
            // 
            // 
            // 
            tbSlope.CustomButton.Image = null;
            tbSlope.CustomButton.Location = new Point(0, 1);
            tbSlope.CustomButton.Name = "";
            tbSlope.CustomButton.Size = new Size(21, 21);
            tbSlope.CustomButton.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Blue;
            tbSlope.CustomButton.TabIndex = 1;
            tbSlope.CustomButton.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Light;
            tbSlope.CustomButton.UseSelectable = true;
            tbSlope.CustomButton.Visible = false;
            tbSlope.Lines = new string[]
    {
    "10"
    };
            tbSlope.Location = new Point(944, 3);
            tbSlope.MaxLength = 32767;
            tbSlope.Name = "tbSlope";
            tbSlope.PasswordChar = '\0';
            tbSlope.ScrollBars = ScrollBars.None;
            tbSlope.SelectedText = "";
            tbSlope.SelectionLength = 0;
            tbSlope.SelectionStart = 0;
            tbSlope.ShortcutsEnabled = true;
            tbSlope.Size = new Size(22, 23);
            tbSlope.TabIndex = 0;
            tbSlope.Text = "10";
            tbSlope.UseSelectable = true;
            tbSlope.WaterMarkColor = Color.FromArgb(109, 109, 109);
            tbSlope.WaterMarkFont = new Font("Segoe UI", 12F, FontStyle.Italic, GraphicsUnit.Pixel);
            // 
            // lbLoess
            // 
            lbLoess.AutoSize = true;
            lbLoess.Location = new Point(972, 0);
            lbLoess.Name = "lbLoess";
            lbLoess.Size = new Size(40, 19);
            lbLoess.TabIndex = 9;
            lbLoess.Text = "Loess";
            // 
            // tbLoess
            // 
            // 
            // 
            // 
            tbLoess.CustomButton.Image = null;
            tbLoess.CustomButton.Location = new Point(4, 1);
            tbLoess.CustomButton.Name = "";
            tbLoess.CustomButton.Size = new Size(21, 21);
            tbLoess.CustomButton.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Blue;
            tbLoess.CustomButton.TabIndex = 1;
            tbLoess.CustomButton.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Light;
            tbLoess.CustomButton.UseSelectable = true;
            tbLoess.CustomButton.Visible = false;
            tbLoess.Lines = new string[]
    {
    "0.05"
    };
            tbLoess.Location = new Point(1018, 3);
            tbLoess.MaxLength = 32767;
            tbLoess.Name = "tbLoess";
            tbLoess.PasswordChar = '\0';
            tbLoess.ScrollBars = ScrollBars.None;
            tbLoess.SelectedText = "";
            tbLoess.SelectionLength = 0;
            tbLoess.SelectionStart = 0;
            tbLoess.ShortcutsEnabled = true;
            tbLoess.Size = new Size(26, 23);
            tbLoess.TabIndex = 10;
            tbLoess.Text = "0.05";
            tbLoess.UseSelectable = true;
            tbLoess.WaterMarkColor = Color.FromArgb(109, 109, 109);
            tbLoess.WaterMarkFont = new Font("Segoe UI", 12F, FontStyle.Italic, GraphicsUnit.Pixel);
            // 
            // formsPlot1
            // 
            formsPlot1.DisplayScale = 1F;
            formsPlot1.Dock = DockStyle.Fill;
            formsPlot1.Location = new Point(3, 43);
            formsPlot1.Name = "formsPlot1";
            formsPlot1.Size = new Size(1070, 199);
            formsPlot1.TabIndex = 5;
            formsPlot1.ClientSizeChanged += formsPlot1_ClientSizeChanged;
            // 
            // flowLayoutPanel2
            // 
            flowLayoutPanel2.AutoScroll = true;
            flowLayoutPanel2.Controls.Add(formsPlot2);
            flowLayoutPanel2.Controls.Add(formsPlot3);
            flowLayoutPanel2.Dock = DockStyle.Fill;
            flowLayoutPanel2.Location = new Point(3, 248);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            flowLayoutPanel2.Size = new Size(1070, 199);
            flowLayoutPanel2.TabIndex = 6;
            // 
            // formsPlot2
            // 
            formsPlot2.DisplayScale = 1F;
            formsPlot2.Location = new Point(3, 3);
            formsPlot2.Name = "formsPlot2";
            formsPlot2.Size = new Size(1045, 199);
            formsPlot2.TabIndex = 0;
            // 
            // formsPlot3
            // 
            formsPlot3.DisplayScale = 1F;
            formsPlot3.Location = new Point(3, 208);
            formsPlot3.Name = "formsPlot3";
            formsPlot3.Size = new Size(1045, 199);
            formsPlot3.TabIndex = 1;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1076, 450);
            Controls.Add(tableLayoutPanel1);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Form1";
            tableLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            flowLayoutPanel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private FlowLayoutPanel flowLayoutPanel1;
        private ReaLTaiizor.Controls.PoisonTextBox tbMovingAverage;
        private ReaLTaiizor.Controls.PoisonTextBox tbTower1;
        private ReaLTaiizor.Controls.PoisonDateTime poisonDateTime1;
        private ReaLTaiizor.Controls.PoisonDateTime poisonDateTime2;
        private ReaLTaiizor.Controls.PoisonButton btnExecute;
        private CheckBox cbSelectPoints;
        private CheckBox cbShowPoints;
        private ReaLTaiizor.Controls.PoisonLabel lbMovingAverage;
        private ReaLTaiizor.Controls.PoisonLabel lbSlope;
        private ReaLTaiizor.Controls.PoisonTextBox tbSlope;
        private ReaLTaiizor.Controls.PoisonLabel lbLoess;
        private ReaLTaiizor.Controls.PoisonTextBox tbLoess;
        private ScottPlot.WinForms.FormsPlot formsPlot1;
        private FlowLayoutPanel flowLayoutPanel2;
        private ScottPlot.WinForms.FormsPlot formsPlot2;
        private ScottPlot.WinForms.FormsPlot formsPlot3;
    }
}
