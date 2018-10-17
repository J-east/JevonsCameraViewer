using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using DirectShowLib;
using System.Threading;
using EyeTracking;

namespace CameraViewer {
    public partial class CameraAdjustments : UserControl {
        public Camera camera;
        bool xCamera = false;

        bool isLoaded = false;
        public CameraAdjustments() {
            InitializeComponent();
            isLoaded = true;
        }

        public void InitializeVariables(Camera thisCamera, bool xcamera) {
            camera = thisCamera;
            xCamera = xcamera;

            GetCamList();

            if (xCamera) {
                cbRotate.Checked = Program.Settings.Cam1.CamRotate;
                cbMirror1.Checked = Program.Settings.Cam1.CamMirror;
                cbDrawGrid1.Checked = Program.Settings.Cam1.DrawGrid;
                cbGrayscale.Checked = Program.Settings.Cam1.grayScale;
                cbInvert.Checked = Program.Settings.Cam1.Invert;
                cbEdgeDetect.Checked = Program.Settings.Cam1.EdgeDetect;
                cbNoiseReduction.Checked = Program.Settings.Cam1.NoiseReduction;
                cbThreashold.Checked = Program.Settings.Cam1.Threshold;
                cbContrast.Checked = Program.Settings.Cam1.Contrast;
                cbZoom.Checked = Program.Settings.Cam1.Zoom;

                nGridInterval.Value = Program.Settings.Cam1.GridSpacing < 2 ? 2 : Program.Settings.Cam1.GridSpacing;
                cbEdgeDetection.SelectedIndex = Program.Settings.Cam1.EdgeDetectVal;
                cbNoiseReduce.SelectedIndex = Program.Settings.Cam1.NoiseReductionVal;
                nThreashold.Value = Program.Settings.Cam1.ThresholdVal;
                nZoom.Value = Program.Settings.Cam1.ZoomVal;
                lblSaveFileLocation.Text = string.IsNullOrWhiteSpace(Program.Settings.Cam1.SaveLocation) ? "Save Location..." : Program.Settings.Cam1.SaveLocation.Substring(Program.Settings.Cam1.SaveLocation.Length < 50 ? 0 : Program.Settings.Cam1.SaveLocation.Length - 50);

                // I don't want to see anything related to eyetracking
                nEyeTrackingTuningX.Visible = false;
                label23.Visible = false;
                label22.Visible = false;
                label21.Visible = false;
                label20.Visible = false;
                tbEyeSX.Visible = false;
                tbEyeRectY.Visible = false;
                tbEyeRectX.Visible = false;
                tbEyeSY.Visible = false;
                nEyeTrackingTuningY.Visible = false;
                cbEyeTracking.Visible = false;
                bInitialize.Visible = false;
                bReset.Visible = false;
                lblInitializeStatus.Visible = false;
                lblEyeTrackingInfo.Visible = false;
                cbIsEyeCam.Visible = false;
                bGoBack.Visible = false;
                lblEyetracking.Visible = false;
            }
            else {
                cbRotate.Checked = Program.Settings.Cam2.CamRotate;
                cbMirror1.Checked = Program.Settings.Cam2.CamMirror;
                cbDrawGrid1.Checked = Program.Settings.Cam2.DrawGrid;
                cbGrayscale.Checked = Program.Settings.Cam2.grayScale;
                cbInvert.Checked = Program.Settings.Cam2.Invert;
                cbEdgeDetect.Checked = Program.Settings.Cam2.EdgeDetect;
                cbNoiseReduction.Checked = Program.Settings.Cam2.NoiseReduction;
                cbThreashold.Checked = Program.Settings.Cam2.Threshold;
                cbContrast.Checked = Program.Settings.Cam2.Contrast;
                cbZoom.Checked = Program.Settings.Cam2.Zoom;

                nGridInterval.Value = Program.Settings.Cam2.GridSpacing < 2 ? 2 : Program.Settings.Cam2.GridSpacing;
                cbEdgeDetection.SelectedIndex = Program.Settings.Cam2.EdgeDetectVal;
                cbNoiseReduce.SelectedIndex = Program.Settings.Cam2.NoiseReductionVal;
                nThreashold.Value = Program.Settings.Cam2.ThresholdVal;
                nZoom.Value = Program.Settings.Cam2.ZoomVal;

                lblSaveFileLocation.Text = string.IsNullOrWhiteSpace(Program.Settings.Cam2.SaveLocation) ? "Save Location..." : Program.Settings.Cam1.SaveLocation.Substring(Program.Settings.Cam2.SaveLocation.Length < 50 ? 0 : Program.Settings.Cam2.SaveLocation.Length - 50);
            }
        }

        internal void UpdateEyeTracking() {
            if (camera.eyeTracker.recordingPoints) {
                lblInitializeStatus.Text = $"point {camera.eyeTracker.xCal + 1 * camera.eyeTracker.yCal + 1} of {Eyetracking.maxSize}";
            }
            else {
                lblInitializeStatus.Text = $"Eye tracking initialized";
            }

            lblEyeTrackingInfo.Text = $"x = {camera.eyeTracker.TrackedXVal}  y = {camera.eyeTracker.TrackedYVal}";
        }

        // =================================================================================
        // get the devices         
        private void GetCamList() {
            List<string> Devices = camera.GetDeviceList();
            cbCamSelection.Items.Clear();
            if (Devices.Count != 0) {
                for (int i = 0; i < Devices.Count; i++) {
                    cbCamSelection.Items.Add(i.ToString() + ": " + Devices[i]);
                }
            }
            else {
                cbCamSelection.Items.Add("----");
                XCamStatus_label.Text = "No Cam";
            }
            if (xCamera) {
                if ((Devices.Count > Program.Settings.Cam1.CamIndex) && (Program.Settings.Cam1.CamIndex > 0)) {
                    cbCamSelection.SelectedIndex = Program.Settings.Cam1.CamIndex;
                }
                else {
                    cbCamSelection.SelectedIndex = 0;
                }
            }
            else {
                if ((Devices.Count > Program.Settings.Cam2.CamIndex) && (Program.Settings.Cam2.CamIndex > 0)) {
                    cbCamSelection.SelectedIndex = Program.Settings.Cam2.CamIndex;
                }
                else {
                    cbCamSelection.SelectedIndex = 0;
                }
            }
        }

        private void xCamSelect_Click(object sender, EventArgs e) {
            while (camera.Active) {
                camera.SignalToStop();
                Thread.Sleep(50);
                camera.Active = false;
            }

            List<string> Monikers = camera.GetMonikerStrings();
            if (xCamera) {
                Program.Settings.Cam1.CamIndex = cbCamSelection.SelectedIndex;
                Program.Settings.Cam1.CamMoniker = Monikers[cbCamSelection.SelectedIndex];
                AppSettings<Program.MySettings>.Save(Program.Settings);
            }
            else {
                Program.Settings.Cam2.CamIndex = cbCamSelection.SelectedIndex;
                Program.Settings.Cam2.CamMoniker = Monikers[cbCamSelection.SelectedIndex];
                AppSettings<Program.MySettings>.Save(Program.Settings);
            }

            camera.MonikerString = Monikers[cbCamSelection.SelectedIndex];

            camera.Active = true;

            camera.Start("DownCamera", Monikers[cbCamSelection.SelectedIndex]);

            if (!camera.ReceivingFrames) {
                MessageBox.Show("Camera being used by another process");
            }
        }

        private void cbRotate_CheckedChanged(object sender, EventArgs e) {
            try { camera.rotateCam = cbRotate.Checked; } catch { }
            // yes i am lazy
            if (xCamera)
                Program.Settings.Cam1.CamRotate = cbRotate.Checked;
            else
                Program.Settings.Cam2.CamRotate = cbRotate.Checked;
            AppSettings<Program.MySettings>.Save(Program.Settings);
        }

        private void cbRotationAmount_SelectedIndexChanged(object sender, EventArgs e) {
            try { camera.rotateAmount = cbRotationAmount.SelectedIndex; } catch { }
            // yes i am lazy
            if (xCamera)
                Program.Settings.Cam1.CamRotate = cbRotate.Checked;
            else
                Program.Settings.Cam2.CamRotate = cbRotate.Checked;
            AppSettings<Program.MySettings>.Save(Program.Settings);
        }

        private void cbMirror1_CheckedChanged(object sender, EventArgs e) {
            try { camera.Mirror = cbMirror1.Checked; } catch { }
            if (xCamera)
                Program.Settings.Cam1.CamMirror = cbMirror1.Checked;
            else
                Program.Settings.Cam2.CamMirror = cbMirror1.Checked;
            AppSettings<Program.MySettings>.Save(Program.Settings);
        }

        private void cbDrawGrid1_CheckedChanged(object sender, EventArgs e) {
            try { camera.DrawGrid = cbDrawGrid1.Checked; } catch { }
            if (xCamera)
                Program.Settings.Cam1.DrawGrid = cbDrawGrid1.Checked;
            else
                Program.Settings.Cam2.DrawGrid = cbDrawGrid1.Checked;
            AppSettings<Program.MySettings>.Save(Program.Settings);
        }

        private void cbGrayscale_CheckedChanged(object sender, EventArgs e) {
            try {
                camera.GrayScale = cbGrayscale.Checked;
            }
            catch { }
            if (xCamera)
                Program.Settings.Cam1.grayScale = cbGrayscale.Checked;
            else
                Program.Settings.Cam2.grayScale = cbGrayscale.Checked;
            AppSettings<Program.MySettings>.Save(Program.Settings);
        }

        private void cbInvert_CheckedChanged(object sender, EventArgs e) {
            try {
                camera.Invert = cbInvert.Checked;
            }
            catch { }
            if (xCamera)
                Program.Settings.Cam1.Invert = cbInvert.Checked;
            else
                Program.Settings.Cam2.Invert = cbInvert.Checked;
            AppSettings<Program.MySettings>.Save(Program.Settings);
        }

        private void cbEdgeDetect_CheckedChanged(object sender, EventArgs e) {
            try {
                camera.EdgeDetect = cbEdgeDetect.Checked;
            }
            catch { }
            if (xCamera)
                Program.Settings.Cam1.EdgeDetect = cbEdgeDetect.Checked;
            else
                Program.Settings.Cam2.EdgeDetect = cbEdgeDetect.Checked;
            AppSettings<Program.MySettings>.Save(Program.Settings);
        }

        private void cbNoiseReduction_CheckedChanged(object sender, EventArgs e) {
            try {
                camera.NoiseReduce = cbNoiseReduction.Checked;
            }
            catch { }
            if (xCamera)
                Program.Settings.Cam1.NoiseReduction = cbNoiseReduction.Checked;
            else
                Program.Settings.Cam2.NoiseReduction = cbNoiseReduction.Checked;
            AppSettings<Program.MySettings>.Save(Program.Settings);
        }

        private void cbThreashold_CheckedChanged(object sender, EventArgs e) {
            try {
                camera.Threshold = cbThreashold.Checked;
            }
            catch { }
            if (xCamera)
                Program.Settings.Cam1.Threshold = cbThreashold.Checked;
            else
                Program.Settings.Cam2.Threshold = cbThreashold.Checked;
            AppSettings<Program.MySettings>.Save(Program.Settings);
        }

        private void cbContrast_CheckedChanged(object sender, EventArgs e) {
            try {
                camera.Contrast = cbContrast.Checked;
            }
            catch { }
            if (xCamera)
                Program.Settings.Cam1.Contrast = cbContrast.Checked;
            else
                Program.Settings.Cam2.Contrast = cbContrast.Checked;
            AppSettings<Program.MySettings>.Save(Program.Settings);
        }

        private void cbZoom_CheckedChanged(object sender, EventArgs e) {
            try {
                camera.Zoom = cbZoom.Checked;
            }
            catch { }
            if (xCamera)
                Program.Settings.Cam1.Zoom = cbZoom.Checked;
            else
                Program.Settings.Cam2.Zoom = cbZoom.Checked;
            AppSettings<Program.MySettings>.Save(Program.Settings);
        }

        // values
        private void nGridInterval_ValueChanged(object sender, EventArgs e) {
            try {
                camera.GridIncrement = (int)nGridInterval.Value;
            }
            catch { }
            if (xCamera)
                Program.Settings.Cam1.GridSpacing = camera.GridIncrement;
            else
                Program.Settings.Cam2.GridSpacing = camera.GridIncrement;
            AppSettings<Program.MySettings>.Save(Program.Settings);
        }

        private void cbEdgeDetection_SelectedIndexChanged(object sender, EventArgs e) {
            try { camera.EdgeDetectValue = (int)cbEdgeDetection.SelectedIndex; } catch { }
            if (xCamera)
                Program.Settings.Cam1.EdgeDetectVal = camera.EdgeDetectValue;
            else
                Program.Settings.Cam2.EdgeDetectVal = camera.EdgeDetectValue;
            AppSettings<Program.MySettings>.Save(Program.Settings);
        }

        private void cbNoiseReduce_SelectedIndexChanged(object sender, EventArgs e) {
            try { camera.NoiseReduceValue = (int)cbNoiseReduce.SelectedIndex; } catch { }
            if (xCamera)
                Program.Settings.Cam1.NoiseReductionVal = camera.NoiseReduceValue;
            else
                Program.Settings.Cam2.NoiseReductionVal = camera.NoiseReduceValue;
            AppSettings<Program.MySettings>.Save(Program.Settings);
        }

        private void nThreashold_ValueChanged(object sender, EventArgs e) {
            try { camera.ThresholdValue = (int)nThreashold.Value; } catch { }
            if (xCamera)
                Program.Settings.Cam1.ThresholdVal = camera.ThresholdValue;
            else
                Program.Settings.Cam2.ThresholdVal = camera.ThresholdValue;
            AppSettings<Program.MySettings>.Save(Program.Settings);
        }

        private void nZoom_ValueChanged(object sender, EventArgs e) {
            try { camera.ZoomValue = (int)nZoom.Value; } catch { }
            if (xCamera)
                Program.Settings.Cam1.GridSpacing = camera.ZoomValue;
            else
                Program.Settings.Cam2.GridSpacing = camera.ZoomValue;
            AppSettings<Program.MySettings>.Save(Program.Settings);
        }

        private void bSetSaveLocation_Click(object sender, EventArgs e) {
            var directoryDialog = new CommonOpenFileDialog {
                IsFolderPicker = true,
                Title = "Select Folder"
            };

            directoryDialog.ShowDialog();
            try {
                if (!string.IsNullOrWhiteSpace(directoryDialog.FileName)) {
                    if (xCamera) {
                        Program.Settings.Cam1.SaveLocation = directoryDialog.FileName;
                        lblSaveFileLocation.Text = string.IsNullOrWhiteSpace(Program.Settings.Cam1.SaveLocation) ? "Save Location..." : Program.Settings.Cam1.SaveLocation.Substring(Program.Settings.Cam1.SaveLocation.Length < 50 ? 0 : Program.Settings.Cam1.SaveLocation.Length - 50);
                    }
                    else {
                        Program.Settings.Cam2.SaveLocation = directoryDialog.FileName;
                        lblSaveFileLocation.Text = string.IsNullOrWhiteSpace(Program.Settings.Cam2.SaveLocation) ? "Save Location..." : Program.Settings.Cam1.SaveLocation.Substring(Program.Settings.Cam2.SaveLocation.Length < 50 ? 0 : Program.Settings.Cam2.SaveLocation.Length - 50);
                    }

                    AppSettings<Program.MySettings>.Save(Program.Settings);
                }
            }
            catch { }
        }


        private void cbLockExposure_CheckedChanged(object sender, EventArgs e) {
            if (!camera.Active) {
                return;
            }

            if (!isLoaded) {
                return;
            }

            if (cbLockExposure.Checked) {
                int g = 0;
                AForge.Video.DirectShow.CameraControlFlags controlFlags;
                // max -9 min 0
                camera.VideoSource.GetCameraProperty(AForge.Video.DirectShow.CameraControlProperty.Exposure, out g, out controlFlags);
                camera.VideoSource.SetCameraProperty(AForge.Video.DirectShow.CameraControlProperty.Exposure, g, AForge.Video.DirectShow.CameraControlFlags.Manual);
                camera.VideoSource.GetCameraProperty(AForge.Video.DirectShow.CameraControlProperty.Exposure, out g, out controlFlags);
                trackBarExposure.Value = g * -1 + 1;
                trackBarExposure.Visible = true;
            }
            else {
                camera.VideoSource.SetCameraProperty(AForge.Video.DirectShow.CameraControlProperty.Exposure, 10, AForge.Video.DirectShow.CameraControlFlags.Auto);
                trackBarExposure.Visible = false;
            }
        }

        private void trackBarExposure_Scroll(object sender, EventArgs e) {
            if (!isLoaded) {
                return;
            }
            camera.VideoSource.SetCameraProperty(AForge.Video.DirectShow.CameraControlProperty.Exposure, (trackBarExposure.Value - 1) * -1, AForge.Video.DirectShow.CameraControlFlags.Manual);
        }

        private void trackBar1_Scroll(object sender, EventArgs e) {
            camera.targetFramesSecond = trackBar1.Value;
            lblFrameRate.Text = $"Framerate tuning: {camera.targetFramesSecond}";
        }

        private void cbLineDetection_CheckedChanged(object sender, EventArgs e) {
            camera.ShapeVariables.calcLines = cbLineDetection.Checked;
        }

        private void cbCircleDetection_CheckedChanged(object sender, EventArgs e) {
            camera.ShapeVariables.calcCircles = cbCircleDetection.Checked;
        }

        private void cbRectangleTriDetection_CheckedChanged(object sender, EventArgs e) {
            camera.ShapeVariables.calcRectTri = cbRectangleTriDetection.Checked;
        }

        private void nlineCanny_ValueChanged(object sender, EventArgs e) {
            camera.ShapeVariables.lineCannyThreshold = (double)nlineCanny.Value;
        }

        private void nLineThreashold_ValueChanged(object sender, EventArgs e) {
            camera.ShapeVariables.lineThreshold = (int)nLineThreashold.Value;
        }

        private void nThresholdLinking_ValueChanged(object sender, EventArgs e) {
            camera.ShapeVariables.cannyThresholdLinking = (double)nThresholdLinking.Value;
        }

        private void nMinLineWidth_ValueChanged(object sender, EventArgs e) {
            camera.ShapeVariables.minLineWidth = (double)nMinLineWidth.Value;
        }

        private void nMinRadius_ValueChanged(object sender, EventArgs e) {
            camera.ShapeVariables.minradius = (int)nMinRadius.Value;
        }

        private void nMaxRadius_ValueChanged(object sender, EventArgs e) {
            camera.ShapeVariables.maxRadius = (int)nMaxRadius.Value;
        }

        private void nCircleCanny_ValueChanged(object sender, EventArgs e) {
            camera.ShapeVariables.circleCannyThreshold = (int)nCircleCanny.Value;
        }

        private void nCircleAccumulator_ValueChanged(object sender, EventArgs e) {
            camera.ShapeVariables.circleAccumulatorThreshold = (int)nCircleAccumulator.Value;
        }

        private void cbVisualFlow_CheckedChanged(object sender, EventArgs e) {
            camera.optiVariables.calcOpticalFlow = cbVisualFlow.Checked;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e) {
            camera.optiVariables.stepRate = (int)nFlowDensity.Value;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e) {
            camera.optiVariables.frameReduction = (int)((int)nResolutionReduction.Value % 2 == 1 ? nResolutionReduction.Value - 1 : nResolutionReduction.Value);
        }

        private void numericUpDown1_ValueChanged_1(object sender, EventArgs e) {
            camera.optiVariables.shiftThatCounts = (int)numericUpDown1.Value;
        }

        private void cbEyeTracking_CheckedChanged(object sender, EventArgs e) {

        }

        // start the initialization
        private void bInitialize_Click(object sender, EventArgs e) {
            if (!camera.eyeTracker.Initialize()) {
                MessageBox.Show("please set eye tracking camera");
                return;
            }
        }

        private void cbIsEyeCam_CheckedChanged(object sender, EventArgs e) {
            camera.isEyeCamera = cbIsEyeCam.Checked;
        }

        private void bReset_Click(object sender, EventArgs e) {
            camera.eyeTracker.Initialize(true);
        }

        private void cbR_CheckedChanged(object sender, EventArgs e) {
            try {
                camera.R = cbR.Checked;
            }
            catch { }
            if (xCamera)
                Program.Settings.Cam1.R = cbR.Checked;
            else
                Program.Settings.Cam2.R = cbR.Checked;
            AppSettings<Program.MySettings>.Save(Program.Settings);
        }

        private void cbG_CheckedChanged(object sender, EventArgs e) {
            try {
                camera.G = cbG.Checked;
            }
            catch { }
            if (xCamera)
                Program.Settings.Cam1.G = cbG.Checked;
            else
                Program.Settings.Cam2.G = cbG.Checked;
            AppSettings<Program.MySettings>.Save(Program.Settings);
        }

        private void cbB_CheckedChanged(object sender, EventArgs e) {
            try {
                camera.B = cbB.Checked;
            }
            catch { }
            if (xCamera)
                Program.Settings.Cam1.B = cbB.Checked;
            else
                Program.Settings.Cam2.B = cbB.Checked;
            AppSettings<Program.MySettings>.Save(Program.Settings);
        }

        private void XCam_comboBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (xCamera) {
                Program.Settings.Cam1.CamIndex = cbCamSelection.SelectedIndex;
                AppSettings<Program.MySettings>.Save(Program.Settings);
            }
            else {
                Program.Settings.Cam2.CamIndex = cbCamSelection.SelectedIndex;
                AppSettings<Program.MySettings>.Save(Program.Settings);
            }

        }

        private void tbEyeRectX_KeyPress(object sender, KeyPressEventArgs e) {
            if (!(Char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back)))
                e.Handled = true;
        }

        private void tbEyeRectY_KeyPress(object sender, KeyPressEventArgs e) {
            if (!(Char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back)))
                e.Handled = true;
        }

        private void tbEyeSX_KeyPress(object sender, KeyPressEventArgs e) {
            if (!(Char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back)))
                e.Handled = true;
        }

        private void tbEyeSY_KeyPress(object sender, KeyPressEventArgs e) {
            if (!(Char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back)))
                e.Handled = true;
        }

        private void nEyeTrackingTuningX_ValueChanged(object sender, EventArgs e) {
            camera.eyeTracker.xTune = (int)nEyeTrackingTuningX.Value;
        }

        private void nEyeTrackingTuningY_ValueChanged(object sender, EventArgs e) {
            camera.eyeTracker.yTune = (int)nEyeTrackingTuningY.Value;
        }

        private void tbEyeRectX_TextChanged(object sender, EventArgs e) {
            try {
                camera.eyeTracker.rectX = int.Parse(tbEyeRectX.Text);
            }
            catch { }
        }

        private void tbEyeRectY_TextChanged(object sender, EventArgs e) {
            try {
                camera.eyeTracker.rectY = int.Parse(tbEyeRectY.Text);
            }
            catch { }
        }

        private void tbEyeSX_TextChanged(object sender, EventArgs e) {
            try {
                camera.eyeTracker.rectHeight = int.Parse(tbEyeSX.Text);
            }
            catch { }
        }

        private void tbEyeSY_TextChanged(object sender, EventArgs e) {
            try {
                camera.eyeTracker.rectWidth = int.Parse(tbEyeSY.Text);
            }
            catch { }
        }
    }
}
