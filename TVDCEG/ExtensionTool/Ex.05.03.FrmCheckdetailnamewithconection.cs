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
    public partial class FrmCheckdetailnamewithconection : Form
    {
        public Checkdetailnamewithconectioncmd _data;
        public FrmCheckdetailnamewithconection(Checkdetailnamewithconectioncmd data)
        {
            _data = data;
            InitializeComponent();
        }

        private void FrmCheckdetailnamewithconection_Load(object sender, EventArgs e)
        {
            lb_view.Items.Clear();
            _data.listsource.ForEach(x => lb_view.Items.Add(x));
        }
    }
}
