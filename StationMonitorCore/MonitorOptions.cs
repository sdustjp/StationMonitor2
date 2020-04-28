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
        public class MonitorOptions : Dictionary<string, MonitorOption> { }

        public class MonitorOption
        {
            public string StrValue { get; } = "";
            public int IntValue { get; } = 0;
            public float FloatValue { get; } = 0.0f;
            public Color ColorValue { get; } = new Color();
            public List<string> StrValues { get; } = new List<string>();
            public List<int> IntValues { get; } = new List<int>();
            public List<float> FloatValues { get; } = new List<float>();
            public List<Color> ColorValues { get; } = new List<Color>();

            public MonitorOption(string valueText)
            {
                if (!String.IsNullOrEmpty(valueText))
                {
                    StrValue = valueText;
                    int iv; if (int.TryParse(valueText, out iv)) IntValue = iv;
                    float fv; if (float.TryParse(valueText, out fv)) FloatValue = fv;
                    Color cv; if (TryParseColor(valueText, out cv)) ColorValue = cv;

                    foreach (var str in valueText.Split('|'))
                    {
                        StrValues.Add(str);
                        if (int.TryParse(str, out iv)) IntValues.Add(iv);
                        if (float.TryParse(str, out fv)) FloatValues.Add(fv);
                        if (TryParseColor(str, out cv)) ColorValues.Add(cv);
                    }
                }
            }

            public static bool TryParseColor(string str, out Color color)
            {
                str = str.Trim();
                if (str.StartsWith("#") && (str.Length == 7))
                {
                    color = new Color(Convert.ToInt32(str.Substring(1, 2), 16), Convert.ToInt32(str.Substring(3, 2), 16), Convert.ToInt32(str.Substring(5, 2), 16), 255);
                    return true;
                }
                color = new Color();
                return false;
            }
        }
    }
}
