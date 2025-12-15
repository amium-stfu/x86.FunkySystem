using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FunkySystem.Devices;
using FunkySystem.Controls;
using FunkySystem.Core;

namespace FunkySystem.Devices
{
    public partial class FormBatteryData : Form
    {

        public BatteryData Battery;
        public FormBatteryData(BatteryData bat)
        {

            InitializeComponent();
            Battery = bat;
            //Partnumber.SetSource(Battery.Main.Partnumber);
            //SN.SetSource(Battery.Main.Serielnumber);
            //CellType.SetSource(Battery.Main.CellType);
            //Capacity.SetSource(Battery.Main.Capacity);
            //LowerOvershoot.SetSource(Battery.Main.LowerOvershoot);
            //UpperOvershoot.SetSource(Battery.Main.UpperOvershoot);
            //SetTemperature.SetSource(Battery.Main.SetTemperature);


            displayMain.Add(new object[] { Battery.Main.Partnumber, Battery.Main.Serielnumber, "<space>" }, primary: true);
            displayMain.Add(new object[] { Battery.Main.CellType, Battery.Main.Capacity, "<space>" }, primary: true);
            displayMain.Add(new object[] { Battery.Main.SetTemperature, "<space>", "<space>" }, primary: true);
            displayMain.Add(new object[] { Battery.Main.LowerOvershoot, Battery.Main.UpperOvershoot, "<space>" }, primary: true);
            displayMain.Add(new object[] { Battery.Main.CutoffDischarge, Battery.Main.CutoffCharge, "<space>" }, primary: true);
            displayMain.UpdateDisplay();



            displayPhysicalSpecificationsData.Add(new object[] { Battery.PhysicalSpecifications.Width, Battery.PhysicalSpecifications.Thickness,"<space>" }, primary: true);
            displayPhysicalSpecificationsData.Add(new object[] { Battery.PhysicalSpecifications.Height, Battery.PhysicalSpecifications.Weight, "<space>" }, primary: true);
            displayPhysicalSpecificationsData.Add(new object[] { Battery.PhysicalSpecifications.Density,  "<space>", "<space>",  }, primary: true);
            displayPhysicalSpecificationsData.UpdateDisplay();

            displayInner.Add(new object[] { Battery.InnerCellMaterials.Anode, Battery.InnerCellMaterials.Cathode, "<space>" }, primary: true);
            displayInner.Add(new object[] { Battery.InnerCellMaterials.Electrolyte, Battery.InnerCellMaterials.Seperator, "<space>" }, primary: true);
            displayInner.Add(new object[] { Battery.InnerCellMaterials.ElectrodeCeramicCoating, "<space>", "<space>" }, primary: true);
            displayInner.UpdateDisplay();

            displayElectric.Add(new object[] { Battery.ElectricalProperties.LowerOvershotLimitVoltage, Battery.ElectricalProperties.UpperOvershotLimitVoltage, "<space>" }, primary: true);
            displayElectric.Add(new object[] { Battery.ElectricalProperties.LowerOvershotLimitTime, Battery.ElectricalProperties.UpperOvershotLimitTime, "<space>" }, primary: true);
            displayElectric.Add(new object[] { Battery.ElectricalProperties.LowerOvershotLimitOccurences, Battery.ElectricalProperties.UpperOvershotLimitOccurences, "<space>" }, primary: true);
            displayElectric.UpdateDisplay();

            displayThermal.Add(new object[] { Battery.ThermalProperties.OperationMin, Battery.ThermalProperties.OperationMax, "<space>" }, primary: true);
            displayThermal.Add(new object[] { Battery.ThermalProperties.StorageMin, Battery.ThermalProperties.StorageMax, "<space>" }, primary: true);
            displayThermal.Add(new object[] { Battery.ThermalProperties.SafetyMin, Battery.ThermalProperties.SafetyMax, "<space>" }, primary: true);
            displayThermal.UpdateDisplay();


        }



        private void button1_Click(object sender, EventArgs e)
        {
            Battery.Load();
        }

        void Update()
        {


        }

        private void FormBatteryData_FormClosed(object sender, FormClosedEventArgs e)
        {
            SignalPool.RemoveTemporarySignals();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
