using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Drawing;
using System.Threading;
using System.Drawing.Imaging;

using AForge;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;
using System.Threading.Tasks;
using DirectShowLib;
using System.Diagnostics;
using System.Runtime.InteropServices;
//using OpenCV64Methods;


namespace CameraViewer {
    public class Camera {
        public VideoCaptureDevice VideoSource = null;
        private MainForm MainForm;
        private CameraAdjustments cameraAdjustments;
        //OpenCVMethods openCVMethods = new OpenCVMethods();

        public bool isFullScreenMode = false;
        public Size fullscreenSize = new Size(1920, 1080);

        public bool rotateCam = false;
        public int rotateAmount = 1;

        //public ShapeDetectionVariables ShapeVariables = new ShapeDetectionVariables();
        //public OpticalFlowVariable optiVariables = new OpticalFlowVariable();

        public Eyetracking eyeTracker;

        public bool isEyeCamera { get; set; } = false;

        public Camera(MainForm MainF, CameraAdjustments cameraAdj) {
            MainForm = MainF;
            cameraAdjustments = cameraAdj;
        }

        public Camera(MainForm MainF, CameraAdjustments cameraAdj, Eyetracking eyeTracker) {
            MainForm = MainF;
            cameraAdjustments = cameraAdj;
            this.eyeTracker = eyeTracker;
        }

        public bool Active { get; set; }

        public void SignalToStop()      // Asks nicely
        {
            VideoSource.SignalToStop();
        }

        public void Close() {
            if (!(VideoSource == null)) {
                if (!VideoSource.IsRunning) {
                    return;
                }
                VideoSource.SignalToStop();
                VideoSource.WaitForStop();  // problem?
                VideoSource.NewFrame -= new NewFrameEventHandler(Video_NewFrame);
                VideoSource = null;

                MonikerString = "Stopped";
            }
        }

        public void DisplayPropertyPage() {
            VideoSource.DisplayPropertyPage(IntPtr.Zero);
        }

        public class ProtectedPictureBox : System.Windows.Forms.PictureBox {
            protected override void OnPaint(PaintEventArgs e) {
                try {
                    lock (_locker) {
                        try {
                            base.OnPaint(e);
                        }
                        catch { }
                    }
                }
                catch { }
            }
        }

        private static object _locker = new object();

        ProtectedPictureBox _imageBox;
        public ProtectedPictureBox ImageBox {
            get {
                return (_imageBox);
            }
            set {
                lock (_locker) {
                    _imageBox = value;
                }
            }
        }

        public int ImgCenterX { get; set; }
        public int ImgCenterY { get; set; }
        public int ImgSizeX { get; set; }
        public int ImgSizeY { get; set; }

        public string MonikerString = "unconnected";

        public bool ReceivingFrames { get; set; }

        public IAMVideoProcAmp cameraControls;
        public bool Start(string cam, string MonikerStr) {
            try {
                // enumerate video devices
                FilterInfoCollection videoDevices = new FilterInfoCollection(AForge.Video.DirectShow.FilterCategory.VideoInputDevice);
                // create the video source (check that the camera exists is already done
                VideoSource = new VideoCaptureDevice(MonikerStr);

                VideoSource.VideoResolution = VideoSource.VideoCapabilities[17];
                VideoSource.NewFrame += new NewFrameEventHandler(Video_NewFrame);
                ReceivingFrames = false;

                // try ten times to start
                int tries = 0;

                while (tries < 80)  // 4s maximum to a camera to start
                {
                    // VideoSource.Start() checks running status, is safe to call multiple times
                    tries++;
                    VideoSource.Start();
                    if (!ReceivingFrames) {
                        // 50 ms pause, processing events so that videosource has a chance
                        for (int i = 0; i < 5; i++) {
                            Thread.Sleep(10);
                            Application.DoEvents();
                        }
                    }
                    else {
                        break;
                    }
                }

                cameraControls = VideoSource.SourceObject as IAMVideoProcAmp;

                if (!ReceivingFrames) {
                    return false;
                }

                VideoCapabilities Capability = VideoSource.VideoCapabilities[17];

                ImgSizeX = Capability.FrameSize.Width;
                ImgSizeY = Capability.FrameSize.Height;
                ImgCenterX = ImgSizeX / 2;
                ImgCenterY = ImgSizeY / 2;
                lock (_locker) { }  // wait
                //PauseProcessing = false;
                return true;
            }
            catch {
                return false;
            }
        }

        public List<string> GetDeviceList() {
            List<string> Devices = new List<string>();

            FilterInfoCollection videoDevices = new FilterInfoCollection(AForge.Video.DirectShow.FilterCategory.VideoInputDevice);
            foreach (AForge.Video.DirectShow.FilterInfo device in videoDevices) {
                Devices.Add(device.Name);
            }
            return (Devices);
        }

        public List<string> GetMonikerStrings() {
            List<string> Monikers = new List<string>();

            FilterInfoCollection videoDevices = new FilterInfoCollection(AForge.Video.DirectShow.FilterCategory.VideoInputDevice);
            foreach (AForge.Video.DirectShow.FilterInfo device in videoDevices) {
                Monikers.Add(device.MonikerString);
            }
            return (Monikers);
        }


        private bool GetFrame = false;     // Tells we need to get a frame from the stream 
        private Bitmap TempFrame;      // for temp processing

        public bool Mirror { get; set; }

        public bool GrayScale { get; set; }
        public bool R { get; set; }
        public bool G { get; set; }
        public bool B { get; set; }

        public bool Invert { get; set; }
        public bool EdgeDetect { get; set; }
        public int EdgeDetectValue { get; set; }

        public bool NoiseReduce { get; set; }
        public int NoiseReduceValue { get; set; }

        public bool Threshold { get; set; }
        public int ThresholdValue { get; set; }

        public bool Zoom { get; set; }
        public int ZoomValue { get; set; }

        public bool Contrast { get; set; }
        public int ContrastValue { get; set; }

        public bool DrawGrid { get; set; }   // Draws grid for size measurement
        public int GridIncrement { get; set; } = 20;

        public readonly bool PauseProcessing = false;
        private bool paused = true;



        private System.Drawing.Point[] ToPointsArray(List<IntPoint> points) {
            System.Drawing.Point[] array = new System.Drawing.Point[points.Count];

            for (int i = 0, n = points.Count; i < n; i++) {
                array[i] = new System.Drawing.Point(points[i].X, points[i].Y);
            }

            return array;
        }

        Bitmap frame;
        DateTime timeStart = DateTime.Now;
        public int currentFramesSecond { get; private set; } = 0;
        public int targetFramesSecond { get; set; } = 25;

        public bool processing = false;
        int processingCount = 0;
        private void Video_NewFrame(object sender, NewFrameEventArgs eventArgs) {
            ReceivingFrames = true;
            if (processing && processingCount < 100) {   // dump it
                processingCount++;
                Thread.Sleep(2);
                return;
            }
            if ((DateTime.Now - timeStart).Milliseconds < targetFramesSecond) {
                return;
            }

            processing = true;
            processingCount = 0;

            lock (_locker) {
                frame = (Bitmap)eventArgs.Frame.Clone();
            }

            if (GetFrame) {
                if (TempFrame != null) {
                    TempFrame.Dispose();
                }
                TempFrame = (Bitmap)frame.Clone();
                GetFrame = false;
            }

            if (PauseProcessing) {
                frame.Dispose();
                paused = true;
                return;
            };

            if (!Active) {
                frame.Dispose();
                return;
            }

            if (GrayScale)
                GrayscaleImg(ref frame, R, G, B);
            if (Invert)
                InvertImg(ref frame);
            if (EdgeDetect)
                EdgeDetectImg(ref frame);
            if (NoiseReduce)
                NoiseReduction(ref frame);
            if (Threshold)
                ThresholdImg(ref frame);
            if (Contrast)
                ContrastNormalize(ref frame);

            if (Mirror) {
                frame = MirrorImg(frame);
            }

            if (DrawGrid) {
                DrawGridFunc(frame);
            }

            if (Zoom)
                ZoomImg(ref frame, ZoomValue);

            if (this.rotateCam) {
                switch (rotateAmount) {
                    case 0:
                        frame.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;
                    case 1:
                        frame.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        break;
                    case 2:
                        frame.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;
                    default:
                        frame.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        break;
                }
            }

            if (eyeTracker.isActive) {
                frame = HandleEyeTracking(frame);
            }

            // send to openCV for additional stuff
            //if (!(!ShapeVariables.calcCircles && !ShapeVariables.calcLines && !ShapeVariables.calcRectTri))
            //    frame = OpenCVMethods.PerformShapeDetection(frame, ShapeVariables);

            //if (optiVariables.calcOpticalFlow) {
            //    frame = openCVMethods.Dense_Optical_Flow(frame, optiVariables);
            //}

            // handle fullscreen mode
            //if (isFullScreenMode) {
            //    try {
            //        frame = OpenCVMethods.ResizeImage(frame, fullscreenSize);
            //    }
            //    catch {
            //        frame = ResizeImageNoCuda(frame, fullscreenSize);
            //    }
            //}

            // getting accessViolation Exceptions
            Bitmap frame2;
            lock (_locker) {
                frame2 = (Bitmap)frame.Clone();
            }

            processing = false;

            currentFramesSecond = 1000 / ((DateTime.Now - timeStart).Milliseconds + 1);
            cameraAdjustments.label1.BeginInvoke((MethodInvoker)delegate () { cameraAdjustments.label1.Text = $"Camera {(DateTime.Now - timeStart).Milliseconds}ms {currentFramesSecond}fps"; });


            lock (_locker) {
                if (ImageBox.Image != null) {
                    ImageBox.Image.Dispose();
                }
                ImageBox.Image = (Bitmap)frame2.Clone();
            }


            frame.Dispose();
            frame2.Dispose();
            processing = false;
            Thread.Sleep(1);
            timeStart = DateTime.Now;
        }

        public double xEyeDetection;
        public double yEyeDetection;
        private Bitmap HandleEyeTracking(Bitmap frame) {
            // get the current tracking point, notes: this could probably be enhanced with sparse optical flow point tracking
            // if the environment is favorable however, this technique does work extremely well
            if (isEyeCamera) {
                Tuple<double, double> retVal = FindBlobLocation(frame);
                xEyeDetection = retVal.Item1;
                yEyeDetection = retVal.Item2;

                Pen GPen = new Pen(Color.LawnGreen, 1);
                Graphics g = Graphics.FromImage(frame);
                // now draw the current locations on the frame
                int size1 = eyeTracker.eyeTrackingMatrix.GetLength(1);
                int size0 = eyeTracker.eyeTrackingMatrix.GetLength(0);
                for (int i = 0; i < size0; i++) {
                    for (int j = 0; j < size1; j++)
                        g.DrawArc(GPen, new Rectangle(eyeTracker.eyeTrackingMatrix[i, j], new Size(2, 2)), 0, 360);
                }

                g.DrawRectangle(GPen, eyeTracker.rectX, eyeTracker.rectY, eyeTracker.rectHeight, eyeTracker.rectWidth);

                g.Dispose();

                return frame;
            }

            if (eyeTracker.recordingPoints) {
                Pen BlackPen = new Pen(Color.AliceBlue, 2);
                Graphics g = Graphics.FromImage(frame);
                System.Drawing.Point calibrationTarget = ScalePoints(eyeTracker.xCal, eyeTracker.yCal, Eyetracking.maxSize, Eyetracking.maxSize);
                calibrationTarget.X += frame.Height/18;
                calibrationTarget.Y += frame.Height / 18;
                g.DrawArc(BlackPen, new Rectangle(calibrationTarget, new Size(12, 12)), 0, 360);
                calibrationTarget.X += 2;
                calibrationTarget.Y += 2;
                BlackPen = new Pen(Color.DarkOrange, 2);
                g.DrawArc(BlackPen, new Rectangle(calibrationTarget, new Size(9, 9)), 0, 360);
                g.Dispose();
            }
            else {  // draw the location!!
                Pen BlackPen = new Pen(Color.AliceBlue, 4);
                Graphics g = Graphics.FromImage(frame);
                System.Drawing.Point rectPoint = ScalePoints(eyeTracker.TrackedXVal, eyeTracker.TrackedYVal, Eyetracking.maxSize, Eyetracking.maxSize);
                rectPoint.X += frame.Height / 18;
                rectPoint.Y += frame.Height / 20;
                rectPoint.Y += eyeTracker.yTune;
                rectPoint.X += eyeTracker.xTune;
                g.DrawArc(BlackPen, new Rectangle(rectPoint, new Size(40, 40)), 0, 360);
                rectPoint.X += 2;
                rectPoint.Y += 2;
                BlackPen = new Pen(Color.DarkOrange, 3);
                g.DrawArc(BlackPen, new Rectangle(rectPoint, new Size(37, 37)), 0, 360);
                g.Dispose();
            }

            return frame;
        }

        private void NoiseReduction(ref Bitmap frame) {
            frame = Grayscale.CommonAlgorithms.RMY.Apply(frame);    // Make gray
            switch (NoiseReduceValue) {
                case 1:
                    BilateralSmoothing Bil_filter = new BilateralSmoothing();
                    Bil_filter.KernelSize = 7;
                    Bil_filter.SpatialFactor = 10;
                    Bil_filter.ColorFactor = 30;
                    Bil_filter.ColorPower = 0.5;
                    Bil_filter.ApplyInPlace(frame);
                    break;

                case 2:
                    Median M_filter = new Median();
                    M_filter.ApplyInPlace(frame);
                    break;

                case 3:
                    Mean Meanfilter = new Mean();
                    // apply the MirrFilter
                    Meanfilter.ApplyInPlace(frame);
                    break;

                default:
                    Median Median_filter = new Median();
                    Median_filter.ApplyInPlace(frame);
                    break;
            }
            GrayscaleToRGB RGBfilter = new GrayscaleToRGB();    // back to color format
            frame = RGBfilter.Apply(frame);
        }

        public static Bitmap ResizeImageNoCuda(Bitmap imgToResize, Size size) {
            try {
                Bitmap b = new Bitmap(size.Width, size.Height);
                using (Graphics g = Graphics.FromImage((System.Drawing.Image)b)) {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
                    g.DrawImage(imgToResize, 0, 0, size.Width, size.Height);
                }
                return b;
            }
            catch {
                return imgToResize;
            }
        }

        

        private void ZoomImg(ref Bitmap frame, double Factor) {
            if (rotateCam && (rotateAmount == 0 || rotateAmount == 2)) {
                int x = (int)(1280 * 1.7);
                int y = (int)(720 * 1.7);
                //try {
                //    frame = OpenCVMethods.ResizeImage(frame, new Size(x, y));
                //}
                //catch {
                //    frame = ResizeImageNoCuda(frame, new Size(x, y));
                //}
            }

            if (Factor < 0.1) {
                return;
            }
            int centerX = frame.Width / 2;
            int centerY = frame.Height / 2;
            int OrgSizeX = frame.Width;
            int OrgSizeY = frame.Height;

            int fromX = centerX - (int)(centerX / Factor);
            int fromY = centerY - (int)(centerY / Factor);
            int SizeX = (int)(OrgSizeX / Factor);
            int SizeY = (int)(OrgSizeY / Factor);
            Crop CrFilter = new Crop(new Rectangle(fromX, fromY, SizeX, SizeY));
            frame = CrFilter.Apply(frame);
            //try {
            //    frame = OpenCVMethods.ResizeImage(frame, new Size(OrgSizeX, OrgSizeY));
            //}
            //catch {
            //    frame = ResizeImageNoCuda(frame, new Size(OrgSizeX, OrgSizeY));
            //}
        }

        private void EdgeDetectImg(ref Bitmap frame) {
            frame = Grayscale.CommonAlgorithms.RMY.Apply(frame);    // Make gray
            switch (EdgeDetectValue) {
                case 1:
                    SobelEdgeDetector SobelFilter = new SobelEdgeDetector();
                    SobelFilter.ApplyInPlace(frame);
                    break;

                case 2:
                    DifferenceEdgeDetector DifferenceFilter = new DifferenceEdgeDetector();
                    DifferenceFilter.ApplyInPlace(frame);
                    break;

                case 3:
                    HomogenityEdgeDetector HomogenityFilter = new HomogenityEdgeDetector();
                    HomogenityFilter.ApplyInPlace(frame);
                    break;

                case 4:
                    CannyEdgeDetector Cannyfilter = new CannyEdgeDetector();
                    // apply the MirrFilter
                    Cannyfilter.ApplyInPlace(frame);
                    break;

                default:
                    HomogenityEdgeDetector filter = new HomogenityEdgeDetector();
                    filter.ApplyInPlace(frame);
                    break;
            }
            GrayscaleToRGB RGBfilter = new GrayscaleToRGB();    // back to color format
            frame = RGBfilter.Apply(frame);
        }

        private void InvertImg(ref Bitmap frame) {
            Invert filter = new Invert();
            filter.ApplyInPlace(frame);
        }

        private void ThresholdImg(ref Bitmap frame) {
            frame = Grayscale.CommonAlgorithms.RMY.Apply(frame);
            Threshold filter = new Threshold(ThresholdValue);
            filter.ApplyInPlace(frame);
            GrayscaleToRGB toColFilter = new GrayscaleToRGB();
            frame = toColFilter.Apply(frame);
        }

        public Tuple<double, double> FindBlobLocation(Bitmap bmp) {
            Tuple<double, double> toRet = new Tuple<double, double>(0, 0);

            // Set up the detector with default parameters.
            BlobCounter blobCounter = new BlobCounter();

            blobCounter.FilterBlobs = true;
            blobCounter.MinHeight = 5;
            blobCounter.MinWidth = 5;
            blobCounter.MaxHeight = 20;
            blobCounter.MaxHeight = 20;            

            blobCounter.ProcessImage(bmp);
            Rectangle[] rects = blobCounter.GetObjectsRectangles();

            List<Rectangle> filteredRects = rects.Where(a => a.Location.X > eyeTracker.rectX && a.Location.X < (eyeTracker.rectX + eyeTracker.rectWidth)
                && a.Location.Y > eyeTracker.rectY && a.Location.Y < eyeTracker.rectY + eyeTracker.rectHeight).ToList();

            if (filteredRects.Any()) {
                toRet = new Tuple<double, double>(filteredRects.First().Location.X, filteredRects.First().Location.Y);
            }

            return toRet;
        }

        private void ContrastNormalize(ref Bitmap frame) {
            ContrastStretch filter = new ContrastStretch();
            filter.ApplyInPlace(frame);
        }

        private void GrayscaleImg(ref Bitmap frame, bool R, bool G, bool B) {
            // create filter
            Bitmap fr;
            if (!(!R && !G && !B) && (!R || !G || !B)) {
                EuclideanColorFiltering filter = new EuclideanColorFiltering();
                // set center colol and radius
                filter.CenterColor = new AForge.Imaging.RGB(Color.FromArgb(30, 200, 30));
                filter.Radius = 150;
                // apply the filter
                filter.ApplyInPlace(frame);
                Grayscale toGrFilter = new Grayscale(R ? 0.3 : 0, G ? .9 : 0, B ? 0.3 : 0);
                fr = toGrFilter.Apply(frame);
                GrayscaleToRGB toColFilter = new GrayscaleToRGB();
                frame = toColFilter.Apply(fr);
            }
            else {
                frame = Grayscale.CommonAlgorithms.RMY.Apply(frame);
                GrayscaleToRGB toColFilter = new GrayscaleToRGB();
                frame = toColFilter.Apply(frame);
            }
        }

        private Bitmap MirrorImg(Bitmap frame) {
            Mirror Mfilter = new Mirror(false, true);
            Mfilter.ApplyInPlace(frame);
            return (frame);
        }

        private System.Drawing.Point ScalePoints(int x, int y, int maxX, int maxY) {
            int px = (int)((double)x / maxX * ImgSizeX);
            int py = (int)((double)y / maxY * ImgSizeY);
            return new System.Drawing.Point(px, py);
        }

        private System.Drawing.Point ScalePoints(double x, double y, int maxX, int maxY) {
            int px = (int)(x / maxX * ImgSizeX);
            int py = (int)(y / maxY * ImgSizeY);
            return new System.Drawing.Point(px, py);
        }

        private void DrawGridFunc(Bitmap img) {
            Pen BlackPen = new Pen(Color.Green, 1);
            Graphics g = Graphics.FromImage(img);
            int x1, x2, y1, y2;
            int step = this.GridIncrement;
            // vertical
            int i = 0;
            while (i < ImgSizeX) {
                g.DrawLine(BlackPen, i, 0, i, ImgSizeY);
                i = i + step;
            }
            // horizontal
            i = 0;
            while (i < ImgSizeY) {
                g.DrawLine(BlackPen, 0, i, ImgSizeX, i);
                i = i + step;
            }

            BlackPen.Dispose();
            g.Dispose();
        }
    }
}
