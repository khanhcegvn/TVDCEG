using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using a = Autodesk.Revit.DB;

namespace TVDCEG
{
    public partial class Frmchangeworkplane : Form
    {
        public CheckWorkPlanecmd _data;
        public a.Document _doc;
        public List<a.ReferencePlane> newlist = new List<a.ReferencePlane>();
        public Frmchangeworkplane(CheckWorkPlanecmd data, a.Document doc)
        {
            _data = data;
            _doc = doc;
            InitializeComponent();
            ShowALL();
            lbxplane.DisplayMember = "Name";
        }
        private void Frm_Load(object sender, EventArgs e)
        {

        }
        private void ShowALL()
        {
            Filterlist();
        }
        private void lbxWorkplane(List<a.ReferencePlane> planes)
        {
            lbxplane.Items.Clear();
            planes.ForEach(x => lbxplane.Items.Add(x));
        }
        private void Filterlist()
        {
            lbxplane.Items.Clear();
            var views = new List<a.ReferencePlane>();
            newlist = _data.listskp;
            foreach (var view in newlist)
            {
                if (view.Name.ToUpper().Contains(textBox_Search.Text.ToUpper()))
                {
                    views.Add(view);
                }
            }
            views = views.OrderBy(x => x.Name).ToList();
            lbxWorkplane(views);
        }
        private void Lbxplane_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void TextBox_Search_TextChanged(object sender, EventArgs e)
        {
            Filterlist();
        }

        private void Btn_OK_Click(object sender, EventArgs e)
        {
            //var planeid = lbxplane.SelectedItem;
            //a.ReferencePlane pop = planeid as a.ReferencePlane;
            //lbr_.Instance.ChangeWorkPlaneOfTee(_doc, _data.instance, pop);
        }
    }
}
