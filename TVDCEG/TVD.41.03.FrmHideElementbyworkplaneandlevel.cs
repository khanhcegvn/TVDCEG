using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using a=Autodesk.Revit.DB;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.UI;

namespace TVDCEG
{
    public partial class FrmHideElementbyworkplaneandlevel : Form
    {
        private Dictionary<string, List<a.FamilyInstance>> dic = new Dictionary<string, List<a.FamilyInstance>>();
        private a.Document _doc;
        public ExternalEvent ExEvent { get; set; }
        public List<a.FamilyInstance> familyInstances = new List<a.FamilyInstance>();
        public ICollection<a.ElementId> ids = new List<a.ElementId>();
        public FrmHideElementbyworkplaneandlevel(a.Document doc)
        {
            _doc = doc;
            InitializeComponent();
        }
        private void FrmHideElementbyworkplaneandlevel_Load(object sender, EventArgs e)
        {
            Data();
        }
        private void OnButtonClicked()
        {
            int num = (int)this.ExEvent.Raise();
        }
        public void Data()
        {
            dic = HideElementbyworkplaneandlevel.Instance.Filterelementbylevel(_doc, _doc.ActiveView);
            var liststring = dic.Keys.ToList();
            liststring.Sort();
            liststring.ForEach(x => lb_view.Items.Add(x));
        }
        private void Listboxdata(List<string> list)
        {
            lb_view.Items.Clear();
            list.Sort();
            list.ForEach(x => lb_view.Items.Add(x));
        }
        private void Filter()
        {
            List<string> list = new List<string>();
            foreach (var item in dic.Keys)
            {
                if(item.ToUpper().Contains(txt_Search.Text.ToUpper()))
                {
                    list.Add(item);
                }
            }
            Listboxdata(list);
        }
        private void txt_Search_TextChanged(object sender, EventArgs e)
        {
            Filter();
        }
        private void btn_OK_Click(object sender, EventArgs e)
        {
            var values = lb_view.SelectedItems.Cast<string>();
            List<a.ElementId> list = new List<a.ElementId>();
            foreach (var txt in values)
            {
                foreach (var item in dic[txt])
                {
                    list.Add(item.Id);
                }
            }
            ids = HideElementbyworkplaneandlevel.Instance.GetAllNestedFamily(_doc,list);
            this.OnButtonClicked();
        }
        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
