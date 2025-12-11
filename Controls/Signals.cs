using FunkySystem.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FunkySystem.Signals
{
    // ---------------------------------------------------------
    // Property Item
    // ---------------------------------------------------------
    public class PropertyItem
    {
        [System.Xml.Serialization.XmlAttribute("Key")]
        public string Key { get; set; }

        [System.Xml.Serialization.XmlAttribute("Value")]
        public string Value { get; set; }
    }

    // ---------------------------------------------------------
    // Base class (with PendingWrite implementation)
    // ---------------------------------------------------------
    public abstract class BaseSignalCommon
    {
        public string Name { get; protected set; }
        public ulong LastUpdate { get; protected set; }
        public bool register;
        public string LastSender { get; protected set; }

        public event EventHandler? ValueChanged;

        // -----------------------------
        // WRITE (Sollwert)
        // -----------------------------
        public object? PendingWrite { get; protected set; }
        public bool HasPendingWrite => PendingWrite != null;

        //In case the signal supports writeback (Example can id read 0x123 and write 0x223)
        public bool SupportsWriteback { get; protected set; }

        public void ClearPendingWrite() => PendingWrite = null;

        public bool MatchesPendingWrite()
        {
            return PendingWrite != null && Equals(ValueAsObject, PendingWrite);
        }

        protected void SetPendingWrite(object? value)
        {
            PendingWrite = value;
        }

        // -----------------------------
        // PROPERTIES
        // -----------------------------
        public Dictionary<string, string> Properties { get; set; } = new();
        public IEnumerable<string> PropertyKeys => Properties.Keys;

        public string GetProperty(string key, string fallback = "")
        {
            return Properties.TryGetValue(key, out var val) ? val ?? fallback : fallback;
        }

        public void SetProperty(string key, string? value)
        {
            if (value == null)
                Properties.Remove(key);
            else
                Properties[key] = value;

            UpdateStorage("Code");
        }

        // -----------------------------
        // INTERNAL UPDATES
        // -----------------------------
        protected void RaiseValueChanged()
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        protected void UpdateStorage(string sender = null)
        {
            if (!register) return;

            LastUpdate = (ulong)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
            if (sender != null)
                LastSender = sender;

            SignalPool.Set(Name, this);
            RaiseValueChanged();
        }

        public void SetLastSender(string sender) => LastSender = sender;

        // -----------------------------
        // ABSTRACT VALUE
        // -----------------------------
        public abstract object ValueAsObject { get; set; }

        public virtual string Type => "BaseSignal";
    }

    // ---------------------------------------------------------
    // DOUBLE SIGNAL
    // ---------------------------------------------------------
    public abstract class BaseSignalDouble : BaseSignalCommon
    {
        public abstract double Value { get; set; }

        public override object ValueAsObject
        {
            get => Value;
            set => Value = value is double d ? d : Convert.ToDouble(value);
        }

        [JsonIgnore]
        public double? Write
        {
            get => PendingWrite as double?;
            set => SetPendingWrite(value);
        }
    }

    // ---------------------------------------------------------
    // BOOL SIGNAL
    // ---------------------------------------------------------
    public abstract class BaseSignalBool : BaseSignalCommon
    {
        public abstract bool? Value { get; set; }

        public override object ValueAsObject
        {
            get => Value;
            set => Value = value is bool b ? b : Convert.ToBoolean(value);
        }

 

        [JsonIgnore]
        public bool? Write
        {
            get => PendingWrite as bool?;
            set => SetPendingWrite(value);
        }
    }

    // ---------------------------------------------------------
    // STRING SIGNAL
    // ---------------------------------------------------------
    public abstract class BaseSignalString : BaseSignalCommon
    {
        public abstract string Value { get; set; }

        public override object ValueAsObject
        {
            get => Value;
            set => Value = value?.ToString() ?? "";
        }

        [JsonIgnore]
        public string Write
        {
            get => PendingWrite as string;
            set => SetPendingWrite(value);
        }
    }

    // ---------------------------------------------------------
    // DOUBLE SIGNAL IMPLEMENTATION
    // ---------------------------------------------------------
    public class Signal : BaseSignalDouble
    {
        private double _value;

        [JsonIgnore]
        public Action OnUpdate = null;

        public override double Value
        {
            get
            {
                if (SignalPool.TryGet(Name, out var obj) && obj is Signal s)
                    return s._value;
                return double.NaN;
            }
            set
            {
                _value = value;
                UpdateStorage("Code");
            }
        }

        public double DoubleValue
        {
            get => _value;
            set
            {
                _value = value;
                UpdateStorage("Code");
                OnUpdate?.Invoke();
            }
        }

        public bool WriteBack
        {
            get => SupportsWriteback;
            set => SupportsWriteback = value;
        }

        public Signal() { }

        public Signal(string name, string text = null, string unit = "", string format = "0.000",
                      double value = double.NaN, bool register = true, bool writeBack = false)
        {
            SupportsWriteback = writeBack;
            this.register = register;
            Name = name;
            Value = value;

            SetProperty("unit", unit);
            SetProperty("text", text ?? name);
            SetProperty("format", format);

            UpdateStorage();
        }

        public string Unit
        {
            get => GetProperty("unit", "");
            set => SetProperty("unit", value);
        }

        public string Text
        {
            get => GetProperty("text", "#" + Name);
            set => SetProperty("text", value);
        }

        public string Format
        {
            get => GetProperty("format", "0.000");
            set => SetProperty("format", value);
        }

        public override string Type => "Signal";
    }

    // ---------------------------------------------------------
    // STRING SIGNAL IMPLEMENTATION
    // ---------------------------------------------------------
    public class StringSignal : BaseSignalString
    {
        private string _value;

        [JsonIgnore]
        public Action OnUpdate = null;

        public bool WriteBack
        {
            get => SupportsWriteback;
            set => SupportsWriteback = value;
        }

        public override string Value
        {
            get
            {
                if (SignalPool.TryGet(Name, out var obj) && obj is StringSignal s)
                    return s._value;
                return "NA";
            }
            set
            {
                _value = value ?? "NA";
                UpdateStorage("Code");
                OnUpdate?.Invoke();
            }
        }

        public StringSignal() { }

        public StringSignal(string name, string text = null, string value = null, bool register = true, bool writeBack = false)
        {
            SupportsWriteback = writeBack;
            this.register = register;
            Name = name;
            Value = value ?? "NA";
            Text = text ?? ("#" + name);

            UpdateStorage();
        }

        public string Text
        {
            get => GetProperty("text", "#" + Name);
            set => SetProperty("text", value);
        }

        public override string Type => "StringSignal";
    }

    // ---------------------------------------------------------
    // BOOL SIGNAL IMPLEMENTATION
    // ---------------------------------------------------------
    public class BoolSignal : BaseSignalBool
    {
        private bool _value;

        public bool WriteBack
        {
            get => SupportsWriteback;
            set => SupportsWriteback = value;
        }

        public override bool? Value
        {
            get
            {
                if (SignalPool.TryGet(Name, out var obj) && obj is BoolSignal s)
                    return s._value;
                return null;
            }
            set
            {
                _value = value ?? false;
                UpdateStorage("Code");
            }
        }

        public BoolSignal() { }

        public BoolSignal(string name, string text = null, bool register = true, bool writeBack = false)
        {
            SupportsWriteback = writeBack;
            this.register = register;
            Name = name;
            Text = text ?? "#" + name;

            UpdateStorage("Code");
        }

        public string Text
        {
            get => GetProperty("text", "#" + Name);
            set => SetProperty("text", value);
        }

        public override string Type => "BoolSignal";

   
    }

    // ---------------------------------------------------------
    // Digital SIGNAL
    // ---------------------------------------------------------

    public class BitSignal : BoolSignal
    {
        public string SourceName { get; }
        public int BitIndex { get; }

        public BitSignal() { }
        public BitSignal(string name, string sourceName, int bitIndex)
            : base(name, text: null, register: true)
        {
            SourceName = sourceName;
            BitIndex = bitIndex;
            SupportsWriteback = true;

            // erste Initialisierung
            UpdateFromSource();
        }

        

        public void UpdateFromSource()
        {
            if (SignalPool.TryGet(SourceName, out var src) && src is BaseSignalCommon s)
            {
                try
                {
                    int v = Convert.ToInt32(s.ValueAsObject);
                    Value = ((v >> BitIndex) & 1) == 1;
                }
                catch
                {
                    Value = false;
                }
            }
        }
    }








    // ---------------------------------------------------------
    // MODULE SIGNAL
    // ---------------------------------------------------------
    public class Module : Signal
    {
        public Signal Set { get; set; }
        public Signal Out { get; set; }
        public Signal State { get; set; }
        public Signal Command { get; set; }

        public bool WriteBack
        {
            get => SupportsWriteback;
            set 
            {
                SupportsWriteback = value;
                Set.WriteBack = value;
                Out.WriteBack = value;
                State.WriteBack = value;
                

            }
        }

        public Module() { }

        public Module(string name, string text = null, string unit = "",
                      string format = "0.000", bool register = true, bool writeBack = false)
            : base(name, text, unit, format, double.NaN, register, writeBack)
        {
            Name = name;

            Set = new Signal(name + ".set", text + " Set", unit, format, register: register);
            Out = new Signal(name + ".out", text + " Out", "%", "0.00", register: register);
            State = new Signal(name + ".state", text + " State", unit, format, register: register);

            UpdateStorage();
        }

        public override string Type => "Module";
    }

    public class FormulaSignal : Signal
    {
        // Formel kann zur Laufzeit geändert werden
        private Func<CancellationToken, Task<double>> _formula;

        private readonly List<FunkySystem.Signals.BaseSignalCommon> _triggerSignals = new();
        private readonly TimeSpan? _interval;
        private readonly SemaphoreSlim _calcLock = new(1, 1);
        private readonly object _lifecycleLock = new();

        private CancellationTokenSource? _cts;
        private Task? _loopTask;

        // Optionale Begrenzung zur Vermeidung extrem großer Werte
        private const double MaxMagnitude = 1e300;

        public FormulaSignal(
            string name,
            Func<CancellationToken, Task<double>> formula,
            IEnumerable<FunkySystem.Signals.BaseSignalCommon>? triggeredBySignals = null,
            int? triggeredByInterval = null,
            string? text = null,
            string unit = "",
            string format = "0.000",
            bool register = true)
            : base(name, text, unit, format, double.NaN, register)
        {
            _formula = formula ?? throw new ArgumentNullException(nameof(formula));

            if (triggeredBySignals != null)
                _triggerSignals.AddRange(triggeredBySignals);

            if (triggeredByInterval != null)
                _interval = TimeSpan.FromMilliseconds(triggeredByInterval.Value);
        }

        /// <summary>
        /// Komfort-Property zum Setzen der Formel.
        /// Verwendung im Script: signal.Formula = async ct => ...;
        /// </summary>
        public Func<CancellationToken, Task<double>> Formula
        {
            set => SetFormula(value);
        }

        /// <summary>
        /// Setzt die Formel zur Laufzeit neu. Optional kann direkt neu berechnet werden.
        /// Nur diese eine Signatur, damit das Scripting-API einfach bleibt.
        /// </summary>
        private void SetFormula(Func<CancellationToken, Task<double>> newFormula, bool recalculateImmediately = false)
        {
            if (newFormula == null) throw new ArgumentNullException(nameof(newFormula));

            _formula = newFormula;

            if (recalculateImmediately)
            {
                var token = _cts?.Token ?? CancellationToken.None;
                _ = RecalculateAsync(token); // Fire-and-forget, Fehler werden in RecalculateAsync behandelt
            }
        }

        public void Start()
        {
            // Lebenszyklus-Aufrufe seriell halten
            Stop();

            lock (_lifecycleLock)
            {
                _cts = new CancellationTokenSource();
                var token = _cts.Token;

                if (_interval.HasValue)
                {
                    var interval = _interval.Value;

                    _loopTask = Task.Run(async () =>
                    {
                        try
                        {
                            using var timer = new PeriodicTimer(interval);

                            // Optional: sofort einmal berechnen
                            await RecalculateAsync(token).ConfigureAwait(false);

                            while (await timer.WaitForNextTickAsync(token).ConfigureAwait(false))
                            {
                                await RecalculateAsync(token).ConfigureAwait(false);
                            }
                        }
                        catch (OperationCanceledException) when (token.IsCancellationRequested)
                        {
                            // normaler Abbruch – kein Fehler
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"FormulaSignal loop error: {ex}");
                        }
                    });
                }
                else
                {
                    // Event-Trigger registrieren (aktuelles Token nicht capturen)
                    foreach (var sig in _triggerSignals)
                    {
                        sig.ValueChanged += OnSignalValueChanged;
                    }
                }
            }
        }

        private async void OnSignalValueChanged(object? sender, EventArgs e)
        {
            // Falls in der Zwischenzeit Stop() aufgerufen wurde
            var cts = _cts;
            if (cts == null)
                return;

            var token = cts.Token;
            try
            {
                await RecalculateAsync(token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                // normaler Abbruch
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"FormulaSignal event error: {ex}");
            }
        }

        /// <summary>
        /// Stoppt das Signal. Async/Sync-Handhabung erfolgt intern.
        /// Für Scripts gibt es nur diese eine Stop()-Methode.
        /// </summary>
        public void Stop()
        {
            Task? task;
            CancellationTokenSource? cts;

            lock (_lifecycleLock)
            {
                // Events deregistrieren
                foreach (var sig in _triggerSignals)
                {
                    sig.ValueChanged -= OnSignalValueChanged;
                }

                cts = _cts;
                task = _loopTask;

                _cts = null;
                _loopTask = null;
            }

            if (cts is not null)
            {
                try { cts.Cancel(); }
                catch { /* ignore */ }
                finally { cts.Dispose(); }
            }

            if (task is not null)
            {
                // Hintergrund-Warte-Task, blockiert den Aufrufer nicht
                _ = Task.Run(async () =>
                {
                    try { await task.ConfigureAwait(false); }
                    catch (OperationCanceledException) { /* normal */ }
                    catch (Exception ex) { Debug.WriteLine($"FormulaSignal stop error: {ex}"); }
                });
            }
        }

        private async Task RecalculateAsync(CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;

            bool entered;
            try
            {
                // Non-blocking – bei belegt: Berechnung überspringen
                entered = await _calcLock.WaitAsync(0, token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                return; // normaler Abbruch
            }

            if (!entered)
                return;

            try
            {
                var formula = _formula;
                if (formula == null)
                    return;

                double newValue;
                try
                {
                    newValue = await formula(token).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (token.IsCancellationRequested)
                {
                    // Berechnung wurde abgebrochen
                    return;
                }

                // Plausibilitäts-/Überlauf-Check
                if (double.IsNaN(newValue) || double.IsInfinity(newValue))
                {
                    Debug.WriteLine("FormulaSignal: Formel liefert NaN oder Infinity – Wert wird nicht übernommen.");
                    return;
                }

                if (Math.Abs(newValue) > MaxMagnitude)
                {
                    newValue = Math.Sign(newValue) * MaxMagnitude;
                }

                if (!token.IsCancellationRequested)
                {
                    Value = newValue;
                }
            }
            catch (Exception ex)
            {
                // Nur echte Fehler loggen – Cancellation wird oben abgefangen
                Debug.WriteLine($"FormulaSignal error: {ex}");
            }
            finally
            {
                _calcLock.Release();
            }



        }

        public class MappingSignal1D : Signal
        {
            private readonly Mapping1D _map;

            /// <summary>
            /// Eingang für die X-Größe (z.B. Temperatur).
            /// </summary>
            public Signal Set { get; }

            public string UnitX
            {
                get => GetProperty("unitX", "");
                set => SetProperty("unitX", value);
            }

            public string UnitY
            {
                get => Unit; // Alias auf normales Unit
                set => Unit = value;
            }

            public string LabelX
            {
                get => GetProperty("labelX", "X");
                set => SetProperty("labelX", value);
            }

            public string LabelY
            {
                get => GetProperty("labelY", "Y");
                set => SetProperty("labelY", value);
            }

            public MappingSignal1D(
                string name,
                Mapping1D map,
                string unitX = "",
                string unitY = "",
                string text = null,
                bool register = true)
                : base(name, text ?? name, unitY, "0.000", double.NaN, register, writeBack: false)
            {
                _map = map ?? throw new ArgumentNullException(nameof(map));

                // Metadaten
                UnitX = unitX;
                UnitY = unitY;
                LabelX = "X";
                LabelY = "Y";

                // X-Eingangssignal anlegen (z.B. "CutOff.Umin.set")
                Set = new Signal(name + ".set", (text ?? name) + " Set", unitX, "0.000", double.NaN, register: register, writeBack: true);
            }

            /// <summary>
            /// Mapping-Signal ist read-only, daher eigener Getter.
            /// Set.Value ist der X-Wert, Value gibt den interpolierten Y-Wert zurück.
            /// </summary>
            public override double Value
            {
                get
                {
                    var x = Set.Value;
                    return _map.Interpolate(x);
                }
                set => throw new NotSupportedException("MappingSignal1D is read-only. Verwende Set.Value.");
            }

            public override string Type => "Mapping1D";

            /// <summary>
            /// Komfortmethode für direkten Aufruf ohne Set-Änderung.
            /// </summary>
            public double Eval(double x) => _map.Interpolate(x);
        }


    }









}

// ---------------------------------------------------------
// FORMULA SIGNAL
// ---------------------------------------------------------













