﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraViewer {
    public class Eyetracking {
        public static int maxSize { get; } = 10;
        public Point[,] eyeTrackingMatrix = new Point[maxSize, maxSize];
        public int xCal, yCal;

        public int xTune = 0;
        public int yTune = 0;

        // primatives because thread safety + laziness
        public int rectX = 700;
        public int rectY = 130;
        public int rectHeight = 350;
        public int rectWidth = 250;        

        public Camera tempCam1;
        public Camera tempCam2;

        Camera eyeCam;
        Camera frontCam;

        public bool recordingPoints { get; private set; }
        public bool isActive { get; private set; }

        internal void SetCameraPosition(Camera eyeTrackingCam) {
            eyeCam = eyeTrackingCam;
            eyeCam.isEyeCamera = true;
            if (tempCam1 == eyeTrackingCam) {
                frontCam = tempCam2;
            }
            else {
                frontCam = tempCam1;
            }
            frontCam.isEyeCamera = false;
        }

        private double _trackedXval, _trackedYval;
        public double TrackedXVal {
            get {
                // everything is going to be inverted, the basic idea here, is that we have a matrix of calibration points
                // the calibration points line up with a grid of (at this time) 10x10 points
                // we will return a value between 0 and 10, basically interpolated between the two X points for the given y value
                try {
                    Tuple<int, double> point = GetNearestPoint(new Point((int)eyeCam.xEyeDetection, (int)eyeCam.yEyeDetection), ConvertMatrixToList(eyeTrackingMatrix));
                    Point closestPoint = new Point(point.Item1 % 10, point.Item1 / 10);
                    // now that we have the closest point we can interpolate
                    if (eyeCam.xEyeDetection > eyeTrackingMatrix[closestPoint.X, closestPoint.Y].X) {   // x is greater than the closest point, means we need interpolate between this and the lower point
                        if (eyeCam.yEyeDetection > eyeTrackingMatrix[closestPoint.X, closestPoint.Y].Y) {
                            if (closestPoint.X >= 1 && closestPoint.Y >= 1) {
                                _trackedXval = Math.Abs(linear(eyeCam.xEyeDetection, eyeTrackingMatrix[closestPoint.X, closestPoint.Y].X,
                                    eyeTrackingMatrix[closestPoint.X - 1, closestPoint.Y - 1].X, closestPoint.X, closestPoint.X - 1));
                                _trackedYval = Math.Abs(linear(eyeCam.yEyeDetection, eyeTrackingMatrix[closestPoint.X, closestPoint.Y].Y,
                                    eyeTrackingMatrix[closestPoint.X - 1, closestPoint.Y - 1].Y, closestPoint.Y, closestPoint.Y - 1));
                            }
                            else {
                                _trackedXval = closestPoint.X;
                                _trackedYval = closestPoint.Y;
                            }
                        }
                        else {
                            if (closestPoint.X >= 1 && closestPoint.Y <= Eyetracking.maxSize - 2) {
                                _trackedXval = Math.Abs(linear(eyeCam.xEyeDetection, eyeTrackingMatrix[closestPoint.X, closestPoint.Y].X,
                                    eyeTrackingMatrix[closestPoint.X - 1, closestPoint.Y - 1].X, closestPoint.X, closestPoint.X - 1));
                                _trackedYval = Math.Abs(linear(eyeCam.yEyeDetection, eyeTrackingMatrix[closestPoint.X, closestPoint.Y].Y,
                                    eyeTrackingMatrix[closestPoint.X - 1, closestPoint.Y - 1].Y, closestPoint.Y, closestPoint.Y - 1));
                            }
                            else {
                                _trackedXval = closestPoint.X;
                                _trackedYval = closestPoint.Y;
                            }
                        }
                    }
                    else {
                        if (eyeCam.yEyeDetection > eyeTrackingMatrix[closestPoint.X, closestPoint.Y].Y) {
                            if (closestPoint.X <= Eyetracking.maxSize - 2 && closestPoint.Y >= 1) {
                                _trackedXval = Math.Abs(linear(eyeCam.xEyeDetection, eyeTrackingMatrix[closestPoint.X, closestPoint.Y].X,
                                    eyeTrackingMatrix[closestPoint.X - 1, closestPoint.Y - 1].X, closestPoint.X, closestPoint.X - 1));
                                _trackedYval = Math.Abs(linear(eyeCam.yEyeDetection, eyeTrackingMatrix[closestPoint.X, closestPoint.Y].Y,
                                    eyeTrackingMatrix[closestPoint.X - 1, closestPoint.Y - 1].Y, closestPoint.Y, closestPoint.Y - 1));
                            }
                            else {
                                _trackedXval = closestPoint.X;
                                _trackedYval = closestPoint.Y;
                            }
                        }
                        else {
                            if (closestPoint.X <= Eyetracking.maxSize - 2 && closestPoint.Y <= Eyetracking.maxSize - 2) {
                                _trackedXval = Math.Abs(linear(eyeCam.xEyeDetection, eyeTrackingMatrix[closestPoint.X, closestPoint.Y].X,
                                    eyeTrackingMatrix[closestPoint.X - 1, closestPoint.Y - 1].X, closestPoint.X, closestPoint.X - 1));
                                _trackedYval = Math.Abs(linear(eyeCam.yEyeDetection, eyeTrackingMatrix[closestPoint.X, closestPoint.Y].Y,
                                    eyeTrackingMatrix[closestPoint.X - 1, closestPoint.Y - 1].Y, closestPoint.Y, closestPoint.Y - 1));
                            }
                            else {
                                _trackedXval = closestPoint.X;
                                _trackedYval = closestPoint.Y;
                            }
                        }
                    }

                    return _trackedXval;
                }
                catch { return 0; }
            }
        }
        // ask for TrackedXVal first!!
        public double TrackedYVal {
            get {
                return _trackedYval;
            }
        }

        static public double linear(double x, double x0, double x1, double y0, double y1) {
            if ((x1 - x0) == 0) {
                return (y0 + y1) / 2;
            }
            return y0 + (x - x0) * (y1 - y0) / (x1 - x0);
        }

        // output will can be reconstructed based on the formula (x+(y*10))
        private static List<Point> ConvertMatrixToList(Point[,] inputMatrix) {
            List<Point> toRet = new List<Point>();
            for (int i = 0; i < inputMatrix.GetLength(1); i++) {
                for (int j = 0; j < inputMatrix.GetLength(0); j++) {
                    toRet.Add(inputMatrix[j, i]);
                }
            }
            return toRet;
        }


        /// <summary>
        /// below code was taken from Dmitry  https://codereview.stackexchange.com/users/39186/dmitry 
        /// see answer: https://codereview.stackexchange.com/questions/139059/order-a-list-of-points-by-closest-distance
        /// </summary>
        /// <param name="toPoint"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        /// 
        private static Tuple<int, double> GetNearestPoint(Point toPoint, List<Point> points) {
            int nearestPoint = 0;
            double minDist2 = double.MaxValue;
            int i = 0;
            for (i = 0; i < points.Count; i++) {
                Point p = points[i];
                double dist2 = Distance2(p, toPoint);
                if (dist2 < minDist2) {
                    minDist2 = dist2;
                    nearestPoint = i;
                }
            }
            return new Tuple<int, double>(nearestPoint, minDist2);
        }

        private static double Pow2(double x) {
            return x * x;
        }

        private static double Distance2(Point p1, Point p2) {
            return Pow2(p2.X - p1.X) + Pow2(p2.Y - p1.Y);
        }
        /// <summary>
        /// end code taken from Dmitry  https://codereview.stackexchange.com/users/39186/dmitry 
        /// see answer: https://codereview.stackexchange.com/questions/139059/order-a-list-of-points-by-closest-distance
        /// </summary>


        internal bool Initialize(bool forceReInit = false) {
            if (!recordingPoints || forceReInit) {
                if (eyeCam == null)
                    return false;

                xCal = 0;
                yCal = 0;

                recordingPoints = true;
                isActive = true;
            }
            return true;
        }

        internal void GoBackOne() {
            if (xCal == 0 && yCal == 0)
                return;
            if (xCal-- == 0) {
                xCal = maxSize;
                if (yCal == 0) {
                    return;
                }
                yCal--;
            }
        }

        internal void RecordCalibrationPoint() {
            eyeTrackingMatrix[xCal, yCal] = new Point((int)eyeCam.xEyeDetection, (int)eyeCam.yEyeDetection);
            if (xCal++ == maxSize-1) {
                xCal = 0;
                if (yCal++ == maxSize-1) {
                    recordingPoints = false;
                }
            }
        }
    }
}
