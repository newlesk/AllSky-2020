using Astro;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.CV.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZWOptical.ASISDK;
using static ZWOptical.ASISDK.ASICameraDll2;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using Emgu.CV.Util;
using OpenCvSharp.Blob;
using OpenCvSharp.Extensions;
using OpenCvSharp;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace AllSky_2020
{
    public enum CAMERASTSTATE { CAPTURING, DRAWING }

    public partial class MainWindows : Form
    {
        public CAMERASTSTATE _CAMERASTSTATE;
        public string SystemMessage = "Ready.";
        private Image<Bgr, Byte> RootFrame = null, ProcessFrame = null, ROIFrame, CircleImage = null, KeoGramsFrame = null;
        private Image<Gray, Byte> HoughCirclesFrame = null, ProcessFrameGray = null;
        private Rectangle ROIRec;
        private Rectangle KeoGramsImage;
        private IntPtr ImageBuf;
        private bool GetImageState = false;
        private bool IsDefineROI = false;
        private bool IsDefineOrigin = false;
        private int CameraId, ConnectedCameras;
        public string SaveFileDialog;
        private int ColorValue;
        private double CentroidX, CentroidY;
        private ASI_CAMERA_INFO Info;
        private Stopwatch ExposureCounter;
        private string FolderName;
        private string TimeNow;
        private string TimeFolder;
        private string TimeBefore;
        private string TimeNowChack;
        private int HoughCirclesX, HoughCirclesY;
        private bool IsExpSuccess;
        private bool Recover;
        private double CannyThreshold;
        private double CircleAccumulatorThreshold;
        private double GoldenRatio = (1 + (Math.Sqrt(5)) / 2);
        private int HoughCirclesRadius = 0;
        private bool HoughCirclesStatus;
        private int MaxLightHdr;
        private int MinLightHdr;
        private Image<Bgr, Byte> HdrHigh = null;
        private Image<Bgr, Byte> HdrLow = null;
        private Image<Bgr, Byte> HdrMedium = null;
        private Image<Bgr, Byte> HdrOutput = null;
        private Image<Gray, Byte> CroppedImage = null;
        private Image<Bgr, Byte> ImageFrame = null;
        private Image<Bgr, Byte> HdrHighUp = null;
        private Image<Bgr, Byte> HdrHighUpper = null;
        //private Image<Bgr, Byte> NonHdr = null;
        private int MaxLight;
        private int MinLight;
        private bool HdrOn = false;
        private int ImageMergeStatus;
        private int ModeHDR = 0;
        private double HDRHighExposure = 0;


        public MainWindows()
        {
            InitializeComponent();
        }

        private void InitializeSystem()
        {
            ExposureCounter = new Stopwatch();
            LoadSetting();
            InitializeCamera();
            CaptureImage();
        }

        private void InitializeCamera()

        {
            ConnectedCameras = ASICameraDll2.ASIGetNumOfConnectedCameras();

            if (ConnectedCameras > 0)
            {
                CameraId = AppSetting.Data.CameraId;
                CameraConnection();
                AppSetting.Data.ImageWidth = Info.MaxWidth;
                AppSetting.Data.ImageHeight = Info.MaxHeight;
                AppSetting.Data.ImageSize = (AppSetting.Data.ImageWidth * AppSetting.Data.ImageHeight * 3);
                AppSetting.Save();
                if (Info.Name != "")
                {
                    for (int i = 0; i < ConnectedCameras; i++)
                    {

                        CameraList.Items.Add(Info.Name);

                    }
                    CameraList.Text = Info.Name;
                }
                else
                    CameraList.Items.Add("ERROR_GET_NAME");
            }
            else
            {
                if (ConnectedCameras > 0)
                {
                    SystemMessage = "Retring to connect to the camera.";
                    Application.Restart();

                }
                //Application.Restart();
                //InitializeCamera();
                if (ConnectedCameras == 0 && MessageBox.Show("No camera connected. Please check the cable.", "Message", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    Application.Restart();
                    //InitializeCamera();
                }
                else
                    Application.Exit();
            }
        }
        private void CameraConnection()
        {
            Recover = false;
            Info = new ASI_CAMERA_INFO();
            ASI_ERROR_CODE CAMProError = ASICameraDll2.ASIGetCameraProperty(out Info, CameraId);
            ASICameraDll2.ASIOpenCamera(CameraId);
            ASICameraDll2.ASIInitCamera(CameraId);
            ASICameraDll2.ASISetROIFormat(CameraId, Info.MaxWidth, Info.MaxHeight, 1, ASI_IMG_TYPE.ASI_IMG_RGB24);

            ASICameraDll2.ASISetControlValue(CameraId, ASI_CONTROL_TYPE.ASI_BANDWIDTHOVERLOAD, 40);
            //ASICameraDll2.ASISetControlValue(CameraId, ASI_CONTROL_TYPE.ASI_HIGH_SPEED_MODE, 0);
            ASICameraDll2.ASISetControlValue(CameraId, ASI_CONTROL_TYPE.ASI_AUTO_MAX_EXP, 10000000);
            ASICameraDll2.ASISetControlValue(CameraId, ASI_CONTROL_TYPE.ASI_GAIN, (int)AppSetting.Data.MIN_ISO);
            ASICameraDll2.ASISetControlValue(CameraId, ASI_CONTROL_TYPE.ASI_AUTO_MAX_GAIN, (int)AppSetting.Data.MAX_ISO);

            ASICameraDll2.ASISetControlValue(CameraId, ASI_CONTROL_TYPE.ASI_WB_R, 65);
            ASICameraDll2.ASISetControlValue(CameraId, ASI_CONTROL_TYPE.ASI_WB_B, 85);
            ASICameraDll2.ASISetControlValue(CameraId, ASI_CONTROL_TYPE.ASI_GAMMA, 50);
            ASICameraDll2.ASISetControlValue(CameraId, ASI_CONTROL_TYPE.ASI_BRIGHTNESS, 50);
            ASICameraDll2.ASISetControlValue(CameraId, ASI_CONTROL_TYPE.ASI_HIGH_SPEED_MODE, 0);
        }
        private void LoadSetting()
        {
            AppSetting.LoadSetting();

            ROITextX.Text = AppSetting.Data.ROIX.ToString();
            ROITextY.Text = AppSetting.Data.ROIY.ToString();
            ROITextWidth.Text = string.Format("{0:0}", AppSetting.Data.ImageWidth);
            ROITextHeight.Text = string.Format("{0:0}", AppSetting.Data.ImageHeight);
            ExpouseTimeText.Text = AppSetting.Data.ExposureTime.ToString();
            AreaDisplay.Checked = AppSetting.Data.IS_DISPLAY_AREA;
            OriginDisplay.Checked = AppSetting.Data.IS_DISPLAY_ORGIN;
            OriginXText.Text = string.Format("{0:0}", AppSetting.Data.OriginX);
            OriginYText.Text = string.Format("{0:0}", AppSetting.Data.OriginY);
            OriginWidthText.Text = string.Format("{0:0}", AppSetting.Data.OriginWidth);
            OriginHeightText.Text = string.Format("{0:0}", AppSetting.Data.OriginHeight);
            AreaTextX.Text = AppSetting.Data.ROIX.ToString();
            AreaTextY.Text = AppSetting.Data.ROIY.ToString();
            AreaTextWidth.Text = AppSetting.Data.ROIWidth.ToString();
            AreaTextHeight.Text = AppSetting.Data.ROIHeight.ToString();
            MIN_ISOText.Text = string.Format("{0:0}", AppSetting.Data.MIN_ISO);
            MAX_ISOText.Text = string.Format("{0:0}", AppSetting.Data.MAX_ISO);
            MIN_SHUTTERText.Text = string.Format("{0:0}", AppSetting.Data.MIN_SHUTTER);
            MAX_SHUTTERText.Text = string.Format("{0:0}", AppSetting.Data.MAX_SHUTTER);
            MIN_APERTUREText.Text = string.Format("{0:0}", AppSetting.Data.MIN_APERTURE);
            pixelvalues_min.Text = AppSetting.Data.MinLight.ToString();
            pixelvalues_max.Text = AppSetting.Data.MaxLight.ToString();
            HoughCircles_Profile.Text = "Select Your Profile";
            ProfilePixelValues.Text = "Select Your Profile";
            MinLightHdr = AppSetting.Data.MinLight;
            MaxLightHdr = AppSetting.Data.MaxLight;
            HdrDetectionHigh.Text = AppSetting.Data.HdrDetectionHigh.ToString();
            HdrDetectionLow.Text = AppSetting.Data.HdrDetectionLow.ToString();
            int HoughCircleslineCount = File.ReadLines(@"./HoughCircles_Profile.txt").Count();
            int PixellineCount = File.ReadLines(@"./Pixel_Profile.txt").Count();
            pixelValuesHighHDR.Text = AppSetting.Data.hdrPixelvalueshigh.ToString();
            pixelValuesLowHDR.Text = AppSetting.Data.hdrPixelvalueslow.ToString();
            AutoISO.Checked = true;


            if (IsAutoExposureTime.CheckState == 0)
            {
                IsAutoExposureTime.Checked = true;
                FocusPoint.Text = "21 Focus Points";
                SpeedMode.Checked = true;
            }
            for (int i = 0; i < (HoughCircleslineCount / 2); i++)
            {

                HoughCircles_Profile.Items.Add("Profile " + i);

            }
            for (int i = 0; i < (PixellineCount / 2); i++)
            {
                ProfilePixelValues.Items.Add("Profile " + i);
            }
        }

        private void CalculateZoomScale()
        {
            if (RootFrame == null || MainImageControl.Image == null)
                return;

            int ControlSize = 0;

            if (MainImageControl.Image.Size.Width > MainImageControl.Image.Size.Height)
            {
                ControlSize = MainImageControl.Image.Size.Width;
                AppSetting.Data.ZoomScale = (double)MainImageControl.Size.Width / (double)RootFrame.Size.Width;
            }
            else
            {
                ControlSize = MainImageControl.Image.Size.Height;
                AppSetting.Data.ZoomScale = (double)MainImageControl.Size.Height / (double)RootFrame.Size.Height;
            }

            MainImageControl.SetZoomScale(AppSetting.Data.ZoomScale, new System.Drawing.Point(0, 0));

        }

        private void CaptureImage()
        {

            Task Capdatatain = Task.Run(async () =>
            {
            STARTPROCESS:


                if (ConnectedCameras > 0)
                {

                    SystemMessage = "Ready.";

                    int ExpTime = Convert.ToInt32(AppSetting.Data.ExposureTime * 1000.0);

                    bool IsExpIdel = false;
                    ASICameraDll2.ASISetControlValue(CameraId, ASI_CONTROL_TYPE.ASI_EXPOSURE, ExpTime);
                    ASI_EXPOSURE_STATUS ExpStatus = ASI_EXPOSURE_STATUS.ASI_EXP_IDLE;
                    ASI_ERROR_CODE GetExpError = ASI_ERROR_CODE.ASI_SUCCESS;

                    while (true)
                    {

                        GetExpError = ASICameraDll2.ASIGetExpStatus(CameraId, out ExpStatus);
                        if (ExpStatus == ASI_EXPOSURE_STATUS.ASI_EXP_IDLE && GetExpError == ASI_ERROR_CODE.ASI_SUCCESS)
                        {
                            IsExpIdel = true;
                            break;
                        }
                        else if (ExpStatus == ASI_EXPOSURE_STATUS.ASI_EXP_FAILED && GetExpError != ASI_ERROR_CODE.ASI_SUCCESS)
                            break;

                    }

                    if (IsExpIdel)
                    {

                        ExposureCounter.Restart();
                        ASICameraDll2.ASIStartExposure(CameraId, ASI_BOOL.ASI_FALSE);
                    }
                    else
                    {
                        ASICameraDll2.ASICloseCamera(CameraId);

                        CameraConnection();

                        goto STARTPROCESS;
                    }

                    IsExpSuccess = false;
                    ASICameraDll2.ASISetControlValue(CameraId, ASI_CONTROL_TYPE.ASI_EXPOSURE, ExpTime);

                    while (true)
                    {

                        GetExpError = ASICameraDll2.ASIGetExpStatus(CameraId, out ExpStatus);
                        if (ExpStatus == ASI_EXPOSURE_STATUS.ASI_EXP_SUCCESS && GetExpError == ASI_ERROR_CODE.ASI_SUCCESS)
                        {
                            IsExpSuccess = true;
                            break;
                        }
                        else if (ExpStatus == ASI_EXPOSURE_STATUS.ASI_EXP_FAILED && GetExpError != ASI_ERROR_CODE.ASI_SUCCESS)
                            break;
                    }

                    ASICameraDll2.ASIStopExposure(CameraId);

                    if (IsExpSuccess)
                    {

                        ExposureCounter.Stop();

                        if (ImageBuf != IntPtr.Zero)
                        {
                            Marshal.FreeCoTaskMem(ImageBuf);
                            ImageBuf = IntPtr.Zero;
                        }

                        ImageBuf = Marshal.AllocCoTaskMem((int)AppSetting.Data.ImageSize);
                        GetExpError = ASICameraDll2.ASIGetDataAfterExp(CameraId, ImageBuf, (int)AppSetting.Data.ImageSize);
                        if (GetExpError == ASI_ERROR_CODE.ASI_SUCCESS)
                        {

                            RootFrame = new Image<Bgr, byte>((int)AppSetting.Data.ImageWidth, (int)AppSetting.Data.ImageHeight, (int)AppSetting.Data.ImageWidth * 3, ImageBuf);
                            ProcessFrameGray = RootFrame.Convert<Gray, Byte>();
                            ProcessFrame = RootFrame.Copy();
                            HoughCirclesFrame = RootFrame.Convert<Gray, Byte>();
                            ROIFrame = RootFrame.Copy();
                            KeoGramsFrame = RootFrame.Copy();
                            HdrOutput = RootFrame.Copy();
                            Recover = true;
                        }

                    }
                    else
                    {
                        ASICameraDll2.ASICloseCamera(CameraId);
                        CameraConnection();
                    }
                    GC.Collect();

                    if (ColorValue <= 135 && AppSetting.Data.ExposureTime < 1 && SpeedMode.CheckState == 0)
                    {
                        await Task.Delay(50000);

                    }
                    else if (ColorValue <= AppSetting.Data.MaxLight && ColorValue >= AppSetting.Data.MinLight && AppSetting.Data.ExposureTime < 1000 && SpeedMode.CheckState == 0)
                    {
                        await Task.Delay(50000);

                    }
                    else if (ColorValue <= AppSetting.Data.MaxLight && ColorValue >= AppSetting.Data.MinLight && AppSetting.Data.ExposureTime >= 1000 && AppSetting.Data.ExposureTime < 10000 && SpeedMode.CheckState == 0)
                    {
                        await Task.Delay(5000);

                    }
                    else
                    {
                        await Task.Delay(1000);

                    }

                    goto STARTPROCESS;
                }
                else
                {
                    SystemMessage = "No camera connected";
                    SpinWait.SpinUntil(() => false, 5000);
                    ConnectedCameras = ASICameraDll2.ASIGetNumOfConnectedCameras();

                    if (ConnectedCameras > 0)
                    {
                        SystemMessage = "Retring to connect to the camera.";
                        InitializeCamera();
                    }
                    else
                    {
                        RootFrame = new Image<Bgr, byte>(AppDomain.CurrentDomain.BaseDirectory + "TAllSyk.jpg");
                        ProcessFrame = RootFrame.Copy();
                        ROIFrame = RootFrame.Copy();

                        GC.Collect();
                    }


                    goto STARTPROCESS;
                }
            });
        }

        //--------------------------------------------------------------------------------------Event Handler--------------------------------------------------------------------------------

        private void MainWindows_Load(object sender, EventArgs e)
        {
            InitializeSystem();
        }

        private void MainImageControl_MouseMove(object sender, MouseEventArgs e)
        {

            int offsetX = (int)(e.Location.X / MainImageControl.ZoomScale);
            int offsetY = (int)(e.Location.Y / MainImageControl.ZoomScale);
            int horizontalScrollBarValue = MainImageControl.HorizontalScrollBar.Visible ? (int)MainImageControl.HorizontalScrollBar.Value : 0;
            int verticalScrollBarValue = MainImageControl.VerticalScrollBar.Visible ? (int)MainImageControl.VerticalScrollBar.Value : 0;

            if (IsDefineROI)
            {
                int EndPointX = (offsetX + horizontalScrollBarValue);
                int EndPointY = (offsetY + horizontalScrollBarValue);
                Console.WriteLine(horizontalScrollBarValue);
                Console.WriteLine(verticalScrollBarValue);
                AppSetting.Data.ROIWidth = Math.Abs(AppSetting.Data.ROIX - EndPointX);
                AppSetting.Data.ROIHeight = Math.Abs(AppSetting.Data.ROIY - EndPointY);

                System.Drawing.Size ROISize = new System.Drawing.Size((int)AppSetting.Data.ROIWidth, (int)AppSetting.Data.ROIHeight);
                Point ROIPoint = new Point(AppSetting.Data.ROIX, AppSetting.Data.ROIY);
                ROIRec = new Rectangle(ROIPoint, ROISize);
            }

            XYPosText.Text = "X = " + Convert.ToString(offsetX + horizontalScrollBarValue) + ", Y = " + Convert.ToString(offsetY + verticalScrollBarValue);
        }

        private void BtnSetCameraSetting_Click(object sender, EventArgs e)
        {
            double MIN_ISO = 0.0, MAX_ISO = 0.0, MIN_SHUTTER = 0.0, MAX_SHUTTER = 0.0, MIN_APERTURE = 0.0;

            if (double.TryParse(MIN_ISOText.Text, out MIN_ISO) && double.TryParse(MAX_ISOText.Text, out MAX_ISO) && double.TryParse(MIN_SHUTTERText.Text, out MIN_SHUTTER) &&
                double.TryParse(MAX_SHUTTERText.Text, out MAX_SHUTTER) && double.TryParse(MIN_APERTUREText.Text, out MIN_APERTURE))
            {
                AppSetting.Data.MIN_ISO = MIN_ISO;
                AppSetting.Data.MAX_ISO = MAX_ISO;
                AppSetting.Data.MIN_SHUTTER = MIN_SHUTTER;
                AppSetting.Data.MAX_SHUTTER = MAX_SHUTTER;
                AppSetting.Data.MIN_APERTURE = MIN_APERTURE;
                AppSetting.Save();
            }
            ASICameraDll2.ASISetControlValue(CameraId, ASI_CONTROL_TYPE.ASI_GAIN, (int)AppSetting.Data.MIN_ISO);
            ASICameraDll2.ASISetControlValue(CameraId, ASI_CONTROL_TYPE.ASI_AUTO_MAX_GAIN, (int)AppSetting.Data.MAX_ISO);
        }

        private void BtnDefineOrigin_Click(object sender, EventArgs e)
        {
            double OriginX = 0.0, OriginY = 0.0, OriginWidth = 0.0, OriginHeight = 0.0;

            if (double.TryParse(OriginXText.Text, out OriginX) && double.TryParse(OriginYText.Text, out OriginY) && double.TryParse(OriginWidthText.Text, out OriginWidth) && double.TryParse(OriginHeightText.Text, out OriginHeight))
            {
                AppSetting.Data.OriginX = OriginX;
                AppSetting.Data.OriginY = OriginY;
                AppSetting.Data.OriginWidth = OriginWidth;
                AppSetting.Data.OriginHeight = OriginHeight;
                AppSetting.Save();

            }


        }

        private void OriginDisplay_CheckedChanged(object sender, EventArgs e)
        {
            AppSetting.Data.IS_DISPLAY_ORGIN = OriginDisplay.Checked;
        }


        private void IsAutoExposureTime_CheckedChanged(object sender, EventArgs e)

        {

            if (AppSetting.Data.MIN_ISO >= 50)
                AppSetting.Data.ExposureTime = 15000;

            if (AppSetting.Data.MIN_ISO >= 100)
                AppSetting.Data.ExposureTime = 8000;

            if (AppSetting.Data.MIN_ISO >= 200)
                AppSetting.Data.ExposureTime = 4000;

            if (AppSetting.Data.MIN_ISO >= 400)
                AppSetting.Data.ExposureTime = 2000;

            if (AppSetting.Data.MIN_ISO >= 800)
                AppSetting.Data.ExposureTime = 1000;

            if (AppSetting.Data.MIN_ISO >= 1600)
                AppSetting.Data.ExposureTime = 500;

            if (AppSetting.Data.MIN_ISO >= 3200)
                AppSetting.Data.ExposureTime = 250;

            if (AppSetting.Data.MIN_ISO >= 6400)
                AppSetting.Data.ExposureTime = 125;

            if (AppSetting.Data.MIN_ISO >= 12800)
                AppSetting.Data.ExposureTime = 66;

            if (AppSetting.Data.MIN_ISO >= 25600)
                AppSetting.Data.ExposureTime = 33;

        }

        private void Savebutton_Click(object sender, EventArgs e)
        {
            var folderBrowserDialog1 = new FolderBrowserDialog();
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                FolderName = folderBrowserDialog1.SelectedPath;
            }

            AppSetting.Data.SaveFileDialog = FolderName;
            AppSetting.Save();

        }
        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }

            return null;
        }


        private void SetCameraId_Click(object sender, EventArgs e)
        {

            for (int i = 0; i < ConnectedCameras; i++)
            {
                if (CameraList.Text == CameraList.SelectedItem.ToString())
                {
                    AppSetting.Data.CameraId = i;
                    AppSetting.Save();
                }
            }
            System.Diagnostics.Debug.WriteLine("CameraID = " + AppSetting.Data.CameraId);
            //Application.Restart();
            InitializeSystem();
        }
        private void ResetZoom_Click(object sender, EventArgs e)
        {
            ROIRec.Width = 0;
            ROIRec.Height = 0;
        }


        private void SavePixelValues_Click(object sender, EventArgs e)
        {
            AppSetting.Data.MaxLight = int.Parse(pixelvalues_max.Text);
            AppSetting.Data.MinLight = int.Parse(pixelvalues_min.Text);
            MaxLightHdr = AppSetting.Data.MaxLight;
            MinLightHdr = AppSetting.Data.MinLight;
        }
        private void SaveProfilePixel_Click(object sender, EventArgs e)
        {
            File.AppendAllText(@"./Pixel_Profile.txt", pixelvalues_max.Text + "\n" + pixelvalues_min.Text + "\n");
            int lineCount = File.ReadLines(@"./Pixel_Profile.txt").Count();
            ProfilePixelValues.Items.Clear();
            for (int i = 0; i < (lineCount / 2); i++)
            {

                ProfilePixelValues.Items.Add("Profile " + i);

            }
        }

        private void ProfilePixelValues_SelectedIndexChanged(object sender, EventArgs e)
        {
            int lineCount = File.ReadLines(@"./Pixel_Profile.txt").Count();
            string[] lines = File.ReadAllLines(@"./Pixel_Profile.txt", Encoding.UTF8);


            for (int i = 0; i < lineCount; i++)
            {
                if (ProfilePixelValues.Text == "Profile 0")
                {


                    MaxLight = int.Parse(lines[0]);


                    MinLight = int.Parse(lines[1]);
                }

                else if (ProfilePixelValues.Text == "Profile " + i && ProfilePixelValues.Text != "Profile 0")
                {

                    MaxLight = int.Parse(lines[i * 2]);


                    MinLight = int.Parse(lines[i * 2 + 1]);
                }


            }

            pixelvalues_min.Text = MinLight.ToString();
            pixelvalues_max.Text = MaxLight.ToString();
            AppSetting.Data.MaxLight = MaxLight;
            AppSetting.Data.MinLight = MinLight;
            MaxLightHdr = AppSetting.Data.MaxLight;
            MinLightHdr = AppSetting.Data.MinLight;
        }

        private void MainImageControl_Click(object sender, EventArgs e)
        {

        }

        private void ROIImage_Click(object sender, EventArgs e)
        {

        }

        private void ClearProfilePixel_Click(object sender, EventArgs e)
        {
            File.WriteAllText(@"./Pixel_Profile.txt", String.Empty);
            HoughCircles_Profile.Items.Clear();
        }

        private void hdrDetection_Click(object sender, EventArgs e)
        {
            AppSetting.Data.HdrDetectionHigh = Int32.Parse(HdrDetectionHigh.Text);
            AppSetting.Data.HdrDetectionLow = Int32.Parse(HdrDetectionLow.Text);
            AppSetting.Data.hdrPixelvalueshigh = Int32.Parse(pixelValuesHighHDR.Text);
            AppSetting.Data.hdrPixelvalueslow = Int32.Parse(pixelValuesLowHDR.Text);


        }

        private void autoHDR_CheckedChanged(object sender, EventArgs e)
        {
            ImageMergeStatus = 0;
            ModeHDR = 0;
        }

        private void autoHDR_CheckedChanged_1(object sender, EventArgs e)
        {
            ImageMergeStatus = 0;
        }

        private void Save_HoughCircles_Click(object sender, EventArgs e)
        {
            HoughCirclesStatus = true;
            int CameraWidth = Int16.Parse(ROITextWidth.Text);
            int CameraHeight = Int16.Parse(ROITextHeight.Text);
            StringBuilder msgBuilder = new StringBuilder("Performance: ");

            //Load the image from file and resize it for display
            Image<Bgr, Byte> img = ROIFrame.Resize(CameraWidth / 10, CameraHeight / 10, Emgu.CV.CvEnum.Inter.Linear, true);

            //Convert the image to grayscale and filter out the noise
            UMat uimage = new UMat();
            CvInvoke.CvtColor(img, uimage, ColorConversion.Bgr2Gray);

            //use image pyr to remove noise
            UMat pyrDown = new UMat();
            CvInvoke.PyrDown(uimage, pyrDown);
            CvInvoke.PyrUp(pyrDown, uimage);

            //Image<Gray, Byte> gray = img.Convert<Gray, Byte>().PyrDown().PyrUp();

            #region circle detection
            Stopwatch watch = Stopwatch.StartNew();

            CannyThreshold = double.Parse(CannyThreshold_Box.Text);
            CircleAccumulatorThreshold = double.Parse(CircleAccumulatorThreshold_Box.Text);

            //double CircleAccumulatorThreshold = 120;

            CircleAccumulatorThreshold_Box.Text = CircleAccumulatorThreshold.ToString();
            //CircleF[] circles = CvInvoke.HoughCircles(uimage, HoughType.Gradient, 2.0, 20.0, CannyThreshold, CircleAccumulatorThreshold, 5);
            CircleF[] circles = CvInvoke.HoughCircles(uimage, HoughType.Gradient, 1.5, 27.0, CannyThreshold, CircleAccumulatorThreshold, 5);
            System.Diagnostics.Debug.WriteLine(circles);
            watch.Stop();
            msgBuilder.Append(String.Format("Hough circles - {0} ms; ", watch.ElapsedMilliseconds));
            #endregion

            #region draw circles
            CircleImage = img;
            foreach (CircleF circle in circles)
            {
                CircleImage.Draw(circle, new Bgr(Color.Red), 2);
                HoughCirclesX = (int)circle.Center.X;
                HoughCirclesY = (int)circle.Center.Y;
                HoughCirclesRadius = (int)circle.Radius;

            }

            HoughCircles.Image = CircleImage;
            #endregion
            //IsAutoExposureTime.Checked = true;
            HoughCirclesStatus = false;
        }

        private void SaveProfile_HoughCircles_Click(object sender, EventArgs e)
        {
            File.AppendAllText(@"./HoughCircles_Profile.txt", CannyThreshold_Box.Text + "\n" + CircleAccumulatorThreshold_Box.Text + "\n");
            int lineCount = File.ReadLines(@"./HoughCircles_Profile.txt").Count();
            HoughCircles_Profile.Items.Clear();
            for (int i = 0; i < (lineCount / 2); i++)
            {

                HoughCircles_Profile.Items.Add("Profile " + i);

            }

        }

        private void HoughCircles_Profile_SelectedIndexChanged(object sender, EventArgs e)
        {
            HoughCirclesStatus = true;
            int lineCount = File.ReadLines(@"./HoughCircles_Profile.txt").Count();
            string[] lines = File.ReadAllLines(@"./HoughCircles_Profile.txt", Encoding.UTF8);


            for (int i = 0; i < lineCount; i++)
            {
                if (HoughCircles_Profile.Text == "Profile 0")
                {

                    CannyThreshold_Box.Text = lines[0];


                    CircleAccumulatorThreshold_Box.Text = lines[1];

                }

                else if (HoughCircles_Profile.Text == "Profile " + i && HoughCircles_Profile.Text != "Profile 0")
                {
                    CannyThreshold_Box.Text = lines[i * 2];


                    CircleAccumulatorThreshold_Box.Text = lines[i * 2 + 1];
                }


            }

            int CameraWidth = Int16.Parse(ROITextWidth.Text);
            int CameraHeight = Int16.Parse(ROITextHeight.Text);

            StringBuilder msgBuilder = new StringBuilder("Performance: ");

            //Load the image from file and resize it for display
            Image<Bgr, Byte> img = ROIFrame.Resize(CameraWidth / 10, CameraHeight / 10, Emgu.CV.CvEnum.Inter.Linear, true);

            //Convert the image to grayscale and filter out the noise
            UMat uimage = new UMat();
            CvInvoke.CvtColor(img, uimage, ColorConversion.Bgr2Gray);

            //use image pyr to remove noise
            UMat pyrDown = new UMat();
            CvInvoke.PyrDown(uimage, pyrDown);
            CvInvoke.PyrUp(pyrDown, uimage);

            //Image<Gray, Byte> gray = img.Convert<Gray, Byte>().PyrDown().PyrUp();

            #region circle detection
            Stopwatch watch = Stopwatch.StartNew();

            CannyThreshold = double.Parse(CannyThreshold_Box.Text);
            CircleAccumulatorThreshold = double.Parse(CircleAccumulatorThreshold_Box.Text);
            CircleAccumulatorThreshold_Box.Text = CircleAccumulatorThreshold.ToString();
            CircleF[] circles = CvInvoke.HoughCircles(uimage, HoughType.Gradient, 1.5, 27.0, CannyThreshold, CircleAccumulatorThreshold, 5);
            System.Diagnostics.Debug.WriteLine(circles);
            watch.Stop();
            msgBuilder.Append(String.Format("Hough circles - {0} ms; ", watch.ElapsedMilliseconds));
            #endregion

            #region draw circles
            CircleImage = img;
            foreach (CircleF circle in circles)
            {
                CircleImage.Draw(circle, new Bgr(Color.Red), 2);
                HoughCirclesX = (int)circle.Center.X;
                HoughCirclesY = (int)circle.Center.Y;
                HoughCirclesRadius = (int)circle.Radius;




            }

            HoughCircles.Image = CircleImage;
            #endregion
            HoughCirclesStatus = false;
        }

        private void Clear_Profile_Click(object sender, EventArgs e)
        {
            File.WriteAllText(@"./HoughCircles_Profile.txt", String.Empty);
            HoughCircles_Profile.Items.Clear();
        }


        public void ExposureAdjust()
        {
            if (CameraStateText.Text != "ASI_EXP_WORKING"
                && Recover != false
                && ColorValue >= MinLight
                && AppSetting.Data.ExposureTime < 240000
                && AppSetting.Data.ExposureTime > 1)
            {
                if (AppSetting.Data.ExposureTime == 240000 && CameraStateText.Text != "ASI_EXP_WORKING" && Recover != false)
                {
                    AppSetting.Data.ExposureTime = 120000;
                    Recover = false;
                }
                else if (AppSetting.Data.ExposureTime >= 120000 && AppSetting.Data.ExposureTime < 240000 && Recover != false)
                {
                    AppSetting.Data.ExposureTime = 60000;
                    Recover = false;
                }
                else if (AppSetting.Data.ExposureTime >= 60000 && AppSetting.Data.ExposureTime < 120000 && Recover != false)
                {
                    AppSetting.Data.ExposureTime = 30000;
                    Recover = false;
                }

                else if (AppSetting.Data.ExposureTime >= 30000 && AppSetting.Data.ExposureTime < 60000 && Recover != false)
                {

                    AppSetting.Data.ExposureTime = 15000;

                    Recover = false;
                }

                else if (AppSetting.Data.ExposureTime >= 15000 && AppSetting.Data.ExposureTime < 30000 && Recover != false)
                {
                    AppSetting.Data.ExposureTime = 8000;
                    if (AppSetting.Data.MIN_ISO >= 100)
                    {
                        AppSetting.Data.MIN_ISO = 50;
                        MIN_ISOText.Text = AppSetting.Data.MIN_ISO.ToString();
                        ASICameraDll2.ASISetControlValue(CameraId, ASI_CONTROL_TYPE.ASI_GAIN, (int)AppSetting.Data.MIN_ISO);

                    }
                    Recover = false;
                }

                else if (AppSetting.Data.ExposureTime >= 8000 && AppSetting.Data.ExposureTime < 15000 && Recover != false)
                {
                    AppSetting.Data.ExposureTime = 4000;
                    Recover = false;
                }


                else if (AppSetting.Data.ExposureTime >= 4000 && AppSetting.Data.ExposureTime < 8000 && Recover != false)
                {
                    AppSetting.Data.ExposureTime = 2000;

                    Recover = false;
                }


                else if (AppSetting.Data.ExposureTime >= 2000 && AppSetting.Data.ExposureTime < 4000 && Recover != false)
                {
                    AppSetting.Data.ExposureTime = 1000;
                    if (AppSetting.Data.MIN_ISO >= 50)
                    {
                        AppSetting.Data.MIN_ISO = 0;
                        MIN_ISOText.Text = AppSetting.Data.MIN_ISO.ToString();
                        ASICameraDll2.ASISetControlValue(CameraId, ASI_CONTROL_TYPE.ASI_GAIN, (int)AppSetting.Data.MIN_ISO);

                    }
                    Recover = false;
                }


                else if (AppSetting.Data.ExposureTime >= 1000 && AppSetting.Data.ExposureTime < 2000 && Recover != false)
                {
                    AppSetting.Data.ExposureTime = 500;
                    Recover = false;
                }


                else if (AppSetting.Data.ExposureTime >= 500 && AppSetting.Data.ExposureTime < 1000 && Recover != false)
                {
                    AppSetting.Data.ExposureTime = 250;
                    Recover = false;
                }


                else if (AppSetting.Data.ExposureTime >= 250 && AppSetting.Data.ExposureTime < 500 && Recover != false)
                {
                    AppSetting.Data.ExposureTime = 125;
                    Recover = false;
                }


                else if (AppSetting.Data.ExposureTime >= 125 && AppSetting.Data.ExposureTime < 250 && Recover != false)
                {
                    AppSetting.Data.ExposureTime = 66;
                    Recover = false;
                }


                else if (AppSetting.Data.ExposureTime >= 66 && AppSetting.Data.ExposureTime < 125 && Recover != false)
                {
                    AppSetting.Data.ExposureTime = 33;
                    Recover = false;
                }


                else if (AppSetting.Data.ExposureTime >= 33 && AppSetting.Data.ExposureTime < 66 && Recover != false)
                {
                    AppSetting.Data.ExposureTime = 16;
                    Recover = false;
                }

                else if (AppSetting.Data.ExposureTime >= 16 && AppSetting.Data.ExposureTime < 33 && Recover != false)
                {
                    AppSetting.Data.ExposureTime = 8;
                    Recover = false;
                }
                else if (AppSetting.Data.ExposureTime >= 8 && AppSetting.Data.ExposureTime < 16 && Recover != false)
                {
                    AppSetting.Data.ExposureTime = 4;
                    Recover = false;
                }
                else if (AppSetting.Data.ExposureTime >= 4 && AppSetting.Data.ExposureTime < 8 && Recover != false)
                {
                    AppSetting.Data.ExposureTime = 2;
                    Recover = false;
                }
                else if (AppSetting.Data.ExposureTime >= 2 && AppSetting.Data.ExposureTime < 4 && Recover != false)
                {
                    AppSetting.Data.ExposureTime = 1;
                    Recover = false;
                }
                else if (AppSetting.Data.ExposureTime >= 1 && AppSetting.Data.ExposureTime < 2 && Recover != false)
                {
                    AppSetting.Data.ExposureTime = 0.5;
                    Recover = false;
                }

            }

            else if (CameraStateText.Text != "ASI_EXP_WORKING"
                && Recover != false
                && ColorValue <= MinLight
                && AppSetting.Data.ExposureTime > 1
                && AppSetting.Data.ExposureTime < 240000)
            {

                if (AppSetting.Data.ExposureTime == 120000 && CameraStateText.Text != "ASI_EXP_WORKING" && Recover != false)
                {
                    AppSetting.Data.ExposureTime = 240000;
                    Recover = false;
                }
                else if (AppSetting.Data.ExposureTime <= 60000 && AppSetting.Data.ExposureTime > 30000 && Recover != false)
                {
                    AppSetting.Data.ExposureTime = 120000;
                    Recover = false;
                }
                else if (AppSetting.Data.ExposureTime <= 30000 && AppSetting.Data.ExposureTime > 15000 && Recover != false)
                {
                    AppSetting.Data.ExposureTime = 60000;

                    Recover = false;
                }
                else if (AppSetting.Data.ExposureTime <= 15000 && AppSetting.Data.ExposureTime > 8000 && Recover != false)
                {
                    AppSetting.Data.ExposureTime = 30000;
                    if (ColorValue <= 30)
                    {
                        AppSetting.Data.ExposureTime = 120000;
                    }
                    Recover = false;
                }
                else if (AppSetting.Data.ExposureTime <= 8000 && AppSetting.Data.ExposureTime > 4000 && Recover != false)
                {
                    AppSetting.Data.ExposureTime = 15000;
                    if (AppSetting.Data.MIN_ISO < 100)
                    {
                        AppSetting.Data.MIN_ISO = 100;
                        MIN_ISOText.Text = AppSetting.Data.MIN_ISO.ToString();
                        ASICameraDll2.ASISetControlValue(CameraId, ASI_CONTROL_TYPE.ASI_GAIN, (int)AppSetting.Data.MIN_ISO);

                    }

                    Recover = false;
                }

                else if (AppSetting.Data.ExposureTime <= 4000 && AppSetting.Data.ExposureTime > 2000 && Recover != false)
                {
                    AppSetting.Data.ExposureTime = 8000;
                    Recover = false;
                }


                else if (AppSetting.Data.ExposureTime <= 2000 && AppSetting.Data.ExposureTime > 1000 && Recover != false)
                {

                    AppSetting.Data.ExposureTime = 4000;
                    Recover = false;
                }


                else if (AppSetting.Data.ExposureTime <= 1000 && AppSetting.Data.ExposureTime > 500 && Recover != false)
                {
                    AppSetting.Data.ExposureTime = 2000;
                    if (AppSetting.Data.MIN_ISO < 50)
                    {
                        AppSetting.Data.MIN_ISO = 50;
                        MIN_ISOText.Text = AppSetting.Data.MIN_ISO.ToString();
                        ASICameraDll2.ASISetControlValue(CameraId, ASI_CONTROL_TYPE.ASI_GAIN, (int)AppSetting.Data.MIN_ISO);

                    }

                    Recover = false;
                }


                else if (AppSetting.Data.ExposureTime <= 500 && AppSetting.Data.ExposureTime > 250 && Recover != false)
                {
                    AppSetting.Data.ExposureTime = 1000;
                    Recover = false;
                }


                else if (AppSetting.Data.ExposureTime <= 250 && AppSetting.Data.ExposureTime > 125 && Recover != false)
                {
                    AppSetting.Data.ExposureTime = 500;
                    Recover = false;
                }


                else if (AppSetting.Data.ExposureTime <= 125 && AppSetting.Data.ExposureTime > 66 && Recover != false)
                {
                    AppSetting.Data.ExposureTime = 250;
                    Recover = false;
                }


                else if (AppSetting.Data.ExposureTime <= 66 && AppSetting.Data.ExposureTime > 33 && Recover != false)
                {
                    AppSetting.Data.ExposureTime = 125;
                    Recover = false;
                }


                else if (AppSetting.Data.ExposureTime <= 33 && AppSetting.Data.ExposureTime > 16 && Recover != false)
                {
                    AppSetting.Data.ExposureTime = 66;
                    Recover = false;
                }


                else if (AppSetting.Data.ExposureTime <= 16 && AppSetting.Data.ExposureTime > 8 && Recover != false)
                {
                    AppSetting.Data.ExposureTime = 33;
                    Recover = false;
                }

                else if (AppSetting.Data.ExposureTime <= 8 && AppSetting.Data.ExposureTime > 4 && Recover != false)
                {
                    AppSetting.Data.ExposureTime = 16;
                    Recover = false;
                }
                else if (AppSetting.Data.ExposureTime <= 4 && AppSetting.Data.ExposureTime > 2 && Recover != false)
                {
                    AppSetting.Data.ExposureTime = 8;
                    Recover = false;
                }
                else if (AppSetting.Data.ExposureTime <= 2 && AppSetting.Data.ExposureTime > 1 && Recover != false)
                {
                    AppSetting.Data.ExposureTime = 4;
                    Recover = false;
                }
                else if (AppSetting.Data.ExposureTime <= 1 && AppSetting.Data.ExposureTime > 0.5 && Recover != false)
                {
                    AppSetting.Data.ExposureTime = 2;
                    Recover = false;
                }
                else if (AppSetting.Data.ExposureTime <= 0.5 && Recover != false)
                {
                    AppSetting.Data.ExposureTime = 1;
                    Recover = false;
                }
            }
        }

        public void Focus_Point()
        {
            Bitmap BmpImageFrame = ImageFrame.ToBitmap();
            int CameraWidth;
            int CameraHeight;
            int Thickness = 5;
            //When open houghCircles Change focus
            if (HoughCirclesX > 0 && HoughCirclesY > 0)
            {
                CameraWidth = (HoughCirclesX * 10) * 2;
                CameraHeight = (HoughCirclesY * 10) * 2;
            }
            else
            {
                CameraWidth = Int16.Parse(ROITextWidth.Text);
                CameraHeight = Int16.Parse(ROITextHeight.Text);
            }

            if (FocusPoint.Text == "21 Focus Points" && HoughCirclesStatus == false)
            {
                checkBoxCenter.Checked = false;
                checkBoxAverage.Checked = true;
                //=======================================Center Line======================================== 
                CentroidX = (int)(CameraWidth / 2);
                CentroidY = (int)(CameraHeight / 2);
                //left 
                double CentroidXleft = (int)(CameraWidth / 2.5);
                double CentroidYleft = (int)(CameraHeight / 2);
                //left 
                double CentroidXleftmore = (int)(CameraWidth / 3.2);
                double CentroidYleftmore = (int)(CameraHeight / 2);
                //right
                double CentroidXright = (int)(CameraWidth / 1.7);
                double CentroidYright = (int)(CameraHeight / 2);
                //right
                double CentroidXrightmore = (int)(CameraWidth / 1.5);
                double CentroidYrightmore = (int)(CameraHeight / 2);
                //=========================================UP Line======================================
                double CentroidX_UP = (int)(CameraWidth / 2);
                double CentroidY_UP = (int)(CameraHeight / 3);
                //left 
                double CentroidXleft_UP = (int)(CameraWidth / 2.5);
                double CentroidYleft_UP = (int)(CameraHeight / 3);
                //left 
                double CentroidXleftmore_UP = (int)(CameraWidth / 3.2);
                double CentroidYleftmore_UP = (int)(CameraHeight / 3);
                //right
                double CentroidXright_UP = (int)(CameraWidth / 1.7);
                double CentroidYright_UP = (int)(CameraHeight / 3);
                //right
                double CentroidXrightmore_UP = (int)(CameraWidth / 1.5);
                double CentroidYrightmore_UP = (int)(CameraHeight / 3);
                //=======================================DOWN Line========================================
                double CentroidX_DOWN = (int)(CameraWidth / 2);
                double CentroidY_DOWN = (int)(CameraHeight / 1.5);
                //left 
                double CentroidXleft_DOWN = (int)(CameraWidth / 2.5);
                double CentroidYleft_DOWN = (int)(CameraHeight / 1.5);
                //left 
                double CentroidXleftmore_DOWN = (int)(CameraWidth / 3.2);
                double CentroidYleftmore_DOWN = (int)(CameraHeight / 1.5);
                //right
                double CentroidXright_DOWN = (int)(CameraWidth / 1.7);
                double CentroidYright_DOWN = (int)(CameraHeight / 1.5);
                //right
                double CentroidXrightmore_DOWN = (int)(CameraWidth / 1.5);
                double CentroidYrightmore_DOWN = (int)(CameraHeight / 1.5);
                //=========================================UP 3 DOT======================================
                double CentroidX_UP2 = (int)(CameraWidth / 2);
                double CentroidY_UP2 = (int)(CameraHeight / 6);
                //right
                double CentroidXright_UP2 = (int)(CameraWidth / 1.7);
                double CentroidYright_UP2 = (int)(CameraHeight / 6);
                //left 
                double CentroidXleft_UP2 = (int)(CameraWidth / 2.5);
                double CentroidYleft_UP2 = (int)(CameraHeight / 6);
                //=======================================DOWN 3 DOT========================================
                double CentroidX_DOWN2 = (int)(CameraWidth / 2);
                double CentroidY_DOWN2 = (int)(CameraHeight / 1.2);
                //right
                double CentroidXright_DOWN2 = (int)(CameraWidth / 1.7);
                double CentroidYright_DOWN2 = (int)(CameraHeight / 1.2);
                //left 
                double CentroidXleft_DOWN2 = (int)(CameraWidth / 2.5);
                double CentroidYleft_DOWN2 = (int)(CameraHeight / 1.2);

                if (ShowFocusPoint.CheckState != 0)
                {
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidX.ToString()), Int32.Parse(CentroidY.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                    //left 
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleft.ToString()), Int32.Parse(CentroidYleft.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                    //left 
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleftmore.ToString()), Int32.Parse(CentroidYleftmore.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                    //right
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXright.ToString()), Int32.Parse(CentroidYright.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                    //right
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXrightmore.ToString()), Int32.Parse(CentroidYrightmore.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                    //=========================================UP======================================
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidX_UP.ToString()), Int32.Parse(CentroidY_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                    //left 
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleft_UP.ToString()), Int32.Parse(CentroidYleft_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                    //left 
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleftmore_UP.ToString()), Int32.Parse(CentroidYleftmore_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                    //right
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXright_UP.ToString()), Int32.Parse(CentroidYright_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                    //right
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXrightmore_UP.ToString()), Int32.Parse(CentroidYrightmore_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                    //=======================================DOWN========================================
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidX_DOWN.ToString()), Int32.Parse(CentroidY_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                    //left 
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleft_DOWN.ToString()), Int32.Parse(CentroidYleft_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                    //left 
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleftmore_DOWN.ToString()), Int32.Parse(CentroidYleftmore_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                    //right
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXright_DOWN.ToString()), Int32.Parse(CentroidYright_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                    //right
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXrightmore_DOWN.ToString()), Int32.Parse(CentroidYrightmore_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                    //=========================================UP 3 DOT======================================
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidX_UP2.ToString()), Int32.Parse(CentroidY_UP2.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                    //right
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXright_UP2.ToString()), Int32.Parse(CentroidYright_UP2.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleft_UP2.ToString()), Int32.Parse(CentroidYleft_UP2.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                    //=======================================DOWN 3 DOT========================================
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidX_DOWN2.ToString()), Int32.Parse(CentroidY_DOWN2.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleft_DOWN2.ToString()), Int32.Parse(CentroidYleft_DOWN2.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                    //right
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXright_DOWN2.ToString()), Int32.Parse(CentroidYright_DOWN2.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                }

                //==================Readlight Center
                for (double i = CentroidX - 10; i < CentroidX; i++)
                {
                    for (double j = CentroidY - 10; j < CentroidY; j++)
                    {
                        if (CentroidX > CentroidY)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }
                    }
                }
                for (double i = CentroidXleftmore - 10; i < CentroidXleftmore; i++)
                {
                    for (double j = CentroidYleftmore - 10; j < CentroidYleftmore; j++)
                    {
                        if (CentroidXleftmore > CentroidYleftmore)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }
                    }
                }
                for (double i = CentroidXleft - 10; i < CentroidXleft; i++)
                {
                    for (double j = CentroidYleft - 10; j < CentroidYleft; j++)
                    {
                        if (CentroidXleft > CentroidYleft)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));

                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }
                    }
                }
                for (double i = CentroidXright - 10; i < CentroidXright; i++)
                {
                    for (double j = CentroidYright - 10; j < CentroidYright; j++)
                    {
                        if (CentroidXright > CentroidYright)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }
                    }
                }
                for (double i = CentroidXrightmore - 10; i < CentroidXrightmore; i++)
                {
                    for (double j = CentroidYrightmore - 10; j < CentroidYrightmore; j++)
                    {
                        if (CentroidXrightmore > CentroidYrightmore)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }
                    }
                }
                //==================Readlight UP
                for (double i = CentroidX_UP - 10; i < CentroidX_UP; i++)
                {
                    for (double j = CentroidY_UP - 10; j < CentroidY_UP; j++)
                    {
                        if (CentroidX_UP > CentroidY_UP)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }
                    }
                }
                for (double i = CentroidXleft_UP - 10; i < CentroidXleft_UP; i++)
                {
                    for (double j = CentroidYleft_UP - 10; j < CentroidYleft_UP; j++)
                    {
                        if (CentroidXleft_UP > CentroidYleft_UP)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }
                    }
                }
                for (double i = CentroidXleftmore_UP - 10; i < CentroidXleftmore_UP; i++)
                {
                    for (double j = CentroidYleftmore_UP - 10; j < CentroidYleftmore_UP; j++)
                    {
                        if (CentroidXleftmore_UP > CentroidYleftmore_UP)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }
                    }
                }
                for (double i = CentroidXright_UP - 10; i < CentroidXright_UP; i++)
                {
                    for (double j = CentroidYright_UP - 10; j < CentroidYright_UP; j++)

                    {
                        if (CentroidXright_UP > CentroidYright_UP)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }

                    }

                }
                for (double i = CentroidXrightmore_UP - 10; i < CentroidXrightmore_UP; i++)
                {
                    for (double j = CentroidYrightmore_UP - 10; j < CentroidYrightmore_UP; j++)
                    {
                        if (CentroidXrightmore_UP > CentroidYrightmore_UP)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }
                    }
                }
                //==================Readlight Down
                for (double i = CentroidX_DOWN - 10; i < CentroidX_DOWN; i++)
                {
                    for (double j = CentroidY_DOWN - 10; j < CentroidY_DOWN; j++)
                    {
                        if (CentroidX_DOWN > CentroidY_DOWN)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }
                    }
                }
                for (double i = CentroidXleft_DOWN - 10; i < CentroidXleft_DOWN; i++)
                {
                    for (double j = CentroidYleft_DOWN - 10; j < CentroidYleft_DOWN; j++)
                    {
                        if (CentroidXleft_DOWN > CentroidYleft_DOWN)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }
                    }
                }
                for (double i = CentroidXleftmore_DOWN - 10; i < CentroidXleftmore_DOWN; i++)
                {
                    for (double j = CentroidYleftmore_DOWN - 10; j < CentroidYleftmore_DOWN; j++)
                    {
                        if (CentroidXleftmore_DOWN > CentroidYleftmore_DOWN)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }
                    }
                }
                for (double i = CentroidXright_DOWN - 10; i < CentroidXright_DOWN; i++)
                {
                    for (double j = CentroidYright_DOWN - 10; j < CentroidYright_DOWN; j++)
                    {
                        if (CentroidXright_DOWN > CentroidYright_DOWN)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }
                    }
                }
                for (double i = CentroidXrightmore_DOWN - 10; i < CentroidXrightmore_DOWN; i++)
                {
                    for (double j = CentroidYrightmore_DOWN - 10; j < CentroidYrightmore_DOWN; j++)
                    {
                        if (CentroidXrightmore_DOWN > CentroidYrightmore_DOWN)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }
                    }
                }
                //==================Readlight UP DOT
                for (double i = CentroidX_UP2 - 10; i < CentroidX_UP2; i++)
                {
                    for (double j = CentroidY_UP2 - 10; j < CentroidY_UP2; j++)
                    {
                        if (CentroidX_UP2 > CentroidY_UP2)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }
                    }
                }
                for (double i = CentroidXright_UP2 - 10; i < CentroidXright_UP2; i++)
                {
                    for (double j = CentroidYright_UP2 - 10; j < CentroidYright_UP2; j++)
                    {
                        if (CentroidXright_UP2 > CentroidYright_UP2)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }
                    }
                }
                for (double i = CentroidXleft_UP2 - 10; i < CentroidXleft_UP2; i++)
                {
                    for (double j = CentroidYleft_UP2 - 10; j < CentroidYleft_UP2; j++)
                    {
                        if (CentroidXleft_UP2 > CentroidYleft_UP2)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }
                    }
                }
                //==================Readlight DOWN DOT
                for (double i = CentroidX_DOWN2 - 10; i < CentroidX_DOWN2; i++)
                {
                    for (double j = CentroidY_DOWN2 - 10; j < CentroidY_DOWN2; j++)
                    {
                        if (CentroidX_DOWN2 > CentroidY_DOWN2)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }
                    }
                }
                for (double i = CentroidXright_DOWN2 - 10; i < CentroidXright_DOWN2; i++)
                {
                    for (double j = CentroidYright_DOWN2 - 10; j < CentroidYright_DOWN2; j++)
                    {
                        if (CentroidXright_DOWN2 > CentroidYright_DOWN2)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }
                    }
                }
                for (double i = CentroidXleft_DOWN2 - 10; i < CentroidXleft_DOWN2; i++)
                {
                    for (double j = CentroidYleft_DOWN2 - 10; j < CentroidYleft_DOWN2; j++)
                    {
                        if (CentroidXleft_DOWN2 > CentroidYleft_DOWN2)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }
                    }
                }
                ColorValue = (ColorValue / 2100);

            }
            else if (FocusPoint.Text == "15 Focus Points")
            {
                checkBoxCenter.Checked = false;
                checkBoxAverage.Checked = true;
                //=======================================Center======================================== 
                CentroidX = (int)(CameraWidth / 2);
                CentroidY = (int)(CameraHeight / 2);
                //left 
                double CentroidXleft = (int)(CameraWidth / 2.5);
                double CentroidYleft = (int)(CameraHeight / 2);
                //left 
                double CentroidXleftmore = (int)(CameraWidth / 3.2);
                double CentroidYleftmore = (int)(CameraHeight / 2);
                //right
                double CentroidXright = (int)(CameraWidth / 1.7);
                double CentroidYright = (int)(CameraHeight / 2);
                //right
                double CentroidXrightmore = (int)(CameraWidth / 1.5);
                double CentroidYrightmore = (int)(CameraHeight / 2);
                //=========================================UP======================================
                double CentroidX_UP = (int)(CameraWidth / 2);
                double CentroidY_UP = (int)(CameraHeight / 3);
                //left 
                double CentroidXleft_UP = (int)(CameraWidth / 2.5);
                double CentroidYleft_UP = (int)(CameraHeight / 3);
                //left 
                double CentroidXleftmore_UP = (int)(CameraWidth / 3.2);
                double CentroidYleftmore_UP = (int)(CameraHeight / 3);
                //right
                double CentroidXright_UP = (int)(CameraWidth / 1.7);
                double CentroidYright_UP = (int)(CameraHeight / 3);
                //right
                double CentroidXrightmore_UP = (int)(CameraWidth / 1.5);
                double CentroidYrightmore_UP = (int)(CameraHeight / 3);
                //=======================================DOWN========================================
                double CentroidX_DOWN = (int)(CameraWidth / 2);
                double CentroidY_DOWN = (int)(CameraHeight / 1.5);
                //left 
                double CentroidXleft_DOWN = (int)(CameraWidth / 2.5);
                double CentroidYleft_DOWN = (int)(CameraHeight / 1.5);
                //left 
                double CentroidXleftmore_DOWN = (int)(CameraWidth / 3.2);
                double CentroidYleftmore_DOWN = (int)(CameraHeight / 1.5);
                //right
                double CentroidXright_DOWN = (int)(CameraWidth / 1.7);
                double CentroidYright_DOWN = (int)(CameraHeight / 1.5);
                //right
                double CentroidXrightmore_DOWN = (int)(CameraWidth / 1.5);
                double CentroidYrightmore_DOWN = (int)(CameraHeight / 1.5);

                if (ShowFocusPoint.CheckState != 0)
                {
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidX.ToString()), Int32.Parse(CentroidY.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                    //left 
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleft.ToString()), Int32.Parse(CentroidYleft.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                    //left 
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleftmore.ToString()), Int32.Parse(CentroidYleftmore.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                    //right
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXright.ToString()), Int32.Parse(CentroidYright.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                    //right
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXrightmore.ToString()), Int32.Parse(CentroidYrightmore.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                    //=========================================UP======================================
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidX_UP.ToString()), Int32.Parse(CentroidY_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                    //left
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleft_UP.ToString()), Int32.Parse(CentroidYleft_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                    //left 
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleftmore_UP.ToString()), Int32.Parse(CentroidYleftmore_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                    //right
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXright_UP.ToString()), Int32.Parse(CentroidYright_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                    //right
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXrightmore_UP.ToString()), Int32.Parse(CentroidYrightmore_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                    //=======================================DOWN========================================
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidX_DOWN.ToString()), Int32.Parse(CentroidY_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                    //left 
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleft_DOWN.ToString()), Int32.Parse(CentroidYleft_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                    //left 
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleftmore_DOWN.ToString()), Int32.Parse(CentroidYleftmore_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                    //right
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXright_DOWN.ToString()), Int32.Parse(CentroidYright_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                    //right
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXrightmore_DOWN.ToString()), Int32.Parse(CentroidYrightmore_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                }
                //==================Readlight Center
                for (double i = CentroidX - 10; i < CentroidX; i++)
                {
                    for (double j = CentroidY - 10; j < CentroidY; j++)
                    {
                        if (CentroidX > CentroidY)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }
                    }
                }
                for (double i = CentroidXleftmore - 10; i < CentroidXleftmore; i++)
                {
                    for (double j = CentroidYleftmore - 10; j < CentroidYleftmore; j++)
                    {
                        if (CentroidXleftmore > CentroidYleftmore)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;

                        }
                    }
                }
                for (double i = CentroidXleft - 10; i < CentroidXleft; i++)
                {
                    for (double j = CentroidYleft - 10; j < CentroidYleft; j++)
                    {
                        if (CentroidXleft > CentroidYleft)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }
                    }
                }
                for (double i = CentroidXright - 10; i < CentroidXright; i++)
                {
                    for (double j = CentroidYright - 10; j < CentroidYright; j++)
                    {
                        if (CentroidXright > CentroidYright)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }
                    }
                }
                for (double i = CentroidXrightmore - 10; i < CentroidXrightmore; i++)
                {
                    for (double j = CentroidYrightmore - 10; j < CentroidYrightmore; j++)
                    {
                        if (CentroidXrightmore > CentroidYrightmore)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }
                    }
                }
                //==================Readlight UP
                for (double i = CentroidX_UP - 10; i < CentroidX_UP; i++)
                {
                    for (double j = CentroidY_UP - 10; j < CentroidY_UP; j++)
                    {
                        if (CentroidX_UP > CentroidY_UP)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }
                    }
                }
                for (double i = CentroidXleft_UP - 10; i < CentroidXleft_UP; i++)
                {
                    for (double j = CentroidYleft_UP - 10; j < CentroidYleft_UP; j++)
                    {
                        if (CentroidXleft_UP > CentroidYleft_UP)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }
                    }
                }
                for (double i = CentroidXleftmore_UP - 10; i < CentroidXleftmore_UP; i++)
                {
                    for (double j = CentroidYleftmore_UP - 10; j < CentroidYleftmore_UP; j++)
                    {
                        if (CentroidXleftmore_UP > CentroidYleftmore_UP)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }
                    }
                }
                for (double i = CentroidXright_UP - 10; i < CentroidXright_UP; i++)
                {
                    for (double j = CentroidYright_UP - 10; j < CentroidYright_UP; j++)
                    {
                        if (CentroidXright_UP > CentroidYright_UP)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }
                    }
                }
                for (double i = CentroidXrightmore_UP - 10; i < CentroidXrightmore_UP; i++)
                {
                    for (double j = CentroidYrightmore_UP - 10; j < CentroidYrightmore_UP; j++)
                    {
                        if (CentroidXrightmore_UP > CentroidYrightmore_UP)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }
                    }
                }
                //==================Readlight Down
                for (double i = CentroidX_DOWN - 10; i < CentroidX_DOWN; i++)
                {
                    for (double j = CentroidY_DOWN - 10; j < CentroidY_DOWN; j++)
                    {
                        if (CentroidX_DOWN > CentroidY_DOWN)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }
                    }
                }
                for (double i = CentroidXleft_DOWN - 10; i < CentroidXleft_DOWN; i++)
                {
                    for (double j = CentroidYleft_DOWN - 10; j < CentroidYleft_DOWN; j++)
                    {
                        if (CentroidXleft_DOWN > CentroidYleft_DOWN)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }
                    }
                }
                for (double i = CentroidXleftmore_DOWN - 10; i < CentroidXleftmore_DOWN; i++)
                {
                    for (double j = CentroidYleftmore_DOWN - 10; j < CentroidYleftmore_DOWN; j++)
                    {
                        if (CentroidXleftmore_DOWN > CentroidYleftmore_DOWN)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }
                    }
                }
                for (double i = CentroidXright_DOWN - 10; i < CentroidXright_DOWN; i++)
                {
                    for (double j = CentroidYright_DOWN - 10; j < CentroidYright_DOWN; j++)
                    {
                        if (CentroidXright_DOWN > CentroidYright_DOWN)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }
                    }
                }
                for (double i = CentroidXrightmore_DOWN - 10; i < CentroidXrightmore_DOWN; i++)
                {
                    for (double j = CentroidYrightmore_DOWN - 10; j < CentroidYrightmore_DOWN; j++)
                    {
                        if (CentroidXrightmore_DOWN > CentroidYrightmore_DOWN)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }
                    }
                }
                ColorValue = (ColorValue / 1500);
            }
            else if (FocusPoint.Text == "9 Focus Points")
            {
                checkBoxCenter.Checked = false;
                checkBoxAverage.Checked = true;
                //=======================================Center======================================== 
                CentroidX = (int)(CameraWidth / 2);
                CentroidY = (int)(CameraHeight / 2);
                //left 
                double CentroidXleftmore = (int)(CameraWidth / 3.2);
                double CentroidYleftmore = (int)(CameraHeight / 2);
                //right
                double CentroidXrightmore = (int)(CameraWidth / 1.5);
                double CentroidYrightmore = (int)(CameraHeight / 2);
                //=========================================UP======================================
                double CentroidX_UP = (int)(CameraWidth / 2);
                double CentroidY_UP = (int)(CameraHeight / 3);
                //left 
                double CentroidXleftmore_UP = (int)(CameraWidth / 3.2);
                double CentroidYleftmore_UP = (int)(CameraHeight / 3);
                //right
                double CentroidXrightmore_UP = (int)(CameraWidth / 1.5);
                double CentroidYrightmore_UP = (int)(CameraHeight / 3);
                //=======================================DOWN========================================
                double CentroidX_DOWN = (int)(CameraWidth / 2);
                double CentroidY_DOWN = (int)(CameraHeight / 1.5);
                //left 
                double CentroidXleftmore_DOWN = (int)(CameraWidth / 3.2);
                double CentroidYleftmore_DOWN = (int)(CameraHeight / 1.5);
                //right
                double CentroidXrightmore_DOWN = (int)(CameraWidth / 1.5);
                double CentroidYrightmore_DOWN = (int)(CameraHeight / 1.5);
                if (ShowFocusPoint.CheckState != 0)
                {
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidX.ToString()), Int32.Parse(CentroidY.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                    //left 
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleftmore.ToString()), Int32.Parse(CentroidYleftmore.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                    //right
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXrightmore.ToString()), Int32.Parse(CentroidYrightmore.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                    //=========================================UP======================================
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidX_UP.ToString()), Int32.Parse(CentroidY_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                    //left 
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleftmore_UP.ToString()), Int32.Parse(CentroidYleftmore_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                    //right
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXrightmore_UP.ToString()), Int32.Parse(CentroidYrightmore_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                    //=======================================DOWN========================================
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidX_DOWN.ToString()), Int32.Parse(CentroidY_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                    //left 
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleftmore_DOWN.ToString()), Int32.Parse(CentroidYleftmore_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                    //right
                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXrightmore_DOWN.ToString()), Int32.Parse(CentroidYrightmore_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                }

                //==================Readlight Center
                for (double i = CentroidX - 10; i < CentroidX; i++)
                {
                    for (double j = CentroidY - 10; j < CentroidY; j++)
                    {
                        if (CentroidX > CentroidY)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }
                    }
                }
                for (double i = CentroidXleftmore - 10; i < CentroidXleftmore; i++)
                {
                    for (double j = CentroidYleftmore - 10; j < CentroidYleftmore; j++)
                    {
                        if (CentroidXleftmore > CentroidYleftmore)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }
                    }
                }
                for (double i = CentroidXrightmore - 10; i < CentroidXrightmore; i++)
                {
                    for (double j = CentroidYrightmore - 10; j < CentroidYrightmore; j++)
                    {
                        if (CentroidXrightmore > CentroidYrightmore)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }
                    }
                }
                //==================Readlight UP
                for (double i = CentroidX_UP - 10; i < CentroidX_UP; i++)
                {
                    for (double j = CentroidY_UP - 10; j < CentroidY_UP; j++)
                    {
                        if (CentroidX_UP > CentroidY_UP)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }
                    }
                }
                for (double i = CentroidXleftmore_UP - 10; i < CentroidXleftmore_UP; i++)
                {
                    for (double j = CentroidYleftmore_UP - 10; j < CentroidYleftmore_UP; j++)
                    {
                        if (CentroidXleftmore_UP > CentroidYleftmore_UP)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }
                    }
                }
                for (double i = CentroidXrightmore_UP - 10; i < CentroidXrightmore_UP; i++)
                {
                    for (double j = CentroidYrightmore_UP - 10; j < CentroidYrightmore_UP; j++)
                    {
                        if (CentroidXrightmore_UP > CentroidYrightmore_UP)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }
                    }
                }
                //==================Readlight Down
                for (double i = CentroidX_DOWN - 10; i < CentroidX_DOWN; i++)
                {
                    for (double j = CentroidY_DOWN - 10; j < CentroidY_DOWN; j++)
                    {
                        if (CentroidX_DOWN > CentroidY_DOWN)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }
                    }
                }
                for (double i = CentroidXleftmore_DOWN - 10; i < CentroidXleftmore_DOWN; i++)
                {
                    for (double j = CentroidYleftmore_DOWN - 10; j < CentroidYleftmore_DOWN; j++)
                    {
                        if (CentroidXleftmore_DOWN > CentroidYleftmore_DOWN)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }
                    }
                }
                for (double i = CentroidXrightmore_DOWN - 10; i < CentroidXrightmore_DOWN; i++)
                {
                    for (double j = CentroidYrightmore_DOWN - 10; j < CentroidYrightmore_DOWN; j++)
                    {
                        if (CentroidXrightmore_DOWN > CentroidYrightmore_DOWN)
                        {
                            Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                            ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                        }
                    }
                }
                ColorValue = (ColorValue / 900);
            }
            else if (FocusPoint.Text == "Histogram" && Histogramcheck.Checked == false)
            {
                FocusPoint.Text = "21 Focus Points";
                Histogramcheck.Checked = false;
                MessageBox.Show("Please turn on Histogram.", "Message", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

            }

        }

        private void UITimer_Tick(object sender, EventArgs e)
        {
            if (CameraStateText.Text == "ASI_EXP_FAILED")
            {

                if (ConnectedCameras > 0)
                {
                    ConnectedCameras = ASICameraDll2.ASIGetNumOfConnectedCameras();
                }
                if (ConnectedCameras > 0)
                {
                    SystemMessage = "Retring to connect to the camera.";
                    Application.Restart();
                    //return;
                    //InitializeSystem();
                    //InitializeSystem();
                }
                SystemMessage = "No camera connected";
                MainImageControl.Image = new Image<Bgr, byte>(AppDomain.CurrentDomain.BaseDirectory + "TAllSyk.jpg");
                ROIImage.Image = new Image<Bgr, byte>(AppDomain.CurrentDomain.BaseDirectory + "Nocameraconnected.jpg");
                GC.Collect();
            }
            else
            {
                pixelValuesText.Text = ColorValue.ToString();
                ASI_EXPOSURE_STATUS ExpStatus = ASI_EXPOSURE_STATUS.ASI_EXP_IDLE;
                ASI_ERROR_CODE GetExpError = ASICameraDll2.ASIGetExpStatus(CameraId, out ExpStatus);

                if (GetExpError == ASI_ERROR_CODE.ASI_SUCCESS)
                    CameraStateText.Text = ExpStatus.ToString();
                else
                    CameraStateText.Text = GetExpError.ToString();

                ExposuringText.Text = string.Format("{0:0.00}", Math.Log(Math.Pow(2.8, 2) / (AppSetting.Data.ExposureTime / 1000), 2.0));
                SavePath.Text = AppSetting.Data.SaveFileDialog;
                MessageStatusText.Text = SystemMessage;
                GetDataFailedText.Text = GetImageState.ToString();
                CalculateZoomScale();
                if (IsDefineOrigin)
                    BtnDefineOrigin.Text = "Stop Define";
                else
                    BtnDefineOrigin.Text = "Start Define";

                if (ROIRec.Width > 0 && ROIRec.Height > 0)
                {
                    ROIFrame.ROI = ROIRec;
                    ROIImage.Image = ROIFrame.Convert<Gray, Byte>();
                }
                else
                {
                    ROIImage.Image = ProcessFrameGray;
                }

                if (ProcessFrame != null && ConnectedCameras > 0)
                {
                    if (IsDefineROI)
                        CvInvoke.Rectangle(ProcessFrame, ROIRec, new Bgr(Color.White).MCvScalar, 2);

                    if (AppSetting.Data.IS_DISPLAY_ORGIN)
                        CvInvoke.Rectangle(ProcessFrame, new Rectangle((int)AppSetting.Data.OriginX, (int)AppSetting.Data.OriginY, (int)AppSetting.Data.OriginWidth, (int)AppSetting.Data.OriginHeight), new Bgr(Color.LightGreen).MCvScalar, 2);

                    AltAz SunAltAz = SunHanler.GetSunPosition();
                    Point SunXY = AzmAltToXY.CalculateXYPoint(SunAltAz.Alt.Degs, SunAltAz.Az.Degs, 20);
                    //CvInvoke.Circle(ProcessFrame, SunXY, 20, new Bgr(Color.LightGreen).MCvScalar, 2);

                    int CameraWidth;
                    int CameraHeight;
                    //When open houghCircles Change focus
                    if (HoughCirclesX > 0 && HoughCirclesY > 0)
                    {
                        CameraWidth = (HoughCirclesX * 10) * 2;
                        CameraHeight = (HoughCirclesY * 10) * 2;
                    }
                    else
                    {
                        CameraWidth = Int16.Parse(ROITextWidth.Text);
                        CameraHeight = Int16.Parse(ROITextHeight.Text);
                    }



                    if (autoHDR.Checked == false && hdr_On.Checked == false)
                    {
                        MainImageControl.Image = ProcessFrame;
                    }

                    string TimesStamp = DateTime.Now.ToUniversalTime().ToString("MM/dd/yyyy hh:mm:ss tt");
                    int Thickness = 5;
                    int BorderHeight = Int32.Parse(AppSetting.Data.ImageHeight.ToString());
                    int BorderWidth = Int32.Parse(AppSetting.Data.ImageWidth.ToString());
                    float ExposureTimeShow = float.Parse(AppSetting.Data.ExposureTime.ToString());

                    ImageFrame = ProcessFrame;
                    TimeNow = DateTime.Now.ToString("yyyy_MM_dd__HH_mm_ss");
                    TimeFolder = DateTime.Now.ToString("yyyy-MM-dd");
                    TimeNowChack = DateTime.Now.ToString("yyyy_MM_dd__HH_mm");
                    BorderHeight = BorderHeight - 300;
                    var BorderTime = new Rectangle(0, BorderHeight, 800, 100);
                    ImageFrame.Draw(BorderTime, new Bgr(Color.Black), -1);
                    ImageFrame.Draw(BorderTime, new Bgr(Color.White), 2);
                    CvInvoke.PutText(ImageFrame, "UTC " + TimesStamp, new Point(0, BorderHeight + 50), FontFace.HersheySimplex, 1.5, new Bgr(Color.White).MCvScalar, Thickness);
                    BorderWidth = BorderWidth - 600;
                    var BorderExposureTime = new Rectangle(BorderWidth, BorderHeight, 300, 100);
                    ImageFrame.Draw(BorderExposureTime, new Bgr(Color.Black), -1);
                    ImageFrame.Draw(BorderExposureTime, new Bgr(Color.White), 2);
                    if (ExposureTimeShow < 1000 && ExposureTimeShow > 1)
                    {
                        CvInvoke.PutText(ImageFrame, Math.Round(AppSetting.Data.ExposureTime, 0) + " ms", new Point(BorderWidth + 50, BorderHeight + 50), FontFace.HersheySimplex, 1.5, new Bgr(Color.White).MCvScalar, Thickness);
                    }
                    else if (ExposureTimeShow >= 1000)
                    {
                        CvInvoke.PutText(ImageFrame, Math.Round(AppSetting.Data.ExposureTime / 1000, 0) + " sec", new Point(BorderWidth + 50, BorderHeight + 50), FontFace.HersheySimplex, 1.5, new Bgr(Color.White).MCvScalar, Thickness);
                    }
                    else if (ExposureTimeShow >= 0 && ExposureTimeShow <= 1)
                    {
                        CvInvoke.PutText(ImageFrame, Math.Round(AppSetting.Data.ExposureTime, 2) + " ms", new Point(BorderWidth + 50, BorderHeight + 50), FontFace.HersheySimplex, 1.5, new Bgr(Color.White).MCvScalar, Thickness);
                    }

                    Bitmap BmpImageFrame = ImageFrame.ToBitmap();

                    if (IsAutoExposureTime.CheckState != 0) //On-Off AutoExposureTime 
                    {
                        if (AutoISO.CheckState != 0)
                        {
                            if (AppSetting.Data.ExposureTime <= 1000)
                            {
                                if (AppSetting.Data.MIN_ISO > 0)
                                {
                                    AppSetting.Data.MIN_ISO = 0;
                                    MIN_ISOText.Text = AppSetting.Data.MIN_ISO.ToString();
                                    ASICameraDll2.ASISetControlValue(CameraId, ASI_CONTROL_TYPE.ASI_GAIN, (int)AppSetting.Data.MIN_ISO);

                                }
                            }
                            else if (AppSetting.Data.ExposureTime >= 10000 && AppSetting.Data.ExposureTime < 100000)
                            {
                                if (AppSetting.Data.MIN_ISO < 50)
                                {
                                    AppSetting.Data.MIN_ISO = 50;
                                    MIN_ISOText.Text = AppSetting.Data.MIN_ISO.ToString();
                                    ASICameraDll2.ASISetControlValue(CameraId, ASI_CONTROL_TYPE.ASI_GAIN, (int)AppSetting.Data.MIN_ISO);

                                }

                            }
                            else if (AppSetting.Data.ExposureTime >= 100000)
                            {
                                if (AppSetting.Data.MIN_ISO < 100)
                                {
                                    AppSetting.Data.MIN_ISO = 100;
                                    MIN_ISOText.Text = AppSetting.Data.MIN_ISO.ToString();
                                    ASICameraDll2.ASISetControlValue(CameraId, ASI_CONTROL_TYPE.ASI_GAIN, (int)AppSetting.Data.MIN_ISO);

                                }
                                else
                                {
                                    if (AppSetting.Data.MIN_ISO <= 100 && ColorValue < MinLight)
                                    {
                                        AppSetting.Data.MIN_ISO = 200;
                                        MIN_ISOText.Text = AppSetting.Data.MIN_ISO.ToString();
                                        ASICameraDll2.ASISetControlValue(CameraId, ASI_CONTROL_TYPE.ASI_GAIN, (int)AppSetting.Data.MIN_ISO);

                                    }else if (AppSetting.Data.MIN_ISO <= 200 && ColorValue < MinLight)
                                    {
                                        AppSetting.Data.MIN_ISO = 300;
                                        MIN_ISOText.Text = AppSetting.Data.MIN_ISO.ToString();
                                        ASICameraDll2.ASISetControlValue(CameraId, ASI_CONTROL_TYPE.ASI_GAIN, (int)AppSetting.Data.MIN_ISO);

                                    }
                                    else if (AppSetting.Data.MIN_ISO <= 300 && ColorValue < MinLight)
                                    {
                                        AppSetting.Data.MIN_ISO = 400;
                                        MIN_ISOText.Text = AppSetting.Data.MIN_ISO.ToString();
                                        ASICameraDll2.ASISetControlValue(CameraId, ASI_CONTROL_TYPE.ASI_GAIN, (int)AppSetting.Data.MIN_ISO);

                                    }
                                }

                            }

                        }


                        if (checkBoxCenter.CheckState != 0) //checkBoxCenter
                        {
                            checkBoxAverage.Checked = false;
                            CentroidX = (int)(CameraWidth / 2);
                            CentroidY = (int)(CameraHeight / 2);
                            for (double i = CentroidX - 10; i < CentroidX; i++)
                            {
                                for (double j = CentroidY - 10; j < CentroidY; j++)
                                {
                                    Color pixel = BmpImageFrame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                                    ColorValue += (pixel.R + pixel.B + pixel.G) / 3;
                                }
                            }
                            ColorValue = (ColorValue / 100);
                            if (ShowFocusPoint.CheckState != 0)
                            {
                                CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidX.ToString()), Int32.Parse(CentroidY.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, Thickness);
                            }
                        }
                        else
                        {
                            Focus_Point();
                            int HdrDetectionHigh = 0;
                            int HdrDetectionLow = 0;
                            int HdrDetectionmedium = 0;
                            double BestValue;
                            double ControlJump = 1;
                            Console.WriteLine("Pixel Values: " + ColorValue);

                            if (Histogramcheck.CheckState != 0 && HoughCirclesRadius != 0)
                            {

                                int ColorHisto = 0;
                                Image<Gray, Byte> HistoImage = HoughCirclesFrame;
                                HistoImage.ROI = new Rectangle((int)CentroidX - HoughCirclesRadius * 7, (int)CentroidY - HoughCirclesRadius * 7, HoughCirclesRadius * 7 * 2, HoughCirclesRadius * 7 * 2);
                                CroppedImage = HistoImage.Copy();
                                Histo.ClearHistogram();
                                Histo.GenerateHistograms(CroppedImage.Convert<Gray, Byte>(), 256);
                                if (FocusPoint.Text == "Histogram")
                                {
                                    for (int i = 0; i < CroppedImage.Width; i++)
                                    {
                                        for (int j = 0; j < CroppedImage.Height; j++)
                                        {
                                            ColorHisto += CroppedImage.Data[i, j, 0];
                                            if (autoHDR.Checked == true)
                                                if (CroppedImage.Data[i, j, 0] >= AppSetting.Data.hdrPixelvalueshigh)
                                                    HdrDetectionHigh += 1;
                                                else if (CroppedImage.Data[i, j, 0] <= AppSetting.Data.hdrPixelvalueslow)
                                                    HdrDetectionLow += 1;
                                                else
                                                    HdrDetectionmedium += 1;
                                        }
                                    }
                                    ColorHisto = ColorHisto / (CroppedImage.Width * CroppedImage.Height);
                                    Console.WriteLine(ColorHisto);
                                    ColorValue = ColorHisto;
                                }
                                Histo.Refresh();
                            }
                            else if (Histogramcheck.Checked == true && HoughCirclesRadius == 0)
                            {
                                Histogramcheck.Checked = false;
                                MessageBox.Show("Please turn on HoughCircles.", "Message", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                            }
                            //=========================Auto HDR===========================
                            if (autoHDR.Checked == true && FocusPoint.Text == "Histogram")
                            {

                                HdrDetectionHighText.Text = HdrDetectionHigh.ToString();
                                HdrDetectionmediumText.Text = HdrDetectionmedium.ToString();
                                HdrDetectionLowText.Text = HdrDetectionLow.ToString();

                                Console.WriteLine("HdrDetectionHigh =" + HdrDetectionHigh);
                                Console.WriteLine("HdrDetectionLow =" + HdrDetectionLow);
                                Console.WriteLine("HdrDetectionmedium =" + HdrDetectionmedium);

                                ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                                System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 50L);
                                myEncoderParameters.Param[0] = myEncoderParameter;

                                if (HdrDetectionHigh > AppSetting.Data.HdrDetectionHigh && HdrDetectionLow > AppSetting.Data.HdrDetectionLow)
                                {
                                    HdrOn = true;
                                }

                                if (HdrOn == true)
                                {

                                    Console.WriteLine("OnHDR");
                                    bool DoOne = false;

                                    if (AppSetting.Data.ExposureTime > 1)
                                    {
                                        if (ColorValue > MinLight - 2 && ColorValue <= MaxLight + 2)
                                        {
                                            ControlJump = 2.1;
                                            Console.WriteLine("ControlJump = 2;");
                                        }
                                        else if (ColorValue > MinLight - 40 && ColorValue <= MaxLight + 40)
                                        {
                                            ControlJump = 2;
                                            Console.WriteLine("ControlJump = 2;");
                                        }
                                        else
                                        {
                                            Console.WriteLine("ControlJump = 1;");
                                            ControlJump = 1;
                                            ExposureAdjust();
                                        }
                                    }
                                    else
                                    {
                                        ControlJump = 1.8;
                                    }

                                    if (ColorValue > MaxLightHdr && CameraStateText.Text != "ASI_EXP_WORKING"
                                        && Recover != false && AppSetting.Data.ExposureTime >= 1 && AppSetting.Data.ExposureTime < (AppSetting.Data.MAX_SHUTTER / 4))
                                    {
                                        //AppSetting.Data.ExposureTime -
                                        BestValue = ((AppSetting.Data.ExposureTime / GoldenRatio) * (ControlJump));
                                        Console.WriteLine("BestValue" + BestValue);
                                        AppSetting.Data.ExposureTime = Math.Round(BestValue, 2);
                                        Recover = false;
                                        if (AppSetting.Data.ExposureTime < 0.3)
                                        {
                                            AppSetting.Data.ExposureTime = 0.3;
                                        }

                                    }
                                    else if (ColorValue < MinLightHdr && CameraStateText.Text != "ASI_EXP_WORKING" && Recover != false
                                        && AppSetting.Data.ExposureTime >= 1 && AppSetting.Data.ExposureTime < (AppSetting.Data.MAX_SHUTTER / 4))
                                    {
                                        //AppSetting.Data.ExposureTime +
                                        BestValue = ((AppSetting.Data.ExposureTime * GoldenRatio) / (ControlJump));
                                        Console.WriteLine("BestValue" + BestValue);
                                        AppSetting.Data.ExposureTime = Math.Round(BestValue, 2);
                                        Recover = false;
                                    }



                                    if (AppSetting.Data.ExposureTime < 1)
                                    {

                                        float ExpTimesMedium = 1, ExpTimesMax = 1, ExpTimesMin = 1;
                                        Directory.CreateDirectory(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\HDR" + @"\");

                                        if (ModeHDR == 0)
                                        {
                                            if (ImageMergeStatus == 0 && Recover != false)
                                            {
                                                AppSetting.Data.ExposureTime = 0.5;

                                            }
                                            else
                                            {
                                                if (AppSetting.Data.ExposureTime != 0.5 && AppSetting.Data.ExposureTime != 0.3
                                                    && AppSetting.Data.ExposureTime != 0.6 && AppSetting.Data.ExposureTime != 0.7
                                                    && AppSetting.Data.ExposureTime != 0.8
                                                    && ModeHDR == 0)
                                                {
                                                    AppSetting.Data.ExposureTime = 0.5;
                                                }

                                            }

                                            if (AppSetting.Data.ExposureTime == 0.5 && DoOne == false && Recover != false)
                                            {

                                                if (ColorValue <= 50)
                                                {
                                                    AppSetting.Data.ExposureTime = 2;
                                                }

                                                HdrMedium = HdrOutput;
                                                // NonHdr = ProcessFrame;
                                                HdrMedium._GammaCorrect(0.4);
                                                Console.WriteLine("ExpTimesMedium = " + ExpTimesMedium);
                                                HdrMedium.ToBitmap().Save(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\HDR" + @"\" + "HdrMedium.jpg", jpgEncoder, myEncoderParameters);
                                                AppSetting.Data.ExposureTime = 0.3;
                                                DoOne = true;
                                                ImageMergeStatus = 1;
                                                ExpTimesMedium = (float)AppSetting.Data.ExposureTime / 1000;
                                                Recover = false;

                                            }
                                            else if (AppSetting.Data.ExposureTime == 0.3 && DoOne == false && Recover != false)
                                            {

                                                DoOne = true;
                                                ImageMergeStatus = 2;
                                                ExpTimesMin = (float)AppSetting.Data.ExposureTime / 1000;
                                                HdrLow = HdrOutput;
                                                HdrLow._GammaCorrect(0.3);
                                                Console.WriteLine("ExpTimesMin = " + ExpTimesMin);
                                                HdrLow.ToBitmap().Save(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\HDR" + @"\" + "HdrLow.jpg", jpgEncoder, myEncoderParameters);
                                                AppSetting.Data.ExposureTime = 0.6;
                                                Recover = false;
                                            }
                                            else if (AppSetting.Data.ExposureTime == 0.6 && DoOne == false && Recover != false)
                                            {
                                                DoOne = true;
                                                ExpTimesMax = (float)AppSetting.Data.ExposureTime / 1000;
                                                HdrHigh = HdrOutput;
                                                HdrHigh._GammaCorrect(0.5);
                                                HdrHigh.ToBitmap().Save(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\HDR" + @"\" + "HdrHigh.jpg", jpgEncoder, myEncoderParameters);
                                                AppSetting.Data.ExposureTime = 0.7;
                                                Recover = false;



                                            }
                                            else if (AppSetting.Data.ExposureTime == 0.7 && DoOne == false && Recover != false)
                                            {
                                                DoOne = true;
                                                HdrHighUp = HdrOutput;
                                                HdrHighUp._GammaCorrect(0.6);
                                                HdrHighUp.ToBitmap().Save(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\HDR" + @"\" + "HdrHighUp.jpg", jpgEncoder, myEncoderParameters);
                                                AppSetting.Data.ExposureTime = 0.8;
                                                Recover = false;



                                            }
                                            else if (AppSetting.Data.ExposureTime == 0.8 && DoOne == false && Recover != false)
                                            {
                                                DoOne = true;
                                                HdrHighUpper = HdrOutput;
                                                HdrHighUpper._GammaCorrect(0.7);
                                                HdrHighUpper.ToBitmap().Save(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\HDR" + @"\" + "HdrHighUpper.jpg", jpgEncoder, myEncoderParameters);
                                                AppSetting.Data.ExposureTime = 0.5;
                                                Recover = false;
                                                ImageMergeStatus = 5;



                                            }
                                        }
                                        else
                                        {
                                            if (ColorValue > MaxLightHdr && CameraStateText.Text != "ASI_EXP_WORKING" && Recover != false)
                                            {
                                                //AppSetting.Data.ExposureTime -
                                                BestValue = ((AppSetting.Data.ExposureTime / GoldenRatio) * (ControlJump));
                                                Console.WriteLine("BestValue" + BestValue);
                                                AppSetting.Data.ExposureTime = Math.Round(BestValue, 2);
                                                Recover = false;
                                                if (AppSetting.Data.ExposureTime < 0.3)
                                                {
                                                    AppSetting.Data.ExposureTime = 0.3;
                                                }

                                            }
                                            else if (ColorValue < MinLightHdr && CameraStateText.Text != "ASI_EXP_WORKING" && Recover != false)
                                            {
                                                //AppSetting.Data.ExposureTime +
                                                BestValue = ((AppSetting.Data.ExposureTime * GoldenRatio) / (ControlJump));
                                                Console.WriteLine("BestValue" + BestValue);
                                                AppSetting.Data.ExposureTime = Math.Round(BestValue, 2);
                                                Recover = false;
                                            }

                                            if (ColorValue >= MinLightHdr && ColorValue <= MaxLightHdr && DoOne == false)
                                            {


                                                Directory.CreateDirectory(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\HDR" + @"\");

                                                if (MaxLightHdr == AppSetting.Data.MaxLight && MinLightHdr == AppSetting.Data.MinLight)
                                                {
                                                    if (HdrDetectionHigh <= AppSetting.Data.HdrDetectionHigh && HdrDetectionLow <= AppSetting.Data.HdrDetectionLow)
                                                    {
                                                        HdrOn = false;
                                                        ImageMergeStatus = 0;
                                                    }
                                                    DoOne = true;
                                                    Recover = false;
                                                    ImageMergeStatus = 1;
                                                    ExpTimesMedium = (float)AppSetting.Data.ExposureTime / 1000;
                                                    HdrMedium = HdrOutput;
                                                    //NonHdr = ProcessFrame;
                                                    HdrMedium._GammaCorrect(0.4);
                                                    Console.WriteLine("ExpTimesMedium = " + ExpTimesMedium);
                                                    HdrMedium.ToBitmap().Save(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\HDR" + @"\" + "HdrMedium.jpg", jpgEncoder, myEncoderParameters);
                                                    MaxLightHdr = AppSetting.Data.MaxLight - 40;
                                                    MinLightHdr = AppSetting.Data.MinLight - 40;




                                                }
                                                else if (MaxLightHdr == AppSetting.Data.MaxLight - 40 && MinLightHdr == AppSetting.Data.MinLight - 40 && DoOne == false)
                                                {
                                                    DoOne = true;
                                                    Recover = false;
                                                    ImageMergeStatus = 2;
                                                    ExpTimesMin = (float)AppSetting.Data.ExposureTime / 1000;
                                                    HdrLow = HdrOutput;
                                                    HdrLow._GammaCorrect(0.3);
                                                    Console.WriteLine("ExpTimesMin = " + ExpTimesMin);
                                                    HdrLow.ToBitmap().Save(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\HDR" + @"\" + "HdrLow.jpg", jpgEncoder, myEncoderParameters);
                                                    MaxLightHdr = AppSetting.Data.MaxLight + 50;
                                                    MinLightHdr = AppSetting.Data.MinLight + 30;



                                                }
                                                else if (MaxLightHdr == AppSetting.Data.MaxLight + 50 && MinLightHdr == AppSetting.Data.MinLight + 30 && DoOne == false)
                                                {
                                                    DoOne = true;
                                                    Recover = false;
                                                    ImageMergeStatus = 3;
                                                    ExpTimesMax = (float)AppSetting.Data.ExposureTime / 1000;
                                                    HdrHigh = HdrOutput;
                                                    HdrHigh._GammaCorrect(0.5);
                                                    HdrHigh.ToBitmap().Save(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\HDR" + @"\" + "HdrHigh.jpg", jpgEncoder, myEncoderParameters);

                                                    MaxLightHdr = AppSetting.Data.MaxLight;
                                                    MinLightHdr = AppSetting.Data.MinLight;



                                                }
                                            }

                                        }





                                    }
                                    else if (AppSetting.Data.ExposureTime >= 1)
                                    {
                                        if (AppSetting.Data.ExposureTime < (AppSetting.Data.MAX_SHUTTER / 4))
                                        {
                                            if (ColorValue >= MinLightHdr && ColorValue <= MaxLightHdr && DoOne == false)
                                            {

                                                float ExpTimesMedium = 1, ExpTimesMax = 1, ExpTimesMin = 1;
                                                Directory.CreateDirectory(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\HDR" + @"\");

                                                if (MaxLightHdr == AppSetting.Data.MaxLight && MinLightHdr == AppSetting.Data.MinLight && DoOne == false && Recover != false)
                                                {

                                                    DoOne = true;
                                                    Recover = false;
                                                    ImageMergeStatus = 1;
                                                    ExpTimesMedium = (float)AppSetting.Data.ExposureTime / 1000;
                                                    HdrMedium = HdrOutput;
                                                    //NonHdr = ProcessFrame;
                                                    HdrMedium._GammaCorrect(0.4);
                                                    Console.WriteLine("ExpTimesMedium = " + ExpTimesMedium);
                                                    HdrMedium.ToBitmap().Save(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\HDR" + @"\" + "HdrMedium.jpg", jpgEncoder, myEncoderParameters);
                                                    MaxLightHdr = AppSetting.Data.MaxLight - 40;
                                                    MinLightHdr = AppSetting.Data.MinLight - 40;
                                                    ModeHDR = 1;



                                                }
                                                else if (MaxLightHdr == AppSetting.Data.MaxLight - 40 && MinLightHdr == AppSetting.Data.MinLight - 40 && DoOne == false && Recover != false)
                                                {
                                                    DoOne = true;
                                                    Recover = false;
                                                    ImageMergeStatus = 2;
                                                    ExpTimesMin = (float)AppSetting.Data.ExposureTime / 1000;
                                                    HdrLow = HdrOutput;
                                                    HdrLow._GammaCorrect(0.3);
                                                    Console.WriteLine("ExpTimesMin = " + ExpTimesMin);
                                                    HdrLow.ToBitmap().Save(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\HDR" + @"\" + "HdrLow.jpg", jpgEncoder, myEncoderParameters);
                                                    MaxLightHdr = AppSetting.Data.MaxLight + 50;
                                                    MinLightHdr = AppSetting.Data.MinLight + 30;
                                                    ModeHDR = 2;


                                                }
                                                else if (MaxLightHdr == AppSetting.Data.MaxLight + 50 && MinLightHdr == AppSetting.Data.MinLight + 30 && DoOne == false && Recover != false)
                                                {
                                                    DoOne = true;
                                                    Recover = false;
                                                    ImageMergeStatus = 3;
                                                    ExpTimesMax = (float)AppSetting.Data.ExposureTime / 1000;
                                                    HdrHigh = HdrOutput;
                                                    HdrHigh._GammaCorrect(0.5);
                                                    HdrHigh.ToBitmap().Save(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\HDR" + @"\" + "HdrHigh.jpg", jpgEncoder, myEncoderParameters);
                                                    MaxLightHdr = AppSetting.Data.MaxLight;
                                                    MinLightHdr = AppSetting.Data.MinLight;
                                                    ModeHDR = 3;


                                                }
                                            }


                                        }
                                        else
                                        {

                                            if (AppSetting.Data.ExposureTime == HDRHighExposure)
                                            {

                                                float ExpTimesMedium = 1, ExpTimesMax = 1, ExpTimesMin = 1;
                                                Directory.CreateDirectory(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\HDR" + @"\");

                                                if (HDRHighExposure == (AppSetting.Data.MAX_SHUTTER / 2) && Recover != false && DoOne == false)
                                                {

                                                    DoOne = true;
                                                    Recover = false;
                                                    //ImageMergeStatus = 1;
                                                    ExpTimesMedium = (float)AppSetting.Data.ExposureTime / 1000;
                                                    HdrMedium = HdrOutput;
                                                    //NonHdr = ProcessFrame;
                                                    HdrMedium._GammaCorrect(0.4);
                                                    Console.WriteLine("ExpTimesMedium = " + ExpTimesMedium);
                                                    HdrMedium.ToBitmap().Save(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\HDR" + @"\" + "HdrMedium.jpg", jpgEncoder, myEncoderParameters);
                                                    ModeHDR = 1;
                                                    AppSetting.Data.ExposureTime = AppSetting.Data.MAX_SHUTTER / 4;
                                                    HDRHighExposure = (AppSetting.Data.MAX_SHUTTER / 4);




                                                }
                                                else if (HDRHighExposure == (AppSetting.Data.MAX_SHUTTER / 4) && Recover != false && DoOne == false)
                                                {
                                                    DoOne = true;
                                                    Recover = false;
                                                    //ImageMergeStatus = 2;
                                                    ExpTimesMin = (float)AppSetting.Data.ExposureTime / 1000;
                                                    HdrLow = HdrOutput;
                                                    HdrLow._GammaCorrect(0.3);
                                                    Console.WriteLine("ExpTimesMin = " + ExpTimesMin);
                                                    HdrLow.ToBitmap().Save(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\HDR" + @"\" + "HdrLow.jpg", jpgEncoder, myEncoderParameters);
                                                    ModeHDR = 2;
                                                    AppSetting.Data.ExposureTime = AppSetting.Data.MAX_SHUTTER;
                                                    HDRHighExposure = AppSetting.Data.MAX_SHUTTER;


                                                }
                                                else if (HDRHighExposure == AppSetting.Data.MAX_SHUTTER && Recover != false && DoOne == false)
                                                {
                                                    DoOne = true;
                                                    Recover = false;
                                                    ImageMergeStatus = 3;
                                                    ExpTimesMax = (float)AppSetting.Data.ExposureTime / 1000;
                                                    HdrHigh = HdrOutput;
                                                    HdrHigh._GammaCorrect(0.5);
                                                    HdrHigh.ToBitmap().Save(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\HDR" + @"\" + "HdrHigh.jpg", jpgEncoder, myEncoderParameters);
                                                    ModeHDR = 3;
                                                    AppSetting.Data.ExposureTime = AppSetting.Data.MAX_SHUTTER / 2;
                                                    HDRHighExposure = AppSetting.Data.MAX_SHUTTER / 2;


                                                }
                                            }
                                            else
                                            {

                                                Recover = false;
                                                HDRHighExposure = AppSetting.Data.MAX_SHUTTER / 2;
                                                AppSetting.Data.ExposureTime = HDRHighExposure;
                                            }
                                        }

                                    }

                                    if (ImageMergeStatus == 3 || ImageMergeStatus == 5)
                                    {
                                        ModeHDR = 0;
                                        ImageMergeStatus = 0;
                                        if (AppSetting.Data.ExposureTime > AppSetting.Data.MAX_SHUTTER)
                                        {
                                            AppSetting.Data.ExposureTime = AppSetting.Data.MAX_SHUTTER;
                                        }
                                        if (ImageMergeStatus == 3)
                                        {
                                            for (int i = 0; i < HdrMedium.Height; i++)
                                            {
                                                for (int j = 0; j < HdrMedium.Width; j++)
                                                {
                                                    if (HdrMedium.Data[i, j, 0] > AppSetting.Data.MaxLight && HdrMedium.Data[i, j, 1] > AppSetting.Data.MaxLight && HdrMedium.Data[i, j, 2] > AppSetting.Data.MaxLight)
                                                    {
                                                        HdrMedium.Data[i, j, 0] = (byte)((HdrMedium.Data[i, j, 0] + HdrLow.Data[i, j, 0]) / 2);
                                                        HdrMedium.Data[i, j, 1] = (byte)((HdrMedium.Data[i, j, 1] + HdrLow.Data[i, j, 1]) / 2);
                                                        HdrMedium.Data[i, j, 2] = (byte)((HdrMedium.Data[i, j, 2] + HdrLow.Data[i, j, 2]) / 2);


                                                    }
                                                    else if (HdrMedium.Data[i, j, 0] < AppSetting.Data.MinLight && HdrMedium.Data[i, j, 1] < AppSetting.Data.MinLight && HdrMedium.Data[i, j, 2] < AppSetting.Data.MinLight
                                                        && HdrMedium.Data[i, j, 0] > 10 && HdrMedium.Data[i, j, 1] > 10 && HdrMedium.Data[i, j, 2] > 10)
                                                    {
                                                        HdrMedium.Data[i, j, 0] = (byte)((HdrMedium.Data[i, j, 0] + HdrHigh.Data[i, j, 0]) / 2);
                                                        HdrMedium.Data[i, j, 1] = (byte)((HdrMedium.Data[i, j, 1] + HdrHigh.Data[i, j, 1]) / 2);
                                                        HdrMedium.Data[i, j, 2] = (byte)((HdrMedium.Data[i, j, 2] + HdrHigh.Data[i, j, 2]) / 2);

                                                    }
                                                }
                                            }
                                        }
                                        else if (AppSetting.Data.ExposureTime < 1)
                                        {
                                            for (int i = 0; i < HdrMedium.Height; i++)
                                            {
                                                for (int j = 0; j < HdrMedium.Width; j++)
                                                {
                                                    if (HdrMedium.Data[i, j, 0] > AppSetting.Data.MaxLight
                                                        && HdrMedium.Data[i, j, 1] > AppSetting.Data.MaxLight
                                                        && HdrMedium.Data[i, j, 2] > AppSetting.Data.MaxLight)
                                                    {
                                                        HdrMedium.Data[i, j, 0] = (byte)((HdrMedium.Data[i, j, 0] + HdrLow.Data[i, j, 0]) / 2);
                                                        HdrMedium.Data[i, j, 1] = (byte)((HdrMedium.Data[i, j, 1] + HdrLow.Data[i, j, 1]) / 2);
                                                        HdrMedium.Data[i, j, 2] = (byte)((HdrMedium.Data[i, j, 2] + HdrLow.Data[i, j, 2]) / 2);
                                                    }
                                                    else if (HdrMedium.Data[i, j, 0] < AppSetting.Data.MinLight
                                                        && HdrMedium.Data[i, j, 1] < AppSetting.Data.MinLight
                                                        && HdrMedium.Data[i, j, 2] < AppSetting.Data.MinLight
                                                        && HdrMedium.Data[i, j, 0] > AppSetting.Data.MinLight - 10
                                                        && HdrMedium.Data[i, j, 1] > AppSetting.Data.MinLight - 10
                                                        && HdrMedium.Data[i, j, 2] > AppSetting.Data.MinLight - 10
                                                        && HdrMedium.Data[i, j, 0] > 10
                                                        && HdrMedium.Data[i, j, 1] > 10
                                                        && HdrMedium.Data[i, j, 2] > 10)
                                                    {
                                                        HdrMedium.Data[i, j, 0] = (byte)((HdrMedium.Data[i, j, 0] + HdrHigh.Data[i, j, 0]) / 2);
                                                        HdrMedium.Data[i, j, 1] = (byte)((HdrMedium.Data[i, j, 1] + HdrHigh.Data[i, j, 1]) / 2);
                                                        HdrMedium.Data[i, j, 2] = (byte)((HdrMedium.Data[i, j, 2] + HdrHigh.Data[i, j, 2]) / 2);

                                                    }
                                                    else if (HdrMedium.Data[i, j, 0] < AppSetting.Data.MinLight - 10
                                                        && HdrMedium.Data[i, j, 1] < AppSetting.Data.MinLight - 10
                                                        && HdrMedium.Data[i, j, 2] < AppSetting.Data.MinLight - 10
                                                        && HdrMedium.Data[i, j, 0] > AppSetting.Data.MinLight - 20
                                                        && HdrMedium.Data[i, j, 1] > AppSetting.Data.MinLight - 20
                                                        && HdrMedium.Data[i, j, 2] > AppSetting.Data.MinLight - 20
                                                        && HdrMedium.Data[i, j, 0] > 10
                                                        && HdrMedium.Data[i, j, 1] > 10
                                                        && HdrMedium.Data[i, j, 2] > 10)
                                                    {
                                                        HdrMedium.Data[i, j, 0] = (byte)((HdrMedium.Data[i, j, 0] + HdrHighUp.Data[i, j, 0]) / 2);
                                                        HdrMedium.Data[i, j, 1] = (byte)((HdrMedium.Data[i, j, 1] + HdrHighUp.Data[i, j, 1]) / 2);
                                                        HdrMedium.Data[i, j, 2] = (byte)((HdrMedium.Data[i, j, 2] + HdrHighUp.Data[i, j, 2]) / 2);

                                                    }
                                                    else if (HdrMedium.Data[i, j, 0] < AppSetting.Data.MinLight - 20
                                                        && HdrMedium.Data[i, j, 1] < AppSetting.Data.MinLight - 20
                                                        && HdrMedium.Data[i, j, 2] < AppSetting.Data.MinLight - 20
                                                        && HdrMedium.Data[i, j, 0] > 10
                                                        && HdrMedium.Data[i, j, 1] > 10
                                                        && HdrMedium.Data[i, j, 2] > 10)
                                                    {
                                                        HdrMedium.Data[i, j, 0] = (byte)((HdrMedium.Data[i, j, 0] + HdrHighUpper.Data[i, j, 0]) / 2);
                                                        HdrMedium.Data[i, j, 1] = (byte)((HdrMedium.Data[i, j, 1] + HdrHighUpper.Data[i, j, 1]) / 2);
                                                        HdrMedium.Data[i, j, 2] = (byte)((HdrMedium.Data[i, j, 2] + HdrHighUpper.Data[i, j, 2]) / 2);

                                                    }
                                                }
                                            }
                                        }

                                        HdrMedium._SmoothGaussian(1);
                                        HdrMedium._EqualizeHist();
                                        HdrMedium._GammaCorrect(1.4d);
                                        Image<Bgr, Byte> ImageHdrMedium = HdrMedium;
                                        ImageHdrMedium.Draw(BorderTime, new Bgr(Color.Black), -1);
                                        ImageHdrMedium.Draw(BorderTime, new Bgr(Color.White), 2);
                                        ImageHdrMedium.Draw(BorderExposureTime, new Bgr(Color.Black), -1);
                                        ImageHdrMedium.Draw(BorderExposureTime, new Bgr(Color.White), 2);
                                        CvInvoke.PutText(ImageHdrMedium, "UTC " + TimesStamp, new Point(0, BorderHeight + 50), FontFace.HersheySimplex, 1.5, new Bgr(Color.White).MCvScalar, Thickness);

                                        if (ExposureTimeShow < 1000 && ExposureTimeShow > 1)
                                        {
                                            CvInvoke.PutText(ImageHdrMedium, Math.Round(AppSetting.Data.ExposureTime, 0) + " ms", new Point(BorderWidth + 50, BorderHeight + 50), FontFace.HersheySimplex, 1.5, new Bgr(Color.White).MCvScalar, Thickness);
                                        }
                                        else if (ExposureTimeShow >= 1000)
                                        {
                                            CvInvoke.PutText(ImageHdrMedium, Math.Round(AppSetting.Data.ExposureTime / 1000, 0) + " sec", new Point(BorderWidth + 50, BorderHeight + 50), FontFace.HersheySimplex, 1.5, new Bgr(Color.White).MCvScalar, Thickness);
                                        }
                                        else if (ExposureTimeShow >= 0 && ExposureTimeShow <= 1)
                                        {
                                            CvInvoke.PutText(ImageHdrMedium, Math.Round(AppSetting.Data.ExposureTime, 2) + " ms", new Point(BorderWidth + 50, BorderHeight + 50), FontFace.HersheySimplex, 1.5, new Bgr(Color.White).MCvScalar, Thickness);
                                        }
                                        CvInvoke.PutText(ImageHdrMedium, "AUTO HDR", new Point(BorderWidth - 300, BorderHeight + 50), FontFace.HersheySimplex, 1.5, new Bgr(Color.White).MCvScalar, Thickness);
                                        Directory.CreateDirectory(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\" + TimeFolder);
                                        //Directory.CreateDirectory(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\NonHdr\" + TimeFolder);

                                        if (AppSetting.Data.SaveFileDialog != "" && TimeNowChack != TimeBefore && ConnectedCameras > 0 || AppSetting.Data.ExposureTime >= 60000)
                                        {
                                            if (StopSave.Checked == false)
                                            {
                                                TimeBefore = DateTime.Now.ToString("yyyy_MM_dd__HH_mm");
                                                //NonHdr.ToBitmap().Save(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\NonHdr\" + TimeFolder + @"\" + TimeNow + ".jpg", jpgEncoder,
                                                //myEncoderParameters);
                                                ImageHdrMedium.ToBitmap().Save(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\" + TimeFolder + @"\" + TimeNow + ".jpg", jpgEncoder, myEncoderParameters);
                                            }

                                            HdrOn = false;

                                        }
                                        MainImageControl.Image = ImageHdrMedium;


                                    }

                                }
                                else
                                {
                                    if (AppSetting.Data.ExposureTime > AppSetting.Data.MAX_SHUTTER)
                                    {
                                        AppSetting.Data.ExposureTime = AppSetting.Data.MAX_SHUTTER;
                                    }
                                    MainImageControl.Image = ImageFrame;
                                    if (AppSetting.Data.ExposureTime > 1)
                                    {
                                        if (ColorValue > MinLight - 2 && ColorValue <= MaxLight + 2)
                                        {
                                            ControlJump = 2.11;
                                            Console.WriteLine("ControlJump = 2.11;");
                                        }
                                        else if (ColorValue > MinLight - 5 && ColorValue <= MaxLight + 5)
                                        {
                                            ControlJump = 2.1;
                                            Console.WriteLine("ControlJump = 2.1;");
                                        }
                                        else if (ColorValue > MinLight - 40 && ColorValue <= MaxLight + 40)
                                        {
                                            ControlJump = 2;
                                            Console.WriteLine("ControlJump = 2;");
                                        }
                                        else
                                        {
                                            Console.WriteLine("ControlJump = 1;");
                                            ControlJump = 1;
                                            ExposureAdjust();
                                        }
                                    }
                                    else
                                    {
                                        ControlJump = 1.7;
                                    }

                                    if (ColorValue > MaxLightHdr && CameraStateText.Text != "ASI_EXP_WORKING"
                                        && Recover != false)
                                    {
                                        //AppSetting.Data.ExposureTime -
                                        BestValue = ((AppSetting.Data.ExposureTime / GoldenRatio) * (ControlJump));
                                        Console.WriteLine("BestValue" + BestValue);
                                        AppSetting.Data.ExposureTime = Math.Round(BestValue, 2);
                                        Recover = false;
                                        if (AppSetting.Data.ExposureTime < 0.3)
                                        {
                                            AppSetting.Data.ExposureTime = 0.3;
                                        }
                                        if (AppSetting.Data.ExposureTime >= AppSetting.Data.MAX_SHUTTER)
                                        {
                                            AppSetting.Data.ExposureTime = AppSetting.Data.MAX_SHUTTER;
                                        }

                                    }
                                    else if (ColorValue < MinLightHdr && CameraStateText.Text != "ASI_EXP_WORKING"
                                        && Recover != false
                                        && AppSetting.Data.ExposureTime <= AppSetting.Data.MAX_SHUTTER)
                                    {
                                        //AppSetting.Data.ExposureTime +
                                        BestValue = ((AppSetting.Data.ExposureTime * GoldenRatio) / (ControlJump));
                                        Console.WriteLine("BestValue" + BestValue);
                                        AppSetting.Data.ExposureTime = Math.Round(BestValue, 2);
                                        Recover = false;
                                    }
                                    //TimeBefore = DateTime.Now.ToString("yyyy_MM_dd__HH_mm");
                                    Directory.CreateDirectory(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\" + TimeFolder);

                                    if (AppSetting.Data.SaveFileDialog != "" && TimeNowChack != TimeBefore && ConnectedCameras > 0)
                                    {

                                        TimeBefore = DateTime.Now.ToString("yyyy_MM_dd__HH_mm");
                                        ImageFrame.ToBitmap().Save(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\" + TimeFolder + @"\" + TimeNow + ".jpg", jpgEncoder, myEncoderParameters);

                                    }
                                }
                            }
                            else if (autoHDR.CheckState != 0 && FocusPoint.Text != "Histogram")
                            {
                                autoHDR.Checked = false;
                                MessageBox.Show("Please select FocusPoint.Text to Histogram.", "Message", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                            }
                            //=========================Force HDR===========================
                            else if (hdr_On.CheckState != 0)
                            {
                                bool DoOne;
                                DoOne = false;
                                int ImageMergeStatus = 0;
                                autoHDR.Checked = false;
                                if (AppSetting.Data.ExposureTime > 1)
                                {

                                    if (ColorValue > MinLight - 2 && ColorValue <= MaxLight + 2 && AppSetting.Data.ExposureTime < 5000)
                                    {
                                        ControlJump = 2.1;
                                        Console.WriteLine("ControlJump = 2;");
                                    }
                                    else if (ColorValue > MinLight - 40 && ColorValue <= MaxLight + 40)
                                    {
                                        ControlJump = 2;
                                        Console.WriteLine("ControlJump = 2;");
                                    }
                                    else
                                    {
                                        Console.WriteLine("ControlJump = 1;");
                                        ControlJump = 1;
                                        ExposureAdjust();
                                    }

                                }
                                else
                                {
                                    ControlJump = 1.6;
                                }

                                if (ColorValue > MaxLightHdr && CameraStateText.Text != "ASI_EXP_WORKING" && Recover != false)
                                {
                                    //AppSetting.Data.ExposureTime -
                                    BestValue = ((AppSetting.Data.ExposureTime / GoldenRatio) * (ControlJump));
                                    Console.WriteLine("BestValue" + BestValue);
                                    AppSetting.Data.ExposureTime = Math.Round(BestValue, 2);
                                    Recover = false;
                                    if (AppSetting.Data.ExposureTime < 0.3)
                                    {
                                        AppSetting.Data.ExposureTime = 0.3;
                                    }

                                }
                                else if (ColorValue < MinLightHdr && CameraStateText.Text != "ASI_EXP_WORKING" && Recover != false)
                                {
                                    //AppSetting.Data.ExposureTime +
                                    BestValue = ((AppSetting.Data.ExposureTime * GoldenRatio) / (ControlJump));
                                    Console.WriteLine("BestValue" + BestValue);
                                    AppSetting.Data.ExposureTime = Math.Round(BestValue, 2);
                                    Recover = false;

                                }
                                else if (ColorValue >= MinLightHdr - 10 && ColorValue <= MaxLightHdr + 10 && DoOne == false)
                                {
                                    ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                                    System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                                    EncoderParameters myEncoderParameters = new EncoderParameters(1);
                                    EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 50L);
                                    myEncoderParameters.Param[0] = myEncoderParameter;
                                    float ExpTimesMedium = 1, ExpTimesMax = 1, ExpTimesMin = 1;
                                    Directory.CreateDirectory(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\HDR" + @"\");
                                    if (MaxLightHdr == AppSetting.Data.MaxLight && MinLightHdr == AppSetting.Data.MinLight)
                                    {
                                        DoOne = true;
                                        ImageMergeStatus = 1;
                                        ExpTimesMedium = (float)AppSetting.Data.ExposureTime / 1000;
                                        //ProcessFrame.ToBitmap().Save(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\HDR" + @"\" + "HdrMedium.jpg", jpgEncoder, myEncoderParameters);
                                        HdrMedium = HdrOutput;
                                        //NonHdr = ProcessFrame;
                                        HdrMedium._GammaCorrect(0.5);
                                        Console.WriteLine("ExpTimesMedium = " + ExpTimesMedium);
                                        HdrMedium.ToBitmap().Save(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\HDR" + @"\" + "HdrMedium.jpg", jpgEncoder, myEncoderParameters);
                                        MaxLightHdr = AppSetting.Data.MaxLight - 40;
                                        MinLightHdr = AppSetting.Data.MinLight - 40;



                                    }
                                    else if (MaxLightHdr == AppSetting.Data.MaxLight - 40 && MinLightHdr == AppSetting.Data.MinLight - 40 && DoOne == false)
                                    {
                                        DoOne = true;
                                        ImageMergeStatus = 2;
                                        ExpTimesMin = (float)AppSetting.Data.ExposureTime / 1000;
                                        HdrLow = HdrOutput;
                                        HdrLow._GammaCorrect(0.5);
                                        Console.WriteLine("ExpTimesMin = " + ExpTimesMin);
                                        HdrLow.ToBitmap().Save(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\HDR" + @"\" + "HdrLow.jpg", jpgEncoder, myEncoderParameters);
                                        MaxLightHdr = AppSetting.Data.MaxLight + 50;
                                        MinLightHdr = AppSetting.Data.MinLight + 30;


                                    }
                                    else if (MaxLightHdr == AppSetting.Data.MaxLight + 50 && MinLightHdr == AppSetting.Data.MinLight + 30 && DoOne == false)
                                    {
                                        DoOne = true;
                                        ImageMergeStatus = 3;
                                        ExpTimesMax = (float)AppSetting.Data.ExposureTime / 1000;
                                        HdrHigh = HdrOutput;
                                        HdrHigh._GammaCorrect(0.5);
                                        HdrHigh.ToBitmap().Save(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\HDR" + @"\" + "HdrHigh.jpg", jpgEncoder, myEncoderParameters);
                                        MaxLightHdr = AppSetting.Data.MaxLight;
                                        MinLightHdr = AppSetting.Data.MinLight;
                                    }
                                    if (ImageMergeStatus == 3)
                                    {
                                        if (AppSetting.Data.ExposureTime > AppSetting.Data.MAX_SHUTTER)
                                        {
                                            AppSetting.Data.ExposureTime = AppSetting.Data.MAX_SHUTTER;
                                        }
                                        ImageMergeStatus = 0;

                                        for (int i = 0; i < HdrMedium.Height; i++)
                                        {
                                            for (int j = 0; j < HdrMedium.Width; j++)
                                            {
                                                if (HdrMedium.Data[i, j, 0] > AppSetting.Data.MaxLight
                                                    && HdrMedium.Data[i, j, 1] > AppSetting.Data.MaxLight
                                                    && HdrMedium.Data[i, j, 2] > AppSetting.Data.MaxLight)
                                                {

                                                    HdrMedium.Data[i, j, 0] = (byte)((HdrMedium.Data[i, j, 0] + HdrLow.Data[i, j, 0]) / 2);
                                                    HdrMedium.Data[i, j, 1] = (byte)((HdrMedium.Data[i, j, 1] + HdrLow.Data[i, j, 1]) / 2);
                                                    HdrMedium.Data[i, j, 2] = (byte)((HdrMedium.Data[i, j, 2] + HdrLow.Data[i, j, 2]) / 2);

                                                }
                                                else if (HdrMedium.Data[i, j, 0] < AppSetting.Data.MinLight
                                                    && HdrMedium.Data[i, j, 1] < AppSetting.Data.MinLight
                                                    && HdrMedium.Data[i, j, 2] < AppSetting.Data.MinLight
                                                    && HdrMedium.Data[i, j, 0] > 10
                                                    && HdrMedium.Data[i, j, 1] > 10
                                                    && HdrMedium.Data[i, j, 2] > 10)
                                                {

                                                    HdrMedium.Data[i, j, 0] = (byte)((HdrMedium.Data[i, j, 0] + HdrHigh.Data[i, j, 0]) / 2);
                                                    HdrMedium.Data[i, j, 1] = (byte)((HdrMedium.Data[i, j, 1] + HdrHigh.Data[i, j, 1]) / 2);
                                                    HdrMedium.Data[i, j, 2] = (byte)((HdrMedium.Data[i, j, 2] + HdrHigh.Data[i, j, 2]) / 2);

                                                }

                                            }


                                        }
                                        HdrMedium._SmoothGaussian(1);
                                        HdrMedium._EqualizeHist();
                                        HdrMedium._GammaCorrect(1.3d);
                                        Image<Bgr, Byte> ImageHdrMedium = HdrMedium;
                                        ImageHdrMedium.Draw(BorderTime, new Bgr(Color.Black), -1);
                                        ImageHdrMedium.Draw(BorderTime, new Bgr(Color.White), 2);
                                        ImageHdrMedium.Draw(BorderExposureTime, new Bgr(Color.Black), -1);
                                        ImageHdrMedium.Draw(BorderExposureTime, new Bgr(Color.White), 2);
                                        CvInvoke.PutText(ImageHdrMedium, "UTC " + TimesStamp, new Point(0, BorderHeight + 50), FontFace.HersheySimplex, 1.5, new Bgr(Color.White).MCvScalar, Thickness);
                                        if (ExposureTimeShow < 1000 && ExposureTimeShow > 1)
                                        {
                                            CvInvoke.PutText(ImageHdrMedium, Math.Round(AppSetting.Data.ExposureTime, 0) + " ms", new Point(BorderWidth + 50, BorderHeight + 50), FontFace.HersheySimplex, 1.5, new Bgr(Color.White).MCvScalar, Thickness);
                                        }
                                        else if (ExposureTimeShow >= 1000)
                                        {
                                            CvInvoke.PutText(ImageHdrMedium, Math.Round(AppSetting.Data.ExposureTime / 1000, 0) + " sec", new Point(BorderWidth + 50, BorderHeight + 50), FontFace.HersheySimplex, 1.5, new Bgr(Color.White).MCvScalar, Thickness);
                                        }
                                        else if (ExposureTimeShow >= 0 && ExposureTimeShow <= 1)
                                        {
                                            CvInvoke.PutText(ImageHdrMedium, Math.Round(AppSetting.Data.ExposureTime, 2) + " ms", new Point(BorderWidth + 50, BorderHeight + 50), FontFace.HersheySimplex, 1.5, new Bgr(Color.White).MCvScalar, Thickness);
                                        }

                                        CvInvoke.PutText(ImageHdrMedium, "HDR ON", new Point(BorderWidth - 200, BorderHeight + 50), FontFace.HersheySimplex, 1.5, new Bgr(Color.White).MCvScalar, Thickness);
                                        Directory.CreateDirectory(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\" + TimeFolder);
                                        //Directory.CreateDirectory(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\NonHdr\" + TimeFolder);
                                        //TimeNowChack = DateTime.Now.ToString("yyyy_MM_dd__HH_mm");
                                        if (AppSetting.Data.SaveFileDialog != "" && TimeNowChack != TimeBefore && ConnectedCameras > 0 && StopSave.Checked == false || AppSetting.Data.ExposureTime >= 60000)
                                        {
                                            TimeBefore = DateTime.Now.ToString("yyyy_MM_dd__HH_mm");


                                            //NonHdr.ToBitmap().Save(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\NonHdr\" + TimeFolder + @"\" + TimeNow + ".jpg", jpgEncoder,
                                            //myEncoderParameters);

                                            ImageHdrMedium.ToBitmap().Save(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\" + TimeFolder + @"\" + TimeNow + ".jpg", jpgEncoder,
                                            myEncoderParameters);
                                        }


                                        MainImageControl.Image = ImageHdrMedium;
                                    }





                                }




                            }
                            if (autoHDR.Checked == false && hdr_On.Checked == false)
                            {


                                MinLight = AppSetting.Data.MinLight;
                                MaxLight = AppSetting.Data.MaxLight;

                                if (AppSetting.Data.ExposureTime > 1)
                                {



                                    if (ColorValue > MinLight - 2 && ColorValue <= MaxLight + 2)
                                    {
                                        ControlJump = 2.11;
                                        Console.WriteLine("ControlJump = 2.11;");
                                    }
                                    else if (ColorValue > MinLight - 5 && ColorValue <= MaxLight + 5)
                                    {
                                        ControlJump = 2.1;
                                        Console.WriteLine("ControlJump = 2.1;");
                                    }
                                    else if (ColorValue > MinLight - 40 && ColorValue <= MaxLight + 40)
                                    {
                                        ControlJump = 2;
                                        Console.WriteLine("ControlJump = 2;");
                                    }
                                    else
                                    {
                                        Console.WriteLine("ControlJump = 1;");
                                        ControlJump = 1;
                                        ExposureAdjust();
                                    }

                                }
                                else
                                {
                                    ControlJump = 1.7;
                                }
                                if (ColorValue >= MaxLight && CameraStateText.Text != "ASI_EXP_WORKING" && Recover != false
                                   )
                                {

                                    BestValue = ((AppSetting.Data.ExposureTime / GoldenRatio) * (ControlJump));
                                    Console.WriteLine("BestValue" + BestValue);
                                    AppSetting.Data.ExposureTime = Math.Round(BestValue, 2);
                                    Recover = false;
                                    if (AppSetting.Data.ExposureTime < 0.3)
                                    {
                                        AppSetting.Data.ExposureTime = 0.3;
                                    }
                                    //AppSetting.Data.ExposureTime -
                                }
                                else if (ColorValue <= MinLight && CameraStateText.Text != "ASI_EXP_WORKING" && Recover != false && AppSetting.Data.ExposureTime <= AppSetting.Data.MAX_SHUTTER)
                                {
                                    if (AppSetting.Data.ExposureTime < 0.3)
                                    {
                                        AppSetting.Data.ExposureTime = 0.3;
                                    }

                                    BestValue = ((AppSetting.Data.ExposureTime * GoldenRatio) / (ControlJump));
                                    Console.WriteLine("BestValue" + BestValue);
                                    AppSetting.Data.ExposureTime = Math.Round(BestValue, 2);

                                    Recover = false;
                                    //AppSetting.Data.ExposureTime +
                                }
                                else if (AppSetting.Data.ExposureTime > AppSetting.Data.MAX_SHUTTER)
                                {
                                    AppSetting.Data.ExposureTime = AppSetting.Data.MAX_SHUTTER;
                                }


                            }


                            ExpouseTimeText.Value = (decimal)AppSetting.Data.ExposureTime;

                            KeoGramsImage = new Rectangle(new Point(CameraWidth / 2, CameraHeight / 2), new Size(1, CameraHeight));
                            KeoGramsFrame.ROI = KeoGramsImage;
                            Bitmap outputImage = KeoGramsFrame.ToBitmap();



                            if (keoGrams.Checked == true && StopSave.Checked == false)
                            {
                                ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                                System.Drawing.Imaging.Encoder myEncoder =
                                        System.Drawing.Imaging.Encoder.Quality;
                                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 50L);


                                myEncoderParameters.Param[0] = myEncoderParameter;

                                string timeMinutes = DateTime.Now.AddMinutes(-1).ToString("yyyy_MM_dd__HH_mm");
                                Directory.CreateDirectory(AppSetting.Data.SaveFileDialog + @"\keoGrams" + @"\" + TimeFolder);
                                Bitmap KeoGramsImage = KeoGramsFrame.ToBitmap();
                                string pathKeograms = AppSetting.Data.SaveFileDialog + @"\keoGrams" + @"\" + TimeFolder + @"\" + timeMinutes + "keoGrams" + ".jpg";
                                Console.WriteLine(pathKeograms);
                                int Minutesinday = (int)DateTime.Now.TimeOfDay.TotalMinutes;


                                if (File.Exists(pathKeograms))
                                {
                                    KeoGramsImage = new Bitmap(pathKeograms);
                                    Console.WriteLine("KeoGramsImage1");
                                }
                                else if (File.Exists(AppSetting.Data.SaveFileDialog + @"\keoGrams" + @"\" + TimeFolder + @"\" + "keoGrams" + ".jpg") && !File.Exists(pathKeograms))
                                {
                                    KeoGramsImage = new Bitmap(AppSetting.Data.SaveFileDialog + @"\keoGrams" + @"\" + TimeFolder + @"\" + "keoGrams" + ".jpg");
                                    Console.WriteLine("KeoGramsImage2");
                                }

                                Bitmap keograms = KeoGramsFrame.ToBitmap();

                                if (!File.Exists(AppSetting.Data.SaveFileDialog + @"\keoGrams" + @"\" + TimeFolder + @"\" + "keoGrams" + ".jpg"))
                                {



                                    var bitmap = new Bitmap(Minutesinday, KeoGramsImage.Height);

                                    for (var x = 0; x < bitmap.Width; x++)
                                    {
                                        for (var y = 0; y < bitmap.Height; y++)
                                        {

                                            bitmap.SetPixel(x, y, Color.Black);

                                        }
                                    }

                                    bitmap.Save(AppSetting.Data.SaveFileDialog + @"\keoGrams" + @"\" + TimeFolder + @"\" + "keoGrams" + ".jpg", jpgEncoder,
                                    myEncoderParameters);
                                    Console.WriteLine("KeoGramsImagebitmap1");

                                }
                                else if (!File.Exists(pathKeograms))
                                {
                                    Console.WriteLine("KeoGramsImagebitmap2");
                                    var directory = new DirectoryInfo(AppSetting.Data.SaveFileDialog + @"\keoGrams" + @"\" + TimeFolder);
                                    var myFile = (from f in directory.GetFiles()
                                                  orderby f.LastWriteTime descending
                                                  select f).First();

                                    var bitmap = new Bitmap(AppSetting.Data.SaveFileDialog + @"\keoGrams" + @"\" + TimeFolder + @"\" + myFile.ToString());

                                    for (var x = bitmap.Width; x < Minutesinday; x++)
                                    {
                                        for (var y = 0; y < bitmap.Height; y++)
                                        {

                                            bitmap.SetPixel(x, y, Color.Black);

                                        }
                                    }

                                    KeoGramsImage = bitmap;

                                }




                                //int outputImageHeight = KeoGramsImage.Height > keograms.Height ? KeoGramsImage.Height : keograms.Height;
                                int outputImageHeight = KeoGramsImage.Height;
                                int outputImageWidth = KeoGramsImage.Width + keograms.Width;

                                outputImage = new Bitmap(outputImageWidth, outputImageHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                                Graphics graphics = Graphics.FromImage(outputImage);

                                graphics.DrawImage(KeoGramsImage, new Rectangle(new Point(), KeoGramsImage.Size),
                                    new Rectangle(new Point(), KeoGramsImage.Size), GraphicsUnit.Pixel);
                                graphics.DrawImage(keograms, new Rectangle(new Point(KeoGramsImage.Width, 0), keograms.Size),
                                    new Rectangle(new Point(), keograms.Size), GraphicsUnit.Pixel);


                                //File.Delete(AppSetting.Data.SaveFileDialog + @"\keoGrams" + @"\" + TimeFolder + @"\" + "keoGrams" + ".jpg");
                                if (AppSetting.Data.SaveFileDialog != "" && TimeNowChack != TimeBefore && StopSave.Checked == false)
                                {
                                    TimeBefore = DateTime.Now.ToString("yyyy_MM_dd__HH_mm");
                                    outputImage.Save(AppSetting.Data.SaveFileDialog + @"\keoGrams" + @"\" + TimeFolder + @"\" + DateTime.Now.ToString("yyyy_MM_dd__HH_mm") + "keoGrams" + ".jpg", jpgEncoder,
                                    myEncoderParameters);
                                }



                            }

                            if (AppSetting.Data.SaveFileDialog != "" && TimeNowChack != TimeBefore
                                && ColorValue <= 150
                                && ConnectedCameras > 0 && autoHDR.CheckState == 0
                                && hdr_On.CheckState == 0
                                && StopSave.Checked == false)
                            {
                                TimeBefore = DateTime.Now.ToString("yyyy_MM_dd__HH_mm");

                                ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                                System.Drawing.Imaging.Encoder myEncoder =
                                        System.Drawing.Imaging.Encoder.Quality;
                                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder,
                                50L);

                                myEncoderParameters.Param[0] = myEncoderParameter;
                                Directory.CreateDirectory(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\" + TimeFolder);

                                BmpImageFrame.Save(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\" + TimeFolder + @"\" + TimeNow + ".jpg", jpgEncoder,
                                    myEncoderParameters);



                                if (SaveLog.CheckState != 0)
                                {
                                    File.AppendAllText(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\log " + TimeFolder + ".txt", TimeNow + " ExpouseTime = " + ExpouseTimeText.Value.ToString() + " Exposuring = " + ExposuringText.Text.ToString() + " Color(0-255) = " + ColorValue + "\n");
                                }

                            }





                        }

                    }

                }
                GC.Collect();

            }
        }

        private void ExpouseTimeText_ValueChanged(object sender, EventArgs e)
        {
            if (ExpouseTimeText.Value > 0 && IsAutoExposureTime.CheckState == 0)
            {
                AppSetting.Data.ExposureTime = (double)ExpouseTimeText.Value;
                AppSetting.Save();
            }
        }

        private void MainImageControl_MouseClick(object sender, MouseEventArgs e)
        {
            int offsetX = (int)(e.Location.X / MainImageControl.ZoomScale);
            int offsetY = (int)(e.Location.Y / MainImageControl.ZoomScale);
            int horizontalScrollBarValue = MainImageControl.HorizontalScrollBar.Visible ? (int)MainImageControl.HorizontalScrollBar.Value : 0;
            int verticalScrollBarValue = MainImageControl.VerticalScrollBar.Visible ? (int)MainImageControl.VerticalScrollBar.Value : 0;

            if (!IsDefineROI)
            {
                IsDefineROI = true;
                AppSetting.Data.ROIX = (offsetX + horizontalScrollBarValue);
                AppSetting.Data.ROIY = (offsetY + horizontalScrollBarValue);
            }
            else
            {
                IsDefineROI = false;
                int EndPointX = (offsetX + horizontalScrollBarValue);
                int EndPointY = (offsetY + horizontalScrollBarValue);

                AppSetting.Data.ROIWidth = Math.Abs(AppSetting.Data.ROIX - EndPointX);
                AppSetting.Data.ROIHeight = Math.Abs(AppSetting.Data.ROIY - EndPointY);
                AppSetting.Save();

                Size ROISize = new Size(Math.Abs(AppSetting.Data.ROIX - EndPointX), Math.Abs(AppSetting.Data.ROIY - EndPointY));
                Point ROIPoint = new Point(AppSetting.Data.ROIX, AppSetting.Data.ROIY);
                ROIRec = new Rectangle(ROIPoint, ROISize);
            }
        }

    }
}
