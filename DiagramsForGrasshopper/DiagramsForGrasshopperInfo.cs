﻿using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace DiagramsForGrasshopper
{
    public class DiagramsForGrasshopperInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "DiagramsForGrasshopper";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("bb2ca9eb-5fab-4448-a952-5d648b2a9771");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "";
            }
        }
    }
}
