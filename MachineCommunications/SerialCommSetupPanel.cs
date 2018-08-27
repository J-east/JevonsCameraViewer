using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO.Ports;
using static FileLogger.FileLogger;
using System.Threading.Tasks;

namespace MachineCommunications {
    public partial class SerialCommSetupPanel : UserControl {
        bool Connected { get; set; }
        string CNC_SerialPort { get; set; }
        bool ErrorState { get; set; }        
        public bool JoggingBusy { get; private set; }
        public bool CNC_WriteOk { get; private set; }

        public SerialComm serialComm;            
        private CNC cnc;

        public SerialCommSetupPanel() {
            InitializeComponent();
        }

        public void FinalizeSetup(CNC cnc) {
            this.cnc = cnc;
            serialComm = new SerialComm(cnc, this);
        }

        Thread cancellationThread = null;
        public void SendSerialCommand(string command, int Timeout = 250) {
            if (cnc.ErrorState) {
                AppendToLog("### " + command + " ignored, cnc is in error state");
                throw new Exception("borked");
            }

            serialComm.SerialPortWriteDone = false;
            Task t = new Task(() => { cancellationThread = Thread.CurrentThread; serialComm.Write(command); });
            t.Start();
            int i = 0;
            if (cnc.IsHoming) {
                Timeout = 20000;    // give it 20 seconds
            }
            while (!serialComm.SerialPortWriteDone) {
                Thread.Sleep(2);
                Application.DoEvents();
                i++;
                if (i > Timeout) {
                    cancellationThread.Abort();
                    serialComm.SerialPortWriteDone = true;
                    JoggingBusy = false;
                    cnc.SetErrorState();
                    throw new Exception("borked");
                }
            }            
        }

        bool Connect(String name) {
            if (serialComm.IsOpen) {
                DisplayText("Connecting to serial port " + name + ": already open");
                Connected = true;
                return true;
            }
            serialComm.Open(name);
            ErrorState = false;

            Connected = serialComm.IsOpen;           

            return Connected;
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

        public void DisplayText(string txt) {
            try {
                if (InvokeRequired) { Invoke(new Action<string>(DisplayText), new[] { txt }); return; }
                txt = txt.Replace("\n", "");
                // TinyG sends \n, textbox needs \r\n. (TinyG could be set to send \n\r, which does not work with textbox.)
                // Adding end of line here saves typing elsewhere
                txt = txt + "\r\n";
                if (rbMessages.Text.Length > 1000000) {
                    rbMessages.Text = rbMessages.Text.Substring(rbMessages.Text.Length - 10000);
                }
                rbMessages.AppendText(txt);
                rbMessages.ScrollToCaret();
            }
            catch {
            }
        }

        private void bConnectToDevice_Click(object sender, EventArgs e) {
            if (cbCOM.SelectedItem == null) {
                return;  // no ports
            }

            if (!this.Connected) {
                if (this.CNC_SerialPort != cbCOM.SelectedItem.ToString()) {
                    // user changed the selection
                    bConnectToDevice.Text = "Closing..";
                    this.Close();
                    // 0.5 s delay for the system to clear buffers etc
                    for (int i = 0; i < 250; i++) {
                        Thread.Sleep(2);
                        Application.DoEvents();
                    }
                }
                // reconnect, attempt to clear the error
                if (Connect(cbCOM.SelectedItem.ToString())) {
                    bConnectToDevice.Text = "Connecting..";
                    this.ErrorState = false;
                    this.CNC_SerialPort = cbCOM.SelectedItem.ToString();

                    UpdateCncConnectionStatus(true);

                    cnc.RequestCurrentTinyGConfig();
                }
            }
            else {
                // Close connection
                bConnectToDevice.Text = "Closing..";
                this.Close();
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

        public void Close() {
            serialComm.Close();
            UpdateCncConnectionStatus(false);
        }
    }
}

