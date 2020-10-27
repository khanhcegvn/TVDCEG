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
    public partial class FrmAddValueAssembly : Window, IDisposable
    {
        public string Parametertag;
        private CaculatorAssemblycmd _data;
        public bool check = false;
        public bool All = false;
        public List<ProductCEG> listinstance = new List<ProductCEG>();
        public List<ProductCEG> tong = new List<ProductCEG>();
        public List<AssemblyInstance> listassem = new List<AssemblyInstance>();
        public FrmAddValueAssembly(CaculatorAssemblycmd data)
        {
            _data = data;
            InitializeComponent();
            
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbb_Parameter.ItemsSource = _data.dicpa;
            Listbox();
        }
        public void Dispose()
        {
            Close();
        }
        //private void Listbox()
        //{
        //    lb_viewassembly.Items.Clear();
        //    _data.list.ForEach(x => lb_viewassembly.Items.Add(x));
        //    lb_viewassembly.DisplayMemberPath = "Name";
        //}
        //private void Filter()
        //{
        //    lb_viewassembly.Items.Clear();
        //    List<AssemblyInstance> newlist = new List<AssemblyInstance>();
        //    foreach (var item in _data.list)
        //    {
        //        if(item.Name.ToUpper().Contains(txt_search.Text.ToUpper()))
        //        {
        //            newlist.Add(item);
        //        }
        //    }
        //    Updatelistbox(newlist);
        //}
        private void Listbox()
        {
            lb_viewassembly.Items.Clear();
            _data.dic.Keys.ToList().ForEach(x => lb_viewassembly.Items.Add(x));
            //lb_viewassembly.DisplayMemberPath = "Name";
        }
        private void Filter()
        {
            lb_viewassembly.Items.Clear();
            List<string> newlist = new List<string>();
            foreach (var item in _data.dic.Keys)
            {
                if (item.ToUpper().Contains(txt_search.Text.ToUpper()))
                {
                    newlist.Add(item);
                }
            }
            Updatelistbox(newlist);
        }
        private void Updatelistbox(List<string> assemblyInstances)
        {
            lb_viewassembly.Items.Clear();
            assemblyInstances.ForEach(x => lb_viewassembly.Items.Add(x));
            //lb_viewassembly.DisplayMemberPath = "Name";
        }
        //private void Updatelistbox(List<AssemblyInstance> assemblyInstances)
        //{
        //    lb_viewassembly.Items.Clear();
        //    assemblyInstances.ForEach(x => lb_viewassembly.Items.Add(x));
        //    lb_viewassembly.DisplayMemberPath = "Name";
        //}
        private void btn_OK_Click(object sender, RoutedEventArgs e)
        {
            check = true;
            Parametertag = cbb_Parameter.Text;
            if(ckb_All.IsChecked==true)
            {
                All = true;
                foreach (var item in _data.dic.Keys)
                {
                    foreach (var item2 in _data.dic[item])
                    {
                        tong.Add(item2);
                    }
                }
            }
            else
            {
                //listassem = lb_viewassembly.SelectedItems.Cast<AssemblyInstance>().ToList();
                var text = lb_viewassembly.SelectedItems.Cast<string>().ToList();
                foreach (var item in text)
                {
                    foreach (var item2 in _data.dic[item])
                    {
                        listinstance.Add(item2);
                    }
                }
            }
            Close();
        }

        private void txt_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filter();
        }
    }
}
