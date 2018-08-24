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
        static SerialCommSetupPanel serialSetupControl;
        private SerialComm Com;

        static ManualResetEventSlim _readyEvent = new ManualResetEventSlim(false);

        public CNC(CncSetupControl cncSetup, SerialCommSetupPanel serialCommSetup) {
            setUpControl = cncSetup;
            serialSetupControl = serialCommSetup;
            Com = new SerialComm(this, serialCommSetup);
            SlowXY = false;
            SlowZ = false;
            SlowA = false;
        }

        public ManualResetEventSlim ReadyEvent {
            get {
                return _readyEvent;
            }
        }

        public bool Connected { get; set; }
        public bool ErrorState { get; set; }

        public void Error() {
            ErrorState = true;
            // Connected = false;
            Homing = false;
            _readyEvent.Set();
            serialSetupControl.UpdateCncConnectionStatus(false);
        }

        public void Close() {
            Com.Close();
            ErrorState = false;
            Connected = false;
            Homing = false;
            _readyEvent.Set();
            serialSetupControl.UpdateCncConnectionStatus(false);
        }

        public bool Connect(String name) {
            // For now, just see that the port opens. 
            // TODO: check that there isTinyG, not just any comm port.
            // TODO: check/set default values

            if (Com.IsOpen) {
                AppendToLog("Connecting to serial port " + name + ": already open");
                Connected = true;
                return true;
            }
            Com.Open(name);
            ErrorState = false;
            Homing = false;
            _readyEvent.Set();
            Connected = Com.IsOpen;
            if (!Connected) {
                Error();
            }
            AppendToLog("Connecting to serial port " + name + ", result:" + Com.IsOpen.ToString());

            return Connected;
        }

        public bool Write(string command) {
            if (!Com.IsOpen) {
                AppendToLog("###" + command + " discarded, com not open (readyevent set)");
                _readyEvent.Set();
                Connected = false;
                return false;
            }
            if (ErrorState) {
                AppendToLog("###" + command + " discarded, error state on (readyevent set)");
                _readyEvent.Set();
                return false;
            }
            _readyEvent.Reset();
            bool res = Com.Write(command);
            _readyEvent.Wait();
            if (!res) {
                Error();
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
                Error();
            }
            return res;
        }

        public void ForceWrite(string command) {
            Com.Write(command);
        }

        // Square compensation:
        // The machine will be only approximately square. Fortunately, the squareness is easy to measure with camera.
        // User measures correction value, that we apply to movements and reads.
        // For example, correction value is +0.002, meaning that for every unit of +Y movement, 
        // the machine actually also unintentionally moves 0.002 units to +X. 
        // Therefore, for each movement when the user wants to go to (X, Y),
        // we really go to (X - 0.002*Y, Y)

        // CurrentX/Y is the corrected value that user sees and uses, and reflects a square machine
        // TrueX/Y is what the TinyG actually uses.

        public static double SquareCorrection { get; set; }

        private static double CurrX;
        private static double _trueX;

        public double TrueX {
            get {
                return (_trueX);
            }
            set {
                _trueX = value;
            }
        }

        public double CurrentX {
            get {
                return (CurrX);
            }
            set {
                CurrX = value;
            }
        }

        public static void setCurrX(double x) {
            _trueX = x;
            CurrX = x - CurrY * SquareCorrection;
            //AppendToLog("CNC.setCurrX: x= " + x.ToString() + ", CurrX= " + CurrX.ToString() + ", CurrY= " + CurrY.ToString());
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
        public static void setCurrY(double y) {
            CurrY = y;
            CurrX = _trueX - CurrY * SquareCorrection;
            //AppendToLog("CNC.setCurrY: "+ y.ToString()+ " CurrX= " + CurrX.ToString());
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
        public static void setCurrZ(double z) {
            CurrZ = z;
        }

        private static double CurrA;
        public double CurrentA {
            get {
                return (CurrA);
            }
            set {
                CurrA = value;
            }
        }
        public static void setCurrA(double a) {
            CurrA = a;
        }

        public bool SlackCompensation { get; set; }
        private double SlackCompensationDistance = 0.4;

        public bool SlackCompensationA { get; set; }
        private double SlackCompensationDistanceA = 5.0;

        public string SmallMovementString = "G1 F200 ";

        public bool SlowXY { get; set; }
        public double SlowSpeedXY { get; set; }

        public bool SlowZ { get; set; }
        public double SlowSpeedZ { get; set; }

        public bool SlowA { get; set; }
        public double SlowSpeedA { get; set; }


        public void XY(double X, double Y) {
            double dX = Math.Abs(X - CurrentX);
            double dY = Math.Abs(Y - CurrentY);
            if ((dX < 0.004) && (dY < 0.004)) {
                AppendToLog(" -- zero XY movement command --");
                AppendToLog("ReadyEvent: zero movement command");
                _readyEvent.Set();
                return;   // already there
            }
            if (!SlackCompensation) {
                XY_move(X, Y);
            }
            else {
                XY_move(X, Y);
                XY_move(X - 1.1, Y - 1.1);
                XY_move(X, Y);
            }
        }

        private void XY_move(double X, double Y) {
            string command;
            double dX = Math.Abs(X - CurrentX);
            double dY = Math.Abs(Y - CurrentY);
            if ((dX < 0.004) && (dY < 0.004)) {
                AppendToLog(" -- zero XY movement command --");
                AppendToLog("ReadyEvent: zero movement command");
                _readyEvent.Set();
                return;   // already there
            }
            X = X + SquareCorrection * Y;
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
            _readyEvent.Reset();
            Com.Write("{\"gc\":\"" + command + "\"}");
            _readyEvent.Wait();
        }

        public void XYA(double X, double Y, double Am) {
            bool CompensateXY = false;
            bool CompensateA = false;

            if ((SlackCompensation) && ((CurrentX > X) || (CurrentY > Y))) {
                CompensateXY = true;
            }

            if ((SlackCompensationA) && (CurrentA > (Am - SlackCompensationDistanceA))) {
                CompensateA = true;
            }


            if ((!CompensateXY) && (!CompensateA)) {
                XYA_move(X, Y, Am);
            }
            else if ((CompensateXY) && (!CompensateA)) {
                XYA_move(X - SlackCompensationDistance, Y - SlackCompensationDistance, Am);
                XY_move(X, Y);
            }
            else if ((!CompensateXY) && (CompensateA)) {
                XYA_move(X, Y, Am - SlackCompensationDistanceA);
                A_move(Am);
            }
            else {
                XYA_move(X - SlackCompensationDistance, Y - SlackCompensationDistance, Am - SlackCompensationDistanceA);
                XYA_move(X, Y, Am);
            }
        }

        private void XYA_move(double X, double Y, double Am) {
            string command;
            double dX = Math.Abs(X - CurrentX);
            double dY = Math.Abs(Y - CurrentY);
            double dA = Math.Abs(Am - CurrentA);

            while ((Am - CurrentA) > 180) {
                Am = Am - 360;
            }
            while ((Am - CurrentA) < -180) {
                Am = Am + 360;
            }


            if ((dX < 0.004) && (dY < 0.004) && (dA < 0.01)) {
                AppendToLog(" -- zero XYA movement command --");
                AppendToLog("ReadyEvent: zero movement command");
                _readyEvent.Set();
                return;   // already there
            }

            X = X + SquareCorrection * Y;
            if ((dX < 1.0) && (dY < 1.0)) {
                // small movement
                // First do XY move, then A. This works always.
                // (small moves and fast settings can sometimes cause problems)
                if ((dX < 0.004) && (dY < 0.004)) {
                    AppendToLog(" -- XYA command, XY already there --");
                }
                else {
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
                    _readyEvent.Reset();
                    Com.Write("{\"gc\":\"" + command + "\"}");
                    _readyEvent.Wait();
                }

                // then A:
                if (dA < 0.01 || Am == CurrentA) {
                    _readyEvent.Set();
                    AppendToLog(" -- XYA command, XY already there --");
                }
                else {
                    if (SlowA) {
                        command = "G1 F" + SlowSpeedA.ToString() + " A" + Am.ToString(CultureInfo.InvariantCulture);
                    }
                    else {
                        command = "G0 A" + Am.ToString(CultureInfo.InvariantCulture);
                    }
                    _readyEvent.Reset();
                    Com.Write("{\"gc\":\"" + command + "\"}");
                    _readyEvent.Wait();
                }
            }
            else {
                // normal case, large move
                // Possibilities:
                // SlowA, SlowXY
                // SlowA, FastXY
                // FastA, SlowXY
                // Fast A, Fast XY
                // To avoid side effects, we'll separate a and xy for first three cases
                if (SlowA || (!SlowA && SlowXY)) {
                    // Do A first, then XY
                    if (dA < 0.01) {
                        AppendToLog(" -- XYA command, XY already there --");
                    }
                    else {
                        if (SlowA) {
                            command = "G1 F" + SlowSpeedA.ToString() + " A" + Am.ToString(CultureInfo.InvariantCulture);
                        }
                        else {
                            command = "G0 A" + Am.ToString(CultureInfo.InvariantCulture);
                        }
                        _readyEvent.Reset();
                        Com.Write("{\"gc\":\"" + command + "\"}");
                        _readyEvent.Wait();
                    }
                    // A done, we know XY is slow and large
                    command = "G1 F" + SlowSpeedXY.ToString() +
                                " X" + X.ToString(CultureInfo.InvariantCulture) +
                                " Y" + Y.ToString(CultureInfo.InvariantCulture);
                    _readyEvent.Reset();
                    Com.Write("{\"gc\":\"" + command + "\"}");
                    _readyEvent.Wait();
                }
                else {
                    // Fast A, Fast XY
                    command = "G0 " + "X" + X.ToString(CultureInfo.InvariantCulture) +
                                     " Y" + Y.ToString(CultureInfo.InvariantCulture) +
                                     " A" + Am.ToString(CultureInfo.InvariantCulture);
                    _readyEvent.Reset();
                    Com.Write("{\"gc\":\"" + command + "\"}");
                    _readyEvent.Wait();
                }
            }
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
                _readyEvent.Set();
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
            _readyEvent.Reset();
            Com.Write("{\"gc\":\"" + command + "\"}");
            _readyEvent.Wait();
        }

        public void A(double A) {
            if (Math.Abs(A - CurrentA) < 0.01) {
                AppendToLog(" -- zero A movement command --");
                _readyEvent.Set();
                return;   // already there
            }
            if ((SlackCompensationA) && (CurrentA > (A - SlackCompensationDistanceA))) {
                A_move(A - SlackCompensationDistanceA);
                A_move(A);
            }
            else {
                A_move(A);
            }
        }
        private void A_move(double A) {
            string command;
            if (SlowA) {
                command = "G1 F" + SlowSpeedA.ToString() + " A" + A.ToString(CultureInfo.InvariantCulture);
            }
            else {
                command = "G0 A" + A.ToString(CultureInfo.InvariantCulture);
            }
            _readyEvent.Reset();
            Com.Write("{\"gc\":\"" + command + "\"}");
            _readyEvent.Wait();
        }

        public bool Homing { get; set; }
        public bool IgnoreError { get; set; }

        public void InterpretLine(string line) {
            // This is called from SerialComm dataReceived, and runs in a separate thread than UI            
            AppendToLog(line);

            if (line.Contains("SYSTEM READY")) {
                Error();
                MessageBox.Show("TinyG Reset.","System Reset",MessageBoxButtons.OK);
                serialSetupControl.UpdateCncConnectionStatus(false);
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
                Error();
                MessageBox.Show(
                    "TinyG error. Press Restart Button",
                    "TinyG Error",
                    MessageBoxButtons.OK);
                return;
            }


            if (line.StartsWith("{\"r\":{}")) {
                // ack for g code command
                return;
            }

            /* Special homing handling is not needed in this firmware version
            if (Homing)
            {
                if (line.StartsWith("{\"sr\":"))
                {
                    // Status report
                    NewStatusReport(line);
				}

                if (line.Contains("\"home\":1"))
                {
                    _readyEvent.Set();
                    AppendToLog("ReadyEvent home");
                }
                return; 
            }
            */

            if (line.StartsWith("tinyg [mm] ok>")) {
                // AppendToLog("ReadyEvent ok");
                _readyEvent.Set();
                return;
            }


            if (line.StartsWith("{\"sr\":")) {
                // Status report
                NewStatusReport(line);
                if (line.Contains("\"stat\":3")) {
                    AppendToLog("ReadyEvent stat");
                    _readyEvent.Set();
                }
                return;
            }

            if (line.StartsWith("{\"r\":{\"sr\"")) {
                // Status enquiry response, remove the wrapper:
                line = line.Substring(5);
                int i = line.IndexOf("}}");
                line = line.Substring(0, i + 2);
                NewStatusReport(line);
                _readyEvent.Set();
                AppendToLog("ReadyEvent r:sr");
                return;
            }

            if (line.StartsWith("{\"r\":{\"me") || line.StartsWith("{\"r\":{\"md")) {
                // response to motor power on/off commands
                _readyEvent.Set();
                return;
            }

            if (line.StartsWith("{\"r\":{\"sys\":")) {
                // response to reading settings for saving them
                // remove the wrapper:
                line = line.Substring(5);
                int i = line.IndexOf("}}");
                line = line.Substring(0, i + 2);
                TinyGSettings.TinyG_sys = line;
                _readyEvent.Set();
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
                _readyEvent.Set();
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
                _readyEvent.Set();
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
                _readyEvent.Set();
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
                _readyEvent.Set();
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
                _readyEvent.Set();
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
                _readyEvent.Set();
                AppendToLog("ReadyEvent m3 group");
                return;
            }

            if (line.StartsWith("{\"r\":")) {
                // response to setting a setting or reading motor settings for saving them
                // Replace {"1 with {"motor1 so that the field name is valid
                line = line.Replace("{\"1", "{\"motor1");
                line = line.Replace("{\"2", "{\"motor2");
                line = line.Replace("{\"3", "{\"motor3");
                NewSetting(line);
                _readyEvent.Set();
                AppendToLog("ReadyEvent r");
                return;
            }

        }  // end InterpretLine()

        // =================================================================================
        // TinyG JSON 
        // =================================================================================

        // =================================================================================
        // Status report

        public StatusReport Status;
        public void NewStatusReport(string line) {
            //AppendToLog("NewStatusReport: " + line);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Status = serializer.Deserialize<StatusReport>(line);
        }

        [Serializable]
        public class Sr {
            // mpox, posy, ...: Position
            // NOTE: Some firmware versions use mpox, mpoy,... some use posx, posy, ... 
            // This should be reflected in the public variable names
            private double _posx = 0;
            public double posx 
            {
                get { return _posx; }
                set {
                    _posx = value;
                    CNC.setCurrX(_posx);
                    //CNC.setUpControl.ValueUpdater("posx", _posx.ToString("0.000", CultureInfo.InvariantCulture));
                }
            }

            private double _posy = 0;
            public double posy 
            {
                get { return _posy; }
                set {
                    _posy = value;
                    CNC.setCurrY(_posy);
                    //CNC.setUpControl.ValueUpdater("posy", _posy.ToString("0.000", CultureInfo.InvariantCulture));
                }
            }

            private double _posz = 0;
            public double posz 
            {
                get { return _posz; }
                set {
                    _posz = value;
                    CNC.setCurrZ(_posz);
                    //CNC.setUpControl.ValueUpdater("posz", _posz.ToString("0.000", CultureInfo.InvariantCulture));
                }
            }

            private double _posa = 0;
            public double posa 
            {
                get { return _posa; }
                set {
                    _posa = value;
                    CNC.setCurrA(_posa);
                    //CNC.setUpControl.ValueUpdater("posa", _posa.ToString("0.000", CultureInfo.InvariantCulture));
                }
            }

            /*
            public double ofsx { get; set; }
            public double ofsy { get; set; }
            public double ofsz { get; set; }
            public double ofsa { get; set; }
            public int unit { get; set; }
            public int stat { get; set; }
            public int coor { get; set; }
            public int momo { get; set; }
            public int dist { get; set; }
            public int home { get; set; }
            public int hold { get; set; }
            public int macs { get; set; }
            public int cycs { get; set; }
            public int mots { get; set; }
            public int plan { get; set; }
             * */
        }

        [Serializable]
        public class StatusReport {
            public Sr sr { get; set; }
        }


        // =================================================================================
        // Settings

        public Response Settings;
        public void NewSetting(string line) {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Settings = serializer.Deserialize<Response>(line);
        }

        public class Resp {

            // =========================================================
            // The individual settings we care about and do something
            // when they change.

            // mt: motor timeout
            private string _mt = "";
            public string mt {
                get { return _mt; }
                set {
                    _mt = value;
                    //CNC.setUpControl.ValueUpdater("mt", _mt);
                }
            }

            // *jm: jerk max
            private string _xjm = "";
            public string xjm {
                get { return _xjm; }
                set {
                    _xjm = value;
                    //CNC.setUpControl.ValueUpdater("xjm", _xjm);
                }
            }

            private string _yjm = "";
            public string yjm {
                get { return _yjm; }
                set {
                    _yjm = value;
                    //CNC.setUpControl.ValueUpdater("yjm", _yjm);
                }
            }

            private string _zjm = "";
            public string zjm {
                get { return _zjm; }
                set {
                    _zjm = value;
                    //CNC.setUpControl.ValueUpdater("zjm", _zjm);
                }
            }

            private string _ajm = "";
            public string ajm {
                get { return _ajm; }
                set {
                    _ajm = value;
                    //CNC.setUpControl.ValueUpdater("ajm", _ajm);
                }
            }

            // *vm: velocity max
            private string _xvm = "";
            public string xvm {
                get { return _xvm; }
                set {
                    _xvm = value;
                    //CNC.setUpControl.ValueUpdater("xvm", _xvm);
                }
            }

            private string _yvm = "";
            public string yvm {
                get { return _yvm; }
                set {
                    _yvm = value;
                    //CNC.setUpControl.ValueUpdater("yvm", _yvm);
                }
            }

            private string _zvm = "";
            public string zvm {
                get { return _zvm; }
                set {
                    _zvm = value;
                    //CNC.setUpControl.ValueUpdater("zvm", _zvm);
                }
            }

            private string _avm = "";
            public string avm {
                get { return _avm; }
                set {
                    _avm = value;
                    //CNC.setUpControl.ValueUpdater("avm", _avm);
                }
            }

            // *mi: motor microsteps 
            // Note, that InterpretLine() replaces "1" with "motor1" so we can use valid names
            private string _motor1mi = "";
            public string motor1mi {
                get { return _motor1mi; }
                set {
                    _motor1mi = value;
                    //CNC.setUpControl.ValueUpdater("1mi", _motor1mi);
                }
            }

            private string _motor2mi = "";
            public string motor2mi {
                get { return _motor2mi; }
                set {
                    _motor2mi = value;
                    //CNC.setUpControl.ValueUpdater("2mi", _motor2mi);
                }
            }

            private string _motor3mi = "";
            public string motor3mi {
                get { return _motor3mi; }
                set {
                    _motor3mi = value;
                    //CNC.setUpControl.ValueUpdater("3mi", _motor3mi);
                }
            }

            private string _motor4mi = "";
            public string motor4mi {
                get { return _motor4mi; }
                set {
                    _motor4mi = value;
                    //CNC.setUpControl.ValueUpdater("4mi", _motor4mi);
                }
            }

            // *tr: motor travel per rev. 
            private string _motor1tr = "";
            public string motor1tr {
                get { return _motor1tr; }
                set {
                    _motor1tr = value;
                    //CNC.setUpControl.ValueUpdater("1tr", _motor1tr);
                }
            }

            private string _motor2tr = "";
            public string motor2tr {
                get { return _motor2tr; }
                set {
                    _motor2tr = value;
                    //CNC.setUpControl.ValueUpdater("2tr", _motor2tr);
                }
            }

            private string _motor3tr = "";
            public string motor3tr {
                get { return _motor3tr; }
                set {
                    _motor3tr = value;
                    //CNC.setUpControl.ValueUpdater("3tr", _motor3tr);
                }
            }

            private string _motor4tr = "";
            public string motor4tr {
                get { return _motor4tr; }
                set {
                    _motor4tr = value;
                    //CNC.setUpControl.ValueUpdater("4tr", _motor4tr);
                }
            }

            // *sa: motor step angle 
            private string _motor1sa = "";
            public string motor1sa {
                get { return _motor1sa; }
                set {
                    _motor1sa = value;
                    //CNC.setUpControl.ValueUpdater("1sa", _motor1sa);
                }
            }

            private string _motor2sa = "";
            public string motor2sa {
                get { return _motor2sa; }
                set {
                    _motor2sa = value;
                    //CNC.setUpControl.ValueUpdater("2sa", _motor2sa);
                }
            }

            private string _motor3sa = "";
            public string motor3sa {
                get { return _motor3sa; }
                set {
                    _motor3sa = value;
                    //CNC.setUpControl.ValueUpdater("3sa", _motor3sa);
                }
            }

            private string _motor4sa = "";
            public string motor4sa {
                get { return _motor4sa; }
                set {
                    _motor4sa = value;
                    //CNC.setUpControl.ValueUpdater("4sa", _motor4sa);
                }
            }

            private string _xjh = "";
            public string xjh {
                get { return _xjh; }
                set {
                    _xjh = value;
                    //CNC.setUpControl.ValueUpdater("xjh", _xjh);
                }
            }

            private string _yjh = "";
            public string yjh {
                get { return _yjh; }
                set {
                    _yjh = value;
                    //CNC.setUpControl.ValueUpdater("yjh", _yjh);
                }
            }

            private string _zjh = "";
            public string zjh {
                get { return _zjh; }
                set {
                    _zjh = value;
                    //CNC.setUpControl.ValueUpdater("zjh", _zjh);
                }
            }

            private string _xsv = "";
            public string xsv {
                get { return _xsv; }
                set {
                    _xsv = value;
                    //CNC.setUpControl.ValueUpdater("xsv", _xsv);
                }
            }

            private string _ysv = "";
            public string ysv {
                get { return _ysv; }
                set {
                    _ysv = value;
                    //CNC.setUpControl.ValueUpdater("ysv", _ysv);
                }
            }

            private string _zsv = "";
            public string zsv {
                get { return _zsv; }
                set {
                    _zsv = value;
                    //CNC.setUpControl.ValueUpdater("zsv", _zsv);
                }
            }

            private string _xLV = "";
            public string xLV {
                get { return _xLV; }
                set {
                    _xLV = value;
                    //CNC.setUpControl.ValueUpdater("xLV", _xLV);
                }
            }

            private string _yLV = "";
            public string yLV {
                get { return _yLV; }
                set {
                    _yLV = value;
                    //CNC.setUpControl.ValueUpdater("yLV", _yLV);
                }
            }

            private string _zLV = "";
            public string zLV {
                get { return _zLV; }
                set {
                    _zLV = value;
                    //CNC.setUpControl.ValueUpdater("zLV", _zLV);
                }
            }

            private string _xLB = "";
            public string xLB {
                get { return _xLB; }
                set {
                    _xLB = value;
                    //CNC.setUpControl.ValueUpdater("xLB", _xLB);
                }
            }

            private string _yLB = "";
            public string yLB {
                get { return _yLB; }
                set {
                    _yLB = value;
                    //CNC.setUpControl.ValueUpdater("yLB", _yLB);
                }
            }

            private string _zLB = "";
            public string zLB {
                get { return _zLB; }
                set {
                    _zLB = value;
                    //CNC.setUpControl.ValueUpdater("zLB", _zLB);
                }
            }

            private string _xsn = "";
            public string xsn {
                get { return _xsn; }
                set {
                    _xsn = value;
                    //CNC.setUpControl.ValueUpdater("xsn", _xsn);
                }
            }

            private string _ysn = "";
            public string ysn {
                get { return _ysn; }
                set {
                    _ysn = value;
                    //CNC.setUpControl.ValueUpdater("ysn", _ysn);
                }
            }

            private string _zsn = "";
            public string zsn {
                get { return _zsn; }
                set {
                    _zsn = value;
                    //CNC.setUpControl.ValueUpdater("zsn", _zsn);
                }
            }

            private string _xsx = "";
            public string xsx {
                get { return _xsx; }
                set {
                    _xsx = value;
                    //CNC.setUpControl.ValueUpdater("xsx", _xsx);
                }
            }

            private string _ysx = "";
            public string ysx {
                get { return _ysx; }
                set {
                    _ysx = value;
                    //CNC.setUpControl.ValueUpdater("ysx", _ysx);
                }
            }

            private string _zsx = "";
            public string zsx {
                get { return _zsx; }
                set {
                    _zsx = value;
                    //CNC.setUpControl.ValueUpdater("zsx", _zsx);
                }
            }

        }   // end class Resp

        public class Response {
            public Resp r { get; set; }
            public List<int> f { get; set; }
        }

    }  // end Class CNC
}
