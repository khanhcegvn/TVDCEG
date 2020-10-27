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
    public partial class FrmFilterCustom : Form
    {
        public new string Name;
        public bool check = false;
        private SelectCustomCmd _data;
        public FrmFilterCustom(SelectCustomCmd data)
        {
            _data = data;
            InitializeComponent();
        }

        private void FrmFilterCustom_Load(object sender, EventArgs e)
        {
            try
            {
                txt_name.Text = _data.Setting.Name;
            }
            catch
            {

            }
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            Name = txt_name.Text;
            _data.Setting.Name = Name;
            _data.Setting.SaveSetting();
            check = true;
            Close();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
