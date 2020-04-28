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
        public class Monitor
        {
            protected MyGridProgram Program { get; }
            protected void Echo(string str) { Program.Echo(str); }

            public string Name { get; set; } = "";
            public string Source { get; protected set; } = "";
            public IMyTextSurface Surface { get; set; } = null;
            public List<IMonitorBlock> MonitorBlocks { get; protected set; } = new List<IMonitorBlock>();

            protected MonitorBlockParser Parser { get; }
            protected MonitorBlockBuilder Builder { get; }

            public Monitor(MyGridProgram program, MonitorBlockParser parser, MonitorBlockBuilder builder)
            {
                Program = program;
                Parser = parser;
                Builder = builder;
            }

            public bool TryParse(string sourceText, int surfaceIndex = -1)
            {
                //if (DEBUG) Echo($"Monitor[{Name}].TryParse()");

                MonitorBlocks = new List<IMonitorBlock>();

                var textLines = sourceText.Split('\n');

                StringBuilder textBuf = null;

                bool compileSwitch = true;

                foreach (string textLine in textLines)
                {
                    // comment line
                    if (textLine.StartsWith("%")) { } // ignore
                    // command line
                    else if (textLine.StartsWith("#"))
                    {
                        if (textBuf != null)
                        {
                            if (!TryParseSub(textBuf.ToString())) return false;
                            textBuf = null;
                        }

                        string controlCmd = textLine.Substring(1).Trim();

                        if (controlCmd == "BLOCK_END") compileSwitch = true;
                        else if ((surfaceIndex != -1) && (controlCmd == $"SURFACE_{surfaceIndex}_BLOCK")) compileSwitch = true;
                        else if (controlCmd == "COMMENT_BLOCK") compileSwitch = false;
                    }
                    else if (compileSwitch)
                    {
                        if (textBuf == null) textBuf = new StringBuilder(textLine);
                        else textBuf.Append("\n").Append(textLine);
                    }

                }

                if (textBuf != null) if (!TryParseSub(textBuf.ToString())) return false;

                Source = sourceText;

                return true;
            }

            private bool TryParseSub(string text)
            {
                MonitorBlockParser mbp = Parser;
                MonitorBlockBuilder mbb = Builder;
                IMonitorBlock monb;

                int i = 0;

                while (i < text.Length)
                {
                    try
                    {
                        i = mbp.Parse(text, i);
                    }
                    catch (MissingCharsException ex)
                    {
                        Echo("Parse Error : Missing characters (" + String.Join<char>(",", ex.Chars) + ")");
                        return false;
                    }
                    catch (InvalidCharsException ex)
                    {
                        Echo("Parse Error : Invalid characters (" + String.Join<char>(",", ex.Chars) + ")");
                        return false;
                    }
                    catch (Exception ex)
                    {
                        Echo("Parse Error : Caught exception (" + ex.Message + ":" + ex.ToString() + ")");
                        return false;
                    }

                    monb = mbb.Build(mbp);
                    //if (DEBUG)
                    //{
                    //    if (monb != null)
                    //    {
                    //        Echo("Build monitor block: {");
                    //        Echo($"  BlockName={mbp.BlockName}");
                    //        Echo($"  TargetText={mbp.TargetText}");
                    //        Echo($"  OptionsText={mbp.OptionsText}");
                    //        Echo($"  ContentsText={mbp.ContentsText}");
                    //        Echo("}");
                    //    }
                    //}

                    if (monb != null)
                    {
                        if ((monb is TextBlock) && ((monb as TextBlock).Text == ""))
                        {
                            // skip "return only text block"
                        }
                        else
                        {
                            MonitorBlocks.Add(monb);
                        }
                    }
                }

                return true;
            }

            public void Update()
            {
                //if (DEBUG) Echo($"Monitor[{Name}].Update()");
                foreach (MonitorBlockBase monitorBlock in MonitorBlocks)
                {
                    monitorBlock.Update();
                }
            }
        }
    }
}
