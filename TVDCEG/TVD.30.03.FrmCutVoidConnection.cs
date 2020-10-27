using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using a = Autodesk.Revit.DB;

namespace TVDCEG
{
    public partial class FrmCutVoidConnection : Form
    {
        private a.Document _doc;
        public List<string> ListTypecut = new List<string>();
        public List<a.FamilyInstance> listconn = new List<a.FamilyInstance>();
        public List<a.FamilyInstance> listcut = new List<a.FamilyInstance>();
        private Dictionary<string, List<a.FamilyInstance>> dic = new Dictionary<string, List<a.FamilyInstance>>();
        private Dictionary<string, List<a.FamilyInstance>> dic2 = new Dictionary<string, List<a.FamilyInstance>>();
        public FrmCutVoidConnection(a.Document doc)
        {
            _doc = doc;
            InitializeComponent();
            dic = CutVoidConnection.Instance.GetAllConnection(_doc);
        }
        private void Datalistbox(List<string> list)
        {
            lb_Connection.Items.Clear();
            list.ForEach(x => lb_Connection.Items.Add(x));
        }
        private void Filterlistbox()
        {
            lb_Connection.Items.Clear();
            List<string> newlist = new List<string>();
            foreach (var item in dic.Keys.ToList())
            {
                if(item.Contains(Txt_Search.Text.ToUpper()))
                {
                    newlist.Add(item);
                }
            }
            Datalistbox(newlist);
        }
        private void Datalisttype()
        {
            lb_Typecut.Items.Clear();
            dic2 = CutVoidConnection.Instance.Getallframing(_doc);
            dic2.Keys.ToList().ForEach(x => lb_Typecut.Items.Add(x));
        }
        private void FrmCutVoidConnection_Load(object sender, EventArgs e)
        {
            Filterlistbox();
            Datalisttype();
        }

        private void Txt_Search_TextChanged(object sender, EventArgs e)
        {
            Filterlistbox();
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            foreach (var text in lb_Connection.SelectedItems.Cast<string>())
            {
                foreach (var item in dic[text])
                {
                    listconn.Add(item);
                }
            }
            foreach (var text in lb_Typecut.SelectedItems.Cast<string>())
            {
                foreach (var item in dic2[text])
                {
                    listcut.Add(item);
                }
            }
            Close();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
