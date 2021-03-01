using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllSky_2020
{
    class AppSetting
    {
        public class Settings
        {
            public int ROIX { get; set; } = 0;
            public int ROIY { get; set; } = 0;
            public int CameraId { get; set; } = 0;
            public bool IS_DISPLAY_ORGIN { get; set; } = false;
            public bool IS_DISPLAY_AREA { get; set; } = false;
            public double ROIWidth { get; set; } = 0.0;
            public string SaveFileDialog { get; set; } = @"C:\";
            public double ROIHeight { get; set; } = 0.0;
            public double ImageWidth { get; set; } = 0.0;
            public double ImageHeight { get; set; } = 0.0;
            public double ImageSize { get; set; } = 0.0;
            public string NameCamera { get; set; } = null;
            public double OriginX { get; set; } = 0.0;
            public double OriginY { get; set; } = 0.0;
            public double OriginWidth { get; set; } = 0.0;
            public double OriginHeight { get; set; } = 0.0;
            public double TEMPERATURE { get; set; } = 0.0;
            public double ExposureTime { get; set; } = 100;
            public double ZoomScale { get; set; } = 1;
            public int MaxLight { get; set; } = 125;
            public int MinLight { get; set; } = 100;
            public int HdrDetectionHigh { get; set; } = 30000;
            public int HdrDetectionLow { get; set; } = 300000;
            public int hdrPixelvalueshigh { get; set; } = 255;
            public int hdrPixelvalueslow { get; set; } = 60;
            public double MIN_ISO { get; set; } = 0.0;        //100
            public double MAX_ISO { get; set; } = 3200;        //3200
            public double MIN_SHUTTER { get; set; } = 0.0;    //0.25 sec
            public double MAX_SHUTTER { get; set; } = 120000;    //50 sec
            public double MIN_APERTURE { get; set; } = 0.0;   //3.8
            public double LATITUDE { get; set; } = 0.0;
            public double LONGTITUDE { get; set; } = 0.0;
        }

        public static Settings Data;
        private static String SettingPart = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\System_config.json";

        public static void LoadSetting()
        {
            Data = new Settings();

            if (!File.Exists(SettingPart))
            {
                File.Create(SettingPart).Close();
                Save();
            }

            string JsonString = "";

            using (StreamReader r = new StreamReader(SettingPart))
            {
                JsonString = r.ReadToEnd();
            }

            if (JsonString != "")
            {
                Data = JsonConvert.DeserializeObject<Settings>(JsonString);
            }
            else
            {
                File.Create(SettingPart).Close();
                Save();
            }
        }

        public static Boolean Save()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(SettingPart))
                {
                    String DataJson = JsonConvert.SerializeObject(Data, Formatting.Indented);
                    sw.WriteLine(DataJson);
                    sw.Close();

                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Save configulation file error. Because " + e.Message);
                return false;
            }
        }
    }
}
