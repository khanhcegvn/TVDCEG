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
    public class StrandVisibilitycmd : IExternalCommand
    {
        public Document doc;
        public ExternalEvent _event;
        public UIDocument uidoc;
        public Selection sel;
        public double[] parameterStrand = new double[]
            {
                120,121,122,123,124,125,126,127
            };
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
            Doing(doc,doc.ActiveView);
            return Result.Succeeded;
        }
        public bool CheckElementParameter(double[] parameters, double a)
        {
            var result = (from x in parameters.ToList() where a == x select x).ToList();
            if (result.Count != 0) return true;
            else return false;
        }
        public void Doing(Document doc,View view)
        {
                var Allstrands = Getallstrand(doc);
            var strand1 = Allstrands.First();
            if(strand1.IsHidden(view))
            {
                using (Transaction tran = new Transaction(doc,"Unhide Strands"))
                {
                    tran.Start();
                    foreach (var item in Allstrands)
                    {
                        view.UnhideElements(new List<ElementId> { item.Id });
                    }
                    tran.Commit();
                }
            }
            else
            {
                using (Transaction tran = new Transaction(doc,"Hide Strands"))
                {
                    tran.Start();
                    foreach (var item in Allstrands)
                    {
                        view.HideElements(new List<ElementId> { item.Id });
                    }
                    tran.Commit();
                }
            }
        }
        public List<FamilyInstance> Getallstrand(Document doc)
        {
            var col = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_SpecialityEquipment).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().ToList();
            List<FamilyInstance> list = new List<FamilyInstance>();
            foreach (var item in col)
            {
                ElementId faid = item.GetTypeId();
                Element elemtype = doc.GetElement(faid);
                Parameter pa = elemtype.LookupParameter("SORTING_ORDER");
                if(pa!=null)
                {
                    var pa1 = pa.AsInteger();
                    double value = Convert.ToDouble(pa1);
                    if (CheckElementParameter(parameterStrand, value))
                    {
                        list.Add(item);
                    }
                }
            }
            return list;
        }
        public List<FamilyInstance> Getallstrandinview(Document doc)
        {
            var col = new FilteredElementCollector(doc,doc.ActiveView.Id).OfCategory(BuiltInCategory.OST_SpecialityEquipment).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().ToList();
            List<FamilyInstance> list = new List<FamilyInstance>();
            foreach (var item in col)
            {
                ElementId faid = item.GetTypeId();
                Element elemtype = doc.GetElement(faid);
                Parameter pa = elemtype.LookupParameter("SORTING_ORDER");
                if (pa != null)
                {
                    var pa1 = pa.AsInteger();
                    double value = Convert.ToDouble(pa1);
                    if (CheckElementParameter(parameterStrand, value))
                    {
                        list.Add(item);
                    }
                }
            }
            return list;
        }
    }
}
