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

namespace CameraViewer {
    public partial class CameraAdjustments : UserControl {
        public Camera camera;
        bool xCamera = false;

        public CameraAdjustments() {
            InitializeComponent();
        }

        public void InitializeVariables(Camera thisCamera, bool xcamera) {
            camera = thisCamera;
            xCamera = xcamera;

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
                lblSaveFileLocation.Text = string.IsNullOrWhiteSpace(Program.Settings.Cam1.SaveLocation) ? "Save Location..." : Program.Settings.Cam1.SaveLocation.Substring(Program.Settings.Cam1.SaveLocation.Length < 24 ? 0 : Program.Settings.Cam1.SaveLocation.Length - 24);
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

                lblSaveFileLocation.Text = string.IsNullOrWhiteSpace(Program.Settings.Cam2.SaveLocation) ? "Save Location..." : Program.Settings.Cam1.SaveLocation.Substring(Program.Settings.Cam2.SaveLocation.Length < 24 ? 0 : Program.Settings.Cam2.SaveLocation.Length - 24);
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
                camera.BuildOutFunctionsList();
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
                camera.BuildOutFunctionsList();
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
                camera.BuildOutFunctionsList();
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
                camera.BuildOutFunctionsList();
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
                camera.BuildOutFunctionsList();
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
                camera.BuildOutFunctionsList();
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
                camera.BuildOutFunctionsList();
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
                        lblSaveFileLocation.Text = string.IsNullOrWhiteSpace(Program.Settings.Cam1.SaveLocation) ? "Save Location..." : Program.Settings.Cam1.SaveLocation.Substring(Program.Settings.Cam1.SaveLocation.Length < 24 ? 0 : Program.Settings.Cam1.SaveLocation.Length - 24);
                    }
                    else {
                        Program.Settings.Cam2.SaveLocation = directoryDialog.FileName;
                        lblSaveFileLocation.Text = string.IsNullOrWhiteSpace(Program.Settings.Cam2.SaveLocation) ? "Save Location..." : Program.Settings.Cam1.SaveLocation.Substring(Program.Settings.Cam2.SaveLocation.Length < 24 ? 0 : Program.Settings.Cam2.SaveLocation.Length - 24);
                    }

                    AppSettings<Program.MySettings>.Save(Program.Settings);
                }
            }
            catch { }
        }
    }
}
