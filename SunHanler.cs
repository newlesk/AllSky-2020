using Astro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllSky_2020
{
    public static class SunHanler
    {
        private static Sun _Sun = new Sun();
        private static double JD = 0.0;

        public static AltAz GetSunPosition()
        {
            JD = AstroTime.JulianDay(DateTime.UtcNow);
            RaDec SunCoordinate = _Sun.GetRaDec(JD);
            //LatLon StationCoordinate = new LatLon(Angle.FromDegs(AppSetting.Data.LATITUDE), Angle.FromDegs(AppSetting.Data.LONGTITUDE));
            LatLon StationCoordinate = new LatLon(Angle.FromDegs(18.852325), Angle.FromDegs(98.957644));

            AltAz SunAltAz = SunCoordinate.ToAltAz(StationCoordinate, JD);
            //System.Diagnostics.Debug.WriteLine("SunAltAz =" + SunAltAz);
            return SunAltAz;
        }
    }
}
