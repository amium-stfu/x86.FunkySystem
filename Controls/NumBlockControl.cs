using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Markup;
using FunkySystem.Helpers;

namespace FunkySystem.Controls
{
    internal class NumBlockControl : UserControl
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
        public float QwertzAreaRatio = 0.9f;   // ~74% links QWERTZ, ~26% rechts Numblock
        public float SepWeight = 0.5f;          // Breitenfaktor für <sep>
        public float SpaceWeight = 5f;          // Breitenfaktor für <space>
        public float OkCancelWidthRatio = 1.1f;// Anteil der rechten Fläche für OK/Cancel-Spalte

        private bool _capsLock = false;

        private readonly List<KeyItem> _keys = new();
        private RectangleF _okRect = RectangleF.Empty;
        private RectangleF _cancelRect = RectangleF.Empty;

        string[]? units;

        List<string[]> itmes;

     
        public NumBlockControl(string[] units = null)
        {
            itmes = new List<string[]>();
            DoubleBuffered = true;
            ResizeRedraw = true;
           
            this.units = units;

            if(units != null) 
            {
                itmes.Add(units);
                itmes.Add(new[] { "7", "8", "9" });
                itmes.Add(new[] { "3", "4", "5" });
                itmes.Add(new[] { "1", "2", "3" });
                itmes.Add(new[] { "0", ".", "+-" });

            }
            else
            {
                itmes.Add(new[] { "7", "8", "9" });
                itmes.Add(new[] { "3", "4", "5" });
                itmes.Add(new[] { "1", "2", "3" });
                itmes.Add(new[] { "0", ".", "+-" });
            }

       
            BuildLayout();

        }

        #region Layout-Daten (QWERTZ & Numblock)
       // private readonly string[][] _qwertzRows = values.ToArray();

   
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
   
            int qRows = itmes.Count;
            float keyH = (h - (qRows + 1) * margin) / qRows;

            // ---------- QWERTZ ----------
            float y = margin;
            foreach (var row in itmes)
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
            float okCancelW = w - qAreaW;
            float numsW = rightTotalW - okCancelW - margin; // Rest für 3 Spalten

            int numCols = 3;
            float colW = (numsW - (numCols + 1) * margin) / numCols;
            float numXStart = qAreaW ; // rechts vom QWERTZ-Bereich

            // Rechte OK/Cancel-Säule (vertikal gestapelt, jeweils halbe Höhe)
            float okCancelX = numXStart + numsW + margin;
            float okCancelUsableH = (qRows * keyH) + ((qRows - 1) * margin); // gleiche Höhe wie QWERTZ-Block
            float halfH = (h - (4 * margin)) / 2f - margin + 1;             // oben + Mitte + unten Abstände

            var okRect = new RectangleF(numXStart, margin, okCancelW - margin, halfH + margin);
            var cancelRect = new RectangleF(numXStart, 2 * margin + halfH + margin, okCancelW - margin, halfH + margin);

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

