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
        public interface IMonitorBlock { MonitorTarget Target { get; } MonitorOptions Options { get; } MonitorContents Contents { get; } bool Totalize { get; } List<MonitorInfo> BlockInfos { get; } void Update(); }

        public abstract class MonitorBlockBase : IMonitorBlock
        {
            protected MyGridProgram Program { get; }
            protected void Echo(string str) { Program.Echo(str); }
            protected IMyProgrammableBlock Me { get { return Program.Me; } }
            protected IMyGridTerminalSystem GridTerminalSystem { get { return Program.GridTerminalSystem; } }

            public MonitorTarget Target { get; }
            public MonitorOptions Options { get; }
            public MonitorContents Contents { get; }

            public bool Totalize { get; }
            public List<MonitorInfo> BlockInfos { get; protected set; } = null;

            public MonitorBlockBase(MyGridProgram program, MonitorTarget target, MonitorOptions options, MonitorContents contents)
            {
                Program = program;
                Target = target ?? new MonitorTarget(Me);
                Options = options ?? new MonitorOptions();
                Contents = contents ?? new MonitorContents();

                Totalize = Options.ContainsKey("total");
            }
            public MonitorBlockBase(MonitorBlockParser parser) : this(parser.Program, parser.Target, parser.Options, parser.Contents) { }

            public abstract void Update();
        }

        public class MonitorBlock<T> : MonitorBlockBase where T : class
        {
            public Func<IMyTerminalBlock, bool> SubFilter { get; set; } = null;
            public Func<T, int, MonitorInfo> InfoMaker { get; set; } = null;

            public MonitorBlock(MyGridProgram program, MonitorOptions options, MonitorContents contents = null) : base(program, null, options, contents) { }
            public MonitorBlock(MyGridProgram program, MonitorTarget target, MonitorOptions options, MonitorContents contents) : base(program, target, options, contents) { }
            public MonitorBlock(MonitorBlockParser parser) : base(parser) { }

            public override void Update()
            {
                var blocks = new List<IMyTerminalBlock>();
                GetBlocks<T>(blocks, SubFilter);

                BlockInfos = new List<MonitorInfo>();

                MonitorInfo info = new MonitorInfo();
                MonitorInfo totalInfo = null;
                int index = Target.RangeStart;

                foreach (T block in blocks)
                {
                    if (InfoMaker != null) info = InfoMaker(block, index);

                    if (Totalize)
                    {
                        if (totalInfo == null) totalInfo = info;
                        else totalInfo.Totalize(info);
                    }
                    else BlockInfos.Add(info);

                    index++;
                }

                if (Totalize && (blocks.Count > 0)) BlockInfos.Add(totalInfo);

            }

            protected void GetBlocks<V>(List<IMyTerminalBlock> blocks, Func<IMyTerminalBlock, bool> subfilter = null) where V : class
            {
                int index = 0;

                if (subfilter != null)
                {
                    GridTerminalSystem.GetBlocksOfType<V>(blocks, (IMyTerminalBlock block) => {
                        if (!Target.NameFilter(block)) return false;
                        if (!Target.GridFilter(block)) return false;
                        if (!subfilter(block)) return false;
                        if (!Target.IndexFilter(index++)) return false;
                        return true;
                    });
                }
                else
                {
                    GridTerminalSystem.GetBlocksOfType<V>(blocks, (IMyTerminalBlock block) => {
                        if (!Target.NameFilter(block)) return false;
                        if (!Target.GridFilter(block)) return false;
                        if (!Target.IndexFilter(index++)) return false;
                        return true;
                    });
                }
            }

        }

        public class TestBlock : MonitorBlockBase
        {
            public string Text { get; }
            public TestBlock(MyGridProgram program, MonitorOptions options, string text) : base(program, null, options, null) { Text = text; }
            public override void Update() { }
        }

        public class TextBlock : MonitorBlockBase
        {
            public string Text { get; }
            public TextBlock(MyGridProgram program, MonitorOptions options, string text) : base(program, null, options, null) { Text = text; }
            public TextBlock(MyGridProgram program, string text) : base(program, null, null, null) { Text = text; }
            public override void Update() { }
        }
    }
}
