using Autodesk.Revit.UI;
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
    public partial class FrmReport : Form
    {
        private CheckClashProductcmd _data;
        public ExternalEvent ExEvent { get; set; }
        private List<string> listvalues = new List<string>();
        private void OnButtonClicked()
        {
            int num = (int)this.ExEvent.Raise();
        }
        public FrmReport(CheckClashProductcmd data)
        {
            _data = data;
            InitializeComponent();
            Removevaluecontains();
        }
        private void Removevaluecontains()
        {
            List<string> list = new List<string>();
            _data.dic.Keys.ToList().ForEach(x => listvalues.Add(x));
            listvalues.ForEach(x => lb_show.Items.Add(x));
        }

        private void btn_show_Click(object sender, EventArgs e)
        {
            _data.listshow.Clear();
            var y = lb_show.SelectedItems.Cast<string>().ToList();
            _data.listshow = CheckClashProduct.Instance.Stringtoelementid(_data.dic,y);
            this.OnButtonClicked();
        }
    }
}
