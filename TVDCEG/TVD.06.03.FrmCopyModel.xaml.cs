using System.Windows;
using System.Windows.Markup;
using a = Autodesk.Revit.DB;

namespace TVDCEG
{
    /// <summary>
    /// Interaction logic for FrmCopyModel.xaml
    /// </summary>
    public partial class FrmCopyModel : Window, IComponentConnector
    {
        private CopyModelcmd _data;
        private a.Document _doc;
        public FrmCopyModel(CopyModelcmd data, a.Document doc)
        {
            _doc = doc;
            _data = data;
            InitializeComponent();
            Showall();
        }
        private void Showall()
        {
            DataComboboxSource();
            DataComboboxtarget();
        }
        private void DataComboboxSource()
        {
            tbl_datasource.Text = _data.uidoc.Application.ActiveUIDocument.Document.Title;
        }
        private void DataComboboxtarget()
        {
            cbb_Target.Items.Clear();
            _data.listdoc.ForEach(x => cbb_Target.Items.Add(x));
            cbb_Target.DisplayMemberPath = "Title";
        }

        private void btn_OK_Click(object sender, RoutedEventArgs e)
        {
            var source = _data.uidoc.Application.ActiveUIDocument.Document;
            var target = cbb_Target.SelectedItem as a.Document;
            CopyModel.Instance.Copyelement(source, target, _data.ids);
            Close();
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
