﻿using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramLibrary
{
   public  class DiagramFilledCurve : BaseCurveDiagramObject
    {
      
        protected Color m_LineColor;
        protected Color m_BackColour;
        protected System.Drawing.Drawing2D.HatchStyle m_hatchStyle;
        protected bool isSolid = true;

        protected double m_hatchRotation = 0;

        protected double m_hatchScale = 1;

        protected Rhino.Display.DisplayMaterial m_CachedMaterial = null;
        protected Rhino.Display.DisplayMaterial m_CachedSelectedMaterial = null;

        protected List<DiagramCurve> m_InnerCurves =  new List<DiagramCurve>();
        protected List<DiagramCurve> m_OuterCurves = new List<DiagramCurve>();



        public Color BackColour
        {
            get { return m_BackColour; }
            set { m_BackColour = value;
                m_CachedMaterial = null;
            }
        }

        public override Color Colour
        {
            get { return m_Colour; }
    set {
                m_Colour = value;
                m_CachedMaterial = null;
            }
        }



        public static DiagramFilledCurve Create(Curve curve, Color Colour, Color LineColour, float LineWeight)
        {

            DiagramFilledCurve diagramFilledCurve = new DiagramFilledCurve();
            diagramFilledCurve.m_Colour = Colour;
            diagramFilledCurve.m_LineWeight = LineWeight;

            diagramFilledCurve.m_LineColor = LineColour;

         
                diagramFilledCurve.m_OuterCurves.Add(DiagramCurve.Create(curve, LineColour, LineWeight));

            return diagramFilledCurve;
        }




        public static DiagramFilledCurve Create(Curve[] OuterCurves, Curve[] InnerCurves, Color Colour, Color LineColour, float LineWeight)
        {

            DiagramFilledCurve diagramFilledCurve = new DiagramFilledCurve();
            diagramFilledCurve.m_Colour = Colour;
            diagramFilledCurve.m_LineWeight = LineWeight;
          
            diagramFilledCurve. m_LineColor = LineColour;

            for (int i = 0; i < OuterCurves.Length; i++)
            {
                diagramFilledCurve. m_OuterCurves.Add(DiagramCurve.Create(OuterCurves[i], LineColour, LineWeight));
            }
            if (InnerCurves != null)
            {
                for (int i = 0; i < InnerCurves.Length; i++)
                {
                    diagramFilledCurve.m_InnerCurves.Add(DiagramCurve.Create(InnerCurves[i], LineColour, LineWeight));
                }
            }


            return diagramFilledCurve;
        }

        public static List<DiagramFilledCurve> CreateFromBrep(Brep brep, Color Colour, Color LineColour, float LineWeight)
        {
            List<DiagramFilledCurve> hatches = new List<DiagramFilledCurve>();

            for (int i = 0; i < brep.Faces.Count; i++)
            {

                Curve[] crvsInner = brep.Faces[i].DuplicateFace(false).DuplicateNakedEdgeCurves(false, true);
                Curve[] crvsOuter = brep.Faces[i].DuplicateFace(false).DuplicateNakedEdgeCurves(true, false);


                DiagramFilledCurve dHatch = DiagramFilledCurve.Create(crvsOuter, crvsInner, Colour, LineColour, LineWeight);
                hatches.Add(dHatch);


            }

           

            return hatches;
        }

        public override DiagramObject Duplicate()
        {
            DiagramFilledCurve diagramFilledCurve = new DiagramFilledCurve();
            diagramFilledCurve.m_Colour = m_Colour;
            diagramFilledCurve.m_LineWeight = m_LineWeight;
                     diagramFilledCurve.m_LineColor = m_LineColor;

            for (int i = 0; i < m_OuterCurves.Count; i++)
            {
                diagramFilledCurve.m_OuterCurves.Add(m_OuterCurves[i].DuplicateDiagramCurve());
            }

            for (int i = 0; i < m_InnerCurves.Count; i++)
            {
                diagramFilledCurve.m_InnerCurves.Add(m_InnerCurves[i].DuplicateDiagramCurve());
            }
          
            return diagramFilledCurve;
        }



        public override BaseCurveDiagramObject SetLocationAndDirectionForDrawing(Point3d basePoint, Vector3d baseDirection, Point3d location, Vector3d rotation)
        {

            if (baseDirection == Vector3d.Unset)
            {
                return null;
            }


            DiagramFilledCurve clone = Duplicate() as DiagramFilledCurve;
                           

            for (int i = 0; i < clone.m_InnerCurves.Count; i++)

            {

                clone.m_InnerCurves[i].Curve.Translate(new Vector3d(location.X - basePoint.X, location.Y - basePoint.Y, 0));
                double angle = Vector3d.VectorAngle(baseDirection, rotation, Plane.WorldXY);
                clone.m_InnerCurves[i].Curve.Rotate(angle, Plane.WorldXY.Normal, location);

            }



            for (int i = 0; i < clone.m_OuterCurves.Count; i++)

            {

                clone.m_OuterCurves[i].Curve.Translate(new Vector3d(location.X - basePoint.X, location.Y - basePoint.Y, 0));
                double angle = Vector3d.VectorAngle(baseDirection, rotation, Plane.WorldXY);
                clone.m_OuterCurves[i].Curve.Rotate(angle, Plane.WorldXY.Normal, location);

            }



            return clone;
        }




        public Brush GetBrush()
        {
            if (isSolid)
            {
                return new SolidBrush(m_Colour);
                
            }
            else {
                return new System.Drawing.Drawing2D.HatchBrush(m_hatchStyle, m_Colour, m_BackColour);
            }
        }

        public override BoundingBox GetBoundingBox()
        {
            BoundingBox bbox = BoundingBox.Empty;
            for (int i = 0; i < this.m_InnerCurves.Count; i++)
            {
                bbox.Union(this.m_InnerCurves[i].GetBoundingBox());
            }

            for (int i = 0; i < this.m_OuterCurves.Count; i++)
            {
                bbox.Union(this.m_OuterCurves[i].GetBoundingBox());
            }

            return bbox;
        }


       


        public override PointF GetBoundingBoxLocation()
        {
            BoundingBox bbox = GetBoundingBox();
            return Diagram.ConvertPoint(bbox.Min);
        }



        public override void DrawBitmap( Grasshopper.Kernel.GH_Component component, Graphics g)

        {

            

            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
           

           List<Point3d> points3d = new List<Point3d>();

            foreach (DiagramCurve crv in m_OuterCurves)
            {
               
                PointF[] pts = crv.GetPoints();
              
                path.AddLines(pts);
            }

            foreach (DiagramCurve crv in m_InnerCurves)

            {
                System.Drawing.Drawing2D.GraphicsPath holdPath = new System.Drawing.Drawing2D.GraphicsPath();
                PointF[] pts = crv.GetPoints();

                holdPath.AddLines(pts);

                path.AddPath(holdPath, false);
            }




          
        
            g.FillPath(this.GetBrush(),path);

            if (m_LineWeight > 0) {
                g.DrawPath(this.GetPen(), path);

            }


        }


        public override void DrawRhinoPreview( Grasshopper.Kernel.GH_Component component, Rhino.Display.DisplayPipeline pipeline, double tolerance, Transform xform, bool colorOverride, Rhino.RhinoDoc doc, bool Bake)
        {

            Color clr = Diagram.SelectedColor;
            Rhino.Display.DisplayMaterial material = null;
          bool drawLines = m_LineWeight > 0;
            if (colorOverride == false)
            {
                clr = m_Colour;

                if (m_CachedMaterial == null)
                {
                    m_CachedMaterial = new Rhino.Display.DisplayMaterial(clr, 1.0 - (clr.A / 255));
                }

                material = m_CachedMaterial;
              


            }
            else {
                drawLines = true;

                if (m_CachedSelectedMaterial == null)
                {
                    m_CachedSelectedMaterial = new Rhino.Display.DisplayMaterial(clr, 1.0 - (clr.A / 255));
                }
                material = m_CachedSelectedMaterial;
            }


            

            List<DiagramCurve> dcrvs = new List<DiagramCurve>();
            dcrvs.AddRange(m_OuterCurves);
            dcrvs.AddRange(m_InnerCurves);
            Curve[] crvs = dcrvs.Select(x => (Curve)x.GetCurve()).ToArray();
             Brep[] breps = Brep.CreatePlanarBreps(crvs, tolerance);
            if (breps != null)
            {
                //Hatch Experiments to match bush
                // Rhino.DocObjects.HatchPattern pattern = Rhino.RhinoDoc.ActiveDoc.HatchPatterns[1];

                // Hatch[] hatches = Hatch.Create(crvs, 1, m_hatchRotation, m_hatchScale, tolerance);

                // var texture = new Rhino.DocObjects.Texture();
                // texture.TextureType = Rhino.DocObjects.TextureType.Bitmap;


            

                // material.SetBitmapTexture(texture)

                foreach (Brep item in breps)
                //  foreach (Hatch hatch in hatches)
                {
                    if (xform != Transform.ZeroTransformation)
                    {
                        item.Transform(xform);

                    }
                    //pipeline.DrawBrepShaded(item, mat);



                    if (Bake)
                    {
                        var attr = new Rhino.DocObjects.ObjectAttributes();
                        attr.ColorSource = Rhino.DocObjects.ObjectColorSource.ColorFromObject;
                        attr.ObjectColor = clr;
                        string name = "DiagramsMaterial_" + material.Diffuse.R.ToString() + "_" + material.Diffuse.G.ToString() + "_" + material.Diffuse.B.ToString() + "_" + material.Transparency.ToString();

                        int materialIndex =  doc.Materials.Find(name, true);

                        if (materialIndex < 0) {
                            var rhinoMaterial = new Rhino.DocObjects.Material();
                            rhinoMaterial.DiffuseColor = material.Diffuse;
                            rhinoMaterial.Transparency = material.Transparency;

                            rhinoMaterial.Name = name;
                               materialIndex = doc.Materials.Add(rhinoMaterial, false);
                        }
                       
                        attr.MaterialSource = Rhino.DocObjects.ObjectMaterialSource.MaterialFromObject;
                        attr.MaterialIndex = materialIndex;

                        doc.Objects.AddBrep(item, attr);
                    }
                    else
                    {
                      
                        pipeline.DrawBrepShaded(item, material);
                    }


                    
                }
            }



            if (drawLines) {
                foreach (var item in m_OuterCurves)
                {
                    item.DrawRhinoPreview( component,pipeline, tolerance,xform, colorOverride , doc,  Bake);
                }
                foreach (var item in m_InnerCurves)
                {
                    item.DrawRhinoPreview( component,pipeline, tolerance,xform, colorOverride, doc, Bake);
                }

            }


        }

       


        public new Pen GetPen()
        {
            return new Pen(m_LineColor, m_LineWeight);
        }

    }
}
