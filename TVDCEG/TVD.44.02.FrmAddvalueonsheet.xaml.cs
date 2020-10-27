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
using TVDCEG.LBR;
using TVDCEG.Ultis;

namespace TVDCEG
{
    /// <summary>
    /// Interaction logic for FrmProductonlevel.xaml
    /// </summary>
    public partial class FrmAddvalueonsheet : Window, IDisposable
    {
        public List<View> listview = new List<View>();
        private Document _doc;
        private List<View> Source = new List<View>();
        public FrmAddvalueonsheet(Document doc)
        {
            _doc = doc;
            InitializeComponent();
            lb_view.DisplayMemberPath = "Name";
        }

        public void Dispose()
        {
            Close();
        }
        public void Loaddata()
        {
            Source = Getmodelelement.GetAllViews(_doc);
            Listboxview(Source);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Loaddata();
        }
        private void Listboxview(List<View> views)
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
            var views = new List<View>();
            foreach (var view in Source)
            {
                if (view.Name.ToUpper().Contains(txt_search.Text.ToUpper()))
                {
                    views.Add(view);
                }
            }
            Listboxview(views);
        }
        private void btcancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void txt_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterViews();
        }

        private void btOk_Click(object sender, RoutedEventArgs e)
        {
            listview = lb_view.SelectedItems.Cast<View>().ToList();
            Close();
        }
    }
}
