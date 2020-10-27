using System;
using System.Collections.Generic;
using System.Windows.Forms;
using a = Autodesk.Revit.DB;

namespace TVDCEG
{
    public partial class FrmShowRefernceplane : Form
    {
        public CheckWorkPlanecmd _data;
        public a.Document _doc;
        public bool checklevelgrid = false;
        public List<a.FamilyInstance> list = new List<a.FamilyInstance>();
        public Dictionary<string, List<a.FamilyInstance>> diclist = new Dictionary<string, List<a.FamilyInstance>>();
        public List<a.ReferencePlane> newlist = new List<a.ReferencePlane>();
        public FrmShowRefernceplane(CheckWorkPlanecmd data, a.Document doc)
        {
            _data = data;
            _doc = doc;
            InitializeComponent();
        }
        private void FrmShowRefernceplane_Load(object sender, EventArgs e)
        {
            Treeviewallelement.Nodes.Add(_data.elementMgr.AllElementNames);
            Treeviewallelement.TopNode.Expand();
        }
        private void CheckNode(TreeNode node, bool check)
        {
            if (0 < node.Nodes.Count)
            {
                if (node.Checked)
                {
                    node.Expand();
                }
                else
                {
                    node.Collapse();
                }

                foreach (TreeNode t in node.Nodes)
                {
                    t.Checked = check;
                    CheckNode(t, check);
                }
            }
        }
        private void Btn_OK_Click(object sender, EventArgs e)
        {
            //var element = lbx_element.SelectedItems;
            //foreach(var nameitem in element.Cast<string>())
            //{
            //    foreach(var i in diclist[nameitem])
            //    {
            //        list.Add(i);
            //    }
            //}
            _data.elementMgr.SelectElements();
            list = _data.elementMgr.SelectedElement;
            if(checkBox1.Checked)
            {
                checklevelgrid = true;
            }
            Close();
        }

        private void Treeviewallelement_AfterCheck(object sender, TreeViewEventArgs e)
        {
            CheckNode(e.Node, e.Node.Checked);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
