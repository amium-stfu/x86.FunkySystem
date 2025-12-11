using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Schema;
using static System.Net.Mime.MediaTypeNames;

namespace FunkySystem.UI
{
    internal partial class FormKeyboard : Form
    {
        string[] qwertz = {
            "q", "w", "e", "r", "t", "z", "u", "i", "o", "p",
            "<sep>", "a", "s", "d", "f", "g", "h", "j", "k", "l", "<sep>",
            "<capslock>", "y", "x", "c", "v", "b", "n", "m", ".", ",", "_","/", "<space>", "+", "-", "°"
        };

        public Func<string> Getter = null;
        public Action<string> Setter = null;

        bool DarkMode = false;
        bool _copyPast = false;

        string dummy = "";

        public FormKeyboard()
        {
            InitializeComponent();
            create();
            Getter = () => dummy;
            Setter = (s) => dummy = s;
            Text = "Keyboard Input";

        }



        public FormKeyboard(Func<string> getter, Action<string> setter, string text, bool copyPast = false)
        {
            InitializeComponent();
            _copyPast = copyPast;
            Getter = getter;
            Setter = setter;
            //   _onchanged = onchanged;
            btnCheck.Text = "\u2714";
            btnAbort.Text = "\u274C";
            // btnClear.Text = "\u2421";
            Text = text;
            tbResult.Text = Getter();
            tbResult.Select();
        }

        void create()
        {

            panelNumBlock.Controls.Clear();
            panelQuertz.Controls.Clear();


            int h = panelNumBlock.Height / 4;
            int w = panelNumBlock.Width / 3;


            Color ButtonBackColor = DarkMode ? Color.FromArgb(64, 64, 64) : SystemColors.Control;
            Color ButtonForeColor = DarkMode ? Color.White : SystemColors.ControlText;
            Color ButtonBorderColor = DarkMode ? Color.Black : Color.White;


            //Numblock

            string[] num = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "." };
            for (int i = 0; i < num.Length; i++)
            {
                KeyboardButton kb = new KeyboardButton(num[i], tbResult, w, h, DarkMode);
                kb.Size = new Size(w, h);
                if (num[i] == "0") kb.Size = new Size(w * 2, h);

                kb.Dock = DockStyle.Left;
                kb.Anchor = AnchorStyles.Left;
                panelNumBlock.Controls.Add(kb);
            }

            //Quertz
            h = panelQuertz.Height / 4;
            w = panelQuertz.Width / 10;

            for (int i = 0; i < qwertz.Length; i++)
            {
                if (qwertz[i] == "<capslock>")
                {
                    KeyboardButtonCapsLock capsLock = new KeyboardButtonCapsLock(tbResult, panelQuertz);
                    capsLock.Height = h;
                    capsLock.Width = w;
                    capsLock.BackColor = ButtonBackColor;
                    capsLock.ForeColor = ButtonForeColor;
                    capsLock.FlatAppearance.BorderColor = ButtonBorderColor;

                    capsLock.Dock = DockStyle.Left;
                    capsLock.Anchor = AnchorStyles.Left;
                    panelQuertz.Controls.Add(capsLock);
                }
                else if (qwertz[i] == "<sep>")
                {
                    KeyboardButton kb = new KeyboardButton("", tbResult, w, h, DarkMode);
                    kb.Text = "";
                    kb.Width = w / 2;
                    kb.Height = h;
                    kb.Dock = DockStyle.Left;
                    kb.Anchor = AnchorStyles.Left;
                    kb.BackColor = Color.Transparent;
                    panelQuertz.Controls.Add(kb);

                }

                else if (qwertz[i] == "<space>")
                {
                    KeyboardButton kb = new KeyboardButton(qwertz[i], tbResult, w, h, DarkMode);
                    kb.Text = " ";
                    kb.Height = h;
                    kb.Width = w * 5;
                    kb.Dock = DockStyle.Left;
                    kb.Anchor = AnchorStyles.Left;
                    panelQuertz.Controls.Add(kb);
                }
                else
                {
                    KeyboardButton kb = new KeyboardButton(qwertz[i], tbResult, w, h, DarkMode);
                    kb.Dock = DockStyle.Left;
                    kb.Height = h;
                    kb.Width = w;
                    kb.Anchor = AnchorStyles.Left;
                    panelQuertz.Controls.Add(kb);
                }
            }





            if (DarkMode)
            {
                BackColor = Color.FromArgb(30, 30, 30);

            }
            else
            {
                BackColor = Color.LightGray;
            }

            btnPaste.BackColor = ButtonBackColor;
            btnPaste.ForeColor = ButtonForeColor;

            btnPaste.FlatStyle = FlatStyle.Flat;
            btnPaste.FlatAppearance.BorderSize = 1;
            btnPaste.FlatAppearance.BorderColor = ButtonBackColor;
            btnCopy.BackColor = ButtonBackColor;
            btnCopy.ForeColor = ButtonForeColor;
            btnCopy.FlatAppearance.BorderColor = ButtonBorderColor;
            btnAbort.BackColor = ButtonBackColor;
            btnAbort.ForeColor = ButtonForeColor;
            btnAbort.FlatAppearance.BorderColor = ButtonBorderColor;
            btnCheck.BackColor = ButtonBackColor;
            btnCheck.ForeColor = ButtonForeColor;
            btnCheck.FlatAppearance.BorderColor = ButtonBorderColor;
            btnClear.BackColor = ButtonBackColor;
            btnClear.ForeColor = ButtonForeColor;
            btnClear.FlatAppearance.BorderColor = ButtonBorderColor;
            btnRemove.BackColor = ButtonBackColor;
            btnRemove.ForeColor = ButtonForeColor;
            btnRemove.FlatAppearance.BorderColor = ButtonBorderColor;

           

        

            tbResult.BackColor = DarkMode ? Color.DimGray : Color.White;
            tbResult.ForeColor = DarkMode ? Color.White : Color.Black;

            panelQuertz.BackColor = BackColor;
            panelNumBlock.BackColor = BackColor;
            tableKeypad.BackColor = BackColor;

            int bH = panelNumBlock.Height / 4;
            btnCopy.Visible = true;
            btnPaste.Visible = true;

            if (!_copyPast)
            {
                bH = panelNumBlock.Height / 2;
                btnCopy.Visible = false;
                btnPaste.Visible = false;
            }

            btnAbort.Height = bH;
            btnCheck.Height = bH;
            btnCopy.Height = bH;
            btnPaste.Height = bH;

            tbResult.Font = new System.Drawing.Font("Microsoft Sans Serif", tbResult.Height / 3, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

        }


        private void btnClear_Click(object sender, EventArgs e)
        {
            tbResult.Text = "";
            tbResult.Focus();

        }

        private void btnAbort_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            // _onchanged?.Invoke(tbResult.Text);
            Setter(tbResult.Text.ToString());
            this.Close();

        }

        private void tbResult_KeyDown(object sender, KeyEventArgs e)
        {


            if (e.KeyCode == Keys.Enter)
            {
                DialogResult = DialogResult.OK;
                Setter(tbResult.Text.ToString());
                this.Close();
            }
            if (e.KeyCode == Keys.Escape)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }

        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (tbResult.SelectionLength > 0)
            {
                tbResult.SelectedText = "";
            }

            else if (tbResult.SelectionStart > 0)
            {
                int selectionIndex = tbResult.SelectionStart;
                if (selectionIndex > 0)
                {
                    tbResult.Text = tbResult.Text.Remove(selectionIndex - 1, 1);
                    tbResult.SelectionStart = selectionIndex - 1;
                }
            }
            tbResult.Focus();
        }



        private readonly HashSet<char> allowedChars = new HashSet<char>(
         new[] {    'q','w','e','r','t','z','u','i','o','p',
                    'a','s','d','f','g','h','j','k','l',
                    'y','x','c','v','b','n','m',
                    '.', ',', '_', '+', '-', '°',
                    '0','1','2','3','4','5','6','7','8','9','/'
               }
        );


        private void tbResult_KeyPress(object sender, KeyPressEventArgs e)
        {
            {
                if (e.KeyChar == ' ')
                    return;

                if (char.IsControl(e.KeyChar))
                    return;

                if (!allowedChars.Contains(char.ToLower(e.KeyChar)))
                {
                    e.Handled = true;
                }
            }
        }

        internal partial class KeyboardButton : Button
        {
            TextBox TbResult;
            public KeyboardButton(string keyValue, TextBox tbResult, int width, int height, bool darkMode = true)
            {

                FlatStyle = FlatStyle.Flat;
                FlatAppearance.BorderSize = 1;
                FlatAppearance.BorderColor = darkMode ? Color.Black : Color.White;
                BackColor = darkMode ? Color.FromArgb(64, 64, 64) : SystemColors.Control;
                ForeColor = darkMode ? Color.White : SystemColors.ControlText;

                float fontH = (float)height * 0.4f;

                TbResult = tbResult;
                Text = keyValue;
                Dock = System.Windows.Forms.DockStyle.Fill;
                Font = new System.Drawing.Font("Microsoft Sans Serif", fontH, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                Size = new System.Drawing.Size(width, height);


                Margin = new Padding(0);
                Padding = new Padding(0);
                Click += new System.EventHandler(this.click);
            }

            private void click(object sender, EventArgs e)
            {

                if (TbResult.SelectionLength > 0)
                {
                    TbResult.SelectedText = this.Text;
                }

                if (!TbResult.Focused)
                {
                    TbResult.Focus();
                }

                int selectionIndex = TbResult.SelectionStart;
                TbResult.Text = TbResult.Text.Insert(selectionIndex, this.Text);
                TbResult.SelectionStart = selectionIndex + this.Text.Length;

            }
        }
        internal partial class KeyboardButtonCapsLock : Button
        {
            TextBox TbResult;

            string[] quertz = null;

            FlowLayoutPanel panel = null;
            public KeyboardButtonCapsLock(TextBox tbResult, FlowLayoutPanel panel, bool darkMode = true)
            {
                TbResult = tbResult;
                Text = "\u21E7";
                Dock = System.Windows.Forms.DockStyle.Fill;

                FlatStyle = FlatStyle.Flat;
                FlatAppearance.BorderSize = 1;
                FlatAppearance.BorderColor = darkMode ? Color.Black : Color.White;
                BackColor = darkMode ? Color.FromArgb(64, 64, 64) : SystemColors.Control;
                ForeColor = darkMode ? Color.White : SystemColors.ControlText;

                Size = new System.Drawing.Size(40, 40);
                Font = new System.Drawing.Font("Microsoft Sans Serif", Height, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

                Margin = new Padding(0);
                Padding = new Padding(0);
                Click += new System.EventHandler(this.click);
                this.panel = panel;
            }

            private void click(object sender, EventArgs e)
            {
                if (this.Text == "\u21E7")
                {
                    this.Text = "\u21E9";
                    capslock();
                }
                else
                {
                    this.Text = "\u21E7";
                    unCapslock();
                }

            }

            private void capslock()
            {
                for (int i = 0; i < panel.Controls.Count; i++)
                {
                    if (panel.Controls[i] is KeyboardButton)
                    {
                        KeyboardButton kb = (KeyboardButton)panel.Controls[i];
                        kb.Text = kb.Text.ToUpper();
                    }
                }
            }

            private void unCapslock()
            {
                for (int i = 0; i < panel.Controls.Count; i++)
                {
                    if (panel.Controls[i] is KeyboardButton)
                    {
                        KeyboardButton kb = (KeyboardButton)panel.Controls[i];
                        kb.Text = kb.Text.ToLower();
                    }
                }
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbResult.Text)) return;

            if (tbResult.SelectionLength > 0)
            {
                Clipboard.SetText(tbResult.SelectedText);
            }
            else
            {
                Clipboard.SetText(tbResult.Text);
            }
        }

        private void btnPaste_Click(object sender, EventArgs e)
        {
            if (!Clipboard.ContainsText()) return;

            if (tbResult.SelectionLength > 0)
            {
                int selectionIndex = tbResult.SelectionStart;
                tbResult.Text = tbResult.Text.Insert(selectionIndex, Clipboard.GetText());
                tbResult.SelectionStart = selectionIndex + Clipboard.GetText().Length;
            }
            else
            {
                tbResult.SelectedText = Clipboard.GetText();
            }

        }

        private void Keyboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (DialogResult == DialogResult.OK)
            //DialogResult = DialogResult.Cancel;

        }



        private void Keyboard_Shown(object sender, EventArgs e)
        {

            create();

        }

        private void FormKeyboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            
        }
    }
}
