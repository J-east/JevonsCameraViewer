using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web.Script.Serialization;
using System.IO;

namespace CameraViewer {
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
            Application.Run(new MainForm());
            Environment.Exit(0);
        }

        public class MySettings : AppSettings<MySettings> {
            public class CameraSettings {
                public string SaveLocation = "";
                public string CamMoniker = "Hello World";
                public bool CamRotate = false;
                public int CamRotateValue = 0;
                public bool CamMirror = false;
                public bool DrawGrid = false;
                public int GridSpacing = 20;
                public int CamIndex = 1;
                public bool grayScale = false;
                public bool Invert = false;
                public bool EdgeDetect = false;
                public int EdgeDetectVal = 1;
                public bool NoiseReduction = false;
                public int NoiseReductionVal = 1;
                public bool Threshold = false;
                public int ThresholdVal = 1;
                public bool Zoom = false;
                public int ZoomVal = 1;
                public bool Contrast = false;
                public int ContrastVal = 1;
            }

            public CameraSettings Cam1 = new CameraSettings();
            public CameraSettings Cam2 = new CameraSettings();
        }
    }

    public class AppSettings<T> where T : new() {
        private const string DEFAULT_FILENAME = "settings.json";

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
