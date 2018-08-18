namespace MachineCommunications {
    partial class SerialCommSetupPanel {
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
            this.lblComPortStatus = new System.Windows.Forms.Label();
            this.bRefresh = new System.Windows.Forms.Button();
            this.cbCOM = new System.Windows.Forms.ComboBox();
            this.bConnectToDevice = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblComPortStatus
            // 
            this.lblComPortStatus.AutoSize = true;
            this.lblComPortStatus.Location = new System.Drawing.Point(1, 27);
            this.lblComPortStatus.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblComPortStatus.Name = "lblComPortStatus";
            this.lblComPortStatus.Size = new System.Drawing.Size(86, 13);
            this.lblComPortStatus.TabIndex = 13;
            this.lblComPortStatus.Text = "Select COM Port";
            // 
            // bRefresh
            // 
            this.bRefresh.Location = new System.Drawing.Point(142, 2);
            this.bRefresh.Margin = new System.Windows.Forms.Padding(2);
            this.bRefresh.Name = "bRefresh";
            this.bRefresh.Size = new System.Drawing.Size(90, 20);
            this.bRefresh.TabIndex = 12;
            this.bRefresh.Text = "Refresh COM";
            this.bRefresh.UseVisualStyleBackColor = true;
            // 
            // cbCOM
            // 
            this.cbCOM.FormattingEnabled = true;
            this.cbCOM.Location = new System.Drawing.Point(2, 2);
            this.cbCOM.Margin = new System.Windows.Forms.Padding(2);
            this.cbCOM.Name = "cbCOM";
            this.cbCOM.Size = new System.Drawing.Size(137, 21);
            this.cbCOM.TabIndex = 11;
            // 
            // bConnectToDevice
            // 
            this.bConnectToDevice.Location = new System.Drawing.Point(236, 2);
            this.bConnectToDevice.Margin = new System.Windows.Forms.Padding(2);
            this.bConnectToDevice.Name = "bConnectToDevice";
            this.bConnectToDevice.Size = new System.Drawing.Size(87, 19);
            this.bConnectToDevice.TabIndex = 10;
            this.bConnectToDevice.Text = "Connect";
            this.bConnectToDevice.UseVisualStyleBackColor = true;
            // 
            // SerialCommSetupPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblComPortStatus);
            this.Controls.Add(this.bRefresh);
            this.Controls.Add(this.cbCOM);
            this.Controls.Add(this.bConnectToDevice);
            this.Name = "SerialCommSetupPanel";
            this.Size = new System.Drawing.Size(330, 46);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblComPortStatus;
        private System.Windows.Forms.Button bRefresh;
        private System.Windows.Forms.ComboBox cbCOM;
        private System.Windows.Forms.Button bConnectToDevice;
    }
}
