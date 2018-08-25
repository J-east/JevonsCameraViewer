using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            if (!CNC_Write_m("{\"sr\":\"\"}")) {
                return false;
            }

            if (!CNC_Write_m("{\"xjm\":\"\"}")) {
                return false;
            }
            if (!CNC_Write_m("{\"xvm\":\"\"}")) {
                return false;
            }
            if (!CNC_Write_m("{\"xsv\":\"\"}")) {
                return false;
            }
            if (!CNC_Write_m("{\"xlv\":\"\"}")) {
                return false;
            }
            if (!CNC_Write_m("{\"xlb\":\"\"}")) {
                return false;
            }
            if (!CNC_Write_m("{\"xsn\":\"\"}")) {
                return false;
            }
            if (!CNC_Write_m("{\"xjh\":\"\"}")) {
                return false;
            }
            if (!CNC_Write_m("{\"xsx\":\"\"}")) {
                return false;
            }

            if (!CNC_Write_m("{\"1mi\":\"\"}")) {
                return false;
            }
            if (!CNC_Write_m("{\"1sa\":\"\"}")) {
                return false;
            }
            if (!CNC_Write_m("{\"1tr\":\"\"}")) {
                return false;
            }

            if (!CNC_Write_m("{\"yjm\":\"\"}")) {
                return false;
            }
            if (!CNC_Write_m("{\"yvm\":\"\"}")) {
                return false;
            }
            if (!CNC_Write_m("{\"ysn\":\"\"}")) {
                return false;
            }
            if (!CNC_Write_m("{\"ysx\":\"\"}")) {
                return false;
            }
            if (!CNC_Write_m("{\"yjh\":\"\"}")) {
                return false;
            }
            if (!CNC_Write_m("{\"ysv\":\"\"}")) {
                return false;
            }
            if (!CNC_Write_m("{\"ylv\":\"\"}")) {
                return false;
            }
            if (!CNC_Write_m("{\"ylb\":\"\"}")) {
                return false;
            }

            if (!CNC_Write_m("{\"2mi\":\"\"}")) {
                return false;
            }
            if (!CNC_Write_m("{\"2sa\":\"\"}")) {
                return false;
            }
            if (!CNC_Write_m("{\"2tr\":\"\"}")) {
                return false;
            }

            if (!CNC_Write_m("{\"zjm\":\"\"}")) {
                return false;
            }
            if (!CNC_Write_m("{\"zvm\":\"\"}")) {
                return false;
            }
            if (!CNC_Write_m("{\"zsn\":\"\"}")) {
                return false;
            }
            if (!CNC_Write_m("{\"zsx\":\"\"}")) {
                return false;
            }
            if (!CNC_Write_m("{\"zjh\":\"\"}")) {
                return false;
            }
            if (!CNC_Write_m("{\"zsv\":\"\"}")) {
                return false;
            }
            if (!CNC_Write_m("{\"zlv\":\"\"}")) {
                return false;
            }
            if (!CNC_Write_m("{\"zlb\":\"\"}")) {
                return false;
            }

            if (!CNC_Write_m("{\"3mi\":\"\"}")) {
                return false;
            }
            if (!CNC_Write_m("{\"3sa\":\"\"}")) {
                return false;
            }
            if (!CNC_Write_m("{\"3tr\":\"\"}")) {
                return false;
            }

            if (!CNC_Write_m("{\"ajm\":\"\"}")) {
                return false;
            }
            if (!CNC_Write_m("{\"avm\":\"\"}")) {
                return false;
            }

            if (!CNC_Write_m("{\"4mi\":\"\"}")) {
                return false;
            }
            if (!CNC_Write_m("{\"4sa\":\"\"}")) {
                return false;
            }
            if (!CNC_Write_m("{\"4tr\":\"\"}")) {
                return false;
            }

            if (!CNC_Write_m("{\"mt\":\"\"}")) {
                return false;
            }

            // Do settings that need to be done always
            Cnc.IgnoreError = true;
            Nozzle.ProbingMode(false, JSON);
            //PumpDefaultSetting();
            //VacuumDefaultSetting();
            //Thread.Sleep(100);
            //Vacuum_checkBox.Checked = true;
            //Cnc.IgnoreError = false;
            CNC_Write_m("{\"me\":\"\"}");  // motor power on
            MotorPower_checkBox.Checked = true;
            return true;
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
