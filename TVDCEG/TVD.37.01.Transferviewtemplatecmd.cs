#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TVDCEG.LBR;
#endregion

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class Transferviewtemplatecmd : IExternalCommand
    {
        public Document doc;
        public ExternalEvent _event;
        public UIDocument uidoc;
        public List<Document> listdoc = new List<Document>();
        public Selection sel;
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            doc = uidoc.Document;
            sel = uidoc.Selection;
            listdoc = GetDocuments(app);
            using (var form = new FrmTransferviewtemplate(this, doc))
            {
                if (form.ShowDialog() != true && form.check == true)
                {
                    Transferviewtemplate(form.source, form.target, form.listtarget);
                }
            }
            return Result.Succeeded;
        }
        public List<View> GetAllviewtemplate(Document doc)
        {
            var col = (from x in new FilteredElementCollector(doc).OfClass(typeof(View)).Cast<View>() where x.IsTemplate select x).ToList();
            return col;
        }
        public void Transferviewtemplate(Document source, Document target, List<ElementId> elementIds)
        {
            using (Transaction tran = new Transaction(target, "Ivention EXT: Transfer view template"))
            {
                tran.Start();
                FailureHandlingOptions options = tran.GetFailureHandlingOptions();
                IgnoreProcess ignoreProcess = new IgnoreProcess();
                options.SetClearAfterRollback(true);
                options.SetFailuresPreprocessor(ignoreProcess);
                tran.SetFailureHandlingOptions(options);
                CopyPasteOptions coptions = new CopyPasteOptions();
                coptions.SetDuplicateTypeNamesHandler(new CopyHandler());
                ICollection<ElementId> litstrans = ElementTransformUtils.CopyElements(source, elementIds, target, null, coptions);
                tran.Commit(options);
            }
        }
        public List<Document> GetDocuments(Application app)
        {
            List<Document> list = new List<Document>();
            var Alldoc = app.Documents;
            foreach (Document i in Alldoc)
            {
                list.Add(i);
            }
            return list;
        }
    }
}
