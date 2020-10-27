using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using a = Autodesk.Revit.DB;
namespace TVDCEG
{
    public partial class RenameSheet : Form
    {
        private Renamesheetcmd _data;
        private a.Document _doc;
        public bool ttrue = false;
        public int Gotoid;
        public RenameSheet(Renamesheetcmd data, a.Document doc)
        {
            _doc = doc;
            _data = data;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            List<DataRename> SortedList = Renamesheetcmd.ListSheet.OrderBy(o => o.RAssemblyname).ToList();
            foreach (var vl in SortedList)
            {
                var n = dataGridView1.Rows.Add();
                dataGridView1.Rows[n].Cells[0].Value = vl.RAssemblyname;
                dataGridView1.Rows[n].Cells[1].Value = vl.SheetName;
                dataGridView1.Rows[n].Cells[2].Value = vl.SheetNumber;
                dataGridView1.Rows[n].Cells[Column4.Name].Value = vl.Sheet;

            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var lstdata = Renamesheetcmd.ListSheet;
            var listASs = (from x in lstdata select x.RAssemblyname).Distinct().ToList();
            a.Transaction transaction = new a.Transaction(_doc, "aa");

            transaction.Start();

            foreach (var assembly in listASs)
            {
                try
                {
                    //
                    var lstCungtenASs = from x in lstdata where x.RAssemblyname == assembly select x;
                    var i = 1;
                    foreach (var dt in lstCungtenASs)
                    {
                        var vs = _doc.GetElement(dt.Sheet) as a.ViewSheet;

                        vs.Name = assembly;
                        vs.SheetNumber = i.ToString();
                        i++;
                    }

                }
                catch
                {

                }

            }
            transaction.Commit();


            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void goToSheetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ttrue = true;
            var rowIndex = dataGridView1.SelectedCells[0].RowIndex;
            int y = Int32.Parse(dataGridView1.Rows[rowIndex].Cells[Column4.Name].Value.ToString());
            Gotoid = y;
            Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

            updatedgv();

        }
        void updatedgv()
        {
            dataGridView1.Rows.Clear();
            List<DataRename> SortedList = Renamesheetcmd.ListSheet.OrderBy(o => o.RAssemblyname).ToList();
            foreach (var vl in SortedList)
            {
                if (vl.SheetName.Contains(textBox1.Text))
                {
                    var n = dataGridView1.Rows.Add();
                    dataGridView1.Rows[n].Cells[0].Value = vl.RAssemblyname;
                    dataGridView1.Rows[n].Cells[1].Value = vl.SheetName;
                    dataGridView1.Rows[n].Cells[2].Value = vl.SheetNumber;
                }

            }
        }
    }
}
