using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;


namespace TVDCEG.ExtensionTool
{
	public partial class frmFindSheetContainLegend : Form
	{
		private DataSource m_data;

		private IList<LSObject> listObjects = new List<LSObject>();

		private string find;

		public bool isTrue = false;

		public int idToGo;

		public string txt = string.Empty;
		public frmFindSheetContainLegend(DataSource data)
		{
			this.m_data = data;
			this.InitializeComponent();
		}

		private void frmFindSheetContainLegend_Load(object sender, EventArgs e)
		{
			this.listObjects = this.m_data.getObjecs;
			bool flag = this.m_data.legendVP != null;
			if (flag)
			{
				this.textBox1.Text = this.m_data.legendVP.Name;
				IEnumerable<LSObject> source = from x in this.listObjects
				where x.LegendName.Equals(this.textBox1.Text)
				select x;
				this.dataGridView.DataSource = source.ToList<LSObject>();
				this.dataGridView.Columns["LegendName"].Visible = false;
				this.dataGridView.Columns["IdLegend"].Visible = false;
				this.dataGridView.Columns["IdViewSheet"].Visible = false;
			}
			IEnumerable<string> source2 = from x in this.listObjects
			where x.LegendName.Contains(this.textBox1.Text)
			select x.LegendName;
			IEnumerable<string> source3 = source2.Distinct<string>();
			this.listBox1.DataSource = source3.ToList<string>();
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.txt = this.listBox1.SelectedItem.ToString();
			IEnumerable<LSObject> source = from x in this.listObjects
			where x.LegendName.Equals(this.txt)
			select x;
			this.dataGridView.DataSource = source.ToList<LSObject>();
			this.dataGridView.Columns["LegendName"].Visible = false;
			this.dataGridView.Columns["IdLegend"].Visible = false;
			this.dataGridView.Columns["IdViewSheet"].Visible = false;
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			this.listObjects = this.m_data.getObjecs;
			IEnumerable<string> source = from x in this.listObjects
			where x.LegendName.Contains(this.textBox1.Text)
			select x.LegendName;
			IEnumerable<string> source2 = source.Distinct<string>();
			this.listBox1.DataSource = source2.ToList<string>();
		}

		private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
		}

		private void button1_Click(object sender, EventArgs e)
		{
			this.find = this.listBox1.SelectedItem.ToString();
		}

		private void listBox1_DoubleClick(object sender, EventArgs e)
		{
		}

		private void dataGridView_MouseClick(object sender, MouseEventArgs e)
		{
		}

		private void goToSheetToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			this.isTrue = true;
			int rowIndex = this.dataGridView.SelectedCells[0].RowIndex;
			int num = int.Parse(this.dataGridView.SelectedRows[0].Cells[4].Value.ToString());
			this.idToGo = num;
			base.Close();
		}

		private void dataGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{

		}
		private void listBox1_MouseClick(object sender, MouseEventArgs e)
		{
		}
	}
}
