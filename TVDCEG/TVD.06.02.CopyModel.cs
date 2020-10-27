#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
#endregion

namespace TVDCEG
{
    public class CopyModel
    {
        private static CopyModel _instance;
        private CopyModel()
        {

        }
        public static CopyModel Instance => _instance ?? (_instance = new CopyModel());
        public List<Document> GetDocuments(Application app, UIDocument uidoc)
        {
            List<Document> list = new List<Document>();
            var Alldoc = app.Documents;
            var activedoc = uidoc.Application.ActiveUIDocument.Document;
            foreach (Document i in Alldoc)
            {
                if (i.Title == activedoc.Title) continue;
                list.Add(i);
            }
            return list;
        }
        public void Copyelement(Document Source, Document Target, ICollection<ElementId> elementIds)
        {
            using (Transaction t = new Transaction(Target, "Copy Model"))
            {
                t.Start();
                FailureHandlingOptions options = t.GetFailureHandlingOptions();
                MyPreProcessor ignoreProcess = new MyPreProcessor();
                options.SetClearAfterRollback(true);
                options.SetFailuresPreprocessor(ignoreProcess);
                t.SetFailureHandlingOptions(options);
                try
                {
                    ElementTransformUtils.CopyElements(Source, elementIds, Target, Transform.Identity, new CopyPasteOptions());
                }
                catch
                {

                }
                t.Commit();
            }
        }
    }
}
