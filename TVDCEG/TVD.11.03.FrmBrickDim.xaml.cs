using Autodesk.Revit.UI;
using System;
using System.Windows;
using Autodesk.Revit.DB;
using System.Linq;
using System.Windows.Markup;
using System.IO;
using TVDCEG.Ultis;

namespace TVDCEG
{
    /// <summary>
    /// Interaction logic for FrmBrickDim.xaml
    /// </summary>
    public partial class FrmBrickDim : Window, IComponentConnector
    {
        public ExternalEvent ExEvent { get; set; }
        private Document _doc;
        public int space;
        public double Checktype;
        public DimensionType Dimvertical
        {
            get;
            set;
        }

        public DimensionType Dimholizontal1
        {
            get;
            set;
        }
        public DimensionType Dimholizontal2
        {
            get;
            set;
        }
        private DimBrickcmd _data;
        private void OnButtonClicked()
        {
            int num = (int)this.ExEvent.Raise();
        }
        public FrmBrickDim(DimBrickcmd data,Document doc)
        {
            _doc = doc;
            _data = data;
            InitializeComponent();
            Showdimensiontypes();

        }
        private void Showdimensiontypes()
        {
            cbb_dimtypevertical.ItemsSource = _data.dimensionTypes;
            cbb_dimtypeholizontal1.ItemsSource = _data.dimensionTypes;
            cbb_dimtypeholizontal2.ItemsSource = _data.dimensionTypes;
            cbb_dimtypevertical.DisplayMemberPath = "Name";
            cbb_dimtypeholizontal1.DisplayMemberPath = "Name";
            cbb_dimtypeholizontal2.DisplayMemberPath = "Name";
        }
        private void btn_Pick_Click(object sender, RoutedEventArgs e)
        {
            OnButtonClicked();
            if (Rbtn_Holizontal.IsChecked == true)
            {
                space = Convert.ToInt32(txt_CountRow.Text);
                Dimholizontal1 = cbb_dimtypeholizontal1.SelectedItem as DimensionType;
                Dimholizontal2 = cbb_dimtypeholizontal2.SelectedItem as DimensionType;
            }
            else
            {
                Dimvertical = cbb_dimtypevertical.SelectedItem as DimensionType;
            }
        }

        private void Rbtn_Vertical_Checked(object sender, RoutedEventArgs e)
        {
            if(ckc_switch.IsChecked==true)
            {
                Checktype = 5;
                textblock_dimtypevertical.IsEnabled = true;
                textblock_space.IsEnabled = false;
                textblock_dimtype1.IsEnabled = false;
                textblock_dimtype2.IsEnabled = false;
                cbb_dimtypevertical.IsEnabled = true;
                txt_CountRow.IsEnabled = false;
                cbb_dimtypeholizontal1.IsEnabled = false;
                cbb_dimtypeholizontal2.IsEnabled = false;
            }
            else
            {
                Checktype = 1;
                textblock_dimtypevertical.IsEnabled = true;
                textblock_space.IsEnabled = false;
                textblock_dimtype1.IsEnabled = false;
                textblock_dimtype2.IsEnabled = false;
                cbb_dimtypevertical.IsEnabled = true;
                txt_CountRow.IsEnabled = false;
                cbb_dimtypeholizontal1.IsEnabled = false;
                cbb_dimtypeholizontal2.IsEnabled = false;
            }
        }
        private void Rbtn_Holizontal_Checked(object sender, RoutedEventArgs e)
        {
            if(ckc_switch.IsChecked==true)
            {
                Checktype = 6;
                textblock_dimtypevertical.IsEnabled = false;
                textblock_space.IsEnabled = true;
                textblock_dimtype1.IsEnabled = true;
                textblock_dimtype2.IsEnabled = true;
                cbb_dimtypevertical.IsEnabled = false;
                txt_CountRow.IsEnabled = true;
                cbb_dimtypeholizontal1.IsEnabled = true;
                cbb_dimtypeholizontal2.IsEnabled = true;
            }
            else
            {
                Checktype = 2;
                textblock_dimtypevertical.IsEnabled = false;
                textblock_space.IsEnabled = true;
                textblock_dimtype1.IsEnabled = true;
                textblock_dimtype2.IsEnabled = true;
                cbb_dimtypevertical.IsEnabled = false;
                txt_CountRow.IsEnabled = true;
                cbb_dimtypeholizontal1.IsEnabled = true;
                cbb_dimtypeholizontal2.IsEnabled = true;
            }
        }

        private void btn_SaveSetting_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _data.Setting.DimensionVertical = (cbb_dimtypevertical.SelectedItem as DimensionType).Name;
                _data.Setting.Dimensiontypeholizontal1 = (cbb_dimtypeholizontal1.SelectedItem as DimensionType).Name;
                _data.Setting.Dimensiontypeholizontal2 = (cbb_dimtypeholizontal2.SelectedItem as DimensionType).Name;
                _data.Setting.Space = txt_CountRow.Text;
                _data.Setting.SaveSetting();
            }
            catch
            {
                _data.Setting.SaveSetting();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                cbb_dimtypevertical.Text = (from x in _data.dimensionTypes where x.Name == _data.Setting.DimensionVertical select x).Cast<DimensionType>().First().Name;
                cbb_dimtypeholizontal1.Text = (from x in _data.dimensionTypes where x.Name == _data.Setting.Dimensiontypeholizontal1 select x).Cast<DimensionType>().First().Name;
                cbb_dimtypeholizontal2.Text = (from x in _data.dimensionTypes where x.Name == _data.Setting.Dimensiontypeholizontal2 select x).Cast<DimensionType>().First().Name;
                txt_CountRow.Text = _data.Setting.Space;
                Rbtn_Vertical.IsChecked = true;
                Rbtn_Holizontal.IsChecked = false;
                Rbtn_Vertical.IsChecked = false;
            }
            catch
            {

            }
        }

        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            Checktype = 4;
            this.OnButtonClicked();
            Close();
        }
        private void checkbox_Showrow_Checked(object sender, RoutedEventArgs e)
        {
            Checktype = 3;
            this.OnButtonClicked();
           
        }
        private void checkbox_Showrow_Unchecked(object sender, RoutedEventArgs e)
        {
            Checktype = 4;
            this.OnButtonClicked();
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            Checktype = 4;
            this.OnButtonClicked();
        }
    }
}
