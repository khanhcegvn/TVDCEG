#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Linq;
using Autodesk.Revit.UI.Selection;
#endregion

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class Selectbrickcmd : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            IList<Element> rfs = sel.PickElementsByRectangle(new Filterbrick(), "Select brick");
            List<ElementId> Ids = new List<ElementId>();
            rfs.ToList().ForEach(x => Ids.Add(x.Id));
            sel.SetElementIds(Ids);
            return Result.Succeeded;
        }
    }
    public class Filterbrick : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem.Name.Contains("BRICK") && elem.Category.Id.IntegerValue == (int)BuiltInCategory.OST_GenericModel) return true;
            else return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            throw new NotImplementedException();
        }
    }
}
