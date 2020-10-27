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
    public partial class FrmSettingManagerSection : Form
    {
        private ManagerSectioncmd _data;
        private a.Document _doc;
        public Dictionary<string, List<a.Parameter>> dic = new Dictionary<string, List<a.Parameter>>();
        public FrmSettingManagerSection(ManagerSectioncmd data, a.Document doc)
        {
            _data = data;
            _doc = doc;
            InitializeComponent();
            LoadData();
        }
        private void LoadData()
        {
            dic = ManagerSection.Instance.GetFamilyAnnotionSymbol(_doc);
            cbb_FamilySymbol.DataSource = dic.Keys.ToList();
            cbb_FamilySymbol.DisplayMember = "Name";
        }
        private void GetData(string text)
        {
            List<string> ParameterNames = new List<string>();
            dic[text].ToList().ForEach(x => ParameterNames.Add(x.Definition.Name));
            cbb_Parameter.DataSource = ParameterNames;
        }
        private void FrmSetting_Load(object sender, EventArgs e)
        {
            try
            {
               cbb_FamilySymbol.Text = (from x in dic.Keys where x == _data.Setting.FamilySymbol select x).First();
            }
            catch
            {

            }
            try
            {
                var list = dic[cbb_FamilySymbol.Text];
                foreach (var item in list)
                {
                    if(item.Definition.Name==_data.Setting.Parameter)
                    {
                        cbb_Parameter.Text = item.Definition.Name;
                    }
                }
            }
            catch
            {

            }
        }

        private void cbb_FamilySymbol_SelectedIndexChanged(object sender, EventArgs e)
        {
            string text = cbb_FamilySymbol.Text;
            GetData(text);
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            _data.Setting.FamilySymbol = cbb_FamilySymbol.Text;
            _data.Setting.Parameter = cbb_Parameter.Text;
            _data.Setting.SaveSetting();
            Close();
        }
    }
}
