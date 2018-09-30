using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace CameraViewer {
    public partial class MainForm : Form {
        Camera Camera1 { get; set; }
        Camera Camera2 { get; set; }

        ContextMenu cm0;
        ContextMenu cm1;

        Eyetracking eyeTracker = new Eyetracking();
        public MainForm() {
            InitializeComponent();

            cm0 = new ContextMenu();
            var item0 = cm0.MenuItems.Add("Make Full Screen");
            protectedPictureBox0.ContextMenu = cm0;
            item0.Click += new EventHandler(Cam0MakeFullScreen);

            cm1 = new ContextMenu();
            var item1 = cm1.MenuItems.Add("Make Full Screen");
            protectedPictureBox1.ContextMenu = cm1;
            item1.Click += new EventHandler(Cam1MakeFullScreen);


            // Set KeyPreview object to true to allow the form to process 
            // the key before the control with focus processes it.
            this.KeyPreview = true;

            // Associate the event-handling method with the
            // KeyDown event.
            this.KeyDown += new KeyEventHandler(MainForm_KeyDown);
        }

        // press any key to continue
        private void MainForm_KeyDown(object sender, KeyEventArgs e) {
            try {
                eyeTracker.RecordCalibrationPoint();
                e.Handled = true;
                cameraAdjustments1.BeginInvoke((MethodInvoker)delegate (){ cameraAdjustments1.UpdateEyeTracking();});
                cameraAdjustments2.BeginInvoke((MethodInvoker)delegate () { cameraAdjustments2.UpdateEyeTracking(); });
            }
            catch { } // do nothing 
        }

        Size cam0OriginalSize;
        bool cam0IsFullScreen;
        Point cam0OriginalLocation;
        private void Cam0MakeFullScreen(object sender, EventArgs e) {
            if (cam0IsFullScreen) {                
                this.protectedPictureBox0.Size = cam0OriginalSize;
                this.VerticalScroll.Value = 0;
                Application.DoEvents();
                this.protectedPictureBox0.Location = cam0OriginalLocation;
                cam0IsFullScreen = false;
                cm0.MenuItems[0].Text = "Make Full Screen";
                Camera1.isFullScreenMode = false;
                protectedPictureBox0.SendToBack();
            }
            else {
                cam0OriginalSize = this.protectedPictureBox0.Size;
                cam0OriginalLocation = new System.Drawing.Point(515, 0);
                Size maxSize = this.Size;
                this.protectedPictureBox0.Size = maxSize;
                this.protectedPictureBox0.Location = new Point(0, 0);
                this.protectedPictureBox0.BringToFront();
                cam0IsFullScreen = true;
                cm0.MenuItems[0].Text = "Make Normal Size";
                Camera1.isFullScreenMode = true;
                Camera1.fullscreenSize = maxSize;                                
            }
        }

        Size cam1OriginalSize;
        bool cam1IsFullScreen;
        Point cam1OriginalLocation;
        private void Cam1MakeFullScreen(object sender, EventArgs e) {
            if (cam1IsFullScreen) {
                this.protectedPictureBox1.Size = cam1OriginalSize;
                this.VerticalScroll.Value = 0;
                Application.DoEvents();
                this.protectedPictureBox1.Location = cam1OriginalLocation;
                cam1IsFullScreen = false;
                cm1.MenuItems[0].Text = "Make Full Screen";
                Camera2.isFullScreenMode = false;
                protectedPictureBox1.SendToBack();
            }
            else {
                cam1OriginalLocation = new System.Drawing.Point(515, 720);
                cam1OriginalSize = this.protectedPictureBox1.Size;
                Size maxSize = this.Size;
                this.protectedPictureBox1.Size = maxSize;
                this.protectedPictureBox1.Location = new Point(0, 0);
                this.protectedPictureBox1.BringToFront();
                cam1IsFullScreen = true;
                cm1.MenuItems[0].Text = "Make Normal Size";
                Camera2.isFullScreenMode = true;
                Camera2.fullscreenSize = maxSize;
            }
        }

        private void MainForm_Load(object sender, EventArgs e) {

            Camera1 = new Camera(this, cameraAdjustments1, eyeTracker);
            Camera2 = new Camera(this, cameraAdjustments1, eyeTracker);

            eyeTracker.tempCam1 = Camera1;
            eyeTracker.tempCam2 = Camera2;

            Camera1.ImageBox = protectedPictureBox0;
            Camera2.ImageBox = protectedPictureBox1;

            cameraAdjustments1.InitializeVariables(Camera1, true);
            cameraAdjustments2.InitializeVariables(Camera2, false);
        }        

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            System.Windows.Forms.Application.Exit();
        }

        private void protectedPictureBox0_Click(object sender, EventArgs e) {
            try {
                if (protectedPictureBox0.Image != null && !string.IsNullOrWhiteSpace(Program.Settings.Cam1.SaveLocation)) {
                    protectedPictureBox0.Image.Save($"{Program.Settings.Cam1.SaveLocation}\\{DateTime.Now.ToString().Replace(" ", String.Empty).Replace(":", String.Empty)}.bmp");                    
                }
            }
            catch (Exception) {
                MessageBox.Show("There was a problem saving the file." +
                    "Check the file permissions.");
            }
        }

        private void protectedPictureBox1_Click(object sender, EventArgs e) {
            try {
                if (protectedPictureBox1.Image != null && !string.IsNullOrWhiteSpace(Program.Settings.Cam2.SaveLocation)) {
                    protectedPictureBox1.Image.Save($"{Program.Settings.Cam2.SaveLocation}\\{DateTime.Now.ToString().Replace(" ", String.Empty).Replace(":", String.Empty)}.bmp");
                }
            }
            catch (Exception) {
                MessageBox.Show("There was a problem saving the file." +
                    "Check the file permissions.");
            }
        }

        private void tCPProtocolToolStripMenuItem_Click(object sender, EventArgs e) {

        }
    }
}
