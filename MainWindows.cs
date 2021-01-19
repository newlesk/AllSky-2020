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

namespace AllSky_2020
{
    public enum CAMERASTSTATE { CAPTURING, DRAWING }

    public partial class MainWindows : Form
    {
        public CAMERASTSTATE _CAMERASTSTATE;
        public string SystemMessage = "Ready.";

        private Image<Bgr, Byte> RootFrame = null, ProcessFrame = null, ROIFrame, circleImage = null,
            ProcessFrameGray = null, houghCirclesFrame = null, keoGramsFrame = null;
        private Rectangle ROIRec;
        private Rectangle keoGramsImage;
        private IntPtr imageBuf;

        private bool GetImageState = false;
        private bool IsDefineROI = false;
        private bool IsDefineOrigin = false;
        private int CameraId, ConnectedCameras;
        public string SaveFileDialog;
        private int Colorall;
        private double CentroidX, CentroidY;
        private ASI_CAMERA_INFO Info;
        private Stopwatch ExposureCounter;
        private string folderName;
        private string TimeNow;
        private string TimeFolder;
        private string TimeBefore;
        //private ASI_CONTROL_CAPS CAPS;
        private string TimeNowChack;
        //private int CameraCameraId;
        private int HoughCirclesX, HoughCirclesY;
        //private int TimeExposure;
        private bool IsExpSuccess;
        private bool Recover;
        double cannyThreshold;
        double circleAccumulatorThreshold;
        double golden_ratio = (1 + (Math.Sqrt(5)) / 2);
        int HoughCirclesradius = 0;
        bool houghCircles_status, cameraLost = false;

        int maxLight_hdr;
        int minLight_hdr;
        Image<Bgr, Byte> hdrHigh = null;
        Image<Bgr, Byte> hdrLow = null;
        Image<Bgr, Byte> hdrMedium = null;

        int max_light;
        int min_light;








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
            ASICameraDll2.ASISetControlValue(CameraId, ASI_CONTROL_TYPE.ASI_GAIN, 0);
            ASICameraDll2.ASISetControlValue(CameraId, ASI_CONTROL_TYPE.ASI_AUTO_MAX_GAIN, 250);

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
            
            pixelvalues_min.Text = AppSetting.Data.min_light.ToString();
            pixelvalues_max.Text = AppSetting.Data.max_light.ToString();
            
            
            if (IsAutoExposureTime.CheckState == 0)
            {
                IsAutoExposureTime.Checked = true;
                FocusPoint.Text = "21 Focus Points";
                SpeedMode.Checked = true;
            }
            HoughCircles_Profile.Text = "Select Your Profile";
            ProfilePixelValues.Text = "Select Your Profile";

            int HoughCircleslineCount = File.ReadLines(@"./HoughCircles_Profile.txt").Count();
            for (int i = 0; i < (HoughCircleslineCount / 2); i++)
            {

                HoughCircles_Profile.Items.Add("Profile " + i);

            }


            int PixellineCount = File.ReadLines(@"./Pixel_Profile.txt").Count();
            for (int i = 0; i < (PixellineCount / 2); i++)
            {

                HoughCircles_Profile.Items.Add("Profile " + i);

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

            MainImageControl.SetZoomScale(AppSetting.Data.ZoomScale, new Point(0, 0));

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
                    //Recover = false;
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
                            ProcessFrameGray = RootFrame.Copy();
                            ProcessFrame = RootFrame.Copy();
                            houghCirclesFrame = RootFrame.Copy();
                            ROIFrame = RootFrame.Copy();
                            keoGramsFrame = RootFrame.Copy();
                            Recover = true;
                        }
                    }
                    else
                    {
                        ASICameraDll2.ASICloseCamera(CameraId);
                        CameraConnection();
                    }
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    if (Colorall <= 135 && AppSetting.Data.ExposureTime < 1 && SpeedMode.CheckState == 0)
                    {
                        await Task.Delay(50000);
                    }
                    else if (Colorall <= 135 && Colorall >= 80 && AppSetting.Data.ExposureTime < 1000 && SpeedMode.CheckState == 0)
                    {
                        await Task.Delay(50000);
                    }
                    else if (Colorall <= 135 && Colorall >= 80 && AppSetting.Data.ExposureTime >= 1000 && AppSetting.Data.ExposureTime < 10000 && SpeedMode.CheckState == 0)
                    {
                        await Task.Delay(5000);
                    }
                    else
                        await Task.Delay(200);

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

                Size ROISize = new Size((int)AppSetting.Data.ROIWidth, (int)AppSetting.Data.ROIHeight);
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

            // Show the FolderBrowserDialog.
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
                // CamerasIDselect = i;
                if (CameraList.Text == CameraList.SelectedItem.ToString())
                {
                    AppSetting.Data.CameraId = i;
                    AppSetting.Save();
                }
            }
            System.Diagnostics.Debug.WriteLine("CameraID = " + AppSetting.Data.CameraId);
            Application.Restart();
            //CameraConnection();

            //InitializeCamera();



            //AppSetting.Data.CameraId = int.Parse(CameraList.Text);
            // System.Diagnostics.Debug.WriteLine("CameraID = " + AppSetting.Data.CameraId);
            // 
            //InitializeCamera();

        }

        private void ResetZoom_Click(object sender, EventArgs e)
        {
            ROIRec.Width = 0;
            ROIRec.Height = 0;
        }

        
        private void SavePixelValues_Click(object sender, EventArgs e)
        {
            AppSetting.Data.max_light = int.Parse(pixelvalues_max.Text);
            AppSetting.Data.min_light = int.Parse(pixelvalues_min.Text);
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
                if (HoughCircles_Profile.Text == "Profile 0")
                {

                   
                    max_light = int.Parse(lines[0]);

                  
                    min_light = int.Parse(lines[1]);
                }

                else if (HoughCircles_Profile.Text == "Profile " + i && HoughCircles_Profile.Text != "Profile 0")
                {
                    
                    max_light = int.Parse(lines[i * 2]);

                    
                    min_light = int.Parse(lines[i * 2 + 1]);
                }


            }
            
            pixelvalues_min.Text = min_light.ToString();
            pixelvalues_max.Text = max_light.ToString();
            AppSetting.Data.max_light = max_light;
            AppSetting.Data.min_light = min_light;
        }

        private void ClearProfilePixel_Click(object sender, EventArgs e)
        {
            File.WriteAllText(@"./Pixel_Profile.txt", String.Empty);
            HoughCircles_Profile.Items.Clear();
        }

        

        private void Save_HoughCircles_Click(object sender, EventArgs e)
        {
            houghCircles_status = true;



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
                HoughCirclesX = (int)circle.Center.X;
                HoughCirclesY = (int)circle.Center.Y;
                HoughCirclesradius = (int)circle.Radius;

            }

            HoughCircles.Image = circleImage;
            #endregion
            //IsAutoExposureTime.Checked = true;
            houghCircles_status = false;
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
            houghCircles_status = true;
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
                HoughCirclesX = (int)circle.Center.X;
                HoughCirclesY = (int)circle.Center.Y;
                HoughCirclesradius = (int)circle.Radius;




            }

            HoughCircles.Image = circleImage;
            #endregion
            houghCircles_status = false;
        }

        private void Clear_Profile_Click(object sender, EventArgs e)
        {
            File.WriteAllText(@"./HoughCircles_Profile.txt", String.Empty);
            HoughCircles_Profile.Items.Clear();
        }

       



        private void UITimer_Tick(object sender, EventArgs e)
        {
            //GC.Collect(2, GCCollectionMode.Forced);

            if (CameraStateText.Text == "ASI_EXP_FAILED")
            {


                /*if (ConnectedCameras > 0)
                {
                    Application.Restart();
                }*/
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
                    /*System.Diagnostics.Debug.WriteLine("ROIRec.Width == " + ROIRec.Width);
                    System.Diagnostics.Debug.WriteLine("ROIRec.Height == " + ROIRec.Height);*/
                    ROIFrame.ROI = ROIRec;
                    ROIImage.Image = ROIFrame.Convert<Gray, Byte>();

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


                    ROIImage.Image = ProcessFrameGray.Convert<Gray, Byte>();
                    MainImageControl.Image = ProcessFrame;




                    Image<Bgr, Byte> ImageFrame = ProcessFrame;




                    TimeNow = DateTime.Now.ToString("yyyy_MM_dd__HH_mm_ss");
                    TimeFolder = DateTime.Now.ToString("yyyy-MM-dd");
                    TimeNowChack = DateTime.Now.ToString("yyyy_MM_dd__HH_mm");
                    string TimesStamp = DateTime.Now.ToUniversalTime().ToString("MM/dd/yyyy hh:mm:ss tt");
                    int thickness = 5;
                    int borderHeight = Int32.Parse(AppSetting.Data.ImageHeight.ToString());

                    borderHeight = borderHeight - 300;
                    var borderTime = new Rectangle(0, borderHeight, 800, 100);
                    ImageFrame.Draw(borderTime, new Bgr(Color.Black), -1);
                    ImageFrame.Draw(borderTime, new Bgr(Color.White), 2);
                    CvInvoke.PutText(ImageFrame, "UTC " + TimesStamp, new Point(0, borderHeight + 50), FontFace.HersheySimplex, 1.5, new Bgr(Color.White).MCvScalar, thickness);

                    int borderWidth = Int32.Parse(AppSetting.Data.ImageWidth.ToString());
                    borderWidth = borderWidth - 600;

                    float ExposureTimeShow = float.Parse(AppSetting.Data.ExposureTime.ToString());

                    var borderExposureTime = new Rectangle(borderWidth, borderHeight, 300, 100);
                    ImageFrame.Draw(borderExposureTime, new Bgr(Color.Black), -1);
                    ImageFrame.Draw(borderExposureTime, new Bgr(Color.White), 2);
                    if (ExposureTimeShow < 1000 && ExposureTimeShow > 1)
                    {
                        CvInvoke.PutText(ImageFrame, Math.Round(AppSetting.Data.ExposureTime, 0) + " ms", new Point(borderWidth + 50, borderHeight + 50), FontFace.HersheySimplex, 1.5, new Bgr(Color.White).MCvScalar, thickness);
                    }
                    else if (ExposureTimeShow >= 1000)
                    {
                        CvInvoke.PutText(ImageFrame, Math.Round(AppSetting.Data.ExposureTime / 1000, 0) + " sec", new Point(borderWidth + 50, borderHeight + 50), FontFace.HersheySimplex, 1.5, new Bgr(Color.White).MCvScalar, thickness);
                    }
                    else if (ExposureTimeShow >= 0 && ExposureTimeShow <= 1)
                    {
                        CvInvoke.PutText(ImageFrame, Math.Round(AppSetting.Data.ExposureTime, 2) + " ms", new Point(borderWidth + 50, borderHeight + 50), FontFace.HersheySimplex, 1.5, new Bgr(Color.White).MCvScalar, thickness);
                    }

                    Bitmap BmpInput = ImageFrame.ToBitmap();




                    if (IsAutoExposureTime.CheckState != 0) //AutoExposureTime 
                    {

                        if (checkBoxCenter.CheckState != 0) //checkBoxFocus
                        {

                            checkBoxAverage.Checked = false;

                            CentroidX = (int)(CameraWidth / 2);
                            CentroidY = (int)(CameraHeight / 2);

                            for (double i = CentroidX - 10; i < CentroidX; i++)
                            {

                                for (double j = CentroidY - 10; j < CentroidY; j++)

                                {

                                    Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                                    if (pixel.R > 80 && pixel.B > 40 && pixel.G > 107)
                                    {
                                        System.Diagnostics.Debug.WriteLine("purple");
                                    }


                                    Colorall += (pixel.R + pixel.B + pixel.G) / 3;



                                }

                            }

                            Colorall = (Colorall / 100);
                            if (ShowFocusPoint.CheckState != 0)
                            {
                                CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidX.ToString()), Int32.Parse(CentroidY.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);
                            }



                        }
                        else
                        {

                            if (FocusPoint.Text == "21 Focus Points" && houghCircles_status == false)
                            {

                                checkBoxCenter.Checked = false;
                                checkBoxAverage.Checked = true;
                                //=======================================Center======================================== 
                                CentroidX = (int)(CameraWidth / 2);
                                CentroidY = (int)(CameraHeight / 2);
                                //CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidX.ToString()), Int32.Parse(CentroidY.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                //left 
                                double CentroidXleft = (int)(CameraWidth / 2.5);
                                double CentroidYleft = (int)(CameraHeight / 2);
                                //CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleft.ToString()), Int32.Parse(CentroidYleft.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                //left 
                                double CentroidXleftmore = (int)(CameraWidth / 3.2);
                                double CentroidYleftmore = (int)(CameraHeight / 2);


                                //right
                                double CentroidXright = (int)(CameraWidth / 1.7);
                                double CentroidYright = (int)(CameraHeight / 2);
                                //CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXright.ToString()), Int32.Parse(CentroidYright.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                //right
                                double CentroidXrightmore = (int)(CameraWidth / 1.5);
                                double CentroidYrightmore = (int)(CameraHeight / 2);
                                //CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXrightmore.ToString()), Int32.Parse(CentroidYrightmore.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                //=========================================UP======================================

                                double CentroidX_UP = (int)(CameraWidth / 2);
                                double CentroidY_UP = (int)(CameraHeight / 3);
                                //CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidX_UP.ToString()), Int32.Parse(CentroidY_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                //left 
                                double CentroidXleft_UP = (int)(CameraWidth / 2.5);
                                double CentroidYleft_UP = (int)(CameraHeight / 3);
                                //CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleft_UP.ToString()), Int32.Parse(CentroidYleft_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                //left 
                                double CentroidXleftmore_UP = (int)(CameraWidth / 3.2);
                                double CentroidYleftmore_UP = (int)(CameraHeight / 3);

                                //right
                                double CentroidXright_UP = (int)(CameraWidth / 1.7);
                                double CentroidYright_UP = (int)(CameraHeight / 3);
                                //CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXright_UP.ToString()), Int32.Parse(CentroidYright_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                //right
                                double CentroidXrightmore_UP = (int)(CameraWidth / 1.5);
                                double CentroidYrightmore_UP = (int)(CameraHeight / 3);
                                //CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXrightmore_UP.ToString()), Int32.Parse(CentroidYrightmore_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                //=======================================DOWN========================================
                                double CentroidX_DOWN = (int)(CameraWidth / 2);
                                double CentroidY_DOWN = (int)(CameraHeight / 1.5);
                                //CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidX_DOWN.ToString()), Int32.Parse(CentroidY_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                //left 
                                double CentroidXleft_DOWN = (int)(CameraWidth / 2.5);
                                double CentroidYleft_DOWN = (int)(CameraHeight / 1.5);
                                //CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleft_DOWN.ToString()), Int32.Parse(CentroidYleft_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                //left 
                                double CentroidXleftmore_DOWN = (int)(CameraWidth / 3.2);
                                double CentroidYleftmore_DOWN = (int)(CameraHeight / 1.5);

                                //right
                                double CentroidXright_DOWN = (int)(CameraWidth / 1.7);
                                double CentroidYright_DOWN = (int)(CameraHeight / 1.5);
                                //CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXright_DOWN.ToString()), Int32.Parse(CentroidYright_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                //right
                                double CentroidXrightmore_DOWN = (int)(CameraWidth / 1.5);
                                double CentroidYrightmore_DOWN = (int)(CameraHeight / 1.5);
                                //CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXrightmore_DOWN.ToString()), Int32.Parse(CentroidYrightmore_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);


                                //=========================================UP 3 DOT======================================

                                double CentroidX_UP2 = (int)(CameraWidth / 2);
                                double CentroidY_UP2 = (int)(CameraHeight / 6);
                                //CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidX_UP2.ToString()), Int32.Parse(CentroidY_UP2.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);


                                //right
                                double CentroidXright_UP2 = (int)(CameraWidth / 1.7);
                                double CentroidYright_UP2 = (int)(CameraHeight / 6);
                                //CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXright_UP2.ToString()), Int32.Parse(CentroidYright_UP2.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);
                                //left 
                                double CentroidXleft_UP2 = (int)(CameraWidth / 2.5);
                                double CentroidYleft_UP2 = (int)(CameraHeight / 6);



                                //=======================================DOWN 3 DOT========================================
                                double CentroidX_DOWN2 = (int)(CameraWidth / 2);
                                double CentroidY_DOWN2 = (int)(CameraHeight / 1.2);
                                //CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidX_DOWN2.ToString()), Int32.Parse(CentroidY_DOWN2.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);


                                //right
                                double CentroidXright_DOWN2 = (int)(CameraWidth / 1.7);
                                double CentroidYright_DOWN2 = (int)(CameraHeight / 1.2);
                                //CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXright_DOWN2.ToString()), Int32.Parse(CentroidYright_DOWN2.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                //left 
                                double CentroidXleft_DOWN2 = (int)(CameraWidth / 2.5);
                                double CentroidYleft_DOWN2 = (int)(CameraHeight / 1.2);

                                if (ShowFocusPoint.CheckState != 0)
                                {


                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidX.ToString()), Int32.Parse(CentroidY.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //left 

                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleft.ToString()), Int32.Parse(CentroidYleft.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //left 

                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleftmore.ToString()), Int32.Parse(CentroidYleftmore.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //left 

                                    //CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleftmore2.ToString()), Int32.Parse(CentroidYleftmore2.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //right

                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXright.ToString()), Int32.Parse(CentroidYright.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //right

                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXrightmore.ToString()), Int32.Parse(CentroidYrightmore.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //=========================================UP======================================


                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidX_UP.ToString()), Int32.Parse(CentroidY_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //left 

                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleft_UP.ToString()), Int32.Parse(CentroidYleft_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //left 

                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleftmore_UP.ToString()), Int32.Parse(CentroidYleftmore_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //right

                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXright_UP.ToString()), Int32.Parse(CentroidYright_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //right

                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXrightmore_UP.ToString()), Int32.Parse(CentroidYrightmore_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //=======================================DOWN========================================

                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidX_DOWN.ToString()), Int32.Parse(CentroidY_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //left 

                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleft_DOWN.ToString()), Int32.Parse(CentroidYleft_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //left 

                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleftmore_DOWN.ToString()), Int32.Parse(CentroidYleftmore_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //right

                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXright_DOWN.ToString()), Int32.Parse(CentroidYright_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //right

                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXrightmore_DOWN.ToString()), Int32.Parse(CentroidYrightmore_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);


                                    //=========================================UP 3 DOT======================================


                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidX_UP2.ToString()), Int32.Parse(CentroidY_UP2.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);


                                    //right

                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXright_UP2.ToString()), Int32.Parse(CentroidYright_UP2.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleft_UP2.ToString()), Int32.Parse(CentroidYleft_UP2.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);



                                    //=======================================DOWN 3 DOT========================================

                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidX_DOWN2.ToString()), Int32.Parse(CentroidY_DOWN2.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);



                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleft_DOWN2.ToString()), Int32.Parse(CentroidYleft_DOWN2.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //right

                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXright_DOWN2.ToString()), Int32.Parse(CentroidYright_DOWN2.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);


                                }

                                //==================Readlight Center
                                for (double i = CentroidX - 10; i < CentroidX; i++)
                                {

                                    for (double j = CentroidY - 10; j < CentroidY; j++)

                                    {
                                        if (CentroidX > CentroidY)
                                        {
                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;

                                        }

                                    }


                                }

                                for (double i = CentroidXleftmore - 10; i < CentroidXleftmore; i++)
                                {

                                    for (double j = CentroidYleftmore - 10; j < CentroidYleftmore; j++)

                                    {
                                        if (CentroidXleftmore > CentroidYleftmore)
                                        {
                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;
                                        }

                                    }

                                }

                                for (double i = CentroidXleft - 10; i < CentroidXleft; i++)
                                {

                                    for (double j = CentroidYleft - 10; j < CentroidYleft; j++)

                                    {
                                        if (CentroidXleft > CentroidYleft)
                                        {
                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));

                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;

                                        }

                                    }

                                }



                                for (double i = CentroidXright - 10; i < CentroidXright; i++)
                                {

                                    for (double j = CentroidYright - 10; j < CentroidYright; j++)

                                    {
                                        if (CentroidXright > CentroidYright)
                                        {
                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));

                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;

                                        }

                                    }

                                }

                                for (double i = CentroidXrightmore - 10; i < CentroidXrightmore; i++)
                                {

                                    for (double j = CentroidYrightmore - 10; j < CentroidYrightmore; j++)

                                    {
                                        if (CentroidXrightmore > CentroidYrightmore)
                                        {
                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;

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
                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));

                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;

                                        }

                                    }

                                }

                                for (double i = CentroidXleft_UP - 10; i < CentroidXleft_UP; i++)
                                {

                                    for (double j = CentroidYleft_UP - 10; j < CentroidYleft_UP; j++)

                                    {
                                        if (CentroidXleft_UP > CentroidYleft_UP)
                                        {
                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;
                                        }

                                    }

                                }

                                for (double i = CentroidXleftmore_UP - 10; i < CentroidXleftmore_UP; i++)
                                {

                                    for (double j = CentroidYleftmore_UP - 10; j < CentroidYleftmore_UP; j++)

                                    {
                                        if (CentroidXleftmore_UP > CentroidYleftmore_UP)
                                        {
                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;

                                        }

                                    }

                                }



                                for (double i = CentroidXright_UP - 10; i < CentroidXright_UP; i++)
                                {

                                    for (double j = CentroidYright_UP - 10; j < CentroidYright_UP; j++)

                                    {
                                        if (CentroidXright_UP > CentroidYright_UP)
                                        {
                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));

                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;

                                        }

                                    }

                                }

                                for (double i = CentroidXrightmore_UP - 10; i < CentroidXrightmore_UP; i++)
                                {

                                    for (double j = CentroidYrightmore_UP - 10; j < CentroidYrightmore_UP; j++)

                                    {
                                        if (CentroidXrightmore_UP > CentroidYrightmore_UP)
                                        {
                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));

                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;

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
                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;

                                        }

                                    }

                                }

                                for (double i = CentroidXleft_DOWN - 10; i < CentroidXleft_DOWN; i++)
                                {

                                    for (double j = CentroidYleft_DOWN - 10; j < CentroidYleft_DOWN; j++)

                                    {




                                        if (CentroidXleft_DOWN > CentroidYleft_DOWN)
                                        {
                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;

                                        }

                                    }

                                }

                                for (double i = CentroidXleftmore_DOWN - 10; i < CentroidXleftmore_DOWN; i++)
                                {

                                    for (double j = CentroidYleftmore_DOWN - 10; j < CentroidYleftmore_DOWN; j++)

                                    {
                                        if (CentroidXleftmore_DOWN > CentroidYleftmore_DOWN)
                                        {
                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));

                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;

                                        }

                                    }

                                }


                                for (double i = CentroidXright_DOWN - 10; i < CentroidXright_DOWN; i++)
                                {

                                    for (double j = CentroidYright_DOWN - 10; j < CentroidYright_DOWN; j++)

                                    {
                                        if (CentroidXright_DOWN > CentroidYright_DOWN)
                                        {
                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;

                                        }

                                    }

                                }

                                for (double i = CentroidXrightmore_DOWN - 10; i < CentroidXrightmore_DOWN; i++)
                                {

                                    for (double j = CentroidYrightmore_DOWN - 10; j < CentroidYrightmore_DOWN; j++)

                                    {
                                        if (CentroidXrightmore_DOWN > CentroidYrightmore_DOWN)
                                        {
                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));

                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;

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
                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));

                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;

                                        }

                                    }

                                }

                                for (double i = CentroidXright_UP2 - 10; i < CentroidXright_UP2; i++)
                                {

                                    for (double j = CentroidYright_UP2 - 10; j < CentroidYright_UP2; j++)

                                    {
                                        if (CentroidXright_UP2 > CentroidYright_UP2)
                                        {
                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));

                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;
                                        }

                                    }

                                }

                                for (double i = CentroidXleft_UP2 - 10; i < CentroidXleft_UP2; i++)
                                {

                                    for (double j = CentroidYleft_UP2 - 10; j < CentroidYleft_UP2; j++)

                                    {
                                        if (CentroidXleft_UP2 > CentroidYleft_UP2)
                                        {
                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));

                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;

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
                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));

                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;
                                        }


                                    }

                                }

                                for (double i = CentroidXright_DOWN2 - 10; i < CentroidXright_DOWN2; i++)
                                {

                                    for (double j = CentroidYright_DOWN2 - 10; j < CentroidYright_DOWN2; j++)

                                    {
                                        if (CentroidXright_DOWN2 > CentroidYright_DOWN2)
                                        {
                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;

                                        }

                                    }

                                }

                                for (double i = CentroidXleft_DOWN2 - 10; i < CentroidXleft_DOWN2; i++)
                                {

                                    for (double j = CentroidYleft_DOWN2 - 10; j < CentroidYleft_DOWN2; j++)

                                    {
                                        if (CentroidXleft_DOWN2 > CentroidYleft_DOWN2)
                                        {
                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;

                                        }

                                    }

                                }
                                Colorall = (Colorall / 2100);
                                //Recover = false;
                            }
                            else if (FocusPoint.Text == "15 Focus Points")
                            {
                                checkBoxCenter.Checked = false;
                                checkBoxAverage.Checked = true;
                                //=======================================Center======================================== 
                                CentroidX = (int)(CameraWidth / 2);
                                CentroidY = (int)(CameraHeight / 2);
                                //CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidX.ToString()), Int32.Parse(CentroidY.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                //left 
                                double CentroidXleft = (int)(CameraWidth / 2.5);
                                double CentroidYleft = (int)(CameraHeight / 2);
                                //CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleft.ToString()), Int32.Parse(CentroidYleft.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                //left 
                                double CentroidXleftmore = (int)(CameraWidth / 3.2);
                                double CentroidYleftmore = (int)(CameraHeight / 2);


                                //right
                                double CentroidXright = (int)(CameraWidth / 1.7);
                                double CentroidYright = (int)(CameraHeight / 2);
                                //CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXright.ToString()), Int32.Parse(CentroidYright.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                //right
                                double CentroidXrightmore = (int)(CameraWidth / 1.5);
                                double CentroidYrightmore = (int)(CameraHeight / 2);
                                //CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXrightmore.ToString()), Int32.Parse(CentroidYrightmore.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                //=========================================UP======================================

                                double CentroidX_UP = (int)(CameraWidth / 2);
                                double CentroidY_UP = (int)(CameraHeight / 3);
                                //CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidX_UP.ToString()), Int32.Parse(CentroidY_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                //left 
                                double CentroidXleft_UP = (int)(CameraWidth / 2.5);
                                double CentroidYleft_UP = (int)(CameraHeight / 3);
                                //CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleft_UP.ToString()), Int32.Parse(CentroidYleft_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                //left 
                                double CentroidXleftmore_UP = (int)(CameraWidth / 3.2);
                                double CentroidYleftmore_UP = (int)(CameraHeight / 3);

                                //right
                                double CentroidXright_UP = (int)(CameraWidth / 1.7);
                                double CentroidYright_UP = (int)(CameraHeight / 3);
                                //CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXright_UP.ToString()), Int32.Parse(CentroidYright_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                //right
                                double CentroidXrightmore_UP = (int)(CameraWidth / 1.5);
                                double CentroidYrightmore_UP = (int)(CameraHeight / 3);
                                //CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXrightmore_UP.ToString()), Int32.Parse(CentroidYrightmore_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                //=======================================DOWN========================================
                                double CentroidX_DOWN = (int)(CameraWidth / 2);
                                double CentroidY_DOWN = (int)(CameraHeight / 1.5);
                                //CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidX_DOWN.ToString()), Int32.Parse(CentroidY_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                //left 
                                double CentroidXleft_DOWN = (int)(CameraWidth / 2.5);
                                double CentroidYleft_DOWN = (int)(CameraHeight / 1.5);
                                //CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleft_DOWN.ToString()), Int32.Parse(CentroidYleft_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                //left 
                                double CentroidXleftmore_DOWN = (int)(CameraWidth / 3.2);
                                double CentroidYleftmore_DOWN = (int)(CameraHeight / 1.5);

                                //right
                                double CentroidXright_DOWN = (int)(CameraWidth / 1.7);
                                double CentroidYright_DOWN = (int)(CameraHeight / 1.5);
                                //CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXright_DOWN.ToString()), Int32.Parse(CentroidYright_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                //right
                                double CentroidXrightmore_DOWN = (int)(CameraWidth / 1.5);
                                double CentroidYrightmore_DOWN = (int)(CameraHeight / 1.5);
                                //CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXrightmore_DOWN.ToString()), Int32.Parse(CentroidYrightmore_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);




                                if (ShowFocusPoint.CheckState != 0)
                                {


                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidX.ToString()), Int32.Parse(CentroidY.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //left 

                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleft.ToString()), Int32.Parse(CentroidYleft.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //left 

                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleftmore.ToString()), Int32.Parse(CentroidYleftmore.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //left 

                                    //CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleftmore2.ToString()), Int32.Parse(CentroidYleftmore2.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //right

                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXright.ToString()), Int32.Parse(CentroidYright.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //right

                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXrightmore.ToString()), Int32.Parse(CentroidYrightmore.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //=========================================UP======================================


                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidX_UP.ToString()), Int32.Parse(CentroidY_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //left 

                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleft_UP.ToString()), Int32.Parse(CentroidYleft_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //left 

                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleftmore_UP.ToString()), Int32.Parse(CentroidYleftmore_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //right

                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXright_UP.ToString()), Int32.Parse(CentroidYright_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //right

                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXrightmore_UP.ToString()), Int32.Parse(CentroidYrightmore_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //=======================================DOWN========================================

                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidX_DOWN.ToString()), Int32.Parse(CentroidY_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //left 

                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleft_DOWN.ToString()), Int32.Parse(CentroidYleft_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //left 

                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleftmore_DOWN.ToString()), Int32.Parse(CentroidYleftmore_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //right

                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXright_DOWN.ToString()), Int32.Parse(CentroidYright_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //right

                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXrightmore_DOWN.ToString()), Int32.Parse(CentroidYrightmore_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);




                                }

                                //==================Readlight Center
                                for (double i = CentroidX - 10; i < CentroidX; i++)
                                {

                                    for (double j = CentroidY - 10; j < CentroidY; j++)

                                    {
                                        if (CentroidX > CentroidY)
                                        {

                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;
                                        }


                                    }

                                }

                                for (double i = CentroidXleftmore - 10; i < CentroidXleftmore; i++)
                                {

                                    for (double j = CentroidYleftmore - 10; j < CentroidYleftmore; j++)

                                    {
                                        if (CentroidXleftmore > CentroidYleftmore)
                                        {

                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;

                                        }


                                    }

                                }

                                for (double i = CentroidXleft - 10; i < CentroidXleft; i++)
                                {

                                    for (double j = CentroidYleft - 10; j < CentroidYleft; j++)

                                    {
                                        if (CentroidXleft > CentroidYleft)
                                        {
                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;

                                        }

                                    }

                                }



                                for (double i = CentroidXright - 10; i < CentroidXright; i++)
                                {

                                    for (double j = CentroidYright - 10; j < CentroidYright; j++)

                                    {
                                        if (CentroidXright > CentroidYright)
                                        {

                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;
                                        }


                                    }

                                }

                                for (double i = CentroidXrightmore - 10; i < CentroidXrightmore; i++)
                                {

                                    for (double j = CentroidYrightmore - 10; j < CentroidYrightmore; j++)

                                    {
                                        if (CentroidXrightmore > CentroidYrightmore)
                                        {
                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;
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
                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;
                                        }


                                    }

                                }

                                for (double i = CentroidXleft_UP - 10; i < CentroidXleft_UP; i++)
                                {

                                    for (double j = CentroidYleft_UP - 10; j < CentroidYleft_UP; j++)

                                    {
                                        if (CentroidXleft_UP > CentroidYleft_UP)
                                        {
                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;
                                        }


                                    }

                                }

                                for (double i = CentroidXleftmore_UP - 10; i < CentroidXleftmore_UP; i++)
                                {

                                    for (double j = CentroidYleftmore_UP - 10; j < CentroidYleftmore_UP; j++)

                                    {
                                        if (CentroidXleftmore_UP > CentroidYleftmore_UP)
                                        {
                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;
                                        }


                                    }

                                }



                                for (double i = CentroidXright_UP - 10; i < CentroidXright_UP; i++)
                                {

                                    for (double j = CentroidYright_UP - 10; j < CentroidYright_UP; j++)

                                    {
                                        if (CentroidXright_UP > CentroidYright_UP)
                                        {
                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;

                                        }

                                    }

                                }

                                for (double i = CentroidXrightmore_UP - 10; i < CentroidXrightmore_UP; i++)
                                {

                                    for (double j = CentroidYrightmore_UP - 10; j < CentroidYrightmore_UP; j++)

                                    {
                                        if (CentroidXrightmore_UP > CentroidYrightmore_UP)
                                        {

                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;
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
                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;
                                        }


                                    }

                                }

                                for (double i = CentroidXleft_DOWN - 10; i < CentroidXleft_DOWN; i++)
                                {

                                    for (double j = CentroidYleft_DOWN - 10; j < CentroidYleft_DOWN; j++)

                                    {
                                        if (CentroidXleft_DOWN > CentroidYleft_DOWN)
                                        {
                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;
                                        }


                                    }

                                }

                                for (double i = CentroidXleftmore_DOWN - 10; i < CentroidXleftmore_DOWN; i++)
                                {

                                    for (double j = CentroidYleftmore_DOWN - 10; j < CentroidYleftmore_DOWN; j++)

                                    {
                                        if (CentroidXleftmore_DOWN > CentroidYleftmore_DOWN)
                                        {
                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;
                                        }


                                    }

                                }


                                for (double i = CentroidXright_DOWN - 10; i < CentroidXright_DOWN; i++)
                                {

                                    for (double j = CentroidYright_DOWN - 10; j < CentroidYright_DOWN; j++)

                                    {
                                        if (CentroidXright_DOWN > CentroidYright_DOWN)
                                        {
                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;
                                        }


                                    }

                                }

                                for (double i = CentroidXrightmore_DOWN - 10; i < CentroidXrightmore_DOWN; i++)
                                {

                                    for (double j = CentroidYrightmore_DOWN - 10; j < CentroidYrightmore_DOWN; j++)

                                    {
                                        if (CentroidXrightmore_DOWN > CentroidYrightmore_DOWN)
                                        {
                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;
                                        }


                                    }

                                }


                                Colorall = (Colorall / 1500);
                            }
                            else if (FocusPoint.Text == "9 Focus Points")
                            {
                                checkBoxCenter.Checked = false;
                                checkBoxAverage.Checked = true;
                                //=======================================Center======================================== 
                                CentroidX = (int)(CameraWidth / 2);
                                CentroidY = (int)(CameraHeight / 2);
                                //CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidX.ToString()), Int32.Parse(CentroidY.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);


                                //left 
                                double CentroidXleftmore = (int)(CameraWidth / 3.2);
                                double CentroidYleftmore = (int)(CameraHeight / 2);



                                //right
                                double CentroidXrightmore = (int)(CameraWidth / 1.5);
                                double CentroidYrightmore = (int)(CameraHeight / 2);
                                //CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXrightmore.ToString()), Int32.Parse(CentroidYrightmore.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                //=========================================UP======================================

                                double CentroidX_UP = (int)(CameraWidth / 2);
                                double CentroidY_UP = (int)(CameraHeight / 3);
                                //CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidX_UP.ToString()), Int32.Parse(CentroidY_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);


                                //left 
                                double CentroidXleftmore_UP = (int)(CameraWidth / 3.2);
                                double CentroidYleftmore_UP = (int)(CameraHeight / 3);


                                //right
                                double CentroidXrightmore_UP = (int)(CameraWidth / 1.5);
                                double CentroidYrightmore_UP = (int)(CameraHeight / 3);
                                //CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXrightmore_UP.ToString()), Int32.Parse(CentroidYrightmore_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                //=======================================DOWN========================================
                                double CentroidX_DOWN = (int)(CameraWidth / 2);
                                double CentroidY_DOWN = (int)(CameraHeight / 1.5);
                                //CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidX_DOWN.ToString()), Int32.Parse(CentroidY_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);


                                //left 
                                double CentroidXleftmore_DOWN = (int)(CameraWidth / 3.2);
                                double CentroidYleftmore_DOWN = (int)(CameraHeight / 1.5);


                                //right
                                double CentroidXrightmore_DOWN = (int)(CameraWidth / 1.5);
                                double CentroidYrightmore_DOWN = (int)(CameraHeight / 1.5);
                                //CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXrightmore_DOWN.ToString()), Int32.Parse(CentroidYrightmore_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);




                                if (ShowFocusPoint.CheckState != 0)
                                {


                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidX.ToString()), Int32.Parse(CentroidY.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //left 

                                    //CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleft.ToString()), Int32.Parse(CentroidYleft.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //left 

                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleftmore.ToString()), Int32.Parse(CentroidYleftmore.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //left 

                                    //CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleftmore2.ToString()), Int32.Parse(CentroidYleftmore2.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //right

                                    //CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXright.ToString()), Int32.Parse(CentroidYright.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //right

                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXrightmore.ToString()), Int32.Parse(CentroidYrightmore.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //=========================================UP======================================


                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidX_UP.ToString()), Int32.Parse(CentroidY_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //left 

                                    //CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleft_UP.ToString()), Int32.Parse(CentroidYleft_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //left 

                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleftmore_UP.ToString()), Int32.Parse(CentroidYleftmore_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //right

                                    // CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXright_UP.ToString()), Int32.Parse(CentroidYright_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //right

                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXrightmore_UP.ToString()), Int32.Parse(CentroidYrightmore_UP.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //=======================================DOWN========================================

                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidX_DOWN.ToString()), Int32.Parse(CentroidY_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //left 

                                    //CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleft_DOWN.ToString()), Int32.Parse(CentroidYleft_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //left 

                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXleftmore_DOWN.ToString()), Int32.Parse(CentroidYleftmore_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //right

                                    //CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXright_DOWN.ToString()), Int32.Parse(CentroidYright_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

                                    //right

                                    CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidXrightmore_DOWN.ToString()), Int32.Parse(CentroidYrightmore_DOWN.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);




                                }

                                //==================Readlight Center
                                for (double i = CentroidX - 10; i < CentroidX; i++)
                                {

                                    for (double j = CentroidY - 10; j < CentroidY; j++)

                                    {
                                        if (CentroidX > CentroidY)
                                        {
                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;
                                        }


                                    }

                                }

                                for (double i = CentroidXleftmore - 10; i < CentroidXleftmore; i++)
                                {

                                    for (double j = CentroidYleftmore - 10; j < CentroidYleftmore; j++)

                                    {
                                        if (CentroidXleftmore > CentroidYleftmore)
                                        {
                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;
                                        }



                                    }

                                }







                                for (double i = CentroidXrightmore - 10; i < CentroidXrightmore; i++)
                                {

                                    for (double j = CentroidYrightmore - 10; j < CentroidYrightmore; j++)

                                    {
                                        if (CentroidXrightmore > CentroidYrightmore)
                                        {
                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;
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
                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;
                                        }


                                    }

                                }



                                for (double i = CentroidXleftmore_UP - 10; i < CentroidXleftmore_UP; i++)
                                {

                                    for (double j = CentroidYleftmore_UP - 10; j < CentroidYleftmore_UP; j++)

                                    {
                                        if (CentroidXleftmore_UP > CentroidYleftmore_UP)
                                        {
                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;
                                        }


                                    }

                                }





                                for (double i = CentroidXrightmore_UP - 10; i < CentroidXrightmore_UP; i++)
                                {

                                    for (double j = CentroidYrightmore_UP - 10; j < CentroidYrightmore_UP; j++)

                                    {
                                        if (CentroidXrightmore_UP > CentroidYrightmore_UP)
                                        {
                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;

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
                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;
                                        }


                                    }

                                }



                                for (double i = CentroidXleftmore_DOWN - 10; i < CentroidXleftmore_DOWN; i++)
                                {

                                    for (double j = CentroidYleftmore_DOWN - 10; j < CentroidYleftmore_DOWN; j++)

                                    {
                                        if (CentroidXleftmore_DOWN > CentroidYleftmore_DOWN)
                                        {
                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;

                                        }

                                    }

                                }




                                for (double i = CentroidXrightmore_DOWN - 10; i < CentroidXrightmore_DOWN; i++)
                                {

                                    for (double j = CentroidYrightmore_DOWN - 10; j < CentroidYrightmore_DOWN; j++)

                                    {
                                        if (CentroidXrightmore_DOWN > CentroidYrightmore_DOWN)
                                        {
                                            Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));

                                            Colorall += (pixel.R + pixel.B + pixel.G) / 3;
                                        }


                                    }

                                }


                                Colorall = (Colorall / 900);
                            }
                            else if (FocusPoint.Text == "Histogram" && Histogramcheck.Checked == false)
                            {
                                FocusPoint.Text = "21 Focus Points";
                                Histogramcheck.Checked = false;
                                MessageBox.Show("Please turn on Histogram.", "Message", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                            }



                            ExposuringText.Text = string.Format("{0:0.00}", Math.Log(Math.Pow(2.8, 2) / (AppSetting.Data.ExposureTime / 1000), 2.0));

                            if (Histogramcheck.CheckState != 0 && HoughCirclesradius != 0)
                            {

                                int ColorHisto = 0;
                                Image<Bgr, Byte> HistoImage = houghCirclesFrame;
                                HistoImage.ROI = new Rectangle((int)CentroidX - HoughCirclesradius * 7, (int)CentroidY - HoughCirclesradius * 7, HoughCirclesradius * 7 * 2, HoughCirclesradius * 7 * 2);
                                Image<Bgr, Byte> cropped_im = HistoImage.Copy();
                                //Bitmap HistoImageBit = cropped_im.ToBitmap();
                                //ROIImage.Image = cropped_im.Convert<Gray, Byte>();
                                /*for (double i = cropped_im.Width; i < cropped_im.Width; i++)
                                {

                                    for (double j = cropped_im.Height ; j < cropped_im.Height; j++)

                                    {
                                        if (houghCircles_status == false)
                                        {
                                            Color pixel = HistoImageBit.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));

                                            ColorHisto += (pixel.R + pixel.B + pixel.G) / 3;
                                        }


                                    }

                                }*/


                                Histo.ClearHistogram();
                                Histo.GenerateHistograms(cropped_im.Convert<Gray, Byte>(), 256);
                                //ColorHistoShow = ColorHisto / (cropped_im.Height * cropped_im.Width);




                                if (FocusPoint.Text == "Histogram")
                                {

                                    for (int i = 0; i < cropped_im.Width; i++)
                                    {

                                        for (int j = 0; j < cropped_im.Height; j++)

                                        {
                                            ColorHisto += cropped_im.Data[i, j, 0];




                                        }
                                        //Recover = false;
                                    }

                                    ColorHisto = ColorHisto / (cropped_im.Width * cropped_im.Height);
                                    Console.WriteLine(ColorHisto);

                                    Colorall = ColorHisto;

                                }
                                /*if (FocusPoint.Text == "Histogram" && Histogramcheck.CheckState != 0 && HoughCirclesradius != 0)
                                {
                                    Colorall = ColorHistoShow;

                                }*/

                                Histo.Refresh();

                            }
                            else if (Histogramcheck.Checked == true && HoughCirclesradius == 0)
                            {
                                Histogramcheck.Checked = false;
                                MessageBox.Show("Please turn on HoughCircles.", "Message", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);


                            }



                            double bestValue;
                            double Control_Jump = 1;





                            if (hdr_On.CheckState != 0)
                            {
                                bool doOne;
                                doOne = false;
                                int imageMerge_status = 0;


                                if (Colorall <= 150 && Colorall > 80)
                                {
                                    Control_Jump = 2;
                                }

                                else if (Colorall > 150 || Colorall < 100)
                                {
                                    Control_Jump = 1;
                                }


                                if (Colorall > maxLight_hdr && CameraStateText.Text != "ASI_EXP_WORKING" && Recover != false)
                                {


                                    bestValue = ((AppSetting.Data.ExposureTime / golden_ratio) * (Control_Jump));
                                    Console.WriteLine("bestValue" + bestValue);
                                    AppSetting.Data.ExposureTime = Math.Round(bestValue, 2);
                                    Recover = false;
                                    //AppSetting.Data.ExposureTime -
                                }
                                else if (Colorall < minLight_hdr && CameraStateText.Text != "ASI_EXP_WORKING" && Recover != false && AppSetting.Data.ExposureTime <= 120000)
                                {


                                    bestValue = ((AppSetting.Data.ExposureTime * golden_ratio) / (Control_Jump));
                                    Console.WriteLine("bestValue" + bestValue);
                                    AppSetting.Data.ExposureTime = Math.Round(bestValue, 2);
                                    Recover = false;
                                    //AppSetting.Data.ExposureTime +
                                }
                                else if (Colorall >= minLight_hdr && Colorall <= maxLight_hdr && doOne == false)
                                {


                                    if (maxLight_hdr == 255 && minLight_hdr == 200)
                                    {
                                        hdrHigh = RootFrame;


                                        maxLight_hdr = 50;
                                        minLight_hdr = 10;
                                        doOne = true;
                                        imageMerge_status = 1;
                                    }
                                    else if (maxLight_hdr == 50 && minLight_hdr == 10 && doOne == false)
                                    {
                                        hdrLow = RootFrame;



                                        maxLight_hdr = 125;
                                        minLight_hdr = 100;
                                        doOne = true;
                                        imageMerge_status = 2;
                                    }
                                    else if (maxLight_hdr == 125 && minLight_hdr == 100 && doOne == false)
                                    {
                                        hdrMedium = RootFrame;


                                        maxLight_hdr = 255;
                                        minLight_hdr = 200;
                                        doOne = true;
                                        imageMerge_status = 3;
                                    }
                                    if (imageMerge_status == 3)
                                    {
                                        imageMerge_status = 0;

                                        //HoughCircles.Image = hdrMedium + hdrLow + hdrHigh;


                                    }





                                }

                            }
                            else
                            {



                                // ExposureTime Not HDR

                                /*for (int i = 0; i < 2; i++)
                                {*/
                                if (AppSetting.Data.ExposureTime >= 1)
                                {
                                    min_light = AppSetting.Data.min_light;
                                    max_light = AppSetting.Data.max_light;

                                    /*if (Colorall > min_light - 60 && Colorall <= max_light + 60)
                                    {
                                        Control_Jump = 1.5;
                                    }*/
                                    if (Colorall > min_light - 30 && Colorall <= max_light + 30)
                                    {
                                        Control_Jump = 2;
                                    }
                                    if (Colorall > min_light - 10 && Colorall <= max_light + 10)
                                    {
                                        Control_Jump = 2.1;
                                    }
                                    if (Colorall > min_light - 5 && Colorall <= max_light + 5)
                                    {
                                        Control_Jump = 2.11;
                                    }
                                    if (Colorall < min_light - 40 || Colorall > max_light + 40)
                                    {
                                        Control_Jump = 1;
                                    }



                                }
                                else
                                {
                                    max_light = 80;
                                    min_light = 40;
                                    if (Colorall > min_light - 40 && Colorall <= max_light + 40)
                                    {
                                        Control_Jump = 2;
                                    }
                                    if (Colorall > min_light - 10 && Colorall <= max_light + 10)
                                    {
                                        Control_Jump = 2.1;
                                    }
                                    if (Colorall > min_light - 5 && Colorall <= max_light + 5)
                                    {
                                        Control_Jump = 2.11;
                                    }
                                    if (Colorall < min_light - 10 || Colorall > max_light + 40)
                                    {
                                        Control_Jump = 1;
                                    }

                                }


                                if (Colorall >= max_light && CameraStateText.Text != "ASI_EXP_WORKING" && Recover != false)
                                {

                                    //AppSetting.Data.ExposureTime = ExposureTimeAverage;
                                    bestValue = ((AppSetting.Data.ExposureTime / golden_ratio) * (Control_Jump));
                                    Console.WriteLine("bestValue" + bestValue);
                                    AppSetting.Data.ExposureTime = Math.Round(bestValue, 2);
                                    Recover = false;
                                    //AppSetting.Data.ExposureTime -
                                }
                                else if (Colorall <= min_light && CameraStateText.Text != "ASI_EXP_WORKING" && Recover != false && AppSetting.Data.ExposureTime < 120000)
                                {
                                    if (AppSetting.Data.ExposureTime == 0)
                                    {
                                        AppSetting.Data.ExposureTime = 0.1;
                                    }

                                    //AppSetting.Data.ExposureTime = ExposureTimeAverage;
                                    bestValue = ((AppSetting.Data.ExposureTime * golden_ratio) / (Control_Jump));
                                    Console.WriteLine("bestValue" + bestValue);
                                    AppSetting.Data.ExposureTime = Math.Round(bestValue, 2);
                                    Recover = false;
                                    //AppSetting.Data.ExposureTime +
                                }
                                else if (AppSetting.Data.ExposureTime > 120000)
                                {
                                    AppSetting.Data.ExposureTime = 120000;
                                }



                            }

                            //double ExposureTimeAverage = (ExposureTimeMin + ExposureTimeMax) / 2;


                            ExpouseTimeText.Value = (decimal)AppSetting.Data.ExposureTime;

                            keoGramsImage = new Rectangle(new Point(CameraWidth / 2, CameraHeight / 2), new Size(1, CameraHeight));
                            keoGramsFrame.ROI = keoGramsImage;
                            Bitmap outputImage = keoGramsFrame.ToBitmap();



                            if (keoGrams.Checked == true)
                            {
                                string timeMinutes = DateTime.Now.AddMinutes(-1).ToString("yyyy_MM_dd__HH_mm");
                                Directory.CreateDirectory(AppSetting.Data.SaveFileDialog + @"\keoGrams" + @"\" + TimeFolder);

                                Bitmap keogramsImage = keoGramsFrame.ToBitmap();


                                string pathKeograms = AppSetting.Data.SaveFileDialog + @"\keoGrams" + @"\" + TimeFolder + @"\" + timeMinutes + "keoGrams" + ".jpg";
                                Console.WriteLine(pathKeograms);
                                int Minutesinday = (int)DateTime.Now.TimeOfDay.TotalMinutes;


                                if (File.Exists(pathKeograms))
                                {
                                    keogramsImage = new Bitmap(pathKeograms);
                                    Console.WriteLine("keogramsImage1");
                                }
                                else if (File.Exists(AppSetting.Data.SaveFileDialog + @"\keoGrams" + @"\" + TimeFolder + @"\" + "keoGrams" + ".jpg") && !File.Exists(pathKeograms))
                                {
                                    keogramsImage = new Bitmap(AppSetting.Data.SaveFileDialog + @"\keoGrams" + @"\" + TimeFolder + @"\" + "keoGrams" + ".jpg");
                                    Console.WriteLine("keogramsImage2");
                                }

                                Bitmap keograms = keoGramsFrame.ToBitmap();



                                if (!File.Exists(AppSetting.Data.SaveFileDialog + @"\keoGrams" + @"\" + TimeFolder + @"\" + "keoGrams" + ".jpg"))
                                {

                                    ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                                    System.Drawing.Imaging.Encoder myEncoder =
                                            System.Drawing.Imaging.Encoder.Quality;
                                    EncoderParameters myEncoderParameters = new EncoderParameters(1);
                                    EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder,
                                    50L);


                                    myEncoderParameters.Param[0] = myEncoderParameter;

                                    var bitmap = new Bitmap(Minutesinday, keogramsImage.Height);

                                    for (var x = 0; x < bitmap.Width; x++)
                                    {
                                        for (var y = 0; y < bitmap.Height; y++)
                                        {

                                            bitmap.SetPixel(x, y, Color.Black);

                                        }
                                    }

                                    bitmap.Save(AppSetting.Data.SaveFileDialog + @"\keoGrams" + @"\" + TimeFolder + @"\" + "keoGrams" + ".jpg", jpgEncoder,
                                    myEncoderParameters);
                                    Console.WriteLine("keogramsImagebitmap1");

                                }
                                /*else if (!File.Exists(pathKeograms))
                                {
                                    Console.WriteLine("keogramsImagebitmap2");
                                    var directory = new DirectoryInfo(AppSetting.Data.SaveFileDialog + @"\keoGrams" + @"\" + TimeFolder);
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

                                    var bitmap = new Bitmap(AppSetting.Data.SaveFileDialog + @"\keoGrams" + @"\" + TimeFolder + @"\" + myFile.ToString());

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

                            if (AppSetting.Data.SaveFileDialog != "" && TimeNowChack != TimeBefore
                                && Colorall <= 150
                                && ConnectedCameras > 0 && hdr_On.CheckState == 0)
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

                                BmpInput.Save(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\" + TimeFolder + @"\" + TimeNow + ".jpg", jpgEncoder,
                                    myEncoderParameters);
                                if (keoGrams.CheckState != 0)
                                {
                                    //File.Delete(AppSetting.Data.SaveFileDialog + @"\keoGrams" + @"\" + TimeFolder + @"\" + "keoGrams" + ".jpg");
                                    outputImage.Save(AppSetting.Data.SaveFileDialog + @"\keoGrams" + @"\" + TimeFolder + @"\" + DateTime.Now.ToString("yyyy_MM_dd__HH_mm") + "keoGrams" + ".jpg", jpgEncoder,
                                    myEncoderParameters);
                                }


                                if (SaveLog.CheckState != 0)
                                {
                                    File.AppendAllText(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\log " + TimeFolder + ".txt", TimeNow + " ExpouseTime = " + ExpouseTimeText.Value.ToString() + " Exposuring = " + ExposuringText.Text.ToString() + " Color(0-255) = " + Colorall + "\n");
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
