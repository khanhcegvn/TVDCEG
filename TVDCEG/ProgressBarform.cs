using System;
using System.Windows.Forms;

namespace TVDCEG
{
    public partial class ProgressBarform : Form
    {
        public bool iscontinue { get; set; }
        private int _max;
        public string bottommessage;
        public string topmessage;
        private string titlename;
        public ProgressBarform(int max, string title)
        {
            titlename = title;
            _max = max;
            iscontinue = true;
            InitializeComponent();
        }
        private void ProgressBarform_Load(object sender, EventArgs e)
        {
            progressBar1.Minimum = 0;
            progressBar1.Maximum = _max;
        }

        private void so_Click(object sender, EventArgs e)
        {

        }
        public void giatri()
        {
            try
            {
                ++progressBar1.Value;
                so.Text = Math.Round((double)100 * progressBar1.Value / _max, 0).ToString() + "%";
                Text = titlename + " " + progressBar1.Value.ToString() + "/" + _max;
            }
            catch (Exception)
            {

            }
            Application.DoEvents();
        }
        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void dongho_Tick(object sender, EventArgs e)
        {

        }

        private void btn_Abort_Click(object sender, EventArgs e)
        {
            iscontinue = false;
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
