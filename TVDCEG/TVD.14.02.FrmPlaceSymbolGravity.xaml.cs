using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using Autodesk.Revit.DB;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TVDCEG
{
    /// <summary>
    /// Interaction logic for FrmPlaceSymbolGravity.xaml
    /// </summary>
    public partial class FrmPlaceSymbolGravity : Window
    {
        private PlaceSymbolongravityFacecmd _data;
        private Document _doc;
        private FamilySymbol symbol
        {
            get;
            set;
        }
        public FrmPlaceSymbolGravity(PlaceSymbolongravityFacecmd data,Document doc)
        {
            _doc = doc;
            _data = data;
            InitializeComponent();
            Show();
        }
        private void Show()
        {
            Cbb_symbol.ItemsSource = _data.listsymbol;
            Cbb_symbol.DisplayMemberPath = "Name";
        }
        private void btn_OK_Click(object sender, RoutedEventArgs e)
        {
            var symbol = Cbb_symbol.SelectedItem as FamilySymbol;
            _data.Placesymbol(_doc, _data.curves, symbol);
            //_data.Placesymbol(_doc, _data.center, symbol);
            _data.Setting.Symbolongravity = symbol.Name;
            _data.Setting.SaveSetting();
            Close();
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Cbb_symbol.Text = (from x in _data.listsymbol where x.Name == _data.Setting.Symbolongravity select x).Cast<FamilySymbol>().First().Name;
            }
            catch
            {

            }
        }
    }
}
