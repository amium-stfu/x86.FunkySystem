
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using static SkiaSharp.HarfBuzz.SKShaper;

namespace FunkySystem.Controls
{
    public class KeyboardDialog : Form
    {
        private readonly TextBox _tbResult = new TextBox();
        private readonly KeyboardControl _keyboard = new KeyboardControl();
        private readonly Button _btnBackspace = new Button();
        private readonly Button _btnDel = new Button();
        private readonly Panel _topPanel = new Panel();

        Label space = new Label();
        Label space2 = new Label();


        public string ResultText => _tbResult.Text;
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool DarkMode
        {
            get => _keyboard.DarkMode;
            set
            {
                _keyboard.DarkMode = value;
                ApplyTheme();
            }
        }

        public Func<string> Getter = null;
        public Action<string> Setter = null;

        public KeyboardDialog(Func<string> getter, Action<string> setter, string initialText = "", string title = "Keyboard")
        {
            Text = title;
            Getter = getter;
            Setter = setter;

            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            Width = 1100;
            Height = 420;

            // Top-Panel (TextBox + Buttons)
            _topPanel.Dock = DockStyle.Top;
            _topPanel.Height = 70;
            _topPanel.Padding = new Padding(8, 2, 11, 2);

            _tbResult.Dock = DockStyle.Fill;
            _tbResult.Font = new Font("Segoe UI", 24,FontStyle.Bold);
            _tbResult.Text = Getter();
            _tbResult.Multiline = true;
            

            _btnBackspace.Text = "<";
            _btnBackspace.Width = 60;
            _btnBackspace.Dock = DockStyle.Right;
            _btnBackspace.FlatStyle = FlatStyle.Flat;
            _btnBackspace.FlatAppearance.BorderColor = Color.LightGray;
            _btnBackspace.Click += (s, e) =>
            {
                if (_tbResult.SelectionLength > 0)
                {
                    int start = _tbResult.SelectionStart;
                    _tbResult.Text = _tbResult.Text.Remove(start, _tbResult.SelectionLength);
                    _tbResult.SelectionStart = start;
                }
                else if (_tbResult.TextLength > 0 && _tbResult.SelectionStart > 0)
                {
                    int pos = _tbResult.SelectionStart;
                    _tbResult.Text = _tbResult.Text.Remove(pos - 1, 1);
                    _tbResult.SelectionStart = Math.Max(pos - 1, 0);
                }
            };

            _btnDel.Text = "DEL";
            _btnDel.Width = 70;
            _btnDel.Dock = DockStyle.Right;
            _btnDel.FlatStyle = FlatStyle.Flat;
            _btnDel.Click += (s, e) => _tbResult.Clear();
            _btnDel.FlatAppearance.BorderColor = Color.LightGray;

            space.Width = 12;
            space.BackColor = Color.LightSlateGray;
            space.Dock = DockStyle.Right;

            space2.Width = 12;
            space2.BackColor = Color.LightSlateGray;
            space2.Dock = DockStyle.Right;

            _topPanel.Controls.Add(space2);
            _topPanel.Controls.Add(_btnDel);
            _topPanel.Controls.Add(space);
            _topPanel.Controls.Add(_btnBackspace);
           
            _topPanel.Controls.Add(_tbResult);

            // Keyboard darunter
            _keyboard.Dock = DockStyle.Fill;
            
            _keyboard.KeyPressed += key =>
            {
                int pos = _tbResult.SelectionStart;
                if (_tbResult.SelectionLength > 0)
                {
                    _tbResult.Text = _tbResult.Text.Remove(pos, _tbResult.SelectionLength)
                                                   .Insert(pos, key);
                }
                else
                {
                    _tbResult.Text = _tbResult.Text.Insert(pos, key);
                }
                _tbResult.SelectionStart = pos + key.Length;
            };
            _keyboard.OkPressed += () => { DialogResult = DialogResult.OK; Close(); };
            _keyboard.CancelPressed += () => { DialogResult = DialogResult.Cancel; Close(); };

            Controls.Add(_keyboard);
            Controls.Add(_topPanel);

            // --- Breite von <ok> auf DEL/Backspace spiegeln ---
            ApplyOkWidthToTopButtons(_keyboard.OkButtonWidthPx);
            _keyboard.LayoutUpdated += okWidth =>
            {
                ApplyOkWidthToTopButtons(okWidth);
            };
            this.Resize += (s, e) =>
            {
                ApplyOkWidthToTopButtons(_keyboard.OkButtonWidthPx);
            };

            ApplyTheme();

            _tbResult.Select();
            _tbResult.KeyDown += _tbResult_KeyDown;
            _tbResult.KeyPress += _tbResult_KeyPress;
        }

        private void _keyboard_KeyPressed(string obj)
        {
            throw new NotImplementedException();
        }

        public void SelectAll()
        {

            _tbResult.SelectAll();
            _tbResult.Focus();

        }

        private void ApplyOkWidthToTopButtons(float okWidthPx)
        {
            int width = Math.Max(40, (int)Math.Round(okWidthPx));
            _btnBackspace.Width = width;
            _btnDel.Width = width * 2;

            // Optional: dynamische Schrift für die Top-Buttons (40% der Panel-Höhe), fett
            float fontSize = Math.Max(12f, _topPanel.Height * 0.30f);
            var f = new Font("Segoe UI", fontSize, FontStyle.Bold);
            _btnBackspace.Font = f;
            _btnDel.Font = f;
        }

        private void ApplyTheme()
        {
            var back = DarkMode ? Color.FromArgb(30, 30, 30) : Color.LightSlateGray;
            var fore = DarkMode ? Color.White : Color.Black;
            var btnBack = DarkMode ? Color.FromArgb(64, 64, 64) : Color.Gainsboro;

            BackColor = back;

            _tbResult.BackColor = DarkMode ? Color.DimGray : Color.White;
            _tbResult.ForeColor = fore;

            _topPanel.BackColor = back;

            _btnBackspace.BackColor = btnBack;
            _btnBackspace.ForeColor = fore;
            _btnDel.BackColor = btnBack;
            _btnDel.ForeColor = fore;
        }


        private readonly HashSet<char> allowedChars = new HashSet<char>(new[]
        {
            '!','\'','§','$','%','&','/','(',')','=','?',
            'q','w','e','r','t','z','u','i','o','p',
            'a','s','d','f','g','h','j','k','l',
            'y','x','c','v','b','n','m',
            '.',',','_','/','+','-','°',
            '0','1','2','3','4','5','6','7','8','9'
        });

        // KeyDown: nur für Enter/Escape und ggf. Navigations-/Lösch-Tasten
        private void _tbResult_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DialogResult = DialogResult.OK;
                Setter(_tbResult.Text);
                Close();
                return;
            }

            if (e.KeyCode == Keys.Escape)
            {
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }

            // Navigations- und Editier-Tasten zulassen (nicht blockieren)
            if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete ||
                e.KeyCode == Keys.Left || e.KeyCode == Keys.Right ||
                e.KeyCode == Keys.Home || e.KeyCode == Keys.End)
            {
                // nichts tun -> durchlassen
                return;
            }

            // Paste per Strg+V: NICHT hier blocken, sondern in KeyPress/TextChanged bereinigen
            // e.Control && e.KeyCode == Keys.V -> später prüfen
        }

        // KeyPress: hier entscheidest du anhand des Zeichens
        private void _tbResult_KeyPress(object sender, KeyPressEventArgs e)
        {
            char ch = e.KeyChar;

            // Steuerzeichen (Backspace, Enter etc.) immer durchlassen
            if (char.IsControl(ch))
                return;

            // Leerzeichen erlauben? -> hier entscheiden
            if (ch == ' ')
                return;

            // Klein-/Großschreibung angleichen (deine Liste ist klein)
            char normalized = char.ToLower(ch);

            if (!allowedChars.Contains(normalized))
            {
                // Zeicheneingabe unterdrücken
                e.Handled = true;
            }
        }

    }
}
