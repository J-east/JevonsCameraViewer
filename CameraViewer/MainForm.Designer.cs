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
            this.panel1 = new System.Windows.Forms.Panel();
            this.pVideoPorts = new System.Windows.Forms.Panel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tCPProtocolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loggingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.protectedPictureBox0)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.protectedPictureBox1)).BeginInit();
            this.panel1.SuspendLayout();
            this.pVideoPorts.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // protectedPictureBox0
            // 
            this.protectedPictureBox0.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.protectedPictureBox0.Dock = System.Windows.Forms.DockStyle.Top;
            this.protectedPictureBox0.Location = new System.Drawing.Point(0, 0);
            this.protectedPictureBox0.Name = "protectedPictureBox0";
            this.protectedPictureBox0.Size = new System.Drawing.Size(1313, 720);
            this.protectedPictureBox0.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.protectedPictureBox0.TabIndex = 10;
            this.protectedPictureBox0.TabStop = false;
            this.protectedPictureBox0.Click += new System.EventHandler(this.protectedPictureBox0_Click);
            // 
            // protectedPictureBox1
            // 
            this.protectedPictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.protectedPictureBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.protectedPictureBox1.Location = new System.Drawing.Point(0, 720);
            this.protectedPictureBox1.Name = "protectedPictureBox1";
            this.protectedPictureBox1.Size = new System.Drawing.Size(1313, 720);
            this.protectedPictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.protectedPictureBox1.TabIndex = 11;
            this.protectedPictureBox1.TabStop = false;
            this.protectedPictureBox1.Click += new System.EventHandler(this.protectedPictureBox1_Click);
            // 
            // cameraAdjustments1
            // 
            this.cameraAdjustments1.Location = new System.Drawing.Point(3, 3);
            this.cameraAdjustments1.Name = "cameraAdjustments1";
            this.cameraAdjustments1.Size = new System.Drawing.Size(510, 668);
            this.cameraAdjustments1.TabIndex = 15;
            // 
            // cameraAdjustments2
            // 
            this.cameraAdjustments2.Location = new System.Drawing.Point(3, 677);
            this.cameraAdjustments2.Name = "cameraAdjustments2";
            this.cameraAdjustments2.Size = new System.Drawing.Size(507, 778);
            this.cameraAdjustments2.TabIndex = 16;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.cameraAdjustments2);
            this.panel1.Controls.Add(this.cameraAdjustments1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(534, 1044);
            this.panel1.TabIndex = 17;
            // 
            // pVideoPorts
            // 
            this.pVideoPorts.AutoScroll = true;
            this.pVideoPorts.Controls.Add(this.protectedPictureBox0);
            this.pVideoPorts.Controls.Add(this.protectedPictureBox1);
            this.pVideoPorts.Dock = System.Windows.Forms.DockStyle.Right;
            this.pVideoPorts.Location = new System.Drawing.Point(541, 24);
            this.pVideoPorts.Name = "pVideoPorts";
            this.pVideoPorts.Size = new System.Drawing.Size(1330, 1044);
            this.pVideoPorts.TabIndex = 18;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1871, 24);
            this.menuStrip1.TabIndex = 19;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tCPProtocolToolStripMenuItem,
            this.loggingToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // tCPProtocolToolStripMenuItem
            // 
            this.tCPProtocolToolStripMenuItem.Name = "tCPProtocolToolStripMenuItem";
            this.tCPProtocolToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.tCPProtocolToolStripMenuItem.Text = "TCP Protocol";
            this.tCPProtocolToolStripMenuItem.Click += new System.EventHandler(this.tCPProtocolToolStripMenuItem_Click);
            // 
            // loggingToolStripMenuItem
            // 
            this.loggingToolStripMenuItem.Name = "loggingToolStripMenuItem";
            this.loggingToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.loggingToolStripMenuItem.Text = "Logging";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.aboutToolStripMenuItem.Text = "about";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1871, 1068);
            this.Controls.Add(this.pVideoPorts);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Jevons Camera Viewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.protectedPictureBox0)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.protectedPictureBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.pVideoPorts.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Camera.ProtectedPictureBox protectedPictureBox0;
        private Camera.ProtectedPictureBox protectedPictureBox1;
        private CameraAdjustments cameraAdjustments1;
        private CameraAdjustments cameraAdjustments2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel pVideoPorts;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tCPProtocolToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loggingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
    }
}

