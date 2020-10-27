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
using a = Autodesk.Revit.DB;

namespace TVDCEG
{
    public partial class FrmManagerSection : System.Windows.Forms.Form
    {
        private ManagerSectioncmd _data;
        private a.Document _doc;
        private List<ObjectSheetManager> ValueGrid=new List<ObjectSheetManager>();
        public PreviewControl _pc;
        public ExternalEvent ExEvent { get; set; }
        public FrmManagerSection(ManagerSectioncmd data,a.Document doc)
        {
            _data = data;
            _doc = doc;
            InitializeComponent();
            Listboxview(_data.ViewModel.ListObjectSections);
        }
        private void OnButtonClicked()
        {
            int num = (int)this.ExEvent.Raise();
        }
        private void Listboxview(List<ObjectSectionManager> views)
        {
            lb_view.Items.Clear();
            foreach (var i in views)
            {
                lb_view.Items.Add(i);
            }
            lb_view.DisplayMember = "Name";
        }
        private void Loaddata(List<ObjectSheetManager> list)
        {
            foreach (var item in list)
            {
                int index = dataGridView1.Rows.Add();
                DataGridViewRow dataGridViewRow = dataGridView1.Rows[index];
                DataGridViewTextBoxCell dataGridViewTextBoxCell = dataGridViewRow.Cells[Sheets.Name] as DataGridViewTextBoxCell;
                if (dataGridViewTextBoxCell != null)
                {
                    dataGridViewTextBoxCell.Value = item.Name;
                }
                DataGridViewTextBoxCell dataGridViewTextBoxCell2 = dataGridViewRow.Cells[SheetId.Name] as DataGridViewTextBoxCell;
                if (dataGridViewTextBoxCell2 != null)
                {
                    dataGridViewTextBoxCell2.Value = item.Id;
                }
            }
        }
        private void Preview_Control(ElementId id)
        {
            PreviewControl pc = this._pc;
            if (pc != null)
            {
                pc.Dispose();
            }
            this._pc = new PreviewControl(_doc, id);
            this.previewHost.Child = this._pc;
        }

        private void FilterViews()
        {
            lb_view.Items.Clear();
            var views = new List<ObjectSectionManager>();
            foreach (var view in _data.ViewModel.ListObjectSections)
            {
                if (view.Name.ToUpper().Contains(txt_Search.Text.ToUpper()))
                {
                    views.Add(view);
                }
            }
            Listboxview(views);
        }

        private void txt_Search_TextChanged(object sender, EventArgs e)
        {
            FilterViews();
        }

        private void FrmManagerSection_Load(object sender, EventArgs e)
        {
            System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(this.btn_Run, "Run with new setting");
            System.Windows.Forms.ToolTip ToolTip2 = new System.Windows.Forms.ToolTip();
            ToolTip2.SetToolTip(this.btn_Setting, "New setting");
        }

        private void lb_view_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            ValueGrid.Clear();
            var item = lb_view.SelectedItem as ObjectSectionManager;
            foreach (var val in _data.ViewModel.dic3[item.Name] )
            {
                ValueGrid.Add(val);
            }
            Loaddata(ValueGrid);
            Preview_Control(item.Id);
        }

        private void btn_Setting_Click(object sender, EventArgs e)
        {
            var form = new FrmSettingManagerSection(_data, _doc);
            form.ShowDialog();
        }

        private void btn_Run_Click(object sender, EventArgs e)
        {
            this.OnButtonClicked();
            
            Close();
        }
    }
}
