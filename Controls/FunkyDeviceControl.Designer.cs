namespace FunkySystem.Controls
{
    partial class FunkyDeviceControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>


        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tableMain = new TableLayoutPanel();
            panelBottom = new Panel();
            tableBottom = new TableLayoutPanel();
            PanelSequence = new Display();
            LogPanel = new Display();
            PanelDut = new Display();
            ControlSelected = new Panel();
            tableData = new TableLayoutPanel();
            btnState = new ButtonWithIcon();
            DisplayData = new Display();
            tableMain.SuspendLayout();
            panelBottom.SuspendLayout();
            tableBottom.SuspendLayout();
            tableData.SuspendLayout();
            SuspendLayout();
            // 
            // tableMain
            // 
            tableMain.BackColor = Color.White;
            tableMain.ColumnCount = 3;
            tableMain.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 550F));
            tableMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableMain.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60F));
            tableMain.Controls.Add(panelBottom, 0, 3);
            tableMain.Controls.Add(ControlSelected, 1, 0);
            tableMain.Controls.Add(tableData, 0, 0);
            tableMain.Dock = DockStyle.Fill;
            tableMain.Location = new Point(0, 0);
            tableMain.Name = "tableMain";
            tableMain.RowCount = 4;
            tableMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 282F));
            tableMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 6F));
            tableMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 256F));
            tableMain.Size = new Size(1497, 902);
            tableMain.TabIndex = 0;
            // 
            // panelBottom
            // 
            panelBottom.BackColor = Color.White;
            tableMain.SetColumnSpan(panelBottom, 3);
            panelBottom.Controls.Add(tableBottom);
            panelBottom.Dock = DockStyle.Fill;
            panelBottom.Location = new Point(0, 646);
            panelBottom.Margin = new Padding(0);
            panelBottom.Name = "panelBottom";
            panelBottom.Size = new Size(1497, 256);
            panelBottom.TabIndex = 0;
            // 
            // tableBottom
            // 
            tableBottom.ColumnCount = 3;
            tableBottom.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 550F));
            tableBottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableBottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableBottom.Controls.Add(PanelSequence, 2, 0);
            tableBottom.Controls.Add(LogPanel, 1, 0);
            tableBottom.Controls.Add(PanelDut, 0, 0);
            tableBottom.Dock = DockStyle.Fill;
            tableBottom.Location = new Point(0, 0);
            tableBottom.Margin = new Padding(0);
            tableBottom.Name = "tableBottom";
            tableBottom.RowCount = 1;
            tableBottom.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableBottom.Size = new Size(1497, 256);
            tableBottom.TabIndex = 0;
            // 
            // PanelSequence
            // 
            PanelSequence.BackColor = Color.WhiteSmoke;
            PanelSequence.BorderColor = Color.Transparent;
            PanelSequence.ContentBackColor = Color.White;
            PanelSequence.ContentForeColor = Color.White;
            // 
            // 
            // 
            PanelSequence.ContentPanel.AutoScroll = true;
            PanelSequence.ContentPanel.BackColor = Color.WhiteSmoke;
            PanelSequence.ContentPanel.FlowDirection = FlowDirection.TopDown;
            PanelSequence.ContentPanel.Location = new Point(3, 31);
            PanelSequence.ContentPanel.Name = "";
            PanelSequence.ContentPanel.Padding = new Padding(6);
            PanelSequence.ContentPanel.Size = new Size(465, 222);
            PanelSequence.ContentPanel.TabIndex = 0;
            PanelSequence.ContentPanel.WrapContents = false;
            PanelSequence.DefaultBackColor = Color.WhiteSmoke;
            PanelSequence.Dock = DockStyle.Fill;
            PanelSequence.GridScale = 5;
            PanelSequence.HoverColor = Color.Transparent;
            PanelSequence.Location = new Point(1026, 0);
            PanelSequence.Margin = new Padding(3, 0, 0, 0);
            PanelSequence.Name = "PanelSequence";
            PanelSequence.SelectColor = Color.Orange;
            PanelSequence.Selected = false;
            PanelSequence.ShowMenuIcon = true;
            PanelSequence.Size = new Size(471, 256);
            PanelSequence.TabIndex = 1;
            PanelSequence.Text = "infoPanel2";
            PanelSequence.TitleText = "Sequence";
            // 
            // LogPanel
            // 
            LogPanel.BackColor = Color.WhiteSmoke;
            LogPanel.BorderColor = Color.Transparent;
            LogPanel.ContentBackColor = Color.White;
            LogPanel.ContentForeColor = Color.White;
            // 
            // 
            // 
            LogPanel.ContentPanel.AutoScroll = true;
            LogPanel.ContentPanel.BackColor = Color.WhiteSmoke;
            LogPanel.ContentPanel.FlowDirection = FlowDirection.TopDown;
            LogPanel.ContentPanel.Location = new Point(3, 31);
            LogPanel.ContentPanel.Name = "";
            LogPanel.ContentPanel.Padding = new Padding(6);
            LogPanel.ContentPanel.Size = new Size(461, 222);
            LogPanel.ContentPanel.TabIndex = 0;
            LogPanel.ContentPanel.WrapContents = false;
            LogPanel.DefaultBackColor = Color.WhiteSmoke;
            LogPanel.Dock = DockStyle.Fill;
            LogPanel.GridScale = 5;
            LogPanel.HoverColor = Color.Transparent;
            LogPanel.Location = new Point(553, 0);
            LogPanel.Margin = new Padding(3, 0, 3, 0);
            LogPanel.Name = "LogPanel";
            LogPanel.SelectColor = Color.Orange;
            LogPanel.Selected = false;
            LogPanel.ShowMenuIcon = true;
            LogPanel.Size = new Size(467, 256);
            LogPanel.TabIndex = 2;
            LogPanel.Text = "infoPanel1";
            LogPanel.TitleText = "Log";
            LogPanel.Resize += LogPanel_Resize;
            // 
            // PanelDut
            // 
            PanelDut.BackColor = Color.WhiteSmoke;
            PanelDut.BorderColor = Color.Transparent;
            PanelDut.ContentBackColor = Color.WhiteSmoke;
            PanelDut.ContentForeColor = Color.Black;
            // 
            // 
            // 
            PanelDut.ContentPanel.BackColor = Color.WhiteSmoke;
            PanelDut.ContentPanel.Dock = DockStyle.Fill;
            PanelDut.ContentPanel.Location = new Point(3, 31);
            PanelDut.ContentPanel.Name = "";
            PanelDut.ContentPanel.Padding = new Padding(6);
            PanelDut.ContentPanel.Size = new Size(544, 222);
            PanelDut.ContentPanel.TabIndex = 0;
            PanelDut.DefaultBackColor = Color.WhiteSmoke;
            PanelDut.Dock = DockStyle.Fill;
            PanelDut.GridScale = 5;
            PanelDut.HoverColor = Color.Transparent;
            PanelDut.Location = new Point(0, 0);
            PanelDut.Margin = new Padding(0);
            PanelDut.Name = "PanelDut";
            PanelDut.SelectColor = Color.Orange;
            PanelDut.Selected = false;
            PanelDut.ShowMenuIcon = true;
            PanelDut.Size = new Size(550, 256);
            PanelDut.TabIndex = 3;
            PanelDut.Text = "display1";
            PanelDut.TitleText = "Device under test";
            // 
            // ControlSelected
            // 
            ControlSelected.BackColor = Color.WhiteSmoke;
            ControlSelected.Dock = DockStyle.Fill;
            ControlSelected.Location = new Point(553, 2);
            ControlSelected.Margin = new Padding(3, 2, 0, 3);
            ControlSelected.Name = "ControlSelected";
            tableMain.SetRowSpan(ControlSelected, 2);
            ControlSelected.Size = new Size(884, 635);
            ControlSelected.TabIndex = 3;
            // 
            // tableData
            // 
            tableData.BackColor = Color.Transparent;
            tableData.ColumnCount = 1;
            tableData.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableData.Controls.Add(btnState, 0, 0);
            tableData.Controls.Add(DisplayData, 0, 1);
            tableData.Dock = DockStyle.Fill;
            tableData.Location = new Point(3, 3);
            tableData.Name = "tableData";
            tableData.Padding = new Padding(10, 0, 0, 0);
            tableData.RowCount = 2;
            tableMain.SetRowSpan(tableData, 2);
            tableData.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F));
            tableData.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableData.Size = new Size(544, 634);
            tableData.TabIndex = 4;
            // 
            // btnState
            // 
            btnState.BackColor = Color.WhiteSmoke;
            btnState.BorderColor = Color.Transparent;
            btnState.ButtonIcon = "fa:circle-play:black";
            btnState.ButtonText = "Unknown";
            btnState.DefaultBackColor = Color.WhiteSmoke;
            btnState.Dock = DockStyle.Fill;
            btnState.GridScale = 5;
            btnState.HoverColor = Color.Transparent;
            btnState.IconSizeFactor = 0.8D;
            btnState.LedMargin = 2;
            btnState.LedSelectColor = Color.Red;
            btnState.LedSize = 15;
            btnState.LedVisible = false;
            btnState.Location = new Point(10, 0);
            btnState.Margin = new Padding(0);
            btnState.Name = "btnState";
            btnState.SelectColor = Color.Orange;
            btnState.Selected = false;
            btnState.ShortcutText = "";
            btnState.SignalValue = "Unknown";
            btnState.Size = new Size(534, 80);
            btnState.TabIndex = 0;
            btnState.Text = "buttonWithIcon10";
            // 
            // DisplayData
            // 
            DisplayData.BackColor = Color.WhiteSmoke;
            DisplayData.BorderColor = Color.Transparent;
            DisplayData.ContentBackColor = Color.White;
            DisplayData.ContentForeColor = Color.Black;
            // 
            // 
            // 
            DisplayData.ContentPanel.BackColor = Color.WhiteSmoke;
            DisplayData.ContentPanel.Dock = DockStyle.Fill;
            DisplayData.ContentPanel.Location = new Point(3, 31);
            DisplayData.ContentPanel.Name = "";
            DisplayData.ContentPanel.Padding = new Padding(3);
            DisplayData.ContentPanel.Size = new Size(528, 520);
            DisplayData.ContentPanel.TabIndex = 0;
            DisplayData.DefaultBackColor = Color.WhiteSmoke;
            DisplayData.Dock = DockStyle.Fill;
            DisplayData.GridScale = 5;
            DisplayData.HoverColor = Color.Transparent;
            DisplayData.Location = new Point(10, 80);
            DisplayData.Margin = new Padding(0);
            DisplayData.Name = "DisplayData";
            DisplayData.SelectColor = Color.Orange;
            DisplayData.Selected = false;
            DisplayData.ShowMenuIcon = true;
            DisplayData.Size = new Size(534, 554);
            DisplayData.TabIndex = 1;
            DisplayData.Text = "display1";
            DisplayData.TitleText = "";
            // 
            // FunkyDeviceControl
            // 
            Controls.Add(tableMain);
            Name = "FunkyDeviceControl";
            Size = new Size(1497, 902);
            Load += FunkyDeviceControl_Load;
            tableMain.ResumeLayout(false);
            panelBottom.ResumeLayout(false);
            tableBottom.ResumeLayout(false);
            tableData.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private TableLayoutPanel tableMain;
        public Panel panelBottom;
        public Panel ControlSelected;
        public ButtonWithIcon btnState;
        private TableLayoutPanel tableBottom;
        public Display PanelSequence;
        public Display LogPanel;
        private TableLayoutPanel tableData;
        public Display DisplayData;
        public Display PanelDut;
       
    }
}
