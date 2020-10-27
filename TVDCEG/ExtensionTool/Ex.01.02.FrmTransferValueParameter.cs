using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TVDCEG.ExtensionTool
{
    public partial class FrmTransferValueParameter : Form
    {
        public string Parameter_Source;
        public string Parameter_Target;
        private TransferValueParametercmd _data;
        public FrmTransferValueParameter(TransferValueParametercmd data)
        {
            _data = data;
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void LoadData()
        {
            cbb_source.DataSource = _data.dicpa.Keys.ToList();
            cbb_Target.DataSource = _data.dicpa.Keys.ToList();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FrmTransferValueParameter_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            Parameter_Source = cbb_source.Text;
            Parameter_Target = cbb_Target.Text;
            Close();
        }
    }
}
