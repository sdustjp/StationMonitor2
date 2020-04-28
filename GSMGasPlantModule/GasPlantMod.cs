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
        public class GasPlantMod
        {
            public static void Load(StationMonitorCore sm)
            {
                MonitorBlockBuilder mbb = sm.MBBuilder;

                mbb["GasGenerator"] = (MonitorBlockParser parser) => {
                    return new MonitorBlock<IMyGasGenerator>(parser)
                    {
                        InfoMaker = (IMyGasGenerator block, int index) => {
                            return new MonitorInfo
                            {
                                { "block", new CommonInfo(block, index) { Icon = "GasTank" } },
                                { "sto", new Store(block, MyItemType.MakeOre("Ice"), "Stored Ice") }
                            };
                        }
                    };
                };

                mbb["OxygenFarm"] = (MonitorBlockParser parser) => {
                    return new MonitorBlock<IMyOxygenFarm>(parser)
                    {
                        InfoMaker = (IMyOxygenFarm block, int index) => {
                            var store = new Store("Oxygen Output");
                            // CubeBlocks.sbc <SubtypeId>LargeBlockOxygenFarm</SubtypeId> <MaxOutputPerSecond>0.03</MaxOutputPerSecond>
                            store.Max = 0.03f;  // unit is "L"
                            store.Current = store.Max * block.GetOutput();
                            return new MonitorInfo
                            {
                                { "block", new CommonInfo(block, index) { Icon = "GasTank" } },
                                { "sto", store }
                            };
                        }
                    };
                };

                mbb["OxygenTank"] = (MonitorBlockParser parser) => {
                    return new MonitorBlock<IMyGasTank>(parser)
                    {
                        SubFilter = (IMyTerminalBlock block) => {
                            if (block.BlockDefinition.SubtypeId.Contains("Hydrogen")) return false;
                            return true;
                        },

                        InfoMaker = (IMyGasTank block, int index) =>
                        {
                            return new MonitorInfo
                            {
                                { "block", new CommonInfo(block, index) { Icon = "O2Tank" } },
                                { "sto", new Store(block, "Stored Oxygen") }
                            };
                        }
                    };
                };

                mbb["HydrogenTank"] = (MonitorBlockParser parser) => {
                    return new MonitorBlock<IMyGasTank>(parser)
                    {
                        SubFilter = (IMyTerminalBlock block) =>
                        {
                            if (!block.BlockDefinition.SubtypeId.Contains("Hydrogen")) return false;
                            return true;
                        },

                        InfoMaker = (IMyGasTank block, int index) =>
                        {
                            return new MonitorInfo
                            {
                                { "block", new CommonInfo(block, index) { Icon = "H2Tank" } },
                                { "sto", new Store(block, "Stored Hydrogen") }
                            };
                        }
                    };
                };

            }
        }
    }
}
