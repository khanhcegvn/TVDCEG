using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;
using TVDCEG.Ultis;
using System.Collections.ObjectModel;
using Autodesk.Revit.UI;
using System.Windows.Interop;

namespace TVDCEG
{
    /// <summary>
    /// Interaction logic for AddViewsToSheetForm.xaml
    /// </summary>
    public partial class AddViewsToSheetForm : Window, IDisposable
    {
        AddViewsToSheetCmd data;
        Document _doc;
        public ExternalEvent ExEvent { get; set; }
        private void OnButtonClicked()
        {
            int num = (int)this.ExEvent.Raise();
        }
        public PreviewControl pc;
        private List<ViewObject> listobjects = new List<ViewObject>();
        public ObservableCollection<ViewObject> _listObject;
        public AddViewsToSheetForm(AddViewsToSheetCmd _data, Document doc)
        {
            data = _data;
            _doc = doc;
            InitializeComponent();
            lbViews.DisplayMemberPath = "Name";
            cbViewportType.DisplayMemberPath = "Name";
            DisplayAll();
            IntPtr revitWindow;
            revitWindow = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
            //Get window of Revit form Revit handle
            HwndSource hwndSource = HwndSource.FromHwnd(revitWindow);
            var windowRevitOpen = hwndSource.RootVisual as Window;
            this.Owner = windowRevitOpen;
        }

        public void Dispose()
        {
            this.Close();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            data.Data.Views = lbViews.SelectedItems.Cast<View>().ToList();
            data.Data.ViewPortTypeId = (cbViewportType.SelectedItem as Element).Id;
            data.Data.IsDuplicate = cboxIsDuplicate.IsChecked ?? false;
            data.Data.Option = (ViewDuplicateOption)cbDuplicateOption.SelectedItem;
            pc?.Dispose();
            this.OnButtonClicked();
        }
        private void DisplayAll()
        {
            DisplayComboBoxViewType();
            DisplayComboBoxViewportType();
            DisplayListBox(data.Data.AllViews);
            DisplayComboboxDuplicateOption();
        }
        private void Preview_Control(Document doc, ElementId elementId)
        {
            IList<ElementId> viewexport = new List<ElementId>();
            var lip = doc.GetElement(elementId) as View;
            if (lip.ViewType != ViewType.Schedule)
            {
                //viewexport.Add(elementId);
                //ImageExportOptions options = new ImageExportOptions();
                //options.ShadowViewsFileType = options.HLRandWFViewsFileType = ImageFileType.PNG;
                //options.ZoomType = ZoomFitType.FitToPage;
                //options.ExportRange = ExportRange.CurrentView;
                //options.FitDirection = FitDirectionType.Horizontal;
                //options.FitDirection = FitDirectionType.Vertical;
                //options.Zoom = 50;
                //options.FitDirection = FitDirectionType.Horizontal;
                //options.ImageResolution = ImageResolution.DPI_150;
                //string folder = SettingAddview.Instance.GetFolderPath();
                //string fileBase = Path.GetFileNameWithoutExtension(Path.GetTempFileName());
                //options.FilePath = Path.Combine(folder, fileBase);
                //options.ExportRange = ExportRange.SetOfViews;
                //options.ShouldCreateWebSite = false;
                //options.SetViewsAndSheets(viewexport);
                //doc.ExportImage(options);
                //string[] files = Directory.GetFiles(folder, fileBase + "*.png");
                //foreach (string file in files)
                //{
                //    System.Drawing.Image image = System.Drawing.Image.FromFile(file);
                //    Image_preview.Source = new BitmapImage(new Uri(file));
                //}
                pc?.Dispose();
                pc = new PreviewControl(doc, elementId);
                pc.Height = _preview.Height;
                pc.Width = _preview.Width;
                _preview.Children.Add(pc);
            }
        }
        private void Findviewinsheet(Document doc, ElementId elementId)
        {
            listobjects.Clear();
            listobjects = (from x in data.viewObjects where x.IdView == elementId.IntegerValue select x).ToList();
            _listObject = new ObservableCollection<ViewObject>(listobjects);
            Listviewelement.ItemsSource = _listObject;
        }
        private static BitmapImage GetImage(string imageUri)
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(imageUri, UriKind.RelativeOrAbsolute);
            bitmapImage.EndInit();
            return bitmapImage;
        }
        private void DisplayListBox(List<View> views)
        {
            lbViews.Items.Clear();
            views.ForEach(x => lbViews.Items.Add(x));
        }
        private void DisplayComboBoxViewType()
        {
            cbViewType.Items.Add("All");
            data.Data.AllViews.Select(x => x.ViewType).Distinct().ToList().ForEach(x => cbViewType.Items.Add(x));
        }
        private void DisplayComboBoxViewportType()
        {
            var vp = new FilteredElementCollector(data.Doc).OfClass(typeof(Viewport)).FirstOrDefault() as Viewport;
            if (vp != null)
            {
                var eles = vp.GetValidTypes().ToList().Select(x => data.Doc.GetElement(x));
                foreach (var ele in eles)
                {
                    cbViewportType.Items.Add(ele);
                }

            }
        }
        private void DisplayComboboxDuplicateOption()
        {
            cbDuplicateOption.Items.Add(ViewDuplicateOption.AsDependent);
            cbDuplicateOption.Items.Add(ViewDuplicateOption.Duplicate);
            cbDuplicateOption.Items.Add(ViewDuplicateOption.WithDetailing);
        }
        private void ListBoxFilter()
        {
            lbViews.Items.Clear();
            var views = new List<View>();
            foreach (var view in data.Data.AllViews)
            {
                if (view.Name.ToUpper().Contains(tbSearch.Text.ToUpper()) && cbViewType.SelectedValue.ToString() == "All")
                {
                    views.Add(view);
                }
                else if (view.Name.ToUpper().Contains(tbSearch.Text.ToUpper()) && view.ViewType == (ViewType)cbViewType.SelectedItem)
                {
                    views.Add(view);
                }
            }
            ADDVIEW(views);
        }
        private void ADDVIEW(List<View> views)
        {
            views.ForEach(X => lbViews.Items.Add(X));
        }
        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ListBoxFilter();
        }
        private void cbViewType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxFilter();
        }

        private void lbViews_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var i = lbViews.SelectedItems.Cast<View>().ToList();
            if (i.Count != 0)
            {
                if (i.Last().ViewType != ViewType.Schedule)
                {
                    Preview_Control(_doc, i.Last().Id);
                }
                Findviewinsheet(_doc, i.Last().Id);
                pc?.Dispose();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            pc?.Dispose();
        }
    }
    public class ViewObject
    {
        private string _viewname;
        private string _sheetname;
        private string _sheetnumber;
        private int _idview;
        private int _idviewhseet;
        public string ViewName
        {
            get
            {
                return this._viewname;
            }
            set
            {
                this._viewname = value;
            }
        }
        public string SheetName
        {
            get
            {
                return this._sheetname;
            }
            set
            {
                this._sheetname = value;
            }
        }
        public string SheetNumber
        {
            get
            {
                return this._sheetnumber;
            }
            set
            {
                this._sheetnumber = value;
            }
        }
        public int IdView
        {
            get
            {
                return this._idview;
            }
            set
            {
                this._idview = value;
            }
        }
        public int IdViewSheet
        { 
            get
            {
                return this._idviewhseet;
            }
            set
            {
                this._idviewhseet = value;
            }
        }
        public ViewObject(string viewname,string sheetname,string sheetnumber,int idview,int idviewsheet)
        {
            _viewname = viewname;
            _sheetname = sheetname;
            _sheetnumber = sheetnumber;
            _idview = idview;
            _idviewhseet = idviewsheet;
        }
    }
    public class SettingAddview
    {
        private static SettingAddview _intance;
        private SettingAddview()
        {

        }
        public static SettingAddview Instance => _intance ?? (_intance = new SettingAddview());
        public string GetFolderPath()
        {
            string folderPath = SettingExtension.GetSettingPath() + "\\TVD.Addview";
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
            return folderPath;
        }

        public string GetFileName()
        {
            return "Addview.json";
        }

        public string GetFullFileName()
        {
            return GetFolderPath() + "\\" + GetFileName();
        }

        public void SaveSetting()
        {
            var gh = GetFullFileName();
            SettingExtension.SaveSetting(this, GetFullFileName());
        }

        public SettingAddview GetSetting()
        {
            SettingAddview setting = SettingExtension.GetSetting<SettingAddview>(GetFullFileName());
            if (setting == null) setting = new SettingAddview();
            return setting;
        }
    }
}
