using System;

using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Documents;
using FunkySystem.Core;
using FunkySystem.Signals;


namespace FunkySystem.Controls
{

    public class ModuleController : Module
    {
        private readonly Signal _source;

        public Signal Source => _source;
       

        public ModuleController(string name, Signal source, string? text = null,
                                string? unit = null, string? format = null,
                                bool register = true)
            : base(name,
                   text ?? source.Text,
                   unit ?? source.Unit,
                   format ?? source.Format,
                   register: register,
                   writeBack: false)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _source.ValueChanged += OnSourceChanged;

            // Initialer Zustand
            OnSourceChanged(this, EventArgs.Empty);
        }

        public override double Value
        {
            get => _source.Value;
            set
            {
                // Optional: Entweder ignorieren oder auf Set schreiben.
                // Set.Value = value;
            }
        }

        private void OnSourceChanged(object? sender, EventArgs e)
        {
            SetLastSender(_source.Name);
            UpdateStorage("SourceForward");
        }

    }





    /// <summary>
    /// PID-Regler basierend auf Ks, Tu, Tg.
    /// Nutzt Module.Set als Sollwert und schreibt Stellgröße in ein externes Out-Signal.
    /// </summary>
    public class PIDController : Module
    {
        private readonly Signal _source;     // Istwert
        private readonly Signal _out;        // Ausgangssignal
       

        //  public readonly Signal Interval;
        public readonly Signal Ks;
        public readonly Signal Tu;
        public readonly Signal Tg;
        public readonly Signal OutMin;
        public readonly Signal OutMax;
        public readonly Signal DFilterTau;
        public readonly Signal ComputeInterval;
        public readonly Signal OutputInterval;

 
        // Laufzeitvariablen
        private double _lastError;
        private double _integral;


        private readonly System.Threading.Timer _timer;

        private  System.Threading.Timer _computeTimer; // schneller Rechentakt
        private  System.Threading.Timer _outputTimer;  // langsamer Ausgabetakt

        // CHR-Speicher
        private double _e1 = 0;
        private double _e2 = 0;
        private double _o_prev = 0;

        // Zeit
        private ulong _lastTimestamp = 0;

        // Filter
        private double _dFiltered = 0;

        /// <summary>
        /// Updateintervall der Regelschleife in Millisekunden.
        /// </summary>
        public int UpdateInterval { get; }

        public PIDController(
                 string name,
                 Signal source,
                 Signal outSignal,
                 double ks,
                 double tu,
                 double tg,
                 int dFilterTauMs = 200,
                 int computeInterval = 50,
                 int outputInterval = 50,
                 double outMin = 0,
                 double outMax = 100,
                 string? text = null,
                 string? unit = null,
                 string? format = null,
                 bool register = true)
                 : base(name,
                        text ?? source.Text,
                        unit ?? source.Unit,
                        format ?? source.Format,
                        register: register,
                        writeBack: false)
        {

          //  Interval = new(name: $"{name}.Interval", text: "Interval", unit: "ms", format: "0", value: updateIntervalMs);
            Ks = new(name: $"{name}.Ks", text: "Loop factor", unit: "", format: "0.00", value: ks);
            Tu = new(name: $"{name}.Tu", text: "Dead time", unit: "s", format: "0.00", value: tu);
            Tg = new(name: $"{name}.Tg", text: "Rise time", unit: "s", format: "0.00", value: tg);
            OutMin = new(name: $"{name}.OutMin", text: "Out min", unit: "", format: "0.00", value: outMin);
            OutMax = new(name: $"{name}.OutMax", text: "Out max", unit: "", format: "0.00", value: outMax);
            DFilterTau = new($"{name}.DFilterTau", "D Filter Time", unit: "ms", "0", value: dFilterTauMs);
            ComputeInterval = new($"{name}.ComputeInterval", "Compute interval", "ms", "0", value: computeInterval);
            OutputInterval = new($"{name}.OutputInterval", "Output interval", "ms", "0", value: outputInterval);
          




            ComputeInterval.ValueChanged += (s, e) =>
            {
                int iv = (int)ComputeInterval.Value;
                if (iv < 1) iv = 1;
                _computeTimer.Change(iv, iv);
            };

            OutputInterval.ValueChanged += (s, e) =>
            {
                int iv = (int)OutputInterval.Value;
                if (iv < 1) iv = 1;
                _outputTimer.Change(iv, iv);
            };


            _source = source;
            _out = outSignal;

            _source.ValueChanged += (s,e) => UpdateStorage("SourceForward");

            // PID Parameter


            _lastTimestamp = GetTimestamp();
         //   _timer = new System.Threading.Timer(OnTick, null, updateIntervalMs, updateIntervalMs);

        }

        public void Start()
        {
            _computeTimer = new System.Threading.Timer(ComputeTick, null, (int)ComputeInterval.Value, (int)ComputeInterval.Value);
            _outputTimer = new System.Threading.Timer(OutputTick, null, (int)OutputInterval.Value, (int)OutputInterval.Value);
            Out.Value = 0;
        }

        public void Stop()
        {
            _computeTimer?.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            _outputTimer?.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

        }

        public override double Value
        {
            get => _source.Value;
            set
            {
                // Optional: Entweder ignorieren oder auf Set schreiben.
                // Set.Value = value;
            }
        }

        private static ulong GetTimestamp()
        {
            return (ulong)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }


       



        private void ComputeTick(object? state)
        {

            double pv = _source.Value;
            double sp = Set.Value;
            double error;
         
            error = sp - pv;



            ulong now = GetTimestamp();
            double dt = (now - _lastTimestamp) / 1000.0; // Sekunden

            if (_lastTimestamp == 0 || dt <= 0)
            {
                _lastTimestamp = now;
                _e1 = error;
                _e2 = error;
                _dFiltered = 0;
                _o_prev = Out.Value;
                return;
            }

            _lastTimestamp = now;

            // --- Compute-Zeit behaupten wir konstant (für CHR!)
            double T = ComputeInterval.Value / 1000.0; // Sekunden
            if (T < 0.001) T = 0.001;

            // --- CHR-Reglerparameter ---
            double ks = Ks.Value;
            double tu = Tu.Value;
            double tg = Tg.Value;

            if (ks <= 0 || tu <= 0 || tg <= 0)
                return;

            double kr = 0.95 * tg / (ks * tu);
            double tn = 2.4 * tu;
            double tv = 0.42 * tu;

            // --- D-Filter auf tv ---
            double tau = DFilterTau.Value / 1000.0;
            if (tau > 0)
            {
                double alpha = tau / (tau + dt);
                _dFiltered = alpha * _dFiltered + (1.0 - alpha) * tv;
                tv = _dFiltered;
            }

            // --- Digitale Koeffizienten (Tustin) ---
            double b0 = kr * (1.0 + (T / (2 * tn)) + (tv / T));
            double b1 = -kr * (1.0 - (T / (2 * tn)) + (2 * (tv / T)));
            double b2 = kr * (tv / T);

            // --- CHR-Diskret PID ---
            double o = _o_prev;
            o += error * b0;
            o += _e1 * b1;
            o += _e2 * b2;

            // --- Anti-Windup vorbereiten ---
            _o_prev = o;

            _e2 = _e1;
            _e1 = error;

            
            Out.Value = _o_prev;
            UpdateStorage("computeController");


        }


        private void OutputTick(object? state)
        {
            double u_raw = _o_prev;

            double u = Math.Max(OutMin.Value, Math.Min(OutMax.Value, u_raw));

            // Anti-Windup (Rückführung)
            double Kb = 1.0;
            _o_prev += (u - u_raw) * Kb;

            if (double.IsNaN(u) || double.IsInfinity(u)) return;
            _out.Value = u;
         //   Out.Value = u;

        }

        private List<(double t, double pv, double u)> _teachBuffer;
        private bool _teachActive = false;
        private ulong _teachStartTs;


        //------------------------------------------------------------------
        // TEACH MODE (einzige gewünschte Methode)
        //------------------------------------------------------------------

        private void TeachCapture(double pv)
        {
            if (!_teachActive)
                return;

            // Zeit relativ zum Teach-Start
            double t = (GetTimestamp() - _teachStartTs) / 1000.0;

            // Log-Eintrag: Zeit, PV, Stellgröße
            _teachBuffer.Add((t, pv, _o_prev));
        }

        public async Task Teach(int seconds, double outValue)
        {
            if (_teachActive) return;


            _teachActive = true;
            _teachBuffer = new List<(double, double, double)>();
            _teachStartTs = GetTimestamp();


            // Ausgang festsetzen
            _o_prev = Math.Max(OutMin.Value, Math.Min(OutMax.Value, outValue));
            _out.Value = _o_prev;


            // Timer starten
            int totalMs = Math.Max(100, seconds * 1000);

            Task record = Task.Run(async () =>
            {


                while (_teachActive)
                {

                    await Task.Delay(10);
                    TeachCapture(_source.Value);

                    if (_teachBuffer.Count() >= 500)
                    {
                        FinishTeach();
                    }
                }
                   
               

            });
            

            // var _ = new System.Threading.Timer((_) => FinishTeach(), null, seconds*1000, System.Threading.Timeout.Infinite);
        }

        private void FinishTeach()
        {
            if (!_teachActive) return;
            _teachActive = false;


            if (_teachBuffer.Count < 5) return;


            double pv0 = _teachBuffer[0].pv;
            double pvMax = MaxPV();


            // Bereich 20–70% bestimmen
            double lo = pv0 + 0.2 * (pvMax - pv0);
            double hi = pv0 + 0.7 * (pvMax - pv0);


            var pts = _teachBuffer.FindAll(p => p.pv >= lo && p.pv <= hi);
            if (pts.Count < 2) return;


            // Steigung
            double slope = (pts[^1].pv - pts[0].pv) / (pts[^1].t - pts[0].t);


            // Ks
            double uStep = _teachBuffer[0].u;
            double ks = slope / uStep;


            // Tu
            double tu = FindTu(lo);


            // Tg
            double tg = (pvMax - pv0) / slope;


            // CHR
            double kp = 0.95 * tg / (ks * tu);
            double tn = 2.4 * tu;
            double tv = 0.42 * tu;


            // Set Streckenparameter (statt PID-Parameter)
            Ks.Value = ks;
            Tu.Value = tu;
            Tg.Value = tg;
        }

        //------------------------------------------------------------------
        // Hilfsfunktionen
        //------------------------------------------------------------------
        private double MaxPV()
        {
            double m = double.MinValue;
            foreach (var p in _teachBuffer) if (p.pv > m) m = p.pv;
            return m;
        }


        private double FindTu(double limit)
        {
            foreach (var p in _teachBuffer)
                if (p.pv >= limit) return p.t;
            return 0.1;
        }
    }


    public class PredictivePIDController : Module
    {
        public readonly PIDController PID;
        private readonly Signal _source;
        public readonly Signal ComputeValue;

        private readonly Queue<(double value, ulong time)> _history = new();

        public Signal LookBackMs { get; }
        public Signal LookAheadMs { get; }
        public Signal ComputeInterval { get; }
        public Signal OutputInterval { get; }
        public Signal PredictedValue { get; }

        private System.Threading.Timer _computeTimer;
        private System.Threading.Timer _outputTimer;

        private ulong _lastComputeTs = 0;

        public PredictivePIDController(
            string name,
            Signal source,
            Signal outSignal,
            double ks,
            double tu,
            double tg,
            double lookBackMs = 200,
            double lookAheadMs = 200,
            int dFilterTauMs = 200,
            int computeInterval = 50,
            int outputInterval = 50,
            double outMin = 0,
            double outMax = 100
            )
            : base(name, register: true)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            LookBackMs = new($"{name}.LookBack", "LookBack window", "ms", value: lookBackMs);
            LookAheadMs = new($"{name}.LookAhead", "LookAhead window", "ms", value: lookAheadMs);

            ComputeInterval = new($"{name}.ComputeInterval", "Compute interval", "ms", value: computeInterval);
           
            OutputInterval = new($"{name}.OutputInterval", "Output interval", "ms", value: outputInterval);

            PredictedValue = new($"{name}.PredictedValue", "Predicted PV", unit: source.Unit, value:source.Value);

            PID = new PIDController(
                name + ".InnerPID",
                source: PredictedValue,
                outSignal: outSignal,
                ks: ks,
                tu: tu,
                tg: tg,
                dFilterTauMs: dFilterTauMs,
                outMin: outMin,
                outMax: outMax,
                computeInterval: computeInterval,
                outputInterval: outputInterval
                );

            _source.ValueChanged += (s, e) => UpdateStorage("SourceForward");
            Set.ValueChanged += (s, e) => SendSetToPID();

            _computeTimer = new System.Threading.Timer(ComputeTick, null, computeInterval, computeInterval);
        }

        void SendSetToPID()
        {
            Debug.WriteLine($"PredictivePIDController: Set changed to {Set.Value}, forwarding to inner PID.");
            PID.Set.Value = Set.Value;
        }

        public override double Value
        {
            get => _source.Value;
            set
            {
                // Optional: Entweder ignorieren oder auf Set schreiben.
                // Set.Value = value;
            }
        }

        public void Start()
        {
            PID.Start();
        }
        public void Stop()
        {

            PID.Stop();
        }

        private static ulong GetTimestamp()
        {
            return (ulong)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        private void ComputeTick(object? state)
        {
           
            double pv = _source.Value;
            ulong ts = GetTimestamp();

            _history.Enqueue((pv, ts));

            while (_history.Count > 0 && (ts - _history.Peek().time) > (ulong)LookBackMs.Value)
                _history.Dequeue();

            if (_history.Count < 2)
                return;

            var oldest = _history.Peek();
            double dt = (ts - oldest.time) / 1000.0;
            if (dt <= 0) return;

            double slope = (pv - oldest.value) / dt;

            double leadSec = LookAheadMs.Value / 1000.0;
            double pvFuture = pv + slope * leadSec;


            PredictedValue.Value = double.IsNaN(pvFuture) ? _source.Value : pvFuture;


            double errorFuture = PID.Set.Value - pvFuture;

            UpdateStorage("PredictiveCompute");
        }
    }


}
