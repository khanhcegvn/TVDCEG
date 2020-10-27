
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using a = Autodesk.Revit.DB;


namespace TVDCEG
{
    public partial class FrmAddsurfixsheet : Form
    {
        private Addsurfixsheetcmd _data;
        public string prefix = null;
        public string suffix = null;
        public string SheetNumber_ = null;
        public bool checkvalue = true;
        public List<a.ViewSheet> listsheet1 = new List<a.ViewSheet>();
        private Dictionary<string, a.ViewSheet> dic_viewsheet = new Dictionary<string, a.ViewSheet>();
        private a.Document _doc;
        public FrmAddsurfixsheet(Addsurfixsheetcmd data, a.Document doc)
        {
            _data = data;
            _doc = doc;
            InitializeComponent();
            showall();
            listBox_sheet.DisplayMember = "Key";
        }

        private void listBox_sheet_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void showall()
        {
            Listboxview(_data.ListSheet);
        }
        private void Listboxview(Dictionary<string, a.ViewSheet> views)
        {
            listBox_sheet.Items.Clear();
            foreach (var i in views)
            {
                listBox_sheet.Items.Add(i);
            }
        }
        private void FilterViews()
        {
            listBox_sheet.Items.Clear();
            var views = new Dictionary<string, a.ViewSheet>();
            foreach (var view in _data.ListSheet)
            {
                if (view.Key.ToUpper().Contains(textBox1.Text.ToUpper()) || view.Key.ToUpper().Contains(textBox1.Text.ToUpper()))
                {
                    views.Add(view.Key, view.Value);
                }
            }
            Listboxview(views);
        }

        [Obsolete]
        private void button1_Click(object sender, EventArgs e)
        {
            checkvalue = true;
            var t = listBox_sheet.SelectedItems;
            foreach (KeyValuePair<string, a.ViewSheet> keyValue in t)
            {
                listsheet1.Add(keyValue.Value);
            }
            prefix = textBox_prefix.Text.ToString();
            suffix = textBox_suffix.Text.ToString();
            SheetNumber_ = textBox_SheetNumber.Text.ToString();
            Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            FilterViews();
        }

        private void FrmAddsurfixsheet_Load(object sender, EventArgs e)
        {

        }

        private void Button2_Click(object sender, EventArgs e)
        {
            checkvalue = false;
            Close();
        }
    }
}
