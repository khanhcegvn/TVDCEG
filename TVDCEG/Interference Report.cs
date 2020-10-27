using Autodesk.Revit.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using a = Autodesk.Revit.DB;

namespace TVDCEG
{
    public partial class Interference_Report : Form
    {
        private CheckIntersectcmd _data;
        private a.Document _doc;
        public Dictionary<string, List<a.ElementId>> diclist = new Dictionary<string, List<a.ElementId>>();
        private List<a.FamilyInstance> newlist = new List<a.FamilyInstance>();
        private List<string> listvalues = new List<string>();
        private FrmCheck _frmCheck;
        public double s1;
        public ExternalEvent ExEvent { get; set; }
        public Interference_Report(CheckIntersectcmd data, a.Document doc, FrmCheck frmCheck)
        {
            _data = data;
            _doc = doc;
            _frmCheck = frmCheck;
            InitializeComponent();
            Removevaluecontains();
        }

        private void OnButtonClicked()
        {
            int num = (int)this.ExEvent.Raise();
        }
        private void Show_Click(object sender, EventArgs e)
        {
            s1 = 1;
            _data.listshow.Clear();
            var y = listBoxshow.SelectedItems.Cast<string>().ToList();
            _data.listshow = _data.Stringtoelementid(y);
            this.OnButtonClicked();
        }

        private void listBoxshow_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Interference_Report_Load(object sender, EventArgs e)
        {
            //Showall();
        }
        //private void Fillterelement()
        //{
        //    try
        //    {
        //        string type1 = comboBox_type1.SelectedItem.ToString();
        //        string type2 = comboBox_type2.SelectedItem.ToString();
        //        listBoxshow.Items.Clear();
        //        List<string> list = new List<string>();
        //        if (type1 == "All")
        //        {
        //            listvalues.ForEach(y => list.Add(y));
        //            list.ForEach(x => listBoxshow.Items.Add(x));
        //        }
        //        if (type1 != "All" && type2 == "All")
        //        {
        //            foreach (var item2 in listvalues)
        //            {
        //                if (item2.Split(':').ToList()[0].Contains(type1))
        //                {
        //                    list.Add(item2);
        //                }
        //            }
        //            list.ForEach(x => listBoxshow.Items.Add(x));
        //        }
        //        if (type1 != "All" && type2 != "All")
        //        {
        //            foreach (var item2 in listvalues)
        //            {
        //                if (item2.Split(':').ToList()[0].Contains(type1) && item2.Split(':').ToList()[1].Contains(type2))
        //                {
        //                    list.Add(item2);
        //                }
        //            }
        //            list.ForEach(x => listBoxshow.Items.Add(x));
        //        }
        //        if (type1 == "All" && type2 != "All")
        //        {
        //            foreach (var item2 in listvalues)
        //            {
        //                if (item2.Split(':').ToList()[1].Contains(type2))
        //                {
        //                    list.Add(item2);
        //                }
        //            }
        //            list.ForEach(x => listBoxshow.Items.Add(x));
        //        }
        //    }
        //    catch
        //    {

        //    }
        //}
        //private void Showall()
        //{
        //    Datacombobox_type1();
        //    Datacombobox_type2();
        //}
        //private void Datacombobox_type1()
        //{
        //    comboBox_type1.Items.Clear();
        //    comboBox_type1.Items.Add("All");
        //    _data.dic.Keys.ToList().ForEach(x => comboBox_type1.Items.Add(x));
        //    comboBox_type1.DisplayMember = "Name";
        //    comboBox_type1.Text = "All";
        //}
        //private void Datacombobox_type2()
        //{
        //    comboBox_type2.Items.Clear();
        //    comboBox_type2.Items.Add("All");
        //    _data.dic.Keys.ToList().ForEach(x => comboBox_type2.Items.Add(x));
        //    comboBox_type2.DisplayMember = "Name";
        //    comboBox_type2.Text = "All";
        //}
        private void Export_Click(object sender, EventArgs e)
        {
            string Assembly = _frmCheck.Assemblyselectedname + "-" + "(Clash)";
            string json = JsonConvert.SerializeObject(listvalues, Formatting.Indented);
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.FileName = Assembly;
            saveFileDialog.DefaultExt = "txt";
            saveFileDialog.Filter = "txt files  (*.txt) |*.txt";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Stream filestream = saveFileDialog.OpenFile();
                StreamWriter sw = new StreamWriter(filestream);
                sw.Write(json);
                sw.Close();
                filestream.Close();
            }
            Close();
        }
        private void Removevaluecontains()
        {
            List<string> list = new List<string>();
            _data.dic.Keys.ToList().ForEach(x=>listvalues.Add(x));
            listvalues.ForEach(x => listBoxshow.Items.Add(x));
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        //private void comboBox_type1_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    Fillterelement();
        //}
        //private void comboBox_type2_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    Fillterelement();
        //}

        private void btn_Refesh_Click(object sender, EventArgs e)
        {
            s1 = 2;
            this.OnButtonClicked();
            Close();
        }
    }
}
