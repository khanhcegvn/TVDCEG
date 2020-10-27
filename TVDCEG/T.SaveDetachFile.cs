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
    public class SaveDetachFile : IExternalCommand
    {
        public Document doc;
        public Autodesk.Revit.ApplicationServices.Application app;
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
            SaveDetach(doc);
            return Result.Succeeded;
        }
        public void SaveDetach(Document doc)
        {
            System.Windows.Forms.OpenFileDialog theDialogRevit = new System.Windows.Forms.OpenFileDialog();
            theDialogRevit.Title = "Select Revit Project Files";
            theDialogRevit.Filter = "RVT files|*.rvt";
            theDialogRevit.FilterIndex = 1;
            theDialogRevit.InitialDirectory = @"C:\";
            theDialogRevit.Multiselect = true;
            if (theDialogRevit.ShowDialog() == DialogResult.OK)

            {
                string mpath = "";
                string mpathOnlyFilename = "";
                System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
                folderBrowserDialog1.Description = "Select Folder Where Revit Projects to be Saved in Local";
                folderBrowserDialog1.RootFolder = Environment.SpecialFolder.MyComputer;
                if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    mpath = folderBrowserDialog1.SelectedPath;
                    foreach (string projectPath in theDialogRevit.FileNames)
                    {
                        FileInfo filePath = new FileInfo(projectPath);
                        ModelPath mp = ModelPathUtils.ConvertUserVisiblePathToModelPath(filePath.FullName);
                        OpenOptions opt = new OpenOptions();
                        opt.DetachFromCentralOption = DetachFromCentralOption.DetachAndDiscardWorksets;
                        mpathOnlyFilename = filePath.Name;
                        Document openedDoc = app.OpenDocumentFile(mp, opt);
                        SaveAsOptions options = new SaveAsOptions();
                        options.OverwriteExistingFile = true;
                        ModelPath modelPathout = ModelPathUtils.ConvertUserVisiblePathToModelPath(mpath + "\\" + mpathOnlyFilename);
                        openedDoc.SaveAs(modelPathout, options);
                        openedDoc.Close(false);
                    }
                }
            }
        }

    }
}
