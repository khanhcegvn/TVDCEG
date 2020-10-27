using System;
using System.Windows.Forms;
using System.Collections.Generic;
using a = Autodesk.Revit.DB;
using c = Autodesk.Revit.ApplicationServices;


namespace TVDCEG
{
    public partial class FrmModify : Form
    {
        private AddParameterFamily _data;
        private a.Document _doc;
        private c.Application _app;
        private Dictionary<string, string> dic = new Dictionary<string, string>();
        public FrmModify(AddParameterFamily data, a.Document doc, c.Application app)
        {
            _data = data;
            _doc = doc;
            _app = app;
            InitializeComponent();
        }

        private void FrmModify_Load(object sender, EventArgs e)
        {

        }

        private void Btn_OK_Click(object sender, EventArgs e)
        {
            lbr ORI = new lbr();
            if (EMBEDSTANDARD.Checked)
            {
                ORI.RemoveShareParameter(_doc,ref dic);
                ORI.EmbedStandard(_doc, _app,dic);
            }
            if (EMBEDSTANDARD_finish.Checked)
            {
                ORI.RemoveShareParameter(_doc,ref dic);
                ORI.EmbedStandard_finish(_doc, _app,dic);
            }
            if (EMBEDCUSTOM.Checked)
            {
                ORI.RemoveShareParameter(_doc,ref dic);
                ORI.EmbedCustom(_doc, _app,dic);
            }
            if (EMBEDCUSTOM_FINISH.Checked)
            {
                ORI.RemoveShareParameter(_doc,ref dic);
                ORI.EmbedCustom_finish(_doc, _app,dic);
            }
            if (CIPSTANDARD.Checked)
            {
                ORI.RemoveShareParameter(_doc,ref dic);
                ORI.CIPSTANDARD(_doc, _app,dic);
            }
            if (CIPCUSTOM.Checked)
            {
                ORI.RemoveShareParameter(_doc,ref dic);
                ORI.CIPCUSTOM(_doc, _app,dic);
            }
            if (ERECTIONSTANDARD.Checked)
            {
                ORI.RemoveShareParameter(_doc,ref dic);
                ORI.ERECTIONSTANDARD(_doc, _app,dic);
            }
            if (ERECTIONSTANDARD_FINISH.Checked)
            {
                ORI.RemoveShareParameter(_doc,ref dic);
                ORI.ERCTIONSTANDARD_finish(_doc, _app,dic);
            }
            if (Keep_data.Checked)
            {
                ORI.RemoveShareParameterkeep(_doc);
            }
            if(AddPanew.Checked)
            {
                ORI.AddPAnew(_doc, _app, dic);
            }

            Close();

        }

        private void EMBED_CUSTTOM_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void radioButton1_CheckedChanged_1(object sender, EventArgs e)
        {

        }

        private void AddPanew_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
