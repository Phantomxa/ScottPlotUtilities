namespace Measurements
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
            tableLayoutPanel2 = new TableLayoutPanel();
            tbPreformno = new ReaLTaiizor.Controls.PoisonTextBox();
            tbTower = new ReaLTaiizor.Controls.PoisonTextBox();
            tableLayoutPanel3 = new TableLayoutPanel();
            poisonDateTime1 = new ReaLTaiizor.Controls.PoisonDateTime();
            poisonDateTime2 = new ReaLTaiizor.Controls.PoisonDateTime();
            btnExecute = new ReaLTaiizor.Controls.PoisonButton();
            flowLayoutPanel2 = new FlowLayoutPanel();
            formsPlot2 = new ScottPlot.WinForms.FormsPlot();
            tableLayoutPanel1.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            flowLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(flowLayoutPanel1, 0, 0);
            tableLayoutPanel1.Controls.Add(flowLayoutPanel2, 0, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(800, 450);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(tableLayoutPanel2);
            flowLayoutPanel1.Controls.Add(tableLayoutPanel3);
            flowLayoutPanel1.Controls.Add(btnExecute);
            flowLayoutPanel1.Dock = DockStyle.Fill;
            flowLayoutPanel1.Location = new Point(3, 3);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(794, 54);
            flowLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 1;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Controls.Add(tbPreformno, 0, 0);
            tableLayoutPanel2.Controls.Add(tbTower, 0, 1);
            tableLayoutPanel2.Location = new Point(3, 3);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 2;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Size = new Size(80, 55);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // tbPreformno
            // 
            // 
            // 
            // 
            tbPreformno.CustomButton.Image = null;
            tbPreformno.CustomButton.Location = new Point(54, 1);
            tbPreformno.CustomButton.Name = "";
            tbPreformno.CustomButton.Size = new Size(19, 19);
            tbPreformno.CustomButton.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Blue;
            tbPreformno.CustomButton.TabIndex = 1;
            tbPreformno.CustomButton.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Light;
            tbPreformno.CustomButton.UseSelectable = true;
            tbPreformno.CustomButton.Visible = false;
            tbPreformno.Dock = DockStyle.Fill;
            tbPreformno.Lines = new string[]
    {
    "preform_no"
    };
            tbPreformno.Location = new Point(3, 3);
            tbPreformno.MaxLength = 32767;
            tbPreformno.Name = "tbPreformno";
            tbPreformno.PasswordChar = '\0';
            tbPreformno.ScrollBars = ScrollBars.None;
            tbPreformno.SelectedText = "";
            tbPreformno.SelectionLength = 0;
            tbPreformno.SelectionStart = 0;
            tbPreformno.ShortcutsEnabled = true;
            tbPreformno.Size = new Size(74, 21);
            tbPreformno.TabIndex = 0;
            tbPreformno.Text = "preform_no";
            tbPreformno.UseSelectable = true;
            tbPreformno.WaterMarkColor = Color.FromArgb(109, 109, 109);
            tbPreformno.WaterMarkFont = new Font("Segoe UI", 12F, FontStyle.Italic, GraphicsUnit.Pixel);
            // 
            // tbTower
            // 
            // 
            // 
            // 
            tbTower.CustomButton.Image = null;
            tbTower.CustomButton.Location = new Point(54, 2);
            tbTower.CustomButton.Name = "";
            tbTower.CustomButton.Size = new Size(17, 17);
            tbTower.CustomButton.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Blue;
            tbTower.CustomButton.TabIndex = 1;
            tbTower.CustomButton.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Light;
            tbTower.CustomButton.UseSelectable = true;
            tbTower.CustomButton.Visible = false;
            tbTower.Dock = DockStyle.Fill;
            tbTower.Lines = new string[]
    {
    "tower"
    };
            tbTower.Location = new Point(3, 30);
            tbTower.MaxLength = 32767;
            tbTower.Name = "tbTower";
            tbTower.PasswordChar = '\0';
            tbTower.ScrollBars = ScrollBars.None;
            tbTower.SelectedText = "";
            tbTower.SelectionLength = 0;
            tbTower.SelectionStart = 0;
            tbTower.ShortcutsEnabled = true;
            tbTower.Size = new Size(74, 22);
            tbTower.TabIndex = 1;
            tbTower.Text = "tower";
            tbTower.UseSelectable = true;
            tbTower.WaterMarkColor = Color.FromArgb(109, 109, 109);
            tbTower.WaterMarkFont = new Font("Segoe UI", 12F, FontStyle.Italic, GraphicsUnit.Pixel);
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 1;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.Controls.Add(poisonDateTime1, 0, 0);
            tableLayoutPanel3.Controls.Add(poisonDateTime2, 0, 1);
            tableLayoutPanel3.Location = new Point(89, 3);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 2;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.Size = new Size(150, 55);
            tableLayoutPanel3.TabIndex = 1;
            // 
            // poisonDateTime1
            // 
            poisonDateTime1.FontSize = ReaLTaiizor.Extension.Poison.PoisonDateTimeSize.Medium;
            poisonDateTime1.Location = new Point(3, 3);
            poisonDateTime1.MinimumSize = new Size(0, 29);
            poisonDateTime1.Name = "poisonDateTime1";
            poisonDateTime1.Size = new Size(144, 29);
            poisonDateTime1.TabIndex = 0;
            // 
            // poisonDateTime2
            // 
            poisonDateTime2.FontSize = ReaLTaiizor.Extension.Poison.PoisonDateTimeSize.Medium;
            poisonDateTime2.Location = new Point(3, 30);
            poisonDateTime2.MinimumSize = new Size(0, 29);
            poisonDateTime2.Name = "poisonDateTime2";
            poisonDateTime2.Size = new Size(144, 29);
            poisonDateTime2.TabIndex = 1;
            // 
            // btnExecute
            // 
            btnExecute.Location = new Point(245, 3);
            btnExecute.Name = "btnExecute";
            btnExecute.Size = new Size(75, 23);
            btnExecute.TabIndex = 2;
            btnExecute.Text = "Execute";
            btnExecute.UseSelectable = true;
            btnExecute.Click += btnExecute_Click;
            // 
            // flowLayoutPanel2
            // 
            flowLayoutPanel2.Controls.Add(formsPlot2);
            flowLayoutPanel2.Dock = DockStyle.Fill;
            flowLayoutPanel2.Location = new Point(3, 63);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            flowLayoutPanel2.Size = new Size(794, 384);
            flowLayoutPanel2.TabIndex = 1;
            // 
            // formsPlot2
            // 
            formsPlot2.DisplayScale = 1F;
            formsPlot2.Location = new Point(3, 3);
            formsPlot2.Name = "formsPlot2";
            formsPlot2.Size = new Size(738, 359);
            formsPlot2.TabIndex = 1;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(tableLayoutPanel1);
            Name = "Form1";
            Text = "Form1";
            tableLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            flowLayoutPanel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private FlowLayoutPanel flowLayoutPanel1;
        private FlowLayoutPanel flowLayoutPanel2;
        private TableLayoutPanel tableLayoutPanel2;
        private ReaLTaiizor.Controls.PoisonTextBox tbPreformno;
        private ReaLTaiizor.Controls.PoisonTextBox tbTower;
        private TableLayoutPanel tableLayoutPanel3;
        private ReaLTaiizor.Controls.PoisonDateTime poisonDateTime1;
        private ReaLTaiizor.Controls.PoisonDateTime poisonDateTime2;
        private ReaLTaiizor.Controls.PoisonButton btnExecute;
        private ScottPlot.WinForms.FormsPlot formsPlot2;
    }
}
