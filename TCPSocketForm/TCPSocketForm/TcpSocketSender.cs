using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace TCPSocketForm {
    public class AppSettings<T> where T : new() {
        private const string DEFAULT_FILENAME = "TcpSettings.json";

        public void Save(string fileName = DEFAULT_FILENAME) {
            File.WriteAllText(fileName, (new JavaScriptSerializer()).Serialize(this));
        }

        public static void Save(T pSettings, string fileName = DEFAULT_FILENAME) {
            File.WriteAllText(fileName, (new JavaScriptSerializer()).Serialize(pSettings));
        }

        public static T Load(string fileName = DEFAULT_FILENAME) {
            T t = new T();
            if (File.Exists(fileName))
                t = (new JavaScriptSerializer()).Deserialize<T>(File.ReadAllText(fileName));
            return t;
        }
    }

    public class TcpSocketSender {
        public static MySettings Settings;

        public ConcurrentQueue<string> incomingMessages = new ConcurrentQueue<string>();

        static TcpSocketSender() => Settings = MySettings.Load();

        public class MySettings : AppSettings<MySettings> {
            public class TcpSocketSettings {
                public string SaveLocation = "";
                public ushort DestPort = 50000;
                public string DestIpAddr = "1";
                public string ReceivingIpAddr = "1";
                public ushort ReceivingPort = 50001;
            }

            public TcpSocketSettings TcpSettings = new TcpSocketSettings();
        }

        public static async Task<string> SendRequest(string server, int port, string method, string data) {
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
    }
}
