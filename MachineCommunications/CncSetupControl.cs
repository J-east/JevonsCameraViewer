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
