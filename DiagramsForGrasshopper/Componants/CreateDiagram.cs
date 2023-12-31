﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using DiagramLibrary;
using System.Drawing;


// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace DiagramsForGrasshopper
{
    public class CreateDiagram : ReportBaseComponent
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public CreateDiagram()
          : base("CreateDiagram", "CreateDiagram",
              "Description",
              "Display", "Diagram")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Height", "H", "Diagram Height in Pixels", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Width", "W", "Diagram Width in Pixels", GH_ParamAccess.item);
            pManager.AddGenericParameter("DiagramObject", "DO", "Diagram object or Rhino Geometry To Add to Diagram", GH_ParamAccess.list);
            pManager.AddTextParameter("Tile", "T", "Optional Diagram Title", GH_ParamAccess.item, "");
            pManager.AddColourParameter("Colour", "T", "Optional Background Colour", GH_ParamAccess.item, System.Drawing.Color.Transparent);


        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            base.RegisterOutputParams(pManager);
            pManager.AddGenericParameter("Diagram", "D", "The Diagram, use CanvasDiagram or RhinoDiagram Componants to view the diagram", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int width = -1;
            int height = -1;
            List<Object> objs = new List<Object>();
            string title = "";
            Color clr = System.Drawing.Color.Transparent;

            DA.GetData(0, ref width);
            DA.GetData(1, ref height);
            
            DA.GetDataList(2, objs);
            DA.GetData(3, ref title);
            DA.GetData(4, ref clr);





            if (width == double.NaN)
                {
                    AddUsefulMessage(DA, "Either set a valid [Width, Height and Location] or [Rectangle], width cannot be NaN");
                    return;
                }
                if (width < 0)
                {
                    AddUsefulMessage(DA, "Either set a valid [Width, Height and Location] or [Rectangle], width cannot be negative");
                    return;
                }
                if (height == double.NaN)
                {
                    AddUsefulMessage(DA, "Either set a valid [Width, Height and Location] or [Rectangle], height cannot be NaN");
                    return;
                }
                if (height < 0)
                {
                    AddUsefulMessage(DA, "Either set a valid [Width, Height and Location] or [Rectangle], height cannot be negative");
                    return;
                }
          
           


            Diagram diagram = Diagram.Create(width, height, title, clr);
            diagram.AddObjects(objs, GH_Component.DocumentTolerance());





            DA.SetData(1, diagram);

        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("67321aae-32da-4e18-8efc-e0d94f763c78"); }
        }
    }
}
