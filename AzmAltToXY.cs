using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllSky_2020
{
    public static class AzmAltToXY
    {
        public static Point CalculateXYPoint(double Azm, double Alt, double ObjectRadius)
        {
            int X, Y;
            int A = (int)(500 / 2.0), B = (int)(500 / 2.0);
            Azm = Azm * Math.PI / 180.0;
            Alt = Alt * Math.PI / 180.0;
            Azm = Azm - (Math.PI / 2.0);
            Double R = CalculateRadius(Alt);
            X = Convert.ToInt32(A + (R * -1) * Math.Cos(Azm));
            Y = Convert.ToInt32(B + R * Math.Sin(Azm));
            Point point = new Point(X - Convert.ToInt32(ObjectRadius), Y - Convert.ToInt32(ObjectRadius));
            return (point);
        }


        private static Double CalculateRadius(Double El)
        {
            Double Radius = 0;
            if (El > 0)
                try
                {
                    Radius = El * (500 / 4.0) / (Double)(Math.PI / 4.0);
                    Radius = 500 / 2.0 - Radius;
                }
                catch { }
            else
                Radius = 500 / 2.0;
            return Radius;
        }
    }
}
