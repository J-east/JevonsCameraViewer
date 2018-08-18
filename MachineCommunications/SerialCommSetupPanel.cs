using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace MachineCommunications {
    public partial class SerialCommSetupPanel : UserControl {
        public SerialCommSetupPanel() {
            InitializeComponent();
        }

        private void bRefresh_Click(object sender, EventArgs e) {
            foreach (string s in SerialPort.GetPortNames()) {
                cbCOM.Items.Add(s);
            }

            try {
                cbCOM.SelectedIndex = 0;
            }
            catch { }

            if (cbCOM.Items.Count == 0) {
                lblComPortStatus.Text = "No serial ports found. Is device powered on?";
            }
        }

        private void bConnectToDevice_Click(object sender, EventArgs e) {
            if (cbCOM.SelectedItem == null) {
                return;  // no ports
            }

            if (!Loader.Connected) {
                if (Loader.CNC_SerialPort != cbCOM.SelectedItem.ToString()) {
                    // user changed the selection
                    bConnectToDevice.Text = "Closing..";
                    Loader.Close(this);
                    // 0.5 s delay for the system to clear buffers etc
                    for (int i = 0; i < 250; i++) {
                        Thread.Sleep(2);
                        Application.DoEvents();
                    }
                }
                // reconnect, attempt to clear the error
                if (Loader.Connect(cbCOM.SelectedItem.ToString(), this)) {
                    bConnectToDevice.Text = "Connecting..";
                    Loader.ErrorState = false;
                    Loader.CNC_SerialPort = cbCOM.SelectedItem.ToString();

                    UpdateCncConnectionStatus(true);
                }
            }
            else {
                // Close connection
                bConnectToDevice.Text = "Closing..";
                Loader.Close(this);
                for (int i = 0; i < 250; i++) {
                    Thread.Sleep(2);
                    Application.DoEvents();
                }
                UpdateCncConnectionStatus(false);
            }
        }

        public void UpdateCncConnectionStatus(bool v) {
            if (v) {
                lblComPortStatus.Text = "connection successful";
                bConnectToDevice.Text = "Disconnect";
            }
            else {
                lblComPortStatus.Text = "not working";
            }
        }
    }
}
