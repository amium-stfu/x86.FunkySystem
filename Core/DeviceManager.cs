using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace FunkySystem.Core
{

    public static class DeviceManager
    {
        // Wenn du parallele Zugriffe hast, erwäge ConcurrentDictionary oder einen Lock.
        static readonly Dictionary<string, FunkyDevice> Devices = new();

        // Alle Geräte sind entweder Done oder Ready (und es gibt mindestens eines)

        public static bool DeviceExists(string id) => Devices.ContainsKey(id);
        public static int DeviceCount => Devices.Count;

        public static bool AllInit =>
            Devices.Count > 0 &&
            Devices.Values.All(d => d.State == State.Init);

        public static bool AllReady =>
            Devices.Count > 0 &&
            Devices.Values.All(d => d.State == State.Ready);

        public static bool AllDone =>
            Devices.Count > 0 &&
            Devices.Values.All(d => d.State == State.Done);

        public static bool AllDoneOrReady =>
            Devices.Count > 0 &&
            Devices.Values.All(d => d.State is State.Done or State.Ready);

        public static bool AllOff =>
            Devices.Count > 0 &&
            Devices.Values.Any(d => d.State == State.Off);

        public static bool AnyAlert =>
            Devices.Values.Any(d => d.State == State.Alert);

        public static bool AnyWarning =>
            Devices.Values.Any(d => d.State == State.Warning);

        public static bool AnyRunning =>
            Devices.Values.Any(d => d.State == State.Run);

        public static bool AnyInitializing =>
            Devices.Values.Any(d => d.State == State.Init);

        // Hilfsmethoden zum Verwalten der Devices (optional, Beispiel)
        public static void Register(string id, FunkyDevice device) => Devices[id] = device;

        public static void Unregister(string id) => Devices.Remove(id);

        public static bool TryGet(string id, out FunkyDevice? device) => Devices.TryGetValue(id, out device);

        public static IReadOnlyCollection<FunkyDevice> GetAll() => Devices.Values.ToList().AsReadOnly();
    }

  



 

}
