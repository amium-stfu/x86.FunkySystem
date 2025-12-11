namespace FunkySystem.Battery
{
    partial class FormAddTest
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
            tableRoot = new TableLayoutPanel();
            BoxTest = new GroupBox();
            groupBox1 = new GroupBox();
            panelHardwareList = new Panel();
            SequenceName = new FunkySystem.Controls.StringSignalView();
            boxBattery = new GroupBox();
            btnLoadBattery = new FunkySystem.Controls.ButtonWithIcon();
            CutoffDischarge = new FunkySystem.Controls.SignalView();
            SetTemperature = new FunkySystem.Controls.SignalView();
            UpperOvershoot = new FunkySystem.Controls.SignalView();
            LowerOvershoot = new FunkySystem.Controls.SignalView();
            CutoffCharge = new FunkySystem.Controls.SignalView();
            NominalVoltage = new FunkySystem.Controls.SignalView();
            Capacity = new FunkySystem.Controls.SignalView();
            CellType = new FunkySystem.Controls.StringSignalView();
            SN = new FunkySystem.Controls.StringSignalView();
            Partnumber = new FunkySystem.Controls.StringSignalView();
            panel1 = new Panel();
            btnCancel = new FunkySystem.Controls.ButtonWithIcon();
            btnSet = new FunkySystem.Controls.ButtonWithIcon();
            label1 = new Label();
            tableRoot.SuspendLayout();
            BoxTest.SuspendLayout();
            groupBox1.SuspendLayout();
            boxBattery.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // tableRoot
            // 
            tableRoot.BackColor = Color.LemonChiffon;
            tableRoot.ColumnCount = 2;
            tableRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 400F));
            tableRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableRoot.Controls.Add(BoxTest, 0, 1);
            tableRoot.Controls.Add(boxBattery, 1, 1);
            tableRoot.Controls.Add(panel1, 0, 2);
            tableRoot.Controls.Add(label1, 0, 0);
            tableRoot.Dock = DockStyle.Fill;
            tableRoot.Location = new Point(0, 0);
            tableRoot.Name = "tableRoot";
            tableRoot.RowCount = 3;
            tableRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 8.67347F));
            tableRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 91.32653F));
            tableRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 57F));
            tableRoot.Size = new Size(1030, 736);
            tableRoot.TabIndex = 0;
            // 
            // BoxTest
            // 
            BoxTest.Controls.Add(groupBox1);
            BoxTest.Controls.Add(SequenceName);
            BoxTest.Dock = DockStyle.Fill;
            BoxTest.Location = new Point(3, 61);
            BoxTest.Name = "BoxTest";
            BoxTest.Size = new Size(394, 614);
            BoxTest.TabIndex = 0;
            BoxTest.TabStop = false;
            BoxTest.Text = "Test";
            BoxTest.Enter += BoxTest_Enter;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(panelHardwareList);
            groupBox1.Location = new Point(9, 92);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(365, 513);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "Hardware";
            // 
            // panelHardwareList
            // 
            panelHardwareList.Location = new Point(6, 25);
            panelHardwareList.Name = "panelHardwareList";
            panelHardwareList.Size = new Size(353, 482);
            panelHardwareList.TabIndex = 0;
            // 
            // SequenceName
            // 
            SequenceName.BackColor = Color.White;
            SequenceName.BorderColor = Color.Transparent;
            SequenceName.DefaultBackColor = Color.White;
            SequenceName.GridScale = 5;
            SequenceName.HoverColor = Color.Transparent;
            SequenceName.Location = new Point(6, 19);
            SequenceName.Margin = new Padding(0);
            SequenceName.Name = "SequenceName";
            SequenceName.SelectColor = Color.Orange;
            SequenceName.Selected = false;
            SequenceName.SignalText = "Name";
            SequenceName.SignalValue = "Unknown";
            SequenceName.Size = new Size(385, 58);
            SequenceName.SourceName = "Unknown";
            SequenceName.TabIndex = 0;
            SequenceName.Text = "stringSignalView1";
            // 
            // boxBattery
            // 
            boxBattery.Controls.Add(btnLoadBattery);
            boxBattery.Controls.Add(CutoffDischarge);
            boxBattery.Controls.Add(SetTemperature);
            boxBattery.Controls.Add(UpperOvershoot);
            boxBattery.Controls.Add(LowerOvershoot);
            boxBattery.Controls.Add(CutoffCharge);
            boxBattery.Controls.Add(NominalVoltage);
            boxBattery.Controls.Add(Capacity);
            boxBattery.Controls.Add(CellType);
            boxBattery.Controls.Add(SN);
            boxBattery.Controls.Add(Partnumber);
            boxBattery.Dock = DockStyle.Fill;
            boxBattery.Location = new Point(403, 61);
            boxBattery.Name = "boxBattery";
            boxBattery.Size = new Size(624, 614);
            boxBattery.TabIndex = 1;
            boxBattery.TabStop = false;
            boxBattery.Text = "Battery";
            // 
            // btnLoadBattery
            // 
            btnLoadBattery.BackColor = Color.FromArgb(224, 224, 224);
            btnLoadBattery.BorderColor = Color.Transparent;
            btnLoadBattery.ButtonIcon = "fa:file:black";
            btnLoadBattery.ButtonText = "Load Battery";
            btnLoadBattery.DefaultBackColor = Color.FromArgb(224, 224, 224);
            btnLoadBattery.GridScale = 5;
            btnLoadBattery.HoverColor = Color.Transparent;
            btnLoadBattery.IconSizeFactor = 0.6D;
            btnLoadBattery.LedMargin = 2;
            btnLoadBattery.LedSelectColor = Color.Red;
            btnLoadBattery.LedSize = 15;
            btnLoadBattery.LedVisible = false;
            btnLoadBattery.Location = new Point(9, 301);
            btnLoadBattery.Margin = new Padding(0);
            btnLoadBattery.Name = "btnLoadBattery";
            btnLoadBattery.SelectColor = Color.Orange;
            btnLoadBattery.Selected = false;
            btnLoadBattery.ShortcutText = "";
            btnLoadBattery.SignalValue = "Unknown";
            btnLoadBattery.Size = new Size(609, 52);
            btnLoadBattery.TabIndex = 2;
            btnLoadBattery.Text = "buttonWithIcon1";
            btnLoadBattery.Click += btnLoadBattery_Click;
            // 
            // CutoffDischarge
            // 
            CutoffDischarge.BackColor = Color.White;
            CutoffDischarge.BorderColor = Color.Transparent;
            CutoffDischarge.DefaultBackColor = Color.White;
            CutoffDischarge.GridScale = 5;
            CutoffDischarge.HoverColor = Color.Transparent;
            CutoffDischarge.Location = new Point(9, 134);
            CutoffDischarge.Margin = new Padding(0);
            CutoffDischarge.Name = "CutoffDischarge";
            CutoffDischarge.SelectColor = Color.Orange;
            CutoffDischarge.Selected = false;
            CutoffDischarge.SignalText = "CutoffDischarge";
            CutoffDischarge.SignalUnit = "V";
            CutoffDischarge.SignalValue = "23.5";
            CutoffDischarge.Size = new Size(199, 47);
            CutoffDischarge.SourceName = "Unknown";
            CutoffDischarge.TabIndex = 5;
            CutoffDischarge.Text = "signalView1";
            // 
            // SetTemperature
            // 
            SetTemperature.BackColor = Color.White;
            SetTemperature.BorderColor = Color.Transparent;
            SetTemperature.DefaultBackColor = Color.White;
            SetTemperature.GridScale = 5;
            SetTemperature.HoverColor = Color.Transparent;
            SetTemperature.Location = new Point(9, 244);
            SetTemperature.Margin = new Padding(0);
            SetTemperature.Name = "SetTemperature";
            SetTemperature.SelectColor = Color.Orange;
            SetTemperature.Selected = false;
            SetTemperature.SignalText = "SetTemperature";
            SetTemperature.SignalUnit = "°C";
            SetTemperature.SignalValue = "23.5";
            SetTemperature.Size = new Size(199, 47);
            SetTemperature.SourceName = "Unknown";
            SetTemperature.TabIndex = 6;
            SetTemperature.Text = "signalView1";
            // 
            // UpperOvershoot
            // 
            UpperOvershoot.BackColor = Color.White;
            UpperOvershoot.BorderColor = Color.Transparent;
            UpperOvershoot.DefaultBackColor = Color.White;
            UpperOvershoot.GridScale = 5;
            UpperOvershoot.HoverColor = Color.Transparent;
            UpperOvershoot.Location = new Point(215, 189);
            UpperOvershoot.Margin = new Padding(0);
            UpperOvershoot.Name = "UpperOvershoot";
            UpperOvershoot.SelectColor = Color.Orange;
            UpperOvershoot.Selected = false;
            UpperOvershoot.SignalText = "UpperOvershot";
            UpperOvershoot.SignalUnit = "V";
            UpperOvershoot.SignalValue = "23.5";
            UpperOvershoot.Size = new Size(199, 47);
            UpperOvershoot.SourceName = "Unknown";
            UpperOvershoot.TabIndex = 7;
            UpperOvershoot.Text = "signalView1";
            // 
            // LowerOvershoot
            // 
            LowerOvershoot.BackColor = Color.White;
            LowerOvershoot.BorderColor = Color.Transparent;
            LowerOvershoot.DefaultBackColor = Color.White;
            LowerOvershoot.GridScale = 5;
            LowerOvershoot.HoverColor = Color.Transparent;
            LowerOvershoot.Location = new Point(9, 189);
            LowerOvershoot.Margin = new Padding(0);
            LowerOvershoot.Name = "LowerOvershoot";
            LowerOvershoot.SelectColor = Color.Orange;
            LowerOvershoot.Selected = false;
            LowerOvershoot.SignalText = "LowerOvershot";
            LowerOvershoot.SignalUnit = "V";
            LowerOvershoot.SignalValue = "23.5";
            LowerOvershoot.Size = new Size(199, 47);
            LowerOvershoot.SourceName = "Unknown";
            LowerOvershoot.TabIndex = 8;
            LowerOvershoot.Text = "signalView1";
            // 
            // CutoffCharge
            // 
            CutoffCharge.BackColor = Color.White;
            CutoffCharge.BorderColor = Color.Transparent;
            CutoffCharge.DefaultBackColor = Color.White;
            CutoffCharge.GridScale = 5;
            CutoffCharge.HoverColor = Color.Transparent;
            CutoffCharge.Location = new Point(215, 134);
            CutoffCharge.Margin = new Padding(0);
            CutoffCharge.Name = "CutoffCharge";
            CutoffCharge.SelectColor = Color.Orange;
            CutoffCharge.Selected = false;
            CutoffCharge.SignalText = "CutoffCharge";
            CutoffCharge.SignalUnit = "V";
            CutoffCharge.SignalValue = "23.5";
            CutoffCharge.Size = new Size(199, 47);
            CutoffCharge.SourceName = "Unknown";
            CutoffCharge.TabIndex = 9;
            CutoffCharge.Text = "signalView1";
            // 
            // NominalVoltage
            // 
            NominalVoltage.BackColor = Color.White;
            NominalVoltage.BorderColor = Color.Transparent;
            NominalVoltage.DefaultBackColor = Color.White;
            NominalVoltage.GridScale = 5;
            NominalVoltage.HoverColor = Color.Transparent;
            NominalVoltage.Location = new Point(215, 79);
            NominalVoltage.Margin = new Padding(0);
            NominalVoltage.Name = "NominalVoltage";
            NominalVoltage.SelectColor = Color.Orange;
            NominalVoltage.Selected = false;
            NominalVoltage.SignalText = "NominalVoltage";
            NominalVoltage.SignalUnit = "°C";
            NominalVoltage.SignalValue = "23.5";
            NominalVoltage.Size = new Size(199, 47);
            NominalVoltage.SourceName = "Unknown";
            NominalVoltage.TabIndex = 10;
            NominalVoltage.Text = "signalView1";
            // 
            // Capacity
            // 
            Capacity.BackColor = Color.White;
            Capacity.BorderColor = Color.Transparent;
            Capacity.DefaultBackColor = Color.White;
            Capacity.GridScale = 5;
            Capacity.HoverColor = Color.Transparent;
            Capacity.Location = new Point(9, 79);
            Capacity.Margin = new Padding(0);
            Capacity.Name = "Capacity";
            Capacity.SelectColor = Color.Orange;
            Capacity.Selected = false;
            Capacity.SignalText = "Text";
            Capacity.SignalUnit = "°C";
            Capacity.SignalValue = "23.5";
            Capacity.Size = new Size(199, 47);
            Capacity.SourceName = "Unknown";
            Capacity.TabIndex = 11;
            Capacity.Text = "signalView1";
            // 
            // CellType
            // 
            CellType.BackColor = Color.White;
            CellType.BorderColor = Color.Transparent;
            CellType.DefaultBackColor = Color.White;
            CellType.GridScale = 5;
            CellType.HoverColor = Color.Transparent;
            CellType.Location = new Point(420, 24);
            CellType.Margin = new Padding(0);
            CellType.Name = "CellType";
            CellType.SelectColor = Color.Orange;
            CellType.Selected = false;
            CellType.SignalText = "CellType";
            CellType.SignalValue = "Unknown";
            CellType.Size = new Size(199, 47);
            CellType.SourceName = "Unknown";
            CellType.TabIndex = 2;
            CellType.Text = "Partnumber";
            // 
            // SN
            // 
            SN.BackColor = Color.White;
            SN.BorderColor = Color.Transparent;
            SN.DefaultBackColor = Color.White;
            SN.GridScale = 5;
            SN.HoverColor = Color.Transparent;
            SN.Location = new Point(215, 24);
            SN.Margin = new Padding(0);
            SN.Name = "SN";
            SN.SelectColor = Color.Orange;
            SN.Selected = false;
            SN.SignalText = "Serielnumber";
            SN.SignalValue = "Unknown";
            SN.Size = new Size(199, 47);
            SN.SourceName = "Unknown";
            SN.TabIndex = 3;
            SN.Text = "Partnumber";
            // 
            // Partnumber
            // 
            Partnumber.BackColor = Color.White;
            Partnumber.BorderColor = Color.Transparent;
            Partnumber.DefaultBackColor = Color.White;
            Partnumber.GridScale = 5;
            Partnumber.HoverColor = Color.Transparent;
            Partnumber.Location = new Point(9, 24);
            Partnumber.Margin = new Padding(0);
            Partnumber.Name = "Partnumber";
            Partnumber.SelectColor = Color.Orange;
            Partnumber.Selected = false;
            Partnumber.SignalText = "Partnumber";
            Partnumber.SignalValue = "Unknown";
            Partnumber.Size = new Size(199, 47);
            Partnumber.SourceName = "Unknown";
            Partnumber.TabIndex = 4;
            Partnumber.Text = "Partnumber";
            // 
            // panel1
            // 
            tableRoot.SetColumnSpan(panel1, 2);
            panel1.Controls.Add(btnCancel);
            panel1.Controls.Add(btnSet);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(3, 681);
            panel1.Name = "panel1";
            panel1.Size = new Size(1024, 52);
            panel1.TabIndex = 3;
            // 
            // btnCancel
            // 
            btnCancel.BackColor = Color.FromArgb(255, 128, 128);
            btnCancel.BorderColor = Color.Transparent;
            btnCancel.ButtonIcon = "fa:cancel:black";
            btnCancel.ButtonText = "Cancel";
            btnCancel.DefaultBackColor = Color.FromArgb(255, 128, 128);
            btnCancel.Dock = DockStyle.Right;
            btnCancel.GridScale = 5;
            btnCancel.HoverColor = Color.Transparent;
            btnCancel.IconSizeFactor = 0.6D;
            btnCancel.LedMargin = 2;
            btnCancel.LedSelectColor = Color.Red;
            btnCancel.LedSize = 15;
            btnCancel.LedVisible = false;
            btnCancel.Location = new Point(524, 0);
            btnCancel.Margin = new Padding(0);
            btnCancel.Name = "btnCancel";
            btnCancel.SelectColor = Color.Orange;
            btnCancel.Selected = false;
            btnCancel.ShortcutText = "";
            btnCancel.SignalValue = "Unknown";
            btnCancel.Size = new Size(250, 52);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "buttonWithIcon1";
            btnCancel.Click += btnCancel_Click;
            // 
            // btnSet
            // 
            btnSet.BackColor = Color.FromArgb(192, 255, 192);
            btnSet.BorderColor = Color.Transparent;
            btnSet.ButtonIcon = "fa:check:black";
            btnSet.ButtonText = "Set";
            btnSet.DefaultBackColor = Color.FromArgb(192, 255, 192);
            btnSet.Dock = DockStyle.Right;
            btnSet.GridScale = 5;
            btnSet.HoverColor = Color.Transparent;
            btnSet.IconSizeFactor = 0.6D;
            btnSet.LedMargin = 2;
            btnSet.LedSelectColor = Color.Red;
            btnSet.LedSize = 15;
            btnSet.LedVisible = false;
            btnSet.Location = new Point(774, 0);
            btnSet.Margin = new Padding(0);
            btnSet.Name = "btnSet";
            btnSet.SelectColor = Color.Orange;
            btnSet.Selected = false;
            btnSet.ShortcutText = "";
            btnSet.SignalValue = "Unknown";
            btnSet.Size = new Size(250, 52);
            btnSet.TabIndex = 2;
            btnSet.Text = "buttonWithIcon1";
            btnSet.Click += buttonWithIcon3_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            tableRoot.SetColumnSpan(label1, 2);
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            label1.Location = new Point(3, 0);
            label1.Name = "label1";
            label1.Size = new Size(1024, 58);
            label1.TabIndex = 4;
            label1.Text = "Testsequencer config";
            label1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // FormAddTest
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlDark;
            ClientSize = new Size(1030, 736);
            Controls.Add(tableRoot);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormAddTest";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "FormAddTest";
            tableRoot.ResumeLayout(false);
            tableRoot.PerformLayout();
            BoxTest.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            boxBattery.ResumeLayout(false);
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableRoot;
        private GroupBox BoxTest;
        private Controls.StringSignalView SequenceName;
        private GroupBox boxBattery;
        private Controls.SignalView CutoffDischarge;
        private Controls.SignalView SetTemperature;
        private Controls.SignalView UpperOvershoot;
        private Controls.SignalView LowerOvershoot;
        private Controls.SignalView CutoffCharge;
        private Controls.SignalView NominalVoltage;
        private Controls.SignalView Capacity;
        private Controls.StringSignalView CellType;
        private Controls.StringSignalView SN;
        private Controls.StringSignalView Partnumber;
        private GroupBox groupBox1;
        private Panel panelHardwareList;
        private Controls.ButtonWithIcon btnSet;
        private Controls.ButtonWithIcon btnLoadBattery;
        private Panel panel1;
        private Controls.ButtonWithIcon btnCancel;
        private Label label1;
    }
}