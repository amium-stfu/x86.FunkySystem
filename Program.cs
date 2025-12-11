using Serilog;
using Serilog.Core;
using Serilog.Events;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.RightsManagement;
using System.Windows.Input;
using FunkySystem.UI;
using static FunkySystem.Logger;
using FunkySystem.Battery;
using FunkySystem.BatteryCharger;

namespace FunkySystem
{

    
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        /// 

        

        public static readonly Dictionary<string,object> ConnectedDevices = new();


        public static FormBatteryMain? LandingPage;

        public static string SettingsDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings");
        public static string DataDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        public static string ResultDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Result");

        [STAThread]
        static void Main()
        {
            if (!System.IO.Directory.Exists(SettingsDirectory))
                System.IO.Directory.CreateDirectory(SettingsDirectory);

            if (!System.IO.Directory.Exists(DataDirectory))
                System.IO.Directory.CreateDirectory(DataDirectory);

            if (!System.IO.Directory.Exists(ResultDirectory))
                System.IO.Directory.CreateDirectory(ResultDirectory);


            ConnectedDevices.Add("Cycler1", new Cycler("Cycler1", demo: true));
            ConnectedDevices.Add("Cycler2", new Cycler("Cycler2", demo: true));
            ConnectedDevices.Add("Cycler3", new Cycler("Cycler3", demo: true));
            ConnectedDevices.Add("Cycler4", new Cycler("Cycler4", demo: true));
            ConnectedDevices.Add("Cycler5", new Cycler("Cycler5", demo: true));
            ConnectedDevices.Add("Cycler6", new Cycler("Cycler6", demo: true));
            ConnectedDevices.Add("Cycler7", new Cycler("Cycler7", demo: true));
            ConnectedDevices.Add("Cycler8", new Cycler("Cycler8", demo: true));
            ConnectedDevices.Add("ClimaChamber", new DeviceClimaChamber("Oven"));



            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            LandingPage = new FormBatteryMain();
            LandingPage.WindowState = FormWindowState.Maximized;



            Application.Run(LandingPage);
        }
    }

    public class LogEntry
    {
        public DateTime Time { get; set; }
        public LogLevelEnum LevelEnum { get; set; } = LogLevelEnum.Information;
        public string Level => LevelEnum.ToString(); // nie null
        public string Message { get; set; } = "";
    }

    public static class Logger
    {

        public class WinFormsSink : ILogEventSink
        {
            private readonly SynchronizationContext _ui = new WindowsFormsSynchronizationContext();
            public BindingList<LogEntry> Entries { get; } = new();

            public void Emit(LogEvent e)
            {
                var entry = new LogEntry
                {
                    Time = e.Timestamp.LocalDateTime,
                    LevelEnum = (LogLevelEnum)Enum.Parse(typeof(LogLevelEnum), e.Level.ToString(), true),
                    Message = e.RenderMessage() ?? string.Empty
                };

                _ui.Post(_ =>
                {
                    if (Entries.Count > 2000) Entries.RemoveAt(0);
                    Entries.Add(entry);
                }, null);
            }

        }

        public enum LogLevelEnum { Debug, Information, Warning, Error, Fatal }

      


        public static WinFormsSink winFormsSink = new WinFormsSink();

        static ILogger Log = new LoggerConfiguration()
       .MinimumLevel.Debug()
       .WriteTo.File(
           path: EnsureLogDirectory(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log", "log-.txt")),
           fileSizeLimitBytes: 10 * 1024 * 1024,
           rollOnFileSizeLimit: true,
           rollingInterval: RollingInterval.Day,
           outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:l}{NewLine}{Exception}")
       .WriteTo.Sink(winFormsSink) // <-- Wichtig!
       .CreateLogger();

        private static string EnsureLogDirectory(string logPath)
        {
            var dir = System.IO.Path.GetDirectoryName(logPath);
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            return logPath;
        }


        public static void DebugMsg(string msg)
        {
            Debug.WriteLine($"[Debug] {msg}");
            Log.Debug(msg);
        }

        public static void InfoMsg(string msg)
        {
            Debug.WriteLine($"[LOG] {msg}");
            Log.Information(msg);
        }

        public static void WarningMsg(string msg)
        {
            Debug.WriteLine($"[Warning] {msg}");
            Log.Warning(msg);
        }

        public static void FatalMsg(string msg, Exception ex = null)
        {
            Debug.WriteLine($"[FATAL] {msg}");
            if (ex != null) Debug.WriteLine(ex.Message);
            Log.Fatal(ex, msg);
        }
    }
}