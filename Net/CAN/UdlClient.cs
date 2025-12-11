using System;
using System.Threading;
using FunkySystem.Signals;
using FunkySystem.Core;
using System.Diagnostics;

namespace FunkySystem.Net.CAN
{
    public class UdlClient
    {
        public string Name;
        public Can Can;
        private Thread thread;
        Thread writebackThread;
        public bool RemoteTime = false;


        public UdlClient(string name)
        {
            Name = $"{SignalPool.GetNextId}.{name}";
            Debug.WriteLine($"UdlClient created: {Name}");
        }

        public void Open(string ip,int port)
        {
            try { Can?.Close(); } catch { }
            Can = new Can(ip,port);

            try { thread?.Abort(); } catch { }
            thread = new Thread(Idle) { IsBackground = true };
            thread.Start();
            writebackThread = new Thread(WritebackLoop) { IsBackground = true };
            writebackThread.Start();

            Can.MessageReceived += OnCanMessageReceived;
        }

        private void Idle()
        {
            while (true)
            {
                HbIdle();
                Thread.Sleep(100);
            }
        }

        public void OnCanMessageReceived(uint id, byte dlc, byte[] data)
        {
            if (id >= 0x480 && id <= 0x4FF)
                HandlePdo(id, dlc, data);
            else if (id >= 0x700 && id <= 0x7FF)
                HandleHeartbeat(id, dlc, data);
        }

        private void HandlePdo(uint id, byte dlc, byte[] data)
        {
            uint moduleId = (uint)(((id & 0x7F) << 4) | (data[7] & 0x0F));
            string moduleName = $"{Name}.0x{moduleId:X3}";

            Module module;
            if (!SignalPool.TryGet(moduleName, out var existing))
            {
                module = new Module(moduleName);
               
                Modules[moduleId] = module;
                SignalManager.PushSignal(Name, module, direct: true);
            }
            else
            {
                if (existing is not Module)
                {
                    SignalPool.OverwriteKey(moduleName, new Module(moduleName));
                    module = (Module)SignalPool.GetModule(moduleName);
                }
                else
                {
                    module = (Module)existing;
                }

                module.WriteBack = true;
                if(!Modules.ContainsKey(moduleId))
                    Modules[moduleId] = module;
            }

            int type = data[6];
            switch (type)
            {
                case 1:
                    {
                        int raw = BitConverter.ToInt32(data, 0);
                        int state = raw & 0xFF;
                        module.State.Value = state;
                        break;
                    }

                case 2:
                    {
                        float alert = BitConverter.ToSingle(data, 0);
                        module.SetProperty("alert", alert.ToString());
                        break;
                    }

                case 3:
                    {
                        float val = BitConverter.ToSingle(data, 0);
                        ushort metric = (ushort)(data[4] | (data[5] << 8));
                        bool controller = (data[7] & 0x10) != 0;
                        module.Unit = GetUnit(metric, controller);

                        module.Value = val;
                      
                        break;
                    }

                case 4:
                    {
                        float v = BitConverter.ToSingle(data, 0);
                        module.Set.Value = v;
                        break;
                    }

                case 5:
                    {
                        float v = BitConverter.ToSingle(data, 0);
                        module.Out.Value = v;
                        break;
                    }
            }
        }

        private string GetUnit(ushort metric, bool isController)
        {
            return (metric & 0x0FF0) switch
            {
                0x0160 => "V",
                0x0170 => "A",
                0x0210 => "°C",
                0x0110 when (metric & 0x0F) == 0x0A => "%",
                0x0110 when (metric & 0x0F) == 0x0C => "ppm",
                _ => string.Empty,
            };
        }

        private void HandleHeartbeat(uint id, byte dlc, byte[] data)
        {
            // optional implementation
        }

        private DateTime NextHeartbeat = DateTime.Now;
        private void HbIdle()
        {
            if (DateTime.Now >= NextHeartbeat)
            {
                NextHeartbeat = DateTime.Now.AddSeconds(1);
                Can.Transmit(new CanMessage(0x70E, new byte[] { 5, 4 }));

                if (!RemoteTime)
                {
                    long ms = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    var b = BitConverter.GetBytes(ms);

                    Can.Transmit(new CanMessage(0x100, new byte[]
                    {
                        b[0], b[1], b[2], b[3], b[4], b[5], 0x00, 0x08
                    }));
                }
            }
        }

        
        

        private readonly Dictionary<uint, Module> Modules = new();

        private void WritebackLoop()
        {
            while (true)
            {
                try { 
                foreach (var kv in Modules)
                {
                    uint moduleId = kv.Key;
                    Module module = kv.Value;

                    ProcessWriteSet(moduleId, module);
                }
                }
                catch
                {
                    // ignore
                }

                Thread.Sleep(20);
            }
        }

        private void ProcessWriteSet(uint moduleId, Module m)
        {

          //  Debug.WriteLine($"{m.Name} v:{m.Value} s:{m.Set.Value} w:{m.Set.Write} id: 0x{moduleId:X3}" ); 

            if (m.Set?.Write == null)
                return;

            try
            {
                double? desired = null;
                double actual = 0;

                // value

                if (m.Write != null)
                {
                    desired = m.Write;
                    actual = m.Value;

                    if (double.IsNaN((double)desired))
                        return;
                    if (Math.Abs((double)desired - actual) > 0.0001)
                        SendWritePDO(moduleId, (double)desired, 3);
                }

                //set value
                if (m.Set.Write != null)
                {
                    desired = m.Set.Write;
                    actual = m.Set.Value;

                    if (double.IsNaN((double)desired))
                        return;

                    if (Math.Abs((double)desired - actual) > 0.0001)
                        SendWritePDO(moduleId, (double)desired, 4);
                }

                //out value
                if (m.Out.Write != null)
                {
                    desired = m.Out.Write;
                    actual = m.Out.Value;

                    if (double.IsNaN((double)desired))
                        return;
                    if (Math.Abs((double)desired - actual) > 0.0001)
                        SendWritePDO(moduleId, (double)desired, 5);
                }
            }
            catch
            {
                // ignore
            }
        }

        private uint GetWriteIdFromModule(uint moduleId)
        {
            uint baseId = (moduleId >> 4) & 0x7F; // richtige Modulnummer
            return 0x500 | baseId;
        }


        private void SendWritePDO(uint moduleId, double value, int func)
        {
            uint writeId = GetWriteIdFromModule(moduleId);

            Debug.WriteLine($"write id: {writeId:X3} 0x{moduleId:X3}");


            byte[] data = new byte[8];

            // float32
            Array.Copy(BitConverter.GetBytes((float)value), 0, data, 0, 4);

            // metadata
            data[4] = 0;
            data[5] = 0;

            // function = 4 (set)
            data[6] = 4;

            // subchannel
            data[7] = 0;

            Can.Transmit(new CanMessage(writeId, data));
        }


    }
}
