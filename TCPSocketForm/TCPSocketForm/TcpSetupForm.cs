using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.IO;
using System.Net;

namespace TCPSocketForm {
    public partial class TcpSetupForm : Form {
        private int port;
        private IPAddress ipAddress;

        public TcpSetupForm() {
            InitializeComponent();
        }

        private void TcpSetupForm_Load(object sender, EventArgs e) {
            tbDestPort.Text = TcpSocketSender.Settings.TcpSettings.DestPort.ToString();
            tbDestIpAddr.Text = TcpSocketSender.Settings.TcpSettings.DestIpAddr.ToString();
            tbReceivingIpAddr.Text = TcpSocketSender.Settings.TcpSettings.ReceivingIpAddr.ToString();
            tbReceivingPort.Text = TcpSocketSender.Settings.TcpSettings.ReceivingPort.ToString();

            this.port = int.Parse(tbDestPort.Text);
            string hostName = Dns.GetHostName();
            IPHostEntry ipHostInfo = Dns.GetHostEntry(hostName);

            this.ipAddress = null;
            for (int i = 0; i < ipHostInfo.AddressList.Length; ++i) {
                if (ipHostInfo.AddressList[i].AddressFamily == AddressFamily.InterNetwork) {
                    this.ipAddress = ipHostInfo.AddressList[i];
                    break;
                }
            }

            if (this.ipAddress == null)
                throw new Exception("No IPv4 address for server");

            if (tbDestIpAddr.Text == "1") {
                tbDestIpAddr.Text = this.ipAddress.ToString();
                lblLocalIP.Text = $"Local IP address = {this.ipAddress.ToString()}";
            }                

            if (tbReceivingIpAddr.Text == "1")
                tbReceivingIpAddr.Text = this.ipAddress.ToString();
        }

        private async void bSendTest_Click(object sender, EventArgs e) {
            try {
                string server = tbDestIpAddr.Text;
                int port = 50000;
                string method = "average";
                string data = "3 2 1 1";

                Task<string> tsResponse = TcpSocketSender.SendRequest(server, port, method, data);
                listBox1.Items.Add("Sent request, waiting for response");
                await tsResponse;
                double dResponse = double.Parse(tsResponse.Result);
                listBox1.Items.Add("Received response: " + dResponse.ToString("F2"));
            }
            catch (Exception ex) {
                listBox1.Items.Add(ex.Message);
            }
        }

        private void bCancel_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void bSave_Click(object sender, EventArgs e) {
            TcpSocketSender.Settings.TcpSettings.DestPort = ushort.Parse(tbDestPort.Text);
            TcpSocketSender.Settings.TcpSettings.DestIpAddr = tbDestIpAddr.Text;
            TcpSocketSender.Settings.TcpSettings.ReceivingIpAddr = tbReceivingIpAddr.Text;
            TcpSocketSender.Settings.TcpSettings.ReceivingPort = ushort.Parse(tbReceivingPort.Text);

            this.Close();
        }
    }
}
