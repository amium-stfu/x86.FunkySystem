using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunkySystem.Controls
{
    public class ControlRadioMenu : Panel
    {
       
        Dictionary<string, ControlMenuButton> buttons = new Dictionary<string, ControlMenuButton>();

        public ControlRadioMenu()
        {
            // Constructor logic here
        }

        void Select(string name)
        {
            foreach (var btn in buttons)
            {
                btn.Value.SetLedColor(btn.Key == name ? Color.Orange : Color.Transparent);
                btn.Value.Invalidate();
            }
        }

        public void PerfomClick(string name)
        {
            buttons[name]?.PerformLeftClick();
        }

        public void Add(string name, string icon, Action onClick)
        {
            buttons[name] = new ControlMenuButton(icon, onClick, this);
            buttons[name].LeftClicked += (s, e) => Select(name);

        }
        public class ControlMenuButton : ButtonWithIcon
        {
            public ControlMenuButton(string icon, Action onClick, Panel panel)
            {
                BorderColor = Color.Transparent;
                ButtonIcon = icon;
                ButtonText = "Aktion";
                Dock = DockStyle.Top;
                GridScale = 5;
                HoverColor = Color.Transparent;
                IconSizeFactor = 0.6D;
                LedMargin = 3;
                LedSelectColor = Color.Orange;
                LedSize = 10;
                LedVisible = true;
                Location = new Point(0, 420);
                Margin = new Padding(0);
                Name = "btnCamera";
                ShortcutText = "";
                SignalValue = "Unknown";
                Size = new Size(60, 60);
                TabIndex = 7;
                Text = "";
                LeftClicked += (s, e) => onClick();

                
                panel.Controls.Add(this);
            }
        }
    }

}
