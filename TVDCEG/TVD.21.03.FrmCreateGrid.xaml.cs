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
using TVDCEG.Ultis;
using TVDCEG.LBR;

namespace TVDCEG
{
    /// <summary>
    /// Interaction logic for FrmCreateGrid.xaml
    /// </summary>
    public partial class FrmCreateGrid : Window, IDisposable
    {
        public string namegridX;
        public string namegridY;
        public double kc;
        public int counts;
        public bool check = false;
        public List<Layoutdatacreategrid> DataAxisX = new List<Layoutdatacreategrid>();
        public List<Layoutdatacreategrid> DataAxisY = new List<Layoutdatacreategrid>();
        private CreateGridcmd _data;
        public FrmCreateGrid(CreateGridcmd data)
        {
            _data = data;
            InitializeComponent();
        }

        public void Dispose()
        {
            Close();
        }
        private void Data()
        {
            data_axisX.ItemsSource = DataAxisX;
            data_axisY.ItemsSource = DataAxisY;
        }
        private void btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            namegridX = txt_NameGridX.Text;
            namegridY = txt_NameGridY.Text;
            check = true;
            Convertdata();
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Data();
            data_axisX.CellEditEnding += Data_axisX_CellEditEnding;
            data_axisY.CellEditEnding += Data_axisY_CellEditEnding;
        }

        private void Data_axisY_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var column = e.Column as DataGridBoundColumn;
                if (column != null)
                {
                    var bindingPath = (column.Binding as Binding).Path.Path;
                    if (bindingPath == "space")
                    {
                        int rowIndex = e.Row.GetIndex();
                        var el = e.EditingElement as TextBox;
                        var hkl = el.Text;
                        string empty = string.Empty;
                        Ultis.UnitConvert.StringToFeetAndInches(hkl, out empty);
                        el.Text = empty;
                        // rowIndex has the row index
                        // bindingPath has the column's binding
                        // el.Text has the new, user-entered value
                    }
                }
            }
        }

        private void Data_axisX_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var column = e.Column as DataGridBoundColumn;
                if (column != null)
                {
                    var bindingPath = (column.Binding as Binding).Path.Path;
                    if (bindingPath == "space")
                    {
                        int rowIndex = e.Row.GetIndex();
                        var el = e.EditingElement as TextBox;
                        var hkl = el.Text;
                        string empty = string.Empty;
                        Ultis.UnitConvert.StringToFeetAndInches(hkl, out empty);
                        el.Text = empty;
                        // rowIndex has the row index
                        // bindingPath has the column's binding
                        // el.Text has the new, user-entered value
                    }
                }
            }
        }
        // convert dữ liệu trên datagrid về dơn vị trong revit
        private void Convertdata()
        {
            foreach (var item in DataAxisX)
            {
                var space = item.space;
                string bmn;
                var t = UnitConvert.StringToFeetAndInches(space, out bmn);
                item.space = t.ToString();
            }
            foreach (var item in DataAxisY)
            {
                var space = item.space;
                string bmn;
                var t = UnitConvert.StringToFeetAndInches(space, out bmn);
                item.space = t.ToString();
            }
        }
        private void Convertstringtofeetandinch(object sender, EventArgs eventArgs)
        {

        }
    }
}
