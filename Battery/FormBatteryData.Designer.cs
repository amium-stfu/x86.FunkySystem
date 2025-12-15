namespace FunkySystem.Devices
{
    partial class FormBatteryData
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
            button1 = new Button();
            tableLayoutPanel1 = new TableLayoutPanel();
            TabBatteryData = new TabControl();
            tabMain = new TabPage();
            displayMain = new FunkySystem.Controls.Display();
            tabPage2 = new TabPage();
            displayPhysicalSpecificationsData = new FunkySystem.Controls.Display();
            tabPage1 = new TabPage();
            displayInner = new FunkySystem.Controls.Display();
            tabPage3 = new TabPage();
            displayThermal = new FunkySystem.Controls.Display();
            tabPage4 = new TabPage();
            displayElectric = new FunkySystem.Controls.Display();
            button2 = new Button();
            tableLayoutPanel1.SuspendLayout();
            TabBatteryData.SuspendLayout();
            tabMain.SuspendLayout();
            tabPage2.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage3.SuspendLayout();
            tabPage4.SuspendLayout();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(729, 3);
            button1.Name = "button1";
            button1.Size = new Size(68, 23);
            button1.TabIndex = 0;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 90.86651F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 9.13349F));
            tableLayoutPanel1.Controls.Add(TabBatteryData, 0, 0);
            tableLayoutPanel1.Controls.Add(button1, 1, 0);
            tableLayoutPanel1.Controls.Add(button2, 1, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 94.85149F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 5.14851475F));
            tableLayoutPanel1.Size = new Size(800, 505);
            tableLayoutPanel1.TabIndex = 2;
            // 
            // TabBatteryData
            // 
            TabBatteryData.Controls.Add(tabMain);
            TabBatteryData.Controls.Add(tabPage2);
            TabBatteryData.Controls.Add(tabPage1);
            TabBatteryData.Controls.Add(tabPage3);
            TabBatteryData.Controls.Add(tabPage4);
            TabBatteryData.Dock = DockStyle.Fill;
            TabBatteryData.Location = new Point(3, 3);
            TabBatteryData.Name = "TabBatteryData";
            TabBatteryData.SelectedIndex = 0;
            TabBatteryData.Size = new Size(720, 473);
            TabBatteryData.TabIndex = 1;
            // 
            // tabMain
            // 
            tabMain.Controls.Add(displayMain);
            tabMain.Location = new Point(4, 24);
            tabMain.Name = "tabMain";
            tabMain.Padding = new Padding(3);
            tabMain.Size = new Size(712, 445);
            tabMain.TabIndex = 0;
            tabMain.Text = "Main";
            tabMain.UseVisualStyleBackColor = true;
            // 
            // displayMain
            // 
            displayMain.BorderColor = Color.Black;
            displayMain.ContentBackColor = Color.Empty;
            displayMain.ContentForeColor = Color.Empty;
            // 
            // 
            // 
            displayMain.ContentPanel.AutoScroll = true;
            displayMain.ContentPanel.BackColor = SystemColors.Control;
            displayMain.ContentPanel.Dock = DockStyle.Fill;
            displayMain.ContentPanel.Location = new Point(3, 31);
            displayMain.ContentPanel.Name = "";
            displayMain.ContentPanel.Padding = new Padding(6);
            displayMain.ContentPanel.Size = new Size(700, 405);
            displayMain.ContentPanel.TabIndex = 0;
            displayMain.DefaultBackColor = Color.Empty;
            displayMain.Dock = DockStyle.Fill;
            displayMain.GridScale = 5;
            displayMain.HoverColor = Color.Transparent;
            displayMain.Location = new Point(3, 3);
            displayMain.Margin = new Padding(0);
            displayMain.Name = "displayMain";
            displayMain.SelectColor = Color.Orange;
            displayMain.Selected = false;
            displayMain.ShowMenuIcon = false;
            displayMain.Size = new Size(706, 439);
            displayMain.TabIndex = 2;
            displayMain.Text = "display1";
            displayMain.TitleText = "";
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(displayPhysicalSpecificationsData);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(712, 445);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "PhysicalSpecificationsData";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // displayPhysicalSpecificationsData
            // 
            displayPhysicalSpecificationsData.BorderColor = Color.Black;
            displayPhysicalSpecificationsData.ContentBackColor = Color.Empty;
            displayPhysicalSpecificationsData.ContentForeColor = Color.Empty;
            // 
            // 
            // 
            displayPhysicalSpecificationsData.ContentPanel.AutoScroll = true;
            displayPhysicalSpecificationsData.ContentPanel.BackColor = SystemColors.Control;
            displayPhysicalSpecificationsData.ContentPanel.Dock = DockStyle.Fill;
            displayPhysicalSpecificationsData.ContentPanel.Location = new Point(3, 31);
            displayPhysicalSpecificationsData.ContentPanel.Name = "";
            displayPhysicalSpecificationsData.ContentPanel.Padding = new Padding(6);
            displayPhysicalSpecificationsData.ContentPanel.Size = new Size(700, 405);
            displayPhysicalSpecificationsData.ContentPanel.TabIndex = 0;
            displayPhysicalSpecificationsData.DefaultBackColor = Color.Empty;
            displayPhysicalSpecificationsData.Dock = DockStyle.Fill;
            displayPhysicalSpecificationsData.GridScale = 5;
            displayPhysicalSpecificationsData.HoverColor = Color.Transparent;
            displayPhysicalSpecificationsData.Location = new Point(3, 3);
            displayPhysicalSpecificationsData.Margin = new Padding(0);
            displayPhysicalSpecificationsData.Name = "displayPhysicalSpecificationsData";
            displayPhysicalSpecificationsData.SelectColor = Color.Orange;
            displayPhysicalSpecificationsData.Selected = false;
            displayPhysicalSpecificationsData.ShowMenuIcon = false;
            displayPhysicalSpecificationsData.Size = new Size(706, 439);
            displayPhysicalSpecificationsData.TabIndex = 0;
            displayPhysicalSpecificationsData.Text = "display1";
            displayPhysicalSpecificationsData.TitleText = "";
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(displayInner);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(712, 445);
            tabPage1.TabIndex = 2;
            tabPage1.Text = " InnerCellMaterials";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // displayInner
            // 
            displayInner.BorderColor = Color.Black;
            displayInner.ContentBackColor = Color.Empty;
            displayInner.ContentForeColor = Color.Empty;
            // 
            // 
            // 
            displayInner.ContentPanel.AutoScroll = true;
            displayInner.ContentPanel.BackColor = SystemColors.Control;
            displayInner.ContentPanel.Dock = DockStyle.Fill;
            displayInner.ContentPanel.Location = new Point(3, 31);
            displayInner.ContentPanel.Name = "";
            displayInner.ContentPanel.Padding = new Padding(6);
            displayInner.ContentPanel.Size = new Size(700, 405);
            displayInner.ContentPanel.TabIndex = 0;
            displayInner.DefaultBackColor = Color.Empty;
            displayInner.Dock = DockStyle.Fill;
            displayInner.GridScale = 5;
            displayInner.HoverColor = Color.Transparent;
            displayInner.Location = new Point(3, 3);
            displayInner.Margin = new Padding(0);
            displayInner.Name = "displayInner";
            displayInner.SelectColor = Color.Orange;
            displayInner.Selected = false;
            displayInner.ShowMenuIcon = false;
            displayInner.Size = new Size(706, 439);
            displayInner.TabIndex = 0;
            displayInner.Text = "display1";
            displayInner.TitleText = "";
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(displayThermal);
            tabPage3.Location = new Point(4, 24);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3);
            tabPage3.Size = new Size(712, 445);
            tabPage3.TabIndex = 3;
            tabPage3.Text = "ThermalProperties";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // displayThermal
            // 
            displayThermal.BorderColor = Color.Black;
            displayThermal.ContentBackColor = Color.Empty;
            displayThermal.ContentForeColor = Color.Empty;
            // 
            // 
            // 
            displayThermal.ContentPanel.AutoScroll = true;
            displayThermal.ContentPanel.BackColor = SystemColors.Control;
            displayThermal.ContentPanel.Dock = DockStyle.Fill;
            displayThermal.ContentPanel.Location = new Point(3, 31);
            displayThermal.ContentPanel.Name = "";
            displayThermal.ContentPanel.Padding = new Padding(6);
            displayThermal.ContentPanel.Size = new Size(700, 405);
            displayThermal.ContentPanel.TabIndex = 0;
            displayThermal.DefaultBackColor = Color.Empty;
            displayThermal.Dock = DockStyle.Fill;
            displayThermal.GridScale = 5;
            displayThermal.HoverColor = Color.Transparent;
            displayThermal.Location = new Point(3, 3);
            displayThermal.Margin = new Padding(0);
            displayThermal.Name = "displayThermal";
            displayThermal.SelectColor = Color.Orange;
            displayThermal.Selected = false;
            displayThermal.ShowMenuIcon = false;
            displayThermal.Size = new Size(706, 439);
            displayThermal.TabIndex = 0;
            displayThermal.Text = "display1";
            displayThermal.TitleText = "";
            // 
            // tabPage4
            // 
            tabPage4.Controls.Add(displayElectric);
            tabPage4.Location = new Point(4, 24);
            tabPage4.Name = "tabPage4";
            tabPage4.Padding = new Padding(3);
            tabPage4.Size = new Size(712, 445);
            tabPage4.TabIndex = 4;
            tabPage4.Text = "ElectricalProperties";
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // displayElectric
            // 
            displayElectric.BorderColor = Color.Black;
            displayElectric.ContentBackColor = Color.Empty;
            displayElectric.ContentForeColor = Color.Empty;
            // 
            // 
            // 
            displayElectric.ContentPanel.AutoScroll = true;
            displayElectric.ContentPanel.BackColor = SystemColors.Control;
            displayElectric.ContentPanel.Dock = DockStyle.Fill;
            displayElectric.ContentPanel.Location = new Point(3, 31);
            displayElectric.ContentPanel.Name = "";
            displayElectric.ContentPanel.Padding = new Padding(6);
            displayElectric.ContentPanel.Size = new Size(700, 405);
            displayElectric.ContentPanel.TabIndex = 0;
            displayElectric.DefaultBackColor = Color.Empty;
            displayElectric.Dock = DockStyle.Fill;
            displayElectric.GridScale = 5;
            displayElectric.HoverColor = Color.Transparent;
            displayElectric.Location = new Point(3, 3);
            displayElectric.Margin = new Padding(0);
            displayElectric.Name = "displayElectric";
            displayElectric.SelectColor = Color.Orange;
            displayElectric.Selected = false;
            displayElectric.ShowMenuIcon = false;
            displayElectric.Size = new Size(706, 439);
            displayElectric.TabIndex = 0;
            displayElectric.Text = "display1";
            displayElectric.TitleText = "";
            // 
            // button2
            // 
            button2.Location = new Point(729, 482);
            button2.Name = "button2";
            button2.Size = new Size(68, 20);
            button2.TabIndex = 2;
            button2.Text = "button2";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // FormBatteryData
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 505);
            Controls.Add(tableLayoutPanel1);
            Name = "FormBatteryData";
            Text = "FormBatteryData";
            FormClosed += FormBatteryData_FormClosed;
            tableLayoutPanel1.ResumeLayout(false);
            TabBatteryData.ResumeLayout(false);
            tabMain.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage3.ResumeLayout(false);
            tabPage4.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Button button1;
        private TableLayoutPanel tableLayoutPanel1;
        private TabControl TabBatteryData;
        private TabPage tabMain;
        private TabPage tabPage2;
        private Button button2;
        private Controls.Display displayPhysicalSpecificationsData;
        private Controls.Display displayMain;
        private TabPage tabPage1;
        private Controls.Display displayInner;
        private TabPage tabPage3;
        private Controls.Display displayThermal;
        private TabPage tabPage4;
        private Controls.Display displayElectric;
    }
}