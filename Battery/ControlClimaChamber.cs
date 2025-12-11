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
            btnteach.SignalText = "Teach\nTemperature";

            btnteach.Click += (s,e) => { Device.ControllerTemperature.Teach(3, 50); };

            btnStart = new ButtonSimple();
            btnStart.SignalText = "Start\nController";
            btnStart.Click += (s, e) => { Device.ControllerTemperature.Start(); };


            Device = device;

            DisplayData.Add(new object[] { Device.Temperature }, primary: true);
            DisplayData.Add(new object[] {btnteach,btnStart }, primary: true);
            DisplayData.Add(new object[] { Device.ControllerTemperature }, primary: true);
            DisplayData.Add(new object[] { Device.ControllerTemperature.Ks, Device.ControllerTemperature.Tu, Device.ControllerTemperature.Tg }, primary: true);

            // DisplayData.Add(new object[] { Device.ControllerPredictiveTemperature }, primary: true);

            // DisplayData.Add(new object[] { Device.ControllerPredictiveTemperature.PID ,"<space>"}, primary: true);
            Recorder.Add(Device.Temperature, 1);
           // Recorder.Add(Device.ControllerPredictiveTemperature.PredictedValue, 1);
            DisplayData.UpdateDisplay();

            Recorder.SetY1(false, 0, 100, "Y1 Temperature [°C]");


        }

        

    }
}
