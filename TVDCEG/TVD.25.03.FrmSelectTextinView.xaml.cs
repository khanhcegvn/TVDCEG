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

namespace TVDCEG
{
    /// <summary>
    /// Interaction logic for FrmSelectTextinView.xaml
    /// </summary>
    public partial class FrmSelectTextinView : Window, IDisposable
    {
        private Document _doc;
        private SelectTextInviewcmd _data;
        public List<ElementId> ids = new List<ElementId>();
        public bool check = false;
        public List<View> viewstagget = new List<View>();
        public List<View> listview = new List<View>();
        public FrmSelectTextinView(SelectTextInviewcmd data, Document doc)
        {
            _doc = doc;
            _data = data;
            InitializeComponent();
            listBoxdata();
        }
        private void listBoxdata()
        {
            listview = SelectTextInview.Instance.GetAllViews(_doc);
            Listboxview(listview);
            listBox_View.DisplayMemberPath = "Name";
        }
        private void Listboxview(List<View> views)
        {
            listBox_View.Items.Clear();
            foreach (var i in views)
            {
                listBox_View.Items.Add(i);
            }
        }
        private void FilterViews()
        {
            listBox_View.Items.Clear();
            var views = new List<View>();
            foreach (var view in listview)
            {
                if (view.Name.ToUpper().Contains(Txt_Search.Text.ToUpper()))
                {
                    views.Add(view);
                }
            }
            Listboxview(views);
        }

        private void Txt_Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterViews();
        }

        public void Dispose()
        {
            Close();
        }

        private void btn_Select_Click(object sender, RoutedEventArgs e)
        {
            var text = Txt_Contain.Text;
            var col = SelectTextInview.Instance.GetTextNode(_doc, text);
            List<ElementId> ids = new List<ElementId>();
            col.ForEach(x => ids.Add(x.Id));
            _data.sel.SetElementIds(ids);
            Close();
        }

        private void btn_OK_Click(object sender, RoutedEventArgs e)
        {
            check = true;
            var text = Txt_Contain.Text;
            var col = SelectTextInview.Instance.GetTextNode(_doc, text);
            ids = new List<ElementId>();
            col.ForEach(x => ids.Add(x.Id));
            viewstagget = listBox_View.SelectedItems.Cast<View>().ToList();
            Close();
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
