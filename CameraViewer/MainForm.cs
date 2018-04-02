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
        Camera Camera1;
        Camera Camera2;

        ContextMenu cm0;
        ContextMenu cm1;
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
        }

        Size cam0OriginalSize;
        bool cam0IsFullScreen;
        Point cam0OriginalLocation;
        private void Cam0MakeFullScreen(object sender, EventArgs e) {
            if (cam0IsFullScreen) {                
                this.protectedPictureBox0.Size = cam0OriginalSize;
                this.protectedPictureBox0.Location = cam0OriginalLocation;
                cam0IsFullScreen = false;
                cm0.MenuItems[0].Text = "Make Full Screen";
                Camera1.isFullScreenMode = false;
            }
            else {
                cam0OriginalSize = this.protectedPictureBox0.Size;
                cam0OriginalLocation = this.protectedPictureBox0.Location;
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
                this.protectedPictureBox1.Location = cam1OriginalLocation;
                cam1IsFullScreen = false;
                cm1.MenuItems[0].Text = "Make Full Screen";
                Camera2.isFullScreenMode = false;
            }
            else {
                cam1OriginalLocation = this.protectedPictureBox1.Location;
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

        // =================================================================================
        // get the devices         
        private void getXCamList() {
            List<string> Devices = Camera1.GetDeviceList();
            XCam_comboBox.Items.Clear();
            if (Devices.Count != 0) {
                for (int i = 0; i < Devices.Count; i++) {
                    XCam_comboBox.Items.Add(i.ToString() + ": " + Devices[i]);
                }
            }
            else {
                XCam_comboBox.Items.Add("----");
                XCamStatus_label.Text = "No Cam";
            }
            if (
                (Devices.Count > Program.Settings.Cam1.CamIndex) && (Program.Settings.Cam1.CamIndex > 0)) {
                XCam_comboBox.SelectedIndex = Program.Settings.Cam1.CamIndex;
            }
            else {
                XCam_comboBox.SelectedIndex = 0;  // default to first
            }            
        }

        // =================================================================================
        // get the devices         
        private void getYCamList() {
            List<string> Devices = Camera2.GetDeviceList();
            YCam_comboBox.Items.Clear();
            if (Devices.Count != 0) {
                for (int i = 0; i < Devices.Count; i++) {
                    YCam_comboBox.Items.Add(i.ToString() + ": " + Devices[i]);
                }
            }
            else {
                YCam_comboBox.Items.Add("----");
                XCamStatus_label.Text = "No Cam";
            }
            if (
                (Devices.Count > Program.Settings.Cam2.CamIndex) && (Program.Settings.Cam2.CamIndex > 0)) {
                YCam_comboBox.SelectedIndex = Program.Settings.Cam2.CamIndex;
            }
            else {
                YCam_comboBox.SelectedIndex = 0;  // default to first
            }
        }

        private void xCamSelect_Click(object sender, EventArgs e) {
            Program.Settings.Cam1.CamIndex = XCam_comboBox.SelectedIndex;

            while (Camera1.Active) {
                Camera1.SignalToStop();
                Thread.Sleep(50);
                Camera1.Active = false;
            }

            List <string> Monikers = Camera1.GetMonikerStrings();
            Program.Settings.Cam1.CamMoniker = Monikers[XCam_comboBox.SelectedIndex];
            AppSettings<Program.MySettings>.Save(Program.Settings);

            Camera1.MonikerString = Monikers[XCam_comboBox.SelectedIndex];

            Camera1.Active = true;

            Camera1.BuildOutFunctionsList();
            Camera1.Start("DownCamera", Program.Settings.Cam1.CamMoniker);

            if (!Camera1.ReceivingFrames) {
                MessageBox.Show("Camera being used by another process");
            }
        }

        private void YCamSelect_Click(object sender, EventArgs e) {
            Program.Settings.Cam2.CamIndex = YCam_comboBox.SelectedIndex;

            while (Camera2.Active) {
                Camera2.SignalToStop();
                Thread.Sleep(50);
                Camera2.Active = false;
            }

            List<string> Monikers = Camera2.GetMonikerStrings();
            Program.Settings.Cam2.CamMoniker = Monikers[YCam_comboBox.SelectedIndex];
            AppSettings<Program.MySettings>.Save(Program.Settings);

            Camera2.MonikerString = Monikers[YCam_comboBox.SelectedIndex];

            Camera2.Active = true;

            Camera2.BuildOutFunctionsList();
            Camera2.Start("UpCamera", Program.Settings.Cam2.CamMoniker);

            Thread.Sleep(100);

            if (!Camera2.ReceivingFrames) {
                MessageBox.Show("Camera being used by another process");
            }
        }

        private void MainForm_Load(object sender, EventArgs e) {

            Camera1 = new Camera(this);
            Camera2 = new Camera(this);

            getXCamList();
            getYCamList();

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
    }
}
