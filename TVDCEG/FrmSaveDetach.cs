using System;
using System.Windows.Forms;

namespace TVDCEG
{
    public partial class FrmSaveDetach : Form
    {
        public string file;
        private SaveDetach _data;
        public bool checkoption = false;
        public bool checkActivedoc = false;
        public FrmSaveDetach(SaveDetach data)
        {
            _data = data;
            InitializeComponent();
        }

        private void Btn_Open_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.ShowDialog();
            if (of.FileName != "")
            {
                file = of.FileName;
                textBox1.Text = file;
            }
        }

        private void FrmSaveDetach_Load(object sender, EventArgs e)
        {

        }

        private void Btn_OK_Click_1(object sender, EventArgs e)
        {
            if (Rbtn_DisWorkset.Checked)
            {
                checkoption = true;
            }
            if (Rbtn_Workset.Checked)
            {
                checkoption = false;
            }
            Close();
        }

        private void Checkbox_Activedocument_CheckedChanged(object sender, EventArgs e)
        {
            if (checkbox_Activedocument.Checked)
            {
                btn_Open.Enabled = false;
                checkActivedoc = true;
            }
            else
            {
                btn_Open.Enabled = true;
            }
        }
    }
}
