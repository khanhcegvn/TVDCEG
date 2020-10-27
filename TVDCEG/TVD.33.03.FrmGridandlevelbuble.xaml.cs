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
    public partial class FrmGridandlevelbuble : Window, IDisposable
    {
        private Document _doc;
        private Gridandlevelbublecmd _data;
        public FrmGridandlevelbuble(Gridandlevelbublecmd data, Document doc)
        {
            _doc = doc;
            _data = data;
            InitializeComponent();
        }

        public void Dispose()
        {
            Close();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void btOk_Click(object sender, RoutedEventArgs e)
        {
            _data.Databubble.Top = cbTop.IsChecked;
            _data.Databubble.Bottom = cbBot.IsChecked;
            _data.Databubble.Left = cbLeft.IsChecked;
            _data.Databubble.Right = cbRight.IsChecked;
            Close();
        }
    }
}
