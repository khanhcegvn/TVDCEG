#region Namespaces
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
#endregion

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class SaveDetach : IExternalCommand
    {
        public Document doc;
        public string projectPath = null;
        public Autodesk.Revit.ApplicationServices.Application app;
        private string datetimesave;
        public List<Element> listelement = new List<Element>();
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            app = uiapp.Application;
            doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            string format = "dd/MM/yyyy";
            DateTime plt = DateTime.Now;
            string hh = plt.ToString(format);
            datetimesave = hh.Replace("/", "-");
            FrmSaveDetach form = new FrmSaveDetach(this);
            form.ShowDialog();
            projectPath = form.file;
            if (projectPath != null)
            {
                if (form.checkoption == false)
                {
                    SaveDetachworksets();
                }
                if (form.checkoption == true)
                {
                    SaveDetachDisworksets();
                }
            }
            if (projectPath == null && form.checkActivedoc == true)
            {
                if (form.checkoption == false)
                {
                    SaveDetachworksetsActivedoc();
                }
                if (form.checkoption == true)
                {
                    SaveDetachDisworksetsActivedoc();
                }
            }
            return Result.Succeeded;
        }
        public void SaveDetachDisworksets()
        {
            string mpath = "";
            string mpathOnlyFilename = "";
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            folderBrowserDialog1.Description = "Select Folder Where Revit Projects to be Saved in Local";
            folderBrowserDialog1.RootFolder = Environment.SpecialFolder.MyComputer;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                mpath = folderBrowserDialog1.SelectedPath;
                FileInfo filePath = new FileInfo(projectPath);
                ModelPath mp = ModelPathUtils.ConvertUserVisiblePathToModelPath(filePath.FullName);
                OpenOptions opt = new OpenOptions();
                opt.DetachFromCentralOption = DetachFromCentralOption.DetachAndDiscardWorksets;
                mpathOnlyFilename = filePath.Name;
                Document openedDoc = app.OpenDocumentFile(mp, opt);
                SaveAsOptions options = new SaveAsOptions();
                options.OverwriteExistingFile = true;
                ModelPath modelPathout = ModelPathUtils.ConvertUserVisiblePathToModelPath(mpath + "\\" + "Detached" + "_" + datetimesave + "_" + mpathOnlyFilename);
                openedDoc.SaveAs(modelPathout, options);
                openedDoc.Close(true);
            }
        }
        public void SaveDetachworksets()
        {
            string mpath = "";
            string mpathOnlyFilename = "";
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            folderBrowserDialog1.Description = "Select Folder Where Revit Projects to be Saved in Local";
            folderBrowserDialog1.RootFolder = Environment.SpecialFolder.MyComputer;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                mpath = folderBrowserDialog1.SelectedPath;
                FileInfo filePath = new FileInfo(projectPath);
                ModelPath mp = ModelPathUtils.ConvertUserVisiblePathToModelPath(filePath.FullName);
                OpenOptions opt = new OpenOptions();
                opt.DetachFromCentralOption = DetachFromCentralOption.DetachAndPreserveWorksets;
                mpathOnlyFilename = filePath.Name;
                Document openedDoc = app.OpenDocumentFile(mp, opt);
                SaveAsOptions options = new SaveAsOptions();
                WorksharingSaveAsOptions wokrshar = new WorksharingSaveAsOptions();
                wokrshar.SaveAsCentral = true;
                options.SetWorksharingOptions(wokrshar);
                ModelPath modelPathout = ModelPathUtils.ConvertUserVisiblePathToModelPath(mpath + "\\" + "Detached" + "_" + datetimesave + "_" + mpathOnlyFilename);
                openedDoc.SaveAs(modelPathout, options);
                openedDoc.Close(true);
            }
        }
        public void SaveDetachDisworksetsActivedoc()
        {
            string mpath = "";
            string mpathOnlyFilename = "";
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            folderBrowserDialog1.Description = "Select Folder Where Revit Projects to be Saved in Local";
            folderBrowserDialog1.RootFolder = Environment.SpecialFolder.MyComputer;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                mpath = folderBrowserDialog1.SelectedPath;
                FileInfo filePath = new FileInfo(doc.PathName);
                ModelPath mp = ModelPathUtils.ConvertUserVisiblePathToModelPath(filePath.FullName);
                OpenOptions opt = new OpenOptions();
                opt.DetachFromCentralOption = DetachFromCentralOption.DetachAndDiscardWorksets;
                mpathOnlyFilename = filePath.Name;
                Document openedDoc = app.OpenDocumentFile(mp, opt);
                SaveAsOptions options = new SaveAsOptions();
                options.OverwriteExistingFile = true;
                ModelPath modelPathout = ModelPathUtils.ConvertUserVisiblePathToModelPath(mpath + "\\" + "Detached" + "_" + datetimesave + "_" + mpathOnlyFilename);
                openedDoc.SaveAs(modelPathout, options);
                openedDoc.Close(true);
            }
        }
        public void SaveDetachworksetsActivedoc()
        {
            string mpath = "";
            string mpathOnlyFilename = "";
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            folderBrowserDialog1.Description = "Select Folder Where Revit Projects to be Saved in Local";
            folderBrowserDialog1.RootFolder = Environment.SpecialFolder.MyComputer;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                mpath = folderBrowserDialog1.SelectedPath;
                FileInfo filePath = new FileInfo(doc.PathName);
                ModelPath mp = ModelPathUtils.ConvertUserVisiblePathToModelPath(filePath.FullName);
                OpenOptions opt = new OpenOptions();
                opt.DetachFromCentralOption = DetachFromCentralOption.DetachAndPreserveWorksets;
                mpathOnlyFilename = filePath.Name;
                Document openedDoc = app.OpenDocumentFile(mp, opt);
                SaveAsOptions options = new SaveAsOptions();
                WorksharingSaveAsOptions wokrshar = new WorksharingSaveAsOptions();
                wokrshar.SaveAsCentral = false;
                options.SetWorksharingOptions(wokrshar);
                ModelPath modelPathout = ModelPathUtils.ConvertUserVisiblePathToModelPath(mpath + "\\" + "Detached" + "_" + datetimesave + "_" + mpathOnlyFilename);
                openedDoc.SaveAs(modelPathout, options);
                openedDoc.Close(true);
            }
        }
    }
}
