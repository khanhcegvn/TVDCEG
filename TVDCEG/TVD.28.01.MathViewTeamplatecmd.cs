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
#endregion

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class MathViewTemplatecmd : IExternalCommand
    {
        public Document doc;
        public ExternalEvent _event;
        public UIDocument uidoc;
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
            var view = Getviewtemplate(doc, sel);
            MathViewTemplate(doc, view, sel);
            return Result.Succeeded;
        }
        public View Getviewtemplate(Document doc, Selection sel)
        {
            Reference reference = sel.PickObject(ObjectType.Element);
            Viewport viewport = doc.GetElement(reference) as Viewport;
            var parameter = viewport.get_Parameter(BuiltInParameter.VIEW_TEMPLATE);
            ElementId viewtemplateid = parameter.AsElementId();
            return doc.GetElement(viewtemplateid) as View;
        }
        public Dictionary<string, View> Getallviewteamplate(Document doc)
        {
            var col = (from View x in new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(View)) where x.IsTemplate select x).Cast<View>().ToList();
            var dic = col.ToDictionary(x => x.Name, x => x);
            return dic;
        }
        public void MathViewTemplate(Document doc, View viewtemplate, Selection sel)
        {
            IList<Reference> references = sel.PickObjects(ObjectType.Element);
            List<Viewport> viewports = new List<Viewport>();
            references.ToList().ForEach(x => viewports.Add(doc.GetElement(x) as Viewport));
            foreach (Viewport item in viewports)
            {
                Parameter pa = item.get_Parameter(BuiltInParameter.VIEW_TEMPLATE);
                using (Transaction tran = new Transaction(doc, "Invention Ext: Set viewtemplate"))
                {
                    tran.Start();
                    pa.Set(viewtemplate.Id);
                    tran.Commit();
                }
            }
        }
    }
}
