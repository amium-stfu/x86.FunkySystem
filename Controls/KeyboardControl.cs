
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace FunkySystem.Controls
{
    public class KeyboardControl : UserControl
    {
        // Eingabe-Events
        public event Action<string>? KeyPressed;
        public event Action? OkPressed;
        public event Action? CancelPressed;

        /// <summary>
        /// Wird ausgelöst, nachdem das Layout neu berechnet wurde (z. B. nach Resize).
        /// Übergibt die aktuelle Breite des OK-Buttons (in Pixeln) an Abonnenten.
        /// </summary>
        public event Action<float>? LayoutUpdated;

        /// <summary>
        /// Aktuelle Breite des OK-Buttons (in Pixeln).
        /// </summary>
        public float OkButtonWidthPx { get; private set; } = 0f;

        // Darstellung / Layout-Parameter
        public bool DarkMode = false;
        public int Margin = 6;
        public float QwertzAreaRatio = 0.74f;   // ~74% links QWERTZ, ~26% rechts Numblock
        public float SepWeight = 0.5f;          // Breitenfaktor für <sep>
        public float SpaceWeight = 5f;          // Breitenfaktor für <space>
        public float OkCancelWidthRatio = 0.18f;// Anteil der rechten Fläche für OK/Cancel-Spalte

        private bool _capsLock = false;

        private readonly List<KeyItem> _keys = new();
        private RectangleF _okRect = RectangleF.Empty;
        private RectangleF _cancelRect = RectangleF.Empty;

        public KeyboardControl()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            BuildLayout();
        }

        #region Layout-Daten (QWERTZ & Numblock)
        private readonly string[][] _qwertzRows = new[]
        {
            new[] { "!","\"","§","$","%","&","/","(",")","=","?" },
            new[] { "q","w","e","r","t","z","u","i","o","p" },
            new[] { "<sep>","a","s","d","f","g","h","j","k","l","<sep>" },
            new[] { "<capslock>","y","x","c","v","b","n","m",".","," },
            new[] { "_","/","<space>","+","-","°" }
        };

        private readonly string[][] _numRows = new[]
        {
            new[] { "1","2","3" },
            new[] { "4","5","6" },
            new[] { "7","8","9" },
            // Unterste Reihe: 0 (doppelte Breite) + .
            new[] { "0",".","<gap>" } // <gap> nur Platzhalter, wird nicht gezeichnet
        };
        #endregion

        private sealed class KeyItem
        {
            public string Text = "";
            public RectangleF Rect;
            public bool IsSpecial;
            public bool IsSpacer; // <sep> oder <gap>
            public bool IsLetter;
        }

        private void BuildLayout()
        {
            _keys.Clear();

            var w = Width;
            var h = Height;
            if (w <= 0 || h <= 0) return;

            int margin = Margin;
            float qAreaW = (float)Math.Floor(w * QwertzAreaRatio) - margin; // linke Fläche
            float rightTotalW = w - qAreaW - 3 * margin;                    // gesamte rechte Fläche inkl. OK/Cancel-Spalte

            // Höhe pro QWERTZ-Zeile
            int qRows = _qwertzRows.Length;
            float keyH = (h - (qRows + 1) * margin) / qRows;

            // ---------- QWERTZ ----------
            float y = margin;
            foreach (var row in _qwertzRows)
            {
                float totalWeight = 0f;
                int visibleItems = 0;
                foreach (var t in row)
                {
                    totalWeight += t switch
                    {
                        "<sep>" => SepWeight,
                        "<space>" => SpaceWeight,
                        _ => 1f
                    };
                    visibleItems++;
                }

                float rowUsableW = qAreaW - (visibleItems + 1) * margin;
                float unitW = rowUsableW / Math.Max(totalWeight, 1f);

                float x = margin;
                foreach (var t in row)
                {
                    float weight = t switch
                    {
                        "<sep>" => SepWeight,
                        "<space>" => SpaceWeight,
                        _ => 1f
                    };
                    float wKey = unitW * weight;
                    var rect = new RectangleF(x, y, wKey, keyH);

                    bool isSpacer = (t == "<sep>");
                    bool isSpecial = t.StartsWith("<") && t != "<sep>";
                    bool isLetter = !isSpecial && t.Length == 1 && char.IsLetter(t[0]);

                    _keys.Add(new KeyItem
                    {
                        Text = t,
                        Rect = rect,
                        IsSpecial = isSpecial,
                        IsSpacer = isSpacer,
                        IsLetter = isLetter
                    });

                    x += wKey + margin;
                }

                y += keyH + margin;
            }

            // ---------- NUMBLOCK ----------
            // Aufteilung rechts: [Ziffern-Bereich mit 3 Spalten] + [OK/Cancel-Säule]
            float okCancelW = rightTotalW * OkCancelWidthRatio;
            float numsW = rightTotalW - okCancelW - margin; // Rest für 3 Spalten

            int numCols = 3;
            float colW = (numsW - (numCols + 1) * margin) / numCols;
            float numXStart = qAreaW + 2 * margin; // rechts vom QWERTZ-Bereich

            // Ziffern/.-Tasten (3 Spalten × 4 Zeilen)

        
          
            keyH = (h - 6 * margin) / 4 + 1 ;

            float ny = margin;
            for (int r = 0; r < _numRows.Length; r++)
            {
                float nx = numXStart + margin;

                if (r == _numRows.Length - 1)
                {
                    // Unterste Zeile: 0 (zweifache Breite) + .
                    var zeroRect = new RectangleF(nx, ny, 2 * colW + margin, keyH); // +margin absorbiert mittleren Zwischenraum
                    _keys.Add(new KeyItem
                    {
                        Text = "0",
                        Rect = zeroRect,
                        IsSpecial = false,
                        IsSpacer = false,
                        IsLetter = false
                    });

                    float dotX = zeroRect.Right + margin;
                    var dotRect = new RectangleF(dotX, ny, colW, keyH);
                    _keys.Add(new KeyItem
                    {
                        Text = ".",
                        Rect = dotRect,
                        IsSpecial = false,
                        IsSpacer = false,
                        IsLetter = false
                    });
                }
                else
                {
                    foreach (var t in _numRows[r])
                    {
                        bool isSpacer = t == "<gap>";
                        var rect = new RectangleF(nx, ny, colW, keyH );

                        if (!isSpacer)
                        {
                            _keys.Add(new KeyItem
                            {
                                Text = t,
                                Rect = rect,
                                IsSpecial = t.StartsWith("<"),
                                IsSpacer = false,
                                IsLetter = (!t.StartsWith("<") && t.Length == 1 && char.IsLetter(t[0]))
                            });
                        }

                        nx += colW + margin;
                    }
                }

                ny += keyH + margin;
            }

            // Rechte OK/Cancel-Säule (vertikal gestapelt, jeweils halbe Höhe)
            float okCancelX = numXStart + numsW + margin;
            float okCancelUsableH = (qRows * keyH) + ((qRows - 1) * margin); // gleiche Höhe wie QWERTZ-Block
            float halfH = (h - (4 * margin)) / 2f - margin + 1;             // oben + Mitte + unten Abstände

            var okRect = new RectangleF(okCancelX, margin, okCancelW - margin, halfH + margin);
            var cancelRect = new RectangleF(okCancelX, 2 * margin + halfH + margin, okCancelW - margin, halfH + margin);

            // Rechtecke merken & Breite publishen
            _okRect = okRect;
            _cancelRect = cancelRect;
            OkButtonWidthPx = _okRect.Width;

            // Event feuern (z. B. für Dialog zum Spiegeln der Breite)
            LayoutUpdated?.Invoke(OkButtonWidthPx);

            _keys.Add(new KeyItem
            {
                Text = "<ok>",
                Rect = okRect,
                IsSpecial = true,
                IsSpacer = false,
                IsLetter = false
            });
            _keys.Add(new KeyItem
            {
                Text = "<cancel>",
                Rect = cancelRect,
                IsSpecial = true,
                IsSpacer = false,
                IsLetter = false
            });

            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            BuildLayout();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.Clear(DarkMode ? Color.FromArgb(30, 30, 30) : Color.LightSlateGray);

            Color back = DarkMode ? Color.FromArgb(64, 64, 64) : Color.LightGray;
            Color fore = DarkMode ? Color.White : Color.Black;
            using var fill = new SolidBrush(back);
            using var spacerFill = new SolidBrush(DarkMode ? Color.FromArgb(30, 30, 30) : Color.LightSlateGray);
            using var textBrush = new SolidBrush(fore);
            using var borderPen = new Pen(DarkMode ? Color.Black : Color.DarkGray);

            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

            foreach (var key in _keys)
            {
                if (key.IsSpacer)
                {
                    // Spacer (sep/gap) nur Hintergrund – keine Taste
                    g.FillRectangle(spacerFill, key.Rect);
                    continue;
                }

                g.FillRectangle(fill, key.Rect);
                g.DrawRectangle(borderPen, Rectangle.Round(key.Rect));

                string display = key.Text switch
                {
                    "<capslock>" => "⇧",
                    "<space>" => "Space",
                    "<ok>" => "✔",
                    "<cancel>" => "❌",
                    _ => (_capsLock && key.IsLetter)
                                    ? key.Text.ToUpperInvariant()
                                    : key.Text
                };

                // Dynamische Schriftgröße (40% der Button-Höhe), fett
                float fontSize = Height / 11;
                using var dynamicFont = new Font("Segoe UI", fontSize, FontStyle.Bold);
                g.DrawString(display, dynamicFont, textBrush, key.Rect, sf);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            var hit = _keys.FirstOrDefault(k => k.Rect.Contains(e.Location));
            if (hit == null || hit.IsSpacer) return;

            switch (hit.Text)
            {
                case "<capslock>":
                    _capsLock = !_capsLock;
                    Invalidate();
                    break;
                case "<space>":
                    KeyPressed?.Invoke(" ");
                    break;
                case "<ok>":
                    OkPressed?.Invoke();
                    break;
                case "<cancel>":
                    CancelPressed?.Invoke();
                    break;
                default:
                    string s = (_capsLock && hit.IsLetter) ? hit.Text.ToUpperInvariant() : hit.Text;
                    KeyPressed?.Invoke(s);
                    break;
            }
        }

        /// <summary>
        /// Optional: Key-Rechteck abfragen (z. B. "<ok>", "<cancel>", "<space>").
        /// </summary>
        public bool TryGetKeyRect(string keyText, out RectangleF rect)
        {
            var hit = _keys.FirstOrDefault(k =>
                string.Equals(k.Text, keyText, StringComparison.OrdinalIgnoreCase));

            if (hit != null) { rect = hit.Rect; return true; }
            rect = RectangleF.Empty; return false;
        }

        public RectangleF OkRect => _okRect;
        public RectangleF CancelRect => _cancelRect;
    }
}
