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
    public partial class TeeWarpForm : Form
    {
        DoubleTeeWarpController _data;
        public TeeWarpForm(DoubleTeeWarpController data)
        {
            _data = data;
            InitializeComponent();
        }

        private void TeeWarpForm_Load(object sender, EventArgs e)
        {

        }

        private void ok_Click(object sender, EventArgs e)
        {
            _data.TL.Z = double.Parse(a.Text) * 0.0833333;
            _data.TR.Z = double.Parse(b.Text) * 0.0833333;
            _data.BL.Z = double.Parse(c.Text) * 0.0833333;
            _data.BR.Z = double.Parse(d.Text) * 0.0833333;
            this.Close();
        }
    }
}
