using FunkySystem.Controls;
using FunkySystem.Signals;
using ScottPlot.Colormaps;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunkySystem.Helpers

{

    public class SimulationSinus
    {

        Thread IdleThread;

        bool IsRunning = false;

        DateTime Starttime;

        int Interval;


        public double S1;
        public double S2;
        public double S3;

        public SimulationSinus(int interval = 10)
        {
            Interval = interval;
            IdleThread = new Thread(Idle);
            IdleThread.IsBackground = true;
            Start();
        }

        public void Start()
        {
            S1 = 0;
            S2 = 0;
            S3 = 0;

            IsRunning = true;
            Starttime = DateTime.Now;
            IdleThread = new Thread(Idle);
            IdleThread.IsBackground = true;
            IdleThread.Start();
        }

        public void Stop()
        {
            IsRunning = false;
            if (IdleThread != null && IdleThread.IsAlive)
                IdleThread.Join();
        }

        void Idle()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (IsRunning)
            {
                double elapsedSeconds = (DateTime.Now - Starttime).TotalSeconds;
                S1 = Math.Sin(elapsedSeconds);
                S2 = Math.Sin(elapsedSeconds + Math.PI / 2);
                S3 = Math.Sin(elapsedSeconds + Math.PI);
                while (stopwatch.ElapsedMilliseconds < Interval)
                {
                    Thread.SpinWait(1);
                }
                stopwatch.Restart();
            }
        }

    }


    public class SignalSimulator
    {
        private readonly System.Windows.Forms.Timer _timer;
        private readonly Random _random = new Random();
        private readonly Random _random2 = new Random();
        private readonly Random _random3 = new Random();
        private double _baseLevel = 100.0;
        private double _trend = -0.01;
        private double _noiseAmplitude = 1.5;

        private double? _setValue = null;
        private bool _targetReached = false;

        public float Value;

        public event Action<double> OnNewValue;

        public SignalSimulator(double intervalMs = 100)
        {
            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = (int)intervalMs;
            _timer.Tick += (s, e) => GenerateNextValue();
            _timer.Start();
        }

        public void Start() => _timer.Start();
        public void Stop() => _timer.Stop();

        public void SetBaseLevel(double newLevel) => _baseLevel = newLevel;
        public void SetTrend(double newTrend) => _trend = newTrend;
        public void SetNoise(double noiseAmplitude) => _noiseAmplitude = noiseAmplitude;

        public void SetValue(double value, double trend)
        {
            _setValue = value;
            _trend = trend;
            _targetReached = false;
        }

        private int counter = 0;
        private int counterSet = 0;

        private void GenerateNextValue()
        {
            if (counter == 0)
                counterSet = _random2.Next(10, 100);

            double noise = (_random.NextDouble() - 0.5) * 2.0 * _noiseAmplitude;

            if (_setValue.HasValue && !_targetReached)
            {
                // Zielansteuerung
                _baseLevel += _trend;

                bool reached = (_trend < 0 && _baseLevel <= _setValue) ||
                               (_trend > 0 && _baseLevel >= _setValue);

                if (reached)
                {
                    _baseLevel = _setValue.Value;
                    _trend = 0;
                    _targetReached = true;
                }
            }
            else
            {
                // Normales Verhalten nach Zielerreichung
                _baseLevel += _trend;
            }

            double value = _baseLevel + noise;
            Value = (float)value;
            OnNewValue?.Invoke(value);

            counter++;
            if (counter >= counterSet)
            {
                counter = 0;
                if (_trend != 0 && !_setValue.HasValue)
                {
                    SetTrend(_random3.Next(-30, 30) / 1000.0);
                }
            }
        }
    }


    public class DemoModule : Module
    {
        // Set-/Ist-Werte (Tau wirkt als Zeitkonstante in Sekunden)
        private double _tau = 0.1;
        public double Tau
        {
            get { lock (_lock) return _tau; }
            set
            {
                lock (_lock)
                {
                    if (double.IsNaN(value) || value <= 0) value = 1e-3;
                    _tau = value;
                }
            }
        }

        // Update-Periode (ms) für den Simulations-Thread
        public int UpdateRateMs { get; set; } = 100;

        // Rauschparameter
        private double _noiseStrength = 0.0;
        public double NoiseStrength
        {
            get { lock (_lock) return _noiseStrength; }
            set { lock (_lock) _noiseStrength = value < 0 ? 0 : value; }
        }

        // Noise-Frequenz: alle N Updates neuer Noise-Wert
        private int _noiseFrequency = 1;
        public int NoiseFrequency
        {
            get { lock (_lock) return _noiseFrequency; }
            set { lock (_lock) _noiseFrequency = value < 1 ? 1 : value; }
        }

        // Aktueller Noise-Wert
        public double Noise { get; private set; } = 0.0;

        // Peak-Injektion (einmalig addiert beim nächsten Sample)
        private int _noisePeak = 0;

        // Zähler für diskrete Noise-Erneuerung
        private int _noiseCounter = 0;

        // Thread / Timing
        private readonly object _lock = new();
        private readonly AThread _thread;
        private readonly Stopwatch _sw = new();
        private double _lastTime;

        // Letzte gültige Value zur Robustheit
        private double _lastGoodValue = 0;

        public double TauValue
        {
            get { lock (_lock) return _tau; }
            set
            {
                lock (_lock)
                {
                    if (double.IsNaN(value) || value <= 0) value = 1e-3;
                    _tau = value;
                }
            }
        }

        public double NoiseVal
        {
            get { lock (_lock) return _noiseStrength; }
            set { lock (_lock) _noiseStrength = value < 0 ? 0 : value; }
        }

        public int NoiseFreq
        {
            get { lock (_lock) return _noiseFrequency; }
            set { lock (_lock) _noiseFrequency = value < 1 ? 1 : value; }
        }

        public DemoModule(string name, string text, string unit)
            : base(name, register: true)
        {
            Text = text;
            Unit = unit;

            Value = 0;
            Set.Value = 0;
            Out.Value = 0;

            _tau = 0.1;
            _noiseStrength = 0.000;
            _noiseFrequency = 0;

            _sw.Start();
            _lastTime = _sw.Elapsed.TotalSeconds;

            _thread = new AThread("DemoSignalThread", RunLoop, isBackground: true);
            _thread.Start();
        }

        private void RunLoop()
        {
            while (_thread.IsRunning)
            {
                double now = _sw.Elapsed.TotalSeconds;
                double dt = now - _lastTime;
                _lastTime = now;

                if (dt < 0 || dt > 5) dt = 0.0; // Schutz gegen Sprünge

                lock (_lock)
                {
                    StepNoise();
                    StepDynamics(dt);
                }

                // Optional Out-Signal spiegeln
                Out.Value = Value;

                System.Threading.Thread.Sleep(UpdateRateMs);
            }
        }

        private void StepDynamics(double dt)
        {
            // Eingaben prüfen
            double set = Set.Value;
            double tau;
            lock (_lock) tau = _tau;
            if (tau <= 1e-9) tau = 1e-3;

            if (double.IsNaN(set) || double.IsInfinity(set))
                set = _lastGoodValue;

            // 1. Ordnung Low-Pass Annäherung
            // alpha in (0..1), für kleine dt/tau ≈ dt / tau
            double alpha = 1 - Math.Exp(-dt / tau);
            if (alpha < 0) alpha = 0;
            else if (alpha > 1) alpha = 1;

            double newVal = Value + alpha * (set - Value) + Noise;

            if (double.IsNaN(newVal) || double.IsInfinity(newVal))
            {
                Value = _lastGoodValue;
                Noise = 0;
            }
            else
            {
                Value = newVal;
                _lastGoodValue = Value;
            }
        }

        private static readonly ThreadLocal<Random> _rnd = new(() => new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId)));

        private void StepNoise()
        {
            int nf;
            double ns;
            lock (_lock)
            {
                nf = _noiseFrequency;
                ns = _noiseStrength;
            }
            if (nf < 1) nf = 1;

            _noiseCounter++;
            if (_noiseCounter >= nf)
            {
                _noiseCounter = 0;
                var r = _rnd.Value;
                double baseNoise = (r.NextDouble() * 2 - 1) * ns + _noisePeak;
                Noise = baseNoise;
                _noisePeak = 0;
            }
            else
            {
                Noise = _noisePeak;
                _noisePeak = 0;
            }
        }

        public void AddPeak(int min = -500, int max = 500)
        {
            if (min > max) (min, max) = (max, min);
            var r = _rnd.Value;
            lock (_lock)
            {
                _noisePeak = r.Next(min, max + 1);
            }
        }

        public void Stop()
        {
            try { _thread.Stop(); } catch { }
        }
    }

}



