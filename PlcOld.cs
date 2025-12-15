using FunkySystem.Core;
using FunkySystem.Helpers;
using FunkySystem.Signals;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace FunkySystem
{
    public class FunkyPlcOld
    {

        public string Type
        {
            get; set;
        }

        public string Text
        {
            get; set;
        }

        public int? Index
        {
            get; set;
        }

        public int State
        {
            get; set;
        }

        string status;
        public string Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
                //QB.Logger.Info($"{Name}.Status change > {value}");
            }
        }

        private string subStatus;
        public string SubStatus
        {
            get
            {
                return subStatus;
            }
            set
            {
                subStatus = value;
                //QB.Logger.Info($"{Name}.SubStatus change > {value}");
            }
        }
        public DateTime Starttime;
        public DateTime Endtime;
        public DateTime StepStart;

        public string Name { get; set; }
        public string Description { get; set; }
        public string SubDescription { get; set; }
        public string Id { get; set; }

        private System.Threading.Thread runThread = null;
        private System.Threading.Thread idleThread = null;
        private System.Threading.CancellationTokenSource ctsRun;
        private System.Threading.CancellationTokenSource ctsIdle;
        public Dictionary<object, string> Settings = new Dictionary<object, string>();

        // public CsvLogger CsvLog;

        public StringSignal InfoDisplay = new StringSignal("display", text: "Display");

        DeviceLogger Log;

        FunkyDevice Device;


        public FunkyPlcOld(string name, string id, FunkyDevice device)
        {
            Name = name;
            Id = id;

            Status = "";
            SubStatus = "";
            Type = "?";
            Index = 0;
            State = 0;
            Device = device;
            Log = Device.Log;
        }


        public void ExportSettings(string folder)
        {
            Logger.InfoMsg($"[{this.GetType().Name}] {Name}: ExportSettings");

            string name = "New";

            EditValue.WithKeyboardDialog(ref name, text: "Settings Name");

            Dictionary<string, object> list = new Dictionary<string, object>();
            list.Add("Name", Name);
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

            string file = System.IO.Path.Combine(folder, name + ".json");
            System.IO.File.WriteAllText(file, json);

        }
        public void ImportSettings()
        {
            string jsonString = string.Empty;

            using (System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
            {
                openFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
                openFileDialog.Title = "Select a JSON file";

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    jsonString = System.IO.File.ReadAllText(filePath);

                    SetSettings(jsonString);
                }
            }
        }
        public void SetSettings(string json = "")
        {
            Dictionary<string, object> list = JsonSerializer.Deserialize<Dictionary<string, object>>(json);


            foreach (var item in list)
                Log.Info($"[{this.GetType().Name}] {Name}: " + item.Key + ": " + item.Value.ToString());

            string readName = list["Name"].ToString();

            if (Name != readName)
            {
                Log.Error($"[{this.GetType().Name}] {Name}: " + "Wrong procedure");
                return;
            }

            foreach (object s in Settings.Keys)
            {

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
                    Log.Error($"[{this.GetType().Name}] {Name}: " + "Load Settings failed");
                }
            }
        }
        public void RunSequenceFile(FunkyDevice client, string file, bool autoStart)
        {
            if (IsAborted)
            {
                Logger.FatalMsg($"[{this.GetType().Name}] {Name}: " + ".RunSequenceFile() aborted");
                //		throw new SequenceAbortException("Stopped");
                return;
            }
            ;
            if (client == null)
            {
                Logger.FatalMsg($"[{this.GetType().Name}] {Name}: " + ".RunSequenceFile() client is null");
                return;
            }
            client.LoadSequence(file, autoStart);
        }


        public void Destroy()
        {
            ctsRun?.Cancel();
            ctsIdle?.Cancel();
        }

        public double StepDuration()
        {
            TimeSpan d = DateTime.Now - StepStart;
            return d.TotalMilliseconds;
        }
        public void StartStepClock()
        {
            StepStart = DateTime.Now;
        }

        void run(System.Threading.CancellationToken token)
        {
            Index = 0;
            Starttime = DateTime.Now;
            Log.Info($"[{this.GetType().Name}] {Name}: " + ".Run() started");

            try
            {
                Run();
                if (IsAborted) return;
                finalize();
            }
            //		catch (SequenceAbortException ex) {
            //			// Gewollter Abbruch
            //			QB.Logger.Warn(Name + ".Run() was aborted: " + ex.Message);

            //		}
            catch (Exception e)
            {
                Log.Fatal($"[{this.GetType().Name}] {Name}: " + ".Run() Code Error");

                Abort(".Run() Code Error");
            }
        }
        void idle(System.Threading.CancellationToken token)
        {
            Log.Info($"[{this.GetType().Name}] {Name}: " + ".Idle() started");

            try
            {
                Idle();
            }
            catch (Exception e)
            {
                Log.Fatal($"[{this.GetType().Name}] {Name}: " + ".Idle() Code Error");

            }

            Log.Info($"[{this.GetType().Name}] {Name}: " + ".Idle() finalized regular");
        }
        void finalize()
        {
            try
            {
                Log.Info($"[{this.GetType().Name}] {Name}: " + ".Run() finalized regular");
                Finalize();
                ctsRun?.Cancel();
                ctsIdle?.Cancel();
                Endtime = DateTime.Now;

            }
            catch (Exception e)
            {

                Endtime = DateTime.Now;
                Log.Fatal($"[{this.GetType().Name}] {Name}: " + ".Finalize() Code Error");
            }
        }


        public string LastAbortReason;
        public void Abort(string reason = "User")
        {
            if (IsAborted) return;

            ctsRun?.Cancel();
            ctsIdle?.Cancel();

            Log.Info($"[{this.GetType().Name}] {Name}: " + "Abort ");

            LastAbortReason = reason;
            OnAbort();
            Status = "Aborted: " + reason;
            Log.Info($"[{this.GetType().Name}] {Name}: " + " Abort requested: " + reason);
            //	throw new SequenceAbortException(reason);
        }

        public void UserAbort(string reason = "User")
        {

            if (IsAborted) return;

            ctsRun?.Cancel();
            ctsIdle?.Cancel();

            Log.Info($"[{this.GetType().Name}] {Name}: " + "Abort ");

            LastAbortReason = reason;
            OnAbort();
            Status = "Aborted: " + reason;
            Log.Info($"[{this.GetType().Name}] {Name}: " + " Abort requested by user: ");

        }

        public void Start()
        {
            try
            {
                Init();


                ctsIdle = new System.Threading.CancellationTokenSource();
                idleThread = new System.Threading.Thread(() => idle(ctsIdle.Token));
                idleThread.IsBackground = true;
                idleThread.Start();

                ctsRun = new System.Threading.CancellationTokenSource();
                runThread = new System.Threading.Thread(() => run(ctsRun.Token));
                runThread.IsBackground = true;
                runThread.Start();
            }
            catch (Exception e)
            {
                Log.Fatal($"[{this.GetType().Name}] {Name}: " + ".Start() Code Error");
                Abort("Code Error");
            }
        }
        public void Stop()
        {
            finalize();
        }


        public bool IsRunning => ctsRun != null && !ctsRun.IsCancellationRequested;
        public bool IsAborted => ctsRun != null && ctsRun.IsCancellationRequested;

        public void Wait(double d, string unit = "ms", string subStatus = null)
        {
            DateTime start = DateTime.Now;

            if (unit == "ms") d = d;
            if (unit == "s") d = d * 1000;
            if (unit == "m") d = d * 1000 * 60;
            if (unit == "h") d = d * 1000 * 3600;

            while (DateTime.Now < start.AddMilliseconds(d))
            {
                if (IsAborted)
                {
                    Abort("stopped");
                    return;
                }
                System.Threading.Thread.Sleep(10);
                if (subStatus != null)
                    SubStatus = subStatus + " " + FormatTimeSpan(start.AddMilliseconds(d) - DateTime.Now);
            }

        }



        public void ForceCommandSet(Module module, int set, int timeout, string subStatus = null, System.Action onTimeout = null)
        {
            DateTime start = DateTime.Now;
            onTimeout ??= () => Abort();
            while (module.State.Value != set)
            {

                if (IsAborted)
                {
                    return;
                }

                if (DateTime.Now > start.AddMilliseconds(timeout))
                {
                    Log.Fatal($"[{this.GetType().Name}] {Name}: " + $" ForceCommandSetValue timeoout {module.Name} = {set}");
                    onTimeout();
                    break;
                }
                module.Command.Value = set;

                if (subStatus != null)
                    SubStatus = subStatus + " " + FormatTimeSpan(DateTime.Now - start);

                Wait(100);


            }
        }
        public void WaitSignalLeavesRange(Signal signal, double min, double max, int timeout, string subStatus = null, System.Action onTimeout = null)
        {
            DateTime start = DateTime.Now;
            onTimeout ??= () => Abort();
            while (signal.Value > min && signal.Value < max)
            {

                if (DateTime.Now > start.AddMilliseconds(timeout))
                {
                    Log.Fatal($"[{this.GetType().Name}] {Name}: " + $" WaitSignalInRange timeout {signal.Name}");
                    onTimeout();
                    break;
                }

                if (subStatus != null)
                    SubStatus = subStatus + " " + FormatTimeSpan(DateTime.Now - start);

                Wait(100);
                if (IsAborted)
                {
                    break;
                }
            }
            ;
        }

        public void WaitForCondition(Func<bool> condition, int timeout, string status = null, string subStatus = null, System.Action onTimeout = null)
        {
            DateTime start = DateTime.Now;
            onTimeout ??= () => Abort("Timeout in WaitForCondition");

            while (!condition())
            {
                if (IsAborted)
                    return;

                if ((DateTime.Now - start).TotalMilliseconds > timeout)
                {
                    Log.Fatal($"[{this.GetType().Name}] {Name}: " + " WaitForCondition timeout");
                    onTimeout(); // ← das wirft die SequenceAbortException
                    return;
                }

                if (Status != null) Status = status;

                if (subStatus != null)
                    SubStatus = subStatus + " " + FormatTimeSpan(DateTime.Now - start);

                Wait(100);
            }
        }

        void BreakIfStopped()
        {
            if (IsAborted)
                Abort("Stopped");
        }


        string FormatTimeSpan(TimeSpan timeSpan)
        {
            int totalHours = (int)timeSpan.TotalHours;
            return $"{Math.Abs(totalHours):D2}:{Math.Abs(timeSpan.Minutes):D2}:{Math.Abs(timeSpan.Seconds):D2}";
        }


        public virtual void Init()
        {
        }

        public virtual void InitAsSubprocess()
        {
        }

        public virtual void Run()
        {
        }

        public virtual void Idle()
        {
        }

        public virtual void Finalize()
        {
        }

        public virtual void OnAbort()
        {
        }

        public virtual void UpateSequenceDisplay()
        {
        }

        public virtual void StartAsSubprocess()
        {
            try
            {
                InitAsSubprocess();
                ctsIdle = new System.Threading.CancellationTokenSource();
                idleThread = new System.Threading.Thread(() => idle(ctsIdle.Token));
                idleThread.IsBackground = true;
                idleThread.Start();

                ctsRun = new System.Threading.CancellationTokenSource();
                runThread = new System.Threading.Thread(() => run(ctsRun.Token));
                runThread.IsBackground = true;
                runThread.Start();
            }
            catch (Exception e)
            {
                Log.Fatal($"[{this.GetType().Name}] {Name}: " + ".Start() Code Error");

                Abort("Code Error");
            }

        }

        public class SequenceAbortException : Exception
        {
            public SequenceAbortException(string message) : base("Sequence abort " + message)
            {
            }
        }
    }
}
