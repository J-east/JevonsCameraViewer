namespace TCPSocketForm {
    partial class TcpSetupForm {
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
            this.tbDestIpAddr = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblLocalIP = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbDestPort = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbReceivingPort = new System.Windows.Forms.TextBox();
            this.bSendTest = new System.Windows.Forms.Button();
            this.bCancel = new System.Windows.Forms.Button();
            this.bSave = new System.Windows.Forms.Button();
            this.bRequestTest = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.tbReceivingIpAddr = new System.Windows.Forms.TextBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // tbDestIpAddr
            // 
            this.tbDestIpAddr.Location = new System.Drawing.Point(132, 12);
            this.tbDestIpAddr.Name = "tbDestIpAddr";
            this.tbDestIpAddr.Size = new System.Drawing.Size(131, 20);
            this.tbDestIpAddr.TabIndex = 0;
            this.tbDestIpAddr.Text = "1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Destination IP Address";
            // 
            // lblLocalIP
            // 
            this.lblLocalIP.AutoSize = true;
            this.lblLocalIP.Location = new System.Drawing.Point(273, 15);
            this.lblLocalIP.Name = "lblLocalIP";
            this.lblLocalIP.Size = new System.Drawing.Size(98, 13);
            this.lblLocalIP.TabIndex = 2;
            this.lblLocalIP.Text = "Local IP address = ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 38);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Destination Port:";
            // 
            // tbDestPort
            // 
            this.tbDestPort.Location = new System.Drawing.Point(132, 34);
            this.tbDestPort.Name = "tbDestPort";
            this.tbDestPort.Size = new System.Drawing.Size(131, 20);
            this.tbDestPort.TabIndex = 4;
            this.tbDestPort.Text = "50000";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 88);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Recieving Port:";
            // 
            // tbReceivingPort
            // 
            this.tbReceivingPort.Location = new System.Drawing.Point(132, 85);
            this.tbReceivingPort.Name = "tbReceivingPort";
            this.tbReceivingPort.Size = new System.Drawing.Size(131, 20);
            this.tbReceivingPort.TabIndex = 6;
            this.tbReceivingPort.Text = "50001";
            // 
            // bSendTest
            // 
            this.bSendTest.Location = new System.Drawing.Point(16, 113);
            this.bSendTest.Name = "bSendTest";
            this.bSendTest.Size = new System.Drawing.Size(75, 23);
            this.bSendTest.TabIndex = 7;
            this.bSendTest.Text = "Send Test";
            this.bSendTest.UseVisualStyleBackColor = true;
            this.bSendTest.Click += new System.EventHandler(this.bSendTest_Click);
            // 
            // bCancel
            // 
            this.bCancel.Location = new System.Drawing.Point(344, 113);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(75, 23);
            this.bCancel.TabIndex = 8;
            this.bCancel.Text = "Cancel";
            this.bCancel.UseVisualStyleBackColor = true;
            this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
            // 
            // bSave
            // 
            this.bSave.Location = new System.Drawing.Point(425, 113);
            this.bSave.Name = "bSave";
            this.bSave.Size = new System.Drawing.Size(75, 23);
            this.bSave.TabIndex = 9;
            this.bSave.Text = "Save";
            this.bSave.UseVisualStyleBackColor = true;
            this.bSave.Click += new System.EventHandler(this.bSave_Click);
            // 
            // bRequestTest
            // 
            this.bRequestTest.Location = new System.Drawing.Point(97, 113);
            this.bRequestTest.Name = "bRequestTest";
            this.bRequestTest.Size = new System.Drawing.Size(99, 23);
            this.bRequestTest.TabIndex = 10;
            this.bRequestTest.Text = "Request Test";
            this.bRequestTest.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 66);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(109, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Receiving IP Address";
            // 
            // tbReceivingIpAddr
            // 
            this.tbReceivingIpAddr.Location = new System.Drawing.Point(132, 63);
            this.tbReceivingIpAddr.Name = "tbReceivingIpAddr";
            this.tbReceivingIpAddr.Size = new System.Drawing.Size(131, 20);
            this.tbReceivingIpAddr.TabIndex = 11;
            this.tbReceivingIpAddr.Text = "1";
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(276, 49);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(224, 56);
            this.listBox1.TabIndex = 13;
            // 
            // TcpSetupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(512, 144);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tbReceivingIpAddr);
            this.Controls.Add(this.bRequestTest);
            this.Controls.Add(this.bSave);
            this.Controls.Add(this.bCancel);
            this.Controls.Add(this.bSendTest);
            this.Controls.Add(this.tbReceivingPort);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbDestPort);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblLocalIP);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbDestIpAddr);
            this.MaximumSize = new System.Drawing.Size(528, 183);
            this.MinimumSize = new System.Drawing.Size(528, 183);
            this.Name = "TcpSetupForm";
            this.Text = "TCP Setup";
            this.Load += new System.EventHandler(this.TcpSetupForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbDestIpAddr;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblLocalIP;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbDestPort;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbReceivingPort;
        private System.Windows.Forms.Button bSendTest;
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.Button bSave;
        private System.Windows.Forms.Button bRequestTest;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbReceivingIpAddr;
        private System.Windows.Forms.ListBox listBox1;
    }
}

