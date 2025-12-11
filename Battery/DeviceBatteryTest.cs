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


//            string[][] cutOffTable = new[]
//{
//            new[] { "Temp","Umin","Umax","UminPulse","UmaxPulse" },
//            new[] { "-30°C","-30","4,25","2","4,25" },
//            new[] { "-25°C","-25","4,25","2","4,25" },
//            new[] { "-20°C","-20","4,25","2,5","4,25" },
//            new[] { "-10°C","-10","4,25","2,5","4,25" },
//            new[] { "0°C","0","4,25","2,5","4,25" },
//            new[] { "10°C","10","4,25","2,5","4,25" },
//            new[] { "25°C","25","4,25","2,5","4,25" },
//            new[] { "30°C","30","4,25","2,5","4,25" },
//            new[] { "45°C","45","4,25","2,5","4,25" },
//            new[] { "50°C","50","4,25","2,5","4,25" },
//            new[] { "60°C","60","4,25","2,5","4,25" }
//    };

            //CutOff = MappingFactory.CreateMappingSignalFromTable(
            //    name: $"{id}.{name}.CutOff.Umin",
            //    table: cutOffTable,
            //    yColumnName: "Umin",
            //    unitY: "V",
            //    text: "CutOff Umin vs. Temp",
            //    register: true);
            //    CutOff.BindSetTo(Temperature);


            //string[][] dcirTable = new[]
            //{
            //    new[] { "T\\SOC","0%","5%","10%","15%","20%","35%","50%","65%","80%","95%","100%" },
            //    new[] { "-30°C","-/-","-/-","-/-","-/-","-/-","-/-","-/-","-/-","-/-","-/-","-/-" },
            //    new[] { "-25°C","-/-","215,9","159,3","102,7","46,1","25,8","5,8","5,7","5,6","5,4","5,4" },
            //    new[] { "-20°C","-/-","193,7","116,5","76,5","35,5","20,3","4,7","4,6","4,4","4,6","4,6" },
            //    new[] { "-10°C","-/-","128,2","56,9","32,6","6,8","4,6","2,2","2,1","2,2","2,2","2,2" },
            //    new[] { "0°C","-/-","53,5","36,7","19,8","3,0","2,4","1,5","1,5","1,5","1,5","1,5" },
            //    new[] { "10°C","-/-","35,8","22,0","12,3","2,4","1,9","1,2","1,3","1,3","1,3","1,3" },
            //    new[] { "25°C","-/-","3,0","2,3","1,7","1,0","0,9","0,7","0,8","0,7","0,8","0,8" },
            //    new[] { "40°C","-/-","2,2","1,8","1,4","0,9","0,8","0,7","0,7","0,6","0,7","0,7" },
            //    new[] { "50°C","-/-","1,0","0,9","0,8","0,6","0,6","0,6","0,6","0,6","0,6","0,6" },
            //    new[] { "55°C","-/-","1,0","0,9","0,8","0,6","0,6","0,6","0,6","0,6","0,6","0,6" },
            //    new[] { "60°C","-/-","-/-","-/-","-/-","-/-","-/-","-/-","-/-","-/-","-/-","-/-" }
            //};

            //DcIr = MappingFactory.CreateMapping2DFromTable(
            //name: "DCIR.2SecDisch",
            //table: dcirTable,
            //unitZ: "mOhm",
            //text: "2s Discharge DCIR");
            //DcIr.BindYTo(Temperature);
            //DcIr.BindXTo(SOC);





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
