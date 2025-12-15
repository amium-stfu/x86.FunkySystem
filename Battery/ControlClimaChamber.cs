using System;
using System.Collections.Generic;
using System.Text;
using FunkySystem.Controls;


namespace FunkySystem.Battery
{
    public class ControlClimaChamber : FunkyDeviceControl
    {

        public DeviceClimaChamber Device;

        PIDController tc;

        ButtonSimple btnteach;
        ButtonSimple btnStart;

        public ControlClimaChamber(DeviceClimaChamber device)
        {
            HideStatePanel();
            HideControlPanel();
            HideSequencePanel();

            btnteach = new ButtonSimple();
            btnteach.SignalValue = "Start Controller";

            btnteach.Click += (s,e) => { Device.ControllerPredictiveTemperature.Start(); };

            btnStart = new ButtonSimple();
            btnStart.SignalValue = "Stop Controller";
            btnStart.Click += (s, e) => { Device.ControllerPredictiveTemperature.Stop(); };


            Device = device;

            DisplayData.Add(new object[] { Device.Temperature }, primary: true);
            DisplayData.Add(new object[] {btnteach,btnStart }, primary: true);
            DisplayData.Add(new object[] { Device.ControllerPredictiveTemperature }, primary: true);
            DisplayData.Add(new object[] {
                Device.ControllerPredictiveTemperature.PID.Ks,
                Device.ControllerPredictiveTemperature.PID.Tu,
                Device.ControllerPredictiveTemperature.PID.Tg }, primary: true);

            // DisplayData.Add(new object[] { Device.ControllerPredictiveTemperature }, primary: true);

            // DisplayData.Add(new object[] { Device.ControllerPredictiveTemperature.PID ,"<space>"}, primary: true);
            Recorder.Add(Device.Temperature, 1);
           // Recorder.Add(Device.ControllerPredictiveTemperature.PredictedValue, 1);
            DisplayData.UpdateDisplay();

            Recorder.SetY1(false, 0, 100, "Y1 Temperature [°C]");


        }

        

    }
}
