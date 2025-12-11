using FunkySystem.BatteryCharger;
using FunkySystem.Controls;
using ScottPlot.Colormaps;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace FunkySystem.Battery
{
    public class ControlBatteryTest : FunkyDeviceControl
    {
        DeviceBatteryTest Test;

        internal RadioButtonEx ExplorerButton;

        public ButtonWithIcon StateControl => btnState;

        public ControlBatteryTest(DeviceBatteryTest batteryTest)
        {
            Test = batteryTest;
            PanelDut.MenuClicked += PanelDut_MenuClicked;
            Test.Battery.Main.Serielnumber.OnUpdate = () => UpdateButton();
         
        }

        void UpdateButton()
        {
            if (ExplorerButton == null) return;
            ExplorerButton.DisplayText = $"{Test.Name}\r\n{Test.Battery.Main.Serielnumber.Value}";
            ExplorerButton.Invalidate();
        }

        private void PanelDut_MenuClicked(object? sender, EventArgs e)
        {
            Debug.WriteLine(Test.Battery.Main.Serielnumber.Value);
           // return;
            using (var dlg = new FormBatteryData(Test.Battery))
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                { 
                    ExplorerButton.DisplayText = $"{Test.Name}\r\n{Test.Battery.Main.Serielnumber.Value}";
                    ExplorerButton.Invalidate();
                }
            }
        }


        public void UpdateControls()
        {

            DisplayData.ContentBackColor = DisplayData.DefaultBackColor;
            DisplayData.ContentForeColor = Color.Black;
            DisplayData.ShowMenuIcon = false;

            //DisplayData.Add(new[] { Test.CutOff, Test.CutOff.Set }, primary: true);
            //DisplayData.Add(new[] { Test.DcIr, Test.DcIr.X, Test.DcIr.Y }, primary: true);

            DisplayData.Add(new[] { Test.U, Test.I }, primary: true);
            DisplayData.Add(new[] { Test.P, Test.E }, primary: true);
            DisplayData.Add(new[] { Test.C, Test.SOC }, primary: true);
            //Add(new[] { sequence.BatteryTest.P, sequence.BatteryTest.E }, primary: true);
            DisplayData.Add(new[] { Test.Icontrol.Set }, primary: true);
            DisplayData.Add(new object[] { "<space>" }, primary: false);
            DisplayData.Add(new[] { Test.Temperature }, primary: true);

            foreach (Cycler c in Test.Cyclers.Values)
            {
                object? name = c.Name;
                DisplayData.Add(new[] { name, c.U, c.I }, primary: false);
            }



            DisplayData.UpdateDisplay();


            Recorder.Add(Test.U, 1);
            Recorder.Add(Test.I, 2);
            Recorder.Add(Test.C, 3);
            //Recorder.Add(Test.E, 4);

            PanelDut.ContentBackColor = PanelDut.BackColor;
            PanelDut.Add(new object[] { Test.Battery.Main.Serielnumber, Test.Battery.Main.Partnumber }, true);
            PanelDut.Add(new object[] { Test.Battery.Main.CellType, Test.Battery.Main.SetTemperature }, false);

            PanelDut.Add(new object[] { Test.Battery.Main.CutoffDischarge, Test.Battery.Main.CutoffCharge }, false);
            PanelDut.Add(new object[] { Test.Battery.Main.LowerOvershoot, Test.Battery.Main.UpperOvershoot }, false);
            PanelDut.UpdateDisplay();

            InitLog(Test);


            if (Test.Battery.Main.Capacity != null) UpdateChart();

        }


        void UpdateChart()
        {
            Debug.WriteLine("Update Chart Battery Test Control");
            Debug.WriteLine($"CutoffDischarge: {Test.Battery.Main.CutoffDischarge.Value}");
            Debug.WriteLine($"CutoffCharge: {Test.Battery.Main.CutoffCharge.Value}");
            Debug.WriteLine($"Capacity: {Test.Battery.Main.Capacity.Value}");

            Recorder.SetY1(false,Test.Battery.Main.CutoffDischarge.Value - 0.05, Test.Battery.Main.CutoffCharge.Value + 0.05, "Y1 Voltage [U]");
            Recorder.SetY2(true, label:"Y2 Current [I]");
            Recorder.SetY3(false, -1, Test.Battery.Main.Capacity.Value + 1,  label: "Y3 Capacity [Ah]");
        }
       


        private void Serielnumber_ValueChanged(object? sender, EventArgs e)
        {
            ExplorerButton.DisplayText = $"{Test.Name}\r\n{Test.Battery.Main.Serielnumber.Value}";
            throw new NotImplementedException();
        }

        private void BtnConfig_LeftClicked(object? sender, EventArgs e)
        {
           Test.Log.Info($"Config button clicked {Test.Name}");
            if (Test.State == Core.State.Off)
                Test.State = Core.State.Alert;
            else
                Test.State = Core.State.Off;
        }
    }
}
