using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace TCPSocketForm {
    public class TcpReceiver {
        public class AsyncService {
            private IPAddress ipAddress;
            private int port;

            public ConcurrentQueue<string> incomingMessages = new ConcurrentQueue<string>();

            public AsyncService(int port) {
                // set up port and determine IP Address
                this.port = port;
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
            } 
            
            public async void Run() {
                TcpListener listener = new TcpListener(this.ipAddress, this.port);
                listener.Start();
                Console.WriteLine("\nArray Min and Avg service is now running on port " + this.port);
                Console.WriteLine("Hit <enter> to stop service\n");

                while (true) {
                    try {
                        //Console.WriteLine("Waiting for a request ..."); 
                        TcpClient tcpClient = await listener.AcceptTcpClientAsync();
                        Task t = Process(tcpClient);
                        await t; // could combine with above
                    }
                    catch (Exception ex) {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            private async Task Process(TcpClient tcpClient) {
                string clientEndPoint = tcpClient.Client.RemoteEndPoint.ToString();
                //Console.WriteLine("Received connection request from " + clientEndPoint);
                Console.WriteLine("Received connection request from 123.45.678.999");
                try {
                    NetworkStream networkStream = tcpClient.GetStream();
                    StreamReader reader = new StreamReader(networkStream);
                    StreamWriter writer = new StreamWriter(networkStream);
                    writer.AutoFlush = true;
                    while (true) {
                        string request = await reader.ReadLineAsync();
                        if (request != null) {
                            Console.WriteLine("Received service request: " + request);
                            string response = Response(request);
                            Console.WriteLine("Computed response is: " + response + "\n");
                            await writer.WriteLineAsync(response);
                        }
                        else
                            break; // client closede connection
                    }
                    tcpClient.Close();
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                    if (tcpClient.Connected)
                        tcpClient.Close();
                }
            }

            private static string Response(string request) {
                string[] pairs = request.Split('&');
                string methodName = pairs[0].Split('=')[1];
                string valueString = pairs[1].Split('=')[1];

                string[] values = valueString.Split(' ');
                double[] vals = new double[values.Length];
                for (int i = 0; i < values.Length; ++i)
                    vals[i] = double.Parse(values[i]);

                // handle response here

                string response = "";

                return response;
            }
        }
    }
}
