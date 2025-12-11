using FunkySystem.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunkySystem.Battery
{
    internal class ControlOverview : FunkyDeviceControl
    {
        public ControlOverview()
        {
            HideStatePanel();
            HideControlPanel();
            HideSequencePanel();

        }
    }
}
