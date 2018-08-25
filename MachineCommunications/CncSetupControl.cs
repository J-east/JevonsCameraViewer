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

        private bool UpdateWindowValues_m() {
            if (!serialSetup.SendSerialCommand(@"{""sr"":""""}")) {
                return false;
            }

            if (!serialSetup.SendSerialCommand(@"{""xjm"":""""}")) {
                return false;
            }
            if (!serialSetup.SendSerialCommand(@"{""xvm"":""""}")) {
                return false;
            }
            if (!serialSetup.SendSerialCommand(@"{""xsv"":""""}")) {
                return false;
            }
            if (!serialSetup.SendSerialCommand(@"{""xlv"":""""}")) {
                return false;
            }
            if (!serialSetup.SendSerialCommand(@"{""xlb"":""""}")) {
                return false;
            }
            if (!serialSetup.SendSerialCommand(@"{""xsn"":""""}")) {
                return false;
            }
            if (!serialSetup.SendSerialCommand(@"{""xjh"":""""}")) {
                return false;
            }
            if (!serialSetup.SendSerialCommand(@"{""xsx"":""""}")) {
                return false;
            }

            if (!serialSetup.SendSerialCommand(@"{""1mi"":""""}")) {
                return false;
            }
            if (!serialSetup.SendSerialCommand(@"{""1sa"":""""}")) {
                return false;
            }
            if (!serialSetup.SendSerialCommand(@"{""1tr"":""""}")) {
                return false;
            }

            if (!serialSetup.SendSerialCommand(@"{""yjm"":""""}")) {
                return false;
            }
            if (!serialSetup.SendSerialCommand(@"{""yvm"":""""}")) {
                return false;
            }
            if (!serialSetup.SendSerialCommand(@"{""ysn"":""""}")) {
                return false;
            }
            if (!serialSetup.SendSerialCommand(@"{""ysx"":""""}")) {
                return false;
            }
            if (!serialSetup.SendSerialCommand(@"{""yjh"":""""}")) {
                return false;
            }
            if (!serialSetup.SendSerialCommand(@"{""ysv"":""""}")) {
                return false;
            }
            if (!serialSetup.SendSerialCommand(@"{""ylv"":""""}")) {
                return false;
            }
            if (!serialSetup.SendSerialCommand(@"{""ylb"":""""}")) {
                return false;
            }

            if (!serialSetup.SendSerialCommand(@"{""2mi"":""""}")) {
                return false;
            }
            if (!serialSetup.SendSerialCommand(@"{""2sa"":""""}")) {
                return false;
            }
            if (!serialSetup.SendSerialCommand(@"{""2tr"":""""}")) {
                return false;
            }

            if (!serialSetup.SendSerialCommand(@"{""zjm"":""""}")) {
                return false;
            }
            if (!serialSetup.SendSerialCommand(@"{""zvm"":""""}")) {
                return false;
            }
            if (!serialSetup.SendSerialCommand(@"{""zsn"":""""}")) {
                return false;
            }
            if (!serialSetup.SendSerialCommand(@"{""zsx"":""""}")) {
                return false;
            }
            if (!serialSetup.SendSerialCommand(@"{""zjh"":""""}")) {
                return false;
            }
            if (!serialSetup.SendSerialCommand(@"{""zsv"":""""}")) {
                return false;
            }
            if (!serialSetup.SendSerialCommand(@"{""zlv"":""""}")) {
                return false;
            }
            if (!serialSetup.SendSerialCommand(@"{""zlb"":""""}")) {
                return false;
            }

            if (!serialSetup.SendSerialCommand(@"{""3mi"":""""}")) {
                return false;
            }
            if (!serialSetup.SendSerialCommand(@"{""3sa"":""""}")) {
                return false;
            }
            if (!serialSetup.SendSerialCommand(@"{""3tr"":""""}")) {
                return false;
            }

            if (!serialSetup.SendSerialCommand(@"{""ajm"":""""}")) {
                return false;
            }
            if (!serialSetup.SendSerialCommand(@"{""avm"":""""}")) {
                return false;
            }

            if (!serialSetup.SendSerialCommand(@"{""4mi"":""""}")) {
                return false;
            }
            if (!serialSetup.SendSerialCommand(@"{""4sa"":""""}")) {
                return false;
            }
            if (!serialSetup.SendSerialCommand(@"{""4tr"":""""}")) {
                return false;
            }

            if (!serialSetup.SendSerialCommand(@"{""mt"":""""}")) {
                return false;
            }

            // Do settings that need to be done always
            cnc.IgnoreError = true;
            ProbingMode(false);

            serialSetup.SendSerialCommand(@"{""me"":""""}");  // motor power on
            cbMotorPower.Checked = true;
            return true;
        }

        public void ProbingMode(bool set) {
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

        private void xjm_maskedTextBox_TextChanged(object sender, EventArgs e) {

        }

        private void xvm_maskedTextBox_TextChanged(object sender, EventArgs e) {

        }

        private void yjm_maskedTextBox_TextChanged(object sender, EventArgs e) {

        }

        private void yvm_maskedTextBox_TextChanged(object sender, EventArgs e) {

        }

        private void zjm_maskedTextBox_TextChanged(object sender, EventArgs e) {

        }

        private void zvm_maskedTextBox_TextChanged(object sender, EventArgs e) {

        }

        private void bYUp_Click(object sender, EventArgs e) {

        }
    }
}
