using FunkySystem;
using FunkySystem.Core;
using FunkySystem.Signals;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.WinForms.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace FunkySystem
{
    public class FunkyDevice
    {
        public DeviceLogger Log;

        //Settings Handling
        public Dictionary<object, string> Settings = new Dictionary<object, string>();
        public string FolderSettings;
        public string FolderSequences;
        public string FolderWorkflows;
        public string FolderResult;
        public string Name;
        public int Id = 0;
        public Action OnStateChanged;

        private State _state = State.Ready;
        public State State
        {
            get { return _state; }
            set
            {
                if (_state == value)
                    return;

                _state = value;
                Status = _state.ToDescriptionString();
                OnStateChanged?.Invoke();
                Log.Info("State changed -> " + _state.ToDescriptionString());
            }
        }

        public void SetAlertState(string message)
        {
            State = State.Alert;
            Log.Error("Alert State set -> " + message);
        }

        public void SetWarningState(string message)
        {
            State = State.Warning;
            Log.Warn("Warning State set -> " + message);
        }

        public void SetAbortedState(string message)
        {
            State = State.Aborted;
            Log.Error("Aborted State set -> " + message);
        }

        public void SetReadyState()
        {
            State = State.Ready;
            Log.Info("Ready State set");
        }

        public void SetRunningState()
        {
            State = State.Running;
            Log.Info("Running State set");
        }

        public string Status = "unknown";
        public string SubStatus = "unknown";

        public bool Run { get; set; }

        public Dictionary<string, FunkyPlcOld> Sequences = new Dictionary<string, FunkyPlcOld>();

        public FunkyPlcOld SequenceSelected = null;

        public FunkyDevice(string name)
        {
            Name = name;
            Log = new DeviceLogger(Path.Combine(Program.DataDirectory, Name, "Log"));
            Log.Info("Startting Device");

            FolderResult = System.IO.Path.Combine(Program.DataDirectory, Name, "Result");
            if (!System.IO.Directory.Exists(FolderSettings)) System.IO.Directory.CreateDirectory(FolderResult);

            FolderSettings = System.IO.Path.Combine(Program.SettingsDirectory, Name, "Settings");
            if (!System.IO.Directory.Exists(FolderSettings)) System.IO.Directory.CreateDirectory(FolderSettings);

            FolderSequences = System.IO.Path.Combine(Program.SettingsDirectory, Name, "Sequences");
            if (!System.IO.Directory.Exists(FolderSequences)) System.IO.Directory.CreateDirectory(FolderSequences);

            FolderWorkflows = System.IO.Path.Combine(Program.SettingsDirectory, Name, "Workflows");
            if (!System.IO.Directory.Exists(FolderWorkflows)) System.IO.Directory.CreateDirectory(FolderWorkflows);

            Status = State.ToString();
            SubStatus = "";
            DeviceManager.Register(Name, this);
        }

        //Settings
        public void SaveSettings()
        {
            Logger.InfoMsg($"[{this.GetType().Name}] {Name}: " + "SaveSettings");
            string name = "New";

            EditValue.WithKeyboardDialog(ref name, text: "Settings Name");
            Dictionary<string, object> list = new Dictionary<string, object>();
            list.Add("Name", name);
            foreach (object item in Settings.Keys)
            {


                if (item is Signal)
                {
                    Signal read = item as Signal;
                    list.Add(read.Name, read.Value);
                }

                if (item is StringSignal)
                {
                    StringSignal read = item as StringSignal;
                    list.Add(read.Name, read.Value);
                }
            }
            string json = JsonSerializer.Serialize<Dictionary<string, object>>(list, new JsonSerializerOptions
            {
                WriteIndented = true,
                IncludeFields = true
            });
            string filename = System.IO.Path.Combine(Program.SettingsDirectory, FolderSettings, name + ".json");
            System.IO.File.WriteAllText(filename, json);

        }
        public string SettingsFolder()
        {
            return System.IO.Path.Combine(Program.SettingsDirectory, FolderSettings);
        }
        public string SequencesFolder()
        {
            return System.IO.Path.Combine(Program.SettingsDirectory, FolderSequences);
        }
        public void LoadSettings(string filename = null)
        {
            Log.Info("Load settings from file " + filename);
            string jsonString = string.Empty;
            string uri;
            if (filename == null)
            {

                using (System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
                {
                    openFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
                    openFileDialog.Title = "Select a JSON file";

                    if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        uri = openFileDialog.FileName;

                    }
                    else
                    {
                        return;
                    }
                }
            }
            else
            {
                uri = System.IO.Path.Combine(Program.SettingsDirectory, FolderSettings, filename);
            }
            jsonString = System.IO.File.ReadAllText(uri);
            WriteSettings(jsonString);
        }

        public void LoadSequence(string filename = null, bool autoStart = false)
        {
            Log.Info("Load Sequence from file " + filename);
            string uri = System.IO.Path.Combine(Program.SettingsDirectory, FolderSequences, filename);
            string json = System.IO.File.ReadAllText(uri);
            Dictionary<string, object> read = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            SequenceSelected = Sequences[read["Name"].ToString()];
            SequenceSelected.SetSettings(json);
            SequenceSelected.UpateSequenceDisplay();

            if (autoStart)
                StartSequence();
        }

        public void StartSequence()
        {

            Log.Info($"Sequence {SequenceSelected.Text} Start");
            SequenceSelected.Start();

        }

        public virtual void PreWriteSettings()
        {

        }
        public virtual void PostWriteSettings()
        {

        }
        public void WriteSettings(string json = "")
        {
            Dictionary<string, object> list = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

            foreach (var item in list)
                Logger.InfoMsg($"[{this.GetType().Name}] {Name}: " + item.Key + ": " + item.Value.ToString());

            string readName = list["Name"].ToString();

            foreach (object s in Settings.Keys)
            {
                string key = s.ToString();

                

                try
                {
                    if (s is Signal)
                    {
                        Signal set = s as Signal;
                        set.Value = ((string)list[set.Name]).ToDouble();
                    }

                    if (s is StringSignal)
                    {
                        StringSignal set = s as StringSignal;
                        set.Value = list[set.Name].ToString();
                    }
                }
                catch
                {
                    Stop();
                    Log.Error($"[{this.GetType().Name}] {Name}: " + "Load Settings failed");
                }
            }
        }

        public virtual void RunSettings()
        {
        }



        public List<string> GetSettingsList()
        {
            string folderPath = System.IO.Path.Combine(Program.SettingsDirectory, Name, "Presets");

            List<string> fileNamesWithoutExtension = new List<string>();

            if (System.IO.Directory.Exists(folderPath))
            {
                string[] jsonFiles = System.IO.Directory.GetFiles(folderPath, "*.json");

                foreach (string file in jsonFiles)
                {
                    string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(file);
                    fileNamesWithoutExtension.Add(fileNameWithoutExtension);
                }

                // Test output
                foreach (var name in fileNamesWithoutExtension)
                {
                    Log.Info(name);
                }
            }
            else
            {
               Log.Error($"[{this.GetType().Name}] {Name}: " + "Directory does not exist.");
            }

            return fileNamesWithoutExtension;
        }


        public void SetStatus(string status, string substatus)
        {
            Status = status;
            SubStatus = substatus;
        }

        public void SelectSequence(int pIndex)
        {
            string key = Sequences.ElementAt(pIndex).Key;
            SequenceSelected = Sequences[key];
        }

        public virtual void Stop()
        {
            if (SequenceSelected == null) return;
            if (SequenceSelected.IsRunning)
            {
                SequenceSelected.Abort("Stopped");
                Log.Info($"[{this.GetType().Name}] {Name}: " + "Stopped " + SequenceSelected.Name);
            }

        }


        public virtual void UserStop()
        {
            if (SequenceSelected == null) return;
            Status = "stop";
            SubStatus = "Ready";
            if (SequenceSelected.IsRunning)
            {
                SequenceSelected.UserAbort("Stopped by user");
            }
        }

        public virtual void Start()
        {
            if (SequenceSelected == null) return;
            SequenceSelected.Start();
            Log.Info("Start " + SequenceSelected.Name);
        }

    }

    /// <summary>
    /// Eine Zeile im Geräte-Log für die Anzeige im DataGridView.
    /// Spaltennamen sind auf die bestehenden Grid-Spalten abgestimmt.
    /// </summary>
    public class DeviceLogRow
    {
        public DateTime TimeStamp { get; set; }
        public string Level { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Exception { get; set; }
    }


    /// <summary>
    /// Serilog-Sink, der LogEvents direkt in eine BindingSource schreibt.
    /// Jeder FunkyDevice bekommt damit ein eigenes Log.
    /// </summary>
    public sealed class DeviceLogSink : ILogEventSink
    {
        private readonly BindingSource _bindingSource;
        private readonly Func<SynchronizationContext?> _uiContextProvider;
        private readonly int _maxEntries;
        private readonly object _syncRoot = new object();


        public DeviceLogSink(BindingSource bindingSource,
        Func<SynchronizationContext?> uiContextProvider,
        int maxEntries)
        {
            _bindingSource = bindingSource ?? throw new ArgumentNullException(nameof(bindingSource));
            _uiContextProvider = uiContextProvider ?? throw new ArgumentNullException(nameof(uiContextProvider));
            _maxEntries = maxEntries;
        }


        public void Emit(LogEvent logEvent)
        {
            if (logEvent == null)
                return;


            if (_bindingSource.List is not IList list)
                return;


            void AddRow()
            {
                lock (_syncRoot)
                {
                    var row = new DeviceLogRow
                    {
                        TimeStamp = logEvent.Timestamp.ToLocalTime().DateTime,
                        Level = logEvent.Level.ToString(),
                        Message = logEvent.RenderMessage(),
                        Exception = logEvent.Exception?.ToString(),
                    };


                    list.Add(row);


                    if (_maxEntries > 0 && list.Count > _maxEntries)
                    {
                        // älteste Zeile entfernen
                        list.RemoveAt(0);
                    }
                }
            }


            // Aktuellen SynchronizationContext (Benutzeroberflächen-Thread) abfragen
            var context = _uiContextProvider();


            if (context != null)
            {
                context.Post(_ => AddRow(), null);
            }
            else
            {
                // Fallback: noch kein Benutzeroberflächen-Context gesetzt
                AddRow();
            }
        }
    }



    public class DeviceLogger
    {
        public ILogger Log { get; }
        public BindingSource LogBindingSource { get; } = new BindingSource();


        // Wird von der Benutzeroberflächen-Seite (FunkyDeviceControl) gesetzt
        public SynchronizationContext? UiContext;


        // maximale Anzahl Zeilen im Grid
        private const int MaxLogEntries = 5000;


        public DeviceLogger(string folder)
        {
            string logFilePath = System.IO.Path.Combine(folder, "log-.txt");


            // eigener Sink pro DeviceLogger
            var uiSink = new DeviceLogSink(LogBindingSource, () => UiContext, MaxLogEntries);


            Log = new LoggerConfiguration()
            .WriteTo.File(
            path: logFilePath,
            fileSizeLimitBytes: 10 * 1024 * 1024,
            rollOnFileSizeLimit: true,
            rollingInterval: RollingInterval.Day,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:l}{NewLine}{Exception}")
            .WriteTo.Sink(uiSink) // hier statt WriteToGridView()
            .CreateLogger();
        }


        public void Info(string message)
        {
            Log.Information(message);
            Debug.WriteLine("INFO: " + message);
        }


        public void Warn(string message)
        {
            Log.Warning(message);
            Debug.WriteLine("WARN: " + message);
        }


        public void Error(string message)
        {
            Log.Error(message);
            Debug.WriteLine("ERROR: " + message);
        }


        public void Fatal(string message, Exception? ex = null)
        {
            Log.Fatal(message, ex);
            Debug.WriteLine("FATAL: " + message);
        }
    }
}
