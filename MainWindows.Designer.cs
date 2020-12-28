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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.MainImageControl = new Emgu.CV.UI.ImageBox();
            this.ROIImage = new Emgu.CV.UI.ImageBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.TabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.Histogram = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.Histogramcheck = new System.Windows.Forms.CheckBox();
            this.CameraList = new System.Windows.Forms.ComboBox();
            this.label22 = new System.Windows.Forms.Label();
            this.SetCameraId = new System.Windows.Forms.Button();
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
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.Savebutton = new System.Windows.Forms.Button();
            this.SavePath = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.ShowFocusPoint = new System.Windows.Forms.CheckBox();
            this.checkBoxAverage = new System.Windows.Forms.CheckBox();
            this.ResetZoom = new System.Windows.Forms.Button();
            this.checkBoxCenter = new System.Windows.Forms.CheckBox();
            this.ExpouseTimeText = new System.Windows.Forms.NumericUpDown();
            this.label23 = new System.Windows.Forms.Label();
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
            this.IsAutoExposureTime = new System.Windows.Forms.CheckBox();
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
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
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
            ((System.ComponentModel.ISupportInitialize)(this.Histogram)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.groupBox3.SuspendLayout();
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
            this.splitContainer1.Size = new System.Drawing.Size(1340, 831);
            this.splitContainer1.SplitterDistance = 450;
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
            this.splitContainer3.Size = new System.Drawing.Size(1340, 450);
            this.splitContainer3.SplitterDistance = 690;
            this.splitContainer3.TabIndex = 4;
            // 
            // MainImageControl
            // 
            this.MainImageControl.BackColor = System.Drawing.Color.LightGray;
            this.MainImageControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MainImageControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainImageControl.Location = new System.Drawing.Point(0, 0);
            this.MainImageControl.Name = "MainImageControl";
            this.MainImageControl.Size = new System.Drawing.Size(690, 450);
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
            this.ROIImage.Size = new System.Drawing.Size(646, 450);
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
            this.panel3.Size = new System.Drawing.Size(5, 377);
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
            this.TabControl.Size = new System.Drawing.Size(848, 377);
            this.TabControl.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(840, 351);
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
            this.groupBox1.Size = new System.Drawing.Size(834, 345);
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
            this.splitContainer2.Panel1.Controls.Add(this.Histogram);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.Histogramcheck);
            this.splitContainer2.Panel2.Controls.Add(this.CameraList);
            this.splitContainer2.Panel2.Controls.Add(this.label22);
            this.splitContainer2.Panel2.Controls.Add(this.SetCameraId);
            this.splitContainer2.Panel2.Controls.Add(this.OriginDisplay);
            this.splitContainer2.Panel2.Controls.Add(this.AreaDisplay);
            this.splitContainer2.Panel2.Controls.Add(this.BtnDefineOrigin);
            this.splitContainer2.Panel2.Controls.Add(this.BtnSetROI);
            this.splitContainer2.Panel2.Controls.Add(this.OriginXText);
            this.splitContainer2.Panel2.Controls.Add(this.AreaTextX);
            this.splitContainer2.Panel2.Controls.Add(this.label21);
            this.splitContainer2.Panel2.Controls.Add(this.label4);
            this.splitContainer2.Panel2.Controls.Add(this.label20);
            this.splitContainer2.Panel2.Controls.Add(this.label3);
            this.splitContainer2.Panel2.Controls.Add(this.OriginWidthText);
            this.splitContainer2.Panel2.Controls.Add(this.AreaTextWidth);
            this.splitContainer2.Panel2.Controls.Add(this.OriginHeightText);
            this.splitContainer2.Panel2.Controls.Add(this.label19);
            this.splitContainer2.Panel2.Controls.Add(this.AreaTextHeight);
            this.splitContainer2.Panel2.Controls.Add(this.OriginYText);
            this.splitContainer2.Panel2.Controls.Add(this.label1);
            this.splitContainer2.Panel2.Controls.Add(this.label15);
            this.splitContainer2.Panel2.Controls.Add(this.AreaTextY);
            this.splitContainer2.Panel2.Controls.Add(this.label2);
            this.splitContainer2.Size = new System.Drawing.Size(828, 326);
            this.splitContainer2.SplitterDistance = 374;
            this.splitContainer2.TabIndex = 0;
            // 
            // Histogram
            // 
            chartArea1.Name = "ChartArea1";
            this.Histogram.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.Histogram.Legends.Add(legend1);
            this.Histogram.Location = new System.Drawing.Point(13, 13);
            this.Histogram.Name = "Histogram";
            this.Histogram.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Excel;
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Histogram";
            this.Histogram.Series.Add(series1);
            this.Histogram.Size = new System.Drawing.Size(358, 300);
            this.Histogram.TabIndex = 0;
            this.Histogram.Text = "chart1";
            this.Histogram.Click += new System.EventHandler(this.Histogram_Click);
            // 
            // Histogramcheck
            // 
            this.Histogramcheck.AutoSize = true;
            this.Histogramcheck.Location = new System.Drawing.Point(19, 267);
            this.Histogramcheck.Name = "Histogramcheck";
            this.Histogramcheck.Size = new System.Drawing.Size(73, 17);
            this.Histogramcheck.TabIndex = 7;
            this.Histogramcheck.Text = "Histogram";
            this.Histogramcheck.UseVisualStyleBackColor = true;
            this.Histogramcheck.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // CameraList
            // 
            this.CameraList.FormattingEnabled = true;
            this.CameraList.Location = new System.Drawing.Point(105, 207);
            this.CameraList.Name = "CameraList";
            this.CameraList.Size = new System.Drawing.Size(217, 21);
            this.CameraList.TabIndex = 6;
            this.CameraList.SelectedIndexChanged += new System.EventHandler(this.CameraList_SelectedIndexChanged);
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(45, 210);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(54, 13);
            this.label22.TabIndex = 5;
            this.label22.Text = "CameraID";
            this.label22.Click += new System.EventHandler(this.label22_Click);
            // 
            // SetCameraId
            // 
            this.SetCameraId.Location = new System.Drawing.Point(328, 207);
            this.SetCameraId.Name = "SetCameraId";
            this.SetCameraId.Size = new System.Drawing.Size(75, 23);
            this.SetCameraId.TabIndex = 4;
            this.SetCameraId.Text = "Set";
            this.SetCameraId.UseVisualStyleBackColor = true;
            this.SetCameraId.Click += new System.EventHandler(this.SetCameraId_Click);
            // 
            // OriginDisplay
            // 
            this.OriginDisplay.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.OriginDisplay.AutoSize = true;
            this.OriginDisplay.Location = new System.Drawing.Point(327, 130);
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
            this.AreaDisplay.Location = new System.Drawing.Point(122, 130);
            this.AreaDisplay.Name = "AreaDisplay";
            this.AreaDisplay.Size = new System.Drawing.Size(71, 17);
            this.AreaDisplay.TabIndex = 3;
            this.AreaDisplay.Text = "Is Display";
            this.AreaDisplay.UseVisualStyleBackColor = true;
            // 
            // BtnDefineOrigin
            // 
            this.BtnDefineOrigin.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.BtnDefineOrigin.Location = new System.Drawing.Point(315, 156);
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
            this.BtnSetROI.Location = new System.Drawing.Point(105, 156);
            this.BtnSetROI.Name = "BtnSetROI";
            this.BtnSetROI.Size = new System.Drawing.Size(100, 23);
            this.BtnSetROI.TabIndex = 2;
            this.BtnSetROI.Text = "Set";
            this.BtnSetROI.UseVisualStyleBackColor = true;
            this.BtnSetROI.Click += new System.EventHandler(this.BtnSetROI_Click);
            // 
            // OriginXText
            // 
            this.OriginXText.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.OriginXText.Location = new System.Drawing.Point(315, 26);
            this.OriginXText.Name = "OriginXText";
            this.OriginXText.Size = new System.Drawing.Size(100, 20);
            this.OriginXText.TabIndex = 1;
            // 
            // AreaTextX
            // 
            this.AreaTextX.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.AreaTextX.Location = new System.Drawing.Point(105, 26);
            this.AreaTextX.Name = "AreaTextX";
            this.AreaTextX.Size = new System.Drawing.Size(100, 20);
            this.AreaTextX.TabIndex = 1;
            // 
            // label21
            // 
            this.label21.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(236, 107);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(74, 13);
            this.label21.TabIndex = 0;
            this.label21.Text = "Origin Height :";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(33, 107);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Area Height :";
            // 
            // label20
            // 
            this.label20.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(237, 81);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(71, 13);
            this.label20.TabIndex = 0;
            this.label20.Text = "Origin Width :";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(33, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Area Width :";
            // 
            // OriginWidthText
            // 
            this.OriginWidthText.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.OriginWidthText.Location = new System.Drawing.Point(315, 78);
            this.OriginWidthText.Name = "OriginWidthText";
            this.OriginWidthText.Size = new System.Drawing.Size(100, 20);
            this.OriginWidthText.TabIndex = 1;
            // 
            // AreaTextWidth
            // 
            this.AreaTextWidth.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.AreaTextWidth.Location = new System.Drawing.Point(105, 78);
            this.AreaTextWidth.Name = "AreaTextWidth";
            this.AreaTextWidth.Size = new System.Drawing.Size(100, 20);
            this.AreaTextWidth.TabIndex = 1;
            // 
            // OriginHeightText
            // 
            this.OriginHeightText.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.OriginHeightText.Location = new System.Drawing.Point(315, 104);
            this.OriginHeightText.Name = "OriginHeightText";
            this.OriginHeightText.Size = new System.Drawing.Size(100, 20);
            this.OriginHeightText.TabIndex = 1;
            // 
            // label19
            // 
            this.label19.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(260, 29);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(50, 13);
            this.label19.TabIndex = 0;
            this.label19.Text = "Origin X :";
            // 
            // AreaTextHeight
            // 
            this.AreaTextHeight.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.AreaTextHeight.Location = new System.Drawing.Point(105, 104);
            this.AreaTextHeight.Name = "AreaTextHeight";
            this.AreaTextHeight.Size = new System.Drawing.Size(100, 20);
            this.AreaTextHeight.TabIndex = 1;
            // 
            // OriginYText
            // 
            this.OriginYText.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.OriginYText.Location = new System.Drawing.Point(315, 52);
            this.OriginYText.Name = "OriginYText";
            this.OriginYText.Size = new System.Drawing.Size(100, 20);
            this.OriginYText.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(57, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Area X :";
            // 
            // label15
            // 
            this.label15.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(260, 55);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(50, 13);
            this.label15.TabIndex = 0;
            this.label15.Text = "Origin Y :";
            // 
            // AreaTextY
            // 
            this.AreaTextY.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.AreaTextY.Location = new System.Drawing.Point(105, 52);
            this.AreaTextY.Name = "AreaTextY";
            this.AreaTextY.Size = new System.Drawing.Size(100, 20);
            this.AreaTextY.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(57, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Area Y :";
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(840, 351);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Process Image";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.Savebutton);
            this.tabPage3.Controls.Add(this.SavePath);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(840, 351);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
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
            this.groupBox3.Controls.Add(this.ShowFocusPoint);
            this.groupBox3.Controls.Add(this.checkBoxAverage);
            this.groupBox3.Controls.Add(this.ResetZoom);
            this.groupBox3.Controls.Add(this.checkBoxCenter);
            this.groupBox3.Controls.Add(this.ExpouseTimeText);
            this.groupBox3.Controls.Add(this.label23);
            this.groupBox3.Controls.Add(this.groupBox5);
            this.groupBox3.Controls.Add(this.IsAutoExposureTime);
            this.groupBox3.Controls.Add(this.groupBox4);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Right;
            this.groupBox3.Location = new System.Drawing.Point(848, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(492, 377);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Camera State";
            // 
            // ShowFocusPoint
            // 
            this.ShowFocusPoint.AutoSize = true;
            this.ShowFocusPoint.Location = new System.Drawing.Point(301, 349);
            this.ShowFocusPoint.Name = "ShowFocusPoint";
            this.ShowFocusPoint.Size = new System.Drawing.Size(108, 17);
            this.ShowFocusPoint.TabIndex = 11;
            this.ShowFocusPoint.Text = "Show focus point";
            this.ShowFocusPoint.UseVisualStyleBackColor = true;
            // 
            // checkBoxAverage
            // 
            this.checkBoxAverage.AutoSize = true;
            this.checkBoxAverage.Location = new System.Drawing.Point(116, 349);
            this.checkBoxAverage.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxAverage.Name = "checkBoxAverage";
            this.checkBoxAverage.Size = new System.Drawing.Size(66, 17);
            this.checkBoxAverage.TabIndex = 9;
            this.checkBoxAverage.Text = "Average";
            this.checkBoxAverage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBoxAverage.UseVisualStyleBackColor = true;
            this.checkBoxAverage.CheckedChanged += new System.EventHandler(this.checkBoxAverage_CheckedChanged);
            // 
            // ResetZoom
            // 
            this.ResetZoom.Location = new System.Drawing.Point(267, 282);
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
            this.checkBoxCenter.Location = new System.Drawing.Point(188, 349);
            this.checkBoxCenter.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxCenter.Name = "checkBoxCenter";
            this.checkBoxCenter.Size = new System.Drawing.Size(57, 17);
            this.checkBoxCenter.TabIndex = 8;
            this.checkBoxCenter.Text = "Center";
            this.checkBoxCenter.UseVisualStyleBackColor = true;
            this.checkBoxCenter.CheckedChanged += new System.EventHandler(this.checkBoxCenter_CheckedChanged);
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
            this.ExpouseTimeText.Location = new System.Drawing.Point(116, 283);
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
            this.label23.Location = new System.Drawing.Point(53, 327);
            this.label23.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(83, 13);
            this.label23.TabIndex = 7;
            this.label23.Text = "Exposure Focus";
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
            // IsAutoExposureTime
            // 
            this.IsAutoExposureTime.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.IsAutoExposureTime.AutoSize = true;
            this.IsAutoExposureTime.Location = new System.Drawing.Point(116, 308);
            this.IsAutoExposureTime.Name = "IsAutoExposureTime";
            this.IsAutoExposureTime.Size = new System.Drawing.Size(116, 17);
            this.IsAutoExposureTime.TabIndex = 2;
            this.IsAutoExposureTime.Text = "Auto exposure time";
            this.IsAutoExposureTime.UseVisualStyleBackColor = true;
            this.IsAutoExposureTime.CheckedChanged += new System.EventHandler(this.IsAutoExposureTime_CheckedChanged);
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
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(53, 285);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Exp Time :";
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(222, 286);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(20, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "ms";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.XYPosText);
            this.panel2.Controls.Add(this.MessageStatusText);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(5, 836);
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
            this.ClientSize = new System.Drawing.Size(1350, 861);
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
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Histogram)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
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
        private System.Windows.Forms.DataVisualization.Charting.Chart Histogram;
        private System.Windows.Forms.CheckBox Histogramcheck;
        private System.Windows.Forms.CheckBox ShowFocusPoint;
    }
}

