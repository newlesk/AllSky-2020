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
        private Image<Bgr, Byte> RootFrame = null, ProcessFrame = null, ROIFrame, circleImage = null, keoGramsFrame = null;
        private Image<Gray, Byte> houghCircles_Frame = null, ProcessFrameGray = null;
        private Rectangle ROIRec;
        private Rectangle keoGramsImage;
        private IntPtr imageBuf;
        private bool GetImageState = false;
        private bool IsDefineROI = false;
        private bool IsDefineOrigin = false;
        private int CameraId, ConnectedCameras;
        public string SaveFileDialog;
        private int colorValue;
        private double centroidX, centroidY;
        private ASI_CAMERA_INFO Info;
        private Stopwatch exposureCounter;
        private string folderName;
        private string timeNow;
        private string timeFolder;
        private string timeBefore;
        private string timeNow_Chack;
        private int houghCircles_X, houghCircles_Y;
        private bool IsExpSuccess;
        private bool recover;
        private double cannyThreshold;
        private double circleAccumulatorThreshold;
        private double goldenRatio = (1 + (Math.Sqrt(5)) / 2);
        private int houghCirclesradius = 0;
        private bool houghCircles_Status, cameraLost = false;
        private int maxLight_hdr;
        private int minLight_hdr;
        private Image<Bgr, Byte> hdrHigh = null;
        private Image<Bgr, Byte> hdrLow = null;
        private Image<Bgr, Byte> hdrMedium = null;
        private Image<Bgr, Byte> hdrOutput = null;
        private Image<Gray, Byte> croppedImage = null;
        private Image<Bgr, Byte> imageFrame = null;
        private int maxLight;
        private int minLight;
        private bool hdrOn = false;


        public MainWindows()
        {
            InitializeComponent();
        }

        private void InitializeSystem()
        {
            exposureCounter = new Stopwatch();
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
            recover = false;
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
            pixelvalues_min.Text = AppSetting.Data.minLight.ToString();
            pixelvalues_max.Text = AppSetting.Data.maxLight.ToString();
            HoughCircles_Profile.Text = "Select Your Profile";
            ProfilePixelValues.Text = "Select Your Profile";
            minLight_hdr = AppSetting.Data.minLight;
            maxLight_hdr = AppSetting.Data.maxLight;
            int HoughCircleslineCount = File.ReadLines(@"./HoughCircles_Profile.txt").Count();
            int PixellineCount = File.ReadLines(@"./Pixel_Profile.txt").Count();
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
                if (cameraLost == true)
                {

                }

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

                        exposureCounter.Restart();
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

                        exposureCounter.Stop();

                        if (imageBuf != IntPtr.Zero)
                        {
                            Marshal.FreeCoTaskMem(imageBuf);
                            imageBuf = IntPtr.Zero;
                        }

                        imageBuf = Marshal.AllocCoTaskMem((int)AppSetting.Data.ImageSize);
                        GetExpError = ASICameraDll2.ASIGetDataAfterExp(CameraId, imageBuf, (int)AppSetting.Data.ImageSize);
                        if (GetExpError == ASI_ERROR_CODE.ASI_SUCCESS)
                        {
                            RootFrame = new Image<Bgr, byte>((int)AppSetting.Data.ImageWidth, (int)AppSetting.Data.ImageHeight, (int)AppSetting.Data.ImageWidth * 3, imageBuf);
                            ProcessFrameGray = RootFrame.Convert<Gray, Byte>();
                            ProcessFrame = RootFrame.Copy();
                            houghCircles_Frame = RootFrame.Convert<Gray, Byte>();
                            ROIFrame = RootFrame.Copy();
                            keoGramsFrame = RootFrame.Copy();
                            hdrOutput = RootFrame.Copy();
                            recover = true;
                        }

                    }
                    else
                    {
                        ASICameraDll2.ASICloseCamera(CameraId);
                        CameraConnection();
                    }
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    if (colorValue <= 135 && AppSetting.Data.ExposureTime < 1 && SpeedMode.CheckState == 0)
                    {
                        await Task.Delay(50000);
                    }
                    else if (colorValue <= AppSetting.Data.maxLight && colorValue >= AppSetting.Data.minLight && AppSetting.Data.ExposureTime < 1000 && SpeedMode.CheckState == 0)
                    {
                        await Task.Delay(50000);
                    }
                    else if (colorValue <= AppSetting.Data.maxLight && colorValue >= AppSetting.Data.minLight && AppSetting.Data.ExposureTime >= 1000 && AppSetting.Data.ExposureTime < 10000 && SpeedMode.CheckState == 0)
                    {
                        await Task.Delay(5000);
                    }
                    else
                        await Task.Delay(600);
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
                folderName = folderBrowserDialog1.SelectedPath;
            }

            AppSetting.Data.SaveFileDialog = folderName;
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
            Application.Restart();
        }
        private void ResetZoom_Click(object sender, EventArgs e)
        {
            ROIRec.Width = 0;
            ROIRec.Height = 0;
        }


        private void SavePixelValues_Click(object sender, EventArgs e)
        {
            AppSetting.Data.maxLight = int.Parse(pixelvalues_max.Text);
            AppSetting.Data.minLight = int.Parse(pixelvalues_min.Text);
            maxLight_hdr = AppSetting.Data.maxLight;
            minLight_hdr = AppSetting.Data.minLight;
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


                    maxLight = int.Parse(lines[0]);


                    minLight = int.Parse(lines[1]);
                }

                else if (ProfilePixelValues.Text == "Profile " + i && ProfilePixelValues.Text != "Profile 0")
                {

                    maxLight = int.Parse(lines[i * 2]);


                    minLight = int.Parse(lines[i * 2 + 1]);
                }


            }

            pixelvalues_min.Text = minLight.ToString();
            pixelvalues_max.Text = maxLight.ToString();
            AppSetting.Data.maxLight = maxLight;
            AppSetting.Data.minLight = minLight;
            maxLight_hdr = AppSetting.Data.maxLight;
            minLight_hdr = AppSetting.Data.minLight;
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

        private void Save_HoughCircles_Click(object sender, EventArgs e)
        {
            houghCircles_Status = true;
            int cameraWidth = Int16.Parse(ROITextWidth.Text);
            int cameraHeight = Int16.Parse(ROITextHeight.Text);
            StringBuilder msgBuilder = new StringBuilder("Performance: ");

            //Load the image from file and resize it for display
            Image<Bgr, Byte> img = ROIFrame.Resize(cameraWidth / 10, cameraHeight / 10, Emgu.CV.CvEnum.Inter.Linear, true);

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

            cannyThreshold = double.Parse(cannyThreshold_Box.Text);
            circleAccumulatorThreshold = double.Parse(circleAccumulatorThreshold_Box.Text);

            //double circleAccumulatorThreshold = 120;

            circleAccumulatorThreshold_Box.Text = circleAccumulatorThreshold.ToString();
            //CircleF[] circles = CvInvoke.HoughCircles(uimage, HoughType.Gradient, 2.0, 20.0, cannyThreshold, circleAccumulatorThreshold, 5);
            CircleF[] circles = CvInvoke.HoughCircles(uimage, HoughType.Gradient, 1.5, 27.0, cannyThreshold, circleAccumulatorThreshold, 5);
            System.Diagnostics.Debug.WriteLine(circles);
            watch.Stop();
            msgBuilder.Append(String.Format("Hough circles - {0} ms; ", watch.ElapsedMilliseconds));
            #endregion

            #region draw circles
            circleImage = img;
            foreach (CircleF circle in circles)
            {
                circleImage.Draw(circle, new Bgr(Color.Red), 2);
                houghCircles_X = (int)circle.Center.X;
                houghCircles_Y = (int)circle.Center.Y;
                houghCirclesradius = (int)circle.Radius;

            }

            HoughCircles.Image = circleImage;
            #endregion
            //IsAutoExposureTime.Checked = true;
            houghCircles_Status = false;
        }

        private void SaveProfile_HoughCircles_Click(object sender, EventArgs e)
        {
            File.AppendAllText(@"./HoughCircles_Profile.txt", cannyThreshold_Box.Text + "\n" + circleAccumulatorThreshold_Box.Text + "\n");
            int lineCount = File.ReadLines(@"./HoughCircles_Profile.txt").Count();
            HoughCircles_Profile.Items.Clear();
            for (int i = 0; i < (lineCount / 2); i++)
            {

                HoughCircles_Profile.Items.Add("Profile " + i);

            }

        }

        private void HoughCircles_Profile_SelectedIndexChanged(object sender, EventArgs e)
        {
            houghCircles_Status = true;
            int lineCount = File.ReadLines(@"./HoughCircles_Profile.txt").Count();
            string[] lines = File.ReadAllLines(@"./HoughCircles_Profile.txt", Encoding.UTF8);


            for (int i = 0; i < lineCount; i++)
            {
                if (HoughCircles_Profile.Text == "Profile 0")
                {

                    cannyThreshold_Box.Text = lines[0];


                    circleAccumulatorThreshold_Box.Text = lines[1];

                }

                else if (HoughCircles_Profile.Text == "Profile " + i && HoughCircles_Profile.Text != "Profile 0")
                {
                    cannyThreshold_Box.Text = lines[i * 2];


                    circleAccumulatorThreshold_Box.Text = lines[i * 2 + 1];
                }


            }

            int cameraWidth = Int16.Parse(ROITextWidth.Text);
            int cameraHeight = Int16.Parse(ROITextHeight.Text);

            StringBuilder msgBuilder = new StringBuilder("Performance: ");

            //Load the image from file and resize it for display
            Image<Bgr, Byte> img = ROIFrame.Resize(cameraWidth / 10, cameraHeight / 10, Emgu.CV.CvEnum.Inter.Linear, true);

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

            cannyThreshold = double.Parse(cannyThreshold_Box.Text);
            circleAccumulatorThreshold = double.Parse(circleAccumulatorThreshold_Box.Text);
            circleAccumulatorThreshold_Box.Text = circleAccumulatorThreshold.ToString();
            CircleF[] circles = CvInvoke.HoughCircles(uimage, HoughType.Gradient, 1.5, 27.0, cannyThreshold, circleAccumulatorThreshold, 5);
            System.Diagnostics.Debug.WriteLine(circles);
            watch.Stop();
            msgBuilder.Append(String.Format("Hough circles - {0} ms; ", watch.ElapsedMilliseconds));
            #endregion

            #region draw circles
            circleImage = img;
            foreach (CircleF circle in circles)
            {
                circleImage.Draw(circle, new Bgr(Color.Red), 2);
                houghCircles_X = (int)circle.Center.X;
                houghCircles_Y = (int)circle.Center.Y;
                houghCirclesradius = (int)circle.Radius;




            }

            HoughCircles.Image = circleImage;
            #endregion
            houghCircles_Status = false;
        }

        private void Clear_Profile_Click(object sender, EventArgs e)
        {
            File.WriteAllText(@"./HoughCircles_Profile.txt", String.Empty);
            HoughCircles_Profile.Items.Clear();
        }


        public void Exposure_Adjust()
        {
            if (CameraStateText.Text != "ASI_EXP_WORKING"
                && recover != false
                && colorValue >= minLight
                && AppSetting.Data.ExposureTime < 240000
                && AppSetting.Data.ExposureTime > 1)
            {
                if (AppSetting.Data.ExposureTime == 240000 && CameraStateText.Text != "ASI_EXP_WORKING" && recover != false)
                {
                    AppSetting.Data.ExposureTime = 120000;
                    recover = false;
                }
                else if (AppSetting.Data.ExposureTime >= 120000 && AppSetting.Data.ExposureTime < 240000 && recover != false)
                {
                    AppSetting.Data.ExposureTime = 60000;
                    recover = false;
                }
                else if (AppSetting.Data.ExposureTime >= 60000 && AppSetting.Data.ExposureTime < 120000 && recover != false)
                {
                    AppSetting.Data.ExposureTime = 30000;
                    recover = false;
                }

                else if (AppSetting.Data.ExposureTime >= 30000 && AppSetting.Data.ExposureTime < 60000 && recover != false)
                {
                    AppSetting.Data.ExposureTime = 15000;
                    recover = false;
                }

                else if (AppSetting.Data.ExposureTime >= 15000 && AppSetting.Data.ExposureTime < 30000 && recover != false)
                {
                    AppSetting.Data.ExposureTime = 8000;
                    recover = false;
                }

                else if (AppSetting.Data.ExposureTime >= 8000 && AppSetting.Data.ExposureTime < 15000 && recover != false)
                {
                    AppSetting.Data.ExposureTime = 4000;
                    recover = false;
                }


                else if (AppSetting.Data.ExposureTime >= 4000 && AppSetting.Data.ExposureTime < 8000 && recover != false)
                {
                    AppSetting.Data.ExposureTime = 2000;
                    recover = false;
                }


                else if (AppSetting.Data.ExposureTime >= 2000 && AppSetting.Data.ExposureTime < 4000 && recover != false)
                {
                    AppSetting.Data.ExposureTime = 1000;
                    recover = false;
                }


                else if (AppSetting.Data.ExposureTime >= 1000 && AppSetting.Data.ExposureTime < 2000 && recover != false)
                {
                    AppSetting.Data.ExposureTime = 500;
                    recover = false;
                }


                else if (AppSetting.Data.ExposureTime >= 500 && AppSetting.Data.ExposureTime < 1000 && recover != false)
                {
                    AppSetting.Data.ExposureTime = 250;
                    recover = false;
                }


                else if (AppSetting.Data.ExposureTime >= 250 && AppSetting.Data.ExposureTime < 500 && recover != false)
                {
                    AppSetting.Data.ExposureTime = 125;
                    recover = false;
                }


                else if (AppSetting.Data.ExposureTime >= 125 && AppSetting.Data.ExposureTime < 250 && recover != false)
                {
                    AppSetting.Data.ExposureTime = 66;
                    recover = false;
                }


                else if (AppSetting.Data.ExposureTime >= 66 && AppSetting.Data.ExposureTime < 125 && recover != false)
                {
                    AppSetting.Data.ExposureTime = 33;
                    recover = false;
                }


                else if (AppSetting.Data.ExposureTime >= 33 && AppSetting.Data.ExposureTime < 66 && recover != false)
                {
                    AppSetting.Data.ExposureTime = 16;
                    recover = false;
                }

                else if (AppSetting.Data.ExposureTime >= 16 && AppSetting.Data.ExposureTime < 33 && recover != false)
                {
                    AppSetting.Data.ExposureTime = 8;
                    recover = false;
                }
                else if (AppSetting.Data.ExposureTime >= 8 && AppSetting.Data.ExposureTime < 16 && recover != false)
                {
                    AppSetting.Data.ExposureTime = 4;
                    recover = false;
                }
                else if (AppSetting.Data.ExposureTime >= 4 && AppSetting.Data.ExposureTime < 8 && recover != false)
                {
                    AppSetting.Data.ExposureTime = 2;
                    recover = false;
                }
                else if (AppSetting.Data.ExposureTime >= 2 && AppSetting.Data.ExposureTime < 4 && recover != false)
                {
                    AppSetting.Data.ExposureTime = 1;
                    recover = false;
                }
                else if (AppSetting.Data.ExposureTime >= 1 && AppSetting.Data.ExposureTime < 2 && recover != false)
                {
                    AppSetting.Data.ExposureTime = 0.5;
                    recover = false;
                }

            }

            else if (CameraStateText.Text != "ASI_EXP_WORKING"
                && recover != false
                && colorValue <= minLight
                && AppSetting.Data.ExposureTime > 1
                && AppSetting.Data.ExposureTime < 240000)
            {

                if (AppSetting.Data.ExposureTime == 120000 && CameraStateText.Text != "ASI_EXP_WORKING" && recover != false)
                {
                    AppSetting.Data.ExposureTime = 240000;
                    recover = false;
                }
                else if (AppSetting.Data.ExposureTime <= 60000 && AppSetting.Data.ExposureTime > 30000 && recover != false)
                {
                    AppSetting.Data.ExposureTime = 120000;
                    recover = false;
                }
                else if (AppSetting.Data.ExposureTime <= 30000 && AppSetting.Data.ExposureTime > 15000 && recover != false)
                {
                    AppSetting.Data.ExposureTime = 60000;
                    recover = false;
                }
                else if (AppSetting.Data.ExposureTime <= 15000 && AppSetting.Data.ExposureTime > 8000 && recover != false)
                {
                    AppSetting.Data.ExposureTime = 30000;
                    recover = false;
                }
                else if (AppSetting.Data.ExposureTime <= 8000 && AppSetting.Data.ExposureTime > 4000 && recover != false)
                {
                    AppSetting.Data.ExposureTime = 15000;
                    recover = false;
                }

                else if (AppSetting.Data.ExposureTime <= 4000 && AppSetting.Data.ExposureTime > 2000 && recover != false)
                {
                    AppSetting.Data.ExposureTime = 8000;
                    recover = false;
                }


                else if (AppSetting.Data.ExposureTime <= 2000 && AppSetting.Data.ExposureTime > 1000 && recover != false)
                {
                    AppSetting.Data.ExposureTime = 4000;
                    recover = false;
                }


                else if (AppSetting.Data.ExposureTime <= 1000 && AppSetting.Data.ExposureTime > 500 && recover != false)
                {
                    AppSetting.Data.ExposureTime = 2000;
                    recover = false;
                }


                else if (AppSetting.Data.ExposureTime <= 500 && AppSetting.Data.ExposureTime > 250 && recover != false)
                {
                    AppSetting.Data.ExposureTime = 1000;
                    recover = false;
                }


                else if (AppSetting.Data.ExposureTime <= 250 && AppSetting.Data.ExposureTime > 125 && recover != false)
                {
                    AppSetting.Data.ExposureTime = 500;
                    recover = false;
                }


                else if (AppSetting.Data.ExposureTime <= 125 && AppSetting.Data.ExposureTime > 66 && recover != false)
                {
                    AppSetting.Data.ExposureTime = 250;
                    recover = false;
                }


                else if (AppSetting.Data.ExposureTime <= 66 && AppSetting.Data.ExposureTime > 33 && recover != false)
                {
                    AppSetting.Data.ExposureTime = 125;
                    recover = false;
                }


                else if (AppSetting.Data.ExposureTime <= 33 && AppSetting.Data.ExposureTime > 16 && recover != false)
                {
                    AppSetting.Data.ExposureTime = 66;
                    recover = false;
                }


                else if (AppSetting.Data.ExposureTime <= 16 && AppSetting.Data.ExposureTime > 8 && recover != false)
                {
                    AppSetting.Data.ExposureTime = 33;
                    recover = false;
                }

                else if (AppSetting.Data.ExposureTime <= 8 && AppSetting.Data.ExposureTime > 4 && recover != false)
                {
                    AppSetting.Data.ExposureTime = 16;
                    recover = false;
                }
                else if (AppSetting.Data.ExposureTime <= 4 && AppSetting.Data.ExposureTime > 2 && recover != false)
                {
                    AppSetting.Data.ExposureTime = 8;
                    recover = false;
                }
                else if (AppSetting.Data.ExposureTime <= 2 && AppSetting.Data.ExposureTime > 1 && recover != false)
                {
                    AppSetting.Data.ExposureTime = 4;
                    recover = false;
                }
                else if (AppSetting.Data.ExposureTime <= 1 && AppSetting.Data.ExposureTime > 0.5 && recover != false)
                {
                    AppSetting.Data.ExposureTime = 2;
                    recover = false;
                }
                else if (AppSetting.Data.ExposureTime <= 0.5 && recover != false)
                {
                    AppSetting.Data.ExposureTime = 1;
                    recover = false;
                }
            }
        }

        private void UITimer_Tick(object sender, EventArgs e)
        {
            if (CameraStateText.Text == "ASI_EXP_FAILED")
            {
                cameraLost = true;
                if (ConnectedCameras > 0)
                {
                    ConnectedCameras = ASICameraDll2.ASIGetNumOfConnectedCameras();

                }
                if (ConnectedCameras > 0)
                {
                    SystemMessage = "Retring to connect to the camera.";
                    Application.Restart();
                }

                SystemMessage = "No camera connected";
                MainImageControl.Image = new Image<Bgr, byte>(AppDomain.CurrentDomain.BaseDirectory + "TAllSyk.jpg");
                ROIImage.Image = new Image<Bgr, byte>(AppDomain.CurrentDomain.BaseDirectory + "Nocameraconnected.jpg");
                GC.Collect();
            }
            else
            {
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

                    int cameraWidth;
                    int cameraHeight;
                    if (houghCircles_X > 0 && houghCircles_Y > 0)
                    {
                        cameraWidth = (houghCircles_X * 10) * 2;
                        cameraHeight = (houghCircles_Y * 10) * 2;
                    }
                    else
                    {
                        cameraWidth = Int16.Parse(ROITextWidth.Text);
                        cameraHeight = Int16.Parse(ROITextHeight.Text);
                    }

                    if (hdrOn == false && hdr_On.Checked == false)
                    {
                        //ProcessFrame._EqualizeHist();
                        MainImageControl.Image = ProcessFrame;
                    }

                    string timesStamp = DateTime.Now.ToUniversalTime().ToString("MM/dd/yyyy hh:mm:ss tt");
                    int thickness = 5;
                    int borderHeight = Int32.Parse(AppSetting.Data.ImageHeight.ToString());

                    int borderWidth = Int32.Parse(AppSetting.Data.ImageWidth.ToString());
                    float exposureTime_Show = float.Parse(AppSetting.Data.ExposureTime.ToString());

                    imageFrame = ProcessFrame;
                    timeNow = DateTime.Now.ToString("yyyy_MM_dd__HH_mm_ss");
                    timeFolder = DateTime.Now.ToString("yyyy-MM-dd");
                    timeNow_Chack = DateTime.Now.ToString("yyyy_MM_dd__HH_mm");
                    borderHeight = borderHeight - 300;
                    var borderTime = new Rectangle(0, borderHeight, 800, 100);
                    imageFrame.Draw(borderTime, new Bgr(Color.Black), -1);
                    imageFrame.Draw(borderTime, new Bgr(Color.White), 2);
                    CvInvoke.PutText(imageFrame, "UTC " + timesStamp, new Point(0, borderHeight + 50), FontFace.HersheySimplex, 1.5, new Bgr(Color.White).MCvScalar, thickness);
                    borderWidth = borderWidth - 600;
                    var borderExposureTime = new Rectangle(borderWidth, borderHeight, 300, 100);
                    imageFrame.Draw(borderExposureTime, new Bgr(Color.Black), -1);
                    imageFrame.Draw(borderExposureTime, new Bgr(Color.White), 2);
                    if (exposureTime_Show < 1000 && exposureTime_Show > 1)
                    {
                        CvInvoke.PutText(imageFrame, Math.Round(AppSetting.Data.ExposureTime, 0) + " ms", new Point(borderWidth + 50, borderHeight + 50), FontFace.HersheySimplex, 1.5, new Bgr(Color.White).MCvScalar, thickness);
                    }
                    else if (exposureTime_Show >= 1000)
                    {
                        CvInvoke.PutText(imageFrame, Math.Round(AppSetting.Data.ExposureTime / 1000, 0) + " sec", new Point(borderWidth + 50, borderHeight + 50), FontFace.HersheySimplex, 1.5, new Bgr(Color.White).MCvScalar, thickness);
                    }
                    else if (exposureTime_Show >= 0 && exposureTime_Show <= 1)
                    {
                        CvInvoke.PutText(imageFrame, Math.Round(AppSetting.Data.ExposureTime, 2) + " ms", new Point(borderWidth + 50, borderHeight + 50), FontFace.HersheySimplex, 1.5, new Bgr(Color.White).MCvScalar, thickness);
                    }

                    Bitmap bmpImage_Frame = imageFrame.ToBitmap();
                    if (IsAutoExposureTime.CheckState != 0) //AutoExposureTime 
                    {

                        if (checkBoxCenter.CheckState != 0) //checkBoxFocus
                        {
                            checkBoxAverage.Checked = false;
                            centroidX = (int)(cameraWidth / 2);
                            centroidY = (int)(cameraHeight / 2);
                            for (double i = centroidX - 10; i < centroidX; i++)
                            {
                                for (double j = centroidY - 10; j < centroidY; j++)
                                {
                                    Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                                    colorValue += (pixel.R + pixel.B + pixel.G) / 3;
                                }
                            }
                            colorValue = (colorValue / 100);
                            if (ShowFocusPoint.CheckState != 0)
                            {
                                CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidX.ToString()), Int32.Parse(centroidY.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);
                            }
                        }
                        else
                        {

                            if (FocusPoint.Text == "21 Focus Points" && houghCircles_Status == false)
                            {
                                checkBoxCenter.Checked = false;
                                checkBoxAverage.Checked = true;
                                //=======================================Center Line======================================== 
                                centroidX = (int)(cameraWidth / 2);
                                centroidY = (int)(cameraHeight / 2);
                                //left 
                                double centroidXleft = (int)(cameraWidth / 2.5);
                                double centroidYleft = (int)(cameraHeight / 2);

                                //left 
                                double centroidXleftmore = (int)(cameraWidth / 3.2);
                                double centroidYleftmore = (int)(cameraHeight / 2);

                                //right
                                double centroidXright = (int)(cameraWidth / 1.7);
                                double centroidYright = (int)(cameraHeight / 2);

                                //right
                                double centroidXrightmore = (int)(cameraWidth / 1.5);
                                double centroidYrightmore = (int)(cameraHeight / 2);

                                //=========================================UP Line======================================

                                double centroidX_UP = (int)(cameraWidth / 2);
                                double centroidY_UP = (int)(cameraHeight / 3);

                                //left 
                                double centroidXleft_UP = (int)(cameraWidth / 2.5);
                                double centroidYleft_UP = (int)(cameraHeight / 3);

                                //left 
                                double centroidXleftmore_UP = (int)(cameraWidth / 3.2);
                                double centroidYleftmore_UP = (int)(cameraHeight / 3);

                                //right
                                double centroidXright_UP = (int)(cameraWidth / 1.7);
                                double centroidYright_UP = (int)(cameraHeight / 3);

                                //right
                                double centroidXrightmore_UP = (int)(cameraWidth / 1.5);
                                double centroidYrightmore_UP = (int)(cameraHeight / 3);

                                //=======================================DOWN Line========================================
                                double centroidX_DOWN = (int)(cameraWidth / 2);
                                double centroidY_DOWN = (int)(cameraHeight / 1.5);

                                //left 
                                double centroidXleft_DOWN = (int)(cameraWidth / 2.5);
                                double centroidYleft_DOWN = (int)(cameraHeight / 1.5);

                                //left 
                                double centroidXleftmore_DOWN = (int)(cameraWidth / 3.2);
                                double centroidYleftmore_DOWN = (int)(cameraHeight / 1.5);

                                //right
                                double centroidXright_DOWN = (int)(cameraWidth / 1.7);
                                double centroidYright_DOWN = (int)(cameraHeight / 1.5);

                                //right
                                double centroidXrightmore_DOWN = (int)(cameraWidth / 1.5);
                                double centroidYrightmore_DOWN = (int)(cameraHeight / 1.5);

                                //=========================================UP 3 DOT======================================

                                double centroidX_UP2 = (int)(cameraWidth / 2);
                                double centroidY_UP2 = (int)(cameraHeight / 6);

                                //right
                                double centroidXright_UP2 = (int)(cameraWidth / 1.7);
                                double centroidYright_UP2 = (int)(cameraHeight / 6);

                                //left 
                                double centroidXleft_UP2 = (int)(cameraWidth / 2.5);
                                double centroidYleft_UP2 = (int)(cameraHeight / 6);



                                //=======================================DOWN 3 DOT========================================
                                double centroidX_DOWN2 = (int)(cameraWidth / 2);
                                double centroidY_DOWN2 = (int)(cameraHeight / 1.2);

                                //right
                                double centroidXright_DOWN2 = (int)(cameraWidth / 1.7);
                                double centroidYright_DOWN2 = (int)(cameraHeight / 1.2);

                                //left 
                                double centroidXleft_DOWN2 = (int)(cameraWidth / 2.5);
                                double centroidYleft_DOWN2 = (int)(cameraHeight / 1.2);

                                if (ShowFocusPoint.CheckState != 0)
                                {
                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidX.ToString()), Int32.Parse(centroidY.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);
                                    //left 
                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXleft.ToString()), Int32.Parse(centroidYleft.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);
                                    //left 
                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXleftmore.ToString()), Int32.Parse(centroidYleftmore.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);
                                    //right
                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXright.ToString()), Int32.Parse(centroidYright.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);
                                    //right
                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXrightmore.ToString()), Int32.Parse(centroidYrightmore.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);
                                    //=========================================UP======================================
                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidX_UP.ToString()), Int32.Parse(centroidY_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);
                                    //left 
                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXleft_UP.ToString()), Int32.Parse(centroidYleft_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);
                                    //left 
                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXleftmore_UP.ToString()), Int32.Parse(centroidYleftmore_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);
                                    //right
                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXright_UP.ToString()), Int32.Parse(centroidYright_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);
                                    //right
                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXrightmore_UP.ToString()), Int32.Parse(centroidYrightmore_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);
                                    //=======================================DOWN========================================
                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidX_DOWN.ToString()), Int32.Parse(centroidY_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);
                                    //left 
                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXleft_DOWN.ToString()), Int32.Parse(centroidYleft_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);
                                    //left 
                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXleftmore_DOWN.ToString()), Int32.Parse(centroidYleftmore_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);
                                    //right
                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXright_DOWN.ToString()), Int32.Parse(centroidYright_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);
                                    //right
                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXrightmore_DOWN.ToString()), Int32.Parse(centroidYrightmore_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);
                                    //=========================================UP 3 DOT======================================
                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidX_UP2.ToString()), Int32.Parse(centroidY_UP2.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);
                                    //right
                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXright_UP2.ToString()), Int32.Parse(centroidYright_UP2.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);
                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXleft_UP2.ToString()), Int32.Parse(centroidYleft_UP2.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);
                                    //=======================================DOWN 3 DOT========================================
                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidX_DOWN2.ToString()), Int32.Parse(centroidY_DOWN2.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);
                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXleft_DOWN2.ToString()), Int32.Parse(centroidYleft_DOWN2.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);
                                    //right
                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXright_DOWN2.ToString()), Int32.Parse(centroidYright_DOWN2.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);
                                }

                                //==================Readlight Center
                                for (double i = centroidX - 10; i < centroidX; i++)
                                {
                                    for (double j = centroidY - 10; j < centroidY; j++)

                                    {
                                        if (centroidX > centroidY)
                                        {

                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;
                                        }
                                    }
                                }

                                for (double i = centroidXleftmore - 10; i < centroidXleftmore; i++)
                                {
                                    for (double j = centroidYleftmore - 10; j < centroidYleftmore; j++)
                                    {
                                        if (centroidXleftmore > centroidYleftmore)
                                        {
                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;
                                        }
                                    }
                                }

                                for (double i = centroidXleft - 10; i < centroidXleft; i++)
                                {
                                    for (double j = centroidYleft - 10; j < centroidYleft; j++)
                                    {
                                        if (centroidXleft > centroidYleft)
                                        {
                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));

                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;
                                        }
                                    }
                                }

                                for (double i = centroidXright - 10; i < centroidXright; i++)
                                {
                                    for (double j = centroidYright - 10; j < centroidYright; j++)
                                    {
                                        if (centroidXright > centroidYright)
                                        {
                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;
                                        }
                                    }
                                }

                                for (double i = centroidXrightmore - 10; i < centroidXrightmore; i++)
                                {
                                    for (double j = centroidYrightmore - 10; j < centroidYrightmore; j++)
                                    {
                                        if (centroidXrightmore > centroidYrightmore)
                                        {
                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;

                                        }
                                    }
                                }

                                //==================Readlight UP
                                for (double i = centroidX_UP - 10; i < centroidX_UP; i++)
                                {
                                    for (double j = centroidY_UP - 10; j < centroidY_UP; j++)
                                    {
                                        if (centroidX_UP > centroidY_UP)
                                        {
                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));

                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;
                                        }
                                    }
                                }

                                for (double i = centroidXleft_UP - 10; i < centroidXleft_UP; i++)
                                {
                                    for (double j = centroidYleft_UP - 10; j < centroidYleft_UP; j++)
                                    {
                                        if (centroidXleft_UP > centroidYleft_UP)
                                        {
                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;
                                        }
                                    }
                                }

                                for (double i = centroidXleftmore_UP - 10; i < centroidXleftmore_UP; i++)
                                {
                                    for (double j = centroidYleftmore_UP - 10; j < centroidYleftmore_UP; j++)
                                    {
                                        if (centroidXleftmore_UP > centroidYleftmore_UP)
                                        {
                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;

                                        }
                                    }
                                }



                                for (double i = centroidXright_UP - 10; i < centroidXright_UP; i++)
                                {

                                    for (double j = centroidYright_UP - 10; j < centroidYright_UP; j++)

                                    {
                                        if (centroidXright_UP > centroidYright_UP)
                                        {
                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));

                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;

                                        }

                                    }

                                }

                                for (double i = centroidXrightmore_UP - 10; i < centroidXrightmore_UP; i++)
                                {

                                    for (double j = centroidYrightmore_UP - 10; j < centroidYrightmore_UP; j++)

                                    {
                                        if (centroidXrightmore_UP > centroidYrightmore_UP)
                                        {
                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));

                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;

                                        }

                                    }

                                }
                                //==================Readlight Down
                                for (double i = centroidX_DOWN - 10; i < centroidX_DOWN; i++)
                                {

                                    for (double j = centroidY_DOWN - 10; j < centroidY_DOWN; j++)

                                    {
                                        if (centroidX_DOWN > centroidY_DOWN)
                                        {
                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;

                                        }

                                    }

                                }

                                for (double i = centroidXleft_DOWN - 10; i < centroidXleft_DOWN; i++)
                                {

                                    for (double j = centroidYleft_DOWN - 10; j < centroidYleft_DOWN; j++)

                                    {




                                        if (centroidXleft_DOWN > centroidYleft_DOWN)
                                        {
                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;

                                        }

                                    }

                                }

                                for (double i = centroidXleftmore_DOWN - 10; i < centroidXleftmore_DOWN; i++)
                                {

                                    for (double j = centroidYleftmore_DOWN - 10; j < centroidYleftmore_DOWN; j++)

                                    {
                                        if (centroidXleftmore_DOWN > centroidYleftmore_DOWN)
                                        {
                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));

                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;

                                        }

                                    }

                                }


                                for (double i = centroidXright_DOWN - 10; i < centroidXright_DOWN; i++)
                                {

                                    for (double j = centroidYright_DOWN - 10; j < centroidYright_DOWN; j++)

                                    {
                                        if (centroidXright_DOWN > centroidYright_DOWN)
                                        {
                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;

                                        }

                                    }

                                }

                                for (double i = centroidXrightmore_DOWN - 10; i < centroidXrightmore_DOWN; i++)
                                {

                                    for (double j = centroidYrightmore_DOWN - 10; j < centroidYrightmore_DOWN; j++)

                                    {
                                        if (centroidXrightmore_DOWN > centroidYrightmore_DOWN)
                                        {
                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));

                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;

                                        }

                                    }

                                }

                                //==================Readlight UP DOT
                                for (double i = centroidX_UP2 - 10; i < centroidX_UP2; i++)
                                {

                                    for (double j = centroidY_UP2 - 10; j < centroidY_UP2; j++)

                                    {
                                        if (centroidX_UP2 > centroidY_UP2)
                                        {
                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));

                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;

                                        }

                                    }

                                }

                                for (double i = centroidXright_UP2 - 10; i < centroidXright_UP2; i++)
                                {

                                    for (double j = centroidYright_UP2 - 10; j < centroidYright_UP2; j++)

                                    {
                                        if (centroidXright_UP2 > centroidYright_UP2)
                                        {
                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));

                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;
                                        }

                                    }

                                }

                                for (double i = centroidXleft_UP2 - 10; i < centroidXleft_UP2; i++)
                                {

                                    for (double j = centroidYleft_UP2 - 10; j < centroidYleft_UP2; j++)

                                    {
                                        if (centroidXleft_UP2 > centroidYleft_UP2)
                                        {
                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));

                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;

                                        }

                                    }

                                }

                                //==================Readlight DOWN DOT

                                for (double i = centroidX_DOWN2 - 10; i < centroidX_DOWN2; i++)
                                {

                                    for (double j = centroidY_DOWN2 - 10; j < centroidY_DOWN2; j++)

                                    {
                                        if (centroidX_DOWN2 > centroidY_DOWN2)
                                        {
                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));

                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;
                                        }


                                    }

                                }

                                for (double i = centroidXright_DOWN2 - 10; i < centroidXright_DOWN2; i++)
                                {

                                    for (double j = centroidYright_DOWN2 - 10; j < centroidYright_DOWN2; j++)

                                    {
                                        if (centroidXright_DOWN2 > centroidYright_DOWN2)
                                        {
                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;

                                        }

                                    }

                                }

                                for (double i = centroidXleft_DOWN2 - 10; i < centroidXleft_DOWN2; i++)
                                {

                                    for (double j = centroidYleft_DOWN2 - 10; j < centroidYleft_DOWN2; j++)

                                    {
                                        if (centroidXleft_DOWN2 > centroidYleft_DOWN2)
                                        {
                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;

                                        }

                                    }

                                }
                                colorValue = (colorValue / 2100);
                                //recover = false;
                            }
                            else if (FocusPoint.Text == "15 Focus Points")
                            {
                                checkBoxCenter.Checked = false;
                                checkBoxAverage.Checked = true;
                                //=======================================Center======================================== 
                                centroidX = (int)(cameraWidth / 2);
                                centroidY = (int)(cameraHeight / 2);
                                //CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidX.ToString()), Int32.Parse(centroidY.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                //left 
                                double centroidXleft = (int)(cameraWidth / 2.5);
                                double centroidYleft = (int)(cameraHeight / 2);
                                //CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXleft.ToString()), Int32.Parse(centroidYleft.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                //left 
                                double centroidXleftmore = (int)(cameraWidth / 3.2);
                                double centroidYleftmore = (int)(cameraHeight / 2);


                                //right
                                double centroidXright = (int)(cameraWidth / 1.7);
                                double centroidYright = (int)(cameraHeight / 2);
                                //CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXright.ToString()), Int32.Parse(centroidYright.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                //right
                                double centroidXrightmore = (int)(cameraWidth / 1.5);
                                double centroidYrightmore = (int)(cameraHeight / 2);
                                //CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXrightmore.ToString()), Int32.Parse(centroidYrightmore.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                //=========================================UP======================================

                                double centroidX_UP = (int)(cameraWidth / 2);
                                double centroidY_UP = (int)(cameraHeight / 3);
                                //CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidX_UP.ToString()), Int32.Parse(centroidY_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                //left 
                                double centroidXleft_UP = (int)(cameraWidth / 2.5);
                                double centroidYleft_UP = (int)(cameraHeight / 3);
                                //CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXleft_UP.ToString()), Int32.Parse(centroidYleft_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                //left 
                                double centroidXleftmore_UP = (int)(cameraWidth / 3.2);
                                double centroidYleftmore_UP = (int)(cameraHeight / 3);

                                //right
                                double centroidXright_UP = (int)(cameraWidth / 1.7);
                                double centroidYright_UP = (int)(cameraHeight / 3);
                                //CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXright_UP.ToString()), Int32.Parse(centroidYright_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                //right
                                double centroidXrightmore_UP = (int)(cameraWidth / 1.5);
                                double centroidYrightmore_UP = (int)(cameraHeight / 3);
                                //CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXrightmore_UP.ToString()), Int32.Parse(centroidYrightmore_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                //=======================================DOWN========================================
                                double centroidX_DOWN = (int)(cameraWidth / 2);
                                double centroidY_DOWN = (int)(cameraHeight / 1.5);
                                //CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidX_DOWN.ToString()), Int32.Parse(centroidY_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                //left 
                                double centroidXleft_DOWN = (int)(cameraWidth / 2.5);
                                double centroidYleft_DOWN = (int)(cameraHeight / 1.5);
                                //CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXleft_DOWN.ToString()), Int32.Parse(centroidYleft_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                //left 
                                double centroidXleftmore_DOWN = (int)(cameraWidth / 3.2);
                                double centroidYleftmore_DOWN = (int)(cameraHeight / 1.5);

                                //right
                                double centroidXright_DOWN = (int)(cameraWidth / 1.7);
                                double centroidYright_DOWN = (int)(cameraHeight / 1.5);
                                //CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXright_DOWN.ToString()), Int32.Parse(centroidYright_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                //right
                                double centroidXrightmore_DOWN = (int)(cameraWidth / 1.5);
                                double centroidYrightmore_DOWN = (int)(cameraHeight / 1.5);
                                //CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXrightmore_DOWN.ToString()), Int32.Parse(centroidYrightmore_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);




                                if (ShowFocusPoint.CheckState != 0)
                                {


                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidX.ToString()), Int32.Parse(centroidY.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //left 

                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXleft.ToString()), Int32.Parse(centroidYleft.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //left 

                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXleftmore.ToString()), Int32.Parse(centroidYleftmore.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //left 

                                    //CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXleftmore2.ToString()), Int32.Parse(centroidYleftmore2.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //right

                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXright.ToString()), Int32.Parse(centroidYright.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //right

                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXrightmore.ToString()), Int32.Parse(centroidYrightmore.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //=========================================UP======================================


                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidX_UP.ToString()), Int32.Parse(centroidY_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //left 

                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXleft_UP.ToString()), Int32.Parse(centroidYleft_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //left 

                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXleftmore_UP.ToString()), Int32.Parse(centroidYleftmore_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //right

                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXright_UP.ToString()), Int32.Parse(centroidYright_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //right

                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXrightmore_UP.ToString()), Int32.Parse(centroidYrightmore_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //=======================================DOWN========================================

                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidX_DOWN.ToString()), Int32.Parse(centroidY_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //left 

                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXleft_DOWN.ToString()), Int32.Parse(centroidYleft_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //left 

                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXleftmore_DOWN.ToString()), Int32.Parse(centroidYleftmore_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //right

                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXright_DOWN.ToString()), Int32.Parse(centroidYright_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //right

                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXrightmore_DOWN.ToString()), Int32.Parse(centroidYrightmore_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);




                                }

                                //==================Readlight Center
                                for (double i = centroidX - 10; i < centroidX; i++)
                                {

                                    for (double j = centroidY - 10; j < centroidY; j++)

                                    {
                                        if (centroidX > centroidY)
                                        {

                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;
                                        }


                                    }

                                }

                                for (double i = centroidXleftmore - 10; i < centroidXleftmore; i++)
                                {

                                    for (double j = centroidYleftmore - 10; j < centroidYleftmore; j++)

                                    {
                                        if (centroidXleftmore > centroidYleftmore)
                                        {

                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;

                                        }


                                    }

                                }

                                for (double i = centroidXleft - 10; i < centroidXleft; i++)
                                {

                                    for (double j = centroidYleft - 10; j < centroidYleft; j++)

                                    {
                                        if (centroidXleft > centroidYleft)
                                        {
                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;

                                        }

                                    }

                                }



                                for (double i = centroidXright - 10; i < centroidXright; i++)
                                {

                                    for (double j = centroidYright - 10; j < centroidYright; j++)

                                    {
                                        if (centroidXright > centroidYright)
                                        {

                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;
                                        }


                                    }

                                }

                                for (double i = centroidXrightmore - 10; i < centroidXrightmore; i++)
                                {

                                    for (double j = centroidYrightmore - 10; j < centroidYrightmore; j++)

                                    {
                                        if (centroidXrightmore > centroidYrightmore)
                                        {
                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;
                                        }


                                    }

                                }

                                //==================Readlight UP
                                for (double i = centroidX_UP - 10; i < centroidX_UP; i++)
                                {

                                    for (double j = centroidY_UP - 10; j < centroidY_UP; j++)

                                    {
                                        if (centroidX_UP > centroidY_UP)
                                        {
                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;
                                        }


                                    }

                                }

                                for (double i = centroidXleft_UP - 10; i < centroidXleft_UP; i++)
                                {

                                    for (double j = centroidYleft_UP - 10; j < centroidYleft_UP; j++)

                                    {
                                        if (centroidXleft_UP > centroidYleft_UP)
                                        {
                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;
                                        }


                                    }

                                }

                                for (double i = centroidXleftmore_UP - 10; i < centroidXleftmore_UP; i++)
                                {

                                    for (double j = centroidYleftmore_UP - 10; j < centroidYleftmore_UP; j++)

                                    {
                                        if (centroidXleftmore_UP > centroidYleftmore_UP)
                                        {
                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;
                                        }


                                    }

                                }



                                for (double i = centroidXright_UP - 10; i < centroidXright_UP; i++)
                                {

                                    for (double j = centroidYright_UP - 10; j < centroidYright_UP; j++)

                                    {
                                        if (centroidXright_UP > centroidYright_UP)
                                        {
                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;

                                        }

                                    }

                                }

                                for (double i = centroidXrightmore_UP - 10; i < centroidXrightmore_UP; i++)
                                {

                                    for (double j = centroidYrightmore_UP - 10; j < centroidYrightmore_UP; j++)

                                    {
                                        if (centroidXrightmore_UP > centroidYrightmore_UP)
                                        {

                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;
                                        }


                                    }

                                }
                                //==================Readlight Down
                                for (double i = centroidX_DOWN - 10; i < centroidX_DOWN; i++)
                                {

                                    for (double j = centroidY_DOWN - 10; j < centroidY_DOWN; j++)

                                    {
                                        if (centroidX_DOWN > centroidY_DOWN)
                                        {
                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;
                                        }


                                    }

                                }

                                for (double i = centroidXleft_DOWN - 10; i < centroidXleft_DOWN; i++)
                                {

                                    for (double j = centroidYleft_DOWN - 10; j < centroidYleft_DOWN; j++)

                                    {
                                        if (centroidXleft_DOWN > centroidYleft_DOWN)
                                        {
                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;
                                        }


                                    }

                                }

                                for (double i = centroidXleftmore_DOWN - 10; i < centroidXleftmore_DOWN; i++)
                                {

                                    for (double j = centroidYleftmore_DOWN - 10; j < centroidYleftmore_DOWN; j++)

                                    {
                                        if (centroidXleftmore_DOWN > centroidYleftmore_DOWN)
                                        {
                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;
                                        }


                                    }

                                }


                                for (double i = centroidXright_DOWN - 10; i < centroidXright_DOWN; i++)
                                {

                                    for (double j = centroidYright_DOWN - 10; j < centroidYright_DOWN; j++)

                                    {
                                        if (centroidXright_DOWN > centroidYright_DOWN)
                                        {
                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;
                                        }


                                    }

                                }

                                for (double i = centroidXrightmore_DOWN - 10; i < centroidXrightmore_DOWN; i++)
                                {

                                    for (double j = centroidYrightmore_DOWN - 10; j < centroidYrightmore_DOWN; j++)

                                    {
                                        if (centroidXrightmore_DOWN > centroidYrightmore_DOWN)
                                        {
                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;
                                        }


                                    }

                                }


                                colorValue = (colorValue / 1500);
                            }
                            else if (FocusPoint.Text == "9 Focus Points")
                            {
                                checkBoxCenter.Checked = false;
                                checkBoxAverage.Checked = true;
                                //=======================================Center======================================== 
                                centroidX = (int)(cameraWidth / 2);
                                centroidY = (int)(cameraHeight / 2);
                                //CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidX.ToString()), Int32.Parse(centroidY.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);


                                //left 
                                double centroidXleftmore = (int)(cameraWidth / 3.2);
                                double centroidYleftmore = (int)(cameraHeight / 2);



                                //right
                                double centroidXrightmore = (int)(cameraWidth / 1.5);
                                double centroidYrightmore = (int)(cameraHeight / 2);
                                //CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXrightmore.ToString()), Int32.Parse(centroidYrightmore.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                //=========================================UP======================================

                                double centroidX_UP = (int)(cameraWidth / 2);
                                double centroidY_UP = (int)(cameraHeight / 3);
                                //CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidX_UP.ToString()), Int32.Parse(centroidY_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);


                                //left 
                                double centroidXleftmore_UP = (int)(cameraWidth / 3.2);
                                double centroidYleftmore_UP = (int)(cameraHeight / 3);


                                //right
                                double centroidXrightmore_UP = (int)(cameraWidth / 1.5);
                                double centroidYrightmore_UP = (int)(cameraHeight / 3);
                                //CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXrightmore_UP.ToString()), Int32.Parse(centroidYrightmore_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                //=======================================DOWN========================================
                                double centroidX_DOWN = (int)(cameraWidth / 2);
                                double centroidY_DOWN = (int)(cameraHeight / 1.5);
                                //CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidX_DOWN.ToString()), Int32.Parse(centroidY_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);


                                //left 
                                double centroidXleftmore_DOWN = (int)(cameraWidth / 3.2);
                                double centroidYleftmore_DOWN = (int)(cameraHeight / 1.5);


                                //right
                                double centroidXrightmore_DOWN = (int)(cameraWidth / 1.5);
                                double centroidYrightmore_DOWN = (int)(cameraHeight / 1.5);
                                //CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXrightmore_DOWN.ToString()), Int32.Parse(centroidYrightmore_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);




                                if (ShowFocusPoint.CheckState != 0)
                                {
                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidX.ToString()), Int32.Parse(centroidY.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);
                                    //left 
                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXleftmore.ToString()), Int32.Parse(centroidYleftmore.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);
                                    //right
                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXrightmore.ToString()), Int32.Parse(centroidYrightmore.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);
                                    //=========================================UP======================================
                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidX_UP.ToString()), Int32.Parse(centroidY_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);
                                    //left 
                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXleftmore_UP.ToString()), Int32.Parse(centroidYleftmore_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);
                                    //right
                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXrightmore_UP.ToString()), Int32.Parse(centroidYrightmore_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);
                                    //=======================================DOWN========================================
                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidX_DOWN.ToString()), Int32.Parse(centroidY_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);
                                    //left 
                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXleftmore_DOWN.ToString()), Int32.Parse(centroidYleftmore_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);
                                    //right
                                    CvInvoke.PutText(imageFrame, "X", new Point(Int32.Parse(centroidXrightmore_DOWN.ToString()), Int32.Parse(centroidYrightmore_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);
                                }

                                //==================Readlight Center
                                for (double i = centroidX - 10; i < centroidX; i++)
                                {

                                    for (double j = centroidY - 10; j < centroidY; j++)

                                    {
                                        if (centroidX > centroidY)
                                        {
                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;
                                        }


                                    }

                                }

                                for (double i = centroidXleftmore - 10; i < centroidXleftmore; i++)
                                {

                                    for (double j = centroidYleftmore - 10; j < centroidYleftmore; j++)

                                    {
                                        if (centroidXleftmore > centroidYleftmore)
                                        {
                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;
                                        }



                                    }

                                }







                                for (double i = centroidXrightmore - 10; i < centroidXrightmore; i++)
                                {

                                    for (double j = centroidYrightmore - 10; j < centroidYrightmore; j++)

                                    {
                                        if (centroidXrightmore > centroidYrightmore)
                                        {
                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;
                                        }


                                    }

                                }

                                //==================Readlight UP
                                for (double i = centroidX_UP - 10; i < centroidX_UP; i++)
                                {

                                    for (double j = centroidY_UP - 10; j < centroidY_UP; j++)

                                    {
                                        if (centroidX_UP > centroidY_UP)
                                        {
                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;
                                        }


                                    }

                                }



                                for (double i = centroidXleftmore_UP - 10; i < centroidXleftmore_UP; i++)
                                {

                                    for (double j = centroidYleftmore_UP - 10; j < centroidYleftmore_UP; j++)

                                    {
                                        if (centroidXleftmore_UP > centroidYleftmore_UP)
                                        {
                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;
                                        }


                                    }

                                }





                                for (double i = centroidXrightmore_UP - 10; i < centroidXrightmore_UP; i++)
                                {

                                    for (double j = centroidYrightmore_UP - 10; j < centroidYrightmore_UP; j++)

                                    {
                                        if (centroidXrightmore_UP > centroidYrightmore_UP)
                                        {
                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;

                                        }

                                    }

                                }
                                //==================Readlight Down
                                for (double i = centroidX_DOWN - 10; i < centroidX_DOWN; i++)
                                {

                                    for (double j = centroidY_DOWN - 10; j < centroidY_DOWN; j++)

                                    {
                                        if (centroidX_DOWN > centroidY_DOWN)
                                        {
                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;
                                        }


                                    }

                                }



                                for (double i = centroidXleftmore_DOWN - 10; i < centroidXleftmore_DOWN; i++)
                                {

                                    for (double j = centroidYleftmore_DOWN - 10; j < centroidYleftmore_DOWN; j++)

                                    {
                                        if (centroidXleftmore_DOWN > centroidYleftmore_DOWN)
                                        {
                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;

                                        }

                                    }

                                }




                                for (double i = centroidXrightmore_DOWN - 10; i < centroidXrightmore_DOWN; i++)
                                {

                                    for (double j = centroidYrightmore_DOWN - 10; j < centroidYrightmore_DOWN; j++)

                                    {
                                        if (centroidXrightmore_DOWN > centroidYrightmore_DOWN)
                                        {
                                            Color pixel = bmpImage_Frame.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));

                                            colorValue += (pixel.R + pixel.B + pixel.G) / 3;
                                        }


                                    }

                                }


                                colorValue = (colorValue / 900);
                            }
                            else if (FocusPoint.Text == "Histogram" && Histogramcheck.Checked == false)
                            {
                                FocusPoint.Text = "21 Focus Points";
                                Histogramcheck.Checked = false;
                                MessageBox.Show("Please turn on Histogram.", "Message", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                            }



                            ExposuringText.Text = string.Format("{0:0.00}", Math.Log(Math.Pow(2.8, 2) / (AppSetting.Data.ExposureTime / 1000), 2.0));
                            int hdrDetectionhigh = 0;
                            int hdrDetectionlow = 0;
                            int hdrDetectionmedium = 0;
                            double bestValue;
                            double Control_Jump = 1;
                            Console.WriteLine("Pixel Values: " + colorValue);

                            if (Histogramcheck.CheckState != 0 && houghCirclesradius != 0)
                            {

                                int colorHisto = 0;
                                Image<Gray, Byte> histoImage = houghCircles_Frame;
                                histoImage.ROI = new Rectangle((int)centroidX - houghCirclesradius * 7, (int)centroidY - houghCirclesradius * 7, houghCirclesradius * 7 * 2, houghCirclesradius * 7 * 2);
                                croppedImage = histoImage.Copy();
                                Histo.ClearHistogram();
                                Histo.GenerateHistograms(croppedImage.Convert<Gray, Byte>(), 256);

                                if (FocusPoint.Text == "Histogram")
                                {
                                    for (int i = 0; i < croppedImage.Width; i++)
                                    {
                                        for (int j = 0; j < croppedImage.Height; j++)
                                        {
                                            colorHisto += croppedImage.Data[i, j, 0];
                                            if (autoHDR.Checked == true)
                                            {
                                                if (croppedImage.Data[i, j, 0] >= 255)
                                                {
                                                    hdrDetectionhigh += 1;
                                                }
                                                else if (croppedImage.Data[i, j, 0] <= 60)
                                                {
                                                    hdrDetectionlow += 1;

                                                }
                                                else
                                                {
                                                    hdrDetectionmedium += 1;
                                                }
                                            }
                                        }
                                    }

                                    colorHisto = colorHisto / (croppedImage.Width * croppedImage.Height);
                                    Console.WriteLine(colorHisto);
                                    colorValue = colorHisto;
                                }
                                Histo.Refresh();
                            }
                            else if (Histogramcheck.Checked == true && houghCirclesradius == 0)
                            {
                                Histogramcheck.Checked = false;
                                MessageBox.Show("Please turn on HoughCircles.", "Message", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                            }
                            //=========================Auto HDR===========================
                            if (autoHDR.Checked == true && FocusPoint.Text == "Histogram")
                            {
                                Console.WriteLine("hdrDetectionhigh =" + hdrDetectionhigh);
                                Console.WriteLine("hdrDetectionlow =" + hdrDetectionlow);
                                Console.WriteLine("hdrDetectionmedium =" + hdrDetectionmedium);

                                if (hdrDetectionhigh > 12000 && hdrDetectionlow > 300000)
                                {
                                    hdrOn = true;
                                }
                                

                                if (hdrOn == true)
                                {
                                    Console.WriteLine("OnHDR");
                                    bool doOne = false;
                                    int imageMerge_status = 0;

                                    if (AppSetting.Data.ExposureTime > 1)
                                    {
                                        if (colorValue > minLight - 30 && colorValue <= maxLight + 30)
                                        {
                                            Control_Jump = 2;
                                        }
                                        else
                                        {
                                            Exposure_Adjust();
                                            Control_Jump = 1;
                                        }
                                    }
                                    else
                                    {
                                        Control_Jump = 1.6;
                                    }

                                    if (colorValue > maxLight_hdr && CameraStateText.Text != "ASI_EXP_WORKING" 
                                        && recover != false && AppSetting.Data.ExposureTime >= AppSetting.Data.MIN_SHUTTER)
                                    {
                                        //AppSetting.Data.ExposureTime -
                                        bestValue = ((AppSetting.Data.ExposureTime / goldenRatio) * (Control_Jump));
                                        Console.WriteLine("bestValue" + bestValue);
                                        AppSetting.Data.ExposureTime = Math.Round(bestValue, 2);
                                        recover = false;
                                        if (AppSetting.Data.ExposureTime == 0)
                                        {
                                            AppSetting.Data.ExposureTime = 0.1;
                                        }

                                    }
                                    else if (colorValue < minLight_hdr && CameraStateText.Text != "ASI_EXP_WORKING" 
                                        && recover != false
                                        && AppSetting.Data.ExposureTime <= AppSetting.Data.MAX_SHUTTER)
                                    {
                                        //AppSetting.Data.ExposureTime +
                                        bestValue = ((AppSetting.Data.ExposureTime * goldenRatio) / (Control_Jump));
                                        Console.WriteLine("bestValue" + bestValue);
                                        AppSetting.Data.ExposureTime = Math.Round(bestValue, 2);
                                        recover = false;
                                    }
                                    else if (colorValue >= minLight_hdr && colorValue <= maxLight_hdr && doOne == false)
                                    {
                                        ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                                        System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                                        EncoderParameters myEncoderParameters = new EncoderParameters(1);
                                        EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 50L);
                                        myEncoderParameters.Param[0] = myEncoderParameter;
                                        float expTimesMedium = 1, expTimesMax = 1, expTimesMin = 1;


                                        Directory.CreateDirectory(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\HDR" + @"\");

                                        if (maxLight_hdr == AppSetting.Data.maxLight && minLight_hdr == AppSetting.Data.minLight
                                            || AppSetting.Data.ExposureTime >= 120000)
                                        {
                                            doOne = true;
                                            imageMerge_status = 1;
                                            expTimesMedium = (float)AppSetting.Data.ExposureTime / 1000;
                                            hdrMedium = hdrOutput;
                                            hdrMedium._GammaCorrect(0.4);
                                            Console.WriteLine("expTimesMedium = " + expTimesMedium);
                                            hdrMedium.ToBitmap().Save(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\HDR" + @"\" + "hdrMedium.jpg", jpgEncoder, myEncoderParameters);
                                            maxLight_hdr = AppSetting.Data.maxLight - 40;
                                            minLight_hdr = AppSetting.Data.minLight - 40;

                                        }
                                        else if (maxLight_hdr == AppSetting.Data.maxLight - 40 && minLight_hdr == AppSetting.Data.minLight - 40 && doOne == false
                                            || AppSetting.Data.ExposureTime >= 120000)
                                        {
                                            doOne = true;
                                            imageMerge_status = 2;
                                            expTimesMin = (float)AppSetting.Data.ExposureTime / 1000;
                                            hdrLow = hdrOutput;
                                            hdrLow._GammaCorrect(0.4);
                                            Console.WriteLine("expTimesMin = " + expTimesMin);
                                            hdrLow.ToBitmap().Save(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\HDR" + @"\" + "hdrLow.jpg", jpgEncoder, myEncoderParameters);
                                            maxLight_hdr = AppSetting.Data.maxLight + 50;
                                            minLight_hdr = AppSetting.Data.minLight + 30;
                                            if (AppSetting.Data.ExposureTime >= 120000)
                                            {
                                                AppSetting.Data.ExposureTime = 60000;
                                            }
                                        }
                                        else if (maxLight_hdr == AppSetting.Data.maxLight + 50 && minLight_hdr == AppSetting.Data.minLight + 30 && doOne == false
                                            || AppSetting.Data.ExposureTime == 60000)
                                        {
                                            doOne = true;
                                            imageMerge_status = 3;
                                            expTimesMax = (float)AppSetting.Data.ExposureTime / 1000;
                                            hdrHigh = hdrOutput;
                                            hdrHigh._GammaCorrect(0.4);
                                            hdrHigh.ToBitmap().Save(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\HDR" + @"\" + "hdrHigh.jpg", jpgEncoder, myEncoderParameters);
                                            maxLight_hdr = AppSetting.Data.maxLight;
                                            minLight_hdr = AppSetting.Data.minLight;
                                        }
                                        if (imageMerge_status == 3)
                                        {
                                            imageMerge_status = 0;
                                            for (int i = 0; i < hdrMedium.Height; i++)
                                            {
                                                for (int j = 0; j < hdrMedium.Width; j++)
                                                {
                                                    if (hdrMedium.Data[i, j, 0] > AppSetting.Data.maxLight && hdrMedium.Data[i, j, 1] > AppSetting.Data.maxLight && hdrMedium.Data[i, j, 2] > AppSetting.Data.maxLight)
                                                    {
                                                        hdrMedium.Data[i, j, 0] = (byte)((hdrMedium.Data[i, j, 0] + hdrLow.Data[i, j, 0]) / 2);
                                                        hdrMedium.Data[i, j, 1] = (byte)((hdrMedium.Data[i, j, 1] + hdrLow.Data[i, j, 1]) / 2);
                                                        hdrMedium.Data[i, j, 2] = (byte)((hdrMedium.Data[i, j, 2] + hdrLow.Data[i, j, 2]) / 2);
                                                    }
                                                    else if (hdrMedium.Data[i, j, 0] < AppSetting.Data.minLight && hdrMedium.Data[i, j, 1] < AppSetting.Data.minLight && hdrMedium.Data[i, j, 2] < AppSetting.Data.minLight
                                                        && hdrMedium.Data[i, j, 0] > 10 && hdrMedium.Data[i, j, 1] > 10 && hdrMedium.Data[i, j, 2] > 10)
                                                    {
                                                        hdrMedium.Data[i, j, 0] = (byte)((hdrMedium.Data[i, j, 0] + hdrHigh.Data[i, j, 0]) / 2);
                                                        hdrMedium.Data[i, j, 1] = (byte)((hdrMedium.Data[i, j, 1] + hdrHigh.Data[i, j, 1]) / 2);
                                                        hdrMedium.Data[i, j, 2] = (byte)((hdrMedium.Data[i, j, 2] + hdrHigh.Data[i, j, 2]) / 2);

                                                    }
                                                }
                                            }
                                            hdrMedium._EqualizeHist();
                                            hdrMedium._GammaCorrect(1.4d);
                                            Image<Bgr, Byte> ImagehdrMedium = hdrMedium;
                                            ImagehdrMedium.Draw(borderTime, new Bgr(Color.Black), -1);
                                            ImagehdrMedium.Draw(borderTime, new Bgr(Color.White), 2);
                                            ImagehdrMedium.Draw(borderExposureTime, new Bgr(Color.Black), -1);
                                            ImagehdrMedium.Draw(borderExposureTime, new Bgr(Color.White), 2);
                                            CvInvoke.PutText(ImagehdrMedium, "UTC " + timesStamp, new Point(0, borderHeight + 50), FontFace.HersheySimplex, 1.5, new Bgr(Color.White).MCvScalar, thickness);

                                            if (exposureTime_Show < 1000 && exposureTime_Show > 1)
                                            {
                                                CvInvoke.PutText(ImagehdrMedium, Math.Round(AppSetting.Data.ExposureTime, 0) + " ms", new Point(borderWidth + 50, borderHeight + 50), FontFace.HersheySimplex, 1.5, new Bgr(Color.White).MCvScalar, thickness);
                                            }
                                            else if (exposureTime_Show >= 1000)
                                            {
                                                CvInvoke.PutText(ImagehdrMedium, Math.Round(AppSetting.Data.ExposureTime / 1000, 0) + " sec", new Point(borderWidth + 50, borderHeight + 50), FontFace.HersheySimplex, 1.5, new Bgr(Color.White).MCvScalar, thickness);
                                            }
                                            else if (exposureTime_Show >= 0 && exposureTime_Show <= 1)
                                            {
                                                CvInvoke.PutText(ImagehdrMedium, Math.Round(AppSetting.Data.ExposureTime, 2) + " ms", new Point(borderWidth + 50, borderHeight + 50), FontFace.HersheySimplex, 1.5, new Bgr(Color.White).MCvScalar, thickness);
                                            }
                                            CvInvoke.PutText(ImagehdrMedium, "AUTO HDR", new Point(borderWidth - 300, borderHeight + 50), FontFace.HersheySimplex, 1.5, new Bgr(Color.White).MCvScalar, thickness);
                                            Directory.CreateDirectory(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\" + timeFolder);

                                            if (AppSetting.Data.SaveFileDialog != "" && timeNow_Chack != timeBefore && ConnectedCameras > 0)
                                            {
                                                if(StopSave.Checked == false)
                                                {
                                                    timeBefore = DateTime.Now.ToString("yyyy_MM_dd__HH_mm");
                                                    ImagehdrMedium.ToBitmap().Save(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\" + timeFolder + @"\" + timeNow + ".jpg", jpgEncoder, myEncoderParameters);
                                                }

                                                if (hdrDetectionhigh > 12000 && hdrDetectionlow > 200000)
                                                {
                                                    hdrOn = true;
                                                }
                                                else
                                                {
                                                    hdrOn = false;
                                                }
                                            }
                                            MainImageControl.Image = ImagehdrMedium;


                                        }
                                    }
                                }
                                else
                                {
                                    Control_Jump = 1.6;
                                }

                                if (colorValue > maxLight_hdr && CameraStateText.Text != "ASI_EXP_WORKING"
                                    && recover != false && AppSetting.Data.ExposureTime >= AppSetting.Data.MIN_SHUTTER)
                                {
                                    //AppSetting.Data.ExposureTime -
                                    bestValue = ((AppSetting.Data.ExposureTime / goldenRatio) * (Control_Jump));
                                    Console.WriteLine("bestValue" + bestValue);
                                    AppSetting.Data.ExposureTime = Math.Round(bestValue, 2);
                                    recover = false;
                                    if (AppSetting.Data.ExposureTime == 0)
                                    {
                                        AppSetting.Data.ExposureTime = 0.1;
                                    }

                                }
                                else if (colorValue < minLight_hdr && CameraStateText.Text != "ASI_EXP_WORKING"
                                    && recover != false
                                    && AppSetting.Data.ExposureTime <= AppSetting.Data.MAX_SHUTTER)
                                {
                                    //AppSetting.Data.ExposureTime +
                                    bestValue = ((AppSetting.Data.ExposureTime * goldenRatio) / (Control_Jump));
                                    Console.WriteLine("bestValue" + bestValue);
                                    AppSetting.Data.ExposureTime = Math.Round(bestValue, 2);
                                    recover = false;
                                }
                                timeBefore = DateTime.Now.ToString("yyyy_MM_dd__HH_mm");
                                Directory.CreateDirectory(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\" + timeFolder);
                                if (AppSetting.Data.SaveFileDialog != "" && timeNow_Chack != timeBefore && ConnectedCameras > 0 && StopSave.Checked == false)
                                {
                                    timeBefore = DateTime.Now.ToString("yyyy_MM_dd__HH_mm");
                                    ProcessFrame.ToBitmap().Save(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\" + timeFolder + @"\" + timeNow + ".jpg", jpgEncoder, myEncoderParameters);

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
                                bool doOne;
                                doOne = false;
                                int imageMerge_status = 0;
                                autoHDR.Checked = false;
                                if (AppSetting.Data.ExposureTime > 1)
                                {
                                    if (colorValue > minLight - 30 && colorValue <= maxLight + 30)
                                    {
                                        Control_Jump = 2;
                                    }
                                    else
                                    {
                                        Exposure_Adjust();
                                        Control_Jump = 1;
                                    }
                                }
                                else
                                {
                                    Control_Jump = 1.6;
                                }

                                if (colorValue > maxLight_hdr && CameraStateText.Text != "ASI_EXP_WORKING" && recover != false
                                    && AppSetting.Data.ExposureTime >= AppSetting.Data.MIN_SHUTTER)
                                {
                                    //AppSetting.Data.ExposureTime -
                                    bestValue = ((AppSetting.Data.ExposureTime / goldenRatio) * (Control_Jump));
                                    Console.WriteLine("bestValue" + bestValue);
                                    AppSetting.Data.ExposureTime = Math.Round(bestValue, 2);
                                    recover = false;
                                    if (AppSetting.Data.ExposureTime == 0)
                                    {
                                        AppSetting.Data.ExposureTime = 0.1;
                                    }

                                }
                                else if (colorValue < minLight_hdr && CameraStateText.Text != "ASI_EXP_WORKING" && recover != false && AppSetting.Data.ExposureTime <= AppSetting.Data.MAX_SHUTTER)
                                {
                                    //AppSetting.Data.ExposureTime +
                                    bestValue = ((AppSetting.Data.ExposureTime * goldenRatio) / (Control_Jump));
                                    Console.WriteLine("bestValue" + bestValue);
                                    AppSetting.Data.ExposureTime = Math.Round(bestValue, 2);
                                    recover = false;
                                    if (AppSetting.Data.ExposureTime > AppSetting.Data.MAX_SHUTTER)
                                    {
                                        AppSetting.Data.ExposureTime = AppSetting.Data.MAX_SHUTTER;
                                    }
                                }
                                else if (colorValue >= minLight_hdr && colorValue <= maxLight_hdr && doOne == false)
                                {

                                    ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                                    System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                                    EncoderParameters myEncoderParameters = new EncoderParameters(1);
                                    EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 50L);
                                    myEncoderParameters.Param[0] = myEncoderParameter;
                                    float expTimesMedium = 1, expTimesMax = 1, expTimesMin = 1;


                                    Directory.CreateDirectory(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\HDR" + @"\");
                                    if (maxLight_hdr == AppSetting.Data.maxLight && minLight_hdr == AppSetting.Data.minLight
                                             || AppSetting.Data.ExposureTime >= 120000)
                                    {
                                        doOne = true;
                                        imageMerge_status = 1;
                                        expTimesMedium = (float)AppSetting.Data.ExposureTime / 1000;
                                        hdrMedium = hdrOutput;
                                        hdrMedium._GammaCorrect(0.4);
                                        Console.WriteLine("expTimesMedium = " + expTimesMedium);
                                        hdrMedium.ToBitmap().Save(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\HDR" + @"\" + "hdrMedium.jpg", jpgEncoder, myEncoderParameters);
                                        maxLight_hdr = AppSetting.Data.maxLight - 40;
                                        minLight_hdr = AppSetting.Data.minLight - 40;

                                    }
                                    else if (maxLight_hdr == AppSetting.Data.maxLight - 40 && minLight_hdr == AppSetting.Data.minLight - 40 && doOne == false
                                        || AppSetting.Data.ExposureTime >= 120000)
                                    {
                                        doOne = true;
                                        imageMerge_status = 2;
                                        expTimesMin = (float)AppSetting.Data.ExposureTime / 1000;
                                        hdrLow = hdrOutput;
                                        hdrLow._GammaCorrect(0.4);
                                        Console.WriteLine("expTimesMin = " + expTimesMin);
                                        hdrLow.ToBitmap().Save(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\HDR" + @"\" + "hdrLow.jpg", jpgEncoder, myEncoderParameters);
                                        maxLight_hdr = AppSetting.Data.maxLight + 50;
                                        minLight_hdr = AppSetting.Data.minLight + 30;
                                        if (AppSetting.Data.ExposureTime >= 120000)
                                        {
                                            AppSetting.Data.ExposureTime = 60000;
                                        }
                                    }
                                    else if (maxLight_hdr == AppSetting.Data.maxLight + 50 && minLight_hdr == AppSetting.Data.minLight + 30 && doOne == false
                                        || AppSetting.Data.ExposureTime == 60000)
                                    {
                                        doOne = true;
                                        imageMerge_status = 3;
                                        expTimesMax = (float)AppSetting.Data.ExposureTime / 1000;
                                        hdrHigh = hdrOutput;
                                        hdrHigh._GammaCorrect(0.4);
                                        hdrHigh.ToBitmap().Save(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\HDR" + @"\" + "hdrHigh.jpg", jpgEncoder, myEncoderParameters);
                                        maxLight_hdr = AppSetting.Data.maxLight;
                                        minLight_hdr = AppSetting.Data.minLight;
                                    }
                                    if (imageMerge_status == 3)
                                    {
                                        imageMerge_status = 0;

                                        for (int i = 0; i < hdrMedium.Height; i++)
                                        {

                                            for (int j = 0; j < hdrMedium.Width; j++)

                                            {

                                                if (hdrMedium.Data[i, j, 0] > AppSetting.Data.maxLight
                                                    && hdrMedium.Data[i, j, 1] > AppSetting.Data.maxLight
                                                    && hdrMedium.Data[i, j, 2] > AppSetting.Data.maxLight)
                                                {

                                                    hdrMedium.Data[i, j, 0] = (byte)((hdrMedium.Data[i, j, 0] + hdrLow.Data[i, j, 0]) / 2);
                                                    hdrMedium.Data[i, j, 1] = (byte)((hdrMedium.Data[i, j, 1] + hdrLow.Data[i, j, 1]) / 2);
                                                    hdrMedium.Data[i, j, 2] = (byte)((hdrMedium.Data[i, j, 2] + hdrLow.Data[i, j, 2]) / 2);

                                                }
                                                else if (hdrMedium.Data[i, j, 0] < AppSetting.Data.minLight
                                                    && hdrMedium.Data[i, j, 1] < AppSetting.Data.minLight
                                                    && hdrMedium.Data[i, j, 2] < AppSetting.Data.minLight
                                                    && hdrMedium.Data[i, j, 0] > 10
                                                    && hdrMedium.Data[i, j, 1] > 10
                                                    && hdrMedium.Data[i, j, 2] > 10)
                                                {

                                                    hdrMedium.Data[i, j, 0] = (byte)((hdrMedium.Data[i, j, 0] + hdrHigh.Data[i, j, 0]) / 2);
                                                    hdrMedium.Data[i, j, 1] = (byte)((hdrMedium.Data[i, j, 1] + hdrHigh.Data[i, j, 1]) / 2);
                                                    hdrMedium.Data[i, j, 2] = (byte)((hdrMedium.Data[i, j, 2] + hdrHigh.Data[i, j, 2]) / 2);

                                                }

                                            }


                                        }
                                        hdrMedium._SmoothGaussian(1);
                                        hdrMedium._EqualizeHist();
                                        hdrMedium._GammaCorrect(1.4d);
                                        Image<Bgr, Byte> ImagehdrMedium = hdrMedium;
                                        ImagehdrMedium.Draw(borderTime, new Bgr(Color.Black), -1);
                                        ImagehdrMedium.Draw(borderTime, new Bgr(Color.White), 2);
                                        ImagehdrMedium.Draw(borderExposureTime, new Bgr(Color.Black), -1);
                                        ImagehdrMedium.Draw(borderExposureTime, new Bgr(Color.White), 2);
                                        CvInvoke.PutText(ImagehdrMedium, "UTC " + timesStamp, new Point(0, borderHeight + 50), FontFace.HersheySimplex, 1.5, new Bgr(Color.White).MCvScalar, thickness);
                                        if (exposureTime_Show < 1000 && exposureTime_Show > 1)
                                        {
                                            CvInvoke.PutText(ImagehdrMedium, Math.Round(AppSetting.Data.ExposureTime, 0) + " ms", new Point(borderWidth + 50, borderHeight + 50), FontFace.HersheySimplex, 1.5, new Bgr(Color.White).MCvScalar, thickness);
                                        }
                                        else if (exposureTime_Show >= 1000)
                                        {
                                            CvInvoke.PutText(ImagehdrMedium, Math.Round(AppSetting.Data.ExposureTime / 1000, 0) + " sec", new Point(borderWidth + 50, borderHeight + 50), FontFace.HersheySimplex, 1.5, new Bgr(Color.White).MCvScalar, thickness);
                                        }
                                        else if (exposureTime_Show >= 0 && exposureTime_Show <= 1)
                                        {
                                            CvInvoke.PutText(ImagehdrMedium, Math.Round(AppSetting.Data.ExposureTime, 2) + " ms", new Point(borderWidth + 50, borderHeight + 50), FontFace.HersheySimplex, 1.5, new Bgr(Color.White).MCvScalar, thickness);
                                        }

                                        CvInvoke.PutText(ImagehdrMedium, "HDR ON", new Point(borderWidth - 200, borderHeight + 50), FontFace.HersheySimplex, 1.5, new Bgr(Color.White).MCvScalar, thickness);
                                        Directory.CreateDirectory(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\" + timeFolder);
                                        timeNow_Chack = DateTime.Now.ToString("yyyy_MM_dd__HH_mm");
                                        if (AppSetting.Data.SaveFileDialog != "" && timeNow_Chack != timeBefore && ConnectedCameras > 0 && StopSave.Checked == false)
                                        {
                                            timeBefore = DateTime.Now.ToString("yyyy_MM_dd__HH_mm");
                                            ImagehdrMedium.ToBitmap().Save(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\" + timeFolder + @"\" + timeNow + ".jpg", jpgEncoder,
                                            myEncoderParameters);
                                        }


                                        MainImageControl.Image = ImagehdrMedium;
                                    }





                                }




                            }





                            if (hdr_On.CheckState == 0 && autoHDR.Checked == false && hdrOn == false)
                            {


                                minLight = AppSetting.Data.minLight;
                                maxLight = AppSetting.Data.maxLight;
                                

                                if (colorValue > minLight - 30 && colorValue <= maxLight + 30)
                                {
                                    Control_Jump = 2;
                                }
                                else if (colorValue > minLight - 5 && colorValue <= maxLight + 5)
                                {
                                    Control_Jump = 2.1;
                                }
                                else if (colorValue > minLight - 2 && colorValue <= maxLight + 2)
                                {
                                    Control_Jump = 2.11;
                                }
                                else
                                {
                                    Exposure_Adjust();
                                    Control_Jump = 1;
                                }

                                if (colorValue >= maxLight && CameraStateText.Text != "ASI_EXP_WORKING" && recover != false
                                    && AppSetting.Data.ExposureTime >= AppSetting.Data.MIN_SHUTTER)
                                {
                                    //Exposure_Adjust();
                                    //AppSetting.Data.ExposureTime = ExposureTimeAverage;
                                    bestValue = ((AppSetting.Data.ExposureTime / goldenRatio) * (Control_Jump));
                                    Console.WriteLine("bestValue" + bestValue);
                                    AppSetting.Data.ExposureTime = Math.Round(bestValue, 2);
                                    recover = false;
                                    //AppSetting.Data.ExposureTime -
                                }
                                else if (colorValue <= minLight && CameraStateText.Text != "ASI_EXP_WORKING" && recover != false && AppSetting.Data.ExposureTime <= AppSetting.Data.MAX_SHUTTER)
                                {
                                    if (AppSetting.Data.ExposureTime == 0)
                                    {
                                        AppSetting.Data.ExposureTime = 0.1;
                                    }
                                    //Exposure_Adjust();
                                    //AppSetting.Data.ExposureTime = ExposureTimeAverage;
                                    bestValue = ((AppSetting.Data.ExposureTime * goldenRatio) / (Control_Jump));
                                    Console.WriteLine("bestValue" + bestValue);
                                    AppSetting.Data.ExposureTime = Math.Round(bestValue, 2);

                                    recover = false;
                                    //AppSetting.Data.ExposureTime +
                                }
                                else if (AppSetting.Data.ExposureTime > AppSetting.Data.MAX_SHUTTER)
                                {
                                    AppSetting.Data.ExposureTime = AppSetting.Data.MAX_SHUTTER;
                                }


                            }

                            ExpouseTimeText.Value = (decimal)AppSetting.Data.ExposureTime;

                            keoGramsImage = new Rectangle(new Point(cameraWidth / 2, cameraHeight / 2), new Size(1, cameraHeight));
                            keoGramsFrame.ROI = keoGramsImage;
                            Bitmap outputImage = keoGramsFrame.ToBitmap();



                            if (keoGrams.Checked == true && StopSave.Checked == false)
                            {
                                string timeMinutes = DateTime.Now.AddMinutes(-1).ToString("yyyy_MM_dd__HH_mm");
                                Directory.CreateDirectory(AppSetting.Data.SaveFileDialog + @"\keoGrams" + @"\" + timeFolder);
                                Bitmap keogramsImage = keoGramsFrame.ToBitmap();
                                string pathKeograms = AppSetting.Data.SaveFileDialog + @"\keoGrams" + @"\" + timeFolder + @"\" + timeMinutes + "keoGrams" + ".jpg";
                                Console.WriteLine(pathKeograms);
                                int Minutesinday = (int)DateTime.Now.TimeOfDay.TotalMinutes;


                                if (File.Exists(pathKeograms))
                                {
                                    keogramsImage = new Bitmap(pathKeograms);
                                    Console.WriteLine("keogramsImage1");
                                }
                                else if (File.Exists(AppSetting.Data.SaveFileDialog + @"\keoGrams" + @"\" + timeFolder + @"\" + "keoGrams" + ".jpg") && !File.Exists(pathKeograms))
                                {
                                    keogramsImage = new Bitmap(AppSetting.Data.SaveFileDialog + @"\keoGrams" + @"\" + timeFolder + @"\" + "keoGrams" + ".jpg");
                                    Console.WriteLine("keogramsImage2");
                                }

                                Bitmap keograms = keoGramsFrame.ToBitmap();

                                if (!File.Exists(AppSetting.Data.SaveFileDialog + @"\keoGrams" + @"\" + timeFolder + @"\" + "keoGrams" + ".jpg"))
                                {

                                    ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                                    System.Drawing.Imaging.Encoder myEncoder =
                                            System.Drawing.Imaging.Encoder.Quality;
                                    EncoderParameters myEncoderParameters = new EncoderParameters(1);
                                    EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 50L);


                                    myEncoderParameters.Param[0] = myEncoderParameter;

                                    var bitmap = new Bitmap(Minutesinday, keogramsImage.Height);

                                    for (var x = 0; x < bitmap.Width; x++)
                                    {
                                        for (var y = 0; y < bitmap.Height; y++)
                                        {

                                            bitmap.SetPixel(x, y, Color.Black);

                                        }
                                    }

                                    bitmap.Save(AppSetting.Data.SaveFileDialog + @"\keoGrams" + @"\" + timeFolder + @"\" + "keoGrams" + ".jpg", jpgEncoder,
                                    myEncoderParameters);
                                    Console.WriteLine("keogramsImagebitmap1");

                                }
                                /*else if (!File.Exists(pathKeograms))
                                {
                                    Console.WriteLine("keogramsImagebitmap2");
                                    var directory = new DirectoryInfo(AppSetting.Data.SaveFileDialog + @"\keoGrams" + @"\" + timeFolder);
                                    var myFile = (from f in directory.GetFiles()
                                                  orderby f.LastWriteTime descending
                                                  select f).First();


                                    ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                                    System.Drawing.Imaging.Encoder myEncoder =
                                            System.Drawing.Imaging.Encoder.Quality;
                                    EncoderParameters myEncoderParameters = new EncoderParameters(1);
                                    EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder,
                                    50L);


                                    myEncoderParameters.Param[0] = myEncoderParameter;

                                    var bitmap = new Bitmap(AppSetting.Data.SaveFileDialog + @"\keoGrams" + @"\" + timeFolder + @"\" + myFile.ToString());

                                    for (var x = bitmap.Width; x < Minutesinday; x++)
                                    {
                                        for (var y = 0; y < bitmap.Height; y++)
                                        {

                                            bitmap.SetPixel(x, y, Color.Black);

                                        }
                                    }

                                    keogramsImage = bitmap;

                                }*/




                                //int outputImageHeight = keogramsImage.Height > keograms.Height ? keogramsImage.Height : keograms.Height;
                                int outputImageHeight = keogramsImage.Height;
                                int outputImageWidth = keogramsImage.Width + keograms.Width;

                                outputImage = new Bitmap(outputImageWidth, outputImageHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                                Graphics graphics = Graphics.FromImage(outputImage);

                                graphics.DrawImage(keogramsImage, new Rectangle(new Point(), keogramsImage.Size),
                                    new Rectangle(new Point(), keogramsImage.Size), GraphicsUnit.Pixel);
                                graphics.DrawImage(keograms, new Rectangle(new Point(keogramsImage.Width, 0), keograms.Size),
                                    new Rectangle(new Point(), keograms.Size), GraphicsUnit.Pixel);




                            }

                            if (AppSetting.Data.SaveFileDialog != "" && timeNow_Chack != timeBefore
                                && colorValue <= 150
                                && ConnectedCameras > 0 && autoHDR.CheckState == 0
                                && hdr_On.CheckState == 0
                                && StopSave.Checked == false)
                            {
                                timeBefore = DateTime.Now.ToString("yyyy_MM_dd__HH_mm");

                                ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                                System.Drawing.Imaging.Encoder myEncoder =
                                        System.Drawing.Imaging.Encoder.Quality;
                                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder,
                                50L);

                                myEncoderParameters.Param[0] = myEncoderParameter;



                                Directory.CreateDirectory(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\" + timeFolder);

                                bmpImage_Frame.Save(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\" + timeFolder + @"\" + timeNow + ".jpg", jpgEncoder,
                                    myEncoderParameters);
                                if (keoGrams.CheckState != 0)
                                {
                                    //File.Delete(AppSetting.Data.SaveFileDialog + @"\keoGrams" + @"\" + timeFolder + @"\" + "keoGrams" + ".jpg");
                                    outputImage.Save(AppSetting.Data.SaveFileDialog + @"\keoGrams" + @"\" + timeFolder + @"\" + DateTime.Now.ToString("yyyy_MM_dd__HH_mm") + "keoGrams" + ".jpg", jpgEncoder,
                                    myEncoderParameters);
                                }


                                if (SaveLog.CheckState != 0)
                                {
                                    File.AppendAllText(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\log " + timeFolder + ".txt", timeNow + " ExpouseTime = " + ExpouseTimeText.Value.ToString() + " Exposuring = " + ExposuringText.Text.ToString() + " Color(0-255) = " + colorValue + "\n");
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
