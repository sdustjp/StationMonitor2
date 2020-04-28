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
        public class BoxStyle
        {
            public SizeRule SizeRule { get; set; } = SizeRule.Auto;
            public Vector2 Size = new Vector2();
            public TextAlignment Align { get; set; } = TextAlignment.LEFT;
            public float Padding { get; set; } = 0;
            public Vector2 InnerSize { get { return Size - Padding * 2; } }

            public BoxStyle(SizeRule sizeRule, float sizeX = 0, float sizeY = 0, TextAlignment align = TextAlignment.LEFT, float padding = 0)
            {
                SizeRule = sizeRule;
                if (sizeRule == SizeRule.Auto) Size = new Vector2();
                else if (sizeRule == SizeRule.Fix) Size = new Vector2(sizeX, sizeY);
                Align = align;
                Padding = padding;
            }
            public BoxStyle(TextAlignment align = TextAlignment.LEFT, float padding = 0) : this(SizeRule.Auto, 0, 0, align, padding) { }
            public BoxStyle(float sizeX, float sizeY, float padding = 0) : this(SizeRule.Fix, sizeX, sizeY, TextAlignment.LEFT, padding) { }
            public BoxStyle(Vector2 size, TextAlignment align, float padding = 0) : this(SizeRule.Fix, size.X, size.Y, align, padding) { }
            public BoxStyle(BoxStyle style) : this(style.SizeRule, style.Size.X, style.Size.Y, style.Align, style.Padding) { }
        }
        public enum SizeRule { Auto, Fix }
    }
}
