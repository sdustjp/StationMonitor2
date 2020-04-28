// <mdk sortorder="200" />
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
        public partial class GraphicalStationMonitor : StationMonitorCore
        {
            private static GraphicalStationMonitor _instance = null;
            public static new GraphicalStationMonitor GetInstance(MyGridProgram program)
            {
                if (_instance == null) _instance = new GraphicalStationMonitor(program);
                return _instance;
            }

            public static Dictionary<string, SpriteDataBuilder> IconRes = new Dictionary<string, SpriteDataBuilder>();
            public static void LoadSpriteDatas()
            {
                float pi = SpriteData.PI, pi2 = SpriteData.PI_2;

                IconRes.Add("Hoge", new SpriteDataBuilder()
                    .Add(SpriteId.SquareHollow, 0, 0, 1.0f, 1.0f, 1)
                    .Add(SpriteId.SquareSimple, 0, 0, 0.8f, 0.8f, 1)
                    .Add(SpriteId.SquareSimple, 0, 0, 0.3f, 0.1f, 0, 0.0f, 0, 0, 0.3f, 0.1f, 0, 1.0f)
                    .Add(SpriteId.SquareHollow, 0, 0, 0.1f, 0.3f, 0, 0.0f, 0, 0, 0.1f, 0.3f, 0, 1.0f)
                    .Add(SpriteId.SquareSimple, -0.45f, -0.45f, 0.1f, 0.1f, 0)
                    .Add(SpriteId.Circle, 0.45f, 0.45f, 0.1f, 0.1f, 0)
                    .Add(SpriteId.SquareSimple, 0.30f, 0.00f, 0.2f, 0.1f, 0, 0, 0, 0.30f, 0.00f, 0.1f, 0.1f, 0, 1.0f, 1.0f)
                    );

                IconRes.Add("PowerGen", new SpriteDataBuilder()
                    .Add(SpriteId.SquareSimple, 0, 0, 0.9f, 0.9f, 1)
                    .Add(SpriteId.SquareSimple, 0, 0, 0.8f, 0.8f, 0)
                    .Add(SpriteId.IconEnergy, 0, 0, 0.7f, 0.7f, 1)
                    );

                IconRes.Add("GasTank", new SpriteDataBuilder()
                    .Add(SpriteId.SquareSimple, 0, 0, 0.9f, 0.9f, 1)
                    .Add(SpriteId.SquareSimple, 0, 0, 0.8f, 0.8f, 0)
                    .Add(SpriteId.IconOxygen, 0, 0, 0.7f, 0.7f, 1)
                    );

                IconRes.Add("O2Tank", new SpriteDataBuilder()
                    .Add(SpriteId.SquareSimple, 0, 0, 0.9f, 0.9f, 1)
                    .Add(SpriteId.SquareSimple, 0, 0, 0.8f, 0.8f, 0)
                    .Add(SpriteId.IconOxygen, 0, 0, 0.7f, 0.7f, 1)
                    );

                IconRes.Add("H2Tank", new SpriteDataBuilder()
                    .Add(SpriteId.SquareSimple, 0, 0, 0.9f, 0.9f, 1)
                    .Add(SpriteId.SquareSimple, 0, 0, 0.8f, 0.8f, 0)
                    .Add(SpriteId.IconHydrogen, 0, 0, 0.7f, 0.7f, 1)
                    );

                IconRes.Add("Container", new SpriteDataBuilder()
                    .Add(SpriteId.SquareSimple, 0, 0, 0.9f, 0.9f, 1)
                    .Add(SpriteId.SquareSimple, 0, 0, 0.8f, 0.8f, 0)
                    .Add(SpriteId.SquareSimple, 0, -0.15f, 0.25f, 0.2f, 1)
                    .Add(SpriteId.SquareSimple, -0.18f, 0.15f, 0.25f, 0.2f, 1)
                    .Add(SpriteId.SquareSimple, 0.18f, 0.15f, 0.25f, 0.2f, 1)
                    );

                IconRes.Add("Factory", new SpriteDataBuilder()
                    .Add(SpriteId.SquareSimple, 0, 0, 0.9f, 0.9f, 1)
                    .Add(SpriteId.SquareSimple, 0, 0, 0.8f, 0.8f, 0)
                    .Add(SpriteId.SquareSimple, 0, 0, 0.7f, 0.12f, 1)
                    .Add(SpriteId.SquareSimple, 0, 0, 0.7f, 0.12f, 1, pi / 4)
                    .Add(SpriteId.SquareSimple, 0, 0, 0.7f, 0.12f, 1, pi / 2)
                    .Add(SpriteId.SquareSimple, 0, 0, 0.7f, 0.12f, 1, pi * 3 / 4)
                    .Add(SpriteId.Circle, 0, 0, 0.55f, 0.55f, 1)
                    .Add(SpriteId.Circle, 0, 0, 0.2f, 0.2f, 0)
                    );

                IconRes.Add("Windmill", new SpriteDataBuilder()
                    //.Add(SpriteId.SquareSimple, 0, 0, 0.9f, 0.9f, 1)
                    //.Add(SpriteId.SquareSimple, 0, 0, 0.8f, 0.8f, 0)
                    .Add(SpriteId.SemiCircle, 0, -0.2f, 0.3f, 0.15f, 1, pi / 2)
                    .Add(SpriteId.SemiCircle, 0, -0.2f, 0.3f, 0.15f, 1, pi2 * 1 / 3 + pi / 2, pi2 * 1 / 3)
                    .Add(SpriteId.SemiCircle, 0, -0.2f, 0.3f, 0.15f, 1, pi2 * 2 / 3 + pi / 2, pi2 * 2 / 3)
                    );

                IconRes.Add("Sun", new SpriteDataBuilder()
                    //.Add(SpriteId.SquareSimple, 0, 0, 0.9f, 0.9f, 1)
                    //.Add(SpriteId.SquareSimple, 0, 0, 0.8f, 0.8f, 0)
                    .Add(SpriteId.Triangle, 0, -0.3f, 0.1f, 0.15f, 1)
                    .Add(SpriteId.Triangle, 0, -0.3f, 0.1f, 0.15f, 1, pi * 1 / 4, pi * 1 / 4)
                    .Add(SpriteId.Triangle, 0, -0.3f, 0.1f, 0.15f, 1, pi * 2 / 4, pi * 2 / 4)
                    .Add(SpriteId.Triangle, 0, -0.3f, 0.1f, 0.15f, 1, pi * 3 / 4, pi * 3 / 4)
                    .Add(SpriteId.Triangle, 0, -0.3f, 0.1f, 0.15f, 1, pi * 4 / 4, pi * 4 / 4)
                    .Add(SpriteId.Triangle, 0, -0.3f, 0.1f, 0.15f, 1, pi * 5 / 4, pi * 5 / 4)
                    .Add(SpriteId.Triangle, 0, -0.3f, 0.1f, 0.15f, 1, pi * 6 / 4, pi * 6 / 4)
                    .Add(SpriteId.Triangle, 0, -0.3f, 0.1f, 0.15f, 1, pi * 7 / 4, pi * 7 / 4)
                    .Add(SpriteId.Circle, 0, 0, 0.3f, 0.3f, 1)
                    );

            }

            public int FrameCount { get; protected set; } = 0;

            protected GraphicalStationMonitor(MyGridProgram program) : base(program)
            {
                LoadStandardCRProvider();
            }

            public void DrawSprites()
            {
                //if (DEBUG) Echo("DrawSprites()");

                foreach (Monitor monitor in Monitors)
                {
                    //if (DEBUG) Echo($"Draw sprites: monitor[{monitor.Name}]");
                    IMyTextSurface surface = monitor.Surface;

                    surface.ContentType = ContentType.SCRIPT;
                    //surface.BackgroundColor = new Color(0,64,255,255);

                    SpritesFrameBuilder builder = new SpritesFrameBuilder(surface, FrameCount / 10.0f % 1.0f);

                    var frame = surface.DrawFrame();

                    foreach (MonitorBlockBase monitorBlock in monitor.MonitorBlocks)
                    {
                        if (monitorBlock.GetType() == typeof(TestBlock))
                        {
                            //if (DEBUG) Echo("builder.AppendTestSprite()");
                            builder.AppendTestSprite(monitorBlock as TestBlock);
                        }
                        else if (monitorBlock.GetType() == typeof(TextBlock))
                        {
                            //if (DEBUG) Echo("builder.AppendTextBlock()");
                            builder.AppendTextBlock(monitorBlock as TextBlock);
                        }
                        else
                        {
                            //if (DEBUG) Echo("builder.AppendMonitorBlock()");
                            builder.AppendMonitorBlock(monitorBlock);
                        }
                    }

                    frame.AddRange(builder.Sprites);

                    frame.Dispose();

                }

                FrameCount++;
                if (FrameCount >= 100000) FrameCount = FrameCount % 100000;

            }
        }
    }
}
