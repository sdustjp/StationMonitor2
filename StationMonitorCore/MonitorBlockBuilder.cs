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
        public class MonitorBlockBuilder : Dictionary<string, Func<MonitorBlockParser, IMonitorBlock>>
        {
            private MyGridProgram Program { get; }
            private void Echo(string str) { Program.Echo(str); }
            private IMyProgrammableBlock Me { get { return Program.Me; } }

            public MonitorBlockBuilder(MyGridProgram program) { Program = program; }

            public void Add(Func<MonitorBlockParser, IMonitorBlock> func, params string[] keys) { foreach (string key in keys) Add(key, func); }
            public IMonitorBlock Build(MonitorBlockParser parser)
            {
                if (parser.BlockName == null) return null;
                Func<MonitorBlockParser, IMonitorBlock> builder; if (!TryGetValue(parser.BlockName, out builder)) return null;
                return builder(parser);
            }
        }
    }
}
