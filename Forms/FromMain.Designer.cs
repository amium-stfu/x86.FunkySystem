namespace FunkySystem
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            TableRoot = new TableLayoutPanel();
            PanelTestList = new FunkySystem.Controls.RadioButtonFlowPanel();
            btnAddTest = new FunkySystem.Controls.ButtonWithIcon();
            panelHeader = new Panel();
            LabelHeader = new Label();
            pictureBox1 = new PictureBox();
            lineFooter = new Label();
            PanelView = new Panel();
            PanelMainMenu = new FunkySystem.Controls.RadioButtonFlowPanel();
            button1 = new Button();
            lineHeader = new Label();
            TableRoot.SuspendLayout();
            PanelTestList.SuspendLayout();
            panelHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // TableRoot
            // 
            TableRoot.BackColor = Color.White;
            TableRoot.ColumnCount = 1;
            TableRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 82F));
            TableRoot.Controls.Add(PanelTestList, 0, 3);
            TableRoot.Controls.Add(panelHeader, 0, 0);
            TableRoot.Controls.Add(lineFooter, 0, 5);
            TableRoot.Controls.Add(PanelView, 0, 4);
            TableRoot.Controls.Add(PanelMainMenu, 0, 2);
            TableRoot.Controls.Add(button1, 0, 6);
            TableRoot.Controls.Add(lineHeader, 0, 1);
            TableRoot.Dock = DockStyle.Fill;
            TableRoot.Location = new Point(0, 0);
            TableRoot.Name = "TableRoot";
            TableRoot.RowCount = 7;
            TableRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 66F));
            TableRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 2F));
            TableRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            TableRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            TableRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            TableRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 2F));
            TableRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 31F));
            TableRoot.Size = new Size(1331, 673);
            TableRoot.TabIndex = 0;
            TableRoot.Paint += TableRoot_Paint;
            // 
            // PanelTestList
            // 
            PanelTestList.ButtonBackColor = Color.LightGray;
            PanelTestList.ButtonBorderColor = Color.DarkGray;
            PanelTestList.ButtonForeColor = Color.Black;
            PanelTestList.ButtonWidth = 250;
            PanelTestList.Controls.Add(btnAddTest);
            PanelTestList.DefaultColor = Color.DarkGray;
            PanelTestList.Dock = DockStyle.Fill;
            PanelTestList.Location = new Point(10, 121);
            PanelTestList.Margin = new Padding(10, 3, 0, 3);
            PanelTestList.Name = "PanelTestList";
            PanelTestList.SelectColor = Color.GreenYellow;
            PanelTestList.Size = new Size(1321, 44);
            PanelTestList.TabIndex = 15;
            PanelTestList.WrapContents = false;
            // 
            // btnAddTest
            // 
            btnAddTest.BackColor = Color.LightGray;
            btnAddTest.BorderColor = Color.Transparent;
            btnAddTest.ButtonIcon = "fa:plus:black";
            btnAddTest.ButtonText = "";
            btnAddTest.GridScale = 5;
            btnAddTest.HoverColor = Color.WhiteSmoke;
            btnAddTest.IconSizeFactor = 0.8D;
            btnAddTest.LedMargin = 2;
            btnAddTest.LedSelectColor = Color.Red;
            btnAddTest.LedSize = 15;
            btnAddTest.LedVisible = false;
            btnAddTest.Location = new Point(0, 1);
            btnAddTest.Margin = new Padding(0, 1, 0, 0);
            btnAddTest.Name = "btnAddTest";
            btnAddTest.ShortcutText = "";
            btnAddTest.SignalValue = "Unknown";
            btnAddTest.Size = new Size(44, 44);
            btnAddTest.TabIndex = 0;
            btnAddTest.Text = "buttonWithIcon1";
            // 
            // panelHeader
            // 
            panelHeader.Controls.Add(LabelHeader);
            panelHeader.Controls.Add(pictureBox1);
            panelHeader.Dock = DockStyle.Fill;
            panelHeader.Location = new Point(10, 3);
            panelHeader.Margin = new Padding(10, 3, 10, 3);
            panelHeader.Name = "panelHeader";
            panelHeader.Size = new Size(1311, 60);
            panelHeader.TabIndex = 1;
            // 
            // LabelHeader
            // 
            LabelHeader.Dock = DockStyle.Left;
            LabelHeader.Font = new Font("Segoe UI", 26.25F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            LabelHeader.Location = new Point(0, 0);
            LabelHeader.Name = "LabelHeader";
            LabelHeader.Size = new Size(482, 60);
            LabelHeader.TabIndex = 1;
            LabelHeader.Text = "Battery Testbench System";
            LabelHeader.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pictureBox1
            // 
            pictureBox1.Dock = DockStyle.Right;
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(1111, 0);
            pictureBox1.Margin = new Padding(0, 3, 10, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(200, 60);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // lineFooter
            // 
            lineFooter.AutoSize = true;
            lineFooter.BackColor = Color.Black;
            lineFooter.Dock = DockStyle.Fill;
            lineFooter.Location = new Point(3, 640);
            lineFooter.Name = "lineFooter";
            lineFooter.Size = new Size(1325, 2);
            lineFooter.TabIndex = 2;
            lineFooter.Text = "label1";
            // 
            // PanelView
            // 
            PanelView.Dock = DockStyle.Fill;
            PanelView.Location = new Point(0, 168);
            PanelView.Margin = new Padding(0);
            PanelView.Name = "PanelView";
            PanelView.Size = new Size(1331, 472);
            PanelView.TabIndex = 12;
            // 
            // PanelMainMenu
            // 
            PanelMainMenu.ButtonBackColor = Color.LightGray;
            PanelMainMenu.ButtonBorderColor = Color.Transparent;
            PanelMainMenu.ButtonForeColor = Color.Black;
            PanelMainMenu.ButtonWidth = 280;
            PanelMainMenu.DefaultColor = Color.DarkGray;
            PanelMainMenu.Dock = DockStyle.Fill;
            PanelMainMenu.Location = new Point(10, 69);
            PanelMainMenu.Margin = new Padding(10, 1, 10, 0);
            PanelMainMenu.Name = "PanelMainMenu";
            PanelMainMenu.SelectColor = Color.Orange;
            PanelMainMenu.Size = new Size(1311, 49);
            PanelMainMenu.TabIndex = 13;
            PanelMainMenu.WrapContents = false;
            // 
            // button1
            // 
            button1.Location = new Point(3, 645);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 14;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // lineHeader
            // 
            lineHeader.AutoSize = true;
            lineHeader.BackColor = Color.Black;
            lineHeader.Dock = DockStyle.Fill;
            lineHeader.Location = new Point(3, 66);
            lineHeader.Name = "lineHeader";
            lineHeader.Size = new Size(1325, 2);
            lineHeader.TabIndex = 2;
            lineHeader.Text = "label1";
            // 
            // FormMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1331, 673);
            Controls.Add(TableRoot);
            Name = "FormMain";
            Text = "Form1";
            TableRoot.ResumeLayout(false);
            TableRoot.PerformLayout();
            PanelTestList.ResumeLayout(false);
            panelHeader.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        internal TableLayoutPanel TableRoot;
        internal Panel panelHeader;
        internal PictureBox pictureBox1;
        internal Label lineFooter;
        internal Label LabelHeader;
        internal Panel PanelView;
        internal Controls.RadioButtonFlowPanel PanelMainMenu;
        internal Button button1;
        internal Controls.RadioButtonFlowPanel PanelTestList;
        internal Controls.ButtonWithIcon btnAddTest;
        internal Label lineHeader;
    }
}
