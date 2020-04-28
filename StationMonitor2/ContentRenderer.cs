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
        public interface IContentRenderer
        {
            void Render(SpriteBuilder sb, BoxStyle style, MonitorContent content);
        }

        public interface IInfoContentRenderer : IContentRenderer
        {
            void Render(SpriteBuilder sb, BoxStyle style, MonitorContent content, InfoItem info);
        }

        public abstract class InfoContentRendererBase : IInfoContentRenderer
        {
            public void Render(SpriteBuilder sb, BoxStyle style, MonitorContent content) { }
            public abstract void Render(SpriteBuilder sb, BoxStyle style, MonitorContent content, InfoItem info);
        }
        public class InfoContentRenderer<T> : InfoContentRendererBase
        where T : InfoItem
        {
            public override void Render(SpriteBuilder sb, BoxStyle style, MonitorContent content, InfoItem info) { Render(sb, style, content, info as T); }
            public void Render(SpriteBuilder sb, BoxStyle style, MonitorContent content, T info)
            {
                if ((info is ValueInfoBase) && (content.Subtype == "bar"))
                {
                    sb.HorizonalBarBox(style, (info as ValueInfoBase).Ratio, (content.IntValue > 0) ? content.IntValue : 8);
                }
                else
                {
                    string text = info.ToText(content);
                    if (text.Length > 0) sb.TextBox(style, text);
                    else sb.EmptyBox(style);
                }
            }
        }

        public class TextContentRenderer : IContentRenderer
        {
            public void Render(SpriteBuilder sb, BoxStyle style, MonitorContent content)
            {
                sb.TextBox(style, content.StrValue);
            }
        }
    }
}
