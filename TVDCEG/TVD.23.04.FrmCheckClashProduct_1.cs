using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using a=Autodesk.Revit.DB;

namespace TVDCEG
{
    public partial class FrmCheckClashProduct_1 : Form
    {
        public ElementMerge meger=null;
        private a.Document _doc;
        public List<a.FamilyInstance> list = new List<a.FamilyInstance>();
        public bool check = false;
        public FrmCheckClashProduct_1(a.Document doc)
        {
            _doc = doc;
            meger = new ElementMerge(_doc);
            InitializeComponent();
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

        private void FrmCheckClashProduct_1_Load(object sender, EventArgs e)
        {
            treeView_element.Nodes.Add(meger.AllElementNames);
            treeView_element.TopNode.Expand();
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            check = true;
            meger.SelectElements();
            list = meger.SelectedElement;
            Close();
        }

        private void treeView_element_AfterCheck(object sender, TreeViewEventArgs e)
        {
            CheckNode(e.Node, e.Node.Checked);
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
