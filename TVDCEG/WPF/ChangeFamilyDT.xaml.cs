using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;

namespace TVDCEG.WPF
{
    /// <summary>
    /// Interaction logic for ChangeFamilyDT.xaml
    /// </summary>
    public partial class ChangeFamilyDT : Window, IComponentConnector
    {
        private ChangeFamilyDTcmd _data;
        private Document _doc;
        public FamilySymbol _symbaol = null;
        public ChangeFamilyDT(ChangeFamilyDTcmd data, Document doc)
        {
            _data = data;
            _doc = doc;
            InitializeComponent();
            comboboxview(data.LtfamilySymbols);
            Type_dt.DisplayMemberPath = "Name";
        }
        public void Dispose()
        {
            this.Close();
        }
        private void comboboxview(List<FamilySymbol> symbol)
        {
            Type_dt.Items.Clear();
            foreach (var i in symbol)
            {
                Type_dt.Items.Add(i);
            }
        }

        private void Btn_ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            _symbaol = Type_dt.SelectedValue as FamilySymbol;
            _data.Changefamily(_doc, _symbaol, _data.listdt);
            Close();
        }

        private void Type_dt_SelectionChanged()
        {

        }
    }
}
