using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using a=Autodesk.Revit.DB;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TVDCEG
{
    public partial class FrmPlaceSymbolonGravity : Form
    {
        private PlaceSymbolongravityFacecmd _data;
        private a.Document _doc;
        public FrmPlaceSymbolonGravity(PlaceSymbolongravityFacecmd data, a.Document doc)
        {
            _doc = doc;
            _data = data;
            InitializeComponent();
            Show();
        }
        private void Show()
        {
            cbb_symbol.DataSource = _data.listsymbol;
            cbb_symbol.DisplayMember = "Name";
        }
       
        private void FrmPlaceSymbolonGravity_Load(object sender, EventArgs e)
        {

        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            var symbol = cbb_symbol.SelectedItem as a.FamilySymbol;
            _data.Placesymbol(_doc, _data.curves, symbol);
            Close();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
