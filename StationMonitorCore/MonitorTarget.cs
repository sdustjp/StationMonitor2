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
        public class MonitorTarget
        {
            protected IMyProgrammableBlock Me { get; }
            public string Keyword { get; } = "";
            public int RangeStart { get; } = 0;
            public int RangeCount { get; } = 0;
            public int RangeEnd { get; }
            public MonitorTargetGrid Grid { get; } = MonitorTargetGrid.MyConstruct;

            public MonitorTarget(IMyProgrammableBlock me, string keyword = null, int? rangeStart = null, int? rangeCount = null, MonitorTargetGrid? grid = null)
            {
                if (me == null) throw new ArgumentNullException();
                Me = me;
                Keyword = keyword ?? "";
                RangeStart = rangeStart ?? 0;
                RangeCount = rangeCount ?? int.MaxValue;

                if (RangeStart > (int.MaxValue - RangeCount)) RangeEnd = int.MaxValue;
                else RangeEnd = RangeStart + RangeCount - 1;

                Grid = grid ?? MonitorTargetGrid.MyConstruct;
            }

            public bool NameFilter(IMyTerminalBlock block)
            {
                if ((Keyword != "") && (!block.CustomName.Contains(Keyword))) return false;
                return true;
            }

            public bool GridFilter(IMyTerminalBlock block)
            {
                if (Grid == MonitorTargetGrid.MyConstruct) if (!block.IsSameConstructAs(Me)) return false;
                    else if (Grid == MonitorTargetGrid.MyGrid) if (block.CubeGrid != Me.CubeGrid) return false;
                return true;
            }

            public bool IndexFilter(int index)
            {
                if ((index < RangeStart) || (RangeEnd < index)) return false;
                return true;
            }
        }

        public enum MonitorTargetGrid { MyGrid, MyConstruct, All }
    }
}
