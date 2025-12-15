using FunkySystem.Devices;
using FunkySystem.Controls;
using FunkySystem.Core;
using FunkySystem.Signals;
using ScottPlot.Plottables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;


namespace FunkySystem.Controls
{
    public partial class FunkyDeviceControl : UserControl
    {

      

        DataGridView gridLog;

        FunkyDevice Device;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Chart Recorder { get; set; } = new Chart();

        public List<BaseControl> PrimaryData = new List<BaseControl>();
        public List<BaseControl> SecondaryData = new List<BaseControl>();

   
        public void SetDevice(FunkyDevice device)
        {
            Device = device;
            typeof(Panel).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic,
                null, ControlSelected, new object[] { true });

            gridLog.DataBindingComplete -= GridLog_DataBindingComplete;
            gridLog.DataBindingComplete += GridLog_DataBindingComplete;

            InitLog(device);
            Device.OnStateChanged -= UpdateStateButton;
            Device.OnStateChanged += UpdateStateButton;


            UpdateStateButton();
        }

        void UpdateStateButton()
        {
            if (btnState.InvokeRequired)
            {
                btnState.BeginInvoke((Action)UpdateStateButton);
                return;
            }
            btnState.ButtonIcon = Device.State.ToFaIcon();
            btnState.ButtonText = Device.State.ToDescriptionString();
            btnState.Invalidate();
        }


        //  Control GetChart()

        float StateHeight = 50;

        public void HideStatePanel()
        {
            StateHeight = tableData.RowStyles[0].Height;
            tableData.RowStyles[0].Height = 0;
        }

        public void ShowStatePanel()
        {
            tableData.RowStyles[0].Height = StateHeight;
        }

        float controlPanelWidth = 200;
        public void HideControlPanel()
        {
            controlPanelWidth = tableMain.ColumnStyles[2].Width;
            tableMain.ColumnStyles[2].Width = 0;
        }
        public void ShowControlPanel()
        {
            tableMain.ColumnStyles[2].Width = controlPanelWidth;
        }

        float sequencePanelHeight = 200;

        public void HideSequencePanel()
        {
            sequencePanelHeight = tableMain.RowStyles[2].Height;
            tableMain.RowStyles[3].Height = 0;
        }
        public void ShowSequencePanel()
        {
            tableMain.RowStyles[3].Height = sequencePanelHeight;
        }


        private void GridLog_DataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e)
        {
            gridLog.Columns["TimeStamp"].Width = 150;
            gridLog.Columns["Level"].Width = 80;
            gridLog.Columns["Message"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            gridLog.Columns["TimeStamp"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss.fff";

            gridLog.ClearSelection();
            gridLog.CurrentCell = null;
            ScrollToBottomSafely(gridLog);
        }

        public void InitLog(FunkyDevice device)
        {
            device.Log.UiContext ??= SynchronizationContext.Current;
            gridLog.DataSource = device.Log.LogBindingSource;


            gridLog.CellFormatting -= gridLog_CellFormatting;
            gridLog.CellFormatting += gridLog_CellFormatting;

            gridLog.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridLog.Name = "gridLog";
            gridLog.Size = new System.Drawing.Size(LogPanel.ContentPanel.Width, LogPanel.ContentPanel.Height);
            gridLog.TabIndex = 0;
            gridLog.AllowUserToResizeColumns = false;
            gridLog.AllowUserToAddRows = false;
            gridLog.RowHeadersVisible = false;
            gridLog.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            gridLog.MultiSelect = false;
            gridLog.ReadOnly = true;
            gridLog.BackgroundColor = Color.Red;
            gridLog.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            gridLog.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            gridLog.ColumnHeadersVisible = false;
            gridLog.RowHeadersVisible = false;
            gridLog.AllowUserToAddRows = false;
            gridLog.AllowUserToDeleteRows = false;
            gridLog.AllowUserToOrderColumns = false;
            gridLog.AllowUserToResizeColumns = false;

            gridLog.BackgroundColor = System.Drawing.Color.LightGray;
            gridLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            gridLog.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            gridLog.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridLog.Size = new System.Drawing.Size(LogPanel.ContentPanel.Width, LogPanel.ContentPanel.Height);
            gridLog.TabIndex = 0;

            LogPanel.ContentPanel.Controls.Add(this.gridLog);

        }

        protected override void Dispose(bool disposing)
        {


            if (disposing)
            {
                CleanupManaged();
            }
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void CleanupManaged()
        {
            // Logging-Grid aufräumen
            if (gridLog != null)
            {
                gridLog.DataBindingComplete -= GridLog_DataBindingComplete;
                gridLog.CellFormatting -= gridLog_CellFormatting;
                gridLog.DataSource = null;
            }

            // FunkyDevice stoppen, wenn das Control „Eigentümer“ ist
            if (Device != null)
            {
                Device.Stop();          // oder Device.UserStop(), je nach gewünschter Semantik
                Device = null;
            }

            // Chart/Recorder freigeben
            if (Recorder != null)
            {
                Recorder.Dispose();
                Recorder = null;
            }
        }


        private void gridLog_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var row = gridLog.Rows[e.RowIndex];
            string level = row.Cells["Level"].Value?.ToString() ?? string.Empty;

            ApplyLevelStyle(row, level);
        }

        private static void ApplyLevelStyle(DataGridViewRow row, string level)
        {

            switch (level)
            {
                case "Information":
                    SetRowColors(row, Color.White, Color.Black);
                    break;
                case "Warning":
                    SetRowColors(row, Color.Yellow, Color.Black);
                    break;
                case "Error":
                    SetRowColors(row, Color.Tomato, Color.Black);
                    break;
                case "Fatal":
                    SetRowColors(row, Color.Tomato, Color.Black);
                    break;
                default:
                    SetRowColors(row, Color.White, Color.Black);
                    break;
            }
        }

        private static void SetRowColors(DataGridViewRow row, Color back, Color fore)
        {
            var style = row.DefaultCellStyle;
            style.BackColor = back;
            style.ForeColor = fore;
            style.SelectionBackColor = back;
            style.SelectionForeColor = fore;
        }

        private void ScrollToBottomSafely(DataGridView dgv)
        {
            if (dgv.RowCount == 0) return;

            // In manchen Fällen (Sortierung/Binding) muss man das Scrollen verzögert ausführen
            dgv.BeginInvoke((Action)(() =>
            {
                int last = dgv.RowCount - 1;
                try
                {
                    dgv.FirstDisplayedScrollingRowIndex = last;
                }
                catch { /* kann kurzzeitig OutOfRange sein – ignorieren */ }
            }));
        }


        public ControlRadioMenu ControlMenu;

        public FunkyDeviceControl()
        {
            InitializeComponent();
            ControlMenu = new ControlRadioMenu();

            gridLog = new System.Windows.Forms.DataGridView();
            Recorder.BackColor = Color.WhiteSmoke;
            Recorder.BorderColor = Color.WhiteSmoke;
            DisplayData.ShowMenuIcon = false;
            Recorder.Dock = DockStyle.Fill;
            ControlSelected.Controls.Add(Recorder);

            ControlMenu.BackColor = Color.WhiteSmoke;
            ControlMenu.Dock = DockStyle.Fill;
            ControlMenu.Location = new Point(1437, 0);
            ControlMenu.Margin = new Padding(0);
            ControlMenu.Name = "controlRadioMenu";
            tableMain.SetRowSpan(ControlMenu, 2);
            ControlMenu.Size = new Size(60, 640);
            ControlMenu.TabIndex = 5;
            tableMain.Controls.Add(ControlMenu, 2, 0);

            ControlMenu.Add("chart","fa:chart-line:black", () =>
            {
                ShowControl(Recorder);
            });
        }



        private void FunkyDeviceControl_Load(object sender, EventArgs e)
        {
          //  btnChart.PerformLeftClick(); // jetzt hat das Control ein Handle
        }


 
        public void ShowControl(Control control)
        {
          SetControl(control, null);
        }

        void SetControl(Control control, ButtonWithIcon button)
        {
    
            Action apply = () =>
            {
                ControlSelected.SuspendLayout();
                if (button != null)
                    button.SetLedColor(Color.Orange);
                ControlSelected.Controls.Clear();
                control.Dock = DockStyle.Fill;
                ControlSelected.Controls.Add(control);
                ControlSelected.ResumeLayout();
            };

            if (ControlSelected.IsHandleCreated)
            {
                ControlSelected.BeginInvoke(apply);
            }
            else
            {
                // Entweder synchron ausführen, falls wir im UI-Thread sind:
                apply();

                // Alternativ: verzögert ausführen, sobald ein Handle vorliegt:
                // ControlSelected.HandleCreated += (s, e) => ControlSelected.BeginInvoke(apply);
            }
        }

     

        private void LogPanel_Resize(object sender, EventArgs e)
        {
            this.gridLog.Size = new System.Drawing.Size(LogPanel.ContentPanel.Width - 20, LogPanel.ContentPanel.Height - 20);
            if(!gridLog.Columns.Contains("TimeStamp")) return;
            gridLog.Columns["TimeStamp"].Width = 150;
            gridLog.Columns["Level"].Width = 80;
            gridLog.Columns["Message"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

    }



}
