
using FunkySystem.Helpers;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using static SkiaSharp.HarfBuzz.SKShaper;

namespace FunkySystem.Controls
{
    public class NumBlockDialog<T> : Form
    {
        private readonly TextBox _tbResult = new TextBox();
        private readonly NumBlockControl _numBlock;
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
            get => _numBlock.DarkMode;
            set
            {
                _numBlock.DarkMode = value;
                ApplyTheme();
            }
        }

        private readonly Func<T> _getter;
        private readonly Action<T> _setter;

        public NumBlockDialog(Func<T> getter, Action<T> setter,
                                 string title = "Keyboard",
                                 string[]? units = null)

        {
            Text = title;
            _getter = getter;
            _setter = setter;

            _numBlock = new NumBlockControl(units!);

            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            Width = 1100;
            Height = 420;

            // Top-Panel (TextBox + Buttons)
            _topPanel.Dock = DockStyle.Top;
            _topPanel.Height = 70;
            _topPanel.Padding = new Padding(8, 2, 6, 2);

            _tbResult.Dock = DockStyle.Fill;
            _tbResult.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            _tbResult.Text = _getter().ToString().Replace(",",".");
            _tbResult.Multiline = true;
            _tbResult.BorderStyle = BorderStyle.None;


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

            space.Width = 6;
            space.BackColor = Color.LightSlateGray;
            space.Dock = DockStyle.Right;

            space2.Width = 6;
            space2.BackColor = Color.LightSlateGray;
            space2.Dock = DockStyle.Right;

            _topPanel.Controls.Add(space2);
            _topPanel.Controls.Add(_btnDel);
            _topPanel.Controls.Add(space);
            _topPanel.Controls.Add(_btnBackspace);

            _topPanel.Controls.Add(_tbResult);

            // Keyboard darunter
            _numBlock.Dock = DockStyle.Fill;

            _numBlock.KeyPressed += key =>
            {
                if (key == "." && _tbResult.Text.Contains(".")) return;

                if(key == "+-" && _tbResult.Text.Contains("-"))
                {
                    _tbResult.Text = _tbResult.Text.Replace("-", "");
                    return;
                }
                if (key == "+-" && !_tbResult.Text.Contains("-"))
                {
                    _tbResult.Text = "-" + _tbResult.Text;
                    return;
                }


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
            _numBlock.OkPressed += () => { Commit(); };
            _numBlock.CancelPressed += () => { DialogResult = DialogResult.Cancel; Close(); };

            Controls.Add(_numBlock);
            Controls.Add(_topPanel);

            // --- Breite von <ok> auf DEL/Backspace spiegeln ---
            ApplyOkWidthToTopButtons(_numBlock.OkButtonWidthPx);
            _numBlock.LayoutUpdated += okWidth =>
            {
                ApplyOkWidthToTopButtons(okWidth);
            };
            this.Resize += (s, e) =>
            {
                ApplyOkWidthToTopButtons(_numBlock.OkButtonWidthPx);
            };

            ApplyTheme();

            _tbResult.Select();
            _tbResult.KeyDown += _tbResult_KeyDown;
            _tbResult.KeyPress += _tbResult_KeyPress;
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
            '.',',','+',
            '0','1','2','3','4','5','6','7','8','9'
        });

        // KeyDown: nur für Enter/Escape und ggf. Navigations-/Lösch-Tasten



        private static Type GetNonNullableType(Type t)
        {
            return Nullable.GetUnderlyingType(t) ?? t;
        }

        private static bool IsNullable(Type t)
        {
            return Nullable.GetUnderlyingType(t) != null || !t.IsValueType;
        }



        private void Commit()
        {
            string s = _tbResult.Text;
            var culture = System.Globalization.CultureInfo.CurrentCulture;

            try
            {
                // Leere Eingabe -> für Nullable-Typen erlaubt (=> default/null), für non-nullable nicht.
                Type target = GetNonNullableType(typeof(T));
                bool nullable = IsNullable(typeof(T));

                if (string.IsNullOrWhiteSpace(s))
                {
                    if (nullable)
                    {
                        // Für Referenztypen und Nullable<T> ist null okay
                        _setter(default!);
                        DialogResult = DialogResult.OK;
                        Close();
                        return;
                    }
                    else
                    {
                        throw new FormatException("Eingabe darf nicht leer sein.");
                    }
                }

                object boxed;

                if (target == typeof(string))
                {
                    boxed = s;
                }
                else if (target == typeof(int))
                {
                    // Integer: nur ganze Zahlen (inkl. Vorzeichen)
                    if (!int.TryParse(s, System.Globalization.NumberStyles.Integer, culture, out var i))
                        throw new FormatException("Bitte eine gültige ganze Zahl eingeben.");
                    boxed = i;
                }
                else if (target == typeof(double))
                {
                    // Double: erlaubt Dezimaltrennzeichen gemäß Kultur, sowie Tausender
                    if (!double.TryParse(s, System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, culture, out var d))
                        throw new FormatException("Bitte eine gültige Zahl eingeben (z. B. 123,45).");
                    boxed = s.ToDouble();
                }
                else if (target == typeof(decimal))
                {
                    if (!decimal.TryParse(s.Replace(".",","), System.Globalization.NumberStyles.Number, culture, out var m))
                        throw new FormatException("Bitte eine gültige Dezimalzahl eingeben.");
                    boxed = m;
                }
                else if (target == typeof(long))
                {
                    if (!long.TryParse(s, System.Globalization.NumberStyles.Integer, culture, out var l))
                        throw new FormatException("Bitte eine gültige ganze Zahl eingeben.");
                    boxed = l;
                }
                else if (target == typeof(float))
                {
                    if (!float.TryParse(s, System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, culture, out var f))
                        throw new FormatException("Bitte eine gültige Zahl eingeben.");
                    boxed = f;
                }
                else
                {
                    // Fallback: versuche generische Konvertierung
                    boxed = Convert.ChangeType(s, target, culture);
                }

                // Wenn T nullable ist (z. B. int?), in Nullable<T> casten
                T val;
                if (nullable && target.IsValueType)
                {
                    // boxed enthält valueType -> in Nullable<T> verpacken
                    val = (T)boxed;
                }
                else
                {
                    val = (T)boxed;
                }

                _setter(val);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                // Nutzerfreundliches Feedback
                MessageBox.Show(this, ex.Message, "Ungültige Eingabe", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _tbResult.SelectAll();
                _tbResult.Focus();
            }
        }


        private void _tbResult_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Commit();
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

            if(ch == ',') e.KeyChar = '.';

            if (_tbResult.Text.Contains(".") && e.KeyChar == '.') e.Handled = true;

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
