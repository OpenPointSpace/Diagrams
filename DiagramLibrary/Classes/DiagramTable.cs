﻿using Grasshopper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

namespace DiagramLibrary
{
    public class DiagramTable : DiagramObject
    {
        GH_Structure<GH_String> m_Data = new GH_Structure<GH_String>();
        List<double> m_CellWidths = new List<double>();
        List<double> m_CellHeights = new List<double>();

        private PointF m_Location;
        private string m_FontName = "Arial";
        private float m_Padding = 0f;
        private float m_TextSize = 1f;
        private TextJustification m_Justification = TextJustification.None;


     


        public static DiagramTable Create(GH_Structure<GH_String> data, List<double> cellWidths, List<double> cellHeights, float textSize, PointF Location, Color Colour, float LineWeight, string fontName, float padding, TextJustification justification)
        {
            DiagramTable diagramTable = new DiagramTable();
            diagramTable.m_Colour = Colour;
            diagramTable.m_LineWeight = LineWeight;
            diagramTable.m_Data = data;
            diagramTable.m_CellWidths = cellWidths;
            diagramTable.m_CellHeights = cellHeights;
            diagramTable.m_TextSize = textSize;
            diagramTable.m_Location = Location;
            diagramTable.m_FontName = fontName;
            diagramTable.m_Padding = padding;
            diagramTable.m_Justification = justification;
            return diagramTable;
        }

        public override DiagramObject Duplicate()
        {
            DiagramTable diagramTable = new DiagramTable();
            diagramTable.m_Location = m_Location;
            diagramTable.m_Colour = m_Colour;
            diagramTable.m_LineWeight = m_LineWeight;
            diagramTable.m_Data = m_Data;
            diagramTable.m_CellWidths = m_CellWidths;
            diagramTable.m_CellHeights = m_CellHeights;
            diagramTable.m_TextSize = m_TextSize;
            diagramTable.m_FontName = m_FontName;

            diagramTable.m_Padding = m_Padding;
            diagramTable.m_Justification = m_Justification;
            return diagramTable;
        }

        public string ToCSV() {
            string returnString = "";
            for (int i = 0; i < this.m_Data.Branches.Count; i++)
            {
                
                for (int j = 0; j < this.m_Data.Branches[i].Count; j++)
                {
                    returnString += this.m_Data.Branches[i][j].Value + ",";
                }
                returnString += "\n";

            }
            return returnString;
        }

        public override BoundingBox GetBoundingBox()
        {
        
            GetSizes(out SizeF[] sizes, out Rectangle3d[] recs);

            BoundingBox totalRec = recs[0].BoundingBox;

            for (int i = 1; i < recs.Length; i++)
            {
                totalRec.Union(recs[i].BoundingBox);
                    
                  
            }

            totalRec.Union(new Point3d(m_Location.X,m_Location.Y,0));

            return totalRec;

        }


            public void GetSizes(out SizeF[] sizes, out Rectangle3d[] recs)
        {

            int numberOfCols = 0;
            int numberOfRows = 0;

            //calculate sizes
            for (int i = 0; i < this.m_Data.Branches.Count; i++)
            {
                numberOfRows++;
                for (int j = numberOfCols; j < this.m_Data.Branches[i].Count; j++)
                {
                    numberOfCols++;
                }

            }

            sizes = new SizeF[numberOfCols * numberOfRows];
            recs = new Rectangle3d[numberOfCols * numberOfRows];

            double currentX = 0;
            double currnetY = 0;


            for (int rowIndex = 0; rowIndex < numberOfRows; rowIndex++)
            {
                float width = 100; //Absolute Defaults
                float height = 50; //Absolute Defaults

                currentX = 0;
                for (int colIndex = 0; colIndex < numberOfCols; colIndex++)
                {

                    if (m_CellWidths.Count > colIndex)
                    {
                        //Try and Find Width
                        width = (float)m_CellWidths[colIndex];
                    }
                    else
                    {
                        //Fall back on the first width given
                        if (m_CellWidths.Count > 0)
                        {
                            width = (float)m_CellWidths[0];
                        }
                    }


                    if (m_CellHeights.Count > rowIndex)
                    {
                        //Try and Find hieght
                        height = (float)m_CellHeights[rowIndex];
                    }
                    else
                    {
                        //Fall back on the first height given
                        if (m_CellHeights.Count > 0)
                        {
                            height = (float)m_CellHeights[0];
                        }
                    }

                    sizes[rowIndex * numberOfCols + colIndex] = new SizeF(width, height);
                    recs[rowIndex * numberOfCols + colIndex] = new Rectangle3d(new Plane(new Point3d(m_Location.X + currentX, m_Location.Y + currnetY, 0), Plane.WorldXY.ZAxis), width, height);
                    currentX += width;

                }
                currnetY += height;


            }
        }


        public override void DrawBitmap(Graphics g)
        {


            //calculate sizes
            GetSizes(out SizeF[] sizes, out Rectangle3d[] recs);



            //draw lines
            for (int i = 0; i < recs.Length; i++)
            {
                var crv = DiagramCurve.Create(recs[i].ToNurbsCurve(), m_Colour, m_LineWeight);
                crv.DrawBitmap(g);
            }


            //draw text
            int currentIndex = 0;
            for (int i = 0; i < this.m_Data.Branches.Count; i++)
            {

                for (int j = 0; j < this.m_Data.Branches[i].Count; j++)
                {
                    var txt = DiagramText.Create(this.m_Data.Branches[i][j].Value, new PointF((float)recs[currentIndex].Plane.Origin.X, (float)recs[currentIndex].Plane.Origin.Y), m_Colour, m_TextSize, TextJustification.BottomLeft, Color.Transparent, Color.Transparent,-1,  m_FontName, sizes[currentIndex], m_Padding, m_Justification);
                    txt.DrawBitmap(g);
                    currentIndex++;
                }

            }

        }




        public override void DrawRhinoPreview(Rhino.Display.DisplayPipeline pipeline, double tolerance, Transform xform, bool colorOverride)
        {


            //calculate sizes
            GetSizes(out SizeF[] sizes, out Rectangle3d[] recs);



            //draw lines
            for (int i = 0; i < recs.Length; i++)
            {
                var crv = DiagramCurve.Create(recs[i].ToNurbsCurve(), m_Colour, m_LineWeight);
                crv.DrawRhinoPreview(pipeline, tolerance, xform, colorOverride);
            }


            //draw text
            int currentIndex = 0;
            for (int i = 0; i < this.m_Data.Branches.Count; i++)
            {

                for (int j = 0; j < this.m_Data.Branches[i].Count; j++)
                {
                    var txt = DiagramText.Create(this.m_Data.Branches[i][j].Value, new PointF((float)recs[currentIndex].Plane.Origin.X, (float)recs[currentIndex].Plane.Origin.Y), m_Colour, m_TextSize, TextJustification.BottomLeft, Color.Transparent, Color.Transparent, -1, m_FontName, sizes[currentIndex], m_Padding, m_Justification);
                    txt.DrawRhinoPreview(pipeline, tolerance, xform, colorOverride);
                    currentIndex++;
                }

            }





        }
    }



}