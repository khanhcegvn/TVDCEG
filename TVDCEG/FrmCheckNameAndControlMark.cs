using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TVDCEG
{
    public partial class FrmCheckNameAndControlMark : Form
    {
        private Checkfamilynameandcontrolmarkcmd _data;
        private List<INF_Part> iNF_Parts = new List<INF_Part>();
        public FrmCheckNameAndControlMark(Checkfamilynameandcontrolmarkcmd data)
        {
            _data = data;
            InitializeComponent();
        }

        private void FrmCheckNameAndControlMark_Load(object sender, EventArgs e)
        {
            List<string> list = new List<string>(_data.listpart.Keys);
            //list.Sort();
            //foreach(var item in _data.listpart)
            //{
            //    INF_Part iNF_ = new INF_Part()
            //    {
            //        Connection = item.Value.Connection,
            //        PartName = item.Key,
            //        Description=item.Value.Description,
            //        ProductName=item.Value.ProductName
            //    };
            //    iNF_Parts.Add(iNF_);
            //}
            //dataGridView1.DataSource = iNF_Parts;
            foreach (string item in list)
            {
                int index = dataGridView1.Rows.Add();
                DataGridViewRow dataGridViewRow = dataGridView1.Rows[index];
                DataGridViewTextBoxCell dataGridViewTextBoxCell = dataGridViewRow.Cells[partName.Name] as DataGridViewTextBoxCell;
                if (dataGridViewTextBoxCell != null)
                {
                    dataGridViewTextBoxCell.Value = item;
                }
                DataGridViewTextBoxCell dataGridViewTextBoxCell2 = dataGridViewRow.Cells[Description.Name] as DataGridViewTextBoxCell;
                if (dataGridViewTextBoxCell2 != null)
                {
                    dataGridViewTextBoxCell2.Value = _data.listpart[item].Description;
                }
                DataGridViewTextBoxCell dataGridViewTextBoxCell3 = dataGridViewRow.Cells[Connection.Name] as DataGridViewTextBoxCell;
                if (dataGridViewTextBoxCell3 != null)
                {
                    dataGridViewTextBoxCell3.Value = _data.listpart[item].Connection;
                }
                DataGridViewTextBoxCell dataGridViewTextBoxCell4 = dataGridViewRow.Cells[Product.Name] as DataGridViewTextBoxCell;
                if (dataGridViewTextBoxCell4 != null)
                {
                    dataGridViewTextBoxCell4.Value = _data.listpart[item].ProductName;
                }
                DataGridViewTextBoxCell dataGridViewTextBoxCell5 = dataGridViewRow.Cells[PartOwner.Name] as DataGridViewTextBoxCell;
                if (dataGridViewTextBoxCell5 != null)
                {
                    dataGridViewTextBoxCell5.Value = _data.listpart[item].PartOwner;
                }
            }
        }
        public class INF_Part
        {
            public string PartName { get; set; }
            public string Description { get; set; }
            public string Connection { get; set; }
            public string ProductName { get; set; }
        }

        private void btn_Export_Click(object sender, EventArgs e)
        {
            Checkpartinfo.Instance.ExportExcel(_data.listpart, "");
        }
    }
}
