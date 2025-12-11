using FontAwesome.Sharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FunkySystem.Controls
{
    public partial class FormMenu : Form
    {
        public FormMenu()
        {
            InitializeComponent();
            KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    DialogResult = DialogResult.Cancel;
                    Close();
                }
            };

            PanelButtons.PreviewKeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    DialogResult = DialogResult.Cancel;
                    Close();
                }
            };
        }

        public void Add(string text, string icon, Action OnClick)
        {
            ButtonWithIcon btn = new ButtonWithIcon();
            btn.BorderColor = Color.Transparent;
            btn.ButtonIcon = icon;
            btn.ButtonText = text;
            btn.Dock = DockStyle.Top;
            btn.HoverColor = Color.DarkOrange;
            btn.Name = "iconButton5";
            btn.ShortcutText = "";
            btn.SignalValue = "Unknown";
            btn.Size = new Size(PanelButtons.Width, 45);
            btn.TabIndex = 2;
            btn.LedVisible = false;
            btn.Text = "iconButton5";
            btn.Click += (s, e) => { OnClick(); DialogResult = DialogResult.OK; Close(); };
            btn.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    DialogResult = DialogResult.Cancel;
                    Close();
                }
            };

            btn.MouseEnter += (s, e) =>
            {
                if (!entered)
                {
                    entered = true;
                }
            };

            PanelButtons.Controls.Add(btn);

            Height = PanelButtons.Controls.Count * 45 + 20;
        }

        private void FormMenu_MouseLeave(object sender, EventArgs e)
        {
            if (entered)
                Close();
        }

        bool entered = false;
        private void FormMenu_MouseEnter(object sender, EventArgs e)
        {

        }

        private void PanelButtons_MouseEnter(object sender, EventArgs e)
        {
            if (!entered)
            {
                entered = true;
            }
        }
    }
}
