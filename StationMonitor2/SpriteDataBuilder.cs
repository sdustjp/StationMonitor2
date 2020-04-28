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
        public class SpriteDataBuilder
        {
            private List<SpriteData> dlist = new List<SpriteData>();

            public SpriteDataBuilder Add(SpriteData data)
            {
                dlist.Add(data);
                return this;
            }

            public SpriteDataBuilder Add(string id, float posX, float posY, float sizeX, float sizeY, float alpha, SpriteData next = null)
            {
                dlist.Add(new SpriteData(id, posX, posY, sizeX, sizeY, alpha, 0, 0, next));
                return this;
            }
            public SpriteDataBuilder Add(string id, float posX, float posY, float sizeX, float sizeY, float alpha, float rotate, SpriteData next = null)
            {
                dlist.Add(new SpriteData(id, posX, posY, sizeX, sizeY, alpha, rotate, 0, next));
                return this;
            }
            public SpriteDataBuilder Add(string id, float posX, float posY, float sizeX, float sizeY, float alpha, float rotate, float posRot, SpriteData next = null)
            {
                dlist.Add(new SpriteData(id, posX, posY, sizeX, sizeY, alpha, rotate, posRot, next));
                return this;
            }

            public SpriteDataBuilder Add(string id, float posX, float posY, float sizeX, float sizeY, float alpha, float posX2, float posY2, float sizeX2, float sizeY2, float alpha2)
            {
                dlist.Add(new SpriteData(id, posX, posY, sizeX, sizeY, alpha, 0, 0, new SpriteData(id, posX2, posY2, sizeX2, sizeY2, alpha2, 0, 0)));
                return this;
            }
            public SpriteDataBuilder Add(string id, float posX, float posY, float sizeX, float sizeY, float alpha, float rotate, float posX2, float posY2, float sizeX2, float sizeY2, float alpha2, float rotate2)
            {
                dlist.Add(new SpriteData(id, posX, posY, sizeX, sizeY, alpha, rotate, 0, new SpriteData(id, posX2, posY2, sizeX2, sizeY2, alpha2, rotate2, 0)));
                return this;
            }
            public SpriteDataBuilder Add(string id, float posX, float posY, float sizeX, float sizeY, float alpha, float rotate, float posRot, float posX2, float posY2, float sizeX2, float sizeY2, float alpha2, float rotate2, float posRot2)
            {
                dlist.Add(new SpriteData(id, posX, posY, sizeX, sizeY, alpha, rotate, posRot, new SpriteData(id, posX2, posY2, sizeX2, sizeY2, alpha2, rotate2, posRot2)));
                return this;
            }

            public SpriteData[] GetSpriteData()
            {
                return dlist.ToArray();
            }

            public void BuildSprites(List<MySprite> sprites, Vector2 center, Vector2 size, float frame = 0, Color? color = null, Color? bgColor = null)
            {
                Color fcolor = color ?? Color.White;
                Color bcolor = bgColor ?? Color.Black;
                MySprite sprite;
                foreach (SpriteData d in dlist)
                {
                    if (d.Next == null)
                    {
                        Vector2 pos = d.Pos;
                        pos.Rotate(d.PosRot);
                        sprite = MySprite.CreateSprite(d.Id, pos * size + center, d.Size * size);
                        sprite.Color = new Color(fcolor.ToVector3() * d.Alpha + bcolor.ToVector3() * (1.0f - d.Alpha));
                        sprite.RotationOrScale = d.Rotate;
                    }
                    else
                    {
                        Vector2 pos = Vector2.SmoothStep(d.Pos, d.Next.Pos, frame);
                        pos.Rotate(d.PosRot + d.Next.PosRot * frame);
                        sprite = MySprite.CreateSprite(d.Id, pos * size + center, d.Size * size);
                        float alpha = d.Alpha + (d.Next.Alpha - d.Alpha) * frame;
                        sprite.Color = new Color(fcolor.ToVector3() * alpha + bcolor.ToVector3() * (1.0f - alpha));
                        sprite.RotationOrScale = d.Rotate + d.Next.Rotate * frame;
                    }
                    sprites.Add(sprite);
                }
            }
        }
    }
}
