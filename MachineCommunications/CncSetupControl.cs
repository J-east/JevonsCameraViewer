using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace MachineCommunications {
    public partial class CncSetupControl : UserControl {
        CNC cnc;
        SerialCommSetupPanel serialSetup;
        public CncSetupControl() {
            InitializeComponent();
        }

        public void FinalizeSetup(SerialCommSetupPanel serialSetup) {
            this.serialSetup = serialSetup;
            cnc = new CNC(this, serialSetup);
            serialSetup.FinalizeSetup(cnc);
        }

        /// <summary>
        /// sends commands to tinyG to obtain current configuration
        /// </summary>        
        public void UpdateCurrentTinyGConfigUI() {
            // Do settings that need to be done always
            cnc.IgnoreError = true;
            DisableZProbeLimit(false);

            serialSetup.SendSerialCommand(@"{""me"":""""}");  // motor power on
            cbMotorPower.Checked = true;
        }

        public void DisableZProbeLimit(bool set) {
            if (set) {
                serialSetup.SendSerialCommand(@"{""zsn"",0}");
                Thread.Sleep(250);
                serialSetup.SendSerialCommand(@"{""zsx"",1}");
                Thread.Sleep(250);
                serialSetup.SendSerialCommand(@"{""zzb"",0}");
                Thread.Sleep(250);
            }
            else {
                serialSetup.SendSerialCommand(@"{""zsn"",3}");
                Thread.Sleep(250);
                serialSetup.SendSerialCommand(@"{""zsx"",2}");
                Thread.Sleep(250);
                serialSetup.SendSerialCommand(@"{""zzb"",2}");
                Thread.Sleep(250);
            }
        }

        private void cbMotorPower_CheckedChanged(object sender, EventArgs e) {
            cnc.SetMotorPower(cbMotorPower.Checked);
        }

        private void cbVacuumPump_CheckedChanged(object sender, EventArgs e) {
            cnc.SetPumpPower(cbVacuumPump.Checked);
        }

        private void cbSolenoid_CheckedChanged(object sender, EventArgs e) {
            cnc.SetVacuumSolenoid(cbSolenoid.Checked);
        }

        /// <summary>
        /// sends updated values to the tinyG controller
        /// </summary>
        private void bUpdateConfig_Click(object sender, EventArgs e) {

        }

        enum direction {
            YUp,
            YDown,
            XUp,
            XDown,
            ZUp,
            ZDown
        }

        static int moveSPD = 2000;
        string moveCMD = $"{{\"gc\":\"G1 F{moveSPD}";
        int machineSizeX = 400;
        int machineSizeY = 400;
        int machineSizeZ = 30;
        private void JogMachine(direction moveDir) {
            switch (moveDir) {
                case direction.YUp:
                    serialSetup.SendSerialCommand($"{moveCMD} Y{machineSizeY}\"}}");
                    break;
                case direction.YDown:
                    serialSetup.SendSerialCommand($"{moveCMD} Y0\"}}");
                    break;
                case direction.XUp:
                    serialSetup.SendSerialCommand($"{moveCMD} X{machineSizeX}\"}}");
                    break;
                case direction.XDown:
                    serialSetup.SendSerialCommand($"{moveCMD} X0\"}}");
                    break;
                case direction.ZUp:
                    serialSetup.SendSerialCommand($"{moveCMD} Z0\"}}");
                    break;
                case direction.ZDown:
                    serialSetup.SendSerialCommand($"{moveCMD} Z{machineSizeZ}\"}}");
                    break;                
            }

            machineIsMoving = true;
        }

        bool machineIsMoving = false;
        private void bXUp_MouseDown(object sender, MouseEventArgs e) {
            JogMachine(direction.XUp);
        }

        private void bYUp_MouseDown(object sender, MouseEventArgs e) {
            JogMachine(direction.YUp);
        }

        private void bXDown_MouseDown(object sender, MouseEventArgs e) {
            JogMachine(direction.XDown);
        }

        private void bYDown_MouseDown(object sender, MouseEventArgs e) {
            JogMachine(direction.YDown);
        }

        private void bZUp_MouseDown(object sender, MouseEventArgs e) {
            JogMachine(direction.ZUp);
        }

        private void bZDown_MouseDown(object sender, MouseEventArgs e) {
            JogMachine(direction.ZDown);
        }

        private void bYUp_MouseUp(object sender, MouseEventArgs e) {
            serialSetup.SendSerialCommand("!%");
        }

        private void bXDown_MouseUp(object sender, MouseEventArgs e) {
            serialSetup.SendSerialCommand("!%");
        }

        private void bYDown_MouseUp(object sender, MouseEventArgs e) {
            serialSetup.SendSerialCommand("!%");
        }

        private void bXUp_MouseUp(object sender, MouseEventArgs e) {
            serialSetup.SendSerialCommand("!%");
        }

        private void bZUp_MouseUp(object sender, MouseEventArgs e) {
            serialSetup.SendSerialCommand("!%");
        }

        private void bZDown_MouseUp(object sender, MouseEventArgs e) {
            serialSetup.SendSerialCommand("!%");
        }
    }
}
