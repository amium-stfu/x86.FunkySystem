using FunkySystem.Controls;
using FunkySystem.Signals;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FunkySystem.Core
{


    internal static class SignalPool
    {
        private static readonly ConcurrentDictionary<string, object> _values = new();
        internal static void Reset()
        {
            foreach (string key in _values.Keys)
                Remove(key);
        }
        static ConcurrentDictionary<string, List<Action<BaseSignalCommon>>> _pendingLinks = new();

        private static int _clientCounter = 1;
        public static int GetNextId => Interlocked.Increment(ref _clientCounter);


        public static Signal GetSignal(string name, string? unit = null, string? format = null, string? text = null)
        {
            if (_values.ContainsKey(name))
            {
                return _values[name] as Signal;
            }
            else
            {
                Signal signal = new Signal(name, name, "", "", 0.0);
                Set(name, signal);
                signal.Value = 0;

                if (unit != null)
                    signal.Unit = unit;
                if (format != null)
                    signal.Format = format;
                if (text != null)
                    signal.Text = text;


                return signal;
            }
        }
        public static Module GetModule(string name, string? unit = null, string? format = null, string? text = null)
        {
            if (_values.ContainsKey(name))
            {
                return _values[name] as Module;
            }
            else
            {
                Module signal = new Module(name, name, "", "");
                Set(name, signal);
                signal.Value = 0;

                if (unit != null)
                {
                    signal.Unit = unit;
                    signal.Set.Unit = unit;

                }
                if (format != null)
                {
                    signal.Format = format;
                    signal.Set.Format = format;

                }
                if (text != null)
                {
                    signal.Text = text;
                    signal.Set.Text = text+".Set";
                }

                return signal;
            }
        }
        public static StringSignal GetStringSignal(string name)
        {
            if (_values.ContainsKey(name))
            {
                return _values[name] as StringSignal;
            }
            else
            {
                StringSignal signal = new StringSignal(name, name, "");
                Set(name, signal);
                signal.Value = "";
                return signal;
            }
        }
        public static BitSignal GetBit(string sourceName, int bit, string? text = null)
        {
            string name = sourceName + $".0{bit+1}";

            // Schon vorhanden?
            if (TryGet(name, out var existing) && existing is BitSignal bs)
                return bs;

            // Neues Bitsignal erzeugen
            var bitSignal = new BitSignal(name, sourceName, bit);

            // Bei jeder Änderung des Quellsignals aktualisieren
            if (TryGet(sourceName, out var src) && src is BaseSignalCommon sourceSignal)
            {
                sourceSignal.ValueChanged += (_, __) =>
                {
                    bitSignal.UpdateFromSource();
                };
              
            }

            if(text != null)
            {
                bitSignal.Text = text;
            }

            // Registrieren
            Set(name, bitSignal);

            return bitSignal;
        }

        internal static void Set(string key, object value)
        {
            bool isNew = !_values.ContainsKey(key);

            if (isNew)
            {
                _values[key] = value;
                Debug.WriteLine("Add Signal " + key);
          

            }
            else
            {
                _values[key] = value;
            }
            ControlManager.SignalUpdated(key);
        }
        public static T? Get<T>(string key)
        {
            if (_values.TryGetValue(key, out var value) && value is T t)
                return t;
            return default;
        }
        public static bool TryGet(string key, out object value)
        {
            return _values.TryGetValue(key, out value);
        }

        public static void OverwriteKey(string key, object value)
        {
            _values[key] = value;
        
        }

        public static IReadOnlyDictionary<string, object> Snapshot()
        {
            return new Dictionary<string, object>(_values);
        }
        internal static void Remove(string key)
        {
            _values.TryRemove(key, out _);
        }

        public static void RemoveAllKeysStartWith(string prefix)
        {
            var keysToRemove = _values.Keys.Where(k => k.StartsWith(prefix)).ToList();
            foreach (var key in keysToRemove)
            {
                _values.TryRemove(key, out _);
            }
        }

        public static void RemoveTemporarySignals()
        {
            RemoveAllKeysStartWith("#");
        }

        public static IEnumerable<string> Keys => _values.Keys;
        public static DataTable SignalsToDataTable()
        {
            var snapshot = Snapshot(); // Macht ein thread-sicheres Abbild

            DataTable table = new DataTable();
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Text", typeof(string));
            table.Columns.Add("Value", typeof(string));
            table.PrimaryKey = new DataColumn[] { table.Columns["Name"] };

            foreach (var kvp in snapshot)
            {
                if (kvp.Value is BaseSignalCommon signal)
                {
                    string name = signal.Name;
                    string text = signal.GetProperty("Text", "");
                    string valueStr = signal.ValueAsObject?.ToString() ?? "";

                    table.Rows.Add(name, text, valueStr);
                }
            }
            return table;
        }

    }


}
