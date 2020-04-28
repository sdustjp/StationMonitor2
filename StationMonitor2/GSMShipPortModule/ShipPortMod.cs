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
        public class ShipPortMod
        {
            public static void Load(StationMonitorCore sm)
            {
                MonitorBlockBuilder mbb = sm.MBBuilder;

                mbb["AirVent"] = (MonitorBlockParser parser) => {
                    return new MonitorBlock<IMyAirVent>(parser)
                    {
                        InfoMaker = (IMyAirVent block, int index) =>
                        {
                            return new MonitorInfo
                            {
                                { "block", new CommonInfo(block, index) { Icon = "Factory" } },
                                { "vent", new VentInfo(block) }
                            };
                        }
                    };
                };

                mbb["Door"] = (MonitorBlockParser parser) => {
                    return new MonitorBlock<IMyDoor>(parser)
                    {
                        InfoMaker = (IMyDoor block, int index) =>
                        {
                            return new MonitorInfo
                            {
                                { "block", new CommonInfo(block, index) { Icon = "Factory" } },
                                { "door", new DoorInfo(block) }
                            };
                        }
                    };
                };

                mbb["Connector"] = (MonitorBlockParser parser) => {
                    return new MonitorBlock<IMyShipConnector>(parser)
                    {
                        InfoMaker = (IMyShipConnector block, int index) =>
                        {
                            return new MonitorInfo
                            {
                                { "block", new CommonInfo(block, index) { Icon = "Factory" } },
                                { "connector", new ConnectorInfo(block) }
                            };
                        }
                    };
                };

            }
        }
    }
}
