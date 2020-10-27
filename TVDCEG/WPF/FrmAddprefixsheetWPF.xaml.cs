using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace TVDCEG.WPF
{
    /// <summary>
    /// Interaction logic for FrmAddprefixsheetWPF.xaml
    /// </summary>
    public partial class FrmAddprefixsheetWPF : Window, IDisposable
    {
        private Addsurfixsheetcmd _data;
        public string prefix = null;
        public string suffix = null;
        public string SheetNumber_ = null;
        public bool checkvalue = true;
        public List<ViewSheet> listsheet1 = new List<ViewSheet>();
        private Dictionary<string, ViewSheet> dic_viewsheet = new Dictionary<string, ViewSheet>();
        private Document _doc;

        public FrmAddprefixsheetWPF(Addsurfixsheetcmd data, Document doc)
        {
            _data = data;
            _doc = doc;
            InitializeComponent();
            showall();
            listBox_sheet.DisplayMemberPath = "Key";
        }

        public void Dispose()
        {
            this.Close();
        }
        private void showall()
        {
            Listboxview(_data.ListSheet);
        }
        private void Listboxview(Dictionary<string, ViewSheet> views)
        {
            listBox_sheet.Items.Clear();
            foreach (var i in views)
            {
                listBox_sheet.Items.Add(i);
            }
        }
        private void FilterViews()
        {
            listBox_sheet.Items.Clear();
            var views = new Dictionary<string, ViewSheet>();
            foreach (var view in _data.ListSheet)
            {
                if (view.Key.ToUpper().Contains(Txt_Search.Text.ToUpper()))
                {
                    views.Add(view.Key, view.Value);
                }
            }
            Listboxview(views);
        }

        private void FrmAddprefixhseet_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Txt_Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterViews();
        }

        private void Btn_OK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            checkvalue = true;
            var t = listBox_sheet.SelectedItems;
            foreach (KeyValuePair<string, ViewSheet> keyValue in t)
            {
                listsheet1.Add(keyValue.Value);
            }
            prefix = Txt_prefix.Text.ToString();
            suffix = txt_surfix.Text.ToString();
            SheetNumber_ = txt_sheetnumber.Text.ToString();
            Close();
        }
        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
