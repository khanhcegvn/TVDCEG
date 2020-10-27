using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using a = Autodesk.Revit.DB;
using System.Windows.Forms;

namespace TVDCEG
{
    public partial class FrmCutvoidbytypes : Form
    {
        private a.Document _doc;
        private CutVoidByTypescmd _data;
        public bool cut = false;
        public bool uncut = false;
        public List<a.FamilyInstance> list = new List<a.FamilyInstance>(); 
        public FrmCutvoidbytypes(CutVoidByTypescmd data,a.Document doc)
        {
            _doc = doc;
            _data = data;
            InitializeComponent();
        }
        private void Datalisttype()
        {
            Treeviewtypecut.Nodes.Add(_data.cutting.AllElementNames);
            Treeviewtypecut.TopNode.Expand();
            Treeviewtypecut.CheckBoxes = true;
        }

        private void FrmCutvoidbytypes_Load(object sender, EventArgs e)
        {
            Datalisttype();
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
        private void btn_cut_Click(object sender, EventArgs e)
        {
            cut = true;
            _data.cutting.SelectElements();
            list = _data.cutting.SelectedElement;
            Close();
        }

        private void Treeviewtypecut_AfterCheck(object sender, TreeViewEventArgs e)
        {
            CheckNode(e.Node, e.Node.Checked);
        }

        private void btn_uncut_Click(object sender, EventArgs e)
        {
            uncut = true;
            _data.cutting.SelectElements();
            list = _data.cutting.SelectedElement;
            Close();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
