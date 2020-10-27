using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using a = Autodesk.Revit.DB;
namespace TVDCEG
{
    public partial class TransferView : System.Windows.Forms.Form
    {
        private Transfer_view _data;
        public string Title_name;
        private a.Document _doc;
        public a.Document source = null;
        public List<a.View> listview = new List<a.View>();
        public List<a.View> newlist = new List<a.View>();
        public List<a.Document> newdoc = new List<a.Document>();
        public IList<a.ElementId> viewstransfer = new List<a.ElementId>();
        private PreviewControl pc;
        [Obsolete]
        public TransferView(Transfer_view data, a.Document doc)
        {
            _data = data;
            _doc = doc;
            InitializeComponent();
            lb_viewtype.DisplayMember = "Name";
            ShowAll();
        }
        private lbr_view gi = new lbr_view();
        private void Cb_ViewType_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterViews();
        }
        private void TransferView_Load(object sender, EventArgs e)
        {

        }

        [Obsolete]
        private void ShowAll()
        {
            Comboboxproj();
        }
        private void Comboboxtypeview()
        {
            Cb_ViewType.Items.Clear();
            Cb_ViewType.Items.Add("All");
            newlist.Select(x => x.ViewType).Distinct().ToList().ForEach(x => Cb_ViewType.Items.Add(x));
            Cb_ViewType.SelectedIndex = 0;
        }
        private void Listboxview(List<a.View> views)
        {
            lb_viewtype.Items.Clear();
            views.ForEach(x => lb_viewtype.Items.Add(x));
        }
        private void Comboboxproj()
        {
            comboBox_proj.Items.Clear();
            _data.listdoc.Select(x => x.Title).Distinct().ToList().ForEach(x => comboBox_proj.Items.Add(x));
        }
        private void FilterViews()
        {
            lb_viewtype.Items.Clear();
            var views = new List<a.View>();
            foreach (var view in newlist)
            {
                if (view.Name.ToUpper().Contains(textBox_search.Text.ToUpper()) && Cb_ViewType.SelectedItem.ToString() == "All")
                {
                    views.Add(view);
                }
                else if (view.Name.ToUpper().Contains(textBox_search.Text.ToUpper()) && view.ViewType == (a.ViewType)Cb_ViewType.SelectedItem)
                {
                    views.Add(view);
                }
            }
            views = views.OrderBy(x => x.Name).ToList();
            Listboxview(views);
        }
        private void Preview_Control(a.Document doc, a.ElementId elementId)
        {
            //IList<a.ElementId> viewexport = new List<a.ElementId>();
            //viewexport.Add(elementId);
            //ImageExportOptions options = new ImageExportOptions();
            //options.ShadowViewsFileType = options.HLRandWFViewsFileType = ImageFileType.PNG;
            //options.ZoomType = ZoomFitType.Zoom;
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
            //    Image image = Image.FromFile(file);
            //    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            //    pictureBox1.Image = image;
            //}
            a.View view = doc.GetElement(elementId) as a.View;
            pc?.Dispose();
            if (view.ViewType != ViewType.Schedule)
            {
                pc = new PreviewControl(doc, elementId);
                elementHost1.Child = pc;
            }
        }

        private void textBox_search_TextChanged(object sender, EventArgs e)
        {
            FilterViews();
        }
        private void group_Options_Enter(object sender, EventArgs e)
        {

        }
        [Obsolete]
        private void comboBox_proj_SelectedIndexChanged(object sender, EventArgs e)
        {
            newdoc.Clear();
            Title_name = comboBox_proj.SelectedItem.ToString();
            foreach (var source in _data.listdoc)
            {
                if (source.Title.ToString() == Title_name)
                {
                    newlist = gi.GetViews(source);
                }
                else
                {
                    newdoc.Add(source);
                }
            }
            lb_viewtype.Items.Clear();
            newlist.ForEach(x => lb_viewtype.Items.Add(x));
            Comboboxtypeview();
            checkedListBox_prj.Items.Clear();
            newdoc.ForEach(x => checkedListBox_prj.Items.Add(x.Title));
            Getdocumentsource();
        }

        private void lb_view_SelectedIndexChanged(object sender, EventArgs e)
        {
            listview.Clear();
            listview = lb_viewtype.SelectedItems.Cast<a.View>().ToList();
            Preview_Control(source, listview.First().Id);
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void check_btn_transfer()
        {
            if (checkedListBox_prj.CheckedItems.Count == 0)
            {
                btn_Transfer.Enabled = false;
            }
            else if (checkedListBox_prj.CheckedItems.Count != 0)
            {
                btn_Transfer.Enabled = true;
            }
        }
        [Obsolete]
        private void Getdocumentsource()
        {
            foreach (var u in _data.listdoc)
            {
                if (u.Title.ToString() == comboBox_proj.SelectedItem.ToString())
                {
                    source = u;
                }
            }
        }

        [Obsolete]
        private void btn_Transfer_Click(object sender, EventArgs e)
        {
            List<a.Document> listtarget = new List<a.Document>();
            foreach (var i in checkedListBox_prj.CheckedItems)
            {
                foreach (var r in newdoc)
                {
                    if (i.ToString() == r.Title.ToString())
                    {
                        listtarget.Add(r);
                    }
                }
            }
            var listview = lb_viewtype.SelectedItems.Cast<a.View>().ToList();
            foreach (var t in listview)
            {
                viewstransfer.Add(t.Id);
            }
            //foreach (var u in _data.listdoc)
            //{
            //    if (u.Title.ToString() == comboBox_proj.SelectedItem.ToString())
            //    {
            //        source = u;
            //    }
            //}
            foreach (var o in listtarget)
            {
                try
                {
                    _data.TransferViewproject(source, o, viewstransfer, Title_name, _data.versionnumber);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fail: " + ex.Message);
                }
            }
            if (pc != null)
            {
                pc.Dispose();
            }
            Close();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void TransferView_FormClosing(object sender, FormClosingEventArgs e)
        {
            pc?.Dispose();
        }
    }
}
