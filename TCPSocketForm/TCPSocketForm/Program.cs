using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web.Script.Serialization;

namespace TCPSocketForm {
    static class Program {
        public static MySettings Settings;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Settings = MySettings.Load();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TcpSetupForm());
        }

        public class MySettings : AppSettings<MySettings> {
            public class TcpSocketSettings {
                public string SaveLocation = "";
                public ushort DestPort= 50000;
                public string DestIpAddr = "1";
                public string ReceivingIpAddr = "1";
                public ushort ReceivingPort = 50001;
            }

            public TcpSocketSettings TcpSettings = new TcpSocketSettings();
        }
    }

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
}
