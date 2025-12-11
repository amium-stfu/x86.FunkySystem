using FontAwesome.Sharp;
using FunkySystem.Core;
using FunkySystem.Helpers;
using FunkySystem.Signals;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms.Design;

namespace FunkySystem.Controls
{
    public class LimitedSizeDictionary<TKey, TValue>
    {
        private readonly int maxSize;
        private readonly Queue<TKey> keyQueue;
        private readonly Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

        public LimitedSizeDictionary(int maxSize)
        {
            if (maxSize <= 0)
            {
                throw new ArgumentException("maxSize must be greater than zero.");
            }
            this.maxSize = maxSize;
            keyQueue = new Queue<TKey>(maxSize);
        }

        public void Add(TKey key, TValue value)
        {
            lock (dictionary)
            {
                if (dictionary.Count >= maxSize)
                {
                    TKey oldestKey = keyQueue.Dequeue();
                    dictionary.Remove(oldestKey);
                }

                keyQueue.Enqueue(key);
                dictionary[key] = value;
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (dictionary)
            {
                return dictionary.TryGetValue(key, out value);
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                if (dictionary.TryGetValue(key, out TValue value))
                {
                    return value;
                }
                else
                    return default(TValue);
            }
        }

        public bool ContainsKey(TKey key)
        {
            lock (dictionary)
            {
                return dictionary.ContainsKey(key);
            }
        }

        public int GridScale = 5;

    }

    public static class Icons
    {
        static LimitedSizeDictionary<string, Image> _faIconDict = new LimitedSizeDictionary<string, Image>(100);
        public static Image GetFaIcon(string name, int size = 256, object colorName = null)
        {

            if (colorName == null)
                colorName = "black";

            try
            {
                string key = name + ":" + size + ":" + colorName;
                if (_faIconDict.ContainsKey(key))
                    return _faIconDict[key];
                else
                {
                    try
                    {
                        if (size == 0) size = 20;
                        FontAwesome.Sharp.IconChar fa_icon = FontAwesome.Sharp.IconChar.None;
                        bool ok = Enum.TryParse<FontAwesome.Sharp.IconChar>(name.Replace("-", ""), true, out fa_icon);
                        Bitmap bmp = fa_icon.ToBitmap(size, size, colorName.ToColor()); // System.Drawing.Color.Black);
                        //HACK: the returned bmp is not centered. so move it slightly:
                        Bitmap bmp2 = new Bitmap(bmp.Width, bmp.Height, bmp.PixelFormat);
                        using (Graphics graph = Graphics.FromImage(bmp2))
                        {
                            graph.DrawImageUnscaled(bmp, (int)(bmp.Width * 0.01), (int)(bmp.Height * 0.08));
                        }
                        _faIconDict.Add(key, bmp2);
                        return bmp2;
                    }
                    catch
                    {
                        _faIconDict.Add(key, null);
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

    }

    public abstract class BaseControl : Control
    {
        Color borderColor = Color.Black;
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BorderColor
        { get => borderColor;
            set { borderColor = value; Invalidate(); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color HoverColor { get; set; } = Color.Transparent;

        bool selected = false;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool Selected
        {
            get => selected;
            set
            {
                selected = value;
                if (selected)
                {
                    BackColor = SelectColor;
                }
                else
                {
                    BackColor = DefaultBackColor;
                }
                Invalidate();

            }
        }


        Color selectColor = Color.Orange;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color SelectColor
        {
            get => selectColor;
            set
            {
                selectColor = value;
                Invalidate();
            }
        }


        Color defaultBackColor;
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color DefaultBackColor
        {
            get => defaultBackColor;
            set
            {
                defaultBackColor = value;
                BackColor = defaultBackColor;
                Invalidate();
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int GridScale { get; set; } = 5;

        public event EventHandler? LeftClicked;
        public event EventHandler? RightClicked;

        internal object? _source;

        private bool _isDragging = false;
        private Point _dragOffset;

        // Resize support
        private bool _isResizing = false;
        private bool _hoverResizeGrip = false;
        private Point _resizeStart;
        private Size _initialSize;
        protected const int ResizeGripSize = 15;

        // Move support
        private bool _isMoving = false;
        private bool _hoverMoveGrip = false;
        private Point _moveStart;
        private const int MoveGripSize = 15;

        // Neu: ursprüngliche Hintergrundfarbe merken (für Restore)
        private Color _backColorBeforeHover = Color.Empty;

        [DllImport("user32.dll")]
        private static extern short GetKeyState(Keys key);

        protected BaseControl()
        {
            // Make sure the control can receive focus and arrow keys
            this.SetStyle(ControlStyles.Selectable, true);
            this.TabStop = true;

            Margin = new Padding(0);
            Padding = new Padding(0);
        }

        public void SetSource(object source)
        {
            ControlManager.Register(Name, ((BaseSignalCommon)source).Name, this);
            _source = source ?? throw new ArgumentNullException(nameof(source));
            Update();
        }

        protected override bool IsInputKey(Keys keyData)
        {
            var keyCode = keyData & Keys.KeyCode;
            if (keyCode == Keys.Left || keyCode == Keys.Right || keyCode == Keys.Up || keyCode == Keys.Down)
                return true;
            return base.IsInputKey(keyData);
        }

        // helper: snap to grid
        private int Snap(int value)
        {
            int s = Math.Max(1, GridScale);
            return (int)Math.Round(value / (double)s) * s;
        }
        private Point Snap(Point p) => new Point(Snap(p.X), Snap(p.Y));

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            if (HoverColor != Color.Transparent)
            {
                if (_backColorBeforeHover == Color.Empty)
                    _backColorBeforeHover = BackColor; // sichern (einmalig)
                BackColor = HoverColor;
            }
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (HoverColor != Color.Transparent && _backColorBeforeHover != Color.Empty)
            {
                
                BackColor = Selected ? SelectColor : DefaultBackColor;
            }
            if (_hoverResizeGrip)
            {
                _hoverResizeGrip = false;
                Cursor = Cursors.Default;
                Invalidate();
            }
        }


        private bool _hasFocus = false;
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            _hasFocus = true;
            Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {

            base.OnLostFocus(e);
            _hasFocus = false;
            Invalidate();

        }


        protected virtual void OnLeftClicked(EventArgs e)
        {
            LeftClicked?.Invoke(this, e);
        }

        protected virtual void OnRightClicked(EventArgs e)
        {
            RightClicked?.Invoke(this, e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (!Focused) this.Focus();

            if (e.Button == MouseButtons.Left)
            {
                OnLeftClicked(EventArgs.Empty);
            }
            if (e.Button == MouseButtons.Right)
            {
                OnRightClicked(EventArgs.Empty);
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);



            if (_isResizing)
            {
                int dx = e.X - _resizeStart.X;
                int dy = e.Y - _resizeStart.Y;
                int newW = Math.Max(10, _initialSize.Width + dx);
                int newH = Math.Max(10, _initialSize.Height + dy);
                this.Size = new Size(newW, newH);
                this.Invalidate();
                this.Parent?.Invalidate();
                return;
            }

            if (_isDragging)
            {
                var parent = this.Parent;
                if (parent == null) return;
                var newLeft = this.Left + e.X - _dragOffset.X;
                var newTop = this.Top + e.Y - _dragOffset.Y;
                var snapped = Snap(new Point(newLeft, newTop));
                this.Location = snapped;
                parent.Invalidate();
            }
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);



        }
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            // Arrow keys are not delivered via KeyPress; handled in OnKeyDown.
        }

        public abstract void Update();

        // Draw small resize grip (bottom-right) only in edit mode and when hovered/resizing
        protected void DrawResizeGrip(Graphics g)
        {

        }
        private Rectangle GetResizeGripRect()
        {
            return new Rectangle(this.Width - ResizeGripSize - 1, this.Height - ResizeGripSize - 1, ResizeGripSize, ResizeGripSize);
        }
        private bool IsInResizeGrip(Point p)
        {
            return GetResizeGripRect().Contains(p);
        }

        internal StringFormat RightTop = new StringFormat
        {
            Alignment = StringAlignment.Far,
            LineAlignment = StringAlignment.Near
        };
        internal StringFormat LeftTop = new StringFormat
        {
            Alignment = StringAlignment.Near,
            LineAlignment = StringAlignment.Near
        };
        internal StringFormat CenterTop = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Near
        };
        internal StringFormat CenterCenter = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };
        internal StringFormat LeftCenter = new StringFormat
        {
            Alignment = StringAlignment.Near,
            LineAlignment = StringAlignment.Center
        };
        internal StringFormat RightCenter = new StringFormat
        {
            Alignment = StringAlignment.Far,
            LineAlignment = StringAlignment.Center
        };
        internal StringFormat CenterBottom = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Far
        };
        internal StringFormat LeftBottom = new StringFormat
        {
            Alignment = StringAlignment.Near,
            LineAlignment = StringAlignment.Far
        };
        internal StringFormat RightBottom = new StringFormat
        {
            Alignment = StringAlignment.Far,
            LineAlignment = StringAlignment.Far
        };

        internal virtual void Paint(PaintEventArgs e)
        {


        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Paint(e);
        }
    }
    public class SignalView : BaseControl
    {
        protected override void OnBackColorChanged(EventArgs e) { base.OnBackColorChanged(e); Invalidate(); }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string SignalText { get; set; } = "Text";

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string SignalUnit { get; set; } = "°C";

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string SignalValue { get; set; } = "23.5";

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string SourceName { get; set; } = "Unknown";

        public SignalView(Signal signal)
        {

            this.SetStyle(ControlStyles.AllPaintingInWmPaint
                          | ControlStyles.OptimizedDoubleBuffer
                          | ControlStyles.UserPaint, true);
            this.BackColor = Color.White;

            SetSource(signal);
        }


        public SignalView()
        {

            this.SetStyle(ControlStyles.AllPaintingInWmPaint
                          | ControlStyles.OptimizedDoubleBuffer
                          | ControlStyles.UserPaint, true);
            this.BackColor = Color.White;

            LeftClicked += (sender, e) => _edit();
        }

        void _edit()
        {
            double _value = 0;

            if (_source != null)
            {

                var signal = _source as Signals.Signal;

                Debug.WriteLine(signal.Format);
                if (signal.Format != null)
                {
                    if (signal.SupportsWriteback) { }

                    int sep = ConversionExtensions.GetDecimalDigitsFromDotFormat(signal.Format);
                    _value = Math.Round(signal.Value, sep);
                }
                else
                {
                    _value = signal.Value;
                }
            }

            if (EditValue.WithNumPadDialog(ref _value, "Enter new value"))
            {
                if (_source != null)
                {
                    var signal = _source as Signals.Signal;
                    signal.Value = _value;
                    Update();
                }
                else
                {
                    Logger.InfoMsg($"[SignalView] {Name} : Source is null");
                    SignalValue = _value.ToString(CultureInfo.InvariantCulture);
                    Invalidate();
                }
            }
        }
        public override void Update()
        {
            // Debug.WriteLine(Name + "Update");
            var signal = _source as Signals.Signal;
            if (signal == null) return;
            SignalText = signal.Text;
            SignalUnit = signal.Unit;
            if (signal.Value is IFormattable formattable)
            {
                SignalValue = formattable.ToString(signal.Format, CultureInfo.InvariantCulture);
            }
            else
            {
                SignalValue = signal.Value.ToString() ?? string.Empty;
            }
            Invalidate(); // Löst Neuzeichnen aus
        }
        public void SaveInvoke(Action action)
        {
            if (this.InvokeRequired)
                this.Invoke(action);
            else
                action();
        }
        internal override void Paint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            var g = e.Graphics;

            using var borderPen = new Pen(BorderColor);
            g.FillRectangle(new SolidBrush(BackColor), 0, 0, Width - 2, this.ClientSize.Height - 2);
            g.DrawRectangle(borderPen, 0, 0, Width - 2, this.ClientSize.Height - 2);
            using var textBrush = new SolidBrush(ForeColor);

            float fontSize = Height / 4f;
            using var valueFont = new Font(Font.FontFamily, fontSize, FontStyle.Bold);
            using var unitFont = new Font(Font.FontFamily, fontSize, FontStyle.Italic);
            using var headerFont = new Font(Font.FontFamily, fontSize * 0.7f, FontStyle.Regular);

            RectangleF layoutValue = new RectangleF(2, (int)(Height * 0.25), Width - Height - 13, valueFont.Height);
            RectangleF layoutUnit = new RectangleF(Width - Height - 10, (int)(Height * 0.25), Height, valueFont.Height);
            g.DrawString(SignalValue, valueFont, textBrush, layoutValue, RightTop);
            g.DrawString(SignalUnit, unitFont, textBrush, layoutUnit, LeftTop);
            g.DrawString(SignalText, headerFont, textBrush, new PointF(1, 1));

        }
    }
    public class StringSignalView : BaseControl
    {
        protected override void OnBackColorChanged(EventArgs e) { base.OnBackColorChanged(e); Invalidate(); }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string SignalText { get; set; } = "Text";

        //  [Browsable(true)]
        //  [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        //  public string SignalUnit { get; set; } = "°C";

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string SignalValue { get; set; } = "Unknown";

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string SourceName { get; set; } = "Unknown";


        public StringSignalView()
        {

            this.SetStyle(ControlStyles.AllPaintingInWmPaint
                          | ControlStyles.OptimizedDoubleBuffer
                          | ControlStyles.UserPaint, true);
            this.BackColor = Color.White;

            LeftClicked += (sender, e) => _edit();

        }

        public StringSignalView(object source) : this()
        {
            SetSource(source);
            Update();
        }

        public override void Update()
        {
            var signal = _source as Signals.StringSignal;
            if (signal == null) return;
            SignalText = signal.Text;
            SignalValue = signal.Value.ToString() ?? string.Empty;
            
            Invalidate(); // Löst Neuzeichnen aus
        }



        public void SaveInvoke(Action action)
        {
            if (this.InvokeRequired)
                this.Invoke(action);
            else
                action();
        }

        internal override void Paint(PaintEventArgs e)
        {
            //  base.OnPaint(e);
            var g = e.Graphics;

            using var borderPen = new Pen(BorderColor);
            g.FillRectangle(new SolidBrush(BackColor), 0, 0, Width - 2, this.ClientSize.Height - 2);
            g.DrawRectangle(borderPen, 0, 0, Width - 2, this.ClientSize.Height - 2);
            using var textBrush = new SolidBrush(ForeColor);

            float fontSize = Height / 4f;

            using var valueFont = new Font(Font.FontFamily, fontSize, FontStyle.Bold);
            using var unitFont = new Font(Font.FontFamily, fontSize, FontStyle.Italic);
            using var headerFont = new Font(Font.FontFamily, fontSize * 0.7f, FontStyle.Regular);


            RectangleF layoutValue = new RectangleF(10, (int)(Height * 0.25), Width - (Height * 0.3f), valueFont.Height);
            RectangleF layoutUnit = new RectangleF(Width - Height - 10, (int)(Height * 0.25), Height, valueFont.Height);
            g.DrawString(SignalValue, valueFont, textBrush, layoutValue, RightTop);
            g.DrawString(SignalText, headerFont, textBrush, new PointF(1, 1));

            // draw resize grip on top
            DrawResizeGrip(g);
        }


        void _edit()
        {


            string _value = "";

            if (_source != null)
            {
                var signal = _source as Signals.StringSignal;
                _value = signal.Value;
            }

            if (EditValue.WithKeyboardDialog(ref _value, "Enter new value"))
            {
                if (_source != null)
                {
                    var signal = _source as Signals.StringSignal;
                    signal.Value = _value;
                    Update();
                }
                else
                {
                    Logger.InfoMsg($"[StringSignalView] {Name} : Source is null");
                    SignalValue = _value;
                    Invalidate();
                }


            }
        }


    }

    public class ModuleView : BaseControl
    {
        protected override void OnBackColorChanged(EventArgs e) { base.OnBackColorChanged(e); Invalidate(); }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string SignalText { get; set; } = "Text";

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string SignalUnit { get; set; } = "°C";

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string SignalValue { get; set; } = "23.5";

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string OutValue { get; set; } = "45%";

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string SetValue { get; set; } = "34.25";

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string SourceName { get; set; } = "Unknown";

        public ModuleView()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint
                          | ControlStyles.OptimizedDoubleBuffer
                          | ControlStyles.UserPaint, true);
            this.BackColor = Color.White;

            LeftClicked += (sender, e) => _edit();


        }

        void _edit()
        {
            double _value = 0;

            if (_source != null)
            {

                var signal = _source as Signals.Signal;

                Debug.WriteLine(signal.Format);
                if (signal.Format != null)
                {

                    int sep = ConversionExtensions.GetDecimalDigitsFromDotFormat(signal.Format);
                    _value = Math.Round(signal.Value, sep);


                }
                else
                {
                    _value = signal.Value;
                }
            }


            if (EditValue.WithNumPadDialog(ref _value, "Enter new value"))
            {
                if (_source != null)
                {
                    var signal = _source as Signals.Module;

                    if (signal.SupportsWriteback)
                        signal.Set.Write = _value;
                    else
                        signal.Set.Value = _value;


                    Update();
                }
                else
                {
                    Logger.InfoMsg($"[SignalView] {Name} : Source is null");
                    SetValue = _value.ToString(CultureInfo.InvariantCulture);
                    Invalidate();
                }
            }
        }

        public override void Update()
        {
            var signal = _source as Signals.Module;
            if (signal == null) return;
            SignalText = signal.Text;
            SignalUnit = signal.Unit;
            if (signal.Value is IFormattable formattable)
            {
                SignalValue = formattable.ToString(signal.Format, CultureInfo.InvariantCulture);
            }
            else
            {
                SignalValue = signal.Value.ToString() ?? string.Empty;
            }

            if (signal.Out.Value is IFormattable outFormattable)
            {
                OutValue = outFormattable.ToString("0.00", CultureInfo.InvariantCulture) + signal.Out.Unit;
            }
            else
            {
                OutValue = signal.Out.Value.ToString() ?? string.Empty;
            }

            if (signal.Set.Value is IFormattable setFormattable)
            {
                SetValue = setFormattable.ToString(signal.Format, CultureInfo.InvariantCulture);
            }
            else
            {
                SetValue = signal.Set.Value.ToString() ?? string.Empty;
            }


            Invalidate(); // Löst Neuzeichnen aus
        }

        internal override void Paint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            var g = e.Graphics;

            using var borderPen = new Pen(BorderColor);
            g.FillRectangle(new SolidBrush(BackColor), 0, 0, Width - 2, this.ClientSize.Height - 2);
            g.DrawRectangle(borderPen, 0, 0, Width - 2, this.ClientSize.Height - 2);
            using var textBrush = new SolidBrush(ForeColor);

            float fontSize = Height / 4f;


            using var valueFont = new Font(Font.FontFamily, fontSize, FontStyle.Bold);
            using var unitFont = new Font(Font.FontFamily, fontSize, FontStyle.Italic);
            using var headerFont = new Font(Font.FontFamily, fontSize * 0.7f, FontStyle.Regular);
            using var outSetFont = new Font(Font.FontFamily, fontSize * 0.6f, FontStyle.Regular);



            RectangleF layoutValue = new RectangleF(2, (int)(Height * 0.25), Width - Height - 13, valueFont.Height);
            RectangleF layoutUnit = new RectangleF(Width - Height - 10, (int)(Height * 0.25), Height, valueFont.Height);

            RectangleF layoutOut = new RectangleF(2, Height - outSetFont.Height - 2, Width / 2 - 2, outSetFont.Height);
            RectangleF layoutSet = new RectangleF(Width / 2 + 4, Height - outSetFont.Height - 2, Width / 2 - 4, outSetFont.Height);



            g.DrawString(SignalValue, valueFont, textBrush, layoutValue, RightTop);
            g.DrawString(SignalUnit, unitFont, textBrush, layoutUnit, LeftTop);
            g.DrawString("Out: " + OutValue, outSetFont, textBrush, layoutOut, LeftCenter);
            g.DrawString("Set: " + SetValue, outSetFont, textBrush, layoutSet, LeftCenter);
            g.DrawString(SignalText, headerFont, textBrush, new PointF(1, 1));

            // draw resize grip on top
            DrawResizeGrip(g);
        }


    }
    public class ButtonSimple : BaseControl
    {
        protected override void OnBackColorChanged(EventArgs e) { base.OnBackColorChanged(e); Invalidate(); }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string SignalText { get; set; } = "Text";

        //  [Browsable(true)]
        //  [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        //  public string SignalUnit { get; set; } = "°C";

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string SignalValue { get; set; } = "Unknown";

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string SourceName { get; set; } = "Unknown";

        public ButtonSimple()
        {

            this.SetStyle(ControlStyles.AllPaintingInWmPaint
                          | ControlStyles.OptimizedDoubleBuffer
                          | ControlStyles.UserPaint, true);
            this.BackColor = Color.White;

        }

        public override void Update()
        {
            var signal = _source as Signals.StringSignal;
            if (signal == null) return;
            SignalText = signal.Text;
            SignalValue = signal.Value.ToString() ?? string.Empty;

            Invalidate(); // Löst Neuzeichnen aus
        }



        public void SaveInvoke(Action action)
        {
            if (this.InvokeRequired)
                this.Invoke(action);
            else
                action();
        }

        internal override void Paint(PaintEventArgs e)
        {
            //  base.OnPaint(e);
            var g = e.Graphics;


            float fontSize = Height / 4f;

            using var borderPen = new Pen(BorderColor);
            g.FillRectangle(new SolidBrush(BackColor), 0, 0, Width - 2, this.ClientSize.Height - 2);
            g.DrawRectangle(borderPen, 0, 0, Width - 1, this.ClientSize.Height - 1);
            using var textBrush = new SolidBrush(ForeColor);
            using var valueFont = new Font(Font.FontFamily, fontSize, FontStyle.Bold);


            RectangleF layoutValue = new RectangleF(2, 2, Width - 4, Height - 4);

            g.DrawString(SignalValue, valueFont, textBrush, layoutValue, CenterCenter);


            // draw resize grip on top
            DrawResizeGrip(g);
        }
    }
    public class Chart : BaseControl
    {
        protected override void OnBackColorChanged(EventArgs e) { base.OnBackColorChanged(e); Invalidate(); }

        private ScottPlot.WinForms.FormsPlot _chart = new ScottPlot.WinForms.FormsPlot();

        ChartRecorder _recorder;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public List<Signals.Signal> Signals { get; set; } = new();

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int RefreshInterval { get; set; } = 20;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int HistorySeconds { get; set; } = 120;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int ViewSeconds { get; set; } = 30;

        public Chart()
        {
           
            _chart.Padding = new Padding(0);
            _chart.Margin = new Padding(0);
            _chart.BackColor = Color.White;
            Controls.Add(_chart);

            _recorder = new ChartRecorder(Name, _chart, RefreshInterval, HistorySeconds, ViewSeconds);

            this.SetStyle(ControlStyles.AllPaintingInWmPaint
                          | ControlStyles.OptimizedDoubleBuffer
                          | ControlStyles.UserPaint, true);
            this.BackColor = Color.White;

            this.MouseClick += Control_MouseClick;

            LayoutChart();
            _recorder.Start();
            Play();
        }

        ushort id = 0;
        public void Add(Signals.Signal signal, int axis)
        {
            id++;
            _recorder.AddSeries(id: id, name: signal.Name, text: signal.Text, unit: signal.Unit, source: () => signal.Value, axisY: axis, axisX: 1, interval: RefreshInterval);
        }

        public void Add(Signals.Module signal, int axis)
        {
            id++;
            _recorder.AddSeries(id: id, name: signal.Name, text: signal.Text, unit: signal.Unit, source: () => signal.Value, axisY: axis, axisX: 1, interval: RefreshInterval);
        }


        public void SetY1(bool autorange = false , double min = 0, double max = 100, string label = null)
        {
            Debug.WriteLine($"SetY1: autorange={autorange}, min={min}, max={max}, label={label}");

            _recorder.SetY1(autorange, min, max, label);
        }

        public void SetY2(bool autorange = false, double min = 0, double max = 100, string label = null)
        {
            _recorder.SetY2(autorange, min, max, label);
        }

        public void SetY3(bool autorange = false, double min = 0, double max = 100, string label = null)
        {
            _recorder.SetY3(autorange, min, max, label);
        }

        public void SetY4(bool autorange = false, double min = 0, double max = 100, string label = null)
        {
            _recorder.SetY4(autorange, min, max, label);
        }

        public void SetX(int seconds)
        {
            _recorder.SetX(seconds);
        }

        public void SetHistory(int seconds)
        {
            _recorder.SetHistory(seconds);
        }




        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            LayoutChart();
        }

        private void LayoutChart()
        {
            // 1px Border + Platz für Resize-Grip unten/rechts
            int border = 1;
            int reserveRight = ResizeGripSize + 2;
            int reserveBottom = ResizeGripSize + 2;

            int x = border;
            int y = border;
            int w = Math.Max(0, this.ClientSize.Width - (border + reserveRight));
            int h = Math.Max(0, this.ClientSize.Height - (border + reserveBottom + 25));

            _chart.Bounds = new Rectangle(x, y, w, h);
        }

        public override void Update()
        {
        }



        public void SaveInvoke(Action action)
        {
            if (this.InvokeRequired)
                this.Invoke(action);
            else
                action();
        }


        private Rectangle iconPlayRect;
        private Rectangle iconPauseRect;

        internal override void Paint(PaintEventArgs e)
        {
            var g = e.Graphics;

            using var borderPen = new Pen(BorderColor);
            g.FillRectangle(new SolidBrush(BackColor), 0, 0, Width - 2, this.ClientSize.Height - 2);
            g.DrawRectangle(borderPen, 0, 0, Width - 1, this.ClientSize.Height - 1);

            int iconSize = 25;

            iconPlayRect = new Rectangle(Width - 50, Height - 25, iconSize, iconSize);
            iconPauseRect = new Rectangle(Width - 75, Height - 25, iconSize, iconSize);

            //   var iconPlay = new Rectangle(0,0, iconSize, iconSize);
            if (_recorder.Realtime)
            {
                g.DrawImage(Icons.GetFaIcon("play", 23, Color.Orange), iconPlayRect);
                //g.DrawImage(Icons.GetFaIcon("pause", 23, Color.Black), iconPauseRect);
                g.DrawImage(IconChar.Pause.ToBitmap(Color.Black, 23, 0), iconPauseRect);
            }
            else
            {
                g.DrawImage(Icons.GetFaIcon("play", 23, Color.Black), iconPlayRect);
                g.DrawImage(Icons.GetFaIcon("pause", 23, Color.Orange), iconPauseRect);
            }


            // Grip bleibt sichtbar, da _chart kleiner als Client ist
            DrawResizeGrip(g);
        }


        private void Control_MouseClick(object sender, MouseEventArgs e)
        {
            if (iconPlayRect.Contains(e.Location))
            {
                Play();
                Invalidate();
            }
            else if (iconPauseRect.Contains(e.Location))
            {
                Pause();
                Invalidate();
            }
        }


        void Play()
        {
            _recorder.Play();
            _chart.MouseClick += setFocus;

        }

        void Pause()
        {
            _chart.MouseClick -= setFocus;
            _recorder.Pause();

        }

        void setFocus(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("..");
            this.Focus();
        }
    }
    public class ButtonWithIcon : BaseControl
    {
        private string buttonText = "Aktion";
        private string shortcutText = "Ctrl+X";
        private string signalValue = "Unknown";
        private string buttonIcon = "";
        private Image iconImage;
        private int iconSize = 24;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string ButtonText
        {
            get => buttonText;
            set
            {
                buttonText = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string ShortcutText
        {
            get => shortcutText;
            set
            {
                shortcutText = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string SignalValue
        {
            get => signalValue;
            set
            {
                signalValue = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string ButtonIcon
        {
            get => buttonIcon;
            set
            {
                buttonIcon = value;
                iconImage = null; // force reload
                Invalidate();
            }
        }

        public void ResetLedColor()
        {
            LedFillColor = Color.Transparent;

            Invalidate();
        }

        public void SetLedColor(Color color)
        {
            LedFillColor = color;
            Invalidate();
        }

        public void PerformLeftClick()
        {
            OnLeftClicked(EventArgs.Empty);
        }

        public void PerformRightClick()
        {
            OnRightClicked(EventArgs.Empty);
        }



        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            iconImage = null; // reload icon for new size
            Invalidate();
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            Invalidate();
        }

        public override void Update()
        {
            Invalidate();
        }






        bool ledSelected = false;
        public void SetLedSelected(bool selected)
        {
            ledSelected = selected;
            if (ledSelected)
            {
                LedFillColor = LedSelectColor;
            }
            else
            {
                LedFillColor = Color.Transparent;
            }
            Invalidate();
        }




        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color LedSelectColor { get; set; } = Color.Red;

        Color LedFillColor = Color.Transparent;
        Color LedBoarderColor = Color.Black;

        private bool ledVisible = true;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool LedVisible
        {
            get => ledVisible;
            set
            {
                ledVisible = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int LedSize { get; set; } = 15;
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int LedMargin { get; set; } = 2;
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public double IconSizeFactor { get; set; } = 0.6;



        public ButtonWithIcon()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint, true);
            Size = new Size(120, 40);

            Resize += (s,e) => { Invalidate(); };
        }

        internal override void Paint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            using var borderPen = new Pen(BorderColor);
            using var textBrush = new SolidBrush(ForeColor);

            float fontSize = Height / 3f;
            iconSize = (int)(Height * IconSizeFactor);

            using var textFont = new Font(Font.FontFamily, fontSize, FontStyle.Regular);
            using var shortcutFont = new Font(Font.FontFamily, fontSize / 2f, FontStyle.Italic);

            g.FillRectangle(new SolidBrush(BackColor), 0, 0, Width, Height);
            g.DrawRectangle(borderPen, 0, 0, Width - 1, Height - 1);

            // Auto-load icon in correct resolution
            if (iconImage == null && !string.IsNullOrEmpty(ButtonIcon))
            {
                string[] parts = ButtonIcon.Split(':');
                string iconName = parts.Length > 1 ? parts[1] : "";
                string colorName = parts.Length > 2 ? parts[2] : "black";

                iconImage = Icons.GetFaIcon(iconName, iconSize, colorName);
            }

            Rectangle iconRect;
            if (string.IsNullOrEmpty(ButtonText))
            {
                iconRect = new Rectangle((Width - iconSize) / 2, (Height - iconSize) / 2, iconSize, iconSize);
            }
            else
            {
                iconRect = new Rectangle(8, (Height - iconSize) / 2, iconSize, iconSize);
            }

            if (iconImage != null)
                g.DrawImage(iconImage, iconRect);

            if (!string.IsNullOrEmpty(ButtonText))
            {
                var textRect = new Rectangle((int)(Height * 0.8), 0, Width - iconRect.Right - 60, Height);
                g.DrawString(ButtonText, textFont, textBrush, textRect, LeftCenter);
                var shortcutRect = new Rectangle(iconRect.Right + 8, 0, Width - iconRect.Right - 16, Height);
                g.DrawString(ShortcutText, shortcutFont, textBrush, shortcutRect, RightCenter);

            }

            // LED drawing
            if (LedVisible)
            {
                int ledSize = LedSize;
                int ledMargin = LedMargin;

                var ledRect = new Rectangle(
                    ledMargin,           // X: immer vom linken Rand
                    ledMargin,           // Y: immer vom oberen Rand
                    ledSize,
                    ledSize);

                using var fill = new SolidBrush(LedFillColor);
                using var penLed = new Pen(LedBoarderColor, 1);
                g.FillRectangle(fill, ledRect);
                g.DrawRectangle(penLed, ledRect);
            }



        }
    }



    public class BoolSignalView : BaseControl
    {
        protected override void OnBackColorChanged(EventArgs e) { base.OnBackColorChanged(e); Invalidate(); }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string[] TrueFalseText { get; set; } = new[] { "True", "False" };

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color SelectColor { get; set; } = Color.Orange;

        private Rectangle rectTrue;
        private Rectangle rectFalse;

        private BoolSignal _signal;

        public BoolSignalView()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint, true);

            BackColor = Color.White;

            LeftClicked += (s, e) => Focus();
            RightClicked += (s, e) => Focus();
        }

        public override void Update()
        {
            if (_source is BoolSignal sig)
            {
                _signal = sig;
                Invalidate();
            }
        }

     


        internal override void Paint(PaintEventArgs e)
        {
            var g = e.Graphics;

            using var borderPen = new Pen(BorderColor);
            g.FillRectangle(new SolidBrush(BackColor), 0, 0, Width - 2, Height - 2);
            g.DrawRectangle(borderPen, 0, 0, Width - 1, Height - 1);

            if (_signal == null)
                return;

            float m = 6;
            float h = Height * 0.45f;
            float w = (Width * 0.35f) - (2 * m);
            int l = Width - (2 * (int)w) - 3 * (int)m;


            rectTrue = new Rectangle(l, (int)(Height * 0.45), (int)w, (int)h);
            rectFalse = new Rectangle(rectTrue.Right + (int)m, (int)(Height * 0.45), (int)w, (int)h);

            bool? val = _signal.Value ?? false;

            using var f = new Font(Font.FontFamily, Height * 0.25f, FontStyle.Bold);

            using var bTrue = new SolidBrush(val == true ? SelectColor : Color.FromArgb(220, 220, 220));
            using var bFalse = new SolidBrush(val == false ? SelectColor : Color.FromArgb(220, 220, 220));

            using var textBrush = new SolidBrush(ForeColor);
            using var borderBtnPen = new Pen(Color.Black);

            g.FillRectangle(bTrue, rectTrue);
            // g.DrawRectangle(borderBtnPen, rectTrue);

            g.FillRectangle(bFalse, rectFalse);
            //g.DrawRectangle(borderBtnPen, rectFalse);

            g.DrawString(TrueFalseText[0], f, textBrush, rectTrue, CenterCenter);
            g.DrawString(TrueFalseText[1], f, textBrush, rectFalse, CenterCenter);

            using var headerFont = new Font(Font.FontFamily, Height * 0.2f, FontStyle.Regular);
            g.DrawString(_signal.Text, headerFont, textBrush, new PointF(4, 4));

        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (_signal == null)
                return;

            if (rectTrue.Contains(e.Location))
            {
                _edit(true);
            }
            else if (rectFalse.Contains(e.Location))
            {
                _edit(false);
            }
        }

        void _edit(bool set)
        {
            if (_signal is BitSignal bit)
            {
                // 1. Modul holen
                Module sourceModule = SignalPool.GetSignal(bit.SourceName) as Module;
                if (sourceModule == null)
                    return;

                // 2. Aktuelle Set-Wert-Bitmaske holen
                int mask = (int)sourceModule.Set.Value;

                // 3. Neues Bit setzen/löschen
                int newMask;
                if (set)
                    newMask = mask | (1 << bit.BitIndex);
                else
                    newMask = mask & ~(1 << bit.BitIndex);

                // 4. Writeback schreiben (double!)
                if (bit.SupportsWriteback)
                    sourceModule.Set.Write = (double)newMask;
                else
                    sourceModule.Set.Value = (double)newMask;

                return;
            }

            // Standard für normale Signale
            if (_signal.SupportsWriteback)
                _signal.Write = set;
            else
                _signal.Value = set;
        }

    }


    public class Display : BaseControl
    {
        private const int HeaderHeight = 28;
        List<DisplayRow> DataDisplay = new List<DisplayRow>();

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string TitleText { get; set; } = "Device under test";

        public event EventHandler? MenuClicked;

        private FlowLayoutPanel pnlContent;

        private Rectangle menuRect;


        Color contentBackColor;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]

        public Color ContentBackColor
        {
            get => contentBackColor;
            set
            {
                contentBackColor = value;
                Invalidate();
            }
        }


        Color contentForeColor;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]

        public Color ContentForeColor
        {
            get => contentForeColor;
            set
            {
                contentForeColor = value;
                Invalidate();
            }
        }



        bool showMenuIcon = true;
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowMenuIcon
        {
            get => showMenuIcon;
            set
            {
                showMenuIcon = value;
                Invalidate();
            }
        }


        // WICHTIG:
        // Der Designer schreibt Controls in das ContentPanel
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public FlowLayoutPanel ContentPanel => pnlContent;

        public override void Update()
        {
            foreach (Control ctrl in pnlContent.Controls)
            {
                if (ctrl is BaseControl baseCtrl)
                    baseCtrl.Update();
            }
        }

        class DisplayRow
        {
            internal object[] Items;
            internal bool Primary;
            internal int Height;
        }

        public void Add(object[] items, bool primary, int height = 0)
        {

            DataDisplay.Add(new DisplayRow { Items = items, Primary = primary, Height = height });
        }

        public Display()
        {
            pnlContent = new FlowLayoutPanel()
            {
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoScroll = true,
                BackColor = Control.DefaultBackColor,
                Padding = new Padding(6),
                Dock = DockStyle.Fill,
            };
            pnlContent.Height = Height - 10;

            var insetLeftRight = 3;
            var insetTopBottom = 3;

            var x = insetLeftRight;
            var y = HeaderHeight + insetTopBottom;
            var w = Math.Max(0, Width - (2 * insetLeftRight));
            var h = Math.Max(0, Height - HeaderHeight - (2 * insetTopBottom));

            pnlContent.Bounds = new Rectangle(x, y, w, h);

            // Dieses Panel soll der Container für Designer-Drag&Drop sein
            Controls.Add(pnlContent);

            // Layout initial berechnen
       
            Resize += (s, e) => { PerformLayout(); Invalidate(); };
            base.BackColor = Control.DefaultBackColor;
        }

        public void Reset()
        {
            DataDisplay.Clear();
            pnlContent.Controls.Clear();
        }

        public void UpdateDisplay()
        {

          
            pnlContent.SuspendLayout();
            try
            {
                pnlContent.Controls.Clear();
                foreach (var row in DataDisplay)
                {
                    foreach (var item in row.Items)
                    {
                        BaseControl? control = GetControl(item);
                        int width = (pnlContent.Width-25) / row.Items.Count();

                        int height = row.Primary ? 60 : 40;
                        if (row.Height > 0) height = row.Height;

                        Debug.WriteLine(control.GetType().Name + " H: " + height + " W: " + width + " Panel Width " + pnlContent.Width);
                        control.Size = new Size(width, height);
                       

                        control.BackColor = control.Tag == "<space>" ? BackColor : ContentBackColor;
                        control.ForeColor = ContentForeColor;
                        control.BorderColor = Color.Transparent;
                        pnlContent.Controls.Add(control);
                     
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error Update Data " + ex.Message);
               
            }
            finally
            {
                pnlContent.ResumeLayout();

            }
        }
        internal BaseControl? GetControl(object signal)
        {

            if (signal is BaseControl)
            {
                return signal as BaseControl;
            }

            if (signal is string s)
            {

                var c = new LabelControl();
                c.TextValue = s;
                if (s.ToLower() == "<space>")
                {
                    c.TextValue = "";
                    c.BorderColor = Color.Transparent;
                    c.BackColor = Color.WhiteSmoke;
                    c.Tag = "<space>";
                }

                return c;
            }

            if (signal is Module mod)
            {
                var c = new ModuleView();
                c.SetSource(mod);
                c.BorderColor = Color.Transparent;
                return c;
            }


            if (signal is FunkySystem.Signals.Signal sig)
            {
                var c = new SignalView();
                c.SetSource(sig);
                c.BorderColor = Color.Red;
                return c;
            }

            if (signal is FormulaSignal fs)
            {
                var c = new SignalView();
                c.SetSource(fs);
                c.BorderColor = Color.Transparent;
                return c;
            }

            if (signal is StringSignal str)
            {
                var c = new StringSignalView();
                c.SetSource(str);
                c.BorderColor = Color.Transparent;
                return c;
            }



            if (signal is BoolSignal bs)
            {
                var c = new BoolSignalView();
                c.SetSource(bs);
                c.BorderColor = Color.Transparent;
                return c;
            }

            if (signal is BitSignal bit)
            {
                var c = new BoolSignalView();
                c.SetSource(bit); // korrekt: BitSignal übergeben
                c.BorderColor = Color.Transparent;
                return c;
            }

            return null; // unbekannter Typ
        }

        // WICHTIG:
        // Der Designer verwendet DisplayRectangle, um zu wissen,
        // WO Controls eingefügt werden dürfen.
        // Wir verschieben den nutzbaren Bereich unter den Header.
        public override Rectangle DisplayRectangle
            => new Rectangle(0, HeaderHeight, Width, Height - HeaderHeight);


        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);
            // Inset by 1px on each side to keep the outer border visible.
            // Also push below header.
            var insetLeftRight = 3;
            var insetTopBottom = 3;

            var x = insetLeftRight;
            var y = HeaderHeight + insetTopBottom;
            var w = Math.Max(0, Width - (2 * insetLeftRight));
            var h = Math.Max(0, Height - HeaderHeight - (2 * insetTopBottom));

            pnlContent.Bounds = new Rectangle(x, y, w, h);
  
    
        }


        internal override void Paint(PaintEventArgs e)
        {
            var g = e.Graphics;

            // Rahmen
            using (var border = new Pen(BorderColor))
                g.DrawRectangle(border, 0, 0, Width - 1, Height - 1);

            // Header-Titel
            using var titleFont = new Font(Font.FontFamily, 11, FontStyle.Bold);
            using var brush = new SolidBrush(ForeColor);

            g.DrawString(
                TitleText,
                titleFont,
                brush,
                new Rectangle(6, 4, Width - 12, HeaderHeight),
                LeftTop);


            // Menü-Icon
            if (!ShowMenuIcon)
                return;
            int iconSize = 20;
            Image img = Icons.GetFaIcon("Ellipsis", iconSize, Color.Black);

            menuRect = new Rectangle(
                Width - iconSize - 8,
                (HeaderHeight - iconSize) / 2,
                iconSize,
                iconSize);

            if (img != null)
                g.DrawImage(img, menuRect);
        }


        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (menuRect.Contains(e.Location))
                MenuClicked?.Invoke(this, EventArgs.Empty);
        }
    }


        [Designer(typeof(ParentControlDesigner))]
        public class InfoPanelHost : Panel
        {
            public InfoPanelHost()
            {
                BorderStyle = BorderStyle.FixedSingle;
                BackColor = Color.White;
                Resize += (s, e) => { Invalidate(); };
        }

            protected override void OnCreateControl()
            {
                base.OnCreateControl();

                if (DesignMode)
                    return;

                // zur Laufzeit -> echtes InfoPanel einfügen
                var real = new Display();
                real.Dock = DockStyle.Fill;

                // Designer-Kinder (vom Host) in das echte Panel übertragen
                foreach (Control c in this.Controls)
                    real.ContentPanel.Controls.Add(c);

                this.Controls.Clear();
                this.Controls.Add(real);
            }
        }

    public class RadioButtonEx : BaseControl
    {
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color SelectColor { get; set; } = Color.Orange;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color DefaultColor { get; set; } = Color.LightGray;

        bool selected = true;
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool Selected 
        { 
            get => selected; 
            set 
            { 
                selected = value; 
                Invalidate(); 
            }
        }



        private bool showMenuButton = true;
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowMenuButton 
        { get => showMenuButton; 
            
          set { showMenuButton = value; Invalidate(); } 
        
        }



        public event EventHandler? Clicked;
        public event EventHandler? MenuClicked;


        private Rectangle rectNumber;
        private Rectangle rectText;
        private Rectangle rectMenu;


        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string NumberText { get; set; } = "1";


        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string DisplayText 
        { 
            get;
            set; 
        } = "Overview";


        public RadioButtonEx()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.UserPaint, true);
            Height = 32;
            Resize += (s, e) => { Invalidate(); };
            Clicked += (s, e) => { Focus(); };
        }

        public void PerfomClicked() { 
            Clicked?.Invoke(this, EventArgs.Empty);
        }



        public override void Update()
        {
            Invalidate();
        }


        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);


            if (rectMenu.Contains(e.Location) && ShowMenuButton)
            {
                MenuClicked?.Invoke(this, EventArgs.Empty);
                return;
            }


            Clicked?.Invoke(this, EventArgs.Empty);
        }


        internal override void Paint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;


            using var bg = new SolidBrush(BackColor);
            using var border = new Pen(BorderColor);
            g.FillRectangle(bg, 0, 0, Width - 1, Height - 1);
            g.DrawRectangle(border, 0, 0, Width - 1, Height - 1);


            int numWidth = 28;
            int menuSize = 16;
            int margin = 6;


            rectNumber = new Rectangle(2, 2, Height - 4, Height - 4);
            rectText = new Rectangle(rectNumber.Right + margin, 2, Width - numWidth - menuSize - 12, Height - 4);
            rectMenu = new Rectangle(Width - menuSize - margin, (Height - menuSize) / 2, menuSize, menuSize);

          
            using var numBrush = new SolidBrush(selected? SelectColor : DefaultColor);
            g.FillRectangle(numBrush, rectNumber);


            using var textBrush = new SolidBrush(ForeColor);
            using var f = new Font(Font.FontFamily, Height * 0.45f, FontStyle.Bold);

            float fontSize = Height * 0.40f;
            if (DisplayText.Contains("\n"))
            {
                fontSize = Height * 0.25f;
            }

            using var f2 = new Font(Font.FontFamily, fontSize, FontStyle.Regular);


        


            g.DrawString(NumberText, f, textBrush, rectNumber, CenterCenter);
            g.DrawString(DisplayText, f2, textBrush, rectText, LeftCenter);


            if (ShowMenuButton)
            {
                var img = Icons.GetFaIcon("Ellipsis", menuSize, Color.Black);
                if (img != null)
                    g.DrawImage(img, rectMenu);
            }
        }
    }

    public enum HorizontalTextAlignment
    {
        Left,
        Center,
        Right
    }

    public enum VerticalTextAlignment
    {
        Top,
        Center,
        Bottom
    }

    public class LabelControl : BaseControl
    {
        private string _text = "Label";
        private HorizontalTextAlignment _horizontalAlignment = HorizontalTextAlignment.Left;
        private VerticalTextAlignment _verticalAlignment = VerticalTextAlignment.Top;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string TextValue
        {
            get => _text;
            set
            {
                _text = value ?? string.Empty;
                Invalidate();
            }
        }

        // Horizontal alignment selection: Left, Center, Right
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public HorizontalTextAlignment HorizontalAlignment
        {
            get => _horizontalAlignment;
            set
            {
                _horizontalAlignment = value;
                Invalidate();
            }
        }

        // Vertical alignment selection: Top, Center, Bottom
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public VerticalTextAlignment VerticalAlignment
        {
            get => _verticalAlignment;
            set
            {
                _verticalAlignment = value;
                Invalidate();
            }
        }

        // Font height relative to control height (1 line).
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public float FontHeightFactor { get; set; } = 0.3f;

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            Invalidate();
        }

        public LabelControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint, true);
            BackColor = Color.White;
        }

        public override void Update()
        {
            Invalidate();
        }

        private StringFormat ResolveAlignment()
        {
            // Kombiniert horizontale und vertikale Ausrichtung anhand vordefinierter StringFormats aus BaseControl
            return (_horizontalAlignment, _verticalAlignment) switch
            {
                (HorizontalTextAlignment.Left,   VerticalTextAlignment.Top)    => LeftTop,
                (HorizontalTextAlignment.Center, VerticalTextAlignment.Top)    => CenterTop,
                (HorizontalTextAlignment.Right,  VerticalTextAlignment.Top)    => RightTop,

                (HorizontalTextAlignment.Left,   VerticalTextAlignment.Center) => LeftCenter,
                (HorizontalTextAlignment.Center, VerticalTextAlignment.Center) => CenterCenter,
                (HorizontalTextAlignment.Right,  VerticalTextAlignment.Center) => RightCenter,

                (HorizontalTextAlignment.Left,   VerticalTextAlignment.Bottom) => LeftBottom,
                (HorizontalTextAlignment.Center, VerticalTextAlignment.Bottom) => CenterBottom,
                (HorizontalTextAlignment.Right,  VerticalTextAlignment.Bottom) => RightBottom,

                _ => RightCenter
            };
        }

        internal override void Paint(PaintEventArgs e)
        {
            var g = e.Graphics;

            using var borderPen = new Pen(BorderColor);
            using var bgBrush = new SolidBrush(BackColor);
            using var textBrush = new SolidBrush(ForeColor);

            // Background and border
            g.FillRectangle(bgBrush, 0, 0, Width - 1, Height - 1);
            g.DrawRectangle(borderPen, 0, 0, Width - 1, Height - 1);

            // Font size
            float fontSize = Math.Max(1f, Height * FontHeightFactor);
            using var f = new Font(Font.FontFamily, fontSize, FontStyle.Regular);

            // Text layout rectangle (small padding)
            var textRect = new RectangleF(2, 1, Width - 4, Height - 2);

            g.DrawString(_text, f, textBrush, textRect, ResolveAlignment());

            DrawResizeGrip(g);
        }
    }

}
