using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TVDCEG
{
    public partial class FrmIncrease : Form
    {
        private FrmCheckControlNumber _formdata;
        public int increase;
        public FrmIncrease(FrmCheckControlNumber formdata)
        {
            _formdata = formdata;
            InitializeComponent();
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            increase = Convert.ToInt32(txt_increase.Text);
            Close();
        }

    }
}
