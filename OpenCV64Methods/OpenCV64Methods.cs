using Emgu.CV;
using Emgu.CV.Cuda;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenCV64Methods {
    public class ShapeDetectionVariables {
        public bool calcLines { get; set; }
        public bool calcCircles { get; set; }
        public bool calcRectTri { get; set; }
        public double circleAccumulatorThreshold { get; set; } = 30;
        public int minradius { get; set; } = 5;
        public int maxRadius { get; set; } = 100;
        public double lineCannyThreshold { get; set; } = 200;
        public int lineThreshold { get; set; } = 20;
        public double cannyThresholdLinking { get; set; } = 80.0;
        public double circleCannyThreshold { get; set; } = 32;
        public double minLineWidth { get; set; } = 30;
    }

    public class OpticalFlowVariable {
        public int shiftThatCounts { get; set; } = 1;

        public bool calcOpticalFlow { get; set; }
        public int frameReduction { get; set; } = 4;
        public int stepRate { get; set; } = 12;
    }

    public class OpenCVMethods {
        public static Bitmap PerformShapeDetection(Bitmap frame, ShapeDetectionVariables detectionVars) {

            StringBuilder msgBuilder = new StringBuilder("Performance: ");

            Image<Bgr, Byte> img = new Image<Bgr, byte>(frame);
            Mat MatImg = img.Mat;

            Mat outputImg = new Mat();

            if (CudaInvoke.HasCuda) {
                using (GpuMat gMatSrc = new GpuMat())
                using (GpuMat gMatDst = new GpuMat()) {
                    gMatSrc.Upload(MatImg);
                    CudaGaussianFilter noiseReducetion = new CudaGaussianFilter(MatImg.Depth, img.NumberOfChannels, MatImg.Depth, img.NumberOfChannels, new Size(1, 1), 0);
                    noiseReducetion.Apply(gMatSrc, gMatDst);
                    gMatDst.Download(outputImg);
                }
            }
            else {
                Mat pyrDown = new Mat();
                CvInvoke.PyrDown(img, pyrDown);
                CvInvoke.PyrUp(pyrDown, img);
                outputImg = img.Mat;
            }

            UMat uimage = new UMat();
            CvInvoke.CvtColor(outputImg, uimage, ColorConversion.Bgr2Gray);

            CircleF[] circles = new CircleF[0];
            if (detectionVars.calcCircles) {
                circles = CvInvoke.HoughCircles(
                    uimage,
                    HoughType.Gradient, 1.0, 20.0,
                    detectionVars.circleCannyThreshold,
                    detectionVars.circleAccumulatorThreshold == 0 ? 1 : detectionVars.circleAccumulatorThreshold,
                    detectionVars.minradius,
                    detectionVars.maxRadius);
            }

            #region Canny and edge detection
            UMat cannyEdges = new UMat();
            CvInvoke.Canny(uimage, cannyEdges, detectionVars.lineCannyThreshold, detectionVars.cannyThresholdLinking);

            LineSegment2D[] lines = new LineSegment2D[0];
            if (detectionVars.calcLines) {
                lines = CvInvoke.HoughLinesP(
                   cannyEdges,
                   1, //Distance resolution in pixel-related units
                   Math.PI / 45.0, //Angle resolution measured in radians.
                   detectionVars.lineThreshold, //threshold
                   detectionVars.minLineWidth, //min Line width
                   10); //gap between lines
            }
            #endregion

            #region Find triangles and rectangles

            List<RotatedRect> boxList = new List<RotatedRect>(); //a box is a rotated rectangle

            if (detectionVars.calcRectTri) {
                using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint()) {
                    CvInvoke.FindContours(cannyEdges, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);
                    int count = contours.Size;
                    for (int i = 0; i < count; i++) {
                        using (VectorOfPoint contour = contours[i])
                        using (VectorOfPoint approxContour = new VectorOfPoint()) {
                            CvInvoke.ApproxPolyDP(contour, approxContour, CvInvoke.ArcLength(contour, true) * 0.05, true);
                            if (CvInvoke.ContourArea(approxContour, false) > 250) {     //only consider contours with area greater than 250
                                if (approxContour.Size == 4) {                          //The contour has 4 vertices.
                                    #region determine if all the angles in the contour are within [80, 100] degree
                                    bool isRectangle = true;
                                    Point[] pts = approxContour.ToArray();
                                    LineSegment2D[] edges = PointCollection.PolyLine(pts, true);

                                    for (int j = 0; j < edges.Length; j++) {
                                        double angle = Math.Abs(
                                           edges[(j + 1) % edges.Length].GetExteriorAngleDegree(edges[j]));
                                        if (angle < 80 || angle > 100) {
                                            isRectangle = false;
                                            break;
                                        }
                                    }
                                    #endregion

                                    if (isRectangle) boxList.Add(CvInvoke.MinAreaRect(approxContour));
                                }
                            }
                        }
                    }
                }
            }

            #endregion

            Image<Bgra, Byte> alphaImgShape = new Image<Bgra, byte>(img.Size.Width, img.Size.Height, new Bgra(0, 0, 0, .5));
            Mat alphaimg = new Mat();
            CvInvoke.CvtColor(img, alphaimg, ColorConversion.Bgr2Bgra);
            #region draw rectangles and triangles
            if (detectionVars.calcRectTri) {
                Image<Bgr, Byte> triangleRectangleImage = new Image<Bgr, Byte>(img.Size);

                foreach (RotatedRect box in boxList) {
                    CvInvoke.Polylines(triangleRectangleImage, Array.ConvertAll(box.GetVertices(), Point.Round), true, new Bgr(0, 255, 0).MCvScalar, 2);
                }

                CvInvoke.AddWeighted(alphaImgShape, .5, BlackTransparent(triangleRectangleImage), .5, 0, alphaImgShape);


                if (CudaInvoke.HasCuda) {
                    using (GpuMat gMatSrc = new GpuMat())
                    using (GpuMat gMatSrc2 = new GpuMat())
                    using (GpuMat gMatDst = new GpuMat()) {
                        gMatSrc.Upload(alphaimg);
                        gMatSrc2.Upload(alphaImgShape);
                        CudaInvoke.AlphaComp(gMatSrc, gMatSrc2, gMatDst, AlphaCompTypes.Plus);
                        gMatDst.Download(alphaimg);
                    }
                }
                else {
                    img = Overlay(img, alphaImgShape);
                }
            }
            #endregion

            #region draw circles
            if (detectionVars.calcCircles) {
                Image<Bgr, Byte> circleImage = new Image<Bgr, Byte>(img.Size);
                foreach (CircleF circle in circles.Take(10))
                    CvInvoke.Circle(circleImage, Point.Round(circle.Center), (int)circle.Radius, new Bgr(0, 255, 0).MCvScalar, 2);

                alphaImgShape = new Image<Bgra, byte>(img.Size.Width, img.Size.Height, new Bgra(0, 0, 0, .5));
                CvInvoke.AddWeighted(alphaImgShape, .7, BlackTransparent(circleImage), .5, 0, alphaImgShape);
                if (CudaInvoke.HasCuda) {
                    using (GpuMat gMatSrc = new GpuMat())
                    using (GpuMat gMatSrc2 = new GpuMat())
                    using (GpuMat gMatDst = new GpuMat()) {
                        gMatSrc.Upload(alphaimg);
                        gMatSrc2.Upload(alphaImgShape);
                        CudaInvoke.AlphaComp(gMatSrc, gMatSrc2, gMatDst, AlphaCompTypes.Plus);
                        gMatDst.Download(alphaimg);
                    }
                }
                else {
                    img = Overlay(img, alphaImgShape);
                }
            }
            #endregion

            #region draw lines

            if (detectionVars.calcLines) {
                Image<Bgr, Byte> lineImage = new Image<Bgr, Byte>(img.Size);
                foreach (LineSegment2D line in lines)
                    CvInvoke.Line(lineImage, line.P1, line.P2, new Bgr(0, 255, 0).MCvScalar, 2);

                alphaImgShape = new Image<Bgra, byte>(img.Size.Width, img.Size.Height, new Bgra(0, 0, 0, .5));
                CvInvoke.AddWeighted(alphaImgShape, .5, BlackTransparent(lineImage), .5, 0, alphaImgShape);
                if (CudaInvoke.HasCuda) {
                    using (GpuMat gMatSrc = new GpuMat())
                    using (GpuMat gMatSrc2 = new GpuMat())
                    using (GpuMat gMatDst = new GpuMat()) {
                        gMatSrc.Upload(alphaimg);
                        gMatSrc2.Upload(alphaImgShape);
                        CudaInvoke.AlphaComp(gMatSrc, gMatSrc2, gMatDst, AlphaCompTypes.Plus);
                        gMatDst.Download(alphaimg);
                    }
                }
                else {
                    img = Overlay(img, alphaImgShape);
                }
            }
            #endregion

            GC.Collect();   // first time I've had to use this but this program will use as much memory as possible, resulting in corrptions

            return alphaimg.Bitmap ?? frame;
        }


        /// <summary>
        /// note: portions of code below were taken from https://www.codeproject.com/Articles/1192205/Capturing-motion-from-video-using-the-Emgu-CV-libr
        /// author: Markus Koppensteiner
        /// any code taken from codeproject is licenced under "The Code Project Open License (CPOL) 1.02"
        /// this code is licenced under apache and may be redistributed
        /// </summary>

        // emgu code
        Mat matframe;
        Mat prev_frame;
        public double frame_nr = 0;
        double total_frames;
        public Image<Bgr, byte> img_average_vectors;
        string[] str_flow_info;
        int orig_height = 720;
        int frameReduction = 2;
        public List<string[]> lst_measures_flow = new List<string[]>();
        // calculates the optical flow according to the Farneback algorithm 
        public Bitmap Dense_Optical_Flow(Bitmap bmp, OpticalFlowVariable optiVariables) {
            frameReduction = optiVariables.frameReduction < 1 ? 1 : optiVariables.frameReduction;
            // frame becomes previous frame (i.e., prev_frame stores information about current frame)
            prev_frame = matframe;

            Image<Bgr, Byte> imageCV = new Image<Bgr, byte>(bmp); //Image Class from Emgu.CV
            matframe = imageCV.Mat; //This is your Image converted to Mat

            if (prev_frame == null) {
                return bmp;
            }

            // frame_nr increment by number of steps given in textfield on user interface
            frame_nr += 1;


            // intialize this Image Matrix before resizing (see below), so it remains at original size
            img_average_vectors = new Image<Bgr, byte>(matframe.Width, matframe.Height);

            orig_height = matframe.Height;

            Size n_size = new Size(matframe.Width / frameReduction,
                 matframe.Height / frameReduction);

            // Resize frame and previous frame (smaller to reduce processing load)
            //Source

            Mat matFramDst = new Mat();
            using (GpuMat gMatSrc = new GpuMat())
            using (GpuMat gMatDst = new GpuMat()) {
                gMatSrc.Upload(matframe);
                Emgu.CV.Cuda.CudaInvoke.Resize(gMatSrc, gMatDst, new Size(0, 0), (double)1 / frameReduction, (double)1 / frameReduction);
                gMatDst.Download(matFramDst);
            }

            matframe = matFramDst;

            if (prev_frame.Height != matframe.Height) {
                return bmp;
            }



            // images that are compared during the flow operations (see below) 
            // these need to be greyscale images
            Image<Gray, Byte> prev_grey_img, curr_grey_img;

            prev_grey_img = new Image<Gray, byte>(prev_frame.Width, prev_frame.Height);
            curr_grey_img = new Image<Gray, byte>(matframe.Width, matframe.Height);

            // Image arrays to store information of flow vectors (one image array for each direction, which is x and y)
            Image<Gray, float> flow_x;
            Image<Gray, float> flow_y;

            flow_x = new Image<Gray, float>(matframe.Width, matframe.Height);
            flow_y = new Image<Gray, float>(matframe.Width, matframe.Height);

            // assign information stored in frame and previous frame in greyscale images (works without convert function)
            CvInvoke.CvtColor(matframe, curr_grey_img, ColorConversion.Bgr2Gray);
            CvInvoke.CvtColor(prev_frame, prev_grey_img, ColorConversion.Bgr2Gray);


            // Apply Farneback dense optical flow  
            // parameters are the two greyscale images (these are compared) 
            // and two image arrays storing the flow information
            // the results of the procedure are stored
            // the rest of the parameters are: 
            // pryScale: specifies image scale to build pyramids: 0.5 means that each next layer is twice smaller than the former
            // levels: number of pyramid levels: 1 means no extra layers
            // winSize: the average window size; larger values = more robust to noise but more blur
            // iterations: number of iterations at each pyramid level
            // polyN: size of pixel neighbourhood: higher = more precision but more blur
            // polySigma
            // flags


            CvInvoke.CalcOpticalFlowFarneback(prev_grey_img, curr_grey_img, flow_x, flow_y, 0.5, 3, 10, 3, 6, 1.3, 0);


            // call function that shows results of Farneback algorithm  
            Image<Bgr, Byte> farnebackImg = Draw_Farneback_flow_map(matframe.ToImage<Bgr, Byte>(), flow_x, flow_y, optiVariables);// given in global variables section

            // Release memory 
            prev_grey_img.Dispose();
            curr_grey_img.Dispose();
            flow_x.Dispose();
            flow_y.Dispose();

            //return farnebackImg.ToBitmap();

            Image<Bgra, Byte> alphaImgShape = new Image<Bgra, byte>(imageCV.Size.Width, imageCV.Size.Height, new Bgra(0, 0, 0, .5));
            CvInvoke.AddWeighted(alphaImgShape, .5, BlackTransparent(farnebackImg), .5, 0, alphaImgShape);

            Mat alphaimg = new Mat();
            CvInvoke.CvtColor(imageCV, alphaimg, ColorConversion.Bgr2Bgra);

            if (CudaInvoke.HasCuda) {
                using (GpuMat gMatSrc = new GpuMat())
                using (GpuMat gMatSrc2 = new GpuMat())
                using (GpuMat gMatDst = new GpuMat()) {
                    gMatSrc.Upload(alphaimg);
                    gMatSrc2.Upload(alphaImgShape);
                    CudaInvoke.AlphaComp(gMatSrc, gMatSrc2, gMatDst, AlphaCompTypes.Plus);
                    gMatDst.Download(alphaimg);
                }
                return alphaimg.Bitmap;
            }
            else {
                return Overlay(imageCV, alphaImgShape).ToBitmap();
            }
        }

        // function that depicts results of optical flow operations 
        // requires reference to image being processed, the results of Farneback algorithm stored in flow_x and flow_y
        // step gives the distance between pixels that are depicted, shift_that_counts is threshold for vector length that is used for calculations  
        private Image<Bgr, Byte> Draw_Farneback_flow_map(Image<Bgr, Byte> img_curr, Image<Gray, float> flow_x, Image<Gray, float> flow_y, OpticalFlowVariable optiVars) {

            // NOTE: flow Images (flow_x and flow_y) are organized like this:
            // at index (is position of pixel before optical flow operation) of Image array
            // the shift of this specific pixel after the flow operation is stored
            // if no shift has occured value stored at index is zero
            // (i.e., pixel[index] = 0 
            GC.Collect(0, GCCollectionMode.Forced);

            Image<Bgr, Byte> blackFrame = new Image<Bgr, Byte>(new Bitmap(1280 / frameReduction, 720 / frameReduction));

            System.Drawing.Point from_dot_xy = new System.Drawing.Point(); // point variable to draw lines between dots before and after flow (=vectors)
            System.Drawing.Point to_dot_xy = new System.Drawing.Point(); // point variable, which will be endpoint of line between dots before and after flow

            MCvScalar col; // variable to store color values of lines representing flow vectors
            col.V0 = 100;
            col.V1 = 255;
            col.V2 = 0;
            col.V3 = 100;


            // for drawing central line based on window size 
            System.Drawing.Point[] window_centre = new System.Drawing.Point[2];

            window_centre[0].X = img_curr.Width / 2;// * Convert.ToInt32(txt_resize_factor.Text)/ 2;
            window_centre[0].Y = 0;

            window_centre[1].X = img_curr.Width / 2; //* Convert.ToInt32(txt_resize_factor.Text) / 2;
            window_centre[1].Y = orig_height;


            // Point variables that constitute starting point for drawing summed and mean vectors onto image
            System.Drawing.Point vector_right = new System.Drawing.Point();
            System.Drawing.Point vector_left = new System.Drawing.Point();

            // variables used for summing vectors to left and to the right of the window's centre
            System.Drawing.Point vector_right_end_window = new System.Drawing.Point();
            System.Drawing.Point vector_left_end_window = new System.Drawing.Point();


            // determine centre of output window (needed for summed vectors) 
            int mid_point_horz = 1280 * frameReduction / 2; // width
            int mid_point_vert = 720 * frameReduction / 2; // height

            // landmark coordinates that are origin of direction vectors
            // near centre of image window; to depict motion of left and right half of "body" (or more precisely, window)
            vector_right.X = (mid_point_horz + 10) * optiVars.stepRate;
            vector_right.Y = mid_point_vert * optiVars.stepRate;

            vector_left.X = (mid_point_horz - 10) * optiVars.stepRate;
            vector_left.Y = mid_point_vert * optiVars.stepRate;


            // counting landmarks in flow field that exceed a certain value (shift_that_counts); left and right is based on centre of window (half of width)
            double count_X_right = 0;
            double count_Y_right = 0;

            double count_X_left = 0;
            double count_Y_left = 0;

            // loops over image matrix; position of dots before and after optical flow operations are compared and vector is drawn between the old and the new position
            for (int i = 0; i < flow_x.Rows; i += optiVars.stepRate) // NOTE: steps are given by step variable in arguments of method
                for (int j = 0; j < flow_x.Cols; j += optiVars.stepRate) {// BEGIN FOR

                    // pixel shift measured by optical flow is transferred to point variables 
                    // storing starting point of motion (from_dot..) and its end points (to_dot...)

                    to_dot_xy.X = (int)flow_x.Data[i, j, 0]; // access single pixel of flow matrix where x-coords of pixel after flow are stored; only gives the shift
                    to_dot_xy.Y = (int)flow_y.Data[i, j, 0]; // access single pixel of flow matrix where y-coords of pixel after flow are stored; only gives the shift 

                    from_dot_xy.X = j; //  index of loop is  position on image (here: x-coord); X is cols
                    from_dot_xy.Y = i; // index of of loop is  position on image (here: y-coord);  Y is rows


                    // LEFT SIDE OF WINDOW BASED CENTRE
                    if (j < window_centre[0].X) {
                        //  count the x and y indices and sum them when they exceed the value given by shift_that_counts (here:0)
                        if (Math.Abs(to_dot_xy.X) > optiVars.shiftThatCounts) {
                            count_X_left++;

                        }
                        if (Math.Abs(to_dot_xy.Y) > optiVars.shiftThatCounts) {

                            count_Y_left++;

                        }
                        // sum up vectors 
                        vector_left_end_window.Y += to_dot_xy.Y;
                        vector_left_end_window.X += to_dot_xy.X;

                    }
                    else //(j > window_centre[0].X)// WINDOW BASED CENTRE
                    {
                        //  like above; count the x and y indices and sum them 
                        if (Math.Abs(to_dot_xy.X) > optiVars.shiftThatCounts) {

                            count_X_right++;
                        }

                        if (Math.Abs(to_dot_xy.Y) > optiVars.shiftThatCounts) {

                            count_Y_right++;
                        }

                        // sum  vectors 
                        vector_right_end_window.Y += to_dot_xy.Y;
                        vector_right_end_window.X += to_dot_xy.X;
                    }

                    to_dot_xy.X = from_dot_xy.X + to_dot_xy.X; // new x-coord position of pixel (taking into account distance from the origin)   
                    to_dot_xy.Y = from_dot_xy.Y + to_dot_xy.Y; // new y-coord postion of pixel 

                    // draw line between coords on image and pixel shift stored in flow field after applying  optical-flow 
                    if (GetDistance(from_dot_xy.X, from_dot_xy.Y, to_dot_xy.X, to_dot_xy.Y) > optiVars.shiftThatCounts)
                        CvInvoke.Line(blackFrame, from_dot_xy, to_dot_xy, col, 1);


                    //CvInvoke.Imshow("Flow field vectors", img_curr); // show image with flow depicted as lines


                } // END of both for loops  


            Mat blackDst = new Mat();
            Mat BlackMat = blackFrame.Mat;
            using (GpuMat gMatSrc = new GpuMat())
            using (GpuMat gMatDst = new GpuMat()) {
                gMatSrc.Upload(BlackMat);
                Emgu.CV.Cuda.CudaInvoke.Resize(gMatSrc, gMatDst, new Size(0, 0), frameReduction, frameReduction, Inter.Area);
                gMatDst.Download(blackDst);
            }

            GC.Collect();

            return blackDst.ToImage<Bgr, Byte>();
        }

        /// <summary>
        /// end note: portions of code taken from https://www.codeproject.com/Articles/1192205/Capturing-motion-from-video-using-the-Emgu-CV-libr
        /// author: Markus Koppensteiner
        /// any code taken from codeproject is licenced under "The Code Project Open License (CPOL) 1.02"
        /// </summary>

        private static double GetDistance(double x1, double y1, double x2, double y2) {
            return Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
        }

        public static Image<Bgra, Byte> BlackTransparent(Image<Bgr, Byte> image, double additionalFade = 1) {
            Mat imageMat = image.Mat;
            Mat finalMat = new Mat(imageMat.Rows, imageMat.Cols, Emgu.CV.CvEnum.DepthType.Cv8U, 4);
            Mat tmp = new Mat(imageMat.Rows, imageMat.Cols, Emgu.CV.CvEnum.DepthType.Cv8U, 1);
            Mat alpha = new Mat(imageMat.Rows, imageMat.Cols, Emgu.CV.CvEnum.DepthType.Cv8U, 1);

            CvInvoke.CvtColor(imageMat, tmp, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
            CvInvoke.Threshold(tmp, alpha, 100, 255, Emgu.CV.CvEnum.ThresholdType.Binary);


            VectorOfMat rgb = new VectorOfMat(3);

            CvInvoke.Split(imageMat, rgb);

            Mat[] rgba = { rgb[0], rgb[1], rgb[2], alpha };

            VectorOfMat vector = new VectorOfMat(rgba);

            CvInvoke.Merge(vector, finalMat);

            return finalMat.ToImage<Bgra, Byte>();
        }

        public static Image<Bgr, Byte> Overlay(Image<Bgr, Byte> target, Image<Bgra, Byte> overlay) {
            Bitmap bmp = target.Bitmap;
            Graphics gra = Graphics.FromImage(bmp);
            gra.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
            gra.DrawImage(overlay.Bitmap, new System.Drawing.Point(0, 0));

            return target;
        }

        // uses cuda if available
        public static Bitmap ResizeImage(Bitmap imgToResize, Size size) {
            // emgu cuda detection can be a little finicy sometimes, but try catching cost virtually nothing compared to actually changing the size of the img  
            if (CudaInvoke.HasCuda) {
                try {
                    // determine ratio between current and desired size

                    double ratio = (double)size.Height / (imgToResize.Height);
                    Mat dst = new Mat();
                    Image<Bgr, Byte> imageCV = new Image<Bgr, byte>(imgToResize);
                    var result = imageCV.CopyBlank();
                    var handle = GCHandle.Alloc(result);
                    Mat matToResize = imageCV.Mat;
                    using (GpuMat gMatSrc = new GpuMat())
                    using (GpuMat gMatDst = new GpuMat()) {
                        gMatSrc.Upload(matToResize);
                        Emgu.CV.Cuda.CudaInvoke.Resize(gMatSrc, gMatDst, new Size(0, 0), ratio, ratio, Inter.Area);
                        gMatDst.Download(dst);
                    }
                    handle.Free();
                    return dst.Bitmap;
                }
                catch {
                    throw;
                }
            }
            else {
                throw new Exception("no cuda");
            }
        }
    }
}
