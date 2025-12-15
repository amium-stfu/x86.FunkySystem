using FunkySystem.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FunkySystem.BatteryCharger;
using System.Diagnostics;
using FunkySystem.Signals;
using FunkySystem.Core;

namespace FunkySystem.Devices
{
    public partial class FormAddTest : Form
    {

        BatteryData Battery;

        StringSignal name = new StringSignal("#NewTest", "New Test", "NewTest");
        public FormAddTest()
        {
            InitializeComponent();
            UpdateCyclerList();
            Battery = new BatteryData("#AddingTest");
            Partnumber.SetSource(Battery.Main.Partnumber);
            SN.SetSource(Battery.Main.Serielnumber);
            CellType.SetSource(Battery.Main.CellType);
            Capacity.SetSource(Battery.Main.Capacity);
            LowerOvershoot.SetSource(Battery.Main.LowerOvershoot);
            UpperOvershoot.SetSource(Battery.Main.UpperOvershoot);
            SetTemperature.SetSource(Battery.Main.SetTemperature);
            SequenceName.SetSource(name);

            name.Value = "Slot" + DeviceManager.DeviceCount;
        }

        private void BoxTest_Enter(object sender, EventArgs e)
        {

        }

        void UpdateCyclerList()
        {
            panelHardwareList.Controls.Clear();
            foreach (var device in Program.ConnectedDevices)
            {
                if (device.Value is Cycler cycler)
                {
                    if(cycler.InUse)
                    {
                        continue;
                    }
                    CyclerButton btn = new CyclerButton(cycler.Name);
                    btn.LeftClicked += Btn_LeftClicked;
                    panelHardwareList.Controls.Add(btn);
                }
            }
        }

        private void Btn_LeftClicked(object? sender, EventArgs e)
        {
            if (Cyclers.Contains(((CyclerButton)sender).ButtonText))
            {
                Cyclers.Remove(((CyclerButton)sender).ButtonText);
                ((CyclerButton)sender).BackColor = Color.WhiteSmoke;
            }
            else
            {
                Cyclers.Add(((CyclerButton)sender).ButtonText);
                ((CyclerButton)sender).BackColor = Color.Lime;
            }
        }



        List<string> Cyclers = new List<string>();

        public BatterySequenceControl Sequencer;


        private void buttonWithIcon3_Click(object sender, EventArgs e)
        {
            if(DeviceManager.DeviceExists(SequenceName.SignalValue))
            {
                MessageBox.Show("A test with this name already exists. Please choose another name.");
                return;
            }
            
            Sequencer = new BatterySequenceControl(SequenceName.SignalValue, new DeviceCycler(SequenceName.SignalValue));
            Sequencer.BatteryTest.Battery.LoadFromJson("NewTest", BatterUri);
            Sequencer.BatteryTest.Cyclers = new Dictionary<string, Cycler>();

            foreach (var cyclerName in Cyclers)
            {
                var device = Program.ConnectedDevices[cyclerName];
                if (device is Cycler cycler)
                {
                    cycler.InUse = true;
                    Sequencer.BatteryTest.Cyclers.Add(cyclerName, cycler);
                }
            }
            this.DialogResult = DialogResult.OK;
            Close();
        }

        string BatterUri = "";

        private void btnLoadBattery_Click(object sender, EventArgs e)
        {
            BatterUri = Battery.Load();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }

    class CyclerButton : ButtonWithIcon
    {

        public CyclerButton(string name)
        {
            BorderColor = Color.White;
            BackColor = Color.WhiteSmoke;
            ButtonIcon = "fa:charging-station:black";
            ButtonText = name;
            GridScale = 5;
            HoverColor = Color.Transparent;
            IconSizeFactor = 0.6;

            LedVisible = false;
            Dock = DockStyle.Top;
            Name = "";
            ShortcutText = "";
            SignalValue = "Unknown";

            TabIndex = 0;
            Text = "";



        }
    }

}
