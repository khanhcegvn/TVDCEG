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
    public partial class FrmOpenmultisheet : Window, IDisposable
    {
        private Document _doc;
        private Openmultisheetcmd _data;
        public List<Document> newdoc = new List<Document>();
        public List<ViewSheet> listview = new List<ViewSheet>();
        public bool check = false;
        public List<ViewSheet> viewsheettarget = new List<ViewSheet>();
        public FrmOpenmultisheet(Openmultisheetcmd data, Document doc)
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
            showall();
        }
        private void showall()
        {
            Listboxview(_data.dic_sheet);
            lb_view.DisplayMemberPath = "Key";
        }
        private void Listboxview(Dictionary<string, ViewSheet> views)
        {
            lb_view.Items.Clear();
            foreach (var i in views)
            {
                lb_view.Items.Add(i);
            }
        }
        private void FilterViews()
        {
            lb_view.Items.Clear();
            var views = new Dictionary<string, ViewSheet>();
            foreach (var view in _data.dic_sheet)
            {
                if (view.Key.ToUpper().Contains(txt_Search.Text.ToUpper()))
                {
                    views.Add(view.Key, view.Value);
                }
            }
            Listboxview(views);
        }

        private void btOk_Click(object sender, RoutedEventArgs e)
        {
            check = true;
            var t = lb_view.SelectedItems;
            foreach (KeyValuePair<string, ViewSheet> keyValue in t)
            {
                viewsheettarget.Add(keyValue.Value);
            }
            Close();
        }

        private void btcancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void txt_Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterViews();
        }
    }
}
