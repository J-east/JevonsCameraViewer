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

namespace CameraViewer {
    public class Camera {
        private VideoCaptureDevice VideoSource = null;
        private MainForm MainForm;

        public bool isFullScreenMode = false;
        public Size fullscreenSize = new Size(1920, 1080);

        public bool rotateCam = false;
        public int rotateAmount = 1;

        public Camera(MainForm MainF) {
            MainForm = MainF;
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
                lock (_locker) {
                    base.OnPaint(e);
                }
            }
        }

        // _locker must be a static Camera class variable to be available to the overridden OnPaint method
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

        public int FrameCenterX { get; set; }
        public int FrameCenterY { get; set; }
        public int FrameSizeX { get; set; }
        public int FrameSizeY { get; set; }

        public string MonikerString = "unconnected";

        public bool ReceivingFrames { get; set; }

        public bool Start(string cam, string MonikerStr) {
            try {
                // enumerate video devices
                FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
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

                if (!ReceivingFrames) {
                    return false;
                }

                VideoCapabilities Capability = VideoSource.VideoCapabilities[17];

                FrameSizeX = Capability.FrameSize.Width;
                FrameSizeY = Capability.FrameSize.Height;
                FrameCenterX = FrameSizeX / 2;
                FrameCenterY = FrameSizeY / 2;
                lock (_locker) {

                }
                //PauseProcessing = false;
                return true;
            }
            catch {
                return false;
            }
        }

        public List<string> GetDeviceList() {
            List<string> Devices = new List<string>();

            FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo device in videoDevices) {
                Devices.Add(device.Name);
            }
            return (Devices);
        }

        public List<string> GetMonikerStrings() {
            List<string> Monikers = new List<string>();

            FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo device in videoDevices) {
                Monikers.Add(device.MonikerString);
            }
            return (Monikers);
        }


        List<AForgeFunction> MeasurementFunctions = new List<AForgeFunction>();
        // The list of functions processing the image shown to user:
        List<AForgeFunction> DisplayFunctions = new List<AForgeFunction>();


        enum DataGridViewColumns { Function, Active, Int, Double, R, G, B };

        public delegate void AForgeFilter(ref Bitmap frame);
        class AForgeFunction {
            public AForgeFilter func { get; set; }
        }

        private List<AForgeFunction> BuildFilterList() {
            List<AForgeFunction> FunctionList = new List<AForgeFunction>();

            FunctionList.Clear();

            if (Contrast)
                FunctionList.Add(new AForgeFunction() { func = Contrast_scretchFunc });
            if (GrayScale)
                FunctionList.Add(new AForgeFunction() { func = GrayscaleFunc });
            if (Invert)
                FunctionList.Add(new AForgeFunction() { func = InvertFunc });
            if (EdgeDetect)
                FunctionList.Add(new AForgeFunction() { func = Edge_detectFunc });
            if (NoiseReduce)
                FunctionList.Add(new AForgeFunction() { func = NoiseReduction_Funct });
            if (Threshold)
                FunctionList.Add(new AForgeFunction() { func = ThresholdFunc });


            return FunctionList;
        }

        public void BuildOutFunctionsList() {
            bool pause = PauseProcessing;
            if (VideoSource != null) {
                if (ReceivingFrames) {

                    paused = true;
                    while (!paused) {
                        Thread.Sleep(10);
                    };
                }
            }

            DisplayFunctions.Clear();
            DisplayFunctions = BuildFilterList();
        }

        public void ClearDisplayFunctionsList() {
            // Stop video
            bool pause = PauseProcessing;
            if (VideoSource != null) {
                if (ReceivingFrames) {
                    // stop video
                    //PauseProcessing = true;  // ask for stop
                    paused = false;
                    int maxTimes = 10;
                    while (!paused && maxTimes > 0) {
                        maxTimes--;
                        Thread.Sleep(10);  // wait until really stopped
                    };
                }
            }
            DisplayFunctions.Clear();
        }


        private bool GetFrame = false;     // Tells we need to get a frame from the stream 
        private Bitmap TempFrame;      // for temp processing


        static readonly object _object = new object();

        public bool Mirror { get; set; }

        public bool GrayScale { get; set; }

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
        bool TemporaryFrameIsReading = false;
        private void Video_NewFrame(object sender, NewFrameEventArgs eventArgs) {
            ReceivingFrames = true;

            lock (_locker) {
                frame = (Bitmap)eventArgs.Frame.Clone();
            }

            if (GetFrame) {
                if (!TemporaryFrameIsReading) {
                    TemporaryFrameIsReading = true;
                    if (TempFrame != null) {
                        TempFrame.Dispose();
                    }
                    TempFrame = (Bitmap)frame.Clone();
                    GetFrame = false;
                }
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

            try {
                if (DisplayFunctions != null) {
                    foreach (AForgeFunction f in DisplayFunctions) {
                        f.func(ref frame);
                    }
                }
            }
            catch { }

            if (Mirror) {
                frame = MirrorFunc(frame);
            }

            if (DrawGrid) {
                DrawGridFunc(frame);
            }

            if (Zoom)
                ZoomFunc(ref frame, ZoomValue);

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

            // handle fullscreen mode
            if (isFullScreenMode) {
                frame = ResizeImage(frame, fullscreenSize);
            }

            lock (_locker) {
                if (ImageBox.Image != null) {
                    ImageBox.Image.Dispose();
                }
                ImageBox.Image = (Bitmap)frame.Clone();
            }

            frame.Dispose();
        }

        private void NoiseReduction_Funct(ref Bitmap frame) {
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

        // needed whenever the frame size is altered
        public static Bitmap ResizeImage(Bitmap imgToResize, Size size) {
            try {
                Bitmap b = new Bitmap(size.Width, size.Height);
                using (Graphics g = Graphics.FromImage((System.Drawing.Image)b)) {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(imgToResize, 0, 0, size.Width, size.Height);
                }
                return b;
            }
            catch {
                return imgToResize;
            }
        }

        private void ZoomFunc(ref Bitmap frame, double Factor) {
            if (rotateCam && (rotateAmount == 0 || rotateAmount == 2)) {
                int x = (int)(1280 * 1.7);
                int y = (int)(720 * 1.7);
                frame = ResizeImage(frame, new Size(x, y));
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
            ResizeBilinear RBfilter = new ResizeBilinear(OrgSizeX, OrgSizeY);
            frame = RBfilter.Apply(frame);


        }

        // todo change drop down options to explicit
        private void Edge_detectFunc(ref Bitmap frame) {
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

        private void InvertFunc(ref Bitmap frame) {
            Invert filter = new Invert();
            filter.ApplyInPlace(frame);
        }

        private void ThresholdFunc(ref Bitmap frame) {
            frame = Grayscale.CommonAlgorithms.RMY.Apply(frame);
            Threshold filter = new Threshold(ThresholdValue);
            filter.ApplyInPlace(frame);
            GrayscaleToRGB toColFilter = new GrayscaleToRGB();
            frame = toColFilter.Apply(frame);
        }

        private void GrayscaleFunc(ref Bitmap frame) {
            Grayscale toGrFilter = new Grayscale(0.2125, 0.7154, 0.0721);       // create grayscale MirrFilter (BT709)
            Bitmap fr = toGrFilter.Apply(frame);
            GrayscaleToRGB toColFilter = new GrayscaleToRGB();
            frame = toColFilter.Apply(fr);
        }

        private void Contrast_scretchFunc(ref Bitmap frame) {
            ContrastStretch filter = new ContrastStretch();
            // process image
            filter.ApplyInPlace(frame);
        }

        private Bitmap MirrorFunc(Bitmap frame) {
            Mirror Mfilter = new Mirror(false, true);
            // apply the MirrFilter
            Mfilter.ApplyInPlace(frame);
            return (frame);
        }

        private void RotateByFrameCenter(int x, int y, out int px, out int py) {
            px = (int)(Math.Cos(0) * (x - FrameCenterX) - Math.Sin(0) * (y - FrameCenterY) + FrameCenterX);
            py = (int)(Math.Sin(0) * (x - FrameCenterX) + Math.Cos(0) * (y - FrameCenterY) + FrameCenterY);
        }

        private void DrawGridFunc(Bitmap img) {
            Pen BlackPen = new Pen(Color.Green, 1);
            Graphics g = Graphics.FromImage(img);
            int x1, x2, y1, y2;
            int step = this.GridIncrement;
            // vertical
            int i = 0;
            while (i < FrameSizeX) {
                RotateByFrameCenter(FrameCenterX + i, 0, out x1, out y1);
                RotateByFrameCenter(FrameCenterX + i, FrameSizeY, out x2, out y2);
                g.DrawLine(BlackPen, x1, y1, x2, y2);
                RotateByFrameCenter(FrameCenterX - i, 0, out x1, out y1);
                RotateByFrameCenter(FrameCenterX - i, FrameSizeY, out x2, out y2);
                g.DrawLine(BlackPen, x1, y1, x2, y2);
                i = i + step;
            }
            // horizontal
            i = 0;
            while (i < FrameSizeY) {
                RotateByFrameCenter(0, FrameCenterY + i, out x1, out y1);
                RotateByFrameCenter(FrameSizeX, FrameCenterY + i, out x2, out y2);
                g.DrawLine(BlackPen, x1, y1, x2, y2);
                RotateByFrameCenter(0, FrameCenterY - i, out x1, out y1);
                RotateByFrameCenter(FrameSizeX, FrameCenterY - i, out x2, out y2);
                g.DrawLine(BlackPen, x1, y1, x2, y2);
                i = i + step;
            }

            BlackPen.Dispose();
            g.Dispose();
        }
    }
}
