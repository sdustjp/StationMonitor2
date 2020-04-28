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
        public class SpriteData
        {
            public const float PI = 3.141592f;
            public const float PI_2 = PI * 2.0f;

            public string Id;
            public Vector2 Pos;
            public Vector2 Size;
            public float Alpha;
            public float Rotate;
            public float PosRot;
            public SpriteData Next;

            public SpriteData(string id, float posX, float posY, float sizeX, float sizeY, float alpha, SpriteData next = null) : this(id, posX, posY, sizeX, sizeY, alpha, 0.0f, 0.0f, next) { }
            public SpriteData(string id, float posX, float posY, float sizeX, float sizeY, float alpha, float rotate, SpriteData next = null) : this(id, posX, posY, sizeX, sizeY, alpha, rotate, 0.0f, next) { }
            public SpriteData(string id, float posX, float posY, float sizeX, float sizeY, float alpha, float rotate, float posRot, SpriteData next = null) { Id = id; Pos = new Vector2(posX, posY); Size = new Vector2(sizeX, sizeY); Alpha = alpha; Rotate = rotate; PosRot = posRot; Next = next; }
        }
    }
}
