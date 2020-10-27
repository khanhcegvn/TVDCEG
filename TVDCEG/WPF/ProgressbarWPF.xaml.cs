using System;
using System.Windows;
using System.Windows.Markup;
namespace TVDCEG
{
    /// <summary>
    /// Interaction logic for ProgressbarWPF.xaml
    /// </summary>
    public partial class ProgressbarWPF : Window
    {
        public bool iscontinue { get; set; }
        private int _max;
        public string bottommessage;
        public string topmessage;
        private string titlename;
        public double Percent = 0.0;
        public ProgressbarWPF(int max, string title)
        {
            titlename = title;
            this.iscontinue = true;
            _max = max;
            this.DataContext = this;
            InitializeComponent();
        }
       
        private void Progressbar_Loaded(object sender, RoutedEventArgs e)
        {
            ProgressBar_wpf.Minimum = 0;
            ProgressBar_wpf.Maximum = _max;
        }
        public void Giatri()
        {
            try
            {
                ++ProgressBar_wpf.Value;
                sodem.Text = Math.Round((double)100 * ProgressBar_wpf.Value / _max, 0).ToString() + "%";
                Percent = ProgressBar_wpf.Value / _max;
                this.Title = titlename + "  " + ProgressBar_wpf.Value.ToString() + "/" + _max;
                this._topmessagre.Text = topmessage;
            }
            catch (Exception)
            {
            }
            System.Windows.Forms.Application.DoEvents();
        }

        private void Btn_Abort_Click(object sender, RoutedEventArgs e)
        {
            iscontinue = false;
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
