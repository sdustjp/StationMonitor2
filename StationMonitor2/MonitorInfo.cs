using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {

        public class MonitorInfo : Dictionary<string, InfoItem>
        {
            public void Add(InfoItem item, params string[] keys) { foreach (string key in keys) Add(key, item); }
            public void Totalize(MonitorInfo infos) { foreach (KeyValuePair<string, InfoItem> item in this) { InfoItem info; if (infos.TryGetValue(item.Key, out info)) item.Value.Totalize(info); } }
        }

        public abstract class InfoItem
        {
            public abstract void Totalize(InfoItem info);
            public abstract string ToText(MonitorContent content);
            public void AppendString(StringBuilder sb, MonitorContent content) { sb.Append(ToText(content)); }
            public static string BoolToText(bool value, string trueText, string falseText, MonitorContent content)
            {
                if (content.StrValues.Count > 1) return value ? content.StrValues[0] : content.StrValues[1];
                return value ? trueText : falseText;
            }
            public static string EnumToText(int value, string defaults, MonitorContent content)
            {
                if (value < content.StrValues.Count) return content.StrValues[value];
                return defaults;
            }
        }

        public class CommonInfo : InfoItem
        {
            public string Icon { get; set; } = "";
            public string Name { get; set; } = "";
            public int Index { get; set; } = 0;
            public string TypeName { get; set; } = "";
            public string SubtypeId { get; set; } = "";
            public bool IsBeingHacked { get; set; } = false;
            public bool IsFunctional { get; set; } = false;
            public bool IsWorking { get; set; } = false;
            public bool IsEnabled { get; set; } = false;
            public string Description { get; set; } = "";

            public CommonInfo() { }
            public CommonInfo(IMyFunctionalBlock block, int index = 0)
            {
                Index = index;
                Name = block.CustomName;
                SubtypeId = block.BlockDefinition.SubtypeId;
                IsBeingHacked = block.IsBeingHacked;
                IsFunctional = block.IsFunctional;
                IsWorking = block.IsWorking;
                IsEnabled = block.Enabled;
            }
            public CommonInfo(IMyTerminalBlock block, int index = 0)
            {
                Index = index;
                Name = block.CustomName;
                SubtypeId = block.BlockDefinition.SubtypeId;
                IsBeingHacked = block.IsBeingHacked;
                IsFunctional = block.IsFunctional;
                IsWorking = block.IsWorking;
                IsEnabled = true;
            }
            public CommonInfo(string name, int index = 0)
            {
                Index = index;
                Name = name;
            }

            public override void Totalize(InfoItem monitorInfo) { Totalize(monitorInfo as CommonInfo); }
            public void Totalize(CommonInfo info)
            {
                //Index = 0;
                //Name = "";
                //SubtypeId = block.BlockDefinition.SubtypeId;
                IsBeingHacked = IsBeingHacked || info.IsBeingHacked;
                IsFunctional = IsFunctional || info.IsFunctional;
                IsWorking = IsWorking || info.IsWorking;
                IsEnabled = IsEnabled || info.IsEnabled;
            }
            public override string ToText(MonitorContent content)
            {
                switch (content.Subtype)
                {
                    case "index": return Index.ToString();
                    case "name": return Name;
                    case "hack": return BoolToText(IsBeingHacked, "Hacked", "Not Hacked", content);
                    case "func": return BoolToText(IsFunctional, "Functional", "Breaking", content);
                    case "work": return BoolToText(IsWorking, "Working", "Not Working", content);
                    case "enable": return BoolToText(IsEnabled, "Enabled", "Disabled", content);
                    default: return "";
                }
            }
        }

        public abstract class ValueInfoBase : InfoItem
        {
            public float Current { get; set; } = 0;
            public float Max { get; set; } = 0;
            public float Ratio { get { return (Current / Max); } }
            public string Description { get; protected set; } = "";
            public string DefaultUnit { get; set; } = "";

            public ValueInfoBase(string description = "")
            {
                Description = description;
            }

            public override void Totalize(InfoItem info) { Totalize(info as ValueInfoBase); }
            public void Totalize(ValueInfoBase info)
            {
                Current += info.Current;
                Max += info.Max;
            }
            public override string ToText(MonitorContent content)
            {
                float current = Current;
                float max = Max;
                string unit = DefaultUnit;
                switch (content.Subtype)
                {
                    case "current": case "cur": return $"{current:F2} {unit}";
                    case "max": return $"{max:F2} {unit}";
                    case "rate": return $"{current:F2} / {max:F2} {unit}";
                    case "percent": case "p": return $"{Ratio:P2}";
                    case "desc": return Description;
                    default: return "";
                }
            }
        }

        public class Store : ValueInfoBase
        {
            public float Mass { get; set; } = 0;

            public Store(string description = "") : base(description)
            {
                DefaultUnit = "kL";
            }

            public Store(IMyEntity entity, string description = "") : base(description)
            {
                for (int i = 0; i < entity.InventoryCount; i++)
                {
                    IMyInventory inv = entity.GetInventory(i);

                    Mass += (float)inv.CurrentMass;
                    Max += (float)inv.MaxVolume;
                    Current += (float)inv.CurrentVolume;
                    DefaultUnit = "kL";
                }
            }

            public Store(IMyEntity entity, MyItemType itemType, string description = "") : base(description)
            {
                for (int i = 0; i < entity.InventoryCount; i++)
                {
                    IMyInventory inv = entity.GetInventory(i);

                    MyItemInfo itemInfo = itemType.GetItemInfo();
                    float amount = (float)inv.GetItemAmount(itemType);

                    Max += (float)inv.MaxVolume;
                    DefaultUnit = "kL";

                    if (itemInfo.UsesFractions)
                    {
                        Mass += amount;
                        Current += itemInfo.Volume * amount;
                    }
                    else
                    {
                        Mass += itemInfo.Mass * amount;
                        Current += itemInfo.Volume * amount;
                    }
                }
            }

            public Store(IMyInventory inv, string description = "") : base(description)
            {
                Mass = (float)inv.CurrentMass;
                Max = (float)inv.MaxVolume;
                Current = (float)inv.CurrentVolume;
                DefaultUnit = "kL";
            }

            public Store(IMyInventory inv, MyItemType itemType, string description = "") : base(description)
            {
                MyItemInfo itemInfo = itemType.GetItemInfo();
                float amount = (float)inv.GetItemAmount(itemType);

                Max += (float)inv.MaxVolume;
                DefaultUnit = "kL";

                if (itemInfo.UsesFractions)
                {
                    Mass = amount;
                    Current = itemInfo.Volume * amount;
                }
                else
                {
                    Mass = itemInfo.Mass * amount;
                    Current = itemInfo.Volume * amount;
                }
            }

            public Store(IMyGasTank gasTank, string description = "") : base(description)
            {
                // <Capacity>
                switch (gasTank.BlockDefinition.SubtypeId)
                {
                    case "OxygenTankSmall": Max = 50.000f; break;
                    case "LargeHydrogenTank": Max = 5000.000f; break;
                    case "SmallHydrogenTank": Max = 160.000f; break;
                    default: Max = 100.000f; break; // LargeOxygenTank
                }
                Current = Max * (float)gasTank.FilledRatio;
                DefaultUnit = "kL";
            }

            public Store(IMyPowerProducer producer, string description = "") : base(description)
            {
                if (producer.BlockDefinition.SubtypeId.Contains("HydrogenEngine"))
                {
                    var textLines = producer.DetailedInfo.Split('\n');

                    foreach (string text in textLines)
                    {
                        if (text.Contains("Filled:"))
                        {
                            int i = 0, j;
                            j = text.IndexOf('(', i);
                            if (j == -1) return;
                            i = j + 1;

                            j = text.IndexOf('L', i);
                            if (j == -1) return;

                            Current = float.Parse(text.Substring(i, j - i)) * 0.001f;
                            i = j + 2;  // "L/"

                            j = text.IndexOf('L', i);
                            if (j == -1) return;

                            Max = float.Parse(text.Substring(i, j - i)) * 0.001f;

                            return;
                        }
                    }

                    DefaultUnit = "kL";
                }
            }

            public override void Totalize(InfoItem info) { Totalize(info as Store); }
            public void Totalize(Store info)
            {
                base.Totalize(info);
                Mass += info.Mass;
            }

            public override string ToText(MonitorContent content)
            {
                switch (content.Subtype)
                {
                    case "mass": return $"{Mass:F2} kg";
                    default: return base.ToText(content);
                }
            }
        }

        public class VentInfo : ValueInfoBase
        {
            public bool CanPressurize { get; set; } = false;
            public bool Depressurize { get; set; } = false;
            public VentStatus VentStatus { get; set; } = 0;
            public bool Pressurized { get; set; } = true;
            public bool Depressurized { get; set; } = true;
            public bool PressurizationEnabled { get; set; } = false;

            public VentInfo(string description = "") : base(description) { }
            public VentInfo(IMyAirVent airVent, string description = "") : base(description)
            {
                Current = airVent.GetOxygenLevel();
                Max = 1.0f;

                CanPressurize = airVent.CanPressurize;
                Depressurize = airVent.Depressurize;
                VentStatus = airVent.Status;
                Pressurized = (airVent.Status == VentStatus.Pressurized);
                Depressurized = (airVent.Status == VentStatus.Depressurized);
                PressurizationEnabled = airVent.PressurizationEnabled;
            }

            public override void Totalize(InfoItem info) { Totalize(info as VentInfo); }
            public void Totatilze(VentInfo info)
            {
                base.Totalize(info);
                CanPressurize = CanPressurize || info.CanPressurize;
                Pressurized = Pressurized && info.Pressurized;
                Depressurized = Depressurized && info.Depressurized;
                PressurizationEnabled = PressurizationEnabled || info.PressurizationEnabled;
            }
            public override string ToText(MonitorContent content)
            {
                switch (content.Subtype)
                {
                    case "stat": return EnumToText((int)VentStatus, VentStatus.ToString(), content);
                    case "pressurized": return BoolToText(Pressurized, "Pressurized", "", content);
                    case "depressurized": return BoolToText(Depressurized, "Depressurized", "", content);
                    case "can": return BoolToText(CanPressurize, "Can Pressurize", "", content);
                    case "mode": return BoolToText(Depressurize, "Depressurize", "Pressurize", content);
                    case "enable": return BoolToText(PressurizationEnabled, "Pressurization Enabled", "", content);
                    default: return base.ToText(content);
                }
            }
        }

        public class DoorInfo : ValueInfoBase
        {
            public DoorStatus DoorStatus { get; set; } = 0;
            public bool Open { get; set; } = true;
            public bool Close { get; set; } = true;

            public DoorInfo(string description = "") : base(description) { }
            public DoorInfo(IMyDoor door, string description = "") : base(description)
            {
                Current = door.OpenRatio;
                Max = 1.0f;
                DoorStatus = door.Status;
                Open = (door.Status == DoorStatus.Open);
                Close = (door.Status == DoorStatus.Closed);
            }

            public override void Totalize(InfoItem info) { Totalize(info as DoorInfo); }
            public void Totalize(DoorInfo info)
            {
                Open = Open && info.Open;
                Close = Close && info.Close;
            }
            public override string ToText(MonitorContent content)
            {
                switch (content.Subtype)
                {
                    case "stat": return EnumToText((int)DoorStatus, DoorStatus.ToString(), content);
                    case "open": return BoolToText(Open, "Open", "", content);
                    case "close": return BoolToText(Close, "Close", "", content);
                    default: return base.ToText(content);
                }
            }
        }

        public class ConnectorInfo : Store
        {
            public MyShipConnectorStatus ConnectorStatus { get; set; } = 0;
            public bool Connected { get; set; } = true;
            public bool Unconnected { get; set; } = true;
            public string ShipName { get; set; } = "";

            public ConnectorInfo(string description = "") : base(description) { }
            public ConnectorInfo(IMyShipConnector connector, string description = "") : base(connector, description)
            {
                ConnectorStatus = connector.Status;
                Connected = (connector.Status == MyShipConnectorStatus.Connected);
                Unconnected = (connector.Status == MyShipConnectorStatus.Unconnected);

                if (connector.Status == MyShipConnectorStatus.Connected)
                {
                    IMyShipConnector other = connector.OtherConnector;
                    ShipName = other.CubeGrid.CustomName;
                }
            }

            public override void Totalize(InfoItem info) { Totalize(info as ConnectorInfo); }
            public void Totalize(ConnectorInfo info)
            {
                base.Totalize(info);
                Connected = Connected && info.Connected;
                Unconnected = Unconnected && info.Unconnected;
            }
            public override string ToText(MonitorContent content)
            {
                switch (content.Subtype)
                {
                    case "stat": return EnumToText((int)ConnectorStatus, ConnectorStatus.ToString(), content);
                    case "connect": return BoolToText(Connected, "Connected", "", content);
                    case "unconnect": return BoolToText(Unconnected, "Unconnected", "", content);
                    case "ship": return ShipName;
                    default: return base.ToText(content);
                }
            }
        }
    }
}
