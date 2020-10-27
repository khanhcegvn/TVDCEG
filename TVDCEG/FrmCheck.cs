using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using a = Autodesk.Revit.DB;
using b = Autodesk.Revit.UI;


namespace TVDCEG
{
    public partial class FrmCheck : Form
    {
        public ICollection<a.ElementId> listpart = new List<a.ElementId>();
        public ICollection<a.ElementId> idscheck = new List<a.ElementId>();
        private CheckIntersectcmd _data;
        private a.Document _doc;
        public a.AssemblyInstance r2 = null;
        public string Assemblyselectedname;
        public b.UIDocument uidoc;
        public bool checkboxRebar;
        public bool checkboxEmbed;
        public bool checkboxStrand;
        public bool checkboxLifting;
        public FrmCheck(CheckIntersectcmd data, a.Document doc)
        {
            _doc = doc;
            _data = data;
            InitializeComponent();
        }
        List<Data> newlist = new List<Data>();
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void FrmCheck_Load(object sender, EventArgs e)
        {
            listBoxAssembly.DataSource = _data.ListAssembly;
            listBoxAssembly.DisplayMember = "Assemblyname";
            listBoxAssembly.ValueMember = "Assemblyid";
        }
        private void bnt_ok_Click(object sender, EventArgs e)
        {
            var i = listBoxAssembly.SelectedItems.Cast<Data>();
            var i2 = (from x in i select x.Assemblyid).ToList();
            foreach (var r in i2)
            {
                r2 = _data.doc.GetElement(r) as a.AssemblyInstance;
                Assemblyselectedname = r2.AssemblyTypeName;
            }
            Close();
        }
        private void listBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void Searchtxt_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            List<Data> newlist = _data.ListAssembly.OrderBy(o => o.Assemblyname).ToList();
            List<Data> newlist1 = new List<Data>();
            foreach (var dt in newlist)
            {
                if (dt.Assemblyname.ToUpper().Contains(SearchtextBox.Text))
                {
                    newlist1.Add(dt);
                }
            }
            listBoxAssembly.DataSource = null;
            listBoxAssembly.DataSource = newlist1;
            listBoxAssembly.DisplayMember = "Assemblyname";
            listBoxAssembly.ValueMember = "Assemblyid";
        }

        private void checkBox_REBAR_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox_REBAR.Checked)
            {
                checkboxRebar = true;
            }
            else
            {
                checkboxRebar = false;
            }
        }

        private void checkBox_EMBED_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox_EMBED.Checked)
            {
                checkboxEmbed = true;
            }
            else
            {
                checkboxEmbed = false;
            }
        }

        private void checkBox_STRAND_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox_STRAND.Checked)
            {
                checkboxStrand = true;
            }
            else
            {
                checkboxStrand = false;
            }
        }

        private void checkBox_Lifting_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_Lifting.Checked)
            {
                checkboxLifting = true;
            }
            else
            {
                checkboxLifting = false;
            }
        }
        private void Managercheckbox()
        {
            List<CheckBox> checkBoxes = new List<CheckBox>();
            var listcheckbox = this.groupBox1.Controls.OfType<CheckBox>().ToList();
            foreach (var cb in listcheckbox)
            {
                if(!cb.Checked)
                {
                    cb.Checked = true;
                }
                else
                {
                    cb.Checked = false;
                }
            }
        }
        private void btn_Invert_Click(object sender, EventArgs e)
        {
            Managercheckbox();
        }
    }
}
