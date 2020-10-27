using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using a = Autodesk.Revit.DB;

namespace TVDCEG
{
    public partial class FrmCopyReforcementGrand : Form
    {
        private CopyReforcementGrandcmd _data;
        private a.Document _doc;
        public ICollection<a.ElementId> listcopy = new List<a.ElementId>();
        private Dictionary<string, List<a.FamilyInstance>> DIC_listelement = new Dictionary<string, List<a.FamilyInstance>>();
        public FrmCopyReforcementGrand(CopyReforcementGrandcmd data, a.Document doc)
        {
            _data = data;
            _doc = doc;
            InitializeComponent();
            lbx_Showelement.DisplayMember = "Name";
            Showall();
        }

        private void FrmCopyReforcementNotMark_Load(object sender, EventArgs e)
        {

        }
        void Showall()
        {
            ListBoxfilter();
        }
        void ListBoxfilter()
        {
            lbx_Showelement.Items.Clear();
            DIC_listelement.Clear();
            if (ckb_Connection.Checked)
            {
                foreach (var i in _data.dic_connection)
                {
                    if (DIC_listelement.ContainsKey(i.Key))
                    {
                        continue;
                    }
                    else
                    {
                        DIC_listelement.Add(i.Key, i.Value);
                    }
                }
            }
            if (ckb_Reforcement.Checked)
            {
                foreach (var t in _data.dic_element)
                {
                    if (DIC_listelement.ContainsKey(t.Key))
                    {
                        continue;
                    }
                    else
                    {
                        DIC_listelement.Add(t.Key, t.Value);
                    }
                }
            }
            if (!ckb_Connection.Checked && !ckb_Reforcement.Checked)
            {
                foreach (var i in _data.dic_connection)
                {
                    if (DIC_listelement.ContainsKey(i.Key))
                    {
                        continue;
                    }
                    else
                    {
                        DIC_listelement.Add(i.Key, i.Value);
                    }
                }
                foreach (var t in _data.dic_element)
                {
                    if (DIC_listelement.ContainsKey(t.Key))
                    {
                        continue;
                    }
                    else
                    {
                        DIC_listelement.Add(t.Key, t.Value);
                    }
                }
            }
            Listbox(DIC_listelement);
        }
        void Listbox(Dictionary<string, List<a.FamilyInstance>> elements)
        {
            lbx_Showelement.Items.Clear();
            var list = elements.Keys.ToList();
            list.Sort();
            foreach (var key in list)
            {
                lbx_Showelement.Items.Add(key);
            }
        }

        private void Ckb_Reforcement_CheckedChanged(object sender, EventArgs e)
        {
            ListBoxfilter();
        }

        private void Ckb_Connection_CheckedChanged(object sender, EventArgs e)
        {
            ListBoxfilter();
        }

        private void Btn_OK_Click(object sender, EventArgs e)
        {
            listcopy = new List<a.ElementId>();
            foreach (var value in lbx_Showelement.SelectedItems.Cast<string>())
            {
                foreach (var fi in DIC_listelement[value])
                {
                    listcopy.Add(fi.Id);
                }
            }
            Close();
        }

        private void Btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
