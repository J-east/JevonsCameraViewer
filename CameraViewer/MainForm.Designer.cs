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
            this.protectedPictureBox0 = new CameraViewer.Camera.ProtectedPictureBox();
            this.protectedPictureBox1 = new CameraViewer.Camera.ProtectedPictureBox();
            this.cameraAdjustments1 = new CameraViewer.CameraAdjustments();
            this.cameraAdjustments2 = new CameraViewer.CameraAdjustments();
            ((System.ComponentModel.ISupportInitialize)(this.protectedPictureBox0)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.protectedPictureBox1)).BeginInit();
            this.SuspendLayout();

            // 
            // protectedPictureBox0
            // 
            this.protectedPictureBox0.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top)
| System.Windows.Forms.AnchorStyles.Right)));
            this.protectedPictureBox0.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.protectedPictureBox0.Location = new System.Drawing.Point(515, 0);
            this.protectedPictureBox0.Name = "protectedPictureBox0";
            this.protectedPictureBox0.Size = new System.Drawing.Size(1280, 720);
            this.protectedPictureBox0.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.protectedPictureBox0.TabIndex = 10;
            this.protectedPictureBox0.TabStop = false;
            this.protectedPictureBox0.Click += new System.EventHandler(this.protectedPictureBox0_Click);
            this.protectedPictureBox0.SendToBack();
            // 
            // protectedPictureBox1
            // 
            this.protectedPictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top)
| System.Windows.Forms.AnchorStyles.Right)));
            this.protectedPictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.protectedPictureBox1.Location = new System.Drawing.Point(515, 720);
            this.protectedPictureBox1.Name = "protectedPictureBox1";
            this.protectedPictureBox1.Size = new System.Drawing.Size(1280, 720);
            this.protectedPictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.protectedPictureBox1.TabIndex = 11;
            this.protectedPictureBox1.TabStop = false;
            this.protectedPictureBox1.Click += new System.EventHandler(this.protectedPictureBox1_Click);
            this.protectedPictureBox1.SendToBack();

            
            // 
            // cameraAdjustments1
            // 
            this.cameraAdjustments1.Location = new System.Drawing.Point(3, 3);
            this.cameraAdjustments1.Name = "cameraAdjustments1";
            this.cameraAdjustments1.Size = new System.Drawing.Size(510, 720);
            this.cameraAdjustments1.TabIndex = 15;
            // 
            // cameraAdjustments2
            // 
            this.cameraAdjustments2.Location = new System.Drawing.Point(3, 720);
            this.cameraAdjustments2.Name = "cameraAdjustments2";
            this.cameraAdjustments2.Size = new System.Drawing.Size(510, 720);
            this.cameraAdjustments2.TabIndex = 16;


            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1800, 1065);
            this.Controls.Add(this.cameraAdjustments2);
            this.Controls.Add(this.cameraAdjustments1);
            this.Controls.Add(this.protectedPictureBox1);
            this.Controls.Add(this.protectedPictureBox0);

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

        private Camera.ProtectedPictureBox protectedPictureBox0;
        private Camera.ProtectedPictureBox protectedPictureBox1;
        private CameraAdjustments cameraAdjustments1;
        private CameraAdjustments cameraAdjustments2;
    }
}

