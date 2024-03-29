﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramLibrary
{
  public   class DiagramColour
    {
        public static Color ColorFromHSV(double hue, double saturation, double value)
        {

                      //The ranges are 0 - 360 for hue, and 0 - 1 for saturation or value
            int hueInterval = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double hueFractional = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int valueInt = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - hueFractional * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - hueFractional) * saturation));

            if (hueInterval == 0)
                return Color.FromArgb(255, valueInt, t, p);
            else if (hueInterval == 1)
                return Color.FromArgb(255, q, valueInt, p);
            else if (hueInterval == 2)
                return Color.FromArgb(255, p, valueInt, t);
            else if (hueInterval == 3)
                return Color.FromArgb(255, p, q, valueInt);
            else if (hueInterval == 4)
                return Color.FromArgb(255, t, p, valueInt);
            else
                return Color.FromArgb(255, valueInt, p, q);

        }

        public static List<Color> GetColors(int count) {
            //To Do support starting colourss
            List<Color> clrs = new List<Color>();
            Random rand = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < count; i++)
            {
                double h =  rand.NextDouble() * 360;
                double s = rand.NextDouble();
                double v = rand.NextDouble();

                clrs.Add(DiagramColour.ColorFromHSV(h, s, v));
            }

            return clrs;

        }

        public static List<Color> GetColors(int count, Color startingColour)
        {
           
            List<Color> clrs = new List<Color>();
                     for (int i = 0; i < count; i++)
            {
               
                clrs.Add(startingColour);
            }
            return clrs;

        }


        /*
        public static List<Color> GetColors(int count, List<Color> StartingColours)
        {
            //To Do support starting colourss
            throw new NotImplementedException();

        }

    */






    }
}
