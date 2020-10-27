using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using TVDCEG.CEG_INFOR;
using a = Autodesk.Revit.DB;

namespace TVDCEG
{
    public partial class FrmCheckControlNumber : System.Windows.Forms.Form
    {
        private a.Document _doc;
        private CheckControlNumberCmd _data;
        public List<CEG_Product> listsearch = new List<CEG_Product>();
        public CegProduct productsource;

        public ExternalEvent ExEvent { get; set; }
        private void OnButtonClicked()
        {
            int num = (int)this.ExEvent.Raise();
        }
        public FrmCheckControlNumber(CheckControlNumberCmd data,a.Document doc)
        {
            _doc = doc;
            _data = data;
            InitializeComponent();
            LoadData();
        }
        private void LoadData()
        {
            _data.list = CheckControlNumber.Instance.GetallProducts(_doc);
            FillDatatodatagrid(_data.list);
        }
        private void FillDatatodatagrid(List<CEG_Product> list)
        {
            this.dataView.DataSource = new BindingSource { DataSource = list };
            dataView.AutoResizeColumns();
            dataView.Columns[8].Visible = false;
            for (int i = 0; i < dataView.Columns.Count - 1; i++)
            {
                dataView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }
        private void Fillcolor()
        {
            foreach (DataGridViewRow row in dataView.Rows)
            {
                if (string.IsNullOrEmpty(row.Cells[1].Value.ToString()))
                {
                    row.DefaultCellStyle.BackColor = System.Drawing.Color.CornflowerBlue;
                }
            }
        }
        private void FrmCheckControlNumber_Load(object sender, EventArgs e)
        {
            Fillcolor();
        }

        private void ckb_duplicateControlnumber_CheckedChanged(object sender, EventArgs e)
        {
            Dictionary<string,List<DataGridViewRow>> dic = new Dictionary<string,List<DataGridViewRow>>();
            foreach (DataGridViewRow row in dataView.Rows)
            {
                if(dic.ContainsKey(row.Cells[1].Value.ToString()))
                {
                    dic[row.Cells[1].Value.ToString()].Add(row);
                }
                else
                {
                    dic.Add(row.Cells[1].Value.ToString(), new List<DataGridViewRow> { row });
                }
                row.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            }
            if(ckb_duplicateControlnumber.Checked)
            {
                foreach (var item in dic)
                {
                    if (item.Value.Count > 1)
                    {
                        foreach (var item2 in item.Value)
                        {
                            item2.DefaultCellStyle.BackColor = System.Drawing.Color.Chocolate;
                        }
                    }
                }
            }
            else
            {
                Fillcolor();
            }
        }

        private void btn_Select_Click(object sender, EventArgs e)
        {
            _data.flag = 0;
            _data.listproductid.Clear();
            if (dataView.SelectedRows.Count > 0)
            {
                for (int i = 0; i < dataView.SelectedRows.Count; i++)
                {
                    var ml = dataView.SelectedRows[i].Cells["Id"].Value.ToString();
                    int crtid = Convert.ToInt32(ml);
                    a.ElementId elementId = new a.ElementId(crtid);
                    _data.listproductid.Add(elementId);
                }
            }
            this.OnButtonClicked();
        }

        private void btn_increase_Click(object sender, EventArgs e)
        {
            _data.flag = 1;
            var form = new FrmIncrease(this);
            form.ShowDialog();
            List<int> ea = new List<int>();
            if (dataView.SelectedRows.Count > 0)
            {
                for (int i = 0; i < dataView.SelectedRows.Count; i++)
                {
                    var ml = dataView.SelectedRows[i].Index;
                    var k =Convert.ToInt32(dataView.SelectedRows[i].Cells[8].Value.ToString());
                    var item = (from x in _data.list where x.Id == k select x).First();
                    _data.listRenumber.Add(item);
                    //ea.Add(ml);
                }
            }
            //_data.number = ea.Last();
            _data.increase = form.increase;
            this.OnButtonClicked();
        }

        private void txt_search_TextChanged(object sender, EventArgs e)
        {
            Filteritems();
        }
        private void Filteritems()
        {
            dataView.Columns.Clear();
            var items = new List<CEG_Product>();
            foreach (var item in _data.list)
            {
                if (item.CONTROL_MARK.ToUpper().Contains(txt_search.Text.ToUpper())|| item.CONTROL_NUMBER.ToUpper().Contains(txt_search.Text.ToUpper()))
                {
                    items.Add(item);
                }
            }
            FillDatatodatagrid(items);
            Fillcolor();
        }
    }
}
