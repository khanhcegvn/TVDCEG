using Autodesk.Revit.DB;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TVDCEG
{
    /// <summary>
    /// Interaction logic for FrmDimgrid.xaml
    /// </summary>
    public partial class FrmDimgrid : Window
    {
        private Dimgridcmd _data;
        private Document _doc;
        public FrmDimgrid(Dimgridcmd data,Document doc)
        {
            _doc = doc;
            _data = data;
            InitializeComponent();
            Combobox();
        }
        private void Combobox()
        {
            Cbb_typedim.ItemsSource = _data.dimensionTypes;
            Cbb_typedim.DisplayMemberPath = "Name";
        }

        private void btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            var dimtype = Cbb_typedim.SelectedItem as DimensionType;
            _data.Dimelement(_doc, dimtype,_doc.ActiveView);
            _data.Setting.Dimtype = dimtype.Name;
            _data.Setting.SaveSetting();
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Cbb_typedim.Text = (from x in _data.dimensionTypes where x.Name == _data.Setting.Dimtype select x).Cast<DimensionType>().First().Name;
            }
            catch
            {

            }
        }
    }
}
