using FunkySystem.Net.CAN;
using FunkySystem.Signals;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunkySystem.Core
{
    public static class SignalMapper
    {
        public static readonly Dictionary<string, BaseSignalCommon> MappedSignals = new();

        public static BitSignal MapBitSignal(string source, string name, int bit, string text = null)
        {
            string key = $"{source}.{name}";
       
                var bitSignal = SignalPool.GetBit(key, 0);
                if(text != null)
                {
                    bitSignal.Text = text;
                }
               
                return bitSignal;
            
        }

    }
}
