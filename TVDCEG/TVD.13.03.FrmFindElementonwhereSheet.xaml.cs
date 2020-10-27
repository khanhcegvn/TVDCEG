using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Markup;

namespace TVDCEG
{
    /// <summary>
    /// Interaction logic for FrmFindElementonwhereSheet.xaml
    /// </summary>
    public partial class FrmFindElementonwhereSheet : Window, IComponentConnector
    {
        private FindElementonWhereSheetcmd _data;
        public ObservableCollection<ElementFindclass> listObject;
        public FrmFindElementonwhereSheet(FindElementonWhereSheetcmd data)
        {
            _data = data;
            InitializeComponent();
            listObject = new ObservableCollection<ElementFindclass>(_data.elementFindclasses);
            Listviewelement.ItemsSource = listObject;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void btn_Gotosheet_Click(object sender, RoutedEventArgs e)
        {
            var viewsheet = Listviewelement.SelectedItem as ElementFindclass;
            if (viewsheet != null)
            {
                FindElementonWhereSheet.Instance.GotoSheet(_data.uidoc, viewsheet);
                Close();
            }
            else
            {
                MessageBox.Show("Please select element", "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void FilterViews()
        {
            var listelement = new ObservableCollection<ElementFindclass>();
            foreach (var ele in listObject)
            {
                if (ele.Control_mark.ToUpper().Contains(txt_Search.Text.ToUpper()))
                {
                    listelement.Add(ele);
                }
            }
            Listboxview(listelement);
        }
        private void Listboxview(ObservableCollection<ElementFindclass> list)
        {
            Listviewelement.ItemsSource = list;
        }

        private void txt_Search_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            FilterViews();
        }

        private void btn_Export_Click(object sender, RoutedEventArgs e)
        {
            FindElementonWhereSheet.Instance.Export(listObject);
            Close();
        }
    }
}
