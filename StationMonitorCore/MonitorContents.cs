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
        public class MonitorContents : List<List<MonitorContent>> { }

        public class MonitorContent : MonitorOption
        {
            public string Type { get; } = null;
            public string Subtype { get; } = null;

            public TextAlignment Align { get; } = TextAlignment.LEFT;
            public float? Width { get; } = null;

            public MonitorContent(string typeText, string valueText, string styleText = "") : base(valueText)
            {
                typeText = typeText ?? "";

                // alias
                switch (typeText)
                {
                    case "index": typeText = "block.index"; break;
                    case "name": typeText = "block.name"; break;
                }

                var typeParts = typeText.Split('.');

                Type = typeParts[0].Trim();

                if (typeParts.Length > 1) Subtype = typeParts[1].Trim();

                if (!String.IsNullOrWhiteSpace(styleText))
                {
                    string str = styleText.Trim().ToLower();
                    string widthStr = str;

                    switch (str[0])
                    {
                        case 'l':
                            Align = TextAlignment.LEFT;
                            widthStr = str.Substring(1);
                            break;
                        case 'c':
                            Align = TextAlignment.CENTER;
                            widthStr = str.Substring(1);
                            break;
                        case 'r':
                            Align = TextAlignment.RIGHT;
                            widthStr = str.Substring(1);
                            break;
                    }

                    float width; if (float.TryParse(widthStr, out width)) Width = width;

                }
            }
        }
    }
}
