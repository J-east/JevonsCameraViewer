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
            tbDestPort.Text = Program.Settings.TcpSettings.DestPort.ToString();
            tbDestIpAddr.Text = Program.Settings.TcpSettings.DestIpAddr.ToString();
            tbReceivingIpAddr.Text = Program.Settings.TcpSettings.ReceivingIpAddr.ToString();
            tbReceivingPort.Text = Program.Settings.TcpSettings.ReceivingPort.ToString();

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

        private static async Task<string> SendRequest(string server, int port, string method, string data) {
            try {
                // set up IP address of server
                IPAddress ipAddress = null;
                IPHostEntry ipHostInfo = Dns.GetHostEntry(server);
                for (int i = 0; i < ipHostInfo.AddressList.Length; ++i) {
                    if (ipHostInfo.AddressList[i].AddressFamily == AddressFamily.InterNetwork) {
                        ipAddress = ipHostInfo.AddressList[i];
                        break;
                    }
                }
                if (ipAddress == null)
                    throw new Exception("Unable to find an IPv4 address for server");

                TcpClient client = new TcpClient();
                await client.ConnectAsync(ipAddress, port); // connect to the server

                NetworkStream networkStream = client.GetStream();
                StreamWriter writer = new StreamWriter(networkStream);
                StreamReader reader = new StreamReader(networkStream);

                writer.AutoFlush = true;
                string requestData = "method=" + method + "&" + "data=" + data + "&eor"; // 'end-of-requet'
                await writer.WriteLineAsync(requestData);
                string response = await reader.ReadLineAsync();

                client.Close();

                return response;

            }
            catch (Exception ex) {
                return ex.Message;
            }
        } // SendRequest

        private async void bSendTest_Click(object sender, EventArgs e) {
            try {
                string server = tbDestIpAddr.Text;
                int port = 50000;
                string method = "average";
                string data = "3 2 1 1";

                Task<string> tsResponse = SendRequest(server, port, method, data);
                listBox1.Items.Add("Sent request, waiting for response");
                await tsResponse;
                double dResponse = double.Parse(tsResponse.Result);
                listBox1.Items.Add("Received response: " + dResponse.ToString("F2"));
            }
            catch (Exception ex) {
                listBox1.Items.Add(ex.Message);
            }
        }
    }
}
