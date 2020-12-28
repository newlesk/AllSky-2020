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


namespace AllSky_2020
{
    public enum CAMERASTSTATE { CAPTURING, DRAWING }

    public partial class MainWindows : Form
    {
        public CAMERASTSTATE _CAMERASTSTATE;
        public string SystemMessage = "Ready.";

        private Image<Bgr, Byte> RootFrame = null, ProcessFrame = null, ROIFrame;
        private Rectangle ROIRec;
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
        private int ROIRecExposure;
        //private ASI_CONTROL_CAPS CAPS;
        private string TimeNowChack;
        //private int CameraCameraId;
        private int RestratModeCamera;
        //private int TimeExposure;
        private bool IsExpSuccess;
        int[] ColorEx = new int[30];





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

            if (ConnectedCameras > 0 && RestratModeCamera != 1)
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
            if (IsAutoExposureTime.CheckState == 0)
            {
                IsAutoExposureTime.Checked = true;
                //AutoExposureOnTime.Checked = true;
            }

        }






        private void CameraConnection()
        {
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
            Task Capdatatain = Task.Run(() =>
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
                            ProcessFrame = RootFrame.Copy();
                            ROIFrame = RootFrame.Copy();
                        }
                    }
                    else
                    {
                        ASICameraDll2.ASICloseCamera(CameraId);
                        CameraConnection();
                    }

                    GC.Collect();
                    Thread.Sleep(200);
                    //Task.Delay(2000);
                    //Thread.Sleep(CameraTimeWait);
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



        private void ROIImage_Click(object sender, EventArgs e)
        {

        }

        private void BtnSetROI_Click(object sender, EventArgs e)
        {
            //AppSetting.Data.ImageWidth = Int16.Parse(AreaTextX.Text);
            //AppSetting.Data.ImageHeight = Int16.Parse(AreaTextY.Text);
            //AppSetting.Save();
        }

        private void OriginDisplay_CheckedChanged(object sender, EventArgs e)
        {
            AppSetting.Data.IS_DISPLAY_ORGIN = OriginDisplay.Checked;
        }

        private void MainImageControl_Click(object sender, EventArgs e)
        {
            IsAutoExposureTime.CheckState = 0;
            ROIRecExposure = 1;


        }

        private void IsAutoExposureTime_CheckedChanged(object sender, EventArgs e)

        {
            if (ROIRecExposure == 0)
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

        private void label22_Click(object sender, EventArgs e)
        {

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

        private void checkBoxAverage_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBoxCenter_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Histogram_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void CameraList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }




        private void UITimer_Tick(object sender, EventArgs e)
        {


            if (ASICameraDll2.ASIGetNumOfConnectedCameras() == 0)
            {

                RestratModeCamera = 1;
                SystemMessage = "No camera connected";
                MainImageControl.Image = new Image<Bgr, byte>(AppDomain.CurrentDomain.BaseDirectory + "TAllSyk.jpg");
                ROIImage.Image = new Image<Bgr, byte>(AppDomain.CurrentDomain.BaseDirectory + "Nocameraconnected.jpg");
                //MessageBox.Show("No camera connected. Please check the cable.", "Message",  MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                //Tthen = DateTime.Now;

                //GC.Collect();
                //Thread.Sleep(200);
                //GC.Collect();

            }
            else if (RestratModeCamera == 1 && ASICameraDll2.ASIGetNumOfConnectedCameras() > 0)
            {

                //SystemMessage = "Wait for connect again";
                //ROIImage.Image = new Image<Bgr, byte>(AppDomain.CurrentDomain.BaseDirectory + "Waitforconnectagain.jpg");



                //Console.WriteLine("Going to sleep..");

                //Stopwatch stopwatch = Stopwatch.StartNew();
                //while (stopwatch.ElapsedMilliseconds < 2000);

                //Console.WriteLine("Woke up after 2 sec!");
                //
                //Task ControlRestratCameraConnected = Task.Run(() =>{
                //Thread.Sleep(2000);
                //GC.Collect();
                //Thread.Sleep(5000);
                if (ASICameraDll2.ASIGetNumOfConnectedCameras() > 0)
                {
                    //MainWindows_Load(sender,e);
                    RestratModeCamera = 0;
                    //InitializeSystem();

                    //IsExpSuccess = false;

                    Application.Restart();


                }
                //});


            }
            else
            {

                ASI_EXPOSURE_STATUS ExpStatus = ASI_EXPOSURE_STATUS.ASI_EXP_IDLE;
                ASI_ERROR_CODE GetExpError = ASICameraDll2.ASIGetExpStatus(CameraId, out ExpStatus);

                if (GetExpError == ASI_ERROR_CODE.ASI_SUCCESS)
                    CameraStateText.Text = ExpStatus.ToString();
                else
                    CameraStateText.Text = GetExpError.ToString();

                //ExposuringText.Text = string.Format("{0:0.00}", AppSetting.Data.ExposureTime / 1000 - ExposureCounter.Elapsed.TotalSeconds);
                ExposuringText.Text = string.Format("{0:0.00}", Math.Log(Math.Pow(2.8, 2) / (AppSetting.Data.ExposureTime / 1000), 2.0));


                SavePath.Text = AppSetting.Data.SaveFileDialog;


                //System.Diagnostics.Debug.WriteLine(ExposureCounter.Elapsed.TotalSeconds);
                MessageStatusText.Text = SystemMessage;
                GetDataFailedText.Text = GetImageState.ToString();
                CalculateZoomScale();

                if (IsDefineOrigin)
                    BtnDefineOrigin.Text = "Stop Define";
                else
                    BtnDefineOrigin.Text = "Start Define";

                if (ROIRec.Width > 0 && ROIRec.Height > 0)
                {
                    System.Diagnostics.Debug.WriteLine("ROIRec.Width == " + ROIRec.Width);
                    System.Diagnostics.Debug.WriteLine("ROIRec.Height == " + ROIRec.Height);
                    ROIFrame.ROI = ROIRec;
                    ROIImage.Image = ROIFrame.Convert<Gray, Byte>();



                    //if (IsAutoExposureTime.CheckState != 0) //AutoExposureTime 

                    //{



                }



                if (ProcessFrame != null && ASICameraDll2.ASIGetNumOfConnectedCameras() > 0)
                {


                    if (IsDefineROI)
                        CvInvoke.Rectangle(ProcessFrame, ROIRec, new Bgr(Color.White).MCvScalar, 2);

                    if (AppSetting.Data.IS_DISPLAY_ORGIN)
                        CvInvoke.Rectangle(ProcessFrame, new Rectangle((int)AppSetting.Data.OriginX, (int)AppSetting.Data.OriginY, (int)AppSetting.Data.OriginWidth, (int)AppSetting.Data.OriginHeight), new Bgr(Color.LightGreen).MCvScalar, 2);

                    AltAz SunAltAz = SunHanler.GetSunPosition();

                    Point SunXY = AzmAltToXY.CalculateXYPoint(SunAltAz.Alt.Degs, SunAltAz.Az.Degs, 20);

                    //CvInvoke.Circle(ProcessFrame, SunXY, 20, new Bgr(Color.LightGreen).MCvScalar, 2);

                    MainImageControl.Image = ProcessFrame;

                    // if (ROIRec.Width < 1 && ROIRec.Height < 1)
                    // {
                    ROIImage.Image = ProcessFrame; //ROIFrameOnGrayImage                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
                    ROIImage.Image = ROIFrame.Convert<Gray, Byte>();

                    // }





                    //AppSetting.Data.SaveFileDialog = folderName;
                    //string pathString = System.IO.Path.Combine(folderName, "AllSky");
                    //System.IO.Directory.CreateDirectory(pathString);
                    //string fileName = System.IO.Path.GetRandomFileName();

                    Image<Bgr, Byte> ImageFrame = ProcessFrame;

                    //=================================================================================================




                    /* Image<Gray, Byte> gray = ImageFrame.Convert<Gray, Byte>().PyrDown().PyrUp();

                     Gray cannyThreshold = new Gray(180);
                     Gray cannyThresholdLinking = new Gray(120);
                     Gray circleAccumulatorThreshold = new Gray(120);

                     CircleF[] circles = gray.HoughCircles(
                         cannyThreshold,
                         circleAccumulatorThreshold,
                         5.0, //Resolution of the accumulator used to detect centers of the circles
                         10.0, //min distance 
                         5, //min radius
                         0 //max radius
                         )[0]; //Get the circles from the first channel

                     Image<Gray, Byte> cannyEdges = gray.Canny(cannyThreshold, cannyThresholdLinking);
                     LineSegment2D[] lines = cannyEdges.HoughLinesBinary(
                        1, //Distance resolution in pixel-related units
                        Math.PI / 45.0, //Angle resolution measured in radians.
                        20, //threshold
                        30, //min Line width
                        10 //gap between lines
                         )[0]; //Get the lines from the first channel

                     Image<Bgr, Byte> circleImage = ImageFrame.CopyBlank();
                     foreach (CircleF circle in circles)
                         circleImage.Draw(circle, new Bgr(Color.Brown), 2);
                           ImageFrame = circleImage;*/




                    //=================================================================================================


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
                    CvInvoke.PutText(ImageFrame, (int)ExposureTimeShow + " ms", new Point(borderWidth + 50, borderHeight + 50), FontFace.HersheySimplex, 1.5, new Bgr(Color.White).MCvScalar, thickness);


                    Bitmap BmpInput = ImageFrame.ToBitmap();

                    /*RectangleF rectf = new RectangleF(70, 90, 90, 50);

                    Graphics g = Graphics.FromImage(BmpInput);

                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.DrawString("yourText", new Font("Tahoma",8), Brushes.Black, rectf);

                    g.Flush();
                    */





                    //ConnectedCameras = ASICameraDll2.ASIGetNumOfConnectedCameras();
                    //if (ConnectedCameras == 0)
                    //{
                    //UITimer_Tick.Refresh();
                    //System.Threading.Thread.Sleep(1000);
                    //Application.Restart();

                    //}







                    if (IsAutoExposureTime.CheckState != 0) //AutoExposureTime 
                    {

                        //AppSetting.Save();
                        //ExpouseTimeText.Value = (decimal)AppSetting.Data.ExposureTime;


                        // TimeExposure = Int16.Parse(DateTime.Now.ToString("HH"));
                        if (ROIRec.Width < 1 && ROIRec.Height < 1)
                        {

                            ROIRecExposure = 0;


                            if (checkBoxCenter.CheckState != 0) //checkBoxFocus
                            {

                                checkBoxAverage.Checked = false;
                                int CameraWidth = Int16.Parse(ROITextWidth.Text);
                                int CameraHeight = Int16.Parse(ROITextHeight.Text);
                                CentroidX = (int)(CameraWidth / 2);
                                CentroidY = (int)(CameraHeight / 2);
                                //CvInvoke.PutText(ImageFrame, "X", new Point(Int32.Parse(CentroidX.ToString()), Int32.Parse(CentroidY.ToString())), FontFace.HersheySimplex, 1.5, new Bgr(Color.Red).MCvScalar, thickness);

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
                                double CameraWidth = AppSetting.Data.ImageWidth;
                                double CameraHeight = AppSetting.Data.ImageHeight;
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

                                /*for (int i = 0; i < 21;i++)
                                {
                                    ColorEx[i] = 0;
                                }*/
                                //==================Readlight Center
                                for (double i = CentroidX - 10; i < CentroidX; i++)
                                {

                                    for (double j = CentroidY - 10; j < CentroidY; j++)

                                    {

                                        Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                        Colorall += (pixel.R + pixel.B + pixel.G) / 3;
                                        ColorEx[0] += (pixel.R + pixel.B + pixel.G) / 3;


                                    }

                                }

                                for (double i = CentroidXleftmore - 10; i < CentroidXleftmore; i++)
                                {

                                    for (double j = CentroidYleftmore - 10; j < CentroidYleftmore; j++)

                                    {

                                        Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                        Colorall += (pixel.R + pixel.B + pixel.G) / 3;
                                        ColorEx[1] += (pixel.R + pixel.B + pixel.G) / 3;



                                    }

                                }

                                for (double i = CentroidXleft - 10; i < CentroidXleft; i++)
                                {

                                    for (double j = CentroidYleft - 10; j < CentroidYleft; j++)

                                    {

                                        Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                        Colorall += (pixel.R + pixel.B + pixel.G) / 3;
                                        ColorEx[2] += (pixel.R + pixel.B + pixel.G) / 3;


                                    }

                                }



                                for (double i = CentroidXright - 10; i < CentroidXright; i++)
                                {

                                    for (double j = CentroidYright - 10; j < CentroidYright; j++)

                                    {

                                        Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                        Colorall += (pixel.R + pixel.B + pixel.G) / 3;
                                        ColorEx[3] += (pixel.R + pixel.B + pixel.G) / 3;


                                    }

                                }

                                for (double i = CentroidXrightmore - 10; i < CentroidXrightmore; i++)
                                {

                                    for (double j = CentroidYrightmore - 10; j < CentroidYrightmore; j++)

                                    {

                                        Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                        Colorall += (pixel.R + pixel.B + pixel.G) / 3;
                                        ColorEx[4] += (pixel.R + pixel.B + pixel.G) / 3;


                                    }

                                }

                                //==================Readlight UP
                                for (double i = CentroidX_UP - 10; i < CentroidX_UP; i++)
                                {

                                    for (double j = CentroidY_UP - 10; j < CentroidY_UP; j++)

                                    {

                                        Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                        Colorall += (pixel.R + pixel.B + pixel.G) / 3;
                                        ColorEx[5] += (pixel.R + pixel.B + pixel.G) / 3;


                                    }

                                }

                                for (double i = CentroidXleft_UP - 10; i < CentroidXleft_UP; i++)
                                {

                                    for (double j = CentroidYleft_UP - 10; j < CentroidYleft_UP; j++)

                                    {

                                        Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                        Colorall += (pixel.R + pixel.B + pixel.G) / 3;
                                        ColorEx[6] += (pixel.R + pixel.B + pixel.G) / 3;


                                    }

                                }

                                for (double i = CentroidXleftmore_UP - 10; i < CentroidXleftmore_UP; i++)
                                {

                                    for (double j = CentroidYleftmore_UP - 10; j < CentroidYleftmore_UP; j++)

                                    {

                                        Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                        Colorall += (pixel.R + pixel.B + pixel.G) / 3;
                                        ColorEx[7] += (pixel.R + pixel.B + pixel.G) / 3;


                                    }

                                }



                                for (double i = CentroidXright_UP - 10; i < CentroidXright_UP; i++)
                                {

                                    for (double j = CentroidYright_UP - 10; j < CentroidYright_UP; j++)

                                    {

                                        Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                        Colorall += (pixel.R + pixel.B + pixel.G) / 3;
                                        ColorEx[8] += (pixel.R + pixel.B + pixel.G) / 3;


                                    }

                                }

                                for (double i = CentroidXrightmore_UP - 10; i < CentroidXrightmore_UP; i++)
                                {

                                    for (double j = CentroidYrightmore_UP - 10; j < CentroidYrightmore_UP; j++)

                                    {

                                        Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                        Colorall += (pixel.R + pixel.B + pixel.G) / 3;
                                        ColorEx[9] += (pixel.R + pixel.B + pixel.G) / 3;


                                    }

                                }
                                //==================Readlight Down
                                for (double i = CentroidX_DOWN - 10; i < CentroidX_DOWN; i++)
                                {

                                    for (double j = CentroidY_DOWN - 10; j < CentroidY_DOWN; j++)

                                    {

                                        Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                        Colorall += (pixel.R + pixel.B + pixel.G) / 3;
                                        ColorEx[10] += (pixel.R + pixel.B + pixel.G) / 3;


                                    }

                                }

                                for (double i = CentroidXleft_DOWN - 10; i < CentroidXleft_DOWN; i++)
                                {

                                    for (double j = CentroidYleft_DOWN - 10; j < CentroidYleft_DOWN; j++)

                                    {

                                        Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                        Colorall += (pixel.R + pixel.B + pixel.G) / 3;
                                        ColorEx[11] += (pixel.R + pixel.B + pixel.G) / 3;


                                    }

                                }

                                for (double i = CentroidXleftmore_DOWN - 10; i < CentroidXleftmore_DOWN; i++)
                                {

                                    for (double j = CentroidYleftmore_DOWN - 10; j < CentroidYleftmore_DOWN; j++)

                                    {

                                        Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                        Colorall += (pixel.R + pixel.B + pixel.G) / 3;
                                        ColorEx[12] += (pixel.R + pixel.B + pixel.G) / 3;


                                    }

                                }


                                for (double i = CentroidXright_DOWN - 10; i < CentroidXright_DOWN; i++)
                                {

                                    for (double j = CentroidYright_DOWN - 10; j < CentroidYright_DOWN; j++)

                                    {

                                        Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                        Colorall += (pixel.R + pixel.B + pixel.G) / 3;
                                        ColorEx[13] += (pixel.R + pixel.B + pixel.G) / 3;


                                    }

                                }

                                for (double i = CentroidXrightmore_DOWN - 10; i < CentroidXrightmore_DOWN; i++)
                                {

                                    for (double j = CentroidYrightmore_DOWN - 10; j < CentroidYrightmore_DOWN; j++)

                                    {

                                        Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                        Colorall += (pixel.R + pixel.B + pixel.G) / 3;
                                        ColorEx[14] += (pixel.R + pixel.B + pixel.G) / 3;


                                    }

                                }

                                //==================Readlight UP DOT
                                for (double i = CentroidX_UP2 - 10; i < CentroidX_UP2; i++)
                                {

                                    for (double j = CentroidY_UP2 - 10; j < CentroidY_UP2; j++)

                                    {

                                        Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                        Colorall += (pixel.R + pixel.B + pixel.G) / 3;
                                        ColorEx[15] += (pixel.R + pixel.B + pixel.G) / 3;


                                    }

                                }

                                for (double i = CentroidXright_UP2 - 10; i < CentroidXright_UP2; i++)
                                {

                                    for (double j = CentroidYright_UP2 - 10; j < CentroidYright_UP2; j++)

                                    {

                                        Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                        Colorall += (pixel.R + pixel.B + pixel.G) / 3;
                                        ColorEx[16] += (pixel.R + pixel.B + pixel.G) / 3;


                                    }

                                }

                                for (double i = CentroidXleft_UP2 - 10; i < CentroidXleft_UP2; i++)
                                {

                                    for (double j = CentroidYleft_UP2 - 10; j < CentroidYleft_UP2; j++)

                                    {

                                        Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                        Colorall += (pixel.R + pixel.B + pixel.G) / 3;
                                        ColorEx[17] += (pixel.R + pixel.B + pixel.G) / 3;


                                    }

                                }

                                //==================Readlight DOWN DOT

                                for (double i = CentroidX_DOWN2 - 10; i < CentroidX_DOWN2; i++)
                                {

                                    for (double j = CentroidX_DOWN2 - 10; j < CentroidX_DOWN2; j++)

                                    {

                                        Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                                        ColorEx[18] += (pixel.R + pixel.B + pixel.G) / 3;


                                        Colorall += (pixel.R + pixel.B + pixel.G) / 3;



                                    }

                                }

                                for (double i = CentroidXright_DOWN2 - 10; i < CentroidXright_DOWN2; i++)
                                {

                                    for (double j = CentroidYright_DOWN2 - 10; j < CentroidYright_DOWN2; j++)

                                    {

                                        Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                        Colorall += (pixel.R + pixel.B + pixel.G) / 3;
                                        ColorEx[19] += (pixel.R + pixel.B + pixel.G) / 3;


                                    }

                                }

                                for (double i = CentroidXleft_DOWN2 - 10; i < CentroidXleft_DOWN2; i++)
                                {

                                    for (double j = CentroidYleft_DOWN2 - 10; j < CentroidYleft_DOWN2; j++)

                                    {

                                        Color pixel = BmpInput.GetPixel(Convert.ToInt32(i), Convert.ToInt32(j));



                                        Colorall += (pixel.R + pixel.B + pixel.G) / 3;
                                        ColorEx[20] += (pixel.R + pixel.B + pixel.G) / 3;


                                    }

                                }

                                Colorall = (Colorall / 2100);

                            }







                            ExposuringText.Text = string.Format("{0:0.00}", Math.Log(Math.Pow(2.8, 2) / (AppSetting.Data.ExposureTime / 1000), 2.0));







                            //System.Diagnostics.Debug.WriteLine("DateTime.Now = "+DateTime.Now.ToString("HH tt"));

                            //string ExposureAMPM = DateTime.Now.ToString("tt");
                            //System.Diagnostics.Debug.WriteLine("RGBALL = " + Colorall);





                            Task AutoExposureTime = Task.Run(() =>
                            {






                                //  ======================== AutoExposureTime -  ========================
                                //if (CameraStateText.Text != "ASI_EXP_IDLE")
                                if(CameraStateText.Text !=  "ASI_EXP_WORKING")
                                {


                                    if (Colorall >= 255 && AppSetting.Data.ExposureTime >= 15000)
                                    {
                                        AppSetting.Data.ExposureTime = 8000;
                                        Task.Delay(8000);
                                        //GC.Collect();

                                        //goto STARTExposure;
                                    }
                                    else if (Colorall >= 255 && AppSetting.Data.ExposureTime >= 8000)
                                    {
                                        AppSetting.Data.ExposureTime = 4000;
                                        Task.Delay(4000);
                                        // GC.Collect();

                                        //goto STARTExposure;
                                    }
                                    else if (Colorall >= 255 && AppSetting.Data.ExposureTime >= 4000)
                                    {
                                        AppSetting.Data.ExposureTime = 2000;
                                        Task.Delay(2000);
                                        //GC.Collect();

                                        //goto STARTExposure;
                                    }
                                    else if (Colorall >= 255 && AppSetting.Data.ExposureTime >= 2000)
                                    {
                                        AppSetting.Data.ExposureTime = 1000;
                                        Task.Delay(1000);
                                        //GC.Collect();

                                        //goto STARTExposure;
                                    }
                                    else if (Colorall >= 255 && AppSetting.Data.ExposureTime >= 1000)
                                    {
                                        AppSetting.Data.ExposureTime = 500;
                                        Task.Delay(1000);
                                        //GC.Collect();

                                        //goto STARTExposure;
                                    }

                                    else if (Colorall >= 255 && AppSetting.Data.ExposureTime >= 500)
                                    {
                                        AppSetting.Data.ExposureTime = 250;
                                        Task.Delay(1000);
                                        //GC.Collect();

                                        //goto STARTExposure;
                                    }
                                    else if (Colorall >= 255 && AppSetting.Data.ExposureTime >= 250)
                                    {
                                        AppSetting.Data.ExposureTime = 125;
                                        Task.Delay(1000);
                                        //GC.Collect();

                                        //goto STARTExposure;
                                    }
                                    else if (Colorall >= 255 && AppSetting.Data.ExposureTime >= 125)
                                    {
                                        AppSetting.Data.ExposureTime = 66;
                                        Task.Delay(1000);
                                        // GC.Collect();

                                        //goto STARTExposure;
                                    }
                                    else if (Colorall >= 255 && AppSetting.Data.ExposureTime >= 66)
                                    {
                                        AppSetting.Data.ExposureTime = 33;
                                        Task.Delay(1000);
                                    }
                                    else if (Colorall >= 255 && AppSetting.Data.ExposureTime >= 33)
                                    {
                                        AppSetting.Data.ExposureTime = 0;
                                    }
                                    else if (Colorall >= 255 && AppSetting.Data.ExposureTime >= 3)
                                    {
                                        AppSetting.Data.ExposureTime = 0;
                                    }
                                    else if (Colorall >= 255 && AppSetting.Data.ExposureTime >= 2)
                                    {
                                        AppSetting.Data.ExposureTime -= 0.01;
                                    }




                                    //========================================================================================

                                    if (CameraStateText.Text != "ASI_EXP_WORKING")
                                    {
                                        if (Colorall >= 200 && AppSetting.Data.ExposureTime >= 15000 && Colorall < 255)
                                        {
                                            AppSetting.Data.ExposureTime = 9600;
                                            Task.Delay(9600);
                                            //GC.Collect();

                                            //goto STARTExposure;
                                        }
                                        else if (Colorall >= 200 && AppSetting.Data.ExposureTime >= 8000 && Colorall < 255)
                                        {
                                            AppSetting.Data.ExposureTime = 4800;
                                            Task.Delay(4800);
                                            //GC.Collect();

                                            //goto STARTExposure;
                                        }
                                        else if (Colorall >= 200 && AppSetting.Data.ExposureTime >= 4000 && Colorall < 255)
                                        {
                                            AppSetting.Data.ExposureTime = 2400;
                                            Thread.Sleep(2400);
                                            //GC.Collect();

                                            //goto STARTExposure;
                                        }
                                        else if (Colorall >= 200 && AppSetting.Data.ExposureTime >= 2000 && Colorall < 255)
                                        {
                                            AppSetting.Data.ExposureTime = 1200;
                                            Task.Delay(1200);
                                            //GC.Collect();

                                            // goto STARTExposure;
                                        }
                                        else if (Colorall >= 200 && AppSetting.Data.ExposureTime >= 1000 && Colorall < 255)
                                        {
                                            AppSetting.Data.ExposureTime = 600;
                                            Task.Delay(1000);
                                            //GC.Collect();

                                            //goto STARTExposure;
                                        }
                                        else if (Colorall >= 200 && AppSetting.Data.ExposureTime >= 500 && Colorall < 255)
                                        {
                                            AppSetting.Data.ExposureTime = 300;
                                            Task.Delay(1000);
                                            //GC.Collect();

                                            //goto STARTExposure;
                                        }
                                        else if (Colorall >= 200 && AppSetting.Data.ExposureTime >= 250 && Colorall < 255)
                                        {
                                            AppSetting.Data.ExposureTime = 150;
                                            Task.Delay(1000);
                                            //GC.Collect();

                                            //goto STARTExposure;
                                        }
                                        else if (Colorall >= 200 && AppSetting.Data.ExposureTime >= 125 && Colorall < 255)
                                        {
                                            AppSetting.Data.ExposureTime = 80;
                                            Task.Delay(1000);
                                            //GC.Collect();

                                            //goto STARTExposure;
                                        }
                                        else if (Colorall >= 200 && AppSetting.Data.ExposureTime >= 66 && Colorall < 255)
                                        {
                                            AppSetting.Data.ExposureTime = 40;
                                            Task.Delay(1000);
                                        }
                                        else if (Colorall >= 200 && AppSetting.Data.ExposureTime >= 33 && Colorall < 255)
                                        {
                                            AppSetting.Data.ExposureTime = 20;
                                            Task.Delay(1000);
                                        }
                                        else if (Colorall >= 200 && AppSetting.Data.ExposureTime >= 1 && Colorall < 255)
                                        {
                                            AppSetting.Data.ExposureTime -= 0.1;
                                            Task.Delay(1000);
                                        }



                                        //========================================================================================
                                        if (Colorall >= 150 && AppSetting.Data.ExposureTime >= 15000 && Colorall < 200)
                                        {
                                            AppSetting.Data.ExposureTime = 10000;
                                            Task.Delay(10000);
                                            //GC.Collect();

                                            //goto STARTExposure;
                                        }
                                        else if (Colorall >= 150 && AppSetting.Data.ExposureTime >= 8000 && Colorall < 200)
                                        {
                                            AppSetting.Data.ExposureTime = 5000;
                                            Task.Delay(5000);
                                            //GC.Collect();

                                            //goto STARTExposure;
                                        }
                                        else if (Colorall >= 150 && AppSetting.Data.ExposureTime >= 4000 && Colorall < 200)
                                        {
                                            AppSetting.Data.ExposureTime = 2500;
                                            Task.Delay(2500);
                                            //GC.Collect();

                                            //goto STARTExposure;
                                        }
                                        else if (Colorall >= 150 && AppSetting.Data.ExposureTime >= 2000 && Colorall < 200)
                                        {
                                            AppSetting.Data.ExposureTime = 1250;
                                            Task.Delay(1250);
                                            //GC.Collect();

                                            //goto STARTExposure;
                                        }
                                        else if (Colorall >= 150 && AppSetting.Data.ExposureTime >= 1000 && Colorall < 200)
                                        {
                                            AppSetting.Data.ExposureTime = 625;
                                            Task.Delay(1000);
                                            //GC.Collect();

                                            //goto STARTExposure;
                                        }
                                        else if (Colorall >= 150 && AppSetting.Data.ExposureTime >= 500 && Colorall < 200)
                                        {
                                            AppSetting.Data.ExposureTime = 312;
                                            Task.Delay(1000);
                                            //GC.Collect();

                                            //goto STARTExposure;
                                        }
                                        else if (Colorall >= 150 && AppSetting.Data.ExposureTime >= 250 && Colorall < 200)
                                        {
                                            AppSetting.Data.ExposureTime = 160;
                                            Task.Delay(1000);
                                            //GC.Collect();

                                            //goto STARTExposure;
                                        }
                                        else if (Colorall >= 150 && AppSetting.Data.ExposureTime >= 125 && Colorall < 200)
                                        {
                                            AppSetting.Data.ExposureTime = 83;
                                            Task.Delay(1000);
                                        }
                                        else if (Colorall >= 150 && AppSetting.Data.ExposureTime >= 66 && Colorall < 200)
                                        {
                                            AppSetting.Data.ExposureTime = 41;
                                            //Thread.Sleep(1000);
                                        }
                                        else if (Colorall >= 150 && AppSetting.Data.ExposureTime >= 33 && Colorall < 200)
                                        {
                                            AppSetting.Data.ExposureTime = 25;
                                            Task.Delay(1000);
                                        }
                                        else if (Colorall >= 150 && AppSetting.Data.ExposureTime >= 1 && Colorall < 200)
                                        {
                                            AppSetting.Data.ExposureTime -= 0.1;
                                            Task.Delay(1000);
                                        }

                                    }

                                    //========================================================================================
                                    if (Colorall >= 135 && AppSetting.Data.ExposureTime >= 15000 && Colorall < 150)
                                    {
                                        AppSetting.Data.ExposureTime -= 5;
                                    }
                                    else if (Colorall >= 135 && AppSetting.Data.ExposureTime >= 8000 && Colorall < 150)
                                    {
                                        AppSetting.Data.ExposureTime -= 2;
                                    }
                                    else if (Colorall >= 135 && AppSetting.Data.ExposureTime >= 4000 && Colorall < 150)
                                    {
                                        AppSetting.Data.ExposureTime -= 1;
                                    }
                                    else if (Colorall >= 135 && AppSetting.Data.ExposureTime >= 2000 && Colorall < 150)
                                    {
                                        AppSetting.Data.ExposureTime -= 1;
                                    }
                                    else if (Colorall >= 135 && AppSetting.Data.ExposureTime >= 1000 && Colorall < 150)
                                    {
                                        AppSetting.Data.ExposureTime -= 1;
                                    }
                                    else if (Colorall >= 135 && AppSetting.Data.ExposureTime >= 500 && Colorall < 150)
                                    {
                                        AppSetting.Data.ExposureTime -= 1;
                                    }
                                    else if (Colorall >= 135 && AppSetting.Data.ExposureTime >= 250 && Colorall < 150)
                                    {
                                        AppSetting.Data.ExposureTime -= 1;
                                    }
                                    else if (Colorall >= 135 && AppSetting.Data.ExposureTime >= 125 && Colorall < 150)
                                    {
                                        AppSetting.Data.ExposureTime -= 1;
                                    }
                                    else if (Colorall >= 135 && AppSetting.Data.ExposureTime >= 66 && Colorall < 150)
                                    {
                                        AppSetting.Data.ExposureTime -= 1;
                                    }
                                    else if (Colorall >= 135 && AppSetting.Data.ExposureTime >= 33 && Colorall < 150)
                                    {
                                        AppSetting.Data.ExposureTime -= 1;
                                    }
                                    else if (Colorall >= 135 && AppSetting.Data.ExposureTime < 33 && Colorall < 150 && AppSetting.Data.ExposureTime > 1)
                                    {
                                        AppSetting.Data.ExposureTime -= 0.1;
                                    }
                                    else if (Colorall >= 135 && AppSetting.Data.ExposureTime <= 1 && Colorall < 150)
                                    {
                                        AppSetting.Data.ExposureTime -= 0.01;
                                    }



                                    // ======================== AutoExposureTime +  ======================== 

                                    if (AppSetting.Data.ExposureTime <= 120000)
                                    {
                                        if (Colorall <= 80 && AppSetting.Data.ExposureTime >= 15000 && Colorall > 50)
                                        {
                                            AppSetting.Data.ExposureTime += 40;
                                            Task.Delay(1000);
                                            //GC.Collect();

                                            //goto STARTExposure;
                                        }
                                        else if (Colorall <= 80 && AppSetting.Data.ExposureTime >= 8000 && Colorall > 50 && AppSetting.Data.ExposureTime < 15000)
                                        {
                                            AppSetting.Data.ExposureTime += 30;
                                            Task.Delay(1000);
                                            //GC.Collect();

                                            //goto STARTExposure;
                                        }
                                        else if (Colorall <= 80 && AppSetting.Data.ExposureTime >= 4000 && Colorall > 50 && AppSetting.Data.ExposureTime < 8000)
                                        {
                                            AppSetting.Data.ExposureTime += 25;

                                            //GC.Collect();

                                            //goto STARTExposure;
                                        }
                                        else if (Colorall <= 80 && AppSetting.Data.ExposureTime >= 2000 && Colorall > 50 && AppSetting.Data.ExposureTime < 4000)
                                        {
                                            AppSetting.Data.ExposureTime += 20;

                                            //GC.Collect();

                                            //goto STARTExposure;
                                        }
                                        else if (Colorall <= 80 && AppSetting.Data.ExposureTime >= 1000 && Colorall > 50 && AppSetting.Data.ExposureTime < 2000)
                                        {
                                            AppSetting.Data.ExposureTime += 15;

                                            //GC.Collect();

                                            //goto STARTExposure;
                                        }
                                        else if (Colorall <= 80 && AppSetting.Data.ExposureTime >= 500 && AppSetting.Data.ExposureTime < 1000 && Colorall > 50)
                                        {
                                            AppSetting.Data.ExposureTime += 10;

                                        }
                                        else if (Colorall <= 80 && AppSetting.Data.ExposureTime >= 250 && AppSetting.Data.ExposureTime < 500 && Colorall > 50)
                                        {
                                            AppSetting.Data.ExposureTime += 4;

                                        }
                                        else if (Colorall <= 80 && AppSetting.Data.ExposureTime >= 125 && AppSetting.Data.ExposureTime < 250 && Colorall > 50)
                                        {
                                            AppSetting.Data.ExposureTime += 3;

                                        }
                                        else if (Colorall <= 80 && AppSetting.Data.ExposureTime >= 66 && AppSetting.Data.ExposureTime < 125 && Colorall >= 50)
                                        {
                                            AppSetting.Data.ExposureTime += 2;

                                        }
                                        else if (Colorall <= 80 && AppSetting.Data.ExposureTime > 33 && AppSetting.Data.ExposureTime < 66 && Colorall >= 50)
                                        {
                                            AppSetting.Data.ExposureTime += 1;

                                        }
                                        else if (Colorall <= 80 && AppSetting.Data.ExposureTime <= 33 && Colorall >= 50)
                                        {
                                            AppSetting.Data.ExposureTime += 0.1;

                                        }




                                        //========================================================================================

                                        if (CameraStateText.Text != "ASI_EXP_WORKING")
                                        {
                                            if (Colorall <= 50 && AppSetting.Data.ExposureTime >= 30000)
                                            {
                                                AppSetting.Data.ExposureTime += 500;
                                                Task.Delay(30000);
                                                //GC.Collect();

                                                //goto STARTExposure;
                                            }
                                            else if (Colorall <= 50 && AppSetting.Data.ExposureTime >= 15000 && AppSetting.Data.ExposureTime < 30000)
                                            {
                                                AppSetting.Data.ExposureTime = 30000;
                                                Task.Delay(15000);
                                                //GC.Collect();
                                                //goto STARTExposure;
                                            }
                                            else if (Colorall <= 50 && AppSetting.Data.ExposureTime >= 8000 && AppSetting.Data.ExposureTime < 15000)
                                            {
                                                AppSetting.Data.ExposureTime = 15000;
                                                Task.Delay(8000);
                                                //GC.Collect();

                                                //goto STARTExposure;
                                            }
                                            else if (Colorall <= 50 && AppSetting.Data.ExposureTime >= 4000 && AppSetting.Data.ExposureTime < 8000)
                                            {
                                                AppSetting.Data.ExposureTime = 8000;
                                                Task.Delay(4000);
                                                //GC.Collect();

                                                //goto STARTExposure;
                                            }
                                            else if (Colorall <= 50 && AppSetting.Data.ExposureTime >= 2000 && AppSetting.Data.ExposureTime < 4000)
                                            {
                                                AppSetting.Data.ExposureTime = 4000;
                                                Task.Delay(2000);
                                                //GC.Collect();

                                                //goto STARTExposure;
                                            }
                                            else if (Colorall <= 50 && AppSetting.Data.ExposureTime >= 1000 && AppSetting.Data.ExposureTime < 2000)
                                            {
                                                AppSetting.Data.ExposureTime = 2000;
                                                Task.Delay(1000);
                                                //GC.Collect();

                                                //goto STARTExposure;
                                            }
                                            else if (Colorall <= 50 && AppSetting.Data.ExposureTime >= 500 && AppSetting.Data.ExposureTime < 1000)
                                            {
                                                AppSetting.Data.ExposureTime = 1000;
                                                Task.Delay(1000);
                                                //GC.Collect();

                                                //goto STARTExposure;
                                            }
                                            else if (Colorall <= 50 && AppSetting.Data.ExposureTime >= 250 && AppSetting.Data.ExposureTime < 500)
                                            {
                                                AppSetting.Data.ExposureTime = 500;
                                                Task.Delay(1000);
                                                //GC.Collect();

                                                //goto STARTExposure;

                                            }
                                            else if (Colorall <= 50 && AppSetting.Data.ExposureTime >= 125 && AppSetting.Data.ExposureTime < 250)
                                            {
                                                AppSetting.Data.ExposureTime = 250;
                                                Task.Delay(1000);
                                                //GC.Collect();

                                                //goto STARTExposure;
                                            }
                                            else if (Colorall <= 50 && AppSetting.Data.ExposureTime >= 66 && AppSetting.Data.ExposureTime < 125)
                                            {
                                                AppSetting.Data.ExposureTime = 125;
                                                Task.Delay(1000);
                                                //GC.Collect();

                                                //goto STARTExposure;
                                            }
                                            else if (Colorall <= 50 && AppSetting.Data.ExposureTime >= 33 && AppSetting.Data.ExposureTime < 66)
                                            {
                                                AppSetting.Data.ExposureTime = 66;
                                                Task.Delay(1000);
                                                //GC.Collect();

                                                //goto STARTExposure;
                                            }
                                            else if (Colorall <= 50 && AppSetting.Data.ExposureTime >= 33 && AppSetting.Data.ExposureTime < 33)
                                            {
                                                AppSetting.Data.ExposureTime = 33;
                                                Task.Delay(1000);
                                                //GC.Collect();

                                                //goto STARTExposure;
                                            }

                                            else if (Colorall <= 50 && AppSetting.Data.ExposureTime <= 33 && AppSetting.Data.ExposureTime >= 5)
                                            {
                                                AppSetting.Data.ExposureTime += 0.1;

                                            }
                                            else if (Colorall <= 50 && AppSetting.Data.ExposureTime <= 5 && AppSetting.Data.ExposureTime > 0.3)
                                            {
                                                AppSetting.Data.ExposureTime += 0.1;

                                            }
                                            else if (AppSetting.Data.ExposureTime < 1)
                                            {
                                                AppSetting.Data.ExposureTime = 0;
                                                if (Colorall <= 20)
                                                {
                                                    AppSetting.Data.ExposureTime = 0.4;
                                                }
                                                if (Colorall <= 10)
                                                {
                                                    AppSetting.Data.ExposureTime = 1;
                                                }

                                            }










                                        }
                                    }
                                }
                                Thread.Sleep(1000);

                                GC.Collect();


                                //AppSetting.Save();


                            });





                            //AppSetting.Save();
                            if (AppSetting.Data.ExposureTime % 1 == 0)
                            {
                                //AppSetting.Save();
                                ExpouseTimeText.Value = (decimal)AppSetting.Data.ExposureTime;

                            }

                        }

                        else
                            AppSetting.Data.ExposureTime = (double)ExpouseTimeText.Value;

                        //int ColorAll1 = (color1 + color2 + color3 + color4 + color5) /500;
                        //int ColorAll2 = (color6 + color7 + color8 + color9 + color10) / 500;
                        //int ColorAll3 = (color11 + color12 + color13 + color14 + color15) / 500;
                        //int ColorAll4 = (color16 + color17 + color18) / 300;
                        //int ColorAll5 = (color19 + color20 + color21) / 300;
                        
                        for(int i = 0; i < 21; i++)
                        {
                            ColorEx[i] = ColorEx[i] / 100;

                            if (ColorEx[i] >= 255)
                            {
                                ColorEx[i] = 6;
                            }
                            else if (ColorEx[i] >= 200)
                            {
                                ColorEx[i] = 5;
                            }
                            else if (ColorEx[i] >= 150)
                            {
                                ColorEx[i] = 4;
                            }
                            else if (ColorEx[i] >= 80)
                            {
                                ColorEx[i] = 3;
                            }
                            else if (ColorEx[i] >= 50)
                            {
                                ColorEx[i] = 2;
                            }
                            else if (ColorEx[i] >= 0)
                            {
                                ColorEx[i] = 1;
                            }
                        }


                        if (Histogramcheck.CheckState != 0)
                        {
                            Histogram.ChartAreas[0].AxisX.Minimum = 0;
                            Histogram.ChartAreas[0].AxisX.Maximum = 7;
                            Histogram.ChartAreas[0].AxisY.Minimum = 0;
                            Histogram.ChartAreas[0].AxisY.Maximum = 10;
                            /*resethistogram += 1;*/

                            uint[] items = new uint[] {/*(uint)ColorAll1,(uint)ColorAll2,(uint)ColorAll3,(uint)ColorAll4,(uint)ColorAll5,*/
                                (uint)ColorEx[0],  (uint)ColorEx[1],  (uint)ColorEx[2],  (uint)ColorEx[3],
                                (uint)ColorEx[4],  (uint)ColorEx[5],  (uint)ColorEx[6],  (uint)ColorEx[7],
                                (uint)ColorEx[8],  (uint)ColorEx[9],  (uint)ColorEx[10], (uint)ColorEx[11],
                                (uint)ColorEx[12], (uint)ColorEx[13], (uint)ColorEx[14], (uint)ColorEx[15],
                                (uint)ColorEx[16], (uint)ColorEx[17], (uint)ColorEx[18], (uint)ColorEx[19],
                                (uint)ColorEx[20]};
                            SortedDictionary<uint, int> histogram = new SortedDictionary<uint, int>();
                            foreach (uint item in items)
                            {
                                
                                /*if (item >=  255)
                                {
                                    Console.WriteLine(item);
                                    histogram[item] = 6;
                                    
                                }
                                else if (item >= 200)
                                {
                                    Console.WriteLine(item);
                                    histogram[item] = 5;
                                }
                                else if (item >= 150)
                                {
                                    Console.WriteLine(item);
                                    histogram[item] = 4;
                                }
                                else if (item >= 100)
                                {
                                    Console.WriteLine(item);
                                    histogram[item] = 3;
                                }
                                else if (item >= 50)
                                {
                                    Console.WriteLine(item);
                                    histogram[item] = 2;
                                }
                                else if (item >= 0)
                                {
                                    Console.WriteLine(item);
                                    histogram[item] = 1;
                                }*/


                                if (histogram.ContainsKey(item))
                                {
                                    
                                    histogram[item]++;
                                }
                                else
                                {
                                    histogram[item] = 1;
                                }
                            }
                            
                            foreach (var series in Histogram.Series)
                            {
                                series.Points.Clear();
                            }
                            
                            //Histogram.Series["Histogram"].Points.Clear();
                            //Histogram.Update();
                            foreach (KeyValuePair<uint, int> pair in histogram)
                            {
                                //Console.WriteLine("{0} occurred {1} times", pair.Key, pair.Value);
                                
                                Histogram.Series["Histogram"].Points.AddXY(pair.Key, pair.Value);
                                
                                Histogram.Update();


                            }

                            //Histogram.Series[0].Points.RemoveAt(0);
                            //Histogram.Series["Histogram"].Points.AddXY(histogram[1] , "1" );
                            //Histogram.Series["Histogram"].Points.AddXY(0, Colorall);
                            //Histogram.Series["Histogram"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;

                            /*if (Colorall >= 0 && Colorall <= 50)
                            {
                                Histogram.Series["Histogram"].Points.AddXY("0-50", Colorall);
                            }
                            else if (Colorall > 50 && Colorall <= 100)
                            {
                                Histogram.Series["Histogram"].Points.AddXY("50-100", Colorall);
                            }
                            else if (Colorall > 100 && Colorall <= 150)
                            {
                                Histogram.Series["Histogram"].Points.AddXY("100-150", Colorall);
                            }
                            else if (Colorall > 150 && Colorall <= 200)
                            {
                                Histogram.Series["Histogram"].Points.AddXY("150-200", Colorall);
                            }
                            else if (Colorall > 200 && Colorall <= 255)
                            {
                                Histogram.Series["Histogram"].Points.AddXY("200-255", Colorall);
                            }*/

                            Histogram.Series["Histogram"].Color = Color.Red;
                            //this.Histogram.Series.Clear();
                            //Histogram.Series["Histogram"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
                            //Histogram.Series[0].Points[ix].YValues[0] = newvalue;
                            

                            /*if (resethistogram == 1000)
                            {
                                
                                //Histogram.Series.Dispose();
                                //Histogram.Series.Clear();
                                resethistogram = 0;
                            }*/

                        }
                        else
                            Histogram.Series[0].Points.Clear();


                        //color1 = 0; color2 = 0; color3 = 0; color4 = 0;
                        //color5 = 0; color6 = 0; color7 = 0; color8 = 0;
                        //color9 = 0; color10 = 0; color11 = 0; color12 = 0;
                        //color13 = 0; color14 = 0; color15 = 0; color16 = 0;
                        //color17 = 0; color18 = 0; color19 = 0; color20 = 0;
                        //color21 = 0;
                        



                        if (AppSetting.Data.SaveFileDialog != "" && TimeNowChack != TimeBefore && Colorall < 180 && ASICameraDll2.ASIGetNumOfConnectedCameras() > 0)
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

                        }
                        File.AppendAllText(AppSetting.Data.SaveFileDialog + @"\AllSky" + @"\log " + TimeFolder + ".txt", "ExpouseTime = " + ExpouseTimeText.Value.ToString() + " Exposuring = " + ExposuringText.Text.ToString() + " Color(0-255) = " + Colorall + "\n");




                    }

                }

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
