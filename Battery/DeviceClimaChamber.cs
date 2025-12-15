using FunkySystem.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using FunkySystem.Core;
using FunkySystem.Controls;

namespace FunkySystem.Battery
{
    public class DeviceClimaChamber : FunkyDevice
    {

        public DemoModule Temperature;

       
        public PIDController ControllerTemperature;

        public PredictivePIDController ControllerPredictiveTemperature;

        public DeviceClimaChamber(string name) : base(name)
        {
            Id = SignalPool.GetNextId;

            string poolName = $"{Id}.ClimaChamber.{name}";
            Temperature = new DemoModule(name: poolName + ".TC", text: $"{Name} Temperature", unit: "°C");
            Temperature.Set.Value = 25.0;
            Temperature.Value = 25.0;
            Temperature.NoiseStrength = 0.1;
            Temperature.TauValue = 0.25;

            //ControllerTemperature = new PIDController(
            //    name: poolName + ".ControllerTemperature",
            //    text: $"{Name} PID Temperature",
            //    source: Temperature,
            //    outSignal: Temperature.Set,
            //    computeInterval: 10,
            //    outputInterval: 100,
            //    ks: 3,
            //    tu: 0.2,
            //    tg: 1,
            //    outMin: 0,
            //    outMax: 100,
            //    dFilterTauMs: 1000);
            //ControllerTemperature.Set.Value = 25;
            //   ControllerTemperature.Start();

            ControllerPredictiveTemperature = new PredictivePIDController(
                name: poolName + ".PPID",

                source: Temperature,
                outSignal: Temperature.Set,
                computeInterval: 10,
                outputInterval: 100,
                ks: 20,
                tu: 0.2,
                tg: 1,
                lookAheadMs: 200,
                lookBackMs: 500,
                outMin: 0,
                outMax: 100,
                dFilterTauMs: 1000);

            ControllerPredictiveTemperature.Set.Value = 25;
            //ControllerPredictiveTemperature.Start();

        }

    }
}
