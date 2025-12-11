
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace YourNamespace
{
    internal class DotButton : FontAwesome.Sharp.IconButton
    {
        // --- öffentliche Eigenschaften (Designer-serialisierbar) ---
        [Category("Square"), Description("Füllfarbe des Quadrats oben rechts")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color SquareFillColor { get; set; } = Color.Orange;

        [Category("Square"), Description("Randfarbe des Quadrats oben rechts")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color SquareBorderColor { get; set; } = Color.Black;

        [Category("Square"), Description("Größe des Quadrats (Kantenlänge in Pixeln)")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int SquareSize { get; set; } = 10;

        [Category("Square"), Description("Abstand des Quadrats von den Außenrändern")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int SquareMargin { get; set; } = 2;

        [Category("Square"), Description("Ob das Quadrat angezeigt wird")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool SquareVisible { get; set; } = true;

        [Category("Square"), Description("Ob ein ColorDialog beim Klick geöffnet wird")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowColorDialogOnClick { get; set; } = true;

        // --- Ereignis, falls du die Auswahl selber steuern willst ---
        public event EventHandler? SquareClicked;

        public DotButton()
        {
            // Basis-Konfig aus deinem Beispiel
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            BackColor = Color.Transparent;

            IconChar = FontAwesome.Sharp.IconChar.Circle;
            IconColor = Color.Gray;
            IconSize = 16;

            Size = new Size(16, 16);
            Margin = Padding = new Padding(0);

            // optional: TabStop aktivieren => Tastaturfokus möglich
            TabStop = true;

            SquareFillColor = Color.Transparent;
        }

      
        private Rectangle GetSquareRect()
        {
            if (!SquareVisible) return Rectangle.Empty;

            // DPI-Skalierung: falls gewünscht, skaliere SquareSize/Margin mit DeviceDpi
            float dpiScale = DeviceDpi / 96f;
            int s = (int)Math.Max(4, Math.Round(SquareSize * dpiScale));
            int m = (int)Math.Round(SquareMargin * dpiScale);

            return new Rectangle(m, m, s, s);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            if (!SquareVisible) return;

            var g = pevent.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

            Rectangle r = GetSquareRect();
            using var fill = new SolidBrush(SquareFillColor);
            using var pen = new Pen(SquareBorderColor);

            g.FillRectangle(fill, r);
            g.DrawRectangle(pen, r);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (!SquareVisible) return;

            Rectangle r = GetSquareRect();
            if (r.Contains(e.Location))
            {
                // eigenes Ereignis auslösen
                SquareClicked?.Invoke(this, EventArgs.Empty);

                //// optional: ColorDialog öffnen und setzen
                //if (ShowColorDialogOnClick && e.Button == MouseButtons.Left)
                //{
                //    using var cd = new ColorDialog
                //    {
                //        FullOpen = true,
                //        Color = SquareFillColor
                //    };
                //    if (cd.ShowDialog(this.FindForm()) == DialogResult.OK)
                //    {
                //        SquareFillColor = cd.Color;
                //        Invalidate(); // neu zeichnen
                //    }
                //}
            }
        }

        // Tastaturbedienung (Optional): SPACE/ENTER auf das Quadrat wirken lassen
        protected override bool IsInputKey(Keys keyData)
        {
            return keyData is Keys.Space or Keys.Enter || base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (!SquareVisible) return;

            if (e.KeyCode is Keys.Space or Keys.Enter)
            {
                SquareClicked?.Invoke(this, EventArgs.Empty);
                if (ShowColorDialogOnClick)
                {
                    using var cd = new ColorDialog { FullOpen = true, Color = SquareFillColor };
                    if (cd.ShowDialog(this.FindForm()) == DialogResult.OK)
                    {
                        SquareFillColor = cd.Color;
                        Invalidate();
                    }
                }
                e.Handled = true;
            }
        }

        // Fokus-Rechteck (nur optisch, falls du möchtest)
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            base.OnPaintBackground(pevent);
            if (Focused && SquareVisible)
            {
                using var pen = new Pen(Color.FromArgb(128, SquareBorderColor)) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dot };
                var r = GetSquareRect();
                r.Inflate(1, 1);
                pevent.Graphics.DrawRectangle(pen, r);
            }
        }
    }
}
