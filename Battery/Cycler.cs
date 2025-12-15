using FunkySystem.Devices;
using FunkySystem.Core;
using FunkySystem.Helpers;
using FunkySystem.Signals;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunkySystem.BatteryCharger
{
    internal class Cycler
    {
        public string Uri;
        public string DeviceType = "BatteryCycler";
        public string ProcedureName = "none";
        public int Id = 0;
        public string Name;
        public string Text;
        public StringSignal Status { get; set; }
        public string CanIdU = "m110";
        public string CanIdI = "m111";

        public Signal Imax { get; set; }
        public Module U { get; set; }
        public Module I { get; set; }
        //public Signal E { get; set; }
        //public Signal C { get; set; }

        public bool InUse = false;

        public bool DemoMode = false;

        public Cycler(string name, string uri = "127.0.0.1:9001", bool demo = false, string canIdU = "m110", string canIdI = "m111", int id = 0)
        {
            this.Uri = uri;
            this.CanIdU = canIdU;
            this.CanIdI = canIdI;
            this.Id = SignalPool.GetNextId;
            DemoMode = demo;

            Name = name;

            string poolName = $"{Id}.{DeviceType}.{name}";
            Status = new StringSignal(name: poolName + ".Status", text: $"{Name} Status", value: "Init");
            //C = new Signal(name: poolName + ".C", text: $"{Name} Capacity", unit: "Ah", format: "0.00", value: 0.0);
            //E = new Signal(name: poolName + ".E", text: $"{Name} Energy", unit: "Wh", format: "0.00", value: 0.0);
            U = new Module(name: poolName + ".U", text: $"{Name} Voltage", unit: "V", format: "0.00");
            U.Value = 3.5;
            Imax = new Signal(name: poolName + ".Imax", text: "Max Charge Current", unit: "A", format: "0.00", value: 1.0);

            if (demo)
            {
                I = new DemoModule(name: poolName + ".I", text: $"{Name} Current", unit: "A");
               
                I.Set.Value = 0;
            }
            else
            {
                I = new Module(name: poolName + ".I", text: $"{Name} Current", unit: "A", format: "0.00"); 
            }
        }
    }
}
