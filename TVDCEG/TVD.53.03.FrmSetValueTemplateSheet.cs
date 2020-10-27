using a = Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TVDCEG.Extension;
using Newtonsoft.Json;
namespace TVDCEG
{
    public partial class FrmSetValueTemplateSheet : Form
    {
        private SetValueTemplateSheetCmd _data;
        private a.Document _doc;
        public FrmSetValueTemplateSheet(SetValueTemplateSheetCmd data, a.Document doc)
        {
            _doc = doc;
            _data = data;
            InitializeComponent();
            Updatelistbox();
        }

        private void Load()
        {
            DirectoryInfo d = new DirectoryInfo(_data.Setting.GetFolderPath());
            var p = d.GetFiles().ToList();
            string filename = lb_view.SelectedItem.ToString();
            string pathfile = "";
            foreach (var item in p)
            {
                if (item.Name.Replace(".json", "") == filename)
                {
                    pathfile = item.FullName;
                    break;
                }
            }
            var json = File.ReadAllText(pathfile);
            CegParameterSet dictemp = JsonConvert.DeserializeObject<CegParameterSet>(json);
            a.ViewSheet viewSheet = _doc.ActiveView as a.ViewSheet;
            using (a.Transaction tran = new a.Transaction(_doc, "Set Parameter Sheet"))
            {
                tran.Start();
                foreach (var item in dictemp.Parameters)
                {
                    foreach (var item2 in _data.dic.Parameters)
                    {
                        if (item.Value.Name == item2.Value.Name)
                        {
                            a.Parameter pa = viewSheet.LookupParameter(item2.Value.Name);
                            if (pa != null)
                            {
                                a.InternalDefinition definition = pa.Definition as a.InternalDefinition;
                                if (definition.BuiltInParameter != a.BuiltInParameter.SHEET_NAME && definition.BuiltInParameter != a.BuiltInParameter.SHEET_NUMBER)
                                {
                                    switch (item2.Value.Type)
                                    {
                                        case a.StorageType.Double:
                                            pa.Set(item.Value.AsDouble);
                                            break;
                                        case a.StorageType.String:
                                            pa.Set(item.Value.AsString);
                                            break;
                                        case a.StorageType.Integer:
                                            pa.Set(item.Value.AsInteger);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
                tran.Commit();
            }
            Close();
        }
        private void Delete()
        {
            DirectoryInfo d = new DirectoryInfo(_data.Setting.GetFolderPath());
            var p = d.GetFiles().ToList();
            string filename = lb_view.SelectedItem.ToString();
            foreach (var item in p)
            {
                if (item.Name.Replace(".json", "") == filename)
                {
                    File.Delete(item.FullName);
                }
            }
            Updatelistbox();
        }
        private void Save()
        {
            var filename = txt_name.Text;
            if(!string.IsNullOrEmpty(filename))
            {
                DirectoryInfo d = new DirectoryInfo(_data.Setting.GetFolderPath());
                var p = d.GetFiles().ToList();
                List<string> list = new List<string>();
                p.ForEach(x => list.Add(x.Name));
                List<string> newlist = new List<string>();
                foreach (var item in list)
                {
                    var t = item.Replace(".json", "");
                    newlist.Add(t);
                }
                foreach (var item in newlist)
                {
                    if (filename == item)
                    {
                        File.Delete(_data.Setting.GetFolderPath() + "\\" + item);
                        break;
                    }
                }
                SetValueTemplateSheet.Instance.SaveTemplateSheet(_doc, _data.dic, filename, _data.Setting);
                Updatelistbox();
            }    
           else
            {
                MessageBox.Show("Enter name", "Error");
            }
        }
        private void Updatelistbox()
        {
            lb_view.Items.Clear();
            DirectoryInfo d = new DirectoryInfo(_data.Setting.GetFolderPath());
            var p = d.GetFiles().ToList();
            List<string> list = new List<string>();
            p.ForEach(x => list.Add(x.Name));
            List<string> newlist = new List<string>();
            foreach (var item in list)
            {
                var t = item.Replace(".json", "");
                newlist.Add(t);
            }
            newlist.ForEach(x => lb_view.Items.Add(x));
        }
        private void btn_save_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void btn_Load_Click(object sender, EventArgs e)
        {
            Load();
        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
            Delete();
        }

        private void lb_view_SelectedIndexChanged(object sender, EventArgs e)
        {
            string text = lb_view.SelectedItem.ToString();
            txt_name.Text = text;
        }
    }
}
