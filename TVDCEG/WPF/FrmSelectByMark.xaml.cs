using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Markup;
using a = Autodesk.Revit.DB;
using System;

namespace TVDCEG.WPF
{
    /// <summary>
    /// Interaction logic for FrmSelectByMark.xaml
    /// </summary>
    public partial class FrmSelectByMark : Window, IComponentConnector, IDisposable
    {
        private SelectByControlMark _data;
        private a.Document _doc;
        public a.View3D _view3d
        {
            get;
            set;
        }
        public List<a.ElementId> ids = new List<a.ElementId>();
        public FrmSelectByMark(SelectByControlMark data, a.Document doc)
        {
            _data = data;
            _doc = doc;
            InitializeComponent();
            Show();
            Listbox_view.DisplayMemberPath = "Key";
        }
        public void Dispose()
        {
            this.Close();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                cbb_view3d.Text = (from y in _data.list3d where y.Name == _data.Setting.View3d select y).Cast<a.View3D>().First().Name;
            }
            catch
            {

            }
        }
        private void Listboxview(Dictionary<string, List<a.FamilyInstance>> familyInstances)
        {
            Listbox_view.Items.Clear();
            foreach (var i in familyInstances)
            {
                Listbox_view.Items.Add(i);
            }
        }
        private void Filterlistbox()
        {
            Listbox_view.Items.Clear();
            var familyinstances = new Dictionary<string, List<a.FamilyInstance>>();
            foreach (var instance in _data.listinstance)
            {
                if (instance.Key.ToUpper().Contains(txt_Search.Text.ToUpper()))
                {
                    familyinstances.Add(instance.Key, instance.Value);
                }
            }
            Listboxview(familyinstances);
        }
        private new void Show()
        {
            Filterlistbox();
            Comboboxview3d();
        }
        private void Comboboxview3d()
        {
            cbb_view3d.ItemsSource = _data.list3d;
            cbb_view3d.DisplayMemberPath = "Name";
        }
        private void txt_Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filterlistbox();
        }

        private void Btn_Select_Click(object sender, RoutedEventArgs e)
        {
            var b = Listbox_view.SelectedItems;
            foreach (KeyValuePair<string, List<a.FamilyInstance>> keyValue in b)
            {
                foreach (var i in keyValue.Value)
                {
                    ids.Add(i.Id);
                }
            }
            try
            {
                _view3d = cbb_view3d.SelectedItem as a.View3D;
                _data.Setting.View3d = (cbb_view3d.SelectedItem as a.View3D).Name;
                _data.Setting.SaveSetting();
            }
            catch
            {
                _data.Setting.SaveSetting();
            }
            Close();
        }
    }
}
