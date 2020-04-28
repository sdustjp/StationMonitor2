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
    partial class Program : MyGridProgram
    {
        #region mdk macros
        // ========================================================================================
        //  Station Monitor v2
        //
        //  author sdust
        //  update $MDK_DATETIME$
        // ========================================================================================
        #endregion

        // ----------------------------------------------------------------------------------------
        //  Configuration
        // ----------------------------------------------------------------------------------------
        const string PROGRAM_NAME = "Station Monitor v2.0 G alpha 6";
        #region mdk macros
        const string PROGRAM_UPDATE = "$MDK_DATETIME$";
        #endregion

        const string DEFAULT_PANEL_KEYWORD = "[GSM]";
        //const string DEFAULT_PANEL_KEYWORD = "[GSMTest]";

        const bool AUTO_UPDATE = true;
        const bool DEBUG = false;

        // ----------------------------------------------------------------------------------------
        //  Global
        // ----------------------------------------------------------------------------------------

        GraphicalStationMonitor GSM;

        bool ExecSearch = false;
        bool ExecUpdate = false;
        bool ExecDraw = false;

        // ----------------------------------------------------------------------------------------
        //  Program
        // ----------------------------------------------------------------------------------------
        public Program()
        {
            GraphicalStationMonitor.LoadSpriteDatas();

            GSM = GraphicalStationMonitor.GetInstance(this);
            GasPlantMod.Load(GSM);
            PowerPlantMod.Load(GSM);
            FactoryMod.Load(GSM);
            ShipPortMod.Load(GSM);

            if (AUTO_UPDATE) Runtime.UpdateFrequency = UpdateFrequency.Update10;

            GSM.Search();
        }
        // ----------------------------------------------------------------------------------------
        //  Save
        // ----------------------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------
        //  Main
        // ----------------------------------------------------------------------------------------
        public void Main(string argument, UpdateType updateSource)
        {
            Echo("program start!");
            Echo(PROGRAM_NAME);
            Echo(PROGRAM_UPDATE);

            switch (updateSource)
            {
                case UpdateType.Update1:
                case UpdateType.Update10:
                case UpdateType.Update100:

                    Echo($"program count: {GSM.ProgramCount}");
                    Echo($"frame count: {GSM.FrameCount}");

                    if (GSM.ProgramCount % 50 == 0) ExecSearch = true;
                    if (GSM.ProgramCount % 10 == 0) ExecUpdate = true;
                    if (GSM.ProgramCount % 1 == 0) ExecDraw = true;

                    if (ExecSearch) { GSM.Search(); ExecSearch = false; }
                    if (ExecUpdate) { GSM.Update(); ExecUpdate = false; }
                    if (ExecDraw) { GSM.DrawSprites(); ExecDraw = false; }

                    GSM.CountUp();

                    break;

                case UpdateType.Terminal:
                case UpdateType.Trigger:

                    switch (argument)
                    {
                        case "search":
                            ExecSearch = true;
                            break;
                        case "update":
                            ExecUpdate = true;
                            break;
                        case "render":
                            ExecDraw = true;
                            break;
                    }

                    break;

            }

            Echo("program end!");
        }
        // ----------------------------------------------------------------------------------------
    }
}
