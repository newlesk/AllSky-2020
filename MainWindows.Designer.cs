namespace AllSky_2020
{
    partial class MainWindows
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.MainImageControl = new Emgu.CV.UI.ImageBox();
            this.ROIImage = new Emgu.CV.UI.ImageBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.TabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.Histo = new Emgu.CV.UI.HistogramBox();
            this.panel6 = new System.Windows.Forms.Panel();
            this.OriginDisplay = new System.Windows.Forms.CheckBox();
            this.AreaDisplay = new System.Windows.Forms.CheckBox();
            this.BtnDefineOrigin = new System.Windows.Forms.Button();
            this.BtnSetROI = new System.Windows.Forms.Button();
            this.OriginXText = new System.Windows.Forms.TextBox();
            this.AreaTextX = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.OriginWidthText = new System.Windows.Forms.TextBox();
            this.AreaTextWidth = new System.Windows.Forms.TextBox();
            this.OriginHeightText = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.AreaTextHeight = new System.Windows.Forms.TextBox();
            this.OriginYText = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.AreaTextY = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.FocusPointLable = new System.Windows.Forms.Label();
            this.FocusPoint = new System.Windows.Forms.ComboBox();
            this.Histogramcheck = new System.Windows.Forms.CheckBox();
            this.CameraList = new System.Windows.Forms.ComboBox();
            this.label22 = new System.Windows.Forms.Label();
            this.SetCameraId = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label29 = new System.Windows.Forms.Label();
            this.autoHDR = new System.Windows.Forms.CheckBox();
            this.ClearProfilePixel = new System.Windows.Forms.Button();
            this.SaveProfilePixel = new System.Windows.Forms.Button();
            this.label28 = new System.Windows.Forms.Label();
            this.ProfilePixelValues = new System.Windows.Forms.ComboBox();
            this.SavePixelValues = new System.Windows.Forms.Button();
            this.label27 = new System.Windows.Forms.Label();
            this.pixelValuesmin = new System.Windows.Forms.Label();
            this.pixelvalues_max = new System.Windows.Forms.TextBox();
            this.pixelvalues_min = new System.Windows.Forms.TextBox();
            this.keoGrams = new System.Windows.Forms.CheckBox();
            this.hdr_On = new System.Windows.Forms.CheckBox();
            this.cannyThreshold_Box = new System.Windows.Forms.TextBox();
            this.Clear_Profile = new System.Windows.Forms.Button();
            this.label24 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.Save_HoughCircles = new System.Windows.Forms.Button();
            this.HoughCircles_Profile = new System.Windows.Forms.ComboBox();
            this.SaveProfile_HoughCircles = new System.Windows.Forms.Button();
            this.label25 = new System.Windows.Forms.Label();
            this.circleAccumulatorThreshold_Box = new System.Windows.Forms.TextBox();
            this.HoughCircles = new Emgu.CV.UI.ImageBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.StopSave = new System.Windows.Forms.CheckBox();
            this.label30 = new System.Windows.Forms.Label();
            this.SaveLog = new System.Windows.Forms.CheckBox();
            this.Savebutton = new System.Windows.Forms.Button();
            this.SavePath = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.SpeedMode = new System.Windows.Forms.CheckBox();
            this.ShowFocusPoint = new System.Windows.Forms.CheckBox();
            this.checkBoxAverage = new System.Windows.Forms.CheckBox();
            this.ResetZoom = new System.Windows.Forms.Button();
            this.checkBoxCenter = new System.Windows.Forms.CheckBox();
            this.ExpouseTimeText = new System.Windows.Forms.NumericUpDown();
            this.label23 = new System.Windows.Forms.Label();
            this.IsAutoExposureTime = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.ROITextX = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.ROITextWidth = new System.Windows.Forms.TextBox();
            this.GetDataFailedText = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.ExposuringText = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.CameraStateText = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.ROITextHeight = new System.Windows.Forms.TextBox();
            this.ROITextY = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.BtnSetCameraSetting = new System.Windows.Forms.Button();
            this.MIN_ISOText = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.MIN_SHUTTERText = new System.Windows.Forms.TextBox();
            this.MIN_APERTUREText = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.MAX_ISOTextL = new System.Windows.Forms.Label();
            this.MAX_SHUTTERText = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.MAX_ISOText = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.XYPosText = new System.Windows.Forms.Label();
            this.MessageStatusText = new System.Windows.Forms.Label();
            this.UITimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MainImageControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ROIImage)).BeginInit();
            this.TabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel4.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HoughCircles)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ExpouseTimeText)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(5, 5);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer3);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel3);
            this.splitContainer1.Panel2.Controls.Add(this.TabControl);
            this.splitContainer1.Panel2.Controls.Add(this.groupBox3);
            this.splitContainer1.Size = new System.Drawing.Size(1340, 815);
            this.splitContainer1.SplitterDistance = 440;
            this.splitContainer1.TabIndex = 1;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.MainImageControl);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.ROIImage);
            this.splitContainer3.Size = new System.Drawing.Size(1340, 440);
            this.splitContainer3.SplitterDistance = 689;
            this.splitContainer3.TabIndex = 4;
            // 
            // MainImageControl
            // 
            this.MainImageControl.BackColor = System.Drawing.Color.LightGray;
            this.MainImageControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MainImageControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainImageControl.Location = new System.Drawing.Point(0, 0);
            this.MainImageControl.Name = "MainImageControl";
            this.MainImageControl.Size = new System.Drawing.Size(689, 440);
            this.MainImageControl.TabIndex = 3;
            this.MainImageControl.TabStop = false;
            this.MainImageControl.Click += new System.EventHandler(this.MainImageControl_Click);
            this.MainImageControl.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MainImageControl_MouseClick);
            this.MainImageControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainImageControl_MouseMove);
            // 
            // ROIImage
            // 
            this.ROIImage.BackColor = System.Drawing.Color.LightGray;
            this.ROIImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ROIImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ROIImage.Location = new System.Drawing.Point(0, 0);
            this.ROIImage.Name = "ROIImage";
            this.ROIImage.Size = new System.Drawing.Size(647, 440);
            this.ROIImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ROIImage.TabIndex = 4;
            this.ROIImage.TabStop = false;
            this.ROIImage.Click += new System.EventHandler(this.ROIImage_Click);
            // 
            // panel3
            // 
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(843, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(5, 371);
            this.panel3.TabIndex = 2;
            // 
            // TabControl
            // 
            this.TabControl.Controls.Add(this.tabPage1);
            this.TabControl.Controls.Add(this.tabPage2);
            this.TabControl.Controls.Add(this.tabPage3);
            this.TabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabControl.Location = new System.Drawing.Point(0, 0);
            this.TabControl.Name = "TabControl";
            this.TabControl.SelectedIndex = 0;
            this.TabControl.Size = new System.Drawing.Size(848, 371);
            this.TabControl.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(840, 345);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Setting";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.splitContainer2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(834, 339);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Process Area";
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(3, 16);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.Histo);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.panel6);
            this.splitContainer2.Panel2.Controls.Add(this.panel4);
            this.splitContainer2.Size = new System.Drawing.Size(828, 320);
            this.splitContainer2.SplitterDistance = 373;
            this.splitContainer2.TabIndex = 0;
            // 
            // Histo
            // 
            this.Histo.Location = new System.Drawing.Point(3, 3);
            this.Histo.Margin = new System.Windows.Forms.Padding(4);
            this.Histo.Name = "Histo";
            this.Histo.Size = new System.Drawing.Size(368, 320);
            this.Histo.TabIndex = 0;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.OriginDisplay);
            this.panel6.Controls.Add(this.AreaDisplay);
            this.panel6.Controls.Add(this.BtnDefineOrigin);
            this.panel6.Controls.Add(this.BtnSetROI);
            this.panel6.Controls.Add(this.OriginXText);
            this.panel6.Controls.Add(this.AreaTextX);
            this.panel6.Controls.Add(this.label21);
            this.panel6.Controls.Add(this.label4);
            this.panel6.Controls.Add(this.label20);
            this.panel6.Controls.Add(this.label3);
            this.panel6.Controls.Add(this.OriginWidthText);
            this.panel6.Controls.Add(this.AreaTextWidth);
            this.panel6.Controls.Add(this.OriginHeightText);
            this.panel6.Controls.Add(this.label19);
            this.panel6.Controls.Add(this.AreaTextHeight);
            this.panel6.Controls.Add(this.OriginYText);
            this.panel6.Controls.Add(this.label1);
            this.panel6.Controls.Add(this.label15);
            this.panel6.Controls.Add(this.AreaTextY);
            this.panel6.Controls.Add(this.label2);
            this.panel6.Location = new System.Drawing.Point(16, 16);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(411, 169);
            this.panel6.TabIndex = 11;
            // 
            // OriginDisplay
            // 
            this.OriginDisplay.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.OriginDisplay.AutoSize = true;
            this.OriginDisplay.Location = new System.Drawing.Point(308, 114);
            this.OriginDisplay.Name = "OriginDisplay";
            this.OriginDisplay.Size = new System.Drawing.Size(71, 17);
            this.OriginDisplay.TabIndex = 3;
            this.OriginDisplay.Text = "Is Display";
            this.OriginDisplay.UseVisualStyleBackColor = true;
            this.OriginDisplay.CheckedChanged += new System.EventHandler(this.OriginDisplay_CheckedChanged);
            // 
            // AreaDisplay
            // 
            this.AreaDisplay.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.AreaDisplay.AutoSize = true;
            this.AreaDisplay.Location = new System.Drawing.Point(103, 114);
            this.AreaDisplay.Name = "AreaDisplay";
            this.AreaDisplay.Size = new System.Drawing.Size(71, 17);
            this.AreaDisplay.TabIndex = 3;
            this.AreaDisplay.Text = "Is Display";
            this.AreaDisplay.UseVisualStyleBackColor = true;
            // 
            // BtnDefineOrigin
            // 
            this.BtnDefineOrigin.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.BtnDefineOrigin.Location = new System.Drawing.Point(296, 140);
            this.BtnDefineOrigin.Name = "BtnDefineOrigin";
            this.BtnDefineOrigin.Size = new System.Drawing.Size(100, 23);
            this.BtnDefineOrigin.TabIndex = 2;
            this.BtnDefineOrigin.Text = "Set";
            this.BtnDefineOrigin.UseVisualStyleBackColor = true;
            this.BtnDefineOrigin.Click += new System.EventHandler(this.BtnDefineOrigin_Click);
            // 
            // BtnSetROI
            // 
            this.BtnSetROI.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.BtnSetROI.Location = new System.Drawing.Point(86, 140);
            this.BtnSetROI.Name = "BtnSetROI";
            this.BtnSetROI.Size = new System.Drawing.Size(100, 23);
            this.BtnSetROI.TabIndex = 2;
            this.BtnSetROI.Text = "Set";
            this.BtnSetROI.UseVisualStyleBackColor = true;
            // 
            // OriginXText
            // 
            this.OriginXText.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.OriginXText.Location = new System.Drawing.Point(296, 10);
            this.OriginXText.Name = "OriginXText";
            this.OriginXText.Size = new System.Drawing.Size(100, 20);
            this.OriginXText.TabIndex = 1;
            // 
            // AreaTextX
            // 
            this.AreaTextX.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.AreaTextX.Location = new System.Drawing.Point(86, 10);
            this.AreaTextX.Name = "AreaTextX";
            this.AreaTextX.Size = new System.Drawing.Size(100, 20);
            this.AreaTextX.TabIndex = 1;
            // 
            // label21
            // 
            this.label21.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(217, 91);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(74, 13);
            this.label21.TabIndex = 0;
            this.label21.Text = "Origin Height :";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 91);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Area Height :";
            // 
            // label20
            // 
            this.label20.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(218, 65);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(71, 13);
            this.label20.TabIndex = 0;
            this.label20.Text = "Origin Width :";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Area Width :";
            // 
            // OriginWidthText
            // 
            this.OriginWidthText.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.OriginWidthText.Location = new System.Drawing.Point(296, 62);
            this.OriginWidthText.Name = "OriginWidthText";
            this.OriginWidthText.Size = new System.Drawing.Size(100, 20);
            this.OriginWidthText.TabIndex = 1;
            // 
            // AreaTextWidth
            // 
            this.AreaTextWidth.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.AreaTextWidth.Location = new System.Drawing.Point(86, 62);
            this.AreaTextWidth.Name = "AreaTextWidth";
            this.AreaTextWidth.Size = new System.Drawing.Size(100, 20);
            this.AreaTextWidth.TabIndex = 1;
            // 
            // OriginHeightText
            // 
            this.OriginHeightText.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.OriginHeightText.Location = new System.Drawing.Point(296, 88);
            this.OriginHeightText.Name = "OriginHeightText";
            this.OriginHeightText.Size = new System.Drawing.Size(100, 20);
            this.OriginHeightText.TabIndex = 1;
            // 
            // label19
            // 
            this.label19.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(241, 13);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(50, 13);
            this.label19.TabIndex = 0;
            this.label19.Text = "Origin X :";
            // 
            // AreaTextHeight
            // 
            this.AreaTextHeight.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.AreaTextHeight.Location = new System.Drawing.Point(86, 88);
            this.AreaTextHeight.Name = "AreaTextHeight";
            this.AreaTextHeight.Size = new System.Drawing.Size(100, 20);
            this.AreaTextHeight.TabIndex = 1;
            // 
            // OriginYText
            // 
            this.OriginYText.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.OriginYText.Location = new System.Drawing.Point(296, 36);
            this.OriginYText.Name = "OriginYText";
            this.OriginYText.Size = new System.Drawing.Size(100, 20);
            this.OriginYText.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(38, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Area X :";
            // 
            // label15
            // 
            this.label15.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(241, 39);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(50, 13);
            this.label15.TabIndex = 0;
            this.label15.Text = "Origin Y :";
            // 
            // AreaTextY
            // 
            this.AreaTextY.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.AreaTextY.Location = new System.Drawing.Point(86, 36);
            this.AreaTextY.Name = "AreaTextY";
            this.AreaTextY.Size = new System.Drawing.Size(100, 20);
            this.AreaTextY.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(38, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Area Y :";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.FocusPointLable);
            this.panel4.Controls.Add(this.FocusPoint);
            this.panel4.Controls.Add(this.Histogramcheck);
            this.panel4.Controls.Add(this.CameraList);
            this.panel4.Controls.Add(this.label22);
            this.panel4.Controls.Add(this.SetCameraId);
            this.panel4.Location = new System.Drawing.Point(16, 198);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(411, 100);
            this.panel4.TabIndex = 10;
            // 
            // FocusPointLable
            // 
            this.FocusPointLable.AutoSize = true;
            this.FocusPointLable.Location = new System.Drawing.Point(158, 73);
            this.FocusPointLable.Name = "FocusPointLable";
            this.FocusPointLable.Size = new System.Drawing.Size(60, 13);
            this.FocusPointLable.TabIndex = 9;
            this.FocusPointLable.Text = "FocusPoint";
            // 
            // FocusPoint
            // 
            this.FocusPoint.FormattingEnabled = true;
            this.FocusPoint.Items.AddRange(new object[] {
            "21 Focus Points",
            "15 Focus Points",
            "9 Focus Points",
            "Histogram"});
            this.FocusPoint.Location = new System.Drawing.Point(224, 69);
            this.FocusPoint.Name = "FocusPoint";
            this.FocusPoint.Size = new System.Drawing.Size(121, 21);
            this.FocusPoint.TabIndex = 8;
            // 
            // Histogramcheck
            // 
            this.Histogramcheck.AutoSize = true;
            this.Histogramcheck.Location = new System.Drawing.Point(3, 69);
            this.Histogramcheck.Name = "Histogramcheck";
            this.Histogramcheck.Size = new System.Drawing.Size(73, 17);
            this.Histogramcheck.TabIndex = 7;
            this.Histogramcheck.Text = "Histogram";
            this.Histogramcheck.UseVisualStyleBackColor = true;
            // 
            // CameraList
            // 
            this.CameraList.FormattingEnabled = true;
            this.CameraList.Location = new System.Drawing.Point(89, 9);
            this.CameraList.Name = "CameraList";
            this.CameraList.Size = new System.Drawing.Size(217, 21);
            this.CameraList.TabIndex = 6;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(29, 12);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(54, 13);
            this.label22.TabIndex = 5;
            this.label22.Text = "CameraID";
            // 
            // SetCameraId
            // 
            this.SetCameraId.Location = new System.Drawing.Point(312, 9);
            this.SetCameraId.Name = "SetCameraId";
            this.SetCameraId.Size = new System.Drawing.Size(75, 23);
            this.SetCameraId.TabIndex = 4;
            this.SetCameraId.Text = "Set";
            this.SetCameraId.UseVisualStyleBackColor = true;
            this.SetCameraId.Click += new System.EventHandler(this.SetCameraId_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.panel1);
            this.tabPage2.Controls.Add(this.HoughCircles);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(840, 345);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Process Image";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label29);
            this.panel1.Controls.Add(this.autoHDR);
            this.panel1.Controls.Add(this.ClearProfilePixel);
            this.panel1.Controls.Add(this.SaveProfilePixel);
            this.panel1.Controls.Add(this.label28);
            this.panel1.Controls.Add(this.ProfilePixelValues);
            this.panel1.Controls.Add(this.SavePixelValues);
            this.panel1.Controls.Add(this.label27);
            this.panel1.Controls.Add(this.pixelValuesmin);
            this.panel1.Controls.Add(this.pixelvalues_max);
            this.panel1.Controls.Add(this.pixelvalues_min);
            this.panel1.Controls.Add(this.keoGrams);
            this.panel1.Controls.Add(this.hdr_On);
            this.panel1.Controls.Add(this.cannyThreshold_Box);
            this.panel1.Controls.Add(this.Clear_Profile);
            this.panel1.Controls.Add(this.label24);
            this.panel1.Controls.Add(this.label26);
            this.panel1.Controls.Add(this.Save_HoughCircles);
            this.panel1.Controls.Add(this.HoughCircles_Profile);
            this.panel1.Controls.Add(this.SaveProfile_HoughCircles);
            this.panel1.Controls.Add(this.label25);
            this.panel1.Controls.Add(this.circleAccumulatorThreshold_Box);
            this.panel1.Location = new System.Drawing.Point(401, 6);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(432, 339);
            this.panel1.TabIndex = 12;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(3, 12);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(73, 13);
            this.label29.TabIndex = 24;
            this.label29.Text = "Hough Circles";
            // 
            // autoHDR
            // 
            this.autoHDR.AutoSize = true;
            this.autoHDR.Location = new System.Drawing.Point(89, 159);
            this.autoHDR.Name = "autoHDR";
            this.autoHDR.Size = new System.Drawing.Size(83, 17);
            this.autoHDR.TabIndex = 23;
            this.autoHDR.Text = "AUTO HDR";
            this.autoHDR.UseVisualStyleBackColor = true;
            // 
            // ClearProfilePixel
            // 
            this.ClearProfilePixel.Location = new System.Drawing.Point(289, 287);
            this.ClearProfilePixel.Name = "ClearProfilePixel";
            this.ClearProfilePixel.Size = new System.Drawing.Size(75, 23);
            this.ClearProfilePixel.TabIndex = 22;
            this.ClearProfilePixel.Text = "ClearAllProfile";
            this.ClearProfilePixel.UseVisualStyleBackColor = true;
            this.ClearProfilePixel.Click += new System.EventHandler(this.ClearProfilePixel_Click);
            // 
            // SaveProfilePixel
            // 
            this.SaveProfilePixel.Location = new System.Drawing.Point(289, 258);
            this.SaveProfilePixel.Name = "SaveProfilePixel";
            this.SaveProfilePixel.Size = new System.Drawing.Size(75, 23);
            this.SaveProfilePixel.TabIndex = 21;
            this.SaveProfilePixel.Text = "SaveProfile";
            this.SaveProfilePixel.UseVisualStyleBackColor = true;
            this.SaveProfilePixel.Click += new System.EventHandler(this.SaveProfilePixel_Click);
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(286, 207);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(36, 13);
            this.label28.TabIndex = 20;
            this.label28.Text = "Profile";
            // 
            // ProfilePixelValues
            // 
            this.ProfilePixelValues.FormattingEnabled = true;
            this.ProfilePixelValues.Location = new System.Drawing.Point(289, 225);
            this.ProfilePixelValues.Name = "ProfilePixelValues";
            this.ProfilePixelValues.Size = new System.Drawing.Size(121, 21);
            this.ProfilePixelValues.TabIndex = 19;
            this.ProfilePixelValues.SelectedIndexChanged += new System.EventHandler(this.ProfilePixelValues_SelectedIndexChanged);
            // 
            // SavePixelValues
            // 
            this.SavePixelValues.Location = new System.Drawing.Point(97, 260);
            this.SavePixelValues.Name = "SavePixelValues";
            this.SavePixelValues.Size = new System.Drawing.Size(75, 23);
            this.SavePixelValues.TabIndex = 18;
            this.SavePixelValues.Text = "Save";
            this.SavePixelValues.UseVisualStyleBackColor = true;
            this.SavePixelValues.Click += new System.EventHandler(this.SavePixelValues_Click);
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.CausesValidation = false;
            this.label27.Location = new System.Drawing.Point(154, 207);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(87, 13);
            this.label27.TabIndex = 17;
            this.label27.Text = "Pixel Values Max";
            // 
            // pixelValuesmin
            // 
            this.pixelValuesmin.AutoSize = true;
            this.pixelValuesmin.CausesValidation = false;
            this.pixelValuesmin.Location = new System.Drawing.Point(20, 207);
            this.pixelValuesmin.Name = "pixelValuesmin";
            this.pixelValuesmin.Size = new System.Drawing.Size(84, 13);
            this.pixelValuesmin.TabIndex = 16;
            this.pixelValuesmin.Text = "Pixel Values Min";
            // 
            // pixelvalues_max
            // 
            this.pixelvalues_max.Location = new System.Drawing.Point(157, 226);
            this.pixelvalues_max.Name = "pixelvalues_max";
            this.pixelvalues_max.Size = new System.Drawing.Size(100, 20);
            this.pixelvalues_max.TabIndex = 15;
            // 
            // pixelvalues_min
            // 
            this.pixelvalues_min.Location = new System.Drawing.Point(20, 226);
            this.pixelvalues_min.Name = "pixelvalues_min";
            this.pixelvalues_min.Size = new System.Drawing.Size(100, 20);
            this.pixelvalues_min.TabIndex = 14;
            // 
            // keoGrams
            // 
            this.keoGrams.AutoSize = true;
            this.keoGrams.Location = new System.Drawing.Point(184, 159);
            this.keoGrams.Name = "keoGrams";
            this.keoGrams.Size = new System.Drawing.Size(73, 17);
            this.keoGrams.TabIndex = 13;
            this.keoGrams.Text = "Keograms";
            this.keoGrams.UseVisualStyleBackColor = true;
            // 
            // hdr_On
            // 
            this.hdr_On.AutoSize = true;
            this.hdr_On.Location = new System.Drawing.Point(20, 159);
            this.hdr_On.Name = "hdr_On";
            this.hdr_On.Size = new System.Drawing.Size(50, 17);
            this.hdr_On.TabIndex = 12;
            this.hdr_On.Text = "HDR";
            this.hdr_On.UseVisualStyleBackColor = true;
            // 
            // cannyThreshold_Box
            // 
            this.cannyThreshold_Box.Location = new System.Drawing.Point(20, 53);
            this.cannyThreshold_Box.Name = "cannyThreshold_Box";
            this.cannyThreshold_Box.Size = new System.Drawing.Size(100, 20);
            this.cannyThreshold_Box.TabIndex = 3;
            // 
            // Clear_Profile
            // 
            this.Clear_Profile.Location = new System.Drawing.Point(289, 104);
            this.Clear_Profile.Name = "Clear_Profile";
            this.Clear_Profile.Size = new System.Drawing.Size(75, 23);
            this.Clear_Profile.TabIndex = 11;
            this.Clear_Profile.Text = "ClearAllProfile";
            this.Clear_Profile.UseVisualStyleBackColor = true;
            this.Clear_Profile.Click += new System.EventHandler(this.Clear_Profile_Click);
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(17, 33);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(83, 13);
            this.label24.TabIndex = 4;
            this.label24.Text = "cannyThreshold";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(286, 36);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(36, 13);
            this.label26.TabIndex = 10;
            this.label26.Text = "Profile";
            // 
            // Save_HoughCircles
            // 
            this.Save_HoughCircles.Location = new System.Drawing.Point(97, 79);
            this.Save_HoughCircles.Name = "Save_HoughCircles";
            this.Save_HoughCircles.Size = new System.Drawing.Size(75, 23);
            this.Save_HoughCircles.TabIndex = 7;
            this.Save_HoughCircles.Text = "Save";
            this.Save_HoughCircles.UseVisualStyleBackColor = true;
            this.Save_HoughCircles.Click += new System.EventHandler(this.Save_HoughCircles_Click);
            // 
            // HoughCircles_Profile
            // 
            this.HoughCircles_Profile.FormattingEnabled = true;
            this.HoughCircles_Profile.Location = new System.Drawing.Point(289, 52);
            this.HoughCircles_Profile.Name = "HoughCircles_Profile";
            this.HoughCircles_Profile.Size = new System.Drawing.Size(121, 21);
            this.HoughCircles_Profile.TabIndex = 9;
            this.HoughCircles_Profile.SelectedIndexChanged += new System.EventHandler(this.HoughCircles_Profile_SelectedIndexChanged);
            // 
            // SaveProfile_HoughCircles
            // 
            this.SaveProfile_HoughCircles.Location = new System.Drawing.Point(289, 78);
            this.SaveProfile_HoughCircles.Name = "SaveProfile_HoughCircles";
            this.SaveProfile_HoughCircles.Size = new System.Drawing.Size(75, 23);
            this.SaveProfile_HoughCircles.TabIndex = 8;
            this.SaveProfile_HoughCircles.Text = "SaveProfile";
            this.SaveProfile_HoughCircles.UseVisualStyleBackColor = true;
            this.SaveProfile_HoughCircles.Click += new System.EventHandler(this.SaveProfile_HoughCircles_Click);
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(119, 30);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(138, 13);
            this.label25.TabIndex = 5;
            this.label25.Text = "circleAccumulatorThreshold";
            // 
            // circleAccumulatorThreshold_Box
            // 
            this.circleAccumulatorThreshold_Box.Location = new System.Drawing.Point(157, 53);
            this.circleAccumulatorThreshold_Box.Name = "circleAccumulatorThreshold_Box";
            this.circleAccumulatorThreshold_Box.Size = new System.Drawing.Size(99, 20);
            this.circleAccumulatorThreshold_Box.TabIndex = 6;
            // 
            // HoughCircles
            // 
            this.HoughCircles.Location = new System.Drawing.Point(6, 6);
            this.HoughCircles.Name = "HoughCircles";
            this.HoughCircles.Size = new System.Drawing.Size(389, 338);
            this.HoughCircles.TabIndex = 2;
            this.HoughCircles.TabStop = false;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.StopSave);
            this.tabPage3.Controls.Add(this.label30);
            this.tabPage3.Controls.Add(this.SaveLog);
            this.tabPage3.Controls.Add(this.Savebutton);
            this.tabPage3.Controls.Add(this.SavePath);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(840, 345);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Save";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // StopSave
            // 
            this.StopSave.AutoSize = true;
            this.StopSave.Location = new System.Drawing.Point(334, 35);
            this.StopSave.Name = "StopSave";
            this.StopSave.Size = new System.Drawing.Size(73, 17);
            this.StopSave.TabIndex = 4;
            this.StopSave.Text = "StopSave";
            this.StopSave.UseVisualStyleBackColor = true;
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(47, 16);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(61, 13);
            this.label30.TabIndex = 3;
            this.label30.Text = "ImageSave";
            // 
            // SaveLog
            // 
            this.SaveLog.AutoSize = true;
            this.SaveLog.Location = new System.Drawing.Point(50, 58);
            this.SaveLog.Name = "SaveLog";
            this.SaveLog.Size = new System.Drawing.Size(69, 17);
            this.SaveLog.TabIndex = 2;
            this.SaveLog.Text = "SaveLog";
            this.SaveLog.UseVisualStyleBackColor = true;
            // 
            // Savebutton
            // 
            this.Savebutton.Location = new System.Drawing.Point(253, 28);
            this.Savebutton.Name = "Savebutton";
            this.Savebutton.Size = new System.Drawing.Size(75, 23);
            this.Savebutton.TabIndex = 1;
            this.Savebutton.Text = "Save";
            this.Savebutton.UseVisualStyleBackColor = true;
            this.Savebutton.Click += new System.EventHandler(this.Savebutton_Click);
            // 
            // SavePath
            // 
            this.SavePath.Location = new System.Drawing.Point(50, 32);
            this.SavePath.Name = "SavePath";
            this.SavePath.Size = new System.Drawing.Size(183, 20);
            this.SavePath.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.panel5);
            this.groupBox3.Controls.Add(this.groupBox5);
            this.groupBox3.Controls.Add(this.groupBox4);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Right;
            this.groupBox3.Location = new System.Drawing.Point(848, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(492, 371);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Camera State";
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.SpeedMode);
            this.panel5.Controls.Add(this.ShowFocusPoint);
            this.panel5.Controls.Add(this.checkBoxAverage);
            this.panel5.Controls.Add(this.ResetZoom);
            this.panel5.Controls.Add(this.checkBoxCenter);
            this.panel5.Controls.Add(this.ExpouseTimeText);
            this.panel5.Controls.Add(this.label23);
            this.panel5.Controls.Add(this.IsAutoExposureTime);
            this.panel5.Controls.Add(this.label5);
            this.panel5.Controls.Add(this.label6);
            this.panel5.Location = new System.Drawing.Point(45, 261);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(395, 100);
            this.panel5.TabIndex = 14;
            // 
            // SpeedMode
            // 
            this.SpeedMode.AutoSize = true;
            this.SpeedMode.Location = new System.Drawing.Point(194, 31);
            this.SpeedMode.Name = "SpeedMode";
            this.SpeedMode.Size = new System.Drawing.Size(84, 17);
            this.SpeedMode.TabIndex = 13;
            this.SpeedMode.Text = "SpeedMode";
            this.SpeedMode.UseVisualStyleBackColor = true;
            // 
            // ShowFocusPoint
            // 
            this.ShowFocusPoint.AutoSize = true;
            this.ShowFocusPoint.Location = new System.Drawing.Point(257, 72);
            this.ShowFocusPoint.Name = "ShowFocusPoint";
            this.ShowFocusPoint.Size = new System.Drawing.Size(108, 17);
            this.ShowFocusPoint.TabIndex = 11;
            this.ShowFocusPoint.Text = "Show focus point";
            this.ShowFocusPoint.UseVisualStyleBackColor = true;
            // 
            // checkBoxAverage
            // 
            this.checkBoxAverage.AutoSize = true;
            this.checkBoxAverage.Location = new System.Drawing.Point(72, 72);
            this.checkBoxAverage.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxAverage.Name = "checkBoxAverage";
            this.checkBoxAverage.Size = new System.Drawing.Size(66, 17);
            this.checkBoxAverage.TabIndex = 9;
            this.checkBoxAverage.Text = "Average";
            this.checkBoxAverage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBoxAverage.UseVisualStyleBackColor = true;
            // 
            // ResetZoom
            // 
            this.ResetZoom.Location = new System.Drawing.Point(223, 5);
            this.ResetZoom.Name = "ResetZoom";
            this.ResetZoom.Size = new System.Drawing.Size(75, 23);
            this.ResetZoom.TabIndex = 4;
            this.ResetZoom.Text = "ResetZoom";
            this.ResetZoom.UseVisualStyleBackColor = true;
            this.ResetZoom.Click += new System.EventHandler(this.ResetZoom_Click);
            // 
            // checkBoxCenter
            // 
            this.checkBoxCenter.AutoSize = true;
            this.checkBoxCenter.Location = new System.Drawing.Point(144, 72);
            this.checkBoxCenter.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxCenter.Name = "checkBoxCenter";
            this.checkBoxCenter.Size = new System.Drawing.Size(57, 17);
            this.checkBoxCenter.TabIndex = 8;
            this.checkBoxCenter.Text = "Center";
            this.checkBoxCenter.UseVisualStyleBackColor = true;
            // 
            // ExpouseTimeText
            // 
            this.ExpouseTimeText.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ExpouseTimeText.DecimalPlaces = 2;
            this.ExpouseTimeText.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.ExpouseTimeText.Location = new System.Drawing.Point(75, 6);
            this.ExpouseTimeText.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.ExpouseTimeText.Name = "ExpouseTimeText";
            this.ExpouseTimeText.Size = new System.Drawing.Size(100, 20);
            this.ExpouseTimeText.TabIndex = 3;
            this.ExpouseTimeText.ValueChanged += new System.EventHandler(this.ExpouseTimeText_ValueChanged);
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(9, 50);
            this.label23.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(83, 13);
            this.label23.TabIndex = 7;
            this.label23.Text = "Exposure Focus";
            // 
            // IsAutoExposureTime
            // 
            this.IsAutoExposureTime.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.IsAutoExposureTime.AutoSize = true;
            this.IsAutoExposureTime.Location = new System.Drawing.Point(75, 31);
            this.IsAutoExposureTime.Name = "IsAutoExposureTime";
            this.IsAutoExposureTime.Size = new System.Drawing.Size(116, 17);
            this.IsAutoExposureTime.TabIndex = 2;
            this.IsAutoExposureTime.Text = "Auto exposure time";
            this.IsAutoExposureTime.UseVisualStyleBackColor = true;
            this.IsAutoExposureTime.CheckedChanged += new System.EventHandler(this.IsAutoExposureTime_CheckedChanged);
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 8);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Exp Time :";
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(181, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(20, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "ms";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.ROITextX);
            this.groupBox5.Controls.Add(this.label11);
            this.groupBox5.Controls.Add(this.ROITextWidth);
            this.groupBox5.Controls.Add(this.GetDataFailedText);
            this.groupBox5.Controls.Add(this.label14);
            this.groupBox5.Controls.Add(this.ExposuringText);
            this.groupBox5.Controls.Add(this.label7);
            this.groupBox5.Controls.Add(this.CameraStateText);
            this.groupBox5.Controls.Add(this.label12);
            this.groupBox5.Controls.Add(this.label8);
            this.groupBox5.Controls.Add(this.label10);
            this.groupBox5.Controls.Add(this.ROITextHeight);
            this.groupBox5.Controls.Add(this.ROITextY);
            this.groupBox5.Controls.Add(this.label9);
            this.groupBox5.Location = new System.Drawing.Point(17, 25);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(208, 230);
            this.groupBox5.TabIndex = 3;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "State";
            // 
            // ROITextX
            // 
            this.ROITextX.Location = new System.Drawing.Point(97, 26);
            this.ROITextX.Name = "ROITextX";
            this.ROITextX.Size = new System.Drawing.Size(100, 20);
            this.ROITextX.TabIndex = 1;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(14, 133);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(77, 13);
            this.label11.TabIndex = 0;
            this.label11.Text = "Camera State :";
            // 
            // ROITextWidth
            // 
            this.ROITextWidth.Location = new System.Drawing.Point(97, 78);
            this.ROITextWidth.Name = "ROITextWidth";
            this.ROITextWidth.Size = new System.Drawing.Size(100, 20);
            this.ROITextWidth.TabIndex = 1;
            // 
            // GetDataFailedText
            // 
            this.GetDataFailedText.Location = new System.Drawing.Point(97, 182);
            this.GetDataFailedText.Name = "GetDataFailedText";
            this.GetDataFailedText.Size = new System.Drawing.Size(100, 20);
            this.GetDataFailedText.TabIndex = 1;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(11, 185);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(82, 13);
            this.label14.TabIndex = 0;
            this.label14.Text = "Get data failed :";
            // 
            // ExposuringText
            // 
            this.ExposuringText.Location = new System.Drawing.Point(97, 156);
            this.ExposuringText.Name = "ExposuringText";
            this.ExposuringText.Size = new System.Drawing.Size(100, 20);
            this.ExposuringText.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(28, 159);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Exposuring :";
            // 
            // CameraStateText
            // 
            this.CameraStateText.Location = new System.Drawing.Point(97, 130);
            this.CameraStateText.Name = "CameraStateText";
            this.CameraStateText.Size = new System.Drawing.Size(100, 20);
            this.CameraStateText.TabIndex = 1;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(49, 55);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(42, 13);
            this.label12.TabIndex = 0;
            this.label12.Text = "ROI Y :";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(49, 29);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(42, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "ROI X :";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(25, 107);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(66, 13);
            this.label10.TabIndex = 0;
            this.label10.Text = "ROI Height :";
            // 
            // ROITextHeight
            // 
            this.ROITextHeight.Location = new System.Drawing.Point(97, 104);
            this.ROITextHeight.Name = "ROITextHeight";
            this.ROITextHeight.Size = new System.Drawing.Size(100, 20);
            this.ROITextHeight.TabIndex = 1;
            // 
            // ROITextY
            // 
            this.ROITextY.Location = new System.Drawing.Point(97, 52);
            this.ROITextY.Name = "ROITextY";
            this.ROITextY.Size = new System.Drawing.Size(100, 20);
            this.ROITextY.TabIndex = 1;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(28, 81);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(63, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "ROI Width :";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.BtnSetCameraSetting);
            this.groupBox4.Controls.Add(this.MIN_ISOText);
            this.groupBox4.Controls.Add(this.label13);
            this.groupBox4.Controls.Add(this.MIN_SHUTTERText);
            this.groupBox4.Controls.Add(this.MIN_APERTUREText);
            this.groupBox4.Controls.Add(this.label18);
            this.groupBox4.Controls.Add(this.MAX_ISOTextL);
            this.groupBox4.Controls.Add(this.MAX_SHUTTERText);
            this.groupBox4.Controls.Add(this.label16);
            this.groupBox4.Controls.Add(this.label17);
            this.groupBox4.Controls.Add(this.MAX_ISOText);
            this.groupBox4.Location = new System.Drawing.Point(231, 25);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(255, 230);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Setting";
            // 
            // BtnSetCameraSetting
            // 
            this.BtnSetCameraSetting.Location = new System.Drawing.Point(130, 156);
            this.BtnSetCameraSetting.Name = "BtnSetCameraSetting";
            this.BtnSetCameraSetting.Size = new System.Drawing.Size(100, 23);
            this.BtnSetCameraSetting.TabIndex = 2;
            this.BtnSetCameraSetting.Text = "Set";
            this.BtnSetCameraSetting.UseVisualStyleBackColor = true;
            this.BtnSetCameraSetting.Click += new System.EventHandler(this.BtnSetCameraSetting_Click);
            // 
            // MIN_ISOText
            // 
            this.MIN_ISOText.Location = new System.Drawing.Point(130, 26);
            this.MIN_ISOText.Name = "MIN_ISOText";
            this.MIN_ISOText.Size = new System.Drawing.Size(100, 20);
            this.MIN_ISOText.TabIndex = 1;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(26, 133);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(98, 13);
            this.label13.TabIndex = 0;
            this.label13.Text = "MIN_APERTURE :";
            // 
            // MIN_SHUTTERText
            // 
            this.MIN_SHUTTERText.Location = new System.Drawing.Point(130, 78);
            this.MIN_SHUTTERText.Name = "MIN_SHUTTERText";
            this.MIN_SHUTTERText.Size = new System.Drawing.Size(100, 20);
            this.MIN_SHUTTERText.TabIndex = 1;
            // 
            // MIN_APERTUREText
            // 
            this.MIN_APERTUREText.Location = new System.Drawing.Point(130, 130);
            this.MIN_APERTUREText.Name = "MIN_APERTUREText";
            this.MIN_APERTUREText.Size = new System.Drawing.Size(100, 20);
            this.MIN_APERTUREText.TabIndex = 1;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(67, 29);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(57, 13);
            this.label18.TabIndex = 0;
            this.label18.Text = "MIN_ISO :";
            // 
            // MAX_ISOTextL
            // 
            this.MAX_ISOTextL.AutoSize = true;
            this.MAX_ISOTextL.Location = new System.Drawing.Point(64, 55);
            this.MAX_ISOTextL.Name = "MAX_ISOTextL";
            this.MAX_ISOTextL.Size = new System.Drawing.Size(60, 13);
            this.MAX_ISOTextL.TabIndex = 0;
            this.MAX_ISOTextL.Text = "MAX_ISO :";
            // 
            // MAX_SHUTTERText
            // 
            this.MAX_SHUTTERText.Location = new System.Drawing.Point(130, 104);
            this.MAX_SHUTTERText.Name = "MAX_SHUTTERText";
            this.MAX_SHUTTERText.Size = new System.Drawing.Size(100, 20);
            this.MAX_SHUTTERText.TabIndex = 1;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(30, 107);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(94, 13);
            this.label16.TabIndex = 0;
            this.label16.Text = "MAX_SHUTTER :";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(33, 81);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(91, 13);
            this.label17.TabIndex = 0;
            this.label17.Text = "MIN_SHUTTER :";
            // 
            // MAX_ISOText
            // 
            this.MAX_ISOText.Location = new System.Drawing.Point(130, 52);
            this.MAX_ISOText.Name = "MAX_ISOText";
            this.MAX_ISOText.Size = new System.Drawing.Size(100, 20);
            this.MAX_ISOText.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.XYPosText);
            this.panel2.Controls.Add(this.MessageStatusText);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(5, 820);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.panel2.Size = new System.Drawing.Size(1340, 20);
            this.panel2.TabIndex = 4;
            // 
            // XYPosText
            // 
            this.XYPosText.AutoSize = true;
            this.XYPosText.Dock = System.Windows.Forms.DockStyle.Right;
            this.XYPosText.Location = new System.Drawing.Point(1277, 3);
            this.XYPosText.Name = "XYPosText";
            this.XYPosText.Size = new System.Drawing.Size(63, 13);
            this.XYPosText.TabIndex = 1;
            this.XYPosText.Text = "X = 0, Y = 0";
            // 
            // MessageStatusText
            // 
            this.MessageStatusText.AutoSize = true;
            this.MessageStatusText.Dock = System.Windows.Forms.DockStyle.Left;
            this.MessageStatusText.Location = new System.Drawing.Point(0, 3);
            this.MessageStatusText.Name = "MessageStatusText";
            this.MessageStatusText.Size = new System.Drawing.Size(38, 13);
            this.MessageStatusText.TabIndex = 0;
            this.MessageStatusText.Text = "Ready";
            // 
            // UITimer
            // 
            this.UITimer.Enabled = true;
            this.UITimer.Interval = 50;
            this.UITimer.Tick += new System.EventHandler(this.UITimer_Tick);
            // 
            // MainWindows
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1350, 845);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel2);
            this.Name = "MainWindows";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AllSky-2020";
            this.Load += new System.EventHandler(this.MainWindows_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.MainImageControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ROIImage)).EndInit();
            this.TabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HoughCircles)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ExpouseTimeText)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private Emgu.CV.UI.ImageBox MainImageControl;
        private Emgu.CV.UI.ImageBox ROIImage;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.NumericUpDown ExpouseTimeText;
        private System.Windows.Forms.CheckBox IsAutoExposureTime;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Button BtnSetROI;
        private System.Windows.Forms.TextBox AreaTextX;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox AreaTextWidth;
        private System.Windows.Forms.TextBox AreaTextHeight;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox AreaTextY;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox ROITextX;
        private System.Windows.Forms.TextBox CameraStateText;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox ROITextHeight;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox ROITextY;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox ROITextWidth;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label XYPosText;
        private System.Windows.Forms.Label MessageStatusText;
        private System.Windows.Forms.Timer UITimer;
        private System.Windows.Forms.TextBox ExposuringText;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox MIN_ISOText;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox MIN_SHUTTERText;
        private System.Windows.Forms.TextBox MIN_APERTUREText;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label MAX_ISOTextL;
        private System.Windows.Forms.TextBox MAX_SHUTTERText;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox MAX_ISOText;
        private System.Windows.Forms.Button BtnSetCameraSetting;
        private System.Windows.Forms.TextBox GetDataFailedText;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox OriginXText;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox OriginWidthText;
        private System.Windows.Forms.TextBox OriginHeightText;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox OriginYText;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Button BtnDefineOrigin;
        private System.Windows.Forms.CheckBox OriginDisplay;
        private System.Windows.Forms.CheckBox AreaDisplay;
        private System.Windows.Forms.Button Savebutton;
        private System.Windows.Forms.TextBox SavePath;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Button SetCameraId;
        private System.Windows.Forms.Button ResetZoom;
        private System.Windows.Forms.ComboBox CameraList;
        private System.Windows.Forms.TabControl TabControl;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.CheckBox checkBoxAverage;
        private System.Windows.Forms.CheckBox checkBoxCenter;
        private System.Windows.Forms.CheckBox Histogramcheck;
        private System.Windows.Forms.CheckBox ShowFocusPoint;
        private Emgu.CV.UI.ImageBox HoughCircles;
        private System.Windows.Forms.ComboBox FocusPoint;
        private System.Windows.Forms.Label FocusPointLable;
        private System.Windows.Forms.CheckBox SpeedMode;
        private System.Windows.Forms.TextBox cannyThreshold_Box;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox circleAccumulatorThreshold_Box;
        private System.Windows.Forms.Button Save_HoughCircles;
        private System.Windows.Forms.Button SaveProfile_HoughCircles;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.ComboBox HoughCircles_Profile;
        private System.Windows.Forms.Button Clear_Profile;
        private Emgu.CV.UI.HistogramBox Histo;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.CheckBox hdr_On;
        private System.Windows.Forms.CheckBox SaveLog;
        private System.Windows.Forms.CheckBox keoGrams;
        private System.Windows.Forms.Button ClearProfilePixel;
        private System.Windows.Forms.Button SaveProfilePixel;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.ComboBox ProfilePixelValues;
        private System.Windows.Forms.Button SavePixelValues;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label pixelValuesmin;
        private System.Windows.Forms.TextBox pixelvalues_max;
        private System.Windows.Forms.TextBox pixelvalues_min;
        private System.Windows.Forms.CheckBox autoHDR;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.CheckBox StopSave;
        private System.Windows.Forms.Label label30;
    }
}

