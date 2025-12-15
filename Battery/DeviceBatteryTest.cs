using FunkySystem.BatteryCharger;
using FunkySystem.Controls;
using FunkySystem.Core;
using FunkySystem.Helpers;
using FunkySystem.Net.CAN;
using FunkySystem.Signals;
using MQTTnet.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunkySystem.Battery
{
    public class DeviceBatteryTest : FunkyDevice
    {
        public Signal demo = new Signal(name: "Demo Signal", text: "Demo Signal", unit: "V", format: "0.00", value: 2.5);
        public Module demoM = new DemoModule("Test", "Demo", "V");
        public BoolSignal boolSignal = new BoolSignal(name: "Bool Signal", text: "Bool Signal");
        public StringSignal stringSignal = new StringSignal(name: "String Signal", text: "String Signal", value: "Hello World");
        public SignalSimulator d1 = new SignalSimulator();
        public FormulaSignal calc;

        internal Dictionary<string, Cycler> Cyclers = new();

        public UdlClient udlClient;

        public BatteryData Battery;


        public Signal Temperature;

        public Signal DO1_4;
        public BitSignal DO1;
        public BitSignal DO2;
        public BitSignal DO3;
        public BitSignal DO4;

        public FormulaSignal U;
        public FormulaSignal I;
        public FormulaSignal E;
        public FormulaSignal C;
        public FormulaSignal P;
        public FormulaSignal SOC;

        public ModuleController Icontrol;


        //public MappingSignal1D CutOff;
        //public MappingSignal2D DcIr;

        public DeviceBatteryTest(string name) : base (name)
        {
            int id = SignalPool.GetNextId;

            d1.OnNewValue += (v) => demo.Value = v;
            calc = new FormulaSignal(name: "calc", triggeredBySignals: new[] { demo }, formula: async (f) => demoM.Value * demo.Value);
            udlClient = new UdlClient(name: "BatteryTestClient");
            udlClient.Open("127.0.0.1",9001);
    
            Battery = new BatteryData($"{id}.{name}.Battery");

            U = new FormulaSignal(name: $"{id}.{name}.U", triggeredByInterval: 50, formula: async (f) => await calcU(), unit: "V",text:"U");
            I = new FormulaSignal(name: $"{id}.{name}.I", triggeredByInterval: 50, formula: async (f) => await calcI(), unit: "A",text:"I");
            P = new FormulaSignal(name: $"{id}.{name}.P", triggeredBySignals: new[] { U, I }, formula: async (f) => U.Value * I.Value, unit: "W", text: "P");
            E = new FormulaSignal(name: $"{id}.{name}.E", triggeredByInterval:100, formula: async (f) => await calcE(), unit: "Wh", text: "E"); 
            C = new FormulaSignal(name: $"{id}.{name}.C", triggeredByInterval: 100, formula: async (f) => await calcC(), unit: "Ah", text: "C");
            SOC = new FormulaSignal(name: $"{id}.{name}.SOC", triggeredBySignals: new[] { C }, formula: async (f) => await calcSOC(), unit: "%", text: "SOC");
  

            Temperature = SignalPool.GetSignal("10.ClimaChamber.Oven.TC", format:"0.00", text:"Temperature", unit: "°C");
            DO1_4 = SignalPool.GetModule($"{udlClient.Name}.0x310");
            DO1 = SignalPool.GetBit($"{udlClient.Name}.0x310", 0, text: "DO1");
            DO2 = SignalPool.GetBit($"{udlClient.Name}.0x310", 1, text: "DO2");
            DO3 = SignalPool.GetBit($"{udlClient.Name}.0x310", 2, text: "DO3");
            DO4 = SignalPool.GetBit($"{udlClient.Name}.0x310", 3, text: "DO4");

           
           

            Icontrol = new ModuleController(name: $"{id}.{name}.Icontrol", source: I, text: "Current Control", unit: "A");
            Icontrol.Set.ValueChanged += (s, e) => SetI(Icontrol.Set.Value);

        }

        public override void Start()
        {
            U.Start();
            I.Start();
            P.Start();
            ResetStats();
            base.Start();
        }


        void SetI(double set)
        {
            int cyclersCount = Cyclers.Count;
            double perCycler = set / cyclersCount;
            foreach (Cycler c in Cyclers.Values)
            {
                c.I.Set.Value = perCycler;
            }
        }

        public void ResetStats()
        {
            E.Stop();
            C.Stop();
            SOC.Stop();
            E.Value = 0;
            C.Value = 0;
            _lastUpdateE = DateTime.Now;
            E.Start();
            C.Start();
            SOC.Start();
        }


        // Felder
        private DateTime _lastUpdateE = DateTime.UtcNow;
        private DateTime _lastUpdateC = DateTime.UtcNow;

        // Optional: Grenzen
        public double? Emax { get; set; }  // z.B. Nennkapazität * Nennspannung in Wh
        public double? Cmax { get; set; }  // z.B. Nennkapazität in Ah

        async Task<double> calcE()
        {
            // Annahme: P.Value in Watt, E.Value in Wh
            double Eold = E.Value;
            double Pnow = P.Value;

            if (double.IsNaN(Pnow) || double.IsInfinity(Pnow))
                return Eold;

            DateTime now = DateTime.UtcNow;
            TimeSpan dt = now - _lastUpdateE;

            if (dt <= TimeSpan.Zero)
            {
                _lastUpdateE = now;
                return Eold;
            }

            double hours = dt.TotalHours;
            double Enew = Eold + Pnow * hours; // korrekte Integration: W * h = Wh

            // Clamping (falls Grenzen gesetzt)
            if (Emax.HasValue)
                Enew = Math.Clamp(Enew, 0.0, Emax.Value);
            else
                Enew = Math.Max(0.0, Enew);

            _lastUpdateE = now;
            return Enew;
        }

        async Task<double> calcC()
        {
            // Annahme: I.Value in Ampere, C.Value in Ah
            double Cold = C.Value;
            double Inow = I.Value; // <-- Strom ist maßgeblich

            if (double.IsNaN(Inow) || double.IsInfinity(Inow))
                return Cold;

            DateTime now = DateTime.UtcNow;
            TimeSpan dt = now - _lastUpdateC;

            if (dt <= TimeSpan.Zero)
            {
                _lastUpdateC = now;
                return Cold;
            }

            double hours = dt.TotalHours;
            double Cnew = Cold + Inow * hours; // korrekte Integration: A * h = Ah

            // Clamping
            if (Cmax.HasValue)
                Cnew = Math.Clamp(Cnew, 0.0, Cmax.Value);
            else
                Cnew = Math.Max(0.0, Cnew);

            _lastUpdateC = now;
            return Cnew;
        }


        internal async Task<double> calcSOC()
        {
            if (Battery.Main.Capacity.Value == null || Battery.Main.Capacity.Value <= 0)
                return double.NaN;

            double capacity = Battery.Main.Capacity.Value;
            if (capacity <= 0)
                return 0.0;
            return (C.Value / capacity) * 100.0;
        }

        internal async Task<double> calcU()
        {
            if (Cyclers.First().Value.DemoMode)
            {
                return BatteryVoltageModel.VoltageFromCapacity(C.Value, I.Value, Temperature.Value,Battery).Result;
            }


            double sum = 0;
            foreach (Cycler c in Cyclers.Values)
                sum += c.U.Value;
            return sum / Cyclers.Count;
        }

        internal async Task<double> calcI()
        {
            double sum = 0;
            foreach (Cycler c in Cyclers.Values)
                sum += c.I.Value;

            return sum;
        }

        

    }
}
