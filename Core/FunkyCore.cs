using FunkySystem.Roslyn;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunkySystem.Core
{

    public static class Theme
    {
        public static Color FormBackColor = Color.White;

        public static Color PanelSplitterColor = Color.FromArgb(180, 180, 180);
        public static Color PanelBackColor = Color.FromArgb(180, 180, 180);
        public static Color ThumbColor = Color.FromArgb(180, 180, 180);
        public static Color BackColor = Color.FromArgb(230, 230, 230);
        public static Color GridBackColor = Color.FromArgb(220, 220, 220);
        public static Color GridForeColor = Color.Black;
        public static Color ButtonBackColor = Color.FromArgb(230, 230, 230);
        public static Color ButtonForeColor = Color.Black;
        public static Color ButtonIconColor = Color.FromArgb(100, 100, 100);
        public static Color StringInputColor = Color.FromArgb(200, 200, 200);
        public static Color TreeNodeSelectColor = Color.DarkOrchid;
        public static Color TreeNodeDefaultColor = Color.Black;

        public static Color IconColorBefore = Color.Black;
        public enum EditorTheme { Light, Dark }

        static EditorTheme current = EditorTheme.Light;
        public static EditorTheme Current
        {
            get => current;
            set
            {
                if (value == EditorTheme.Dark)
                    ApplyDark();
                else
                    ApplyLight();
                current = value;
            }

        }
        public static bool IsDark => Current == EditorTheme.Dark;

        static void ApplyDark()
        {

            IconColorBefore = ButtonIconColor;
            FormBackColor = Color.Black;
            PanelSplitterColor = Color.FromArgb(70, 70, 70);
            PanelBackColor = Color.FromArgb(60, 60, 60);
            ThumbColor = Color.FromArgb(100, 100, 100);
            BackColor = Color.FromArgb(40, 40, 40);
            GridForeColor = Color.FromArgb(220, 220, 220);
            GridBackColor = Color.FromArgb(70, 70, 70);
            ButtonBackColor = Color.FromArgb(40, 40, 40);
            ButtonForeColor = Color.FromArgb(190, 190, 190);
            ButtonIconColor = Color.FromArgb(150, 150, 150);
            StringInputColor = Color.FromArgb(40, 40, 40);
            TreeNodeSelectColor = Color.Plum;
            TreeNodeDefaultColor = Color.FromArgb(220, 220, 220);

        }
        static void ApplyLight()
        {
            IconColorBefore = ButtonIconColor;
            FormBackColor = Color.White;
            PanelSplitterColor = Color.FromArgb(180, 180, 180);
            PanelBackColor = Color.FromArgb(230, 230, 230);
            ThumbColor = Color.FromArgb(180, 180, 180);
            BackColor = Color.FromArgb(210, 210, 210);
            GridBackColor = Color.FromArgb(220, 220, 220);
            GridForeColor = Color.Black;
            ButtonBackColor = Color.FromArgb(230, 230, 230);
            ButtonForeColor = Color.Black;
            ButtonIconColor = Color.FromArgb(100, 100, 100);
            StringInputColor = Color.FromArgb(200, 200, 200);
            TreeNodeSelectColor = Color.DarkOrchid;
            TreeNodeDefaultColor = Color.Black;

        }

    }



    internal static class FunkyCore
    {
        public static RoslynService Roslyn { get; } = new RoslynService();
    }
}
