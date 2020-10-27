using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using a = Autodesk.Revit.DB;
using System.Windows.Forms;
using Autodesk.Revit.Creation;

namespace TVDCEG
{
    public partial class FrmCheckpartdraw : Form
    {
        private a.Document _doc;
        private Checkpartdrawcmd _data;
        public FrmCheckpartdraw(a.Document doc,Checkpartdrawcmd data)
        {
            _doc = doc;
            _data = data;
            InitializeComponent();
        }
        private void LoadData()
        {
            FillDatatodatagrid(_data.dic);
        }
        private void FillDatatodatagrid(Dictionary<string, Partinfo> dicsource)
        {
            List<string> list = new List<string>(dicsource.Keys).ToList();
            list.Sort();
            dataview.Rows.Clear();
            string text = this.txt_search.Text;
            foreach (string item in list)
            {
                bool flag2 = !item.Contains(text);
                if(!flag2)
                {
                    int index = dataview.Rows.Add();
                    DataGridViewRow dataGridViewRow = dataview.Rows[index];
                    DataGridViewTextBoxCell dataGridViewTextBoxCell = dataGridViewRow.Cells[PartName.Name] as DataGridViewTextBoxCell;
                    if (dataGridViewTextBoxCell != null)
                    {
                        dataGridViewTextBoxCell.Value = item;
                    }
                    DataGridViewTextBoxCell dataGridViewTextBoxCell2 = dataGridViewRow.Cells[Description.Name] as DataGridViewTextBoxCell;
                    if (dataGridViewTextBoxCell2 != null)
                    {
                        dataGridViewTextBoxCell2.Value = dicsource[item].Description;
                    }
                    DataGridViewTextBoxCell dataGridViewTextBoxCell3 = dataGridViewRow.Cells[Connection.Name] as DataGridViewTextBoxCell;
                    if (dataGridViewTextBoxCell3 != null)
                    {
                        dataGridViewTextBoxCell3.Value = dicsource[item].Connection;
                    }
                    DataGridViewTextBoxCell dataGridViewTextBoxCell4 = dataGridViewRow.Cells[Product.Name] as DataGridViewTextBoxCell;
                    if (dataGridViewTextBoxCell4 != null)
                    {
                        dataGridViewTextBoxCell4.Value = dicsource[item].ProductName;
                    }
                    DataGridViewTextBoxCell dataGridViewTextBoxCell5 = dataGridViewRow.Cells[Draw.Name] as DataGridViewTextBoxCell;
                    if (dataGridViewTextBoxCell5 != null)
                    {
                        dataGridViewTextBoxCell5.Value = dicsource[item].Draw;
                    }
                }
            }
            Fillcolor();
        }
        private void Fillcolor()
        {
            foreach (DataGridViewRow row in dataview.Rows)
            {
                if (row.Cells["Draw"].Value != null)
                {
                    if (row.Cells["Draw"].Value.Equals("true"))
                    {
                        row.DefaultCellStyle.BackColor = System.Drawing.Color.CornflowerBlue;
                    }
                }
            }
        }
        private void FrmCheckpartdraw_Load(object sender, EventArgs e)
        {
            LoadData();
            Fillcolor();
        }
       
        private void txt_search_TextChanged(object sender, EventArgs e)
        {
            FillDatatodatagrid(_data.dic);
        }

        private void btn_export_Click(object sender, EventArgs e)
        {
            Checkpartdraw.Instance.ExportExcel(_data.dic,"");
            Close();
        }
    }
}
