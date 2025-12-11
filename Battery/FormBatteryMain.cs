using FontAwesome.Sharp;
using FunkySystem.BatteryCharger;
using FunkySystem.Controls;
using FunkySystem.Core;
using FunkySystem.Signals;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Globalization;
using System.Text;
using System.Windows.Controls;

namespace FunkySystem.Battery
{
    internal class FormBatteryMain : FormMain
    {

        ControlOverview Overview = new ControlOverview();
        ControlClimaChamber ClimaChamber;
        Dictionary<BaseControl, int> TestList = new();


        public FormBatteryMain()
        {

            InitClimaChamber();
            PanelMainMenu.AddRadioButton("OverView","overview", () => ShowOverview());
            PanelMainMenu.AddRadioButton("Test Explorer", "explorer", () => ShowExplorer());
            PanelMainMenu.AddRadioButton("Temp. Control","tc", () => ShowOverTempControl());

            
            btnAddTest.LeftClicked += BtnAddTest_LeftClicked;
            ((RadioButtonEx)PanelMainMenu.Controls[0]).PerfomClicked();
        }


        void InitClimaChamber()
        {
            ClimaChamber = new ControlClimaChamber(Program.ConnectedDevices["ClimaChamber"] as DeviceClimaChamber);
        
        }


        private void BtnAddTest_LeftClicked(object? sender, EventArgs e)
        {
            BatterySequenceControl sequence = null;
            using (var dlg = new FormAddTest())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    sequence = dlg.Sequencer;
                }
            }

            if (sequence == null) return;

            sequence.DeviceControl.UpdateControls();
            AddTest(sequence);
            SignalPool.RemoveTemporarySignals();
        }


        void AddTest(BatterySequenceControl sequnce)
        {
            sequnce.DeviceControl.ExplorerButton = PanelTestList.AddRadioButton(
                 text:$"{sequnce.Name}\r\n{sequnce.BatteryTest.Battery.Main.Serielnumber.Value}",
                 name: sequnce.Name, 
                 onSelect: () => ShowTest(sequnce.DeviceControl, sequnce.DeviceControl.ExplorerButton), 
                 attechment: sequnce
                );
            sequnce.DeviceControl.ExplorerButton.ShowMenuButton = true;
            sequnce.DeviceControl.ExplorerButton.MenuClicked += (s, e) =>
            {
            using (var dlg = new FormMenu())
                {
                    Point buttonScreenPos = sequnce.DeviceControl.ExplorerButton.PointToScreen(new Point(0, sequnce.DeviceControl.ExplorerButton.Height));
                    dlg.StartPosition = FormStartPosition.Manual;
                    dlg.Width = sequnce.DeviceControl.ExplorerButton.Width;
                    dlg.Location = buttonScreenPos;

                    dlg.Add("Delete Test", "fa:minus:black", () =>
                    {
                        if (PanelView.Controls.Count > 0)
                        {
                            if (PanelView.Controls[0] == sequnce.DeviceControl)
                                PanelView.Controls.Clear();
                        }

                        foreach (Cycler cycler in sequnce.BatteryTest.Cyclers.Values)
                            cycler.InUse = false;


                        DeviceManager.Unregister(sequnce.BatteryTest.Name);
                        sequnce.DeviceControl.Dispose();
                        sequnce.DeviceControl.ExplorerButton.Tag = null;
                        PanelTestList.Remove(sequnce.DeviceControl.ExplorerButton);
                    });
                    
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                       
                    }
                }

            };

            sequnce.DeviceControl.ExplorerButton.PerfomClicked();
            sequnce.BatteryTest.Start();
        }


        RadioButtonEx lastClicked = null;

        void ShowTest(ControlBatteryTest test, RadioButtonEx button)
        {
            PanelView.Controls.Clear();
           
            PanelView.Controls.Add(test);

            test.ControlMenu.PerfomClick("chart");
            lastClicked = button;

        }


        void OverviewSelect(String display)
        {
            foreach(System.Windows.Forms.Control ctrl in Overview.DisplayData.ContentPanel.Controls)
            {
                if(ctrl is Display)
                {
                    if(ctrl.Name == display)
                    {
                        Debug.WriteLine("found");
                        ((Display)ctrl).BorderColor = Color.Orange;
                    }
                    else
                    {
                        ((Display)ctrl).BorderColor = Color.Transparent;
                    }
                }
            }

        }


        void UpdateOverview()
        {
            if(PanelTestList.Controls == null || PanelTestList.Controls.Count == 0)
            {
                return;
            }

            Overview.DisplayData.Reset();
            Display ClimaChanberInfo = new();
            ClimaChanberInfo.Name = "ClimaChanberInfo";
            ButtonWithIcon showClima = new ButtonWithIcon();
            showClima.ButtonText = "";
            showClima.ButtonIcon = "fa:chart-line:black";
            showClima.IconSizeFactor = 0.6;
            showClima.HoverColor = Color.LightBlue;
            showClima.LedVisible = false;
            showClima.LeftClicked += (s, e) =>
            {
                showChart(ClimaChamber.Recorder, 0);
                OverviewSelect(ClimaChanberInfo.Name);

            };

            
            ClimaChanberInfo.ShowMenuIcon = false;
            ClimaChanberInfo.TitleText = ClimaChamber.Device.Name;
            ClimaChanberInfo.Width = Overview.DisplayData.Width - 20;
            ClimaChanberInfo.Height = 400;

            ClimaChanberInfo.Add(new object[] { ClimaChamber.Device.Temperature }, primary: true);
            ClimaChanberInfo.Add(new object[] { " ", " ", " ", " ", " ", " ", " ", showClima, " " }, primary: false);
            ClimaChanberInfo.UpdateDisplay();
    
            Overview.DisplayData.Add(new[] { ClimaChanberInfo }, true, 140);
            Overview.DisplayData.Add(new[] { "<space>" }, true, 20);

            int index = 0;
            foreach (var item in PanelTestList.Controls)
            {
               
                if (item is RadioButtonEx rb && rb.Tag is BatterySequenceControl seq)
                {

                    string testName = seq.Name;
                    Display TestInfo = new Display();
                    TestInfo.Name = seq.Name;
                    ButtonWithIcon show = new ButtonWithIcon();
                    show.ButtonText = "";
                    show.ButtonIcon = "fa:chart-line:black";
                    show.IconSizeFactor = 0.6;
                    show.HoverColor = Color.LightBlue;
                    show.LedVisible = false;
                    show.LeftClicked += (s, e) =>
                    {
                       showChart(seq.DeviceControl.Recorder, index);
                       OverviewSelect(testName);
                    };

                    ButtonWithIcon folder = new ButtonWithIcon();
                    folder.ButtonText = "";
                    folder.ButtonIcon = "fa:folder-open:black";
                    folder.IconSizeFactor = 0.6;
                    folder.HoverColor = Color.LightBlue;
                    folder.LedVisible = false;
                    folder.LeftClicked += (s, e) =>
                    {
                        showChart(seq.DeviceControl.Recorder, index);
                    };

                    ButtonWithIcon toTest = new ButtonWithIcon();
                    toTest.ButtonText = "";
                    toTest.ButtonIcon = "fa:arrow-right-from-bracket:black";
                    toTest.IconSizeFactor = 0.6;
                    toTest.HoverColor = Color.LightBlue;
                    toTest.LedVisible = false;
                    toTest.LeftClicked += (s, e) =>
                    {
                        JumpToTest(rb.NumberText);

                    };

                    

                   
                    TestInfo.BorderColor = Color.Red;
                    
                    TestInfo.ShowMenuIcon = false;
                    TestInfo.TitleText = seq.Name;
                    TestInfo.Width = Overview.DisplayData.Width-20;
                    TestInfo.Height = 400;

                    TestInfo.Add(new object[] { seq.BatteryTest.Battery.Main.Serielnumber, seq.BatteryTest.Battery.Main.Partnumber }, primary: true);
                    TestInfo.Add(new object[] { seq.BatteryTest.U, seq.BatteryTest.I, seq.BatteryTest.P, seq.BatteryTest.E }, primary: false);
                    TestInfo.Add(new object[] { " ", " ", " ", " ", " ", " ", folder, show, toTest }, primary: false);
                    TestInfo.UpdateDisplay();
                    Overview.DisplayData.Add(new[] { TestInfo }, true, 175);
                    Overview.DisplayData.Add(new[] { "<space>" }, true, 20);

                    TestInfo = null;
                   
                    index++;
                }
            }
            Overview.DisplayData.UpdateDisplay();
        }




        

        void showChart(Chart rec,int group)
        {
            //foreach(var item in TestList)
            //{
            //    ((BaseControl)item.Key).BackColor = item.Value == group ? Color.Orange : Color.White;
            //}
            Overview.ShowControl(rec);
        }

        void SelectExplorer()
        {
            foreach (RadioButtonEx btn in PanelMainMenu.Controls)
            {
                if (btn.Name == "explorer")
                    btn.PerfomClicked();
            }
        }

        void JumpToTest(string index)
        {
            int i = 0;
            foreach (var item in PanelTestList.Controls)
            {
                if (item is RadioButtonEx rb && rb.Tag is BatterySequenceControl seq)
                {
                    if (rb.NumberText == index)
                    {
                        SelectExplorer();
                        rb.PerfomClicked();
                        break;
                    }
                    i++;
                }
            }

        }


        void ShowOverview()
        {
            PanelView.Controls.Clear();
            TableRoot.RowStyles[3].Height = 0;
            Overview.Dock = DockStyle.Fill;
            UpdateOverview();
            PanelView.Controls.Add(Overview);
            showChart(ClimaChamber.Recorder, 0);
            OverviewSelect("ClimaChanberInfo");
        }
        void ShowOverTempControl()
        {
            PanelView.Controls.Clear();
            TableRoot.RowStyles[3].Height = 0;
            ClimaChamber.Dock = DockStyle.Fill;
            UpdateOverview();
            PanelView.Controls.Add(ClimaChamber);
            ClimaChamber.ControlMenu.PerfomClick("chart");
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // FormBatteryMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            ClientSize = new Size(1331, 673);
            Name = "FormBatteryMain";
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            ResumeLayout(false);

        }

        void ShowExplorer()
        {
            TableRoot.RowStyles[3].Height = 50;
            PanelView.Controls.Clear();

            if (lastClicked != null)
            {
                lastClicked.PerfomClicked();
            }

        }

    }

    public class BatterySequenceControl
    {
        public string Name;
        public DeviceBatteryTest BatteryTest;
        public ControlBatteryTest DeviceControl;
  

        public BatterySequenceControl(string name, DeviceBatteryTest batteryTest)
        {
            Name = name;
            BatteryTest = batteryTest;
            DeviceControl = new ControlBatteryTest(BatteryTest);
      
            DeviceControl.Dock = DockStyle.Fill;
        }
    }
} 