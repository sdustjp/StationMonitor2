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
        public class FactoryMod
        {
            public static Dictionary<string, string> OreNameTable = new Dictionary<string, string> { { "Stone", "Stone" }, { "Iron", "Iron Ore" }, { "Nickel", "Nickel Ore" }, { "Cobalt", "Cobalt Ore" }, { "Magnesium", "Magnesium Ore" }, { "Silicon", "Silicon Ore" }, { "Silver", "Silver Ore" }, { "Gold", "Gold Ore" }, { "Platinum", "Platinum Ore" }, { "Uranium", "Uranium Ore" }, { "Scrap", "Scrap Metal" }, { "Ice", "Ice" }, { "Organic", "Organic" } };
            public static Dictionary<string, string> IngotNameTable = new Dictionary<string, string> { { "Stone", "Gravel" }, { "Iron", "Iron Ingot" }, { "Nickel", "Nickel Ingot" }, { "Cobalt", "Cobalt Ingot" }, { "Magnesium", "Magnesium Powder" }, { "Silicon", "Silicon Wafer" }, { "Silver", "Silver Ingot" }, { "Gold", "Gold Ingot" }, { "Platinum", "Platinum Ingot" }, { "Uranium", "Uranium Ingot" } };
            public static void Load(StationMonitorCore sm)
            {
                MonitorBlockBuilder mbb = sm.MBBuilder;

                mbb["Container"] = (MonitorBlockParser parser) => {

                    MonitorOption option;
                    string targetIngot = null;
                    string targetOre = null;
                    string desc = "";
                    string name;

                    if (parser.Options.TryGetValue("ore", out option))
                    {
                        targetOre = option.StrValue;
                        if (OreNameTable.TryGetValue(targetOre, out name)) desc = $"Stored {name}";
                    }

                    if (parser.Options.TryGetValue("ingot", out option))
                    {
                        targetIngot = option.StrValue;
                        if (IngotNameTable.TryGetValue(targetIngot, out name)) desc = $"Stored {name}";
                    }

                    return new MonitorBlock<IMyCargoContainer>(parser)
                    {
                        InfoMaker = (IMyCargoContainer block, int index) =>
                        {
                            Store store = new Store();
                            if ((targetOre == null) && (targetIngot == null)) store = new Store(block, desc);
                            else if (targetOre != null) store = new Store(block, MyItemType.MakeOre(targetOre), desc);
                            else if (targetIngot != null) store = new Store(block, MyItemType.MakeIngot(targetIngot), desc);

                            return new MonitorInfo
                            {
                                { "block", new CommonInfo(block, index) { Icon = "Container" } },
                                { "sto", store }
                            };
                        }
                    };
                };

                mbb["Inventory"] = (MonitorBlockParser parser) => {

                    MonitorOption option;
                    string targetIngot = null;
                    string targetOre = null;
                    string desc = "";
                    string name;

                    if (parser.Options.TryGetValue("ore", out option))
                    {
                        targetOre = option.StrValue;
                        if (OreNameTable.TryGetValue(targetOre, out name)) desc = $"Stored {name}";
                    }

                    if (parser.Options.TryGetValue("ingot", out option))
                    {
                        targetIngot = option.StrValue;
                        if (IngotNameTable.TryGetValue(targetIngot, out name)) desc = $"Stored {name}";
                    }

                    return new MonitorBlock<IMyTerminalBlock>(parser)
                    {
                        SubFilter = (IMyTerminalBlock block) =>
                        {
                            if (block.InventoryCount == 0) return false;
                            return true;
                        },

                        InfoMaker = (IMyTerminalBlock block, int index) =>
                        {
                            Store store = new Store();
                            if ((targetOre == null) && (targetIngot == null)) store = new Store(block, desc);
                            else if (targetOre != null) store = new Store(block, MyItemType.MakeOre(targetOre), desc);
                            else if (targetIngot != null) store = new Store(block, MyItemType.MakeIngot(targetIngot), desc);

                            return new MonitorInfo
                            {
                                { "block", new CommonInfo(block, index) { Icon = "Container" } },
                                { "sto", store }
                            };
                        }
                    };
                };

                mbb["Refinery"] = (MonitorBlockParser parser) => {
                    return new MonitorBlock<IMyRefinery>(parser)
                    {
                        InfoMaker = (IMyRefinery block, int index) =>
                        {
                            return new MonitorInfo
                            {
                                { "block", new CommonInfo(block, index) { Icon = "Factory" } },
                                { "instore", new Store(block.InputInventory) },
                                { "outstore", new Store(block.OutputInventory) },
                                { "product", new Production(block) }
                            };
                        }
                    };
                };

                mbb["Assembler"] = (MonitorBlockParser parser) => {
                    return new MonitorBlock<IMyAssembler>(parser)
                    {
                        InfoMaker = (IMyAssembler block, int index) =>
                        {
                            return new MonitorInfo
                            {
                                { "block", new CommonInfo(block, index) { Icon = "Factory" } },
                                { "instore", new Store(block.InputInventory) },
                                { "outstore", new Store(block.OutputInventory) },
                                { "product", new Production(block) },
                                { "assemble", new Assembler(block) }
                            };
                        }
                    };
                };

            }
        }

        public class Production : InfoItem
        {
            public bool IsProducing { get; set; } = false;
            public bool IsQueueEmpty { get; set; } = false;
            public List<MyProductionItem> Queue { get; set; } = new List<MyProductionItem>();

            public Production(IMyProductionBlock block)
            {
                IsProducing = block.IsProducing;
                IsQueueEmpty = block.IsQueueEmpty;
                block.GetQueue(Queue);
            }

            public override void Totalize(InfoItem info) { }
            public override string ToText(MonitorContent content)
            {
                switch (content.Subtype)
                {
                    case "producing": return IsProducing ? "Producing" : "";
                    case "empty": return IsQueueEmpty ? "Queue Empty" : "";
                    //case "next": return (Queue.Count > 0) ? Queue[0].ToString() : "Empty";
                    case "next":
                        if (Queue.Count > 0) return Queue[0].BlueprintId.ToString();
                        else return "Empty";
                    default: return "";
                }
            }
        }

        public class Assembler : InfoItem
        {
            public float CurrentProgress { get; set; } = 0.0f;
            public MyAssemblerMode AssemblerMode { get; set; } = 0;
            public bool CooperativeMode { get; set; } = false;
            public bool Repeating { get; set; } = false;

            public Assembler(IMyAssembler assembler)
            {
                CurrentProgress = assembler.CurrentProgress;
                AssemblerMode = assembler.Mode;
                CooperativeMode = assembler.CooperativeMode;
                Repeating = assembler.Repeating;
            }

            public override void Totalize(InfoItem info) { }
            public override string ToText(MonitorContent content)
            {
                switch (content.Subtype)
                {
                    case "progress": return $"{CurrentProgress:P2}";
                    case "mode": return AssemblerMode.ToString();
                    case "coop": return CooperativeMode ? "Cooperative" : "";
                    case "repeat": return Repeating ? "Repeating" : "";
                    default: return "";
                }
            }
        }

    }
}
