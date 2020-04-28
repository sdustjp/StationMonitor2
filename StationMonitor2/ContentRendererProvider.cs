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
        public class ContentRendererProvider : Dictionary<string, Func<IContentRenderer>>
        {
            public void Add(Func<IContentRenderer> func, params string[] keys) { foreach (string key in keys) Add(key, func); }
            public bool TryRender(SpriteBuilder sb, BoxStyle style, MonitorContent content, MonitorInfo info)
            {
                Func<IContentRenderer> provider; if (TryGetValue(content.Type, out provider))
                {
                    IContentRenderer renderer = provider();
                    if (renderer is IInfoContentRenderer) { InfoItem item; if (info.TryGetValue(content.Type, out item)) { (renderer as IInfoContentRenderer).Render(sb, style, content, item); return true; } }
                    else { renderer.Render(sb, style, content); return true; }
                }
                return false;
            }
        }

        public static ContentRendererProvider CRProvider { get; set; } = new ContentRendererProvider();

        public static void LoadStandardCRProvider()
        {
            CRProvider.Add(() => { return new TextContentRenderer(); }, "text");
            CRProvider.Add(() => { return new InfoContentRenderer<CommonInfo>(); }, "block");
            CRProvider.Add(() => { return new InfoContentRenderer<Store>(); }, "sto");
            CRProvider.Add(() => { return new InfoContentRenderer<Output>(); }, "out");
            CRProvider.Add(() => { return new InfoContentRenderer<Efficiency>(); }, "eff");
            CRProvider.Add(() => { return new InfoContentRenderer<Input>(); }, "in");
            CRProvider.Add(() => { return new InfoContentRenderer<Charge>(); }, "cha");

            CRProvider.Add(() => { return new InfoContentRenderer<Production>(); }, "product");
            CRProvider.Add(() => { return new InfoContentRenderer<Assembler>(); }, "assemble");
            CRProvider.Add(() => { return new InfoContentRenderer<VentInfo>(); }, "vent");
            CRProvider.Add(() => { return new InfoContentRenderer<DoorInfo>(); }, "door");
            CRProvider.Add(() => { return new InfoContentRenderer<ConnectorInfo>(); }, "connector");
        }
    }
}
