namespace CameraViewer {
    partial class MainForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.label1 = new System.Windows.Forms.Label();
            this.protectedPictureBox0 = new CameraViewer.Camera.ProtectedPictureBox();
            this.protectedPictureBox1 = new CameraViewer.Camera.ProtectedPictureBox();
            this.XCam_comboBox = new System.Windows.Forms.ComboBox();
            this.XCamStatus_label = new System.Windows.Forms.Label();
            this.YCam_comboBox = new System.Windows.Forms.ComboBox();
            this.YCamStatus_label = new System.Windows.Forms.Label();
            this.xCamSelect = new System.Windows.Forms.Button();
            this.YCamSelect = new System.Windows.Forms.Button();
            this.cameraAdjustments1 = new CameraViewer.CameraAdjustments();
            this.cameraAdjustments2 = new CameraViewer.CameraAdjustments();
            ((System.ComponentModel.ISupportInitialize)(this.protectedPictureBox0)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.protectedPictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Camera 1";
            // 
            // protectedPictureBox0
            // 
            this.protectedPictureBox0.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.protectedPictureBox0.Location = new System.Drawing.Point(243, 0);
            this.protectedPictureBox0.Name = "protectedPictureBox0";
            this.protectedPictureBox0.Size = new System.Drawing.Size(1204, 571);
            this.protectedPictureBox0.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.protectedPictureBox0.TabIndex = 10;
            this.protectedPictureBox0.TabStop = false;
            this.protectedPictureBox0.Click += new System.EventHandler(this.protectedPictureBox0_Click);
            // 
            // protectedPictureBox1
            // 
            this.protectedPictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.protectedPictureBox1.Location = new System.Drawing.Point(243, 577);
            this.protectedPictureBox1.Name = "protectedPictureBox1";
            this.protectedPictureBox1.Size = new System.Drawing.Size(1204, 571);
            this.protectedPictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.protectedPictureBox1.TabIndex = 11;
            this.protectedPictureBox1.TabStop = false;
            this.protectedPictureBox1.Click += new System.EventHandler(this.protectedPictureBox1_Click);
            // 
            // XCam_comboBox
            // 
            this.XCam_comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.XCam_comboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.XCam_comboBox.FormattingEnabled = true;
            this.XCam_comboBox.Location = new System.Drawing.Point(3, 8);
            this.XCam_comboBox.Name = "XCam_comboBox";
            this.XCam_comboBox.Size = new System.Drawing.Size(132, 21);
            this.XCam_comboBox.TabIndex = 5;
            // 
            // XCamStatus_label
            // 
            this.XCamStatus_label.AutoSize = true;
            this.XCamStatus_label.Location = new System.Drawing.Point(0, 610);
            this.XCamStatus_label.Name = "XCamStatus_label";
            this.XCamStatus_label.Size = new System.Drawing.Size(52, 13);
            this.XCamStatus_label.TabIndex = 12;
            this.XCamStatus_label.Text = "Camera 2";
            // 
            // YCam_comboBox
            // 
            this.YCam_comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.YCam_comboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.YCam_comboBox.FormattingEnabled = true;
            this.YCam_comboBox.Location = new System.Drawing.Point(3, 586);
            this.YCam_comboBox.Name = "YCam_comboBox";
            this.YCam_comboBox.Size = new System.Drawing.Size(132, 21);
            this.YCam_comboBox.TabIndex = 5;
            // 
            // YCamStatus_label
            // 
            this.YCamStatus_label.Location = new System.Drawing.Point(0, 0);
            this.YCamStatus_label.Name = "YCamStatus_label";
            this.YCamStatus_label.Size = new System.Drawing.Size(100, 63);
            this.YCamStatus_label.TabIndex = 0;
            // 
            // xCamSelect
            // 
            this.xCamSelect.Location = new System.Drawing.Point(3, 48);
            this.xCamSelect.Name = "xCamSelect";
            this.xCamSelect.Size = new System.Drawing.Size(75, 23);
            this.xCamSelect.TabIndex = 13;
            this.xCamSelect.Text = "Go";
            this.xCamSelect.UseVisualStyleBackColor = true;
            this.xCamSelect.Click += new System.EventHandler(this.xCamSelect_Click);
            // 
            // YCamSelect
            // 
            this.YCamSelect.Location = new System.Drawing.Point(3, 626);
            this.YCamSelect.Name = "YCamSelect";
            this.YCamSelect.Size = new System.Drawing.Size(75, 23);
            this.YCamSelect.TabIndex = 14;
            this.YCamSelect.Text = "Go";
            this.YCamSelect.UseVisualStyleBackColor = true;
            this.YCamSelect.Click += new System.EventHandler(this.YCamSelect_Click);
            // 
            // cameraAdjustments1
            // 
            this.cameraAdjustments1.Location = new System.Drawing.Point(3, 78);
            this.cameraAdjustments1.Name = "cameraAdjustments1";
            this.cameraAdjustments1.Size = new System.Drawing.Size(234, 456);
            this.cameraAdjustments1.TabIndex = 15;
            // 
            // cameraAdjustments2
            // 
            this.cameraAdjustments2.Location = new System.Drawing.Point(3, 655);
            this.cameraAdjustments2.Name = "cameraAdjustments2";
            this.cameraAdjustments2.Size = new System.Drawing.Size(234, 425);
            this.cameraAdjustments2.TabIndex = 16;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1457, 1065);
            this.Controls.Add(this.cameraAdjustments2);
            this.Controls.Add(this.cameraAdjustments1);
            this.Controls.Add(this.YCamSelect);
            this.Controls.Add(this.xCamSelect);
            this.Controls.Add(this.XCamStatus_label);
            this.Controls.Add(this.protectedPictureBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.protectedPictureBox0);
            this.Controls.Add(this.XCam_comboBox);
            this.Controls.Add(this.YCam_comboBox);
            this.Controls.Add(this.YCamStatus_label);
            this.Name = "MainForm";
            this.Text = "Jevons Camera Viewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.protectedPictureBox0)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.protectedPictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private Camera.ProtectedPictureBox protectedPictureBox0;
        private Camera.ProtectedPictureBox protectedPictureBox1;
        private System.Windows.Forms.ComboBox XCam_comboBox;
        private System.Windows.Forms.Label XCamStatus_label;
        private System.Windows.Forms.ComboBox YCam_comboBox;
        private System.Windows.Forms.Label YCamStatus_label;
        private System.Windows.Forms.Button xCamSelect;
        private System.Windows.Forms.Button YCamSelect;
        private CameraAdjustments cameraAdjustments1;
        private CameraAdjustments cameraAdjustments2;
    }
}

