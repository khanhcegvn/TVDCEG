using System;
using System.Windows.Forms;

namespace TVDCEG
{
    public partial class FrmCopyConn_Dt_To_Dt : Form
    {
        public bool Checkfl;
        private CopyConnectionFlangeDTcmd _data;
        public bool checkbutton = false;
        public FrmCopyConn_Dt_To_Dt(CopyConnectionFlangeDTcmd data)
        {
            _data = data;
            InitializeComponent();
        }

        private void FrmCopyConn_Dt_To_Dt_Load(object sender, EventArgs e)
        {

        }
        private void Checkvalue()
        {
            if (radioButton_flangeLeft.Checked)
            {
                Checkfl = false;
            }
            if (radioButton_flangeRight.Checked)
            {
                Checkfl = true;
            }
        }

        private void Btn_OK_Click(object sender, EventArgs e)
        {
            checkbutton = true;
            Close();
        }

        private void RadioButton_flangeLeft_CheckedChanged(object sender, EventArgs e)
        {
            Checkvalue();
        }

        private void RadioButton_flangeRight_CheckedChanged(object sender, EventArgs e)
        {
            Checkvalue();
        }
    }
}
