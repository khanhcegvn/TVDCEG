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
    public partial class FrmTransferviewtemplate : Window, IDisposable
    {
        private Document _doc;
        private Transferviewtemplatecmd _data;
        public List<Document> newdoc = new List<Document>();
        public bool check = false;
        public Document source;
        public Document target;
        public List<ElementId> listtarget = new List<ElementId>();
        public List<View> listviewtemplate = new List<View>();
        public FrmTransferviewtemplate(Transferviewtemplatecmd data, Document doc)
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
            cbb_from.Items.Clear();
            _data.listdoc.ForEach(x => cbb_from.Items.Add(x));
            cbb_from.DisplayMemberPath = "Title";
        }
        private void Datacombobox()
        {
            cbb_To.Items.Clear();
            listviewtemplate.Clear();
            lb_view.Items.Clear();
            string textcbb_from = (cbb_from.SelectedItem as Document).Title;
            newdoc = (from x in _data.listdoc where x.Title != textcbb_from select x).ToList();
            newdoc.ForEach(x => cbb_To.Items.Add(x));
            cbb_To.DisplayMemberPath = "Title";
            Document m_doc = null;
            foreach (var item in _data.listdoc)
            {
                if(item.Title==textcbb_from)
                {
                    m_doc = item;
                }
            }
            listviewtemplate = _data.GetAllviewtemplate(m_doc);
            listviewtemplate.ForEach(x => lb_view.Items.Add(x));
            lb_view.DisplayMemberPath = "Name";
        }

        private void cbb_from_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Datacombobox();
        }

        private void btOk_Click(object sender, RoutedEventArgs e)
        {
            check = true;
            source = cbb_from.SelectedItem as Document;
            target = cbb_To.SelectedItem as Document;
            var views = lb_view.SelectedItems.Cast<View>().ToList();
            views.ForEach(x => listtarget.Add(x.Id));
            Close();
        }

        private void btcancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
