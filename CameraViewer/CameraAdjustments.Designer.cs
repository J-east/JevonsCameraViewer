namespace CameraViewer {
    partial class CameraAdjustments {
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.cbRotationAmount = new System.Windows.Forms.ComboBox();
            this.cbNoiseReduce = new System.Windows.Forms.ComboBox();
            this.cbEdgeDetection = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.nZoom = new System.Windows.Forms.NumericUpDown();
            this.cbZoom = new System.Windows.Forms.CheckBox();
            this.cbContrast = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.nThreashold = new System.Windows.Forms.NumericUpDown();
            this.cbThreashold = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cbNoiseReduction = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cbEdgeDetect = new System.Windows.Forms.CheckBox();
            this.cbInvert = new System.Windows.Forms.CheckBox();
            this.cbGrayscale = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.nGridInterval = new System.Windows.Forms.NumericUpDown();
            this.cbDrawGrid1 = new System.Windows.Forms.CheckBox();
            this.cbMirror1 = new System.Windows.Forms.CheckBox();
            this.cbRotate = new System.Windows.Forms.CheckBox();
            this.bSetSaveLocation = new System.Windows.Forms.Button();
            this.lblSaveFileLocation = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nZoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nThreashold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nGridInterval)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblSaveFileLocation);
            this.panel1.Controls.Add(this.bSetSaveLocation);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.cbRotationAmount);
            this.panel1.Controls.Add(this.cbNoiseReduce);
            this.panel1.Controls.Add(this.cbEdgeDetection);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.nZoom);
            this.panel1.Controls.Add(this.cbZoom);
            this.panel1.Controls.Add(this.cbContrast);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.nThreashold);
            this.panel1.Controls.Add(this.cbThreashold);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.cbNoiseReduction);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.cbEdgeDetect);
            this.panel1.Controls.Add(this.cbInvert);
            this.panel1.Controls.Add(this.cbGrayscale);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.nGridInterval);
            this.panel1.Controls.Add(this.cbDrawGrid1);
            this.panel1.Controls.Add(this.cbMirror1);
            this.panel1.Controls.Add(this.cbRotate);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(233, 420);
            this.panel1.TabIndex = 22;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 46;
            this.label1.Text = "Rotation Amount";
            // 
            // cbRotationAmount
            // 
            this.cbRotationAmount.FormattingEnabled = true;
            this.cbRotationAmount.Items.AddRange(new object[] {
            "90 degrees",
            "180 degrees",
            "270 degrees"});
            this.cbRotationAmount.Location = new System.Drawing.Point(133, 18);
            this.cbRotationAmount.Name = "cbRotationAmount";
            this.cbRotationAmount.Size = new System.Drawing.Size(97, 21);
            this.cbRotationAmount.TabIndex = 45;
            this.cbRotationAmount.SelectedIndexChanged += new System.EventHandler(this.cbRotationAmount_SelectedIndexChanged);
            // 
            // cbNoiseReduce
            // 
            this.cbNoiseReduce.FormattingEnabled = true;
            this.cbNoiseReduce.Items.AddRange(new object[] {
            "Bilarteral Smth",
            "Median",
            "Mean"});
            this.cbNoiseReduce.Location = new System.Drawing.Point(133, 222);
            this.cbNoiseReduce.Name = "cbNoiseReduce";
            this.cbNoiseReduce.Size = new System.Drawing.Size(98, 21);
            this.cbNoiseReduce.TabIndex = 44;
            this.cbNoiseReduce.SelectedIndexChanged += new System.EventHandler(this.cbNoiseReduce_SelectedIndexChanged);
            // 
            // cbEdgeDetection
            // 
            this.cbEdgeDetection.FormattingEnabled = true;
            this.cbEdgeDetection.Items.AddRange(new object[] {
            "Sobel",
            "Difference",
            "Homogenity",
            "Canny"});
            this.cbEdgeDetection.Location = new System.Drawing.Point(133, 162);
            this.cbEdgeDetection.Name = "cbEdgeDetection";
            this.cbEdgeDetection.Size = new System.Drawing.Size(97, 21);
            this.cbEdgeDetection.TabIndex = 43;
            this.cbEdgeDetection.SelectedIndexChanged += new System.EventHandler(this.cbEdgeDetection_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 345);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(67, 13);
            this.label8.TabIndex = 42;
            this.label8.Text = "Zoom Factor";
            // 
            // nZoom
            // 
            this.nZoom.Location = new System.Drawing.Point(170, 343);
            this.nZoom.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nZoom.Name = "nZoom";
            this.nZoom.Size = new System.Drawing.Size(60, 20);
            this.nZoom.TabIndex = 41;
            this.nZoom.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nZoom.ValueChanged += new System.EventHandler(this.nZoom_ValueChanged);
            // 
            // cbZoom
            // 
            this.cbZoom.AutoSize = true;
            this.cbZoom.Location = new System.Drawing.Point(3, 325);
            this.cbZoom.Name = "cbZoom";
            this.cbZoom.Size = new System.Drawing.Size(53, 17);
            this.cbZoom.TabIndex = 40;
            this.cbZoom.Text = "Zoom";
            this.cbZoom.UseVisualStyleBackColor = true;
            this.cbZoom.CheckedChanged += new System.EventHandler(this.cbZoom_CheckedChanged);
            // 
            // cbContrast
            // 
            this.cbContrast.AutoSize = true;
            this.cbContrast.Location = new System.Drawing.Point(3, 302);
            this.cbContrast.Name = "cbContrast";
            this.cbContrast.Size = new System.Drawing.Size(72, 17);
            this.cbContrast.TabIndex = 37;
            this.cbContrast.Text = "Normalize";
            this.cbContrast.UseVisualStyleBackColor = true;
            this.cbContrast.CheckedChanged += new System.EventHandler(this.cbContrast_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 275);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(90, 13);
            this.label7.TabIndex = 36;
            this.label7.Text = "Threashold Value";
            // 
            // nThreashold
            // 
            this.nThreashold.Location = new System.Drawing.Point(170, 273);
            this.nThreashold.Maximum = new decimal(new int[] {
            254,
            0,
            0,
            0});
            this.nThreashold.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nThreashold.Name = "nThreashold";
            this.nThreashold.Size = new System.Drawing.Size(60, 20);
            this.nThreashold.TabIndex = 35;
            this.nThreashold.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nThreashold.ValueChanged += new System.EventHandler(this.nThreashold_ValueChanged);
            // 
            // cbThreashold
            // 
            this.cbThreashold.AutoSize = true;
            this.cbThreashold.Location = new System.Drawing.Point(3, 255);
            this.cbThreashold.Name = "cbThreashold";
            this.cbThreashold.Size = new System.Drawing.Size(79, 17);
            this.cbThreashold.TabIndex = 34;
            this.cbThreashold.Text = "Threashold";
            this.cbThreashold.UseVisualStyleBackColor = true;
            this.cbThreashold.CheckedChanged += new System.EventHandler(this.cbThreashold_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 225);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(116, 13);
            this.label6.TabIndex = 33;
            this.label6.Text = "Noise Reduction Value";
            // 
            // cbNoiseReduction
            // 
            this.cbNoiseReduction.AutoSize = true;
            this.cbNoiseReduction.Location = new System.Drawing.Point(3, 205);
            this.cbNoiseReduction.Name = "cbNoiseReduction";
            this.cbNoiseReduction.Size = new System.Drawing.Size(105, 17);
            this.cbNoiseReduction.TabIndex = 31;
            this.cbNoiseReduction.Text = "Noise Reduction";
            this.cbNoiseReduction.UseVisualStyleBackColor = true;
            this.cbNoiseReduction.CheckedChanged += new System.EventHandler(this.cbNoiseReduction_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 165);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(127, 13);
            this.label5.TabIndex = 30;
            this.label5.Text = "Edge Detection Algorithm";
            // 
            // cbEdgeDetect
            // 
            this.cbEdgeDetect.AutoSize = true;
            this.cbEdgeDetect.Location = new System.Drawing.Point(3, 145);
            this.cbEdgeDetect.Name = "cbEdgeDetect";
            this.cbEdgeDetect.Size = new System.Drawing.Size(86, 17);
            this.cbEdgeDetect.TabIndex = 28;
            this.cbEdgeDetect.Text = "Edge Detect";
            this.cbEdgeDetect.UseVisualStyleBackColor = true;
            this.cbEdgeDetect.CheckedChanged += new System.EventHandler(this.cbEdgeDetect_CheckedChanged);
            // 
            // cbInvert
            // 
            this.cbInvert.AutoSize = true;
            this.cbInvert.Location = new System.Drawing.Point(3, 122);
            this.cbInvert.Name = "cbInvert";
            this.cbInvert.Size = new System.Drawing.Size(53, 17);
            this.cbInvert.TabIndex = 25;
            this.cbInvert.Text = "Invert";
            this.cbInvert.UseVisualStyleBackColor = true;
            this.cbInvert.CheckedChanged += new System.EventHandler(this.cbInvert_CheckedChanged);
            // 
            // cbGrayscale
            // 
            this.cbGrayscale.AutoSize = true;
            this.cbGrayscale.Location = new System.Drawing.Point(3, 107);
            this.cbGrayscale.Name = "cbGrayscale";
            this.cbGrayscale.Size = new System.Drawing.Size(73, 17);
            this.cbGrayscale.TabIndex = 22;
            this.cbGrayscale.Text = "Grayscale";
            this.cbGrayscale.UseVisualStyleBackColor = true;
            this.cbGrayscale.CheckedChanged += new System.EventHandler(this.cbGrayscale_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 21;
            this.label2.Text = "Grid Interval (px)";
            // 
            // nGridInterval
            // 
            this.nGridInterval.Location = new System.Drawing.Point(170, 81);
            this.nGridInterval.Maximum = new decimal(new int[] {
            720,
            0,
            0,
            0});
            this.nGridInterval.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nGridInterval.Name = "nGridInterval";
            this.nGridInterval.Size = new System.Drawing.Size(60, 20);
            this.nGridInterval.TabIndex = 20;
            this.nGridInterval.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nGridInterval.ValueChanged += new System.EventHandler(this.nGridInterval_ValueChanged);
            // 
            // cbDrawGrid1
            // 
            this.cbDrawGrid1.AutoSize = true;
            this.cbDrawGrid1.Location = new System.Drawing.Point(3, 66);
            this.cbDrawGrid1.Name = "cbDrawGrid1";
            this.cbDrawGrid1.Size = new System.Drawing.Size(73, 17);
            this.cbDrawGrid1.TabIndex = 19;
            this.cbDrawGrid1.Text = "Draw Grid";
            this.cbDrawGrid1.UseVisualStyleBackColor = true;
            this.cbDrawGrid1.CheckedChanged += new System.EventHandler(this.cbDrawGrid1_CheckedChanged);
            // 
            // cbMirror1
            // 
            this.cbMirror1.AutoSize = true;
            this.cbMirror1.Location = new System.Drawing.Point(3, 39);
            this.cbMirror1.Name = "cbMirror1";
            this.cbMirror1.Size = new System.Drawing.Size(52, 17);
            this.cbMirror1.TabIndex = 17;
            this.cbMirror1.Text = "Mirror";
            this.cbMirror1.UseVisualStyleBackColor = true;
            this.cbMirror1.CheckedChanged += new System.EventHandler(this.cbMirror1_CheckedChanged);
            // 
            // cbRotate
            // 
            this.cbRotate.AutoSize = true;
            this.cbRotate.Location = new System.Drawing.Point(3, 3);
            this.cbRotate.Name = "cbRotate";
            this.cbRotate.Size = new System.Drawing.Size(71, 17);
            this.cbRotate.TabIndex = 15;
            this.cbRotate.Text = "rotate180";
            this.cbRotate.UseVisualStyleBackColor = true;
            this.cbRotate.CheckedChanged += new System.EventHandler(this.cbRotate_CheckedChanged);
            // 
            // bSetSaveLocation
            // 
            this.bSetSaveLocation.Location = new System.Drawing.Point(6, 385);
            this.bSetSaveLocation.Name = "bSetSaveLocation";
            this.bSetSaveLocation.Size = new System.Drawing.Size(97, 23);
            this.bSetSaveLocation.TabIndex = 47;
            this.bSetSaveLocation.Text = "Save Location...";
            this.bSetSaveLocation.UseVisualStyleBackColor = true;
            this.bSetSaveLocation.Click += new System.EventHandler(this.bSetSaveLocation_Click);
            // 
            // lblSaveFileLocation
            // 
            this.lblSaveFileLocation.AutoSize = true;
            this.lblSaveFileLocation.Location = new System.Drawing.Point(109, 390);
            this.lblSaveFileLocation.Name = "lblSaveFileLocation";
            this.lblSaveFileLocation.Size = new System.Drawing.Size(119, 13);
            this.lblSaveFileLocation.TabIndex = 48;
            this.lblSaveFileLocation.Text = "Save Location to to Set";
            // 
            // CameraAdjustments
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "CameraAdjustments";
            this.Size = new System.Drawing.Size(234, 423);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nZoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nThreashold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nGridInterval)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown nZoom;
        private System.Windows.Forms.CheckBox cbZoom;
        private System.Windows.Forms.CheckBox cbContrast;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown nThreashold;
        private System.Windows.Forms.CheckBox cbThreashold;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox cbNoiseReduction;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox cbEdgeDetect;
        private System.Windows.Forms.CheckBox cbInvert;
        private System.Windows.Forms.CheckBox cbGrayscale;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nGridInterval;
        private System.Windows.Forms.CheckBox cbDrawGrid1;
        private System.Windows.Forms.CheckBox cbMirror1;
        private System.Windows.Forms.CheckBox cbRotate;
        private System.Windows.Forms.ComboBox cbEdgeDetection;
        private System.Windows.Forms.ComboBox cbNoiseReduce;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbRotationAmount;
        private System.Windows.Forms.Label lblSaveFileLocation;
        private System.Windows.Forms.Button bSetSaveLocation;
    }
}
