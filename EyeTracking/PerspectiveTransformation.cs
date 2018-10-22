using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Accord.Math;
using Accord.Math.Geometry;
using Accord.Imaging;
using Accord.Vision;

namespace EyeTracking {
    /// <summary>
    /// this class uses the known location of 4 points to determine the transformation matrix used to remap the tracked eye point
    /// onto a surface even after user moves their head.
    /// </summary>
    public class PerspectiveTransformation {
        MatrixH homography;
        RansacHomographyEstimator ransac = new RansacHomographyEstimator(0.01, 0.95);

        // model used to draw coordinate system's axes
        private Vector3[] axesModel = new Vector3[]
        {
            new Vector3( 0, 0, 0 ),
            new Vector3( 1, 0, 0 ),
            new Vector3( 0, 1, 0 ),
            new Vector3( 0, 0, 1 ),
        };

        // colors used to highlight points on image
        private Color[] pointsColors = new Color[4]
        {
            Color.Yellow, Color.Blue, Color.Red, Color.Lime
        };

        public readonly Accord.Point emptyPoint = new Accord.Point(-30000, -30000);

        // image point of the object to estimate pose for
        private Accord.Point[] imagePoints = new Accord.Point[4];
        public Rectangle[] ImagePoints {
            set {
                imagePoints[0] = new Accord.Point(value[0].Location.X, value[0].Location.Y);
                imagePoints[1] = new Accord.Point(value[1].Location.X, value[1].Location.Y);
                imagePoints[2] = new Accord.Point(value[2].Location.X, value[2].Location.Y);
                imagePoints[3] = new Accord.Point(value[3].Location.X, value[3].Location.Y);
            }
        }

        // camera's focal length, this is really just how the camera relates to the coordinates of the modelPoints
        private float focalLength = 1000;
        // estimated transformation
        private Matrix3x3 rotationMatrix, bestRotationMatrix, alternateRotationMatrix;
        private Vector3 translationVector, bestTranslationVector, alternateTranslationVector;
        private bool isPoseEstimated = false;
        private float modelRadius;
        Matrix4x4 transformationMatrix;

        Vector3 modelCenter;
        private Vector3[] modelPoints = new Vector3[4];
        private PointF[] modelCorners = new PointF[4];
        public PerspectiveTransformation(Rectangle[] InitImagePoints) {
            modelPoints = new Vector3[] {
                new Vector3( InitImagePoints[0].Location.X, 0, InitImagePoints[0].Location.Y),
                new Vector3(  InitImagePoints[1].Location.X, 0, InitImagePoints[1].Location.Y),
                new Vector3(  InitImagePoints[2].Location.X, 0, InitImagePoints[2].Location.Y),
                new Vector3( InitImagePoints[3].Location.X, 0, InitImagePoints[3].Location.Y),
            };
            focalLength = 1000;

            modelCorners = new PointF[] {
                new PointF(InitImagePoints[0].Location.X, InitImagePoints[0].Location.Y),
                new PointF(InitImagePoints[1].Location.X, InitImagePoints[1].Location.Y),
                new PointF(InitImagePoints[2].Location.X, InitImagePoints[2].Location.Y),
                new PointF(InitImagePoints[3].Location.X, InitImagePoints[3].Location.Y),
            };

            // calculate model's center
            modelCenter = new Vector3(
                (modelPoints[0].X + modelPoints[1].X + modelPoints[2].X + modelPoints[3].X) / 4,
                (modelPoints[0].Y + modelPoints[1].Y + modelPoints[2].Y + modelPoints[3].Y) / 4,
                (modelPoints[0].Z + modelPoints[1].Z + modelPoints[2].Z + modelPoints[3].Z) / 4
            );

            // calculate ~ model's radius
            modelRadius = 0;
            foreach (Vector3 modelPoint in modelPoints) {
                float distanceToCenter = (modelPoint - modelCenter).Norm;
                if (distanceToCenter > modelRadius) {
                    modelRadius = distanceToCenter;
                }
            }
        }

        // returns the corrected eye tracking point, requires that image points be updated
        // if we are calibrating, then this gets approached completely differently, it will be the inverse
        public Point CorrectForHeadMovement(Point input, bool isCalibrating = false) {
            if (isCalibrating)
                homography = ransac.Estimate(imagePoints.Select(a => new PointF(a.X, a.Y)).ToArray(), modelCorners);
            else
                homography = ransac.Estimate(modelCorners, imagePoints.Select(a => new PointF(a.X, a.Y)).ToArray());

            PointF ret = homography.TransformPoints(new PointF[] { new PointF(input.X, input.Y) }).First();

            return new Point((int)ret.X, (int)ret.Y);
        }

        Accord.Imaging.MatrixH matrixH;
        public void DoPosit() {
            // run the point through the transformation matrix
            CoplanarPosit coposit = new CoplanarPosit(modelPoints, focalLength);
            coposit.EstimatePose(imagePoints, out rotationMatrix, out translationVector);

            //bestRotationMatrix = coposit.BestEstimatedRotation;
            //bestTranslationVector = coposit.BestEstimatedTranslation;

            //alternateRotationMatrix = coposit.AlternateEstimatedRotation;
            //alternateTranslationVector = coposit.AlternateEstimatedTranslation;

            rotationMatrix = coposit.AlternateEstimatedRotation;
            translationVector = coposit.AlternateEstimatedTranslation;

            transformationMatrix = Matrix4x4.CreateTranslation(translationVector) * Matrix4x4.CreateFromRotation(rotationMatrix);

            UpdateEstimationInformation();
            isPoseEstimated = true;
        }

        private Accord.Point[] PerformProjection(Matrix4x4 transformationMatrix, int viewSize) {
            Accord.Point[] projectedPoints = new Accord.Point[axesModel.Length];

            for (int i = 0; i < axesModel.Length; i++) {
                Vector3 scenePoint = (transformationMatrix * axesModel[i].ToVector4()).ToVector3();

                projectedPoints[i] = new Accord.Point(
                    (int)(scenePoint.X),
                    (int)(scenePoint.Y));
            }

            return projectedPoints;
        }

        public Bitmap DrawPoints(Bitmap frame) {
            Pen GPen = new Pen(Color.LawnGreen, 1);
            Graphics g = Graphics.FromImage(frame);

            for (int i = 0; i < 4; i++) {
                if (imagePoints[i] != emptyPoint) {
                    using (Brush brush = new SolidBrush(pointsColors[i])) {
                        g.FillEllipse(brush, new Rectangle(
                            (int)(imagePoints[i].X),
                            (int)(imagePoints[i].Y),
                            7, 7));
                    }
                }
            }

            if (isPoseEstimated) {
                foreach (Vector3 modelPoint in modelPoints) {
                    float distanceToCenter = (modelPoint - modelCenter).Norm;
                    if (distanceToCenter > modelRadius) {
                        modelRadius = distanceToCenter;
                    }
                }

                foreach (Point p in modelPoints.Select(a => new Point((int)a.X, (int)a.Z))) {
                    Point testPoint = CorrectForHeadMovement(p);

                    using (Brush brush = new SolidBrush(Color.BlueViolet)) {
                        g.FillEllipse(brush, new Rectangle(
                            (int)(testPoint.X),
                            (int)(testPoint.Y),
                            12, 12));
                    }
                }
            }

            return frame;
        }

        private void UpdateEstimationInformation() {
            float estimatedYaw;
            float estimatedPitch;
            float estimatedRoll;

            rotationMatrix.ExtractYawPitchRoll(out estimatedYaw, out estimatedPitch, out estimatedRoll);

            estimatedYaw *= (float)(180.0 / Math.PI);
            estimatedPitch *= (float)(180.0 / Math.PI);
            estimatedRoll *= (float)(180.0 / Math.PI);
        }
    }
}
