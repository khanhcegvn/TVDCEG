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
using Autodesk.Revit.DB;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace TVDCEG
{
    /// <summary>
    /// Interaction logic for FrmAddValue.xaml
    /// </summary>
    public partial class FrmAddValue : Window, IDisposable
    {
        public string Parametertag;
        private AddvaluecontrolMarkcmd _data;
        public bool check = false;
        public List<FamilyInstance> listinstance = new List<FamilyInstance>();
        public FrmAddValue(AddvaluecontrolMarkcmd data)
        {
            _data = data;
            InitializeComponent();
            
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbb_tag.ItemsSource = _data.independentTags;
            cbb_Parameter.ItemsSource = _data.dicpa;
        }
        public void Dispose()
        {
            Close();
        }
        private void btn_OK_Click(object sender, RoutedEventArgs e)
        {
            check = true;
            Parametertag = cbb_Parameter.Text;
            foreach (var item in _data.independentTags[cbb_tag.Text])
            {
                listinstance.Add(item);
            }
            Close();
        }
    }
}
