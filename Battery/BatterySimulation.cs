using FunkySystem.Core;
using FunkySystem.Helpers;
using FunkySystem.Signals;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FunkySystem.Battery
{
    /// <summary>
    /// Sehr einfaches Batteriespannungsmodell:
    /// - Spannung wird nur aus SOC (State of Charge, Ladezustand) linear zwischen Umin und Umax berechnet.
    /// - Optional wird ein ohmscher Innenwiderstand Ri (Internal Resistance, Innenwiderstand) berücksichtigt.
    /// </summary>
    public static class BatteryVoltageModel
    {
        static int id = SignalPool.GetNextId;
        public static double Cmin { get; set; } = 0.0;     // minimale Kapazität [Ah]
        public static double Cmax { get; set; } = 151.1;   // Kapazität bei voll [Ah]
        public static double Umin { get; set; } = 2.5;     // Unterspannung / Cutoff Discharge [V]
        public static double Umax { get; set; } = 4.2;     // Ladeschlussspannung / Cutoff Charge [V]

        static string[][] cutOffTable = new[]
{
            new[] { "Temp","Umin","Umax","UminPulse","UmaxPulse" },
            new[] { "-30°C","-30","4,25","2","4,25" },
            new[] { "-25°C","-25","4,25","2","4,25" },
            new[] { "-20°C","-20","4,25","2,5","4,25" },
            new[] { "-10°C","-10","4,25","2,5","4,25" },
            new[] { "0°C","0","4,25","2,5","4,25" },
            new[] { "10°C","10","4,25","2,5","4,25" },
            new[] { "25°C","25","4,25","2,5","4,25" },
            new[] { "30°C","30","4,25","2,5","4,25" },
            new[] { "45°C","45","4,25","2,5","4,25" },
            new[] { "50°C","50","4,25","2,5","4,25" },
            new[] { "60°C","60","4,25","2,5","4,25" }
    };

        static string[][] dcirTable = new[]
        {
                new[] { "T\\SOC","0%","5%","10%","15%","20%","35%","50%","65%","80%","95%","100%" },
                new[] { "-30°C","0","0","0","0","0","0","0","0","0","0","0" },
                new[] { "-25°C","0","215,9","159,3","102,7","46,1","25,8","5,8","5,7","5,6","5,4","5,4" },
                new[] { "-20°C","0","193,7","116,5","76,5","35,5","20,3","4,7","4,6","4,4","4,6","4,6" },
                new[] { "-10°C","0","128,2","56,9","32,6","6,8","4,6","2,2","2,1","2,2","2,2","2,2" },
                new[] { "0°C","0","53,5","36,7","19,8","3,0","2,4","1,5","1,5","1,5","1,5","1,5" },
                new[] { "10°C","0","35,8","22,0","12,3","2,4","1,9","1,2","1,3","1,3","1,3","1,3" },
                new[] { "25°C","0","3,0","2,3","1,7","1,0","0,9","0,7","0,8","0,7","0,8","0,8" },
                new[] { "40°C","0","2,2","1,8","1,4","0,9","0,8","0,7","0,7","0,6","0,7","0,7" },
                new[] { "50°C","0","1,0","0,9","0,8","0,6","0,6","0,6","0,6","0,6","0,6","0,6" },
                new[] { "55°C","0","1,0","0,9","0,8","0,6","0,6","0,6","0,6","0,6","0,6","0,6" },
                new[] { "60°C","0","0","0","0","0","0","0","0","0","0","0" }
            };

        //public static MappingSignal1D CutOff = MappingFactory.CreateMappingSignalFromTable(
        //        name: $"{id}.BatteryDemo.CutOff.Umin",
        //        table: cutOffTable,
        //        yColumnName: "Umin",
        //        unitY: "V",
        //        text: "CutOff Umin vs. Temp",
        //        register: true);



        public static MappingSignal2D DcIr = MappingFactory.CreateMapping2DFromTable(
            name: $"{id}.BatteryDemo.DCIR.2SecDisch",
            table: dcirTable,
            unitZ: "mOhm",
            text: "2s Discharge DCIR");


        /// <summary>
        /// Konstanter Innenwiderstand Ri (Internal Resistance, Innenwiderstand) [Ohm] als einfacher erster Schritt.
        /// </summary>
        public static double Ri { get; set; } = 0.001;

        /// <summary>
        /// SOC (State of Charge, Ladezustand) als 0..1.
        /// </summary>
        public static double Soc { get; private set; } = 0.0;

        public static BatteryData? Battery;

        /// <summary>
        /// Berechnet die Klemmenspannung aus eingeladener Kapazität.
        /// 
        /// Annahmen:
        /// - chargedAh ist die aktuell eingeladene Kapazität bezogen auf denselben Nullpunkt wie Cmin.
        /// - currentA &gt; 0 bedeutet Entladen, currentA &lt; 0 bedeutet Laden.
        /// - Einfache lineare Kennlinie zwischen Umin (SOC=0) und Umax (SOC=1).
        /// - Optionaler ohmscher Spannungsabfall über Ri.
        /// </summary>
        /// <param name="chargedAh">Aktuelle eingeladene Kapazität [Ah]</param>
        /// <param name="currentA">Momentanstrom [A], Entladung &gt; 0, Ladung &lt; 0</param>
        /// <param name="tempC">Temperatur [°C] (noch unbenutzt im einfachen Modell)</param>
        /// <returns>Berechnete Klemmenspannung [V]</returns>
        public static async Task<double> VoltageFromCapacity(double chargedAh, double currentA, double tempC, BatteryData? battery)
        {
            if(Battery == null && battery != null)
            {
                Battery = battery;
            }

            // Asynchronität wird aktuell nicht genutzt, Methode bleibt aber async
            await Task.Yield();

            if (Battery == null)
                return 0.0;

            // Grenzen aus den Batteriedaten aktualisieren
            Cmax = Battery.Main.Capacity.Value;
            Umin = Battery.Main.CutoffDischarge.Value;
            Umax = Battery.Main.CutoffCharge.Value;

            double capacitySpan = Cmax - Cmin;
            if (capacitySpan <= 0)
                return 0.0;

            // SOC (State of Charge, Ladezustand) aus Kapazität bestimmen und auf 0..1 begrenzen
            Soc = (chargedAh - Cmin) / capacitySpan;
            if (Soc < 0.0) Soc = 0.0;
            if (Soc > 1.0) Soc = 1.0;

            DcIr.X.Value = Soc;
            DcIr.Y.Value = tempC;

            Ri = DcIr.Value / 1000.0; // mOhm -> Ohm 


            // Leerlaufspannung (OCV, Open Circuit Voltage) linear zwischen Umin und Umax
            double uOcv = Umin + Soc * (Umax - Umin);


            double uDrop = currentA * Ri;
            double uLoaded = uOcv + uDrop;

            return uLoaded;
        }

        /// <summary>
        /// Initialisiert das Modell mit Batteriedaten und setzt die Kapazitätsgrenzen.
        /// </summary>
        public static void Reset(BatteryData bat)
        {
            Battery = bat;

            Cmin = 0.0;
            Cmax = Battery.Main.Capacity.Value;
            Umin = Battery.Main.CutoffDischarge.Value;
            Umax = Battery.Main.CutoffCharge.Value;

            Soc = 0.0;
        }
    }
}
