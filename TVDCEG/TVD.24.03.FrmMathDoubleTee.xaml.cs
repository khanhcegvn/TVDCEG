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
    /// Interaction logic for FrmMathDoubleTee.xaml
    /// </summary>
    public partial class FrmMathDoubleTee : Window,IDisposable
    {
        private MathWarpedDoubleteecmd _data;
        public List<DoubleTee> list1 = new List<DoubleTee>();
        public List<DoubleTee> list2 = new List<DoubleTee>();
        public List<string> liststring = new List<string>();
        public Dictionary<string, List<DoubleTee>> dic1 = new Dictionary<string, List<DoubleTee>>();
        public double check = 0;
        public FrmMathDoubleTee(MathWarpedDoubleteecmd data)
        {
            _data = data;
            InitializeComponent();
            Datacombobox();
        }
        private void Datacombobox()
        {
            from_dt.Items.Clear();
            liststring = _data.dic.Keys.ToList();
            var kl = _data.dic.Keys.ToList();
            kl.Sort();
            kl.ForEach(x => from_dt.Items.Add(x));
        }
        private void Datalistbox(List<string> list)
        {
            lb_show.Items.Clear();
            list.ForEach(x => lb_show.Items.Add(x));
        }
        private void btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            check = 1;
            list1 = _data.dic[from_dt.Text].ToList();
            var t = lb_show.SelectedItems.Cast<string>().ToList();
            foreach (var item in t)
            {
                foreach (var i in _data.dic[item])
                {
                    if(dic1.ContainsKey(item))
                    {
                        dic1[item].Add(i);
                    }
                    else
                    {
                        dic1.Add(item, new List<DoubleTee> { i });
                    }
                }
            }
            Close();
        }

        private void from_dt_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var text = from_dt.SelectedValue.ToString();
            List<string> newlist = new List<string>();
            foreach (var item in liststring)
            {
                if (item != text)
                {
                    newlist.Add(item);
                }
            }
            newlist.Sort();
            Datalistbox(newlist);
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btn_Select_Click(object sender, RoutedEventArgs e)
        {
            check = 2;
            Close();
        }

        public void Dispose()
        {
            Close();
        }
    }
}
