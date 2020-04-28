// <mdk sortorder="100" />
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
        public class StationMonitorCore
        {
            private static StationMonitorCore _instance = null;
            public static StationMonitorCore GetInstance(MyGridProgram program)
            {
                if (_instance == null) _instance = new StationMonitorCore(program);
                return _instance;
            }

            public static MyGridProgram Program { get; private set; } = null;
            public static void Echo(string str) { if (Program != null) Program.Echo(str); }
            protected IMyProgrammableBlock Me { get { return Program.Me; } }
            protected IMyGridTerminalSystem GridTerminalSystem { get { return Program.GridTerminalSystem; } }

            public int ProgramCount { get; protected set; } = 0;

            public MonitorBlockParser MBParser { get; set; }
            public MonitorBlockBuilder MBBuilder { get; set; }

            protected StationMonitorCore(MyGridProgram program)
            {
                Program = program;

                MBParser = new MonitorBlockParser(program);
                MBBuilder = new MonitorBlockBuilder(program)
                {
                    ["Test"] = (MonitorBlockParser parser) => { return new TestBlock(Program, parser.Options, parser.ContentsText ?? ""); },
                    ["Text"] = (MonitorBlockParser parser) => { return new TextBlock(Program, parser.Options, parser.ContentsText ?? ""); }
                };
            }

            public string PanelKeyword { get; set; } = DEFAULT_PANEL_KEYWORD;

            protected List<Monitor> Monitors { get; private set; } = new List<Monitor>();

            protected bool BlockFilter(IMyTerminalBlock block)
            {
                if (!block.IsSameConstructAs(Me)) return false;
                if (!block.CustomName.Contains(PanelKeyword)) return false;
                return true;
            }

            public void Search()
            {
                Monitors = new List<Monitor>();

                List<IMyTerminalBlock> blocks;

                blocks = new List<IMyTerminalBlock>();
                GridTerminalSystem.GetBlocksOfType<IMyTextSurfaceProvider>(blocks, BlockFilter);

                foreach (IMyTerminalBlock block in blocks)
                {
                    IMyTextSurfaceProvider provider = block as IMyTextSurfaceProvider;

                    for (int i = 0; i < provider.SurfaceCount; i++)
                    {
                        Monitor monitor = new Monitor(Program, MBParser, MBBuilder)
                        {
                            Surface = provider.GetSurface(i),
                            Name = $"{block.CustomName}[{i}]"
                        };

                        if (!monitor.TryParse(block.CustomData, i))
                        {
                            Echo("parse failure!");
                            continue;
                        }

                        if (monitor.MonitorBlocks.Count > 0)
                        {
                            Monitors.Add(monitor);
                            //if (DEBUG) Echo($"Add monitor: {monitor.Name}");
                        }
                    }
                }

            }

            public void Update() { foreach (Monitor monitor in Monitors) monitor.Update(); }

            public int CountUp() { ProgramCount++; return ProgramCount; }
            public int CountReset() { ProgramCount = 0; return ProgramCount; }
        }
    }
}
