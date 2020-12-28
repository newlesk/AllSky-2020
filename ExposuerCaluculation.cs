using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllSky_2020
{
    public class ExposuerCaluculation
    {       
        double ComputeISO(double aperture, double shutterSpeed, double ev)
        {
            return (Math.Sqrt(aperture) * 100.0) / (shutterSpeed * Math.Pow(2.0, ev));
        }

        double ComputeEV(double aperture, double shutterSpeed, double iso)
        {
            return Math.Log(((Math.Sqrt(aperture) * 100.0) / (shutterSpeed * iso)), 2);
        }

        double ComputeTargetEV(double averageLuminance)
        {
            double K = 12.5;
            return Math.Log((averageLuminance * 100.0 / K), 2);
        }

        double ApplyAperturePriority(double focalLength, double targetEV, double aperture, double iso)
        {
            double shutterSpeed = 1.0 / (focalLength * 1000.0);
            iso = Clamp(ComputeISO(aperture, shutterSpeed, targetEV), AppSetting.Data.MIN_ISO, AppSetting.Data.MAX_ISO);
            double evDiff = targetEV - ComputeEV(aperture, shutterSpeed, iso);

            shutterSpeed = Clamp(shutterSpeed * Math.Pow(Math.Sqrt(2.0f), -evDiff), AppSetting.Data.MIN_SHUTTER, AppSetting.Data.MAX_SHUTTER);

            return shutterSpeed;
        }

        void ApplyShutterPriority(double focalLength, double targetEV, double aperture, double shutterSpeed, double iso)
        {
            aperture = 4.0;
            iso = Clamp(ComputeISO(aperture, shutterSpeed, targetEV), AppSetting.Data.MIN_ISO, AppSetting.Data.MAX_ISO);
            double evDiff = targetEV - ComputeEV(aperture, shutterSpeed, iso);

            aperture = Clamp(aperture * Math.Pow(Math.Sqrt(2.0), evDiff), AppSetting.Data.MIN_APERTURE, AppSetting.Data.MIN_APERTURE);
        }

        void ApplyProgramAuto(double focalLength, double targetEV, double aperture, double shutterSpeed, double iso)
        {
            aperture = 4.0;
            shutterSpeed = 1.0 / (focalLength * 1000.0);
            iso = Clamp(ComputeISO(aperture, shutterSpeed, targetEV), AppSetting.Data.MIN_ISO, AppSetting.Data.MAX_ISO);

            double evDiff = targetEV - ComputeEV(aperture, shutterSpeed, iso);
            aperture = Clamp(aperture * Math.Pow(Math.Sqrt(2.0), evDiff * 0.5), AppSetting.Data.MIN_APERTURE, AppSetting.Data.MIN_APERTURE);

            evDiff = targetEV - ComputeEV(aperture, shutterSpeed, iso);
            shutterSpeed = Clamp(shutterSpeed * Math.Pow(2.0, -evDiff), AppSetting.Data.MIN_SHUTTER, AppSetting.Data.MAX_SHUTTER);
        }

        double Clamp(double InputValue, double Min, double Max)
        {
            if (InputValue < Min)
                InputValue = Min;
            else if (InputValue > Max)
                InputValue = Max;

            return InputValue;
        }
    }
}
