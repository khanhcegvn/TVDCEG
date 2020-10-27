using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TVDCEG
{
    public partial class FrmCheckElementWorkshare : Form
    {
        private CheckElementWorkshareCmd _data;
        public FrmCheckElementWorkshare(CheckElementWorkshareCmd data)
        {
            _data = data;
            InitializeComponent();
            LoadData();
        }

        private void FrmCheckElementWorkshare_Load(object sender, EventArgs e)
        {

        }
        private void LoadData()
        {
            foreach (var item in _data.listceg)
            {
                int index = dataView.Rows.Add();
                DataGridViewRow dataGridViewRow = dataView.Rows[index];
                DataGridViewTextBoxCell dataGridViewTextBoxCell1 = dataGridViewRow.Cells[Name.Name] as DataGridViewTextBoxCell;
                if (dataGridViewTextBoxCell1 != null)
                {
                    dataGridViewTextBoxCell1.Value = item.Name;
                }
                DataGridViewTextBoxCell dataGridViewTextBoxCell2 = dataGridViewRow.Cells[Id.Name] as DataGridViewTextBoxCell;
                if (dataGridViewTextBoxCell2 != null)
                {
                    dataGridViewTextBoxCell2.Value = item.Id;
                }
                DataGridViewTextBoxCell dataGridViewTextBoxCell3 = dataGridViewRow.Cells[Createby.Name] as DataGridViewTextBoxCell;
                if (dataGridViewTextBoxCell3 != null)
                {
                    dataGridViewTextBoxCell3.Value = item.Creater;
                }
                DataGridViewTextBoxCell dataGridViewTextBoxCell5 = dataGridViewRow.Cells[Lastchangeby.Name] as DataGridViewTextBoxCell;
                if (dataGridViewTextBoxCell5 != null)
                {
                    dataGridViewTextBoxCell5.Value = item.LastChangeBy;
                }
                DataGridViewTextBoxCell dataGridViewTextBoxCell6 = dataGridViewRow.Cells[Owner.Name] as DataGridViewTextBoxCell;
                if (dataGridViewTextBoxCell6 != null)
                {
                    dataGridViewTextBoxCell6.Value = item.Owner;
                }
                DataGridViewTextBoxCell dataGridViewTextBoxCell4 = dataGridViewRow.Cells[Id.Name] as DataGridViewTextBoxCell;
                if (dataGridViewTextBoxCell4 != null)
                {
                    dataGridViewTextBoxCell4.Value = item.Id;
                }
            }
        }
    }
}
