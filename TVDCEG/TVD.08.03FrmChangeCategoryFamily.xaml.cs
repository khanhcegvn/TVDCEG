using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using a = Autodesk.Revit.DB;

namespace TVDCEG
{
    /// <summary>
    /// Interaction logic for FrmChangeCategoryFamily.xaml
    /// </summary>
    public partial class FrmChangeCategoryFamily : Window
    {
        private ChangeCategoryfamilycmd _data;
        public FrmChangeCategoryFamily(ChangeCategoryfamilycmd data)
        {
            _data = data;
            InitializeComponent();
        }
        private void ComboboxControl()
        {
            cbx_typecategory.Items.Clear();
            _data.dic.Keys.ToList().ForEach(x => cbx_typecategory.Items.Add(_data.dic[x]));
            cbx_typecategory.DisplayMemberPath = "Name";
        }
        private void Showcontrol()
        {
            ComboboxControl();
            ListboxControl(_data.listfamilysymbol);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Showcontrol();
        }
        private void ListboxControl(List<a.FamilySymbol> familySymbols)
        {
            lb_Category.Items.Clear();
            familySymbols.ForEach(x => lb_Category.Items.Add(x));
            lb_Category.DisplayMemberPath = "Name";
        }
        private void Updatelistbox()
        {
            List<a.FamilySymbol> newlist = new List<a.FamilySymbol>();
            newlist = (from a.FamilySymbol x in _data.listfamilysymbol where x.Name.ToUpper().Contains(txt_search.Text.ToUpper()) select x).ToList();
            ListboxControl(newlist);
        }

        private void txt_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            Updatelistbox();
        }

        private void btn_OK_Click(object sender, RoutedEventArgs e)
        {
            var items = lb_Category.SelectedItems.Cast<a.FamilySymbol>().ToList();
        }
    }
}
