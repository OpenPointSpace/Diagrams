﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;


namespace DiagramLibrary
{
    public class DiagramText : DiagramObject
    {
        protected string m_Text;
        protected PointF m_Location;
        protected TextJustification m_Anchor;
        protected DiagramFilledRectangle m_Mask = null;
        protected float m_TextSize;

        protected string m_FontName = "Arial";
        protected SizeF m_WrapSize;
        protected float m_Padding = 0f;
        protected TextJustification m_Justification = TextJustification.None;


        public PointF Location
        {
            get { return m_Location; }  
            set { m_Location = value; } 
        }

        public string Text
        {
            get { return m_Text; }
            set { m_Text = value; }
        }

        public string FontName
        {
            get { return m_FontName; }
            set { m_FontName = value; }
        }


        public float TextSize
        {
            get { return m_TextSize; }
            set { m_TextSize = value; }
        }




        public static DiagramText Create(string Text, PointF Location,float textSize)
        {
            DiagramText diagramText = new DiagramText();
            diagramText.m_Colour = Diagram.DefaultColor;
            diagramText.m_TextSize = textSize;
            diagramText.m_Text = Text;
            diagramText.m_Location = Location;
            diagramText.m_Anchor = TextJustification.BottomLeft;
            diagramText.m_Mask = DiagramFilledRectangle.Create(new Rectangle3d(Plane.WorldXY, 1, 1), Color.Transparent, Diagram.DefaultColor, 0);//Size is updated at drawtime

            diagramText.m_FontName = "Arial";

            diagramText.m_WrapSize = new Size(-1,-1);
            diagramText.m_Padding = 3f;
            diagramText.m_Justification = TextJustification.BottomLeft;
            return diagramText;
        }


        public static DiagramText Create(string Text, PointF Location, Color Colour, float textSize,
            TextJustification anchor, Color maskColour, Color frameColor,float frameLineWeight, 
            string fontName, SizeF wrapSize, float padding, TextJustification justification)
        {
            DiagramText diagramText = new DiagramText();
            diagramText.m_Colour = Colour;
            diagramText.m_TextSize = textSize;
            diagramText.m_Text = Text;
            diagramText.m_Location = Location;
            diagramText.m_Anchor = anchor;
            diagramText.m_Mask = DiagramFilledRectangle.Create(new Rectangle3d(Plane.WorldXY,1,1),maskColour, frameColor,frameLineWeight);//Size is updated at drawtime

            diagramText.m_FontName = fontName;
     
            diagramText.m_WrapSize = wrapSize;
            diagramText.m_Padding = padding;
            diagramText.m_Justification = justification;
            return diagramText;
        }

        public override DiagramObject Duplicate()
        {
            DiagramText diagramText = new DiagramText();
            diagramText.m_Location = m_Location;
            diagramText.m_Colour = m_Colour;
            diagramText.m_TextSize = m_TextSize;
            diagramText.m_Text = m_Text;
            diagramText.m_Anchor = m_Anchor;
            diagramText.m_Mask = m_Mask;
                       diagramText.m_FontName = m_FontName;
         
            diagramText.m_WrapSize = m_WrapSize;
            diagramText.m_Padding = m_Padding;
            diagramText.m_Justification = m_Justification;
            return diagramText;
        }

        public void SetMask(Color color,bool DrawLine,Color LineColor, float LineWeight)
        {
            m_Mask = DiagramFilledRectangle.Create(new Rectangle3d(Plane.WorldXY, 10, 10), color, LineColor, LineWeight);//Size is updated at drawtime
           
        }

     
        public PointF GetAnchorCompensatedPoint(SizeF size) {
           PointF newPoint;
            switch (m_Anchor)
            {
                case TextJustification.None:
                    newPoint = new PointF(m_Location.X - m_Padding, m_TextSize);
                    break;
                case TextJustification.Left:
                    newPoint = new PointF(m_Location.X - m_Padding, m_TextSize);
                    break;
                case TextJustification.Center:
                    newPoint = new PointF(m_Location.X - (size.Width / 2),m_Location.Y);
                    break;
                case TextJustification.Right:
                     newPoint= new PointF(m_Location.X - size.Width, m_Location.Y);
                    break;
                case TextJustification.Bottom:
                    newPoint = m_Location;
                    break;
                case TextJustification.Middle:
                    newPoint = new PointF(m_Location.X , m_Location.Y - (size.Height / 2));
                    break;
                case TextJustification.Top:
                    newPoint = new PointF(m_Location.X, m_Location.Y - size.Height);
                    break;
                case TextJustification.BottomLeft:
                    newPoint = m_Location;
                    break;
                case TextJustification.BottomCenter:
                    newPoint = new PointF(m_Location.X - (size.Width / 2), m_Location.Y);
                    break;
                case TextJustification.BottomRight:
                    newPoint = new PointF(m_Location.X - size.Width, m_Location.Y);
                    break;
                case TextJustification.MiddleLeft:
                    newPoint = new PointF(m_Location.X, m_Location.Y - (size.Height / 2));
                    break;
                case TextJustification.MiddleCenter:
                    newPoint = new PointF(m_Location.X - (size.Width / 2), m_Location.Y - (size.Height / 2));
                    break;
                case TextJustification.MiddleRight:
                    newPoint = new PointF(m_Location.X- size.Width, m_Location.Y - (size.Height / 2));
                    break;
                case TextJustification.TopLeft:
                    newPoint = new PointF(m_Location.X, m_Location.Y - size.Height);
                    break;
                case TextJustification.TopCenter:
                    newPoint = new PointF(m_Location.X - (size.Width / 2), m_Location.Y - size.Height);
                    break;
                case TextJustification.TopRight:
                    newPoint = new PointF(m_Location.X - size.Width, m_Location.Y - size.Height);
                    break;
                default:
                    newPoint= new PointF(m_Location.X - m_Padding, m_TextSize);
                    break;
            }

            return newPoint;// new PointF(newPoint.X - m_Padding, newPoint.Y - m_Padding);
        }



        public override BoundingBox GetBoundingBox()
        {
            SizeF totalSize;

            //Create a dummy graphics, for the measure text
            using (var g = Graphics.FromImage(new Bitmap(10, 10)))
            {
                SizeF tempSize = CalculteTextSize(g, out totalSize, out List<string> lines, out List<SizeF> rowSizes);

            }

            return new BoundingBox(m_Location.X, m_Location.Y, 0, m_Location.X + totalSize.Width, m_Location.Y + totalSize.Height, 0);
        }


    


        public List<string> CalulateTextLines(Graphics g, Font font, SizeF maxSize, StringFormat format, out SizeF totalSize, out List<SizeF> rowSizes)
        {
            List<string> outputStrings = new List<string>();
            rowSizes = new List<SizeF>();

            string[] words = m_Text.Split(' ');

            string currentLine = "";
            float currentHeight = 0;
            int lineSpacing = font.FontFamily.GetLineSpacing(FontStyle.Regular);
            float lineSpacingPixel = font.Size * lineSpacing / font.FontFamily.GetEmHeight(FontStyle.Regular);
            float maxWidth = 0;
            SizeF lastLineSize = SizeF.Empty;

            for (int i = 0; i < words.Length; i++)
            {
                string tempLine = currentLine + ' ' + words[i]; // Add each word
                SizeF textSize = g.MeasureString(tempLine, font, new SizeF(maxSize.Width, m_TextSize), format, out int charsFitted, out int linesFilled);


                if (maxSize.Height > 0 && currentHeight + textSize.Height >= maxSize.Height)
                {
                    break;
                }

                if (charsFitted != tempLine.Length)
                {
                    outputStrings.Add(currentLine);
                    rowSizes.Add(lastLineSize);
                    currentHeight += lineSpacingPixel;
                    

                    currentLine = words[i];
                    bool isWordTooWide = true;

                    while (isWordTooWide)
                    {
                        SizeF textSize2 = g.MeasureString(currentLine, font, maxSize, format, out int charsFitted2, out int linesFilled2);

                        if (charsFitted2 == 0)
                        {
                            isWordTooWide = false;
                        }

                        if (charsFitted2 != currentLine.Length)
                        {
                            outputStrings.Add(currentLine.Substring(0, charsFitted2));
                            rowSizes.Add(textSize2);
                            currentHeight += lineSpacingPixel;
                           
                            currentLine = currentLine.Substring(charsFitted2 + 1);
                        }
                        else
                        {
                            isWordTooWide = false;

                            if (textSize2.Width > maxWidth)
                            {
                                maxWidth = textSize2.Width;
                            }
                        }
                    }
                }
                else
                {
                    if (textSize.Width > maxWidth)
                    {
                        maxWidth = textSize.Width;
                    }

                    currentLine = tempLine;
                    lastLineSize = textSize;
                }
            }

            
                outputStrings.Add(currentLine);
                rowSizes.Add(g.MeasureString(currentLine, font, maxSize, format));
                currentHeight += lineSpacingPixel;
            
            totalSize = new SizeF(maxWidth, currentHeight);

            return outputStrings;
        }



        public SizeF CalculteTextSize(Graphics g , out SizeF maskSize , out List<string> lines, out List<SizeF> rowSizes) {
            if (m_TextSize <= 0) {
                maskSize = SizeF.Empty;
                lines = new List<string>();
                rowSizes = new List<SizeF>();
                return SizeF.Empty;
            }

            SizeF allowedTextSize = SizeF.Empty;
            var font = new System.Drawing.Font(m_FontName,m_TextSize);

            StringFormat format = new StringFormat();




            if (m_WrapSize.Width < 0)
            {
                //Item is Not rapped, in this case we want the allowed text to be be as big as it needs and the padding is added on top,


                SizeF textSize_NotConstrained = g.MeasureString(m_Text, font);
                maskSize = new SizeF(textSize_NotConstrained.Width + m_Padding + m_Padding, textSize_NotConstrained.Height + m_Padding + m_Padding);
                lines = new List<string> { m_Text };
                rowSizes = new List<SizeF> { textSize_NotConstrained };
                return textSize_NotConstrained;
               
              
            }
                //Item is Wrapped

                if (m_WrapSize.Height < 0)
                {

                //In this case we want the padding to be added to the Height but subracted from the width
                
                lines = CalulateTextLines(g, font, new SizeF(m_WrapSize.Width - m_Padding - m_Padding,-1), format, out SizeF measuredSize, out rowSizes);
                maskSize = new SizeF(m_WrapSize.Width, measuredSize.Height + m_Padding + m_Padding);
                return measuredSize;
            }

            //In this case we want the padding to be subracted from the Height but subracted from the width
            maskSize = m_WrapSize;
            lines = CalulateTextLines(g, font, new SizeF(m_WrapSize.Width - m_Padding - m_Padding, m_WrapSize.Height - m_Padding - m_Padding), format, out SizeF textSize_ConstrainedWidthAndHeight , out rowSizes);
            return textSize_ConstrainedWidthAndHeight;







        }


        public override void DrawBitmap(Grasshopper.Kernel.GH_Component component, Graphics g)
        {

            var font = new System.Drawing.Font(m_FontName, m_TextSize);

            SizeF actualTotalTextSize = CalculteTextSize(g, out SizeF maskSize, out List<string> lines, out List<SizeF> rowSizes);
                       
            PointF anchorCompensatedPoint = GetAnchorCompensatedPoint(maskSize);

            bool maskEnabled = m_Mask.LineWeight > 0;
            if (maskEnabled)
            {
                m_Mask.UpdateRectangle(anchorCompensatedPoint, maskSize);
                m_Mask.DrawBitmap(component,g);
            }

            int lineSpacing = font.FontFamily.GetLineSpacing(FontStyle.Regular);
            float lineSpacingPixel = font.Size * lineSpacing / font.FontFamily.GetEmHeight(FontStyle.Regular);


            float allowedTextSize = maskSize.Height - m_Padding - m_Padding;
            float heightSizeDiffence = allowedTextSize - actualTotalTextSize.Height;
            float verticalFustificationCompensation = 0;

            if (m_Justification == TextJustification.Middle || m_Justification == TextJustification.MiddleLeft || m_Justification == TextJustification.MiddleCenter || m_Justification == TextJustification.MiddleRight)
            {
                verticalFustificationCompensation += heightSizeDiffence / 2;
            }

            if (m_Justification == TextJustification.Top || m_Justification == TextJustification.TopLeft || m_Justification == TextJustification.TopCenter || m_Justification == TextJustification.TopRight)
            {
                verticalFustificationCompensation += heightSizeDiffence;
            }



            //We have to draw this upside down as Bitmap is top to bottom but Rhino is bottom to top, to compensate we mirror-y the image for the canvas, but we have to draw the text upside down so it is correct when flipped.

            g.ScaleTransform(1, -1);// Begin Upside Down,
            
            for (int i = 0; i < lines.Count; i++)
            {
                SizeF rowSize = rowSizes[i];
                float allowedTextSizeWidth = maskSize.Width - m_Padding - m_Padding;

                float widthSizeDiffernce = allowedTextSizeWidth - rowSize.Width;
               
                float justificationCompensation = 0;

                
                if (m_Justification == TextJustification.Center || m_Justification == TextJustification.BottomCenter || m_Justification == TextJustification.MiddleCenter || m_Justification == TextJustification.TopCenter) {
                    justificationCompensation += widthSizeDiffernce / 2;
                }

                if (m_Justification == TextJustification.Right || m_Justification == TextJustification.BottomRight || m_Justification == TextJustification.MiddleRight || m_Justification == TextJustification.TopRight) {
                    justificationCompensation += widthSizeDiffernce;
                }

                //Add Padding and Justification Compensation, note Y is negative and signes are reverse as we drawing this upside down
                PointF pt = new PointF(anchorCompensatedPoint.X + m_Padding + justificationCompensation, -anchorCompensatedPoint.Y - m_Padding - actualTotalTextSize.Height - verticalFustificationCompensation + (lineSpacingPixel * i));

                //  g.DrawString(lines[i], font, this.GetBrush(), new RectangleF(pt, actualTotalTextSize));
                g.DrawString(lines[i], font, this.GetBrush(), new RectangleF(pt, new SizeF(actualTotalTextSize.Width, actualTotalTextSize.Height + (m_TextSize * 0.1F))));
            }
            g.ResetTransform();// End Upside Down



        }

        public Brush GetBrush()
        {
           
                return new SolidBrush(m_Colour);

          
           
        }




        public override void DrawRhinoPreview(Grasshopper.Kernel.GH_Component component, Rhino.Display.DisplayPipeline pipeline, double tolerance, Transform xform,bool colorOverride, Rhino.RhinoDoc doc, bool Bake)
        {


            SizeF actualTotalTextSize = SizeF.Empty;
            SizeF maskSize = SizeF.Empty;

            Color clr = Diagram.SelectedColor;
            bool drawLines = m_Mask.LineWeight > 0;
            if (colorOverride == false)
            {
                clr = m_Colour;
            }
            else
            {
                drawLines = true;
            }


            List<string> lines;
            List<SizeF> rowSizes;

            //Create a dummy graphics, for the measure text
            using (var g = Graphics.FromImage(new Bitmap(10, 10)))
            {
                actualTotalTextSize = CalculteTextSize(g,  out maskSize, out lines, out rowSizes);

               
            }

            var font = new System.Drawing.Font(m_FontName, m_TextSize);
            int lineSpacing = font.FontFamily.GetLineSpacing(FontStyle.Regular);
            float lineSpacingPixel = font.Size * lineSpacing / font.FontFamily.GetEmHeight(FontStyle.Regular);


            float allowedTextSizeHeight = maskSize.Height - m_Padding - m_Padding;
            float heightSizeDiffence = allowedTextSizeHeight -  actualTotalTextSize.Height;
            float verticalFustificationCompensation = 0;

            if (m_Justification == TextJustification.Middle || m_Justification == TextJustification.MiddleLeft || m_Justification == TextJustification.MiddleCenter || m_Justification == TextJustification.MiddleRight)
            {
                verticalFustificationCompensation += heightSizeDiffence / 2;
            }

            if (m_Justification == TextJustification.Top || m_Justification == TextJustification.TopLeft || m_Justification == TextJustification.TopCenter || m_Justification == TextJustification.TopRight)
            {
                verticalFustificationCompensation += heightSizeDiffence;
            }




            PointF anchorCompensatedPoint = GetAnchorCompensatedPoint(maskSize);

            if (drawLines && m_Mask != null)
            {

                m_Mask.UpdateRectangle(anchorCompensatedPoint, maskSize);
                m_Mask.DrawRhinoPreview(component,pipeline, tolerance, xform, colorOverride,  doc,  Bake);
            }

            for (int i = 0; i < lines.Count; i++)
            {
                TextEntity txt = new TextEntity();
                txt.PlainText = lines[i];

                txt.Font = new Rhino.DocObjects.Font(m_FontName, Rhino.DocObjects.Font.FontWeight.Normal, Rhino.DocObjects.Font.FontStyle.Upright, false, false);
                Plane pln = Plane.WorldXY;
                txt.Plane = pln;
                txt.Justification = m_Anchor;
                txt.TextHeight = m_TextSize;



                SizeF rowSize = rowSizes[i];
                float allowedTextSizeWidth = maskSize.Width - m_Padding - m_Padding;

                float widthSizeDiffernce = allowedTextSizeWidth - rowSize.Width;
                float justificationCompensation = 0;

                if (m_Justification == TextJustification.Center || m_Justification == TextJustification.BottomCenter || m_Justification == TextJustification.MiddleCenter || m_Justification == TextJustification.TopCenter)
                {
                    justificationCompensation += widthSizeDiffernce / 2;
                }

                if (m_Justification == TextJustification.Right || m_Justification == TextJustification.BottomRight || m_Justification == TextJustification.MiddleRight || m_Justification == TextJustification.TopRight)
                {
                    justificationCompensation += widthSizeDiffernce;
                }

                //Add Padding and Justification Compensation, note Y is negative and signes are reverse as we drawing this upside down



                Point3d pt = new Point3d(anchorCompensatedPoint.X + m_Padding + justificationCompensation, anchorCompensatedPoint.Y + m_Padding + verticalFustificationCompensation + (lineSpacingPixel * (lines.Count - i)), 0);
                var scale = Transform.Scale(new Point3d(pt.X, pt.Y, 0), m_TextSize);
                var trans = Transform.Translation(new Vector3d(pt.X, pt.Y, 0));
                var localXform = scale * trans;
                var combinedXform = localXform;
                if (xform != Transform.ZeroTransformation)
                {
                    combinedXform = Transform.Multiply(xform, localXform);
                }

                if (Bake)
                {
                    var attr = new Rhino.DocObjects.ObjectAttributes();
                    attr.ColorSource = Rhino.DocObjects.ObjectColorSource.ColorFromObject;
                    attr.ObjectColor = clr;
                    txt.Transform(combinedXform);
                    doc.Objects.AddText(txt, attr);
                }
                else
                {
                    pipeline.DrawText(txt, clr, combinedXform); //scale*trans order matters
                }
            }
                                 

        }

    }
}
