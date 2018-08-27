using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static FileLogger.FileLogger;


namespace MachineCommunications
{
    public class SerialComm {

        SerialPort Port = new SerialPort();
        public bool SerialPortWriteDone { get; set; }

        // To process data on the DataReceived thread, get reference of Cnc, so we can pass data to it.
        private CNC Cnc;
        // To show what we send, we need a reference to mainform.
        private static SerialCommSetupPanel MainForm;

        public SerialComm(CNC caller, SerialCommSetupPanel MainF) {
            Cnc = caller;
            Port.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
            MainForm = MainF;
        }

        public bool IsOpen {
            get {
                return Port.IsOpen;
            }
        }

        private void Close_thread() {
            Port.Close();
        }

        public void Close() {
            try {
                if (Port.IsOpen) {
                    Port.DiscardInBuffer();
                    Port.DiscardOutBuffer();
                }
                // Known issue: Sometimes serial port hangs in app closing. Google says that 
                // the workaround is to close in another thread
                Thread t = new Thread(() => Close_thread());
                t.Start();
            }
            catch {
            }
        }

        public bool Open(string Com) {
            Close();
            try {
                Port.PortName = Com;
                Port.BaudRate = 115200;
                Port.Parity = Parity.None;
                Port.StopBits = StopBits.One;
                Port.DataBits = 8;
                Port.Handshake = Handshake.RequestToSend;
                Port.DtrEnable = true;  // prevent hangs on some drivers
                Port.RtsEnable = true;
                Port.WriteTimeout = 500;
                Port.Open();
                if (Port.IsOpen) {
                    Port.DiscardOutBuffer();
                    Port.DiscardInBuffer();
                }
                // Port.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
                return Port.IsOpen;
            }
            catch {
                return false;
            }
        }

        public bool Write(string TxText) {
            try {
                if (Port.IsOpen) {
                    Port.Write(TxText + "\r");
                    AppendToLog("==> " + TxText);
                    SerialPortWriteDone = true;
                }
                else {
                    AppendToLog("Serial port not open, write discarded: " + TxText);
                }
                return true;
            }
            catch {
                AppendToLog("Serial port write failed.");
                Close();
                return false;
            }
        }


        const int ReadBufferSize = 10000;
        private string RxString = string.Empty;

        void DataReceived(object sender, SerialDataReceivedEventArgs e) {
            //Initialize a buffer to hold the received data 
            byte[] buffer = new byte[ReadBufferSize];
            string WorkingString;

            try {
                //There is no accurate method for checking how many bytes are read 
                //unless you check the return from the Read method 
                int bytesRead = Port.Read(buffer, 0, buffer.Length);

                //The received data is ASCII
                RxString += Encoding.ASCII.GetString(buffer, 0, bytesRead);
                //Process each line
                while (RxString.IndexOf("\n") > -1) {
                    //Even when RxString does contain terminator we cannot assume that it is the last character received 
                    WorkingString = RxString.Substring(0, RxString.IndexOf("\n") + 1);
                    //Remove the data and the terminator from tString 
                    RxString = RxString.Substring(RxString.IndexOf("\n") + 1);
                    Cnc.InterpretLine(WorkingString);
                }
            }
            catch (Exception ex) {
                AppendToLog("########## " + ex);
            }
        }

    }
}
