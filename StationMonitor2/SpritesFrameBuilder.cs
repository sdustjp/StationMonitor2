// <mdk sortorder="300" />
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
        partial class GraphicalStationMonitor
        {
            public class SpritesFrameBuilder
            {
                private List<MySprite> sprites = new List<MySprite>();

                public IMyTextSurface Surface { get; }
                public List<MySprite> Sprites { get { return sprites; } protected set { sprites = value; } }
                public float FrameCount { get; set; }

                public float Scale { get; set; } = 1;
                public float Padding { get; set; } = 0;

                public string FontId { get; set; } = "DEBUG";
                public float FontScale { get; set; } = 1.0f;

                public Vector2 Cursor = new Vector2();
                public Color Color { get; set; } = new Color(255, 255, 255, 255);
                public Color BGColor { get; set; } = new Color(0, 0, 0, 255);

                public SpritesFrameBuilder(IMyTextSurface surface, float frameCount = 0.0f)
                {
                    Surface = surface;
                    FrameCount = frameCount;
                    Color = surface.ScriptForegroundColor;
                    BGColor = surface.ScriptBackgroundColor;
                }

                public BoundingBox2 AppendTestSprite(TestBlock monitorBlock)
                {
                    BoundingBox2 box = new BoundingBox2(Cursor, Cursor);
                    //SpriteMaker m = new SpriteMaker(Surface, Cursor, Scale, FrameCount)
                    //{
                    //    Color = Color,
                    //    BGColor = BGColor,
                    //};
                    //MonitorOption opt;
                    //BoxStyle style;
                    //string type = "";

                    Cursor.Y += box.Height;

                    return box;
                }

                public BoundingBox2 AppendTextBlock(TextBlock monBlock)
                {
                    MonitorOption opt;

                    SpriteBuilder mk = new SpriteBuilder(Surface, Cursor, Scale, FrameCount) { Color = Color, BGColor = BGColor };
                    BoxStyle style = new BoxStyle();

                    if (monBlock.Options.TryGetValue("scale", out opt)) mk.Scale = opt.FloatValue;
                    if (monBlock.Options.TryGetValue("padding", out opt)) style.Padding = opt.FloatValue;
                    if (monBlock.Options.TryGetValue("color", out opt)) mk.Color = opt.ColorValue;
                    if (monBlock.Options.TryGetValue("bgcolor", out opt)) mk.BGColor = opt.ColorValue;

                    mk.TextBox(style, monBlock.Text);
                    Sprites.AddRange(mk.Sprites);

                    Cursor.Y += mk.Box.Height;
                    return mk.Box;
                }

                public BoundingBox2 AppendMonitorBlock(IMonitorBlock monb)
                {
                    BoundingBox2 box = new BoundingBox2(Cursor, Cursor);
                    bool debug = monb.Options.ContainsKey("debug");
                    Vector2 preCursor = Cursor;
                    MonitorOption opt;


                    if (monb.GetType() == typeof(TextBlock))
                    {
                        box = AppendTextBlock(monb as TextBlock);
                    }
                    else
                    {
                        int column = 1;

                        if (monb.Options.TryGetValue("col", out opt)) column = opt.IntValue;

                        int col = 0;
                        BoundingBox2 box2 = new BoundingBox2(Cursor, Cursor);

                        foreach (MonitorInfo info in monb.BlockInfos)
                        {
                            if (col >= column)
                            {
                                col = 0;

                                Cursor.X = preCursor.X;
                                Cursor.Y += box2.Height;

                                if (debug)
                                {
                                    MySprite rect = MySprite.CreateSprite(SpriteId.SquareHollow, box2.Center, box2.Size);
                                    rect.Color = new Color(0, 255, 255, 64);
                                    Sprites.Add(rect);
                                }

                                box = Merge(box, box2);
                                box2 = new BoundingBox2(Cursor, Cursor);
                            }

                            box2 = Merge(box2, AppendMonitorContents(monb, info));

                            col++;

                        }

                        box = Merge(box, box2);
                        Cursor.X = preCursor.X;
                    }

                    if (debug)
                    {
                        MySprite rect = MySprite.CreateSprite(SpriteId.SquareHollow, box.Center, box.Size);
                        rect.Color = new Color(0, 0, 255, 64);
                        Sprites.Add(rect);
                    }

                    Cursor.Y = preCursor.Y + box.Height;
                    return box;
                }

                public BoundingBox2 AppendMonitorContents(IMonitorBlock monb, MonitorInfo info)
                {
                    BoundingBox2 box = new BoundingBox2(Cursor, Cursor);
                    BoundingBox2 box2 = new BoundingBox2(Cursor, Cursor);
                    bool debug = monb.Options.ContainsKey("debug");
                    Vector2 preCursor = Cursor;
                    SpriteBuilder mk;
                    BoxStyle style;
                    MonitorOption opt;
                    float padding = Padding;

                    mk = new SpriteBuilder(Surface, Cursor, Scale, FrameCount)
                    {
                        Color = Color,
                        BGColor = BGColor
                    };

                    if (monb.Options.TryGetValue("scale", out opt)) mk.Scale = opt.FloatValue;
                    if (monb.Options.TryGetValue("padding", out opt)) padding = opt.FloatValue;
                    if (monb.Options.TryGetValue("color", out opt)) mk.Color = opt.ColorValue;
                    if (monb.Options.TryGetValue("bgcolor", out opt)) mk.BGColor = opt.ColorValue;

                    if (monb.Options.ContainsKey("sicon"))
                    {
                        style = new BoxStyle(28, 28, 2);
                        box2 = AppendBlockIcon(mk, style, monb, info);
                    }
                    else if (monb.Options.ContainsKey("icon"))
                    {
                        style = new BoxStyle(56, 56, 4);
                        box2 = AppendBlockIcon(mk, style, monb, info);
                    }

                    mk = new SpriteBuilder(Surface, Cursor, Scale, FrameCount)
                    {
                        Color = Color,
                        BGColor = BGColor
                    };

                    if (monb.Options.TryGetValue("scale", out opt)) mk.Scale = opt.FloatValue;
                    if (monb.Options.TryGetValue("padding", out opt)) padding = opt.FloatValue;
                    if (monb.Options.TryGetValue("color", out opt)) mk.Color = opt.ColorValue;
                    if (monb.Options.TryGetValue("bgcolor", out opt)) mk.BGColor = opt.ColorValue;

                    if (monb.Options.ContainsKey("stscolor")) mk.Color = GetStatusColor(info, mk.Color);

                    Vector2 cursor = mk.Cursor;
                    SpriteBuilder sb;

                    foreach (List<MonitorContent> contentList in monb.Contents)
                    {

                        sb = new SpriteBuilder(mk, cursor);

                        foreach (MonitorContent content in contentList)
                        {

                            if (content.Width == null) style = new BoxStyle(content.Align, padding);
                            else style = new BoxStyle(new Vector2((float)content.Width, 29), content.Align, padding);

                            if (!CRProvider.TryRender(sb, style, content, info)) sb.EmptyBox(style);

                        }

                        cursor.Y += sb.Box.Height;

                        //mk.Cursor.X = cursor.X;
                        //mk.Cursor.Y = Cursor.Y + box.Height;

                        Sprites.AddRange(sb.Sprites);
                        box = Merge(box, sb.Box);

                    }

                    box = Merge(box, box2);

                    if (debug)
                    {
                        MySprite rect = MySprite.CreateSprite(SpriteId.SquareHollow, box.Center, box.Size);
                        int index = 0;
                        InfoItem item; if (info.TryGetValue("block", out item)) index = (item as CommonInfo).Index;

                        if (index % 2 == 0)
                            rect.Color = new Color(255, 0, 0, 64);
                        else
                            rect.Color = new Color(255, 255, 0, 64);

                        Sprites.Add(rect);
                    }

                    Cursor.X = preCursor.X + box.Width;
                    return box;
                }
                // ----
                public BoundingBox2 AppendBlockIcon(SpriteBuilder maker, BoxStyle style, IMonitorBlock monitorBlock, MonitorInfo info)
                {
                    BoundingBox2 box = new BoundingBox2(Cursor, Cursor);
                    SpriteBuilder sb = new SpriteBuilder(maker);

                    InfoItem item; if (info.TryGetValue("block", out item))
                    {
                        CommonInfo commonInfo = item as CommonInfo;
                        string icon = "";

                        if (commonInfo.IsBeingHacked)
                        {
                            if ((FrameCount % 1.0f) < 0.5f) icon = SpriteId.Danger;
                            else icon = SpriteId.Cross;
                        }
                        else if (!commonInfo.IsFunctional) icon = SpriteId.Cross;
                        //else if (!info.BlockInfo.IsEnabled)
                        //    icon = SpriteId.Construction;
                        else if (!commonInfo.IsWorking) icon = SpriteId.Danger;

                        if (icon != "")
                        {
                            sb.SpriteBox(style, icon);
                            //box = maker.SpriteBox(Sprites, style, icon);
                        }
                        else
                        {
                            SpriteDataBuilder sdb; if (IconRes.TryGetValue(commonInfo.Icon, out sdb)) sb.SpriteDataBox(style, sdb);
                            //box = maker.SpriteDataBox(Sprites, style, sdb);
                        }
                    }

                    Cursor.X += sb.Box.Width;
                    //Cursor.X += box.Width;
                    Sprites.AddRange(sb.Sprites);
                    return sb.Box;
                }
                private Color GetStatusColor(MonitorInfo info, Color defaultColor)
                {
                    Color color = defaultColor;

                    InfoItem item; if (info.TryGetValue("block", out item))
                    {
                        CommonInfo commonInfo = item as CommonInfo;
                        if (commonInfo.IsBeingHacked)
                        {
                            if ((FrameCount % 1.0f) < 0.5f)
                                color = Color.Red;
                            else
                                color = Color.Yellow;
                        }
                        else if (!commonInfo.IsFunctional)
                            color = Color.Red;
                        //else if (!info.BlockInfo.IsEnabled)
                        //    color = Color.Gray;
                        else if (!commonInfo.IsWorking)
                            color = Color.Yellow;
                    }

                    return color;
                }
                // ----
                private BoundingBox2 Merge(BoundingBox2 box1, BoundingBox2 box2)
                {
                    return BoundingBox2.CreateMerged(box1, box2);
                }
            }
        }
    }
}
