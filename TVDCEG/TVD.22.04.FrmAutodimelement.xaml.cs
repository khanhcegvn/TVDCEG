using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
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
    /// Interaction logic for FrmAutodimelement.xaml
    /// </summary>
    public partial class FrmAutodimelement : Window
    {
        private AutodimElementcmd _data;
        public ExternalEvent ExEvent { get; set; }
        private void OnButtonClicked()
        {
            int num = (int)this.ExEvent.Raise();
        }
        public DimensionType DimensionType
        {
            get;
            set;
        }
        public FrmAutodimelement(AutodimElementcmd data)
        {
            _data = data;
            InitializeComponent();
            Combobox();
        }
        
        private void btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            DimensionType = cbb_Dimtype.SelectedItem as DimensionType;
            this.OnButtonClicked();
        }
        private void Combobox()
        {
            cbb_Dimtype.ItemsSource = _data.dimensionTypes;
            cbb_Dimtype.DisplayMemberPath = "Name";
        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            _data.Setting.DimensionType = cbb_Dimtype.Text;
            _data.Setting.SaveSetting();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                cbb_Dimtype.Text = (from x in _data.dimensionTypes where x.Name == _data.Setting.DimensionType select x).Cast<DimensionType>().First().Name;
            }
            catch
            {

            }
        }
    }
}
