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
using Autodesk.Revit.DB;

namespace TVDCEG
{
    /// <summary>
    /// Interaction logic for FrmProductonlevel.xaml
    /// </summary>
    public partial class FrmProductonlevel : Window, IDisposable
    {
        private Document _doc;
        private ProductonLevelcmd _data;
        private List<TextNoteType> list = new List<TextNoteType>();
        public TextNoteType textnotetype;
        public FrmProductonlevel(ProductonLevelcmd data, Document doc)
        {
            _doc = doc;
            _data = data;
            InitializeComponent();
            Showtypetext();
        }

        public void Dispose()
        {
            Close();
        }
        private void Showtypetext()
        {
            list = ProductonLevel.Instance.GetalltypeText(_doc);
            cbb_typetext.ItemsSource = list;
            cbb_typetext.DisplayMemberPath = "Name";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                cbb_typetext.Text = (from x in list where x.Name == _data.Setting.TypeText select x).Cast<TextNoteType>().First().Name;
            }
            catch
            {

            }
        }

        private void btn_OK_Click(object sender, RoutedEventArgs e)
        {
            textnotetype = cbb_typetext.SelectedItem as TextNoteType;
            try
            {
                _data.Setting.TypeText = (cbb_typetext.SelectedItem as TextNoteType).Name;
                _data.Setting.SaveSetting();
            }
            catch { _data.Setting.SaveSetting(); }
            Close();
        }
    }
}
