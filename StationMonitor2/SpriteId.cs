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
        public class SpriteId
        {
            public const string Arrow = "Arrow";
            public const string Cross = "Cross";
            public const string Danger = "Danger";
            public const string NoEntry = "No Entry";
            public const string Construction = "Construction";
            public const string WhiteScreen = "White screen";
            public const string Grid = "Grid";

            public const string DecorativeBracketLeft = "DecorativeBracketLeft";
            public const string DecorativeBracketRight = "DecorativeBracketRight";
            public const string SquareTapered = "SquareTapered";
            public const string SquareSimple = "SquareSimple";
            public const string IconEnergy = "IconEnergy";
            public const string IconHydrogen = "IconHydrogen";
            public const string IconOxygen = "IconOxygen";
            public const string IconTemperature = "IconTemperature";

            public const string RightTriangle = "RightTriangle";
            public const string Triangle = "Triangle";
            public const string Circle = "Circle";
            public const string SemiCircle = "SemiCircle";
            public const string CircleHollow = "CircleHollow";
            public const string SquareHollow = "SquareHollow";
        }
    }
}
