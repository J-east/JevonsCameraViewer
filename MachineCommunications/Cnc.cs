using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static FileLogger.FileLogger;
using System.Web.Script.Serialization;

namespace MachineCommunications {
    public class CNC {
        static CncSetupControl setUpControl;
        static SerialCommSetupPanel serialSetup;
        private SerialComm Com;

        static ManualResetEventSlim _CncThreadResetToken = new ManualResetEventSlim(false);

        public CNC(CncSetupControl cncSetup, SerialCommSetupPanel serialCommSetup) {
            setUpControl = cncSetup;
            serialSetup = serialCommSetup;
            Com = new SerialComm(this, serialCommSetup);
            SlowXY = false;
            SlowZ = false;
        }

        public ManualResetEventSlim CncThreadResetToken {
            get {
                return _CncThreadResetToken;
            }
        }

        public bool Connected { get; set; }
        public bool ErrorState { get; set; }

        public void SetErrorState() {
            ErrorState = true;

            IsHoming = false;
            _CncThreadResetToken.Set();
            serialSetup.UpdateCncConnectionStatus(false);
        }

        public void RequestCurrentTinyGConfig() {
            try {
                serialSetup.SendSerialCommand(@"{""sr"":""""}");

                serialSetup.SendSerialCommand(@"{""xjm"":""""}");

                serialSetup.SendSerialCommand(@"{""xvm"":""""}");

                serialSetup.SendSerialCommand(@"{""xsv"":""""}");

                serialSetup.SendSerialCommand(@"{""xlv"":""""}");

                serialSetup.SendSerialCommand(@"{""xlb"":""""}");

                serialSetup.SendSerialCommand(@"{""xsn"":""""}");

                serialSetup.SendSerialCommand(@"{""xjh"":""""}");

                serialSetup.SendSerialCommand(@"{""xsx"":""""}");

                serialSetup.SendSerialCommand(@"{""1mi"":""""}");

                serialSetup.SendSerialCommand(@"{""1sa"":""""}");

                serialSetup.SendSerialCommand(@"{""1tr"":""""}");

                serialSetup.SendSerialCommand(@"{""yjm"":""""}");

                serialSetup.SendSerialCommand(@"{""yvm"":""""}");

                serialSetup.SendSerialCommand(@"{""ysn"":""""}");

                serialSetup.SendSerialCommand(@"{""ysx"":""""}");

                serialSetup.SendSerialCommand(@"{""yjh"":""""}");

                serialSetup.SendSerialCommand(@"{""ysv"":""""}");

                serialSetup.SendSerialCommand(@"{""ylv"":""""}");

                serialSetup.SendSerialCommand(@"{""ylb"":""""}");

                serialSetup.SendSerialCommand(@"{""2mi"":""""}");

                serialSetup.SendSerialCommand(@"{""2sa"":""""}");

                serialSetup.SendSerialCommand(@"{""2tr"":""""}");

                serialSetup.SendSerialCommand(@"{""zjm"":""""}");

                serialSetup.SendSerialCommand(@"{""zvm"":""""}");

                serialSetup.SendSerialCommand(@"{""zsn"":""""}");

                serialSetup.SendSerialCommand(@"{""zsx"":""""}");

                serialSetup.SendSerialCommand(@"{""zjh"":""""}");

                serialSetup.SendSerialCommand(@"{""zsv"":""""}");

                serialSetup.SendSerialCommand(@"{""zlv"":""""}");

                serialSetup.SendSerialCommand(@"{""zlb"":""""}");

                serialSetup.SendSerialCommand(@"{""3mi"":""""}");

                serialSetup.SendSerialCommand(@"{""3sa"":""""}");

                serialSetup.SendSerialCommand(@"{""3tr"":""""}");

                serialSetup.SendSerialCommand(@"{""ajm"":""""}");

                serialSetup.SendSerialCommand(@"{""avm"":""""}");

                serialSetup.SendSerialCommand(@"{""4mi"":""""}");

                serialSetup.SendSerialCommand(@"{""4sa"":""""}");

                serialSetup.SendSerialCommand(@"{""4tr"":""""}");

                serialSetup.SendSerialCommand(@"{""mt"":""""}");
            }
            catch {
                throw new Exception();
            }
        }

        public void Close() {
            Com.Close();
            ErrorState = false;
            Connected = false;
            IsHoming = false;
            _CncThreadResetToken.Set();
            serialSetup.UpdateCncConnectionStatus(false);
        }

        public bool Connect(String name) {
            if (Com.IsOpen) {
                AppendToLog("Connecting to serial port " + name + ": already open");
                Connected = true;
                return true;
            }
            Com.Open(name);
            ErrorState = false;
            IsHoming = false;
            _CncThreadResetToken.Set();
            Connected = Com.IsOpen;
            if (!Connected) {
                SetErrorState();
            }
            AppendToLog("Connecting to serial port " + name + ", result:" + Com.IsOpen.ToString());

            return Connected;
        }

        public bool Write(string command) {
            if (!Com.IsOpen) {
                AppendToLog($"Command: {command} discarded, com not open");
                _CncThreadResetToken.Set();
                Connected = false;
                return false;
            }
            if (ErrorState) {
                AppendToLog($"Command: {command} discarded, CNC in Error State");
                _CncThreadResetToken.Set();
                return false;
            }
            _CncThreadResetToken.Reset();
            bool res = Com.Write(command);
            _CncThreadResetToken.Wait();
            if (!res) {
                SetErrorState();
            }
            return res;
        }

        public bool RawWrite(string command) {
            if (!Com.IsOpen) {
                AppendToLog("###" + command + " discarded, com not open");
                Connected = false;
                return false;
            }
            if (ErrorState) {
                AppendToLog("###" + command + " discarded, error state on");
                return false;
            }
            bool res = Com.Write(command);
            if (!res) {
                SetErrorState();
            }
            return res;
        }

        public void ForceWrite(string command) {
            Com.Write(command);
        }

        private static double CurrX;
        public double CurrentX {
            get {
                return (CurrX);
            }
            set {
                CurrX = value;
            }
        }

        private static double CurrY;
        public double CurrentY {
            get {
                return (CurrY);
            }
            set {
                CurrY = value;
            }
        }

        private static double CurrZ;
        public double CurrentZ {
            get {
                return (CurrZ);
            }
            set {
                CurrZ = value;
            }
        }

        public string SmallMovementString = "G1 F200 ";

        public bool SlowXY { get; set; }
        public double SlowSpeedXY { get; set; }

        public bool SlowZ { get; set; }
        public double SlowSpeedZ { get; set; }

        public void XY(double X, double Y) {
            double dX = Math.Abs(X - CurrentX);
            double dY = Math.Abs(Y - CurrentY);
            if ((dX < 0.004) && (dY < 0.004)) {
                AppendToLog(" -- zero XY movement command --");
                AppendToLog("ReadyEvent: zero movement command");
                _CncThreadResetToken.Set();
                return;   // already there
            }
            XY_move(X, Y);
        }

        private void XY_move(double X, double Y) {
            string command;
            double dX = Math.Abs(X - CurrentX);
            double dY = Math.Abs(Y - CurrentY);
            if ((dX < 0.004) && (dY < 0.004)) {
                AppendToLog(" -- zero XY movement command --");
                AppendToLog("ReadyEvent: zero movement command");
                _CncThreadResetToken.Set();
                return;   // already there
            }
            X = Math.Round(X, 3);
            if ((dX < 1) && (dY < 1)) {
                // Small move
                if (SlowXY) {
                    if ((double)TinyGSettings.CNC_SmallMovementSpeed > SlowSpeedXY) {
                        command = SmallMovementString + "X" + X.ToString(CultureInfo.InvariantCulture) +
                                                       " Y" + Y.ToString(CultureInfo.InvariantCulture);
                    }
                    else {
                        command = "G1 F" + SlowSpeedXY.ToString()
                                + " X" + X.ToString(CultureInfo.InvariantCulture) + " Y" + Y.ToString(CultureInfo.InvariantCulture);
                    }
                }
                else {
                    command = SmallMovementString + "X" + X.ToString(CultureInfo.InvariantCulture) +
                                                   " Y" + Y.ToString(CultureInfo.InvariantCulture);
                }
            }
            else {
                // large move
                if (SlowXY) {
                    command = "G1 F" + SlowSpeedXY.ToString()
                            + " X" + X.ToString(CultureInfo.InvariantCulture) + " Y" + Y.ToString(CultureInfo.InvariantCulture);
                }
                else {
                    command = "G0 " + "X" + X.ToString(CultureInfo.InvariantCulture) + " Y" + Y.ToString(CultureInfo.InvariantCulture);
                }
            }
            _CncThreadResetToken.Reset();
            Com.Write("{\"gc\":\"" + command + "\"}");
            _CncThreadResetToken.Wait();
        }

        public void Z(double Z) {
            // z calibrator (one thing that I never want to happen is for the z to crash into the board)
            // TODO: write this into the settings
            if (Z > 34) {
                MessageBox.Show("tape error, please adjust place height");
                return;
            }

            string command = "G0 Z" + Z.ToString(CultureInfo.InvariantCulture);
            double dZ = Math.Abs(Z - CurrentZ);
            if (dZ < 0.005) {
                AppendToLog(" -- zero Z movement command --");
                AppendToLog("ReadyEvent: zero movement command");
                _CncThreadResetToken.Set();
                return;   // already there
            }
            if (dZ < 1.1) {
                if (SlowZ) {
                    if ((double)TinyGSettings.CNC_SmallMovementSpeed > SlowSpeedZ) {
                        command = "G1 F" + SlowSpeedZ.ToString() + " Z" + Z.ToString(CultureInfo.InvariantCulture);
                    }
                    else {
                        command = "G1 F" + TinyGSettings.CNC_SmallMovementSpeed.ToString() + " Z" + Z.ToString(CultureInfo.InvariantCulture);
                    }
                }
            }
            else {
                if (SlowZ) {
                    command = "G1 F" + SlowSpeedZ.ToString() + " Z" + Z.ToString(CultureInfo.InvariantCulture);
                }
                else {
                    command = "G0 Z" + Z.ToString(CultureInfo.InvariantCulture);
                }
            }
            _CncThreadResetToken.Reset();
            Com.Write("{\"gc\":\"" + command + "\"}");
            _CncThreadResetToken.Wait();
        }

        public void SetMotorPower(bool turnOn) {
            if (turnOn) {
                serialSetup.SendSerialCommand("{\"me\":\"\"}");
            }
            else {
                serialSetup.SendSerialCommand("{\"md\":\"\"}");
            }
        }

        public void SetPumpPower(bool turnOn) {
            if (turnOn) {
                serialSetup.SendSerialCommand("{\"gc\":\"M03\"}");
            }
            else {
                serialSetup.SendSerialCommand("{\"gc\":\"M05\"}");
            }
        }

        public void SetVacuumSolenoid(bool enabled) {
            if (enabled) {
                serialSetup.SendSerialCommand("{\"gc\":\"M08\"}");
            }
            else {
                serialSetup.SendSerialCommand("{\"gc\":\"M09\"}");
            }
        }

        public bool IsHoming { get; set; }
        public bool IgnoreError { get; set; }

        public void InterpretLine(string line) {
            // This is called from SerialComm dataReceived, and runs in a separate thread than UI            
            AppendToLog(line);

            if (line.Contains("SYSTEM READY")) {
                SetErrorState();
                MessageBox.Show("TinyG Reset.", "System Reset", MessageBoxButtons.OK);
                serialSetup.UpdateCncConnectionStatus(false);
                return;
            }

            if (line.StartsWith("{\"r\":{\"msg")) {
                line = line.Substring(13);
                int i = line.IndexOf('"');
                line = line.Substring(0, i);
                MessageBox.Show("TinyG Message:", line, MessageBoxButtons.OK);
                return;
            }


            if (line.StartsWith("{\"er\":")) {
                if (line.Contains("File not open") && IgnoreError) {
                    AppendToLog("### Ignored file not open error ###");
                    return;
                }
                SetErrorState();
                MessageBox.Show(
                    "TinyG Error: Please Press Reset Button to Contiune",
                    "TinyG Error Received",
                    MessageBoxButtons.OK);
                return;
            }


            if (line.StartsWith("{\"r\":{}")) {
                // ack for g code command
                return;
            }

            if (line.StartsWith("tinyg [mm] ok>")) {
                // AppendToLog("ReadyEvent ok");
                _CncThreadResetToken.Set();
                return;
            }


            if (line.StartsWith("{\"sr\":")) {
                // Status report
                NewStatusReport(line);
                if (line.Contains("\"stat\":3")) {
                    AppendToLog("ReadyEvent stat");
                    _CncThreadResetToken.Set();
                }
                return;
            }

            if (line.StartsWith("{\"r\":{\"sr\"")) {
                // Status enquiry response, remove the wrapper:
                line = line.Substring(5);
                int i = line.IndexOf("}}");
                line = line.Substring(0, i + 2);
                NewStatusReport(line);
                _CncThreadResetToken.Set();
                AppendToLog("ReadyEvent r:sr");
                return;
            }

            if (line.StartsWith("{\"r\":{\"me") || line.StartsWith("{\"r\":{\"md")) {
                // response to motor power on/off commands
                _CncThreadResetToken.Set();
                return;
            }

            if (line.StartsWith("{\"r\":{\"sys\":")) {
                // response to reading settings for saving them
                // remove the wrapper:
                line = line.Substring(5);
                int i = line.IndexOf("}}");
                line = line.Substring(0, i + 2);
                TinyGSettings.TinyG_sys = line;
                _CncThreadResetToken.Set();
                AppendToLog("ReadyEvent sys group");
                return;
            }

            if (line.StartsWith("{\"r\":{\"x\":")) {
                // response to reading settings for saving them
                // remove the wrapper:
                line = line.Substring(5);
                int i = line.IndexOf("}}");
                line = line.Substring(0, i + 2);
                TinyGSettings.TinyG_x = line;
                _CncThreadResetToken.Set();
                AppendToLog("ReadyEvent x group");
                return;
            }

            if (line.StartsWith("{\"r\":{\"y\":")) {
                // response to reading settings for saving them
                // remove the wrapper:
                line = line.Substring(5);
                int i = line.IndexOf("}}");
                line = line.Substring(0, i + 2);
                TinyGSettings.TinyG_y = line;
                _CncThreadResetToken.Set();
                AppendToLog("ReadyEvent y group");
                return;
            }

            if (line.StartsWith("{\"r\":{\"z\":")) {
                // response to reading settings for saving them
                // remove the wrapper:
                line = line.Substring(5);
                int i = line.IndexOf("}}");
                line = line.Substring(0, i + 2);
                TinyGSettings.TinyG_z = line;
                _CncThreadResetToken.Set();
                AppendToLog("ReadyEvent z group");
                return;
            }

            if (line.StartsWith("{\"r\":{\"1\":")) {
                // response to reading settings for saving them
                // remove the wrapper:
                line = line.Substring(5);
                int i = line.IndexOf("}}");
                line = line.Substring(0, i + 2);
                TinyGSettings.TinyG_m1 = line;
                _CncThreadResetToken.Set();
                AppendToLog("ReadyEvent m1 group");
                return;
            }

            if (line.StartsWith("{\"r\":{\"2\":")) {
                // response to reading settings for saving them
                // remove the wrapper:
                line = line.Substring(5);
                int i = line.IndexOf("}}");
                line = line.Substring(0, i + 2);
                TinyGSettings.TinyG_m2 = line;
                _CncThreadResetToken.Set();
                AppendToLog("ReadyEvent m2 group");
                return;
            }

            if (line.StartsWith("{\"r\":{\"3\":")) {
                // response to reading settings for saving them
                // remove the wrapper:
                line = line.Substring(5);
                int i = line.IndexOf("}}");
                line = line.Substring(0, i + 2);
                TinyGSettings.TinyG_m3 = line;
                _CncThreadResetToken.Set();
                AppendToLog("ReadyEvent m3 group");
                return;
            }

            if (line.StartsWith("{\"r\":")) {
                // response to setting a setting or reading motor settings for saving them
                // Replace {"1 with {"motor1 so that the field name is valid
                line = line.Replace("{\"1", "{\"motor1");
                line = line.Replace("{\"2", "{\"motor2");
                line = line.Replace("{\"3", "{\"motor3");
                UpdateConfiguration(line);
                _CncThreadResetToken.Set();
                AppendToLog("ReadyEvent r");
                return;
            }

        }  // end InterpretLine()

        public CurrentMachineState cncState;
        public void NewStatusReport(string line) {
            //AppendToLog("NewStatusReport: " + line);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            cncState = serializer.Deserialize<CurrentMachineState>(line);
        }

        [Serializable]
        public class MachinePosition {
            private double _posx = 0;
            public double posx {
                get { return _posx; }
                set {
                    _posx = value;
                }
            }

            private double _posy = 0;
            public double posy {
                get { return _posy; }
                set {
                    _posy = value;
                    CurrY = _posy;
                }
            }

            private double _posz = 0;
            public double posz {
                get { return _posz; }
                set {
                    _posz = value;
                    CurrZ = _posz;
                }
            }
        }

        [Serializable]
        public class CurrentMachineState {
            public MachinePosition sr { get; set; }
        }

        public TinyGResponse Settings;
        public void UpdateConfiguration(string line) {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Settings = serializer.Deserialize<TinyGResponse>(line);
        }

        public class MachineConfiguration {
            // mt: motor timeout
            private string _mt = "";
            public string mt {
                get { return _mt; }
                set {
                    _mt = value;
                }
            }

            // *jm: jerk max
            private string _xjm = "";
            public string xjm {
                get { return _xjm; }
                set {
                    _xjm = value;
                }
            }

            private string _yjm = "";
            public string yjm {
                get { return _yjm; }
                set {
                    _yjm = value;
                }
            }

            private string _zjm = "";
            public string zjm {
                get { return _zjm; }
                set {
                    _zjm = value;
                }
            }

            private string _ajm = "";
            public string ajm {
                get { return _ajm; }
                set {
                    _ajm = value;
                }
            }

            // *vm: velocity max
            private string _xvm = "";
            public string xvm {
                get { return _xvm; }
                set {
                    _xvm = value;
                }
            }

            private string _yvm = "";
            public string yvm {
                get { return _yvm; }
                set {
                    _yvm = value;
                }
            }

            private string _zvm = "";
            public string zvm {
                get { return _zvm; }
                set {
                    _zvm = value;
                }
            }

            private string _avm = "";
            public string avm {
                get { return _avm; }
                set {
                    _avm = value;
                }
            }

            // *mi: motor microsteps 
            // Note, that InterpretLine() replaces "1" with "motor1" so we can use valid names
            private string _motor1mi = "";
            public string motor1mi {
                get { return _motor1mi; }
                set {
                    _motor1mi = value;
                }
            }

            private string _motor2mi = "";
            public string motor2mi {
                get { return _motor2mi; }
                set {
                    _motor2mi = value;
                }
            }

            private string _motor3mi = "";
            public string motor3mi {
                get { return _motor3mi; }
                set {
                    _motor3mi = value;
                }
            }

            private string _motor4mi = "";
            public string motor4mi {
                get { return _motor4mi; }
                set {
                    _motor4mi = value;
                }
            }

            // *tr: motor travel per rev. 
            private string _motor1tr = "";
            public string motor1tr {
                get { return _motor1tr; }
                set {
                    _motor1tr = value;
                }
            }

            private string _motor2tr = "";
            public string motor2tr {
                get { return _motor2tr; }
                set {
                    _motor2tr = value;
                }
            }

            private string _motor3tr = "";
            public string motor3tr {
                get { return _motor3tr; }
                set {
                    _motor3tr = value;
                }
            }

            private string _motor4tr = "";
            public string motor4tr {
                get { return _motor4tr; }
                set {
                    _motor4tr = value;
                }
            }

            // *sa: motor step angle 
            private string _motor1sa = "";
            public string motor1sa {
                get { return _motor1sa; }
                set {
                    _motor1sa = value;
                }
            }

            private string _motor2sa = "";
            public string motor2sa {
                get { return _motor2sa; }
                set {
                    _motor2sa = value;
                }
            }

            private string _motor3sa = "";
            public string motor3sa {
                get { return _motor3sa; }
                set {
                    _motor3sa = value;
                }
            }

            private string _motor4sa = "";
            public string motor4sa {
                get { return _motor4sa; }
                set {
                    _motor4sa = value;
                }
            }

            private string _xjh = "";
            public string xjh {
                get { return _xjh; }
                set {
                    _xjh = value;
                }
            }

            private string _yjh = "";
            public string yjh {
                get { return _yjh; }
                set {
                    _yjh = value;
                }
            }

            private string _zjh = "";
            public string zjh {
                get { return _zjh; }
                set {
                    _zjh = value;
                }
            }

            private string _xsv = "";
            public string xsv {
                get { return _xsv; }
                set {
                    _xsv = value;
                }
            }

            private string _ysv = "";
            public string ysv {
                get { return _ysv; }
                set {
                    _ysv = value;
                }
            }

            private string _zsv = "";
            public string zsv {
                get { return _zsv; }
                set {
                    _zsv = value;
                }
            }

            private string _xLV = "";
            public string xLV {
                get { return _xLV; }
                set {
                    _xLV = value;
                }
            }

            private string _yLV = "";
            public string yLV {
                get { return _yLV; }
                set {
                    _yLV = value;
                }
            }

            private string _zLV = "";
            public string zLV {
                get { return _zLV; }
                set {
                    _zLV = value;
                }
            }

            private string _xLB = "";
            public string xLB {
                get { return _xLB; }
                set {
                    _xLB = value;
                }
            }

            private string _yLB = "";
            public string yLB {
                get { return _yLB; }
                set {
                    _yLB = value;
                }
            }

            private string _zLB = "";
            public string zLB {
                get { return _zLB; }
                set {
                    _zLB = value;
                }
            }

            private string _xsn = "";
            public string xsn {
                get { return _xsn; }
                set {
                    _xsn = value;
                }
            }

            private string _ysn = "";
            public string ysn {
                get { return _ysn; }
                set {
                    _ysn = value;
                }
            }

            private string _zsn = "";
            public string zsn {
                get { return _zsn; }
                set {
                    _zsn = value;
                }
            }

            private string _xsx = "";
            public string xsx {
                get { return _xsx; }
                set {
                    _xsx = value;
                }
            }

            private string _ysx = "";
            public string ysx {
                get { return _ysx; }
                set {
                    _ysx = value;
                }
            }

            private string _zsx = "";
            public string zsx {
                get { return _zsx; }
                set {
                    _zsx = value;
                }
            }

        }

        public class TinyGResponse {
            public MachineConfiguration r { get; set; }
            public List<int> f { get; set; }
        }
    }
}
