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
    public partial class FrmTopDT : Window, IDisposable
    {
        private TopDTcmd _data;
        public string val;
        public string tru;
        public string Suffix;
        public string Prefix;
        public bool check = false;
        public TextNoteType TextNoteType;
        public FrmTopDT(TopDTcmd data)
        {
            _data = data;
            InitializeComponent();
            Loaddata();
        }

        public void Dispose()
        {
            Close();
        }
        public void Loaddata()
        {
            cbb_textNotes.ItemsSource = _data.listTextnotes;
            cbb_textNotes.DisplayMemberPath = "Name";
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                txt_Prefix.Text = _data.Setting.Prefix;
                txt_suffix.Text = _data.Setting.Suffix;
                txt_val.Text = _data.Setting.cong;
                txt_tru.Text = _data.Setting.tru;
            }
            catch
            {

            }
            try
            {
                cbb_textNotes.Text = (from x in _data.listTextnotes where x.Name == _data.Setting.Textnote select x.Name).First();
            }
            catch
            {

            }
        }

        private void txt_val_LostFocus(object sender, RoutedEventArgs e)
        {
            var el = sender as TextBox;
            var hkl = el.Text;
            string empty = string.Empty;
            Ultis.UnitConvert.StringToFeetAndInches(hkl, out empty);
            el.Text = empty;
            if(empty=="0' 0\""||empty=="0\"")
            {
                el.Text = "";
            }
        }
        private bool Checkvalue()
        {
            if (!string.IsNullOrEmpty(txt_val.Text) && !string.IsNullOrEmpty(txt_tru.Text))
            {
                if (txt_val.Text == "" || txt_tru.Text == "")
                {
                    return true;
                }
                else
                {
                    MessageBox.Show("Fill in a vaule \"+\" or \"-\"", "Error");
                    return false;
                }
            }
            else
            {
                if (txt_val.Text == "" || txt_tru.Text == "")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private void btOk_Click(object sender, RoutedEventArgs e)
        {
            if (Checkvalue())
            {
                val = txt_val.Text;
                tru = txt_tru.Text;
                Suffix = txt_suffix.Text;
                Prefix = txt_Prefix.Text;
                TextNoteType = cbb_textNotes.SelectedItem as TextNoteType;
                check = true;
                _data.Setting.Prefix = Prefix;
                _data.Setting.Suffix = Suffix;
                _data.Setting.cong = val;
                _data.Setting.tru = tru;
                _data.Setting.Textnote = TextNoteType.Name;
                _data.Setting.SaveSetting();
                Close();
            }

        }

        private void btcancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void txt_tru_LostFocus(object sender, RoutedEventArgs e)
        {
            var el = sender as TextBox;
            var hkl = el.Text;
            string empty = string.Empty;
            Ultis.UnitConvert.StringToFeetAndInches(hkl, out empty);
            el.Text = empty;
            if (empty == "0' 0\"" || empty == "0\"")
            {
                el.Text = "";
            }
        }
    }
}
