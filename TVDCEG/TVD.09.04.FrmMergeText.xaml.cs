using Autodesk.Revit.UI;
using System.Windows;

namespace TVDCEG
{
    /// <summary>
    /// Interaction logic for FrmMergeText.xaml
    /// </summary>
    public partial class FrmMergeText : Window
    {
        private MergeTextcmd _data;
        public ExternalEvent ExEvent { get; set; }
        private void OnButtonClicked()
        {
            int num = (int)this.ExEvent.Raise();
        }
        public FrmMergeText(MergeTextcmd data)
        {
            _data = data;
            InitializeComponent();
        }

        private void btn_pick_Click(object sender, RoutedEventArgs e)
        {
            _data.btn_pick = true;
            this.OnButtonClicked();
        }

        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
