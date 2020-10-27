using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TVDCEG.WPF
{
    /// <summary>
    /// Interaction logic for FrmRespacedim.xaml
    /// </summary>
    public partial class FrmRespacedim : Window, IDisposable
    {
        public double space;
        public bool radiobutoncheck = true;
        public FrmRespacedim()
        {
            InitializeComponent();
        }

        public void Dispose()
        {
            this.Close();
        }
        private void Checkbutton()
        {
            if (ckb_Switch.IsChecked == false)
            {
                radiobutoncheck = true;
            }
            if (ckb_Switch.IsChecked == true)
            {
                radiobutoncheck = false;
            }
        }
        private void Btn_OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            space = double.Parse(TBX_Space.Text);
            Close();
        }
        private void TBX_Space_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void TBX_Space_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void Ckb_Switch_Click(object sender, RoutedEventArgs e)
        {
            Checkbutton();
        }
    }
}
