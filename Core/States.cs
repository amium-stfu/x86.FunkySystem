using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace FunkySystem.Core
{
    public class FaIconAttribute : Attribute
    {
        public string Value { get; }

        public FaIconAttribute(string value)
        {
            Value = value;
        }
    }
    public enum State
    {
        [Description("Off")]
        [FaIcon("fa:power-off:black")]
        Off = 0,

        [Description("Done")]
        [FaIcon("fa:circle-check:black")]
        Done = 1,

        [Description("Ready")]
        [FaIcon("fa:circle-stop:black")]
        Ready = 2,

        [Description("Initialize")]
        [FaIcon("fa:circle-dot:black")]
        Init = 3,

        [Description("Running")]
        [FaIcon("fa:circle-play:black")]
        Running = 4,

        [Description("Pause")]
        [FaIcon("fa:circle-pause:black")]
        Pause = 5,

        [Description("Info")]
        [FaIcon("fa:circle-info:black")]
        Info = 6,

        [Description("Warning")]
        [FaIcon("fa:circle-exclamation:orange")]
        Warning = 11,

        [Description("Alert")]
        [FaIcon("fa:circle-exclamation:red")]
        Alert = 12,

        [Description("Aborted")]
        [FaIcon("fa:ban:red")]
        Aborted = 13,

    }


    public static class EnumExtensions
    {
        private static T? GetAttribute<T>(this Enum value) where T : Attribute
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            if (name == null)
                return null;

            var field = type.GetField(name);
            return field?.GetCustomAttribute<T>();
        }

        public static string ToDescriptionString(this Enum value)
        {
            var attr = value.GetAttribute<DescriptionAttribute>();
            return attr?.Description ?? value.ToString();
        }

        public static string ToFaIcon(this Enum value)
        {
            var attr = value.GetAttribute<FaIconAttribute>();
            return attr?.Value ?? string.Empty; // oder Default-Icon
        }
    }
}
