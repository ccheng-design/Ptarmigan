using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace _Ptarmigan
{
    public class MissionConfiguration : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public MissionConfiguration()
          : base("MissionConfiguration", "MissionConfig",
              "Creates the Mission Configuration for Flight Path Component",
              "Ptarmigan", "Drone")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("flyToWaylineMode", "WM", "Wayline Mode", GH_ParamAccess.item,"safely");
            pManager.AddTextParameter("finishAction", "FA", "Determines What the Drone does after it finishes its flightpath", GH_ParamAccess.item, "gotoFirstWaypoint");
            pManager.AddTextParameter("exitOnRCLost", "RCLost", "Action for when RC Controller disconnects from Drone", GH_ParamAccess.item, "executeLostAction");
            pManager.AddTextParameter("executeRCLostAction", "Lost Action", "Execute Lost Action", GH_ParamAccess.item);
            pManager.AddNumberParameter("globalTransitionSpeed", "TS", "Overall Global Transition Speed", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("missionConfig", "MC", "Mission Configuration presets for the Flight Path component", GH_ParamAccess.item);
            pManager.AddTextParameter("settings", "S", "Simplified View of the Settings that are Outputted; Use for Debugging", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //MISSION CONFIG

            string flyToWaylineMode = null;
            DA.GetData(0, ref flyToWaylineMode);

            string finishAction = null;
            DA.GetData(1, ref finishAction);

            string exitOnRCLost = null;
            DA.GetData(2, ref exitOnRCLost);

            string executeRCLostAction = null;
            DA.GetData(3, ref executeRCLostAction);

            double globalTransitionalSpeed = 1.5;
            DA.GetData(4, ref globalTransitionalSpeed);



            //Header Section
            string header = @"<?xml version=""1.0"" encoding=""UTF-8""?>" +
            Environment.NewLine +
            @"<kml xmlns=""http://www.opengis.net/kml/2.2"" xmlns:wpml=""http://www.uav.com/wpmz/1.0.2"">" + Environment.NewLine;


            //Document
            string document = @"    <Document>
        <wpml:missionConfig>
            <wpml:flyToWaylineMode>" + flyToWaylineMode + @"</wpml:flyToWaylineMode>
            <wpml:finishAction>" + finishAction + @"</wpml:finishAction>
            <wpml:exitOnRCLost>" + exitOnRCLost + @"</wpml:exitOnRCLost>
            <wpml:executeRCLostAction>" + executeRCLostAction + @"</wpml:executeRCLostAction>
            <wpml:globalTransitionalSpeed>" + globalTransitionalSpeed + @"</wpml:globalTransitionalSpeed>
            <wpml:droneInfo>
                <wpml:droneEnumValue>68</wpml:droneEnumValue>
                <wpml:droneSubEnumValue>0</wpml:droneSubEnumValue>
            </wpml:droneInfo>
        </wpml:missionConfig>";


            //Settings
            string settingmode =
            flyToWaylineMode + Environment.NewLine +
            finishAction + Environment.NewLine +
            exitOnRCLost + Environment.NewLine +
            executeRCLostAction + Environment.NewLine +
            globalTransitionalSpeed + " m/s";


            string missionConfig = header + document;

            string settings = settingmode;


            //OUTPUT
            DA.SetData(0, missionConfig);
            DA.SetData(1, settings);

        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("EC6B6273-3CF6-4BFD-A37A-9535FAEC7F6A"); }
        }
    }
}