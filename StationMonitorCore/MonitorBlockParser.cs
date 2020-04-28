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
        public class MonitorBlockParser
        {
            public MyGridProgram Program { get; }
            private void Echo(string str) { Program.Echo(str); }
            private IMyProgrammableBlock Me { get { return Program.Me; } }

            public string BlockName { get; protected set; } = null;

            public string TargetText { get; protected set; } = null;
            public string OptionsText { get; protected set; } = null;
            public string ContentsText { get; protected set; } = null;

            public MonitorTarget Target { get; protected set; } = null;
            public MonitorOptions Options { get; protected set; } = null;
            public MonitorContents Contents { get; protected set; } = null;

            public MonitorBlockParser(MyGridProgram program) { Program = program; }

            public int Parse(string text, int startIndex = 0)
            {
                Clear();

                int i, j;

                i = startIndex;
                j = text.IndexOf('$', i);

                if (j == -1)
                {
                    // text block
                    BlockName = "Text";
                    ContentsText = text.Substring(i).Trim('\n');

                    return text.Length;
                }

                if (j > i)
                {
                    // text block
                    BlockName = "Text";
                    ContentsText = text.Substring(i, j - i).Trim('\n');

                    return j;
                }

                // monitor block ?

                i = j + 1;
                j = text.IndexOfAny(new[] { '<', '(' }, i);

                if (j == -1) throw new MissingCharsException('<', '(');

                string blockName = text.Substring(i, j - i).Trim();

                // target block
                if (text[j] == '<')
                {
                    i = j + 1;
                    j = text.IndexOf('>', i);

                    if (j == -1) throw new MissingCharsException('>');

                    TargetText = text.Substring(i, j - i).Trim();
                    Target = ParseTarget(TargetText);

                    i = j + 1;
                    j = text.IndexOf('(', i);

                    if (j == -1) throw new MissingCharsException('(');

                    if (text.Substring(i, j - i).Trim().Length > 0)
                        throw new InvalidCharsException(text.Substring(i, j - i).Trim()[0]);
                }

                i = j + 1;
                j = text.IndexOf(')', i);
                if (j == -1) throw new MissingCharsException(')');

                OptionsText = text.Substring(i, j - i).Trim();
                Options = ParseOptions(OptionsText);

                i = j + 1;
                j = text.IndexOfAny(new[] { '{', ';' }, i);
                if (j == -1) throw new MissingCharsException('{', ';');

                // contents block
                if (text[j] == '{')
                {
                    i = j + 1;
                    j = text.IndexOf('}', i);

                    if (j == -1) throw new MissingCharsException('}');

                    ContentsText = text.Substring(i, j - i).Trim();
                    Contents = ParseContents(ContentsText);

                }

                // monitor block
                BlockName = blockName;

                return j + 1;
            }

            private MonitorTarget ParseTarget(string text)
            {
                string keyword = null;
                int? rangeStart = null;
                int? rangeCount = null;
                MonitorTargetGrid? grid = null;

                int i = 0, j;

                j = text.IndexOf('{', i);
                if (j == -1)
                {
                    keyword = text.Trim();
                    if (keyword.Length == 0) keyword = null;
                }
                else
                {
                    i = j + 1;
                    j = text.IndexOf('}', i);
                    if (j == -1) throw new MissingCharsException('}');

                    var strs = text.Substring(i, j - i).Trim().Split(',');

                    if (strs.Length > 0)
                    {
                        string str = strs[0].Trim();
                        if (str != "") { rangeStart = int.Parse(str); }
                    }
                    if (strs.Length > 1)
                    {
                        string str = strs[1].Trim();
                        if (str == "all") { rangeCount = int.MaxValue; }
                        else if (str != "") { rangeCount = int.Parse(str); }
                    }
                    if (strs.Length > 2)
                    {
                        string str = strs[2].Trim();
                        if (str != "")
                        {
                            switch (str)
                            {
                                case "all":
                                    grid = MonitorTargetGrid.All;
                                    break;
                                case "me":
                                    grid = MonitorTargetGrid.MyGrid;
                                    break;
                                case "mycon":
                                    grid = MonitorTargetGrid.MyConstruct;
                                    break;
                                default:
                                    throw new Exception($"'{strs[2].Trim()}' is unknown target grid type.");
                            }
                        }
                    }


                }

                return new MonitorTarget(Me, keyword, rangeStart, rangeCount, grid);
            }

            private MonitorOptions ParseOptions(string text)
            {
                MonitorOptions options = new MonitorOptions();
                if (!String.IsNullOrWhiteSpace(text))
                {
                    var optionTexts = text.Split(',');

                    for (int i = 0; i < optionTexts.Length; i++)
                    {
                        string key;
                        string value;
                        var parts = optionTexts[i].Split(new[] { '=' }, 2);

                        key = parts[0].Trim();
                        if (parts.Length == 1)
                            value = "";
                        else
                            value = parts[1].Trim();

                        options[key] = new MonitorOption(value);

                    }
                }
                return options;
            }

            private MonitorContents ParseContents(string text)
            {
                MonitorContents contents = new MonitorContents();
                if (!String.IsNullOrWhiteSpace(text))
                {
                    var textLines = text.Split(';');

                    string[] styleTexts = new string[0];

                    foreach (var textLine in textLines)
                    {

                        List<MonitorContent> contentsSub = new List<MonitorContent>();

                        string[] contentTexts;
                        var texts = textLine.Split(new[] { ':' }, 2);

                        if (texts.Length == 1)
                        {
                            contentTexts = texts[0].Split(',');
                        }
                        else
                        {
                            styleTexts = texts[0].Split(',');
                            contentTexts = texts[1].Split(',');
                        }

                        int i = 0;

                        foreach (var contentText in contentTexts)
                        {
                            string type;
                            string value;

                            var str = contentText.Trim();
                            if (str.StartsWith("\""))
                            {
                                int j = str.IndexOf('\"', 1);
                                if (j == -1) throw new MissingCharsException('\"');
                                type = "text";
                                value = str.Substring(1, j - 1);
                            }
                            else
                            {
                                var parts = contentText.Split(new[] { '=' }, 2);
                                type = parts[0].Trim();
                                if (parts.Length == 1)
                                    value = "";
                                else
                                    value = parts[1].Trim();
                            }

                            string style;
                            if (i < styleTexts.Length)
                                style = styleTexts[i].Trim();
                            else
                                style = "";

                            contentsSub.Add(new MonitorContent(type, value, style));

                            i++;
                        }

                        contents.Add(contentsSub);
                    }
                }
                return contents;
            }

            public void Clear()
            {
                BlockName = null;

                TargetText = null;
                OptionsText = null;
                ContentsText = null;

                Target = null;
                Options = null;
                Contents = null;
            }
        }
    }
}
