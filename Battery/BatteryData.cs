using FunkySystem.Core;
using FunkySystem.Helpers;
using FunkySystem.Signals;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Documents;
using static FunkySystem.Devices.MainData;

namespace FunkySystem.Devices
{
    public class BatteryData
    {

        [JsonIgnore]
        public string? Uri;

        public MainData Main
        {
            get; set;
        }

        public PhysicalSpecificationsData PhysicalSpecifications { get; set; }
        public InnerCellMaterialsData InnerCellMaterials { get; set; }
        public ThermalPropertiesData ThermalProperties { get; set; }
        public ElectricalPropertiesData ElectricalProperties { get; set; }

        string Id { get; set; }

        public BatteryData(string id)
        {
            Id = id;
            Main = new MainData(Id + ".Main");
            PhysicalSpecifications = new PhysicalSpecificationsData(Id + ".PhysicalSpecifications");
            InnerCellMaterials = new InnerCellMaterialsData(Id + ".InnerCellMaterials");
            ThermalProperties = new ThermalPropertiesData(Id + ".ThermalProperties");
            ElectricalProperties = new ElectricalPropertiesData(Id + ".ElectricalProperties");
        }
        public void SetBattery(string id)
        {
            Main.Serielnumber.Value = id;
            LoadFromJson(sender: "Battery.SetBattery");
        }
        private void UpdateUri(string? uri = null)
        {
            Uri = uri ?? Path.Combine(Program.SettingsDirectory, "Battery", Main.Serielnumber.Value, "Battery.data");
        }

        public void Export()
        {
            string? directoryPath = Path.GetDirectoryName(Uri);
            if (directoryPath == null)
            {
                Logger.FatalMsg("Export Battery Data failed: invalid directory path.");
                return;
            }
            if (!Directory.Exists(directoryPath)) 
                Directory.CreateDirectory(directoryPath);

        }

        public string Load()
        {
            string uri ="";
            using (System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Path.Combine(Program.SettingsDirectory, "Battery");
                openFileDialog.ValidateNames = false;
                openFileDialog.CheckFileExists = false;
                openFileDialog.CheckPathExists = true;
                openFileDialog.FileName = "Battery.data";


                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string? selectedPath = Path.GetDirectoryName(openFileDialog.FileName);
                    if (selectedPath != null)
                    {
                        uri = openFileDialog.FileName;
                        LoadFromJson("Load Battery manual", openFileDialog.FileName);

                        string file = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                        Main.Id = file; 

                    }
                    else
                    {
                        Logger.FatalMsg("Load Battery Data failed: selected path is null.");
                    }
                }
            }
            return uri;
        }

        public void Refresh()
        {
            string uri = Path.Combine(Program.SettingsDirectory, "Battery", Main.Serielnumber.Value, "Battery.data");
            LoadFromJson(uri);
        }
        public void Edit()
        {
            string uri = Path.Combine(Program.SettingsDirectory, "Battery", Main.Serielnumber.Value, "Battery.data");
            System.Diagnostics.Process.Start("notepad.exe", uri);
        }

        public void LoadFromJson(string sender, string uri = null)
        {
            if (!File.Exists(uri)) return;

            Logger.InfoMsg(sender + " -> LoadBatteryData form json + " + uri);

            try
            {
                if (uri == null) UpdateUri(uri); else Uri = uri;
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                string json = File.ReadAllText(Uri);
                var data = JsonSerializer.Deserialize<Dictionary<string, List<JsonValue>>>(json, options);

                // MAIN
                Main.Partnumber.Value = GetJsonValue(data, "Main", "Partnumber", "Value");
                Main.Serielnumber.Value = GetJsonValue(data, "Main", "Serielnumber", "Value");
                Main.CellType.Value = GetJsonValue(data, "Main", "CellType", "Value");
                Main.Capacity.Value = GetJsonValue(data, "Main", "Capacity", "Value").ToDouble();
                Main.NominalVoltage.Value = GetJsonValue(data, "Main", "NominalVoltage", "Value").ToDouble();
                Main.CutoffCharge.Value = GetJsonValue(data, "Main", "CutoffCharge", "Value").ToDouble();
                Main.CutoffDischarge.Value = GetJsonValue(data, "Main", "CutoffDischarge", "Value").ToDouble();
                Main.UpperOvershoot.Value = GetJsonValue(data, "Main", "UpperOvershoot", "Value").ToDouble();
                Main.LowerOvershoot.Value = GetJsonValue(data, "Main", "LowerOvershoot", "Value").ToDouble();
                Main.SetTemperature.Value = GetJsonValue(data, "Main", "SetTemperature", "Value")
                                                .Replace("°C", "").ToDouble();

                // PHYSICAL SPECIFICATIONS
                PhysicalSpecifications.Width.Value =
                    GetJsonValue(data, "PhysicalSpecifications", "Width", "Value").ToDouble();
                PhysicalSpecifications.Thickness.Value =
                    GetJsonValue(data, "PhysicalSpecifications", "Thickness", "Value").ToDouble();
                PhysicalSpecifications.Height.Value =
                    GetJsonValue(data, "PhysicalSpecifications", "Height", "Value").ToDouble();
                PhysicalSpecifications.Weight.Value =
                    GetJsonValue(data, "PhysicalSpecifications", "Weight", "Value").ToDouble();
                PhysicalSpecifications.Density.Value =
                    GetJsonValue(data, "PhysicalSpecifications", "Density", "Value").ToDouble();

                // INNER CELL MATERIALS
                InnerCellMaterials.Anode.Value =
                    GetJsonValue(data, "InnerCellMaterials", "Anode", "Value");
                InnerCellMaterials.Cathode.Value =
                    GetJsonValue(data, "InnerCellMaterials", "Cathode", "Value");
                InnerCellMaterials.Seperator.Value =
                    GetJsonValue(data, "InnerCellMaterials", "Seperator", "Value");
                InnerCellMaterials.ElectrodeCeramicCoating.Value =
                    GetJsonValue(data, "InnerCellMaterials", "ElectrodeCeramicCoating", "Value");
                InnerCellMaterials.Electrolyte.Value =
                    GetJsonValue(data, "InnerCellMaterials", "Electrolyte", "Value");

                // THERMAL PROPERTIES
                ThermalProperties.OperationMin.Value =
                    GetJsonValue(data, "ThermalProperties", "OperationMin", "Value").ToDouble();
                ThermalProperties.OperationMax.Value =
                    GetJsonValue(data, "ThermalProperties", "OperationMax", "Value").ToDouble();
                ThermalProperties.StorageMin.Value =
                    GetJsonValue(data, "ThermalProperties", "StorageMin", "Value").ToDouble();
                ThermalProperties.StorageMax.Value =
                    GetJsonValue(data, "ThermalProperties", "StorageMax", "Value").ToDouble();
                ThermalProperties.SafetyMin.Value =
                    GetJsonValue(data, "ThermalProperties", "SafetyMin", "Value").ToDouble();
                ThermalProperties.SafetyMax.Value =
                    GetJsonValue(data, "ThermalProperties", "SafetyMax", "Value").ToDouble();

                // ELECTRICAL PROPERTIES
                // Hier werden Name + Unit kombiniert, da Upper/LowerOvershotLimit mehrfach vorkommt.
                ElectricalProperties.UpperOvershotLimitVoltage.Value =
                    GetJsonValue(data, "ElectricalProperties", "UpperOvershotLimit", "Value", "V").ToDouble();
                ElectricalProperties.UpperOvershotLimitTime.Value =
                    GetJsonValue(data, "ElectricalProperties", "UpperOvershotLimit", "Value", "s").ToDouble();
                ElectricalProperties.UpperOvershotLimitOccurences.Value =
                    GetJsonValue(data, "ElectricalProperties", "UpperOvershotLimit", "Value", "Occurences").ToDouble();

                ElectricalProperties.LowerOvershotLimitVoltage.Value =
                    GetJsonValue(data, "ElectricalProperties", "LowerOvershotLimit", "Value", "V").ToDouble();
                ElectricalProperties.LowerOvershotLimitTime.Value =
                    GetJsonValue(data, "ElectricalProperties", "LowerOvershotLimit", "Value", "s").ToDouble();
                ElectricalProperties.LowerOvershotLimitOccurences.Value =
                    GetJsonValue(data, "ElectricalProperties", "LowerOvershotLimit", "Value", "Occurences").ToDouble();
            }
            catch (Exception ex)
            {
                Logger.FatalMsg(sender + " -> Battery Load error " + Uri, ex);
            }
        }

        public string GetJsonValue(
    Dictionary<string, List<JsonValue>> data,
    string groupName,
    string name,
    string parameter,
    string unitFilter = null)
        {
            if (!data.ContainsKey(groupName))
                return "NA";

            List<JsonValue> list = data[groupName];

            JsonValue? searchValue = null;
            foreach (JsonValue val in list)
            {
                if (val.Name == name &&
                    (unitFilter == null ||
                     string.Equals(val.Unit, unitFilter, StringComparison.OrdinalIgnoreCase)))
                {
                    searchValue = val;
                    break;
                }
            }

            if (searchValue == null)
                return "NA";

            string result = parameter switch
            {
                "Type" => searchValue.Type ?? "NA",
                "Value" => searchValue.Value ?? "NA",
                "Unit" => searchValue.Unit ?? "NA",
                _ => throw new ArgumentException($"Parameter '{parameter}' is not valid.")
            };

            Debug.WriteLine("result = " + result);
            return result;
        }

    }

    public class MainData
    {

        public string Id { get; set; }

        public StringSignal Partnumber;
        public StringSignal Serielnumber;
        public StringSignal CellType;
        public Signal Capacity;
        public Signal NominalVoltage;
        public Signal CutoffCharge;
        public Signal CutoffDischarge;
        public Signal UpperOvershoot;
        public Signal LowerOvershoot;
        public Signal SetTemperature;

        public MainData(string id)
        {
            Id = id;
            Partnumber = new StringSignal($"{Id}.Partnumber", text: "Partnumber", register: true);
            Serielnumber = new StringSignal($"{Id}.Serielnumber", text: "Serielnumber", register: true);
            CellType = new StringSignal($"{Id}.CellType", text: "Cell Type", register: true);
            Capacity = new Signal($"{Id}.Capacity", text: "Capacity", unit: "Ah", format: "0.00", register: true);
            NominalVoltage = new Signal($"{Id}.NominalVoltage", text: "NominalVoltage", unit: "V", format: "0.00", register: true);
            CutoffCharge = new Signal($"{Id}.CutoffCharge", text: "CutoffCharge", unit: "V", format: "0.00", register: true);
            CutoffDischarge = new Signal($"{Id}.CutoffDischarge", text: "CutoffDischarge", unit: "V", format: "0.00", register: true);
            UpperOvershoot = new Signal($"{Id}.UpperOvershoot", text: "UpperOvershoot", unit: "V", format: "0.00", register: true);
            LowerOvershoot = new Signal($"{Id}.LowerOvershoot", text: "LowerOvershoot", unit: "V", format: "0.00", register: true);
            SetTemperature = new Signal($"{Id}.SetTemperature", text: "SetTemperature", unit: "°C", format: "0.00", register: true);
        }

        public class PhysicalSpecificationsData
        {
            public string Id { get; set; }

            public Signal Width;
            public Signal Thickness;
            public Signal Height;
            public Signal Weight;
            public Signal Density;

            public PhysicalSpecificationsData(string id)
            {
                Id = id;
                Width = new Signal($"{Id}.Width", "Width", "mm", "0.00", register: true);
                Thickness = new Signal($"{Id}.Thickness", "Thickness", "mm", "0.00", register: true);
                Height = new Signal($"{Id}.Height", "Height", "mm", "0.00", register: true);
                Weight = new Signal($"{Id}.Weight", "Weight", "g", "0.0", register: true);
                Density = new Signal($"{Id}.Density", "Density", "Wh/kg", "0.0", register: true);
            }
        }

        public class InnerCellMaterialsData
        {
            public string Id { get; set; }

            public StringSignal Anode;
            public StringSignal Cathode;
            public StringSignal Seperator;
            public StringSignal ElectrodeCeramicCoating;
            public StringSignal Electrolyte;

            public InnerCellMaterialsData(string id)
            {
                Id = id;
                Anode = new StringSignal($"{Id}.Anode", text: "Anode", register: true);
                Cathode = new StringSignal($"{Id}.Cathode", text: "Cathode", register: true);
                Seperator = new StringSignal($"{Id}.Seperator", text: "Seperator", register: true);
                ElectrodeCeramicCoating = new StringSignal($"{Id}.ElectrodeCeramicCoating", text: "Electrode ceramic coating", register: true);
                Electrolyte = new StringSignal($"{Id}.Electrolyte", text: "Electrolyte", register: true);
            }
        }

        public class ThermalPropertiesData
        {
            public string Id { get; set; }

            public Signal OperationMin;
            public Signal OperationMax;
            public Signal StorageMin;
            public Signal StorageMax;
            public Signal SafetyMin;
            public Signal SafetyMax;

            public ThermalPropertiesData(string id)
            {
                Id = id;
                OperationMin = new Signal($"{Id}.OperationMin", "Operation min", "°C", "0.0", register: true);
                OperationMax = new Signal($"{Id}.OperationMax", "Operation max", "°C", "0.0", register: true);
                StorageMin = new Signal($"{Id}.StorageMin", "Storage min", "°C", "0.0", register: true);
                StorageMax = new Signal($"{Id}.StorageMax", "Storage max", "°C", "0.0", register: true);
                SafetyMin = new Signal($"{Id}.SafetyMin", "Safety min", "°C", "0.0", register: true);
                SafetyMax = new Signal($"{Id}.SafetyMax", "Safety max", "°C", "0.0", register: true);
            }
        }

        public class ElectricalPropertiesData
        {
            public string Id { get; set; }

            public Signal UpperOvershotLimitVoltage;
            public Signal UpperOvershotLimitTime;
            public Signal UpperOvershotLimitOccurences;

            public Signal LowerOvershotLimitVoltage;
            public Signal LowerOvershotLimitTime;
            public Signal LowerOvershotLimitOccurences;

            public ElectricalPropertiesData(string id)
            {
                Id = id;

                UpperOvershotLimitVoltage = new Signal(
                    $"{Id}.UpperOvershotLimitVoltage", "Upper overshot limit voltage", "V", "0.00", register: true);
                UpperOvershotLimitTime = new Signal(
                    $"{Id}.UpperOvershotLimitTime", "Upper overshot limit time", "s", "0.0", register: true);
                UpperOvershotLimitOccurences = new Signal(
                    $"{Id}.UpperOvershotLimitOccurences", "Upper overshot limit occurences", "", "0", register: true);

                LowerOvershotLimitVoltage = new Signal(
                    $"{Id}.LowerOvershotLimitVoltage", "Lower overshot limit voltage", "V", "0.00", register: true);
                LowerOvershotLimitTime = new Signal(
                    $"{Id}.LowerOvershotLimitTime", "Lower overshot limit time", "s", "0.0", register: true);
                LowerOvershotLimitOccurences = new Signal(
                    $"{Id}.LowerOvershotLimitOccurences", "Lower overshot limit occurences", "", "0", register: true);
            }
        }




    }
    public class JsonValue
    {
        public string Type
        {
            get; set;
        } = "";

        public string Name
        {
            get; set;
        } = "";
        public string Value
        {
            get; set;
        } = "";
        public string Unit
        {
            get; set;
        } = "";
        public List<List<string>> TableRaw
        {
            get; set;
        } = new();

        public JsonValue()
        {

        }
    }
    public class MapData
    {
        public List<double> XValues
        {
            get; set;
        } = new();
        public List<double> YValues
        {
            get; set;
        } = new();
        public List<List<double>> Values
        {
            get; set;
        } = new();

        public void LoadFromCsv(string path)
        {
            var lines = File.ReadAllLines(path);
            XValues = lines[0].Split(',').Skip(1).Select(s => double.Parse(s.Trim(new[] {
                        '%', ' '
                    }), CultureInfo.InvariantCulture)).ToList();
            Values = new List<List<double>>();
            YValues = new List<double>();

            foreach (var line in lines.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var parts = line.Split(',');
                YValues.Add(double.Parse(parts[0].Replace("°C", "").Trim(), CultureInfo.InvariantCulture));
                Values.Add(parts.Skip(1).Select(p => double.Parse(p.Trim(), CultureInfo.InvariantCulture)).ToList());
            }
        }

        public void FromDictionary(Dictionary<string, object> read)
        {
            try
            {
                YValues = ((JsonElement)read["YValues"]).EnumerateArray().Select(e => e.GetDouble()).ToList();
                XValues = ((JsonElement)read["XValues"]).EnumerateArray().Select(e => e.GetDouble()).ToList();
                Values = Parse2DArray((JsonElement)read["Values"]);
            }
            catch (Exception ex)
            {
                Logger.FatalMsg($"Error converting MapData",ex);
            }
        }

        private List<List<double>> Parse2DArray(JsonElement array)
        {
            var result = new List<List<double>>();
            foreach (var row in array.EnumerateArray())
            {
                var values = row.EnumerateArray().Select(v => v.GetDouble()).ToList();
                result.Add(values);
            }
            return result;
        }
        public double Interpolate(double x, double y)
        {

            if (XValues.Count == 0 || YValues.Count == 0 || Values.Count == 0)
                throw new InvalidOperationException("MapData has no values loaded.");


            double x0 = XValues.Where(v => v <= x).DefaultIfEmpty(XValues.First()).Max();
            double x1 = XValues.Where(v => v >= x).DefaultIfEmpty(XValues.Last()).Min();

            double y0 = YValues.Where(v => v <= y).DefaultIfEmpty(YValues.First()).Max();
            double y1 = YValues.Where(v => v >= y).DefaultIfEmpty(YValues.Last()).Min();

            int xi0 = XValues.FindIndex(v => Math.Abs(v - x0) < 1e-6);
            int xi1 = XValues.FindIndex(v => Math.Abs(v - x1) < 1e-6);
            int yi0 = YValues.FindIndex(v => Math.Abs(v - y0) < 1e-6);
            int yi1 = YValues.FindIndex(v => Math.Abs(v - y1) < 1e-6);

            double v00 = Values[yi0][xi0];
            double v01 = Values[yi0][xi1];
            double v10 = Values[yi1][xi0];
            double v11 = Values[yi1][xi1];

            double Interp(double v0, double v1, double a0, double a1, double a) =>
                (Math.Abs(a1 - a0) < 1e-6) ? v0 : v0 + (v1 - v0) * (a - a0) / (a1 - a0);

            double vTop = Interp(v00, v01, x0, x1, x);
            double vBottom = Interp(v10, v11, x0, x1, x);
            return Interp(vTop, vBottom, y0, y1, y);
        }
    }
}
