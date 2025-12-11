using ScottPlot;
using ScottPlot.AxisPanels;
using ScottPlot.Plottables;
using ScottPlot.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FunkySystem.Controls
{
    
    public partial class ScottPlotChart : UserControl
    {
        private ScottPlot.WinForms.FormsPlot _chart = new ScottPlot.WinForms.FormsPlot();

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int RefreshInterval { get; set; } = 20;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int HistorySeconds { get; set; } = 120;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int ViewSeconds { get; set; } = 30;

        ChartRecorder _recorder;

        bool valueFlagVisible = false;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ValueFlagVisible
        {
            get { return valueFlagVisible; }
            set
            {
                valueFlagVisible = value;
                if (_recorder != null)
                    _recorder.valueFlag.IsVisible = valueFlagVisible;
            }
        }

        bool y1AutoScale = true;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool Y1AutoScale
        {
            get { return y1AutoScale; }
            set
            {
                y1AutoScale = value;
                if (_recorder != null)
                    _recorder.Y1AutoScale = y1AutoScale;
            }
        }
        bool y2AutoScale = true;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool Y2AutoScale
        {
            get { return y2AutoScale; }
            set
            {
                y2AutoScale = value;
                if (_recorder != null)
                    _recorder.Y2AutoScale = y2AutoScale;
            }
        }
        bool y3AutoScale = true;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool Y3AutoScale
        {
            get { return y3AutoScale; }
            set
            {
                y3AutoScale = value;
                if (_recorder != null)
                    _recorder.Y3AutoScale = y3AutoScale;
            }
        }
        bool y4AutoScale = true;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool Y4AutoScale
        {
            get { return y4AutoScale; }
            set
            {
                y4AutoScale = value;
                if (_recorder != null)
                    _recorder.Y4AutoScale = y4AutoScale;
            }
        }



        public ScottPlotChart()
        {
            _chart.Dock = DockStyle.Fill;
            InitializeComponent();
            Controls.Add(_chart);
        }



 

        public void AddSeries(UInt16 id, string name, Func<double> source, int interval, string text = null, string unit = "", int axisX = 1, int axisY = 1, bool stepMode = false)
        {
            if (_recorder == null)
            {
                _recorder = new ChartRecorder("ChartRecorder", _chart, RefreshInterval, HistorySeconds, ViewSeconds);
                _recorder.Start();
            }
            _recorder.AddSeries(id, name, source, interval, text, unit, axisX, axisY, stepMode);
        }

        public void Auto()
        {
            if (_recorder != null) _recorder.Play();
        }
        public void Pause()
        {
            if (_recorder != null) _recorder.Pause();
        }

        public void SetY1(double min, double max)
        {
            _recorder.SetY1(min, max);
        }

        public void SetY2(double min, double max)
        {
           _recorder.SetY2(min, max);
        }

        public void SetY3(double min, double max)
        {
            _recorder.SetY3(min, max);
        }

        public void SetY4(double min, double max)
        {
          _recorder.SetY4(min, max);
        }


        public void DarkTheme()
        {

           _recorder.DarkTheme = true;

        }

        public void LightTheme()
        {
            _recorder.DarkTheme = false;
        }

    }





    public class PlotSeries
    {
        public UInt16 Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public string Unit { get; set; }
        public int AxisX { get; set; }
        public int AxisY { get; set; }
        public List<double> Values { get; set; }
        public List<DateTime> Timestamps { get; set; }
        public Scatter? Plot { get; set; }

        public RightAxis ValueFlag;


        int Xrange { get; set; }
        int Interval { get; set; }
        Func<double> Source { get; set; }

        double LastValue;

        bool StepMode;

        Stopwatch stopwatch = new Stopwatch();
        public PlotSeries(UInt16 id, string name, Func<double> source, int xrange, int interval, int axisX = 1, int axisY = 1, string text = null, string unit = "", bool stepMode = false)
        {
            Id = id;
            Name = name;
            Text = text == null ? name : text;
            Unit = unit;
            AxisX = axisX;
            AxisY = axisY;
            Values = new List<double>();
            Timestamps = new List<DateTime>();
            Source = source;
            Xrange = xrange;
            Interval = interval;
            StepMode = stepMode;
            stopwatch.Start();
            LastValue = Source() == null ? double.NaN : Source();

        }
        // 
        public PlotSeries(UInt16 id, string name, int xrange, int axisX = 1, int axisY = 1, string text = null, string unit = "", bool stepMode = false)
        {
            Id = id;
            Name = name;
            Text = text == null ? name : text;
            Unit = unit;
            AxisX = axisX;
            AxisY = axisY;
            Values = new List<double>();
            Timestamps = new List<DateTime>();
            Xrange = xrange;
            StepMode = stepMode;
        }

        public void RemoveLast()
        {
            Values.RemoveAt(0);
            Timestamps.RemoveAt(0);
        }

        public void Clear()
        {
            Values.Clear();
            Timestamps.Clear();
        }

        public void AddPointOnInterval(DateTime datetime)
        {

            if (stopwatch.ElapsedMilliseconds < Interval) return;

            double value = Source();


            if (StepMode) AddPointValue(datetime, LastValue);


            Timestamps.Add(datetime);
            Values.Add(value);

            LastValue = value;
            stopwatch.Restart();

            if ((Timestamps.Count > 0) && (datetime - Timestamps[0]).TotalSeconds > Xrange)
            {
                Timestamps.RemoveAt(0);
                Values.RemoveAt(0);
            }
        }

        public void AddPoint()
        {
            Timestamps.Add(DateTime.Now);
            Values.Add(Source());
        }

        public void AddPointValue(DateTime datetime, double value)
        {

            if (StepMode)
            {
                Timestamps.Add(datetime);
                Values.Add(LastValue);
            }

            Timestamps.Add(datetime);
            Values.Add(value);

            LastValue = value;
            if ((Timestamps.Count > 0) && (datetime - Timestamps[0]).TotalSeconds > Xrange)
            {
                Timestamps.RemoveAt(0);
                Values.RemoveAt(0);
            }
        }

    }
    public class ChartRecorder
    {
        AThread RecordThread;

        bool IsRunning = false;

        DateTime Starttime;
        int RefreshInterval;

        FormsPlot Chart = new FormsPlot();
        public int History = 30;
        public int ViewRange = 60;

        bool recordMode = true;

        public string Name;

        public RightAxis valueFlag = new RightAxis();

        private Dictionary<string, RightAxis> valueFlags = new Dictionary<string, RightAxis>();


        Dictionary<UInt16, PlotSeries> Series = new Dictionary<UInt16, PlotSeries>();
        public LeftAxis y1 = new LeftAxis();
        public LeftAxis y2 = new LeftAxis();
        public LeftAxis y3 = new LeftAxis();
        public LeftAxis y4 = new LeftAxis();

        System.Windows.Forms.Timer updateTimer;

        public bool Y1AutoScale = true;
        public bool Y2AutoScale = true;
        public bool Y3AutoScale = true;
        public bool Y4AutoScale = true;

        public bool DarkTheme = false;

        public ChartRecorder(string name, FormsPlot chart, int refreshInterval, int history, int viewRange)
        {
            Chart = chart;
            History = history;
            ViewRange = viewRange;
            Name = name;
            RefreshInterval = refreshInterval;
            
        
            RecordThread = new("ChartRecorder", work: () => RecordIdle());
            updateTimer = new System.Windows.Forms.Timer();
            updateTimer.Interval = RefreshInterval;
            updateTimer.Tick += new EventHandler((sender, e) => updateIdle());

            Chart.Plot.Axes.DateTimeTicksBottom();
            Chart.Plot.Axes.AddLeftAxis(y2 = new LeftAxis());
            Chart.Plot.Axes.AddLeftAxis(y3 = new LeftAxis());
            Chart.Plot.Axes.AddLeftAxis(y4 = new LeftAxis());

            Chart.Plot.Axes.AddRightAxis(valueFlag = new RightAxis());
     
            Chart.Plot.Axes.Left.Label.Text = "Y1";

            y1.LabelText = "Y1";
            y2.LabelText = "Y2";
            y3.LabelText = "Y3";
            y4.LabelText = "Y4";

            valueFlag.IsVisible = false;

            y2.IsVisible = false;
            y3.IsVisible = false;
            y4.IsVisible = false;

        }


        public void SetY1(bool Autorange = false, double min = 0, double max = 100, string label = null)
        {
            if (label != null)
                Chart.Plot.Axes.Left.Label.Text = label;

            if (Autorange)
            {
                Y1AutoScale = true;
                return;
            }
            Y1AutoScale = false;

            if (max < min)
            {
                MessageBox.Show("Max must be greater than Min");
                return;
            }
            Chart.Plot.Axes.Left.Min = min;
            Chart.Plot.Axes.Left.Max = max;

      
        }

        public void SetY2(bool Autorange = false, double min = 0, double max = 100, string label = null)
        {
            if (label != null)
                y2.LabelText = label;

            if (Autorange)
            {
                Y2AutoScale = true;
                return;
            }
            Y2AutoScale = false;
            if (max < min)
            {
                MessageBox.Show("Max must be greater than Min");
                return;
            }
            y2.Min = min;
            y2.Max = max;

        
        }

        public void SetY3(bool Autorange = false, double min = 0, double max = 100, string label = null)
        {
            if (label != null)
                y3.LabelText = label;

            if (Autorange)
            {
                Y3AutoScale = true;
                return;
            }

            Y3AutoScale = false;
            if (max < min)
            {
                MessageBox.Show("Max must be greater than Min");
                return;
            }
            y3.Min = min;
            y3.Max = max;

   
        }

        public void SetY4(bool Autorange = false, double min = 0, double max = 100, string label = null)
        {
            if (label != null)
                y4.LabelText = label;

            if (Autorange)
            {
                Y4AutoScale = true;
                return;
            }
            Y4AutoScale = false;
            if (max < min)
            {
                MessageBox.Show("Max must be greater than Min");
                return;
            }
            y4.Min = min;
            y4.Max = max;

     
        }

        public void SetX(int seconds)
        {
            ViewRange = seconds;
        }

        public void SetHistory(int seconds)
        {
            History = seconds;
        }





        public void AddSeries(UInt16 id, string name, Func<double> source, int interval, string text = null, string unit = "", int axisX = 1, int axisY = 1, bool stepMode = false)
        {
            Series.Add(id, new PlotSeries(id: id, name: name, source: source, text: text, unit: unit, axisX: axisX, axisY: axisY, xrange: History, interval: interval, stepMode: stepMode));
        }
        public void AddSeries(UInt16 id, string name, string text = null, string unit = "", int axisX = 1, int axisY = 1, bool stepMode = false)
        {
            if (!Series.ContainsKey(id))
                Series.Add(id, new PlotSeries(id: id, name: name, text: text, unit: unit, axisX: axisX, axisY: axisY, xrange: History, stepMode: stepMode));
        }
        public void AddPointToSeries(UInt16 id, DateTime timestamp, double value)
        {
            Series[id].AddPointValue(timestamp, value);
            // Debug.WriteLine(id + ": " + timestamp + " : " + value);

        }
        public bool HasSeries => Series.Count > 0;
        public void Start(bool record = true)
        {
            recordMode = record;
            IsRunning = true;
            updateTimer.Start();
            Debug.WriteLine(Name + "RecordMode: " + recordMode);
            if (recordMode)
                RecordThread.Start();

            Play();


        }
        public void Stop()
        {
            IsRunning = false;
        }

        private bool realtime = true;
        public bool Realtime
        {
            get
            {
                return realtime;
            }
        }

        public void Pause()
        {
            realtime = false;
            Chart.UserInputProcessor.IsEnabled = true;
            EnableMouseTracker();
        }

        public void Play()
        {
            realtime = true;
            Chart.UserInputProcessor.IsEnabled = false;
            DisableMouseTracker();
            Chart.Plot.Legend.ManualItems.Remove(timeLegend);


        }

        void RecordIdle()
        {
            while (!RecordThread.IsStoppRequest)
            {
                DateTime now = DateTime.Now;
                Dictionary<UInt16, PlotSeries> edit = new Dictionary<UInt16, PlotSeries>(Series);

                lock (edit)
                {
                    foreach (PlotSeries series in edit.Values)
                    {
                        if (RecordThread.IsStoppRequest || !IsRunning) break;
                        series.AddPointOnInterval(now);
                    }
                    Thread.Sleep(1);
                }
                // RecordThread.Wait(1);
            }

        }

        int autoScaleCounter = 0;

        void updateIdle()
        {
            if (IsRunning)
            {
                if (Realtime) UpdateChart();
            }
        }



        void UpdateChart()
        {
            try
            {
                Chart.Plot.Clear();
                y2.IsVisible = false;
                y3.IsVisible = false;
                y4.IsVisible = false;

                if(DarkTheme)
                {

                    Chart.Plot.FigureBackground.Color = Colors.Black;
                    Chart.Plot.DataBackground.Color = Colors.Black.Lighten(0.1);
                    Chart.Plot.Grid.MajorLineColor = Colors.Black.Lighten(0.2);

                    Chart.Plot.Legend.BackgroundColor = Colors.Black.Lighten(0.2);
                    Chart.Plot.Legend.FontColor = Colors.White;
                    Chart.Plot.Legend.ShadowColor = Colors.Transparent;
                    Chart.Plot.Legend.OutlineColor = Colors.Transparent;



                    Chart.Plot.Axes.Left.TickLabelStyle.ForeColor = ScottPlot.Colors.White;
                    Chart.Plot.Axes.Bottom.TickLabelStyle.ForeColor = ScottPlot.Colors.White;

                    Chart.Plot.Axes.Left.Label.ForeColor = ScottPlot.Colors.White;
                    Chart.Plot.Axes.Bottom.Label.ForeColor = ScottPlot.Colors.White;

                    y2.LabelFontColor = ScottPlot.Colors.White;
                    y3.LabelFontColor = ScottPlot.Colors.White;
                    y4.LabelFontColor = ScottPlot.Colors.White;

                    y2.TickLabelStyle.ForeColor = ScottPlot.Colors.White;
                    y3.TickLabelStyle.ForeColor = ScottPlot.Colors.White;
                    y4.TickLabelStyle.ForeColor = ScottPlot.Colors.White;
                }
                else
                {
                    Chart.Plot.FigureBackground.Color = Colors.WhiteSmoke;
                    Chart.Plot.DataBackground.Color = Colors.White;
                    Chart.Plot.Grid.MajorLineColor = Colors.White.Darken(0.2);

                    Chart.Plot.Legend.BackgroundColor = Colors.WhiteSmoke;
                    Chart.Plot.Legend.FontColor = Colors.Black;
                    Chart.Plot.Legend.ShadowColor = Colors.Transparent;
                    Chart.Plot.Legend.OutlineColor = Colors.Transparent;


                    Chart.Plot.Axes.Left.TickLabelStyle.ForeColor = ScottPlot.Colors.Black;
                    Chart.Plot.Axes.Bottom.TickLabelStyle.ForeColor = ScottPlot.Colors.Black;

                    Chart.Plot.Axes.Left.Label.ForeColor = ScottPlot.Colors.Black;
                    Chart.Plot.Axes.Bottom.Label.ForeColor = ScottPlot.Colors.Black;

                    y2.LabelFontColor = ScottPlot.Colors.Black;
                    y3.LabelFontColor = ScottPlot.Colors.Black;
                    y4.LabelFontColor = ScottPlot.Colors.Black;

                    y2.TickLabelStyle.ForeColor = ScottPlot.Colors.Black;
                    y3.TickLabelStyle.ForeColor = ScottPlot.Colors.Black;
                    y4.TickLabelStyle.ForeColor = ScottPlot.Colors.Black;
                }


                foreach (RightAxis axis in valueFlags.Values)
                {
                    Chart.Plot.Axes.Remove(axis);
                }

                var ticks = new ScottPlot.TickGenerators.NumericManual();
                foreach (PlotSeries series in Series.Values)
                {
                    lock (series)
                    {

                        if (series.AxisY == 0) continue;
                        var xs = series.Timestamps.ToArray();
                        var ys = series.Values.ToArray();
                        series.Plot = Chart.Plot.Add.ScatterLine(xs, ys);
                        series.Plot.LegendText = "Y" + series.AxisY + " " + series.Text;

                        double min = 0;
                        double max = 100;

                        try
                        {
                            min = ys.Min();
                            max = ys.Max();
                        }
                        catch
                        {
                         //ignore
                        }

                        string value = ys.Last().ToString(getPattern(min, max));
                        ticks.AddMajor(ys.Last(), $"{series.Text}\r\n{value}");

                        if (series.AxisY == 2)
                        {
                            series.Plot.Axes.YAxis = y2;
                            y2.IsVisible = true;

                        }
                        if (series.AxisY == 3)
                        {
                            series.Plot.Axes.YAxis = y3;
                            y3.IsVisible = true;
                        }
                        if (series.AxisY == 4)
                        {
                            series.Plot.Axes.YAxis = y4;
                            y4.IsVisible = true;
                        }
                    }
                }

                DateTime now = DateTime.Now;
                Chart.Plot.ShowLegend(Alignment.UpperLeft);
                double xMin = now.AddSeconds(-ViewRange).ToOADate();
                double xMax = now.ToOADate();
                Chart.Plot.Axes.SetLimits(xMin, xMax);

             

                if (Y1AutoScale) Chart.Plot.Axes.AutoScaleY();
                if (Y2AutoScale) Chart.Plot.Axes.AutoScaleY(y2);
                if (Y3AutoScale) Chart.Plot.Axes.AutoScaleY(y3);
                if (Y4AutoScale) Chart.Plot.Axes.AutoScaleY(y4);

                if (valueFlag.IsVisible)
                {
                    valueFlag.Min = Chart.Plot.Axes.Left.Min;
                    valueFlag.Max = Chart.Plot.Axes.Left.Max;
                    valueFlag.TickGenerator = ticks;
                }

                Chart.Refresh(); // Verwendet Render statt Refresh, um Flackern zu vermeiden


            }
            catch (Exception ex)
            {
                Debug.WriteLine("ChartUpdate Error: " + ex.Message);
            }
        }
        string getPattern(double min, double max)
        {
            double range = max - min;

            range = Math.Abs(range);

            string pattern = "{V:0.0000}";
            if (range > 10)
                pattern = "{V:0.000}";

            if (range > 100)
                pattern = "{V:0.00}";

            if (range > 1000)
                pattern = "{V:0.0}";

            if (range > 10000)
                pattern = "{V:0}";
            return pattern;
        }


        //Crosshair

        LegendItem timeLegend = new LegendItem();
        Annotation crosshairLegend;
        ScottPlot.Plottables.Crosshair CrosshairY1;

        private bool ShowCursorPosition = true;
        private bool ShowHorizontalCrosshair = true;
        private bool ShowVerticalCrosshair = true;
        private bool isMouseInside = false;

        public void EnableMouseTracker()
        {
            CrosshairY1 = Chart.Plot.Add.Crosshair(0, 0);
            CrosshairY1.TextColor = Colors.White;
            CrosshairY1.TextBackgroundColor = CrosshairY1.HorizontalLine.Color;

            Chart.MouseMove += Chart_MouseMove;

            Chart.MouseLeave += Chart_MouseLeave;

            Chart.Refresh();
        }

        public void DisableMouseTracker()
        {
            Chart.MouseMove -= Chart_MouseMove;
            Chart.MouseLeave -= Chart_MouseLeave;
            Chart.Plot.Remove(CrosshairY1);
            Chart.Refresh();
        }


        private void Chart_MouseMove(object sender, MouseEventArgs e)
        {
            if (realtime) return;

            bool currentlyInside = Chart.ClientRectangle.Contains(Chart.PointToClient(System.Windows.Forms.Cursor.Position));

            if (currentlyInside && !isMouseInside)
            {
                isMouseInside = true;
                Chart.Cursor = Cursors.Cross;
            }
            else if (!currentlyInside && isMouseInside)
            {
                isMouseInside = false;
                Chart.Cursor = Cursors.Default;
            }

            Pixel mousePixel = new(e.X, e.Y);
            Coordinates mouseCoordinates = Chart.Plot.GetCoordinates(mousePixel);
            CrosshairY1.Position = mouseCoordinates;
            double xVal = mouseCoordinates.X;
            DateTime x = DateTime.FromOADate(xVal);

            double y1Value = double.NaN;
            double y2Value = double.NaN;
            double y3Value = double.NaN;
            double y4Value = double.NaN;

            string legendText = "Cursor Position:\r\n";

            if (crosshairLegend != null) Chart.Plot.Remove(crosshairLegend);

            if (Chart.Plot.Axes.Left.IsVisible)
            {
                y1Value = mouseCoordinates.Y;
                legendText += $"Y1: {y1Value:N3}\r\n";
            }

            if (y2.IsVisible)
            {
                y2Value = y2.GetCoordinate(mousePixel.Y, Chart.Plot.LastRender.DataRect);
                legendText += $"Y2: {y2Value:N3}\r\n";
            }
            if (y3.IsVisible)
            {
                y3Value = y3.GetCoordinate(mousePixel.Y, Chart.Plot.LastRender.DataRect);
                legendText += $"Y3: {y3Value:N3}\r\n";
            }
            if (y4.IsVisible)
            {
                y4Value = y4.GetCoordinate(mousePixel.Y, Chart.Plot.LastRender.DataRect);
                legendText += $"Y4: {y2Value:N3}\r\n";
            }
            legendText += "X1: " + x.ToString("yyyy-MM-dd HH:mm:ss.fff");


            Chart.Plot.Legend.ManualItems.Remove(timeLegend);

            bool valueFound = false;
            foreach (PlotSeries series in Series.Values)
            {
                double valueOnX = series.Plot.Data.GetNearestX(mouseCoordinates, Chart.Plot.LastRender).Y;

                if (!double.IsNaN(valueOnX))
                {
                    series.Plot.LegendText = "Y" + series.AxisY + " " + series.Text + ": " + Math.Round(valueOnX, 4);
                    valueFound = true;
                }
                else
                {
                    series.Plot.LegendText = "Y" + series.AxisY + " " + series.Text;


                }

            }
            if (valueFound)
            {
                timeLegend.LineColor = Colors.Transparent;
                timeLegend.MarkerFillColor = Colors.Green;
                timeLegend.MarkerLineColor = Colors.Green;
                timeLegend.LineWidth = 4;
                timeLegend.LabelText = "Time: " + x.ToString("yyyy-MM-dd HH:mm:ss.fff");
                Chart.Plot.Legend.ManualItems.Add(timeLegend);
            }




            if (ShowCursorPosition)
            {
                crosshairLegend = Chart.Plot.Add.Annotation("");
                crosshairLegend.Text = legendText;
                crosshairLegend.LabelBackgroundColor = Colors.White;
                crosshairLegend.LabelBorderColor = Colors.Black;
                crosshairLegend.LabelFontSize = 12;
                crosshairLegend.LabelShadowColor = ScottPlot.Color.FromColor(System.Drawing.Color.Transparent);
                crosshairLegend.Alignment = Alignment.UpperRight;
            }



            CrosshairY1.VerticalLine.IsVisible = ShowVerticalCrosshair;
            CrosshairY1.HorizontalLine.IsVisible = ShowHorizontalCrosshair;


            CrosshairY1.VerticalLine.Color = Colors.Red;
            CrosshairY1.HorizontalLine.Color = Colors.Red;

            Chart.Refresh();
        }

        private void Chart_MouseLeave(object sender, EventArgs e)
        {

            //Debug.WriteLine("Leaving Chart");
            System.Windows.Forms.Cursor.Show();
        }


        public void Destroy()
        {
            IsRunning = false;

            if (updateTimer != null)
            {
                updateTimer.Stop();
                updateTimer.Dispose();
                updateTimer = null;
            }

            if (RecordThread != null)
            {
                try
                {
                    RecordThread.Stop();   // dein AThread-Stop
                }
                catch
                {
                    // optional Logging
                }
            }
        }



        public void SetY1(double min, double max)
        {
          
                Y1AutoScale = false;
                Chart.Plot.Axes.Left.Min = min;
                Chart.Plot.Axes.Left.Max = max;
        }

        public void SetY2(double min, double max)
        {
 
                Y2AutoScale = false;
                y2.Min = min;
                y2.Max = max;
            
        }

        public void SetY3(double min, double max)
        {
          
                Y3AutoScale = false;
                y3.Min = min;
                y4.Max = max;
            
        }

        public void SetY4(double min, double max)
        {
                Y4AutoScale = false;
                y4.Min = min;
                y4.Max = max;
        }

    }


    public class AThread
    {
        public string InstanceName { get; init; }
        private Thread _thread;
        private CancellationTokenSource _cts = new();
        public bool IsRunning => _thread.IsAlive && !_cts.IsCancellationRequested;

        bool done = false;

        public bool IsDone
        {
            get => done;
            set
            {
                if (value && !done)
                {
                    done = true;
                    //Logger.DebugMsg($"[AThread] {InstanceName} marked as done.");
                }
            }
        }

        public bool IsStoppRequest => _cts.IsCancellationRequested;

        public AThread(string instanceName, Action work, bool isBackground = true)
        {
            InstanceName = instanceName;

            _thread = new Thread(() =>
            {

                try
                {
                    done = false;
                    work();
                    done = true;
                }
                catch (OperationCanceledException)
                {
                    //Logger.DebugMsg($"[AThread] {InstanceName} cancelled.");
                    done = true;
                }
                catch (Exception ex)
                {
                    //Logger.DebugMsg($"[AThread] {InstanceName} error: {ex.Message}");
                    done = true;
                }

            });
            _thread.IsBackground = isBackground;

            //ThreadsManager.Register(this);
            //Logger.DebugMsg($"[AThread] {InstanceName}: Registered");
        }

        public void Start()
        {
            //Logger.DebugMsg($"[AThread] {InstanceName}: Try to start");
            if (!IsRunning)
            {
                _thread.Start();
                //Logger.DebugMsg($"[AThread] {InstanceName}: Started");
            }
            else
            {
                //Logger.DebugMsg($"[AThread] {InstanceName}: ThreadState {_thread.ThreadState.ToString()} ");
            }
        }

        public void Wait(int milliSeconds)
        {
            DateTime start = DateTime.Now;
            while (DateTime.Now < start.AddMilliseconds(milliSeconds))
            {
                if (IsStoppRequest || !IsRunning) break;
                System.Threading.Thread.Sleep(5);
            }
        }

        public void Stop()
        {
            if (_thread == null || !_thread.IsAlive)
                return;

            //Logger.DebugMsg($"[AThread] Stop requested: {InstanceName}");

            _cts.Cancel();
            if (!_thread.Join(5000))
            {
                //Logger.DebugMsg($"[AThread] Still running after Cancel: {InstanceName} — trying Interrupt...");
                _thread.Interrupt();

                if (!_thread.Join(1000))
                {
                    //Logger.DebugMsg($"[AThread] Cannot stop thread {InstanceName} cleanly.");
                    throw new InvalidOperationException($"Thread {InstanceName} refused to stop.");
                }
            }


            //Logger.DebugMsg($"[AThread] Cleanly stopped: {InstanceName}");
            done = true;
            //ThreadsManager.Deregister(this);
        }
    }


}
