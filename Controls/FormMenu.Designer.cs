namespace FunkySystem.Controls
{
    partial class FormMenu
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
            PanelButtons = new FlowLayoutPanel();
            SuspendLayout();
            // 
            // PanelButtons
            // 
            PanelButtons.BackColor = Color.White;
            PanelButtons.Dock = DockStyle.Fill;
            PanelButtons.Location = new Point(10, 10);
            PanelButtons.Margin = new Padding(0);
            PanelButtons.Name = "PanelButtons";
            PanelButtons.Size = new Size(307, 430);
            PanelButtons.TabIndex = 0;
            PanelButtons.MouseEnter += PanelButtons_MouseEnter;
            // 
            // FormMenu
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Gainsboro;
            ClientSize = new Size(327, 450);
            Controls.Add(PanelButtons);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormMenu";
            Padding = new Padding(10);
            StartPosition = FormStartPosition.Manual;
            Text = "FormMenu";
            MouseEnter += FormMenu_MouseEnter;
            MouseLeave += FormMenu_MouseLeave;
            ResumeLayout(false);
        }

        #endregion

        private FlowLayoutPanel PanelButtons;
    }
}