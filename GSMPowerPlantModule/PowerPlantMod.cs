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
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public class PowerPlantMod
        {
            public static void Load(StationMonitorCore sm)
            {
                MonitorBlockBuilder mbb = sm.MBBuilder;

                mbb["Reactor"] = (MonitorBlockParser parser) => {
                    return new MonitorBlock<IMyReactor>(parser)
                    {
                        InfoMaker = (IMyReactor block, int index) =>
                        {
                            return new MonitorInfo
                            {
                                { "block", new CommonInfo(block, index) { Icon = "PowerGen" } },
                                { "out", new Output(block, "Output") },
                                { "eff", new Efficiency(block, "Efficiency") },
                                { "sto", new Store(block, MyItemType.MakeIngot("Uranium"), "Stored Uranium") }
                            };
                        }
                    };
                };

                mbb["SolarPanel"] = (MonitorBlockParser parser) => {
                    return new MonitorBlock<IMySolarPanel>(parser)
                    {
                        InfoMaker = (IMySolarPanel block, int index) =>
                        {
                            return new MonitorInfo
                            {
                                { "block", new CommonInfo(block, index) { Icon = "PowerGen" } },
                                { "out", new Output(block, "Output") },
                                { "eff", new Efficiency(block, "Efficiency") }
                            };
                        }
                    };
                };

                mbb["Battery"] = (MonitorBlockParser parser) => {
                    return new MonitorBlock<IMyBatteryBlock>(parser)
                    {
                        InfoMaker = (IMyBatteryBlock block, int index) =>
                        {
                            return new MonitorInfo
                            {
                                { "block", new CommonInfo(block, index) { Icon = "PowerGen" } },
                                { "out", new Output(block, "Output") },
                                { "eff", new Efficiency(block, "Efficiency") },
                                { "in", new Input(block, "Input") },
                                { "cha", new Charge(block, "Charge") }
                            };
                        }
                    };
                };

                mbb["HydrogenEngine"] = (MonitorBlockParser parser) => {
                    return new MonitorBlock<IMyPowerProducer>(parser)
                    {
                        SubFilter = (IMyTerminalBlock block) =>
                        {
                            if (!block.BlockDefinition.SubtypeId.Contains("HydrogenEngine")) return false;
                            return true;
                        },

                        InfoMaker = (IMyPowerProducer block, int index) =>
                        {
                            return new MonitorInfo
                            {
                                { "block", new CommonInfo(block, index) { Icon = "PowerGen" } },
                                { "out", new Output(block, "Output") },
                                { "eff", new Efficiency(block, "Efficiency") },
                                { "sto", new Store(block, "Stored Hydrogen") }
                            };
                        }
                    };
                };

                mbb["WindTurbine"] = (MonitorBlockParser parser) => {
                    return new MonitorBlock<IMyPowerProducer>(parser)
                    {
                        SubFilter = (IMyTerminalBlock block) =>
                        {
                            if (!block.BlockDefinition.SubtypeId.Contains("WindTurbine")) return false;
                            return true;
                        },

                        InfoMaker = (IMyPowerProducer block, int index) =>
                        {
                            return new MonitorInfo
                            {
                                { "block", new CommonInfo(block, index) { Icon = "PowerGen" } },
                                { "out", new Output(block, "Output") },
                                { "eff", new Efficiency(block, "Efficiency") }
                            };
                        }
                    };
                };

                mbb["PowerProducer"] = (MonitorBlockParser parser) => {
                    return new MonitorBlock<IMyPowerProducer>(parser)
                    {
                        InfoMaker = (IMyPowerProducer block, int index) =>
                        {
                            return new MonitorInfo
                            {
                                { "block", new CommonInfo(block, index) { Icon = "PowerGen" } },
                                { "out", new Output(block, "Output") }
                            };
                        }
                    };
                };

                mbb["PowerGenerator"] = (MonitorBlockParser parser) => {
                    return new MonitorBlock<IMyPowerProducer>(parser)
                    {
                        SubFilter = (IMyTerminalBlock block) =>
                        {
                            if (block.BlockDefinition.SubtypeId.Contains("Battery")) return false;
                            return true;
                        },

                        InfoMaker = (IMyPowerProducer block, int index) =>
                        {
                            return new MonitorInfo
                            {
                                { "block", new CommonInfo(block, index) { Icon = "PowerGen" } },
                                { "out", new Output(block, "Output") }
                            };
                        }
                    };
                };

            }
        }

        public abstract class PowerIO : ValueInfoBase
        {
            public PowerIO(string description = "") : base(description)
            {
                DefaultUnit = "MW";
            }
        }

        public class Output : PowerIO
        {
            public Output(string description = "") : base(description) { }
            public Output(IMyPowerProducer powerProcuder, string description = "") : base(description)
            {
                Current = powerProcuder.CurrentOutput;
                Max = powerProcuder.MaxOutput;
            }
        }

        public class Efficiency : PowerIO
        {
            public Efficiency(string description = "") : base(description) { }
            public Efficiency(IMyPowerProducer powerProcuder, string description = "") : base(description)
            {

                Current = powerProcuder.MaxOutput;

                // <MaxPowerOutput>
                switch (powerProcuder.BlockDefinition.SubtypeId)
                {
                    case "LargeBlockBatteryBlock": Max = 12.0f; break;
                    case "SmallBlockBatteryBlock": Max = 4.0f; break;
                    case "SmallBlockSmallBatteryBlock": Max = 0.05f; break;
                    case "SmallBlockSmallGenerator": Max = 0.5f; break;
                    case "SmallBlockLargeGenerator": Max = 14.75f; break;
                    case "LargeBlockSmallGenerator": Max = 15.0f; break;
                    case "LargeBlockLargeGenerator": Max = 300.0f; break;
                    case "LargeHydrogenEngine": Max = 5.0f; break;
                    case "SmallHydrogenEngine": Max = 0.5f; break;
                    case "LargeBlockSolarPanel": Max = 0.16f; break;
                    case "SmallBlockSolarPanel": Max = 0.04f; break;
                    case "LargeBlockWindTurbine": Max = 0.4f; break;
                }

            }
        }

        public class Charge : ValueInfoBase
        {
            public bool IsCharnging { get; set; } = false;
            public ChargeMode ChargeMode { get; set; } = ChargeMode.Auto;

            public Charge(string description = "") : base(description)
            {
                DefaultUnit = "MWh";
            }
            public Charge(IMyBatteryBlock battery, string description = "") : base(description)
            {
                Current = battery.CurrentStoredPower;
                Max = battery.MaxStoredPower;
                IsCharnging = battery.IsCharging;
                ChargeMode = battery.ChargeMode;
                DefaultUnit = "MWh";
            }

            public override string ToText(MonitorContent content)
            {
                switch (content.Subtype)
                {
                    case "charging":
                        if (content.StrValues.Count > 1) return IsCharnging ? content.StrValues[0] : content.StrValues[1];
                        else return IsCharnging ? "Charging" : "";
                    case "mode": return ChargeMode.ToString();
                    default: return base.ToText(content);
                }
            }
        }

        public class Input : PowerIO
        {
            public Input(string description = "") : base(description) { }
            public Input(IMyBatteryBlock battery, string description = "") : base(description)
            {
                Current = battery.CurrentInput;
                Max = battery.MaxInput;
            }
        }

    }
}
