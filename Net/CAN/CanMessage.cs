using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FunkySystem.Net.CAN
{
    public class CanMessage
    {
        public DateTime Date = DateTime.UtcNow;
        public UInt32 Id { get; set; }
        public byte[] Data = new byte[8];
        public CanMessage(UInt32 id, byte[] bytes)
        {
            Date = DateTime.UtcNow;
            Id = id;
            Data = bytes;
        }
        public override string ToString()
        {
            string text = Id.ToString("X4") + ":";
            for (int i = 0; i < 8; i++)
            {
                if (i < Data.Length)
                    text += " " + Data[i].ToString("X2");
                else
                    text += " --";
            }
            return text;
        }
    }
}
