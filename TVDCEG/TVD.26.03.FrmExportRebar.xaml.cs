using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for FrmExportRebar.xaml
    /// </summary>
    public partial class FrmExportRebar : Window,IDisposable
    {
        private ExportRebarSchedulecmd _data;
        public Dictionary<string, List<CEGRebarInfo>> dic = new Dictionary<string, List<CEGRebarInfo>>();
        public ObservableCollection<List<CEGRebarInfo>> listObject;
        public FrmExportRebar(ExportRebarSchedulecmd data)
        {
            _data = data;
            InitializeComponent();
            Convertdic();
        }

        public void Dispose()
        {
            Close();
        }
        private void Convertdic()
        {
            foreach (var item in _data.DicRebar.Keys.ToList())
            {
                foreach (var item2 in _data.DicRebar[item])
                {
                    if(item2.ControlMark!=null)
                    {
                        if (dic.ContainsKey(item2.ControlMark))
                        {
                            dic[item2.ControlMark].Add(item2);
                        }
                        else
                        {
                            dic.Add(item2.ControlMark, new List<CEGRebarInfo> { item2 });
                        }
                    }
                }
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            listObject = new ObservableCollection<List<CEGRebarInfo>>(dic.Values);
            Listviewelement.ItemsSource = listObject;
        }
      
        private void btn_Export_Click(object sender, RoutedEventArgs e)
        {
            ExportRebarSchedule.Instance.Export(listObject);
            Close();
        }
    }
}
