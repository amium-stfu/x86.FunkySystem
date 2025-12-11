using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

namespace FunkySystem.Net.CAN
{
    public class Can
    {
        public delegate void OnMessageReceivedDelegate(uint id, byte dlc, byte[] data);
        public event OnMessageReceivedDelegate MessageReceived;

        private UdpClient udpClient;
        private Thread rxThread;
        private Thread txThread;

        private readonly Queue<CanMessage> txBuffer = new();

        public Can(string ip, int port)
        {
            udpClient = new UdpClient();
            udpClient.Connect(ip, port);

            rxThread = new Thread(RxLoop)
            {
                IsBackground = true,
                Priority = ThreadPriority.Highest
            };
            rxThread.Start();

            txThread = new Thread(TxLoop)
            {
                IsBackground = true
            };
            txThread.Start();
        }

        public void Close()
        {
            try { rxThread?.Abort(); } catch { }
            try { txThread?.Abort(); } catch { }
            try { udpClient?.Close(); } catch { }
        }

        public void Transmit(CanMessage msg)
        {
            lock (txBuffer)
            {
                txBuffer.Enqueue(msg);
            }
        }

        //---------------------------------------------------------
        // TX LOOP (PCAN-UDP kompatibel)
        //---------------------------------------------------------
        private void TxLoop()
        {
            byte[] bytes = new byte[1500];
            int packageCounter = 0;

            while (true)
            {
                if (txBuffer.Count > 0)
                {
                    try
                    {
                        bytes[0] = (byte)(packageCounter >> 24);
                        bytes[1] = (byte)(packageCounter >> 16);
                        bytes[2] = (byte)(packageCounter >> 8);
                        bytes[3] = (byte)(packageCounter >> 0);
                        int offset = 4;

                        while (txBuffer.Count > 0 && offset < 1450)
                        {
                            CanMessage msg;
                            lock (txBuffer)
                                msg = txBuffer.Dequeue();

                            long timestamp = DateTime.Now.Ticks;

                            bytes[offset + 0] = (byte)(msg.Data.Length + 12);
                            bytes[offset + 1] = (byte)(timestamp >> 24);
                            bytes[offset + 2] = (byte)(timestamp >> 16);
                            bytes[offset + 3] = (byte)(timestamp >> 8);
                            bytes[offset + 4] = (byte)(timestamp >> 0);
                            bytes[offset + 5] = 0;
                            bytes[offset + 6] = 0;
                            bytes[offset + 7] = (byte)(msg.Id >> 24);
                            bytes[offset + 8] = (byte)(msg.Id >> 16);
                            bytes[offset + 9] = (byte)(msg.Id >> 8);
                            bytes[offset + 10] = (byte)(msg.Id >> 0);
                            bytes[offset + 11] = (byte)(msg.Data.Length);

                            for (int i = 0; i < msg.Data.Length; i++)
                                bytes[offset + 12 + i] = msg.Data[i];

                            offset += 12 + msg.Data.Length;
                        }

                        packageCounter++;
                        udpClient.Send(bytes, offset);
                    }
                    catch { }
                }

                Thread.Sleep(10);
            }
        }

        //---------------------------------------------------------
        // RX LOOP (PCAN-UDP kompatibel)
        //---------------------------------------------------------
        private void RxLoop()
        {
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            byte[] tmpData = new byte[64];

            while (true)
            {
                try
                {
                    byte[] bytes = udpClient.Receive(ref sender);
                    int offset = 4;

                    while (bytes.Length >= offset + 12)
                    {
                        int totalLen = bytes[offset + 0];

                        uint id = (uint)(
                            (bytes[offset + 7] << 24) |
                            (bytes[offset + 8] << 16) |
                            (bytes[offset + 9] << 8) |
                            (bytes[offset + 10])
                        );

                        byte dlc = bytes[offset + 11];
                        if (dlc > 8) dlc = 8;

                        Array.Copy(bytes, offset + 12, tmpData, 0, dlc);

                        MessageReceived?.Invoke(id, dlc, tmpData);

                        offset += 12 + dlc;
                    }
                }
                catch
                {
                    Thread.Sleep(1);
                }
            }
        }
    }

}