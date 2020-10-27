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
    public partial class FrmAlignRebar : Form
    {
		public string space;
		public bool check = false;
        public FrmAlignRebar()
        {
            InitializeComponent();
        }
		private void TxtBoxOnLostFocus(object sender, EventArgs eventArgs)
		{
			TextBox textBox = sender as TextBox;
			bool flag = textBox == null;
			if (!flag)
			{
				string text = textBox.Text;
				string text2;
				Ultis.UnitConvert.StringToFeetAndInches(text, out text2);
				textBox.Text = text2;
			}
		}

        private void FrmAlignRebar_Load(object sender, EventArgs e)
        {
			this.txt_space.LostFocus += this.TxtBoxOnLostFocus;
		}

        private void btn_OK_Click(object sender, EventArgs e)
        {
			space = txt_space.Text;
			check = true;
			Close();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
			Close();
        }
    }
}
