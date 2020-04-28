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
        public class SpriteBuilder
        {
            public IMyTextSurface Surface { get; }
            public List<MySprite> Sprites { get; } = new List<MySprite>();

            public string FontId { get; set; } = "DEBUG";
            public float FontScale { get; set; } = 1.0f;
            public Color Color { get; set; } = new Color();
            public Color BGColor { get; set; } = new Color();

            private Vector2 _cursor = new Vector2();
            public Vector2 Cursor { get { return _cursor; } protected set { _cursor = value; } }
            public BoundingBox2 Box { get; protected set; }

            public float Scale { get; set; } = 1.0f;
            public float FrameCount { get; set; } = 0.0f;

            public SpriteBuilder(IMyTextSurface surface)
            {
                Surface = surface;
                Color = surface.ScriptForegroundColor;
                BGColor = surface.ScriptBackgroundColor;
                Box = new BoundingBox2(Cursor, Cursor);
            }
            public SpriteBuilder(IMyTextSurface surface, Vector2 pos, float scale = 1.0f, float frame = 0) : this(surface)
            {
                Cursor = pos;
                Box = new BoundingBox2(Cursor, Cursor);
                Scale = scale;
                FrameCount = frame;
            }
            public SpriteBuilder(SpriteBuilder sb) : this(sb, sb.Cursor) { }
            public SpriteBuilder(SpriteBuilder sb, Vector2 pos)
            {
                Surface = sb.Surface;
                FontId = sb.FontId; FontScale = sb.FontScale;
                Color = sb.Color; BGColor = sb.BGColor;
                Cursor = pos;
                Box = new BoundingBox2(Cursor, Cursor);
                Scale = sb.Scale;
                FrameCount = sb.FrameCount;
            }

            public void EmptyBox(BoxStyle style)
            {
                Vector2 size = style.Size * Scale;
                BoundingBox2 box = new BoundingBox2(Cursor, Cursor + size);

                MoveCursor(box.Width);
                MergeBox(box);
            }

            public void TextBox(BoxStyle style, string text)
            {
                if (style.SizeRule == SizeRule.Auto)
                {
                    style = new BoxStyle(style) { Size = CalcStringSize(text, FontId, FontScale) };
                }

                Vector2 size = style.Size * Scale;
                TextAlignment align = style.Align;
                MySprite sprite;
                BoundingBox2 box = new BoundingBox2(Cursor, Cursor + size);
                Vector2 innerSize = style.InnerSize * Scale;

                float textScale;
                if (size.Y != 0)
                    textScale = innerSize.Y / size.Y;
                else
                    textScale = 0;

                Vector2 textPos = new Vector2(box.Center.X, box.Center.Y - innerSize.Y / 2.0f);
                if (align == TextAlignment.LEFT)
                    textPos.X -= innerSize.X / 2.0f;
                else if (align == TextAlignment.RIGHT)
                    textPos.X += innerSize.X / 2.0f;

                if (Sprites != null)
                {
                    //sprite = MySprite.CreateSprite(SpriteId.SquareSimple, box.Center, box.Size);
                    //sprite.Color = BGColor;
                    //Sprites.Add(sprite);

                    sprite = MySprite.CreateText(text, FontId, Color, textScale * FontScale * Scale, align);
                    sprite.Position = textPos;
                    Sprites.Add(sprite);
                }

                MoveCursor(box.Width);
                MergeBox(box);
            }

            public void SpriteDataBox(BoxStyle style, SpriteDataBuilder data)
            {
                Vector2 size = style.Size * Scale;
                BoundingBox2 box = new BoundingBox2(Cursor, Cursor + size);

                if (Sprites != null)
                {
                    data.BuildSprites(Sprites, box.Center, style.InnerSize * Scale, FrameCount, Color, BGColor);
                }

                MoveCursor(box.Width);
                MergeBox(box);
            }

            public void SpriteBox(BoxStyle style, string spriteId)
            {
                BoundingBox2 box = new BoundingBox2(Cursor, Cursor + style.Size * Scale);

                if (Sprites != null)
                {
                    Sprites.Add(new MySprite(SpriteType.TEXTURE, spriteId, box.Center, style.InnerSize * Scale, Color));
                }

                MoveCursor(box.Width);
                MergeBox(box);
            }

            public void HorizonalBarBox(BoxStyle style, float value, int separate = 8, bool discrete = false)
            {
                if (style.SizeRule == SizeRule.Auto)
                {
                    style = new BoxStyle(style) { Size = new Vector2(100, 29) };
                }

                Vector2 size = style.Size * Scale;
                BoundingBox2 box = new BoundingBox2(Cursor, Cursor + size);

                if (Sprites != null)
                {
                    Vector2 innerSize = style.InnerSize * Scale;
                    MySprite sprite;

                    sprite = MySprite.CreateSprite(SpriteId.SquareHollow, new Vector2(), new Vector2(1.0f, 1.0f));
                    sprite.Color = Color;
                    sprite.Position = sprite.Position * innerSize + box.Center;
                    sprite.Size *= innerSize;
                    Sprites.Add(sprite);

                    //sprite = MySprite.CreateText($"value={value}", FontId, Color, 1.0f, TextAlignment.LEFT);
                    //sprite.Position = new Vector2(-0.45f, -0.45f);
                    //sprite.Position = sprite.Position * size + center;
                    //sprites.Add(sprite);

                    if (value > 1.0f) { value = 1.0f; }
                    if (value < 0.0f) { value = 0.0f; }
                    if (Single.IsNaN(value)) { value = 0.0f; }

                    double len = value * separate;
                    float delta = (float)(len - Math.Floor(len));
                    Vector2 deltaVec = new Vector2(delta, 1.0f);

                    int top;

                    if (discrete)
                        top = Convert.ToInt32(len);
                    else
                        top = Convert.ToInt32(Math.Floor(len));

                    Vector2 cellPos = new Vector2(0, 0.0f);
                    Vector2 cellSize = new Vector2(0.9f / separate, 0.9f);
                    //Vector2 innerCellSize = new Vector2(cellSize.X * 0.8f, cellSize.Y * 0.8f);
                    Vector2 innerCellSize = new Vector2(cellSize.X - 0.04f, cellSize.Y * 0.8f);

                    for (int i = 0; i < separate; i++)
                    {
                        if (((discrete) && (i < top)) || ((!discrete) && (i <= top)))
                        {
                            //cellPos.X = 0.92f * (i - (int)((separate - 1)/2)) / separate;

                            cellPos.X = (0.0f - (0.9f / 2)) + (cellSize.X / 2) + (i * cellSize.X);

                            if (i == top)
                            {
                                cellPos.X += ((delta - 1) * innerCellSize.X) / 2.0f;
                                sprite = MySprite.CreateSprite(SpriteId.SquareSimple, cellPos, innerCellSize * deltaVec);
                            }
                            else
                            {
                                sprite = MySprite.CreateSprite(SpriteId.SquareSimple, cellPos, innerCellSize);
                            }
                            //sprite.Color = new Color(0, 255, 255);
                            sprite.Color = Color;
                            sprite.Position = sprite.Position * innerSize + box.Center;
                            sprite.Size *= innerSize;
                            Sprites.Add(sprite);
                        }
                    }
                }

                MoveCursor(box.Width);
                MergeBox(box);
            }

            public void MoveCursor(float x, float y = 0.0f) { _cursor.X += x; _cursor.Y += y; }
            public void MergeBox(BoundingBox2 box) { Box = BoundingBox2.CreateMerged(Box, box); }

            public static Vector2 CalcStringSize(string text, string fontId, float scale)
            {
                //float chx = (512.0f - 20.48f) / 17.0f / 2.0f * scale;
                //float chy = (512.0f - 20.48f) / 17.0f * scale;
                float chx = 14.5f * scale;
                float chy = 29.0f * scale;

                Vector2 size = new Vector2();
                float x, y = chy;
                int i = 0, j;

                while (true)
                {
                    j = text.IndexOf('\n', i);
                    size.Y += y;

                    if (j == -1)
                    {
                        x = (text.Length - i) * chx;
                        if (x > size.X) { size.X = x; }
                        break;
                    }
                    else
                    {
                        x = (j - i) * chx;
                        if (x > size.X) { size.X = x; }
                        i = j + 1;
                    }
                }

                return size;
            }
        }
    }
}
