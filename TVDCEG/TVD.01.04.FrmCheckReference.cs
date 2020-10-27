using Autodesk.Revit.UI;
using System;
using System.Windows.Forms;
using a = Autodesk.Revit.DB;

namespace TVDCEG
{
    public partial class FrmCheckReference : Form
    {
        private CheckWorkPlanecmd _data;
        private a.Document _doc;
        public bool checklevelgrid = false;
        public ExternalEvent ExEvent { get; set; }
        private void OnButtonClicked()
        {
            int num = (int)this.ExEvent.Raise();
        }
        public FrmCheckReference(CheckWorkPlanecmd data, a.Document doc)
        {
            _data = data;
            _doc = doc;
            InitializeComponent();
        }
        private void FrmCheckReference_Load(object sender, EventArgs e)
        {
            var gh = UpdateGroupinstance.Instance.Filterlist(_data.listsource);
            foreach (var item in gh)
            {
                int index = dataGridView1.Rows.Add();
                DataGridViewRow dataGridViewRow = dataGridView1.Rows[index];
                DataGridViewTextBoxCell dataGridViewTextBoxCell = dataGridViewRow.Cells[Name.Name] as DataGridViewTextBoxCell;
                if (dataGridViewTextBoxCell != null)
                {
                    dataGridViewTextBoxCell.Value = item.Value.Name;
                }
                DataGridViewTextBoxCell dataGridViewTextBoxCell2 = dataGridViewRow.Cells[Id.Name] as DataGridViewTextBoxCell;
                if (dataGridViewTextBoxCell2 != null)
                {
                    dataGridViewTextBoxCell2.Value = item.Value.ID;
                }
                DataGridViewTextBoxCell dataGridViewTextBoxCell3 = dataGridViewRow.Cells[WorkPlane.Name] as DataGridViewTextBoxCell;
                if (dataGridViewTextBoxCell3 != null)
                {
                    dataGridViewTextBoxCell3.Value = item.Value.Workplane;
                }
                //dataGridView1.Columns[0].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                //dataGridView1.Columns[1].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                //dataGridView1.Columns[2].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                //dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            }
        }
        private void btn_export_Click(object sender, EventArgs e)
        {
            _data.Export(_data.listsource);
            Close();
        }
        private void btn_SelectElement_Click(object sender, EventArgs e)
        {
            _data.list.Clear();
            if (dataGridView1.SelectedRows.Count > 0)
            {
                for (int i = 0; i < dataGridView1.SelectedRows.Count; i++)
                {
                    var ml = dataGridView1.SelectedRows[i].Cells["Id"].Value.ToString();
                    string[] list = ml.Split(';');
                    foreach (var j in list)
                    {
                        int crtid = Convert.ToInt32(j);
                        a.ElementId elementId = new a.ElementId(crtid);
                        _data.list.Add(elementId);
                    }
                }
            }
            this.OnButtonClicked();
        }
        private void Updatedatagridview(bool check)
        {
            dataGridView1.Rows.Clear();
            _data.listsource.Clear();
            _data.Updateinfo(_data.source,check);
            var gh = UpdateGroupinstance.Instance.Filterlist(_data.listsource);
            foreach (var item in gh)
            {
                int index = dataGridView1.Rows.Add();
                DataGridViewRow dataGridViewRow = dataGridView1.Rows[index];
                DataGridViewTextBoxCell dataGridViewTextBoxCell = dataGridViewRow.Cells[Name.Name] as DataGridViewTextBoxCell;
                if (dataGridViewTextBoxCell != null)
                {
                    dataGridViewTextBoxCell.Value = item.Value.Name;
                }
                DataGridViewTextBoxCell dataGridViewTextBoxCell2 = dataGridViewRow.Cells[Id.Name] as DataGridViewTextBoxCell;
                if (dataGridViewTextBoxCell2 != null)
                {
                    dataGridViewTextBoxCell2.Value = item.Value.ID;
                }
                DataGridViewTextBoxCell dataGridViewTextBoxCell3 = dataGridViewRow.Cells[WorkPlane.Name] as DataGridViewTextBoxCell;
                if (dataGridViewTextBoxCell3 != null)
                {
                    dataGridViewTextBoxCell3.Value = item.Value.Workplane;
                }
            }
        }
        private void Searchworkplane()
        {
            
        }
        private void btn_Refresh_Click(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
            {
                checklevelgrid = true;
            }
            else
            {
                checklevelgrid = false;
            }
            Updatedatagridview(checklevelgrid);
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
