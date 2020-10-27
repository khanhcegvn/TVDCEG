using Autodesk.Revit.DB;
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
    public partial class FrmAddvalueParameter : System.Windows.Forms.Form
    {
        private Addvalueparametercmd _data;
        public List<FamilyInstance> list = new List<FamilyInstance>();
        public FrmAddvalueParameter(Addvalueparametercmd data)
        {
            _data = data;
            InitializeComponent();
        }
        private void Loaddata()
        {
            lb_view.Items.Clear();
            lb_view.DataSource = _data.dic.Keys.ToList();
            cbb_Parameter.Items.Clear();
            cbb_Parameter.DataSource = _data.dicpa.Keys.ToList();
        }

        private void FrmAddvalueParameter_Load(object sender, EventArgs e)
        {
            Loaddata();
        }
    }
}
