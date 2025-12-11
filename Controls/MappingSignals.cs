using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;


namespace FunkySystem.Signals
{
    public static class MappingFactory
    {
        /// <summary>
        /// Baut ein MappingSignal1D aus einer String-Tabelle:
        /// - Erste Zeile = Header
        /// - Erste Spalte = X (z. B. Temp)
        /// - yColumnName = Name der Y-Spalte (z. B. "Umin")
        /// </summary>
        public static MappingSignal1D CreateMappingSignalFromTable(
            string name,
            string[][] table,
            string yColumnName,
            string unitY,
            string text = null,
            bool register = true)
        {
            if (table == null || table.Length < 2)
                throw new ArgumentException("Tabelle muss mindestens Header + 1 Datenzeile enthalten.", nameof(table));

            var header = table[0];
            if (header.Length < 2)
                throw new ArgumentException("Tabelle braucht mindestens eine X- und eine Y-Spalte.", nameof(table));

            int xColIndex = 0; // erste Spalte = X (Temp)
            int yColIndex = Array.IndexOf(header, yColumnName);
            if (yColIndex < 0)
                throw new ArgumentException($"Spalte '{yColumnName}' nicht im Header gefunden.", nameof(yColumnName));

            string unitX = DetectUnitFromSamples(table, xColIndex);

            var xList = new List<double>();
            var yList = new List<double>();

            for (int r = 1; r < table.Length; r++)
            {
                var row = table[r];
                if (row.Length <= Math.Max(xColIndex, yColIndex))
                    continue;

                string sx = row[xColIndex];
                string sy = row[yColIndex];

                if (string.IsNullOrWhiteSpace(sx) || string.IsNullOrWhiteSpace(sy))
                    continue;

                double x = ParseDoubleWithUnit(sx);
                double y = ParseDouble(sy);

                xList.Add(x);
                yList.Add(y);
            }

            if (xList.Count < 2)
                throw new InvalidOperationException("Es wurden weniger als 2 gültige Datenpunkte gefunden.");

            var map = new Mapping1D(xList.ToArray(), yList.ToArray());
            return new MappingSignal1D(
                name: name,
                map: map,
                unitX: unitX,
                unitY: unitY,
                text: text ?? name,
                register: register);
        }


        /// <summary>
        /// Baut ein MappingSignal2D aus einer String-Tabelle im Format
        /// ["T\\SOC", "0%", "5%", ...]
        /// ["-30°C", "-/-",  "215,9", ...]
        /// usw.
        /// </summary>
        public static MappingSignal2D CreateMapping2DFromTable(
            string name,
            string[][] table,
            string unitZ,
            string text = null,
            bool register = true)
        {
            if (table == null || table.Length < 3)
                throw new ArgumentException("Tabelle muss Header + mindestens 2 Datenzeilen enthalten.", nameof(table));

            var header = table[0];
            if (header.Length < 3)
                throw new ArgumentException("Header braucht mindestens eine X-Achse und 2 Spalten.", nameof(table));

            int xCount = header.Length - 1; // Spalten 1..N = SOC
            int yCount = table.Length - 1;  // Zeilen 1..M = Temp

            double[] xAxis = new double[xCount];
            double[] yAxis = new double[yCount];
            double[,] zValues = new double[yCount, xCount];

            // X-Achse (SOC) aus dem Header parsen
            for (int c = 1; c < header.Length; c++)
            {
                string label = header[c];
                xAxis[c - 1] = ParseDoubleWithUnit(label);
            }

            // Y-Achse (Temp) + Wertefeld parsen
            for (int r = 1; r < table.Length; r++)
            {
                var row = table[r];
                if (row.Length == 0)
                    continue;

                string tempStr = row[0];
                yAxis[r - 1] = ParseDoubleWithUnit(tempStr);

                for (int c = 1; c < header.Length; c++)
                {
                    string cell = c < row.Length ? row[c] : null;

                    if (string.IsNullOrWhiteSpace(cell) || cell == "-/-")
                    {
                        zValues[r - 1, c - 1] = double.NaN;
                    }
                    else
                    {
                        zValues[r - 1, c - 1] = ParseDouble(cell);
                    }
                }
            }

            string unitX = "%"; // aus "0%" etc.
            string unitY = DetectUnitFromSamples(table, 0); // z. B. "°C" aus "-30°C"

            var map = new Mapping2D(xAxis, yAxis, zValues);

            return new MappingSignal2D(
                name: name,
                map: map,
                unitX: unitX,
                unitY: unitY,
                unitZ: unitZ,
                text: text ?? name,
                register: register);
        }


        /// <summary>
        /// Parst Werte wie "-30°C" → -30.
        /// Entfernt alle Nicht-Ziffern außer Vorzeichen, Punkt, Komma.
        /// </summary>
        private static double ParseDoubleWithUnit(string s)
        {
            s = s?.Trim() ?? "";
            var chars = s.Where(c =>
                char.IsDigit(c) ||
                c == '-' || c == '+' ||
                c == ',' || c == '.').ToArray();

            var numStr = new string(chars).Replace(',', '.');
            if (string.IsNullOrWhiteSpace(numStr))
                throw new FormatException($"Konnte keine Zahl aus '{s}' extrahieren.");

            return double.Parse(numStr, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Parst „normale“ Zahlen mit Komma oder Punkt als Dezimaltrenner.
        /// </summary>
        private static double ParseDouble(string s)
        {
            s = (s ?? "").Trim().Replace(',', '.');
            return double.Parse(s, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Versucht aus der ersten Datenzeile die Einheit abzuleiten.
        /// Beispiel: "-30°C" → "°C".
        /// </summary>
        private static string DetectUnitFromSamples(string[][] table, int colIndex)
        {
            if (table.Length < 2)
                return "";

            string sample = table[1][colIndex]?.Trim() ?? "";
            if (sample.Length == 0)
                return "";

            var numericChars = sample.Where(c =>
                char.IsDigit(c) ||
                c == '-' || c == '+' ||
                c == ',' || c == '.').ToArray();

            var numericPart = new string(numericChars);
            string unit = sample.Replace(numericPart, "").Trim();

            return unit;
        }
    }

    public sealed class Mapping1D
    {
        public double[] X { get; }
        public double[] Y { get; }

        public Mapping1D(double[] x, double[] y)
        {
            if (x == null) throw new ArgumentNullException(nameof(x));
            if (y == null) throw new ArgumentNullException(nameof(y));
            if (x.Length != y.Length) throw new ArgumentException("X/Y length mismatch");
            if (x.Length < 2) throw new ArgumentException("At least 2 points required");

            // Hier keine Sortierung, falls du die Punkte bewusst schon sortiert lieferst.
            X = (double[])x.Clone();
            Y = (double[])y.Clone();
        }

        public double Interpolate(double x)
        {
            if (double.IsNaN(x)) return double.NaN;

            if (x <= X[0]) return Y[0];
            if (x >= X[^1]) return Y[^1];

            // Segment suchen
            int hi = Array.BinarySearch(X, x);
            if (hi >= 0)
                return Y[hi];

            hi = ~hi;
            int lo = hi - 1;

            double x0 = X[lo];
            double x1 = X[hi];
            double y0 = Y[lo];
            double y1 = Y[hi];

            double t = (x - x0) / (x1 - x0);
            return y0 + t * (y1 - y0);
        }




    }
    public sealed class Mapping2D
    {
        // X-Achse (z. B. SOC)
        public double[] X { get; }

        // Y-Achse (z. B. Temperatur)
        public double[] Y { get; }

        // Wertefeld [yIndex, xIndex]
        public double[,] Z { get; }

        public Mapping2D(double[] x, double[] y, double[,] z)
        {
            if (x == null) throw new ArgumentNullException(nameof(x));
            if (y == null) throw new ArgumentNullException(nameof(y));
            if (z == null) throw new ArgumentNullException(nameof(z));

            if (z.GetLength(0) != y.Length || z.GetLength(1) != x.Length)
                throw new ArgumentException("Dimensionen von Z passen nicht zu X/Y.");

            if (x.Length < 2 || y.Length < 2)
                throw new ArgumentException("Mindestens 2 Stützpunkte pro Achse erforderlich.");

            X = (double[])x.Clone();
            Y = (double[])y.Clone();
            Z = (double[,])z.Clone();
        }

        /// <summary>
        /// Bilineare Interpolation. Außerhalb der Achsen wird geklemmt.
        /// NaN in den Eckpunkten führt aktuell zu NaN im Ergebnis.
        /// </summary>
        public double Interpolate(double x, double y)
        {
            if (double.IsNaN(x) || double.IsNaN(y))
                return double.NaN;

            // Auf Bereich klemmen
            x = Math.Min(Math.Max(x, X[0]), X[^1]);
            y = Math.Min(Math.Max(y, Y[0]), Y[^1]);

            // Indizes für X
            int hiX = Array.BinarySearch(X, x);
            int loX;
            if (hiX >= 0)
            {
                if (hiX == 0)
                {
                    loX = hiX;
                }
                else if (hiX == X.Length - 1)
                {
                    loX = hiX - 1;
                }
                else
                {
                    loX = hiX;
                    hiX = hiX + 1;
                }
            }
            else
            {
                hiX = ~hiX;
                loX = hiX - 1;
            }

            // Indizes für Y
            int hiY = Array.BinarySearch(Y, y);
            int loY;
            if (hiY >= 0)
            {
                if (hiY == 0)
                {
                    loY = hiY;
                }
                else if (hiY == Y.Length - 1)
                {
                    loY = hiY - 1;
                }
                else
                {
                    loY = hiY;
                    hiY = hiY + 1;
                }
            }
            else
            {
                hiY = ~hiY;
                loY = hiY - 1;
            }

            double x0 = X[loX];
            double x1 = X[hiX];
            double y0 = Y[loY];
            double y1 = Y[hiY];

            double q11 = Z[loY, loX];
            double q21 = Z[loY, hiX];
            double q12 = Z[hiY, loX];
            double q22 = Z[hiY, hiX];

            if (double.IsNaN(q11) || double.IsNaN(q21) || double.IsNaN(q12) || double.IsNaN(q22))
                return double.NaN;

            if (x1 == x0 && y1 == y0)
                return q11;

            if (x1 == x0)
            {
                double ty = (y - y0) / (y1 - y0);
                return q11 + ty * (q12 - q11);
            }

            if (y1 == y0)
            {
                double tx = (x - x0) / (x1 - x0);
                return q11 + tx * (q21 - q11);
            }

            double tx2 = (x - x0) / (x1 - x0);
            double ty2 = (y - y0) / (y1 - y0);

            double r1 = q11 + tx2 * (q21 - q11); // untere Kante
            double r2 = q12 + tx2 * (q22 - q12); // obere Kante

            return r1 + ty2 * (r2 - r1);
        }
    }


    public class MappingSignal1D : Signal
    {
        private readonly Mapping1D _map;

        /// <summary>
        /// Eingang für den X-Wert (z.B. Temperatur).
        /// </summary>
        public Signal Set { get; }

        public string UnitX
        {
            get => GetProperty("unitX", "");
            set => SetProperty("unitX", value);
        }

        public string UnitY
        {
            get => Unit;          // Alias auf Unit vom Basis-Signal
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

            UnitX = unitX;
            UnitY = unitY;
            LabelX = "X";
            LabelY = "Y";

            // X-Eingangssignal anlegen, z.B. "CutOff.Umin.set"
            Set = new Signal(
                name + ".set",
                (text ?? name) + " Set",
                unitX,
                "0.000",
                double.NaN,
                register: register,
                writeBack: false);

            // WICHTIG: auf Änderungen von Set reagieren
            Set.ValueChanged += (_, __) => Recalculate();

            // Einmal initial berechnen (falls Set schon einen sinnvollen Wert hat)
            Recalculate();
        }

        public void BindSetTo(BaseSignalCommon source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            // Initialwert übernehmen
            TryUpdateFromSource(source);

            // Bei jeder Änderung nachziehen
            source.ValueChanged += (_, __) => TryUpdateFromSource(source);
        }

        private void TryUpdateFromSource(BaseSignalCommon source)
        {
            try
            {
                // Nur wenn es sinnvoll in double konvertierbar ist
                double x = Convert.ToDouble(source.ValueAsObject);
                Set.Value = x;   // triggert intern Recalculate() → Value wird neu interpoliert
            }
            catch
            {
                // optional: Logging
            }
        }

        private void Recalculate()
        {
            var x = Set.Value;
            // hier NICHT direkt UpdateStorage aufrufen, sondern über Value / DoubleValue gehen
            Value = _map.Interpolate(x);
        }

        /// <summary>
        /// Value gibt immer y = f(Set.Value) zurück.
        /// Schreiben wird intern auf den Basis-Wert gemappt, damit generischer Code
        /// (z.B. Konstruktor von Signal) weiterhin funktioniert.
        /// </summary>
        public override double Value
        {
            get => _map.Interpolate(Set.Value);
            set => DoubleValue = value; // schreibt _value und ruft UpdateStorage("Code") auf
        }

        public override string Type => "Mapping1D";

        /// <summary>
        /// Direkte Auswertung, ohne Set.Value zu ändern.
        /// </summary>
        public double Eval(double x) => _map.Interpolate(x);
    }

    /// 2D-Mapping-Signal mit zwei Eingängen (X, Y).
    /// Beispiel: DCIR(T, SOC).
    /// Nutzung:
    ///   dcir.X.Value = soc;
    ///   dcir.Y.Value = temp;
    ///   var z = dcir.Value; // mOhm
    /// </summary>
    public class MappingSignal2D : Signal
    {
        private readonly Mapping2D _map;

        public Signal X { get; }
        public Signal Y { get; }

        public string UnitX
        {
            get => GetProperty("unitX", "");
            set => SetProperty("unitX", value);
        }

        public string UnitY
        {
            get => GetProperty("unitY", "");
            set => SetProperty("unitY", value);
        }

        public string UnitZ
        {
            get => Unit;
            set => Unit = value;
        }

        public MappingSignal2D(
            string name,
            Mapping2D map,
            string unitX = "",
            string unitY = "",
            string unitZ = "",
            string text = null,
            bool register = true)
            : base(name, text ?? name, unitZ, "0.000", double.NaN, register, writeBack: false)
        {
            _map = map ?? throw new ArgumentNullException(nameof(map));

            UnitX = unitX;
            UnitY = unitY;
            UnitZ = unitZ;

            X = new Signal(name + ".x", (text ?? name) + " X", unitX, "0.000", double.NaN, register: register, writeBack: false);
            Y = new Signal(name + ".y", (text ?? name) + " Y", unitY, "0.000", double.NaN, register: register, writeBack: false);

            X.ValueChanged += (_, __) => Recalculate();
            Y.ValueChanged += (_, __) => Recalculate();

            Recalculate();
        }

        private void Recalculate()
        {
            Value = _map.Interpolate(X.Value, Y.Value);
        }

        public override double Value
        {
            get => _map.Interpolate(X.Value, Y.Value);
            set => DoubleValue = value; // löst UpdateStorage("Code") aus
        }

        public override string Type => "Mapping2D";

        public double Eval(double x, double y) => _map.Interpolate(x, y);

        // Optional: Bind-Helfer für X/Y an andere Signale
        public void BindXTo(BaseSignalCommon source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            UpdateFromSource(source, isX: true);
            source.ValueChanged += (_, __) => UpdateFromSource(source, isX: true);
        }

        public void BindYTo(BaseSignalCommon source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            UpdateFromSource(source, isX: false);
            source.ValueChanged += (_, __) => UpdateFromSource(source, isX: false);
        }

        private void UpdateFromSource(BaseSignalCommon source, bool isX)
        {
            try
            {
                double v = Convert.ToDouble(source.ValueAsObject, CultureInfo.InvariantCulture);
                if (isX)
                    X.Value = v;
                else
                    Y.Value = v;
            }
            catch
            {
                // Ignorieren oder Logging
            }
        }
    }

}
