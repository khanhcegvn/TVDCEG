#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Linq;
using System.Collections.Generic;
using TVDCEG.LBR;
using TVDCEG;
using TVDCEG.Ultis;
using System.Windows.Documents;
#endregion

namespace TVDCEG.ExtensionTool
{
    [Transaction(TransactionMode.Manual)]
    public class TransferProductcodecmd : IExternalCommand
    {
        public Dictionary<string, Parameter> dicpa = new Dictionary<string, Parameter>();
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
            var list = sel.GetElementIds().ToList();
            List<FamilyInstance> Plist = new List<FamilyInstance>();
            list.ForEach(x => Plist.Add((FamilyInstance)doc.GetElement(x)));
            Setvalue(doc, Plist, "PRODUCT_CODE");
            //dicpa = GetParameterelements(doc);
            return Result.Succeeded;
        }
        public Dictionary<string, Parameter> GetParameterelements(Document doc)
        {
            Dictionary<string, Parameter> dic = new Dictionary<string, Parameter>();
            var col = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralFraming).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().ToList();
            foreach (var item in col)
            {
                var pas = item.ParametersMap;
                foreach (var item2 in pas)
                {
                    var value = item2 as Parameter;
                    if (dic.ContainsKey(value.Definition.Name))
                    {
                        continue;
                    }
                    else
                    {
                        dic.Add(value.Definition.Name, value);
                    }
                }
            }
            return dic;
        }
        public List<FamilyInstance> GetProducts(Document doc, string source, string target)
        {
            List<FamilyInstance> list = new List<FamilyInstance>();
            var col = new FilteredElementCollector(doc).WhereElementIsNotElementType().OfClass(typeof(IndependentTag)).Where(x => x.Category.Name == "Multi-Category Tags" || x.Category.Name == "Structural Framing Tags").Cast<IndependentTag>().ToList();
            ElementtransformToCopy elementtransformToCopy = new ElementtransformToCopy();
            List<FamilyInstance> listsource = new List<FamilyInstance>();
            foreach (var tag in col)
            {
                FamilyInstance instancesource = (doc.GetElement(tag.TaggedLocalElementId) as FamilyInstance).GetSuperInstances();
                if (instancesource.LookupParameter(source) != null && instancesource.LookupParameter(target) != null)
                {
                    list.Add(instancesource);
                }
            }
            return list;
        }
        public void Setvalue(Document doc, List<FamilyInstance> list, string source)
        {
            using (Transaction tran = new Transaction(doc, "Set"))
            {
                tran.Start();
                ProgressBarform progressBarform = new ProgressBarform(list.Count, "Loading...");
                progressBarform.Show();
                foreach (var i in list)
                {
                    progressBarform.giatri();
                    if (progressBarform.iscontinue == false)
                    {
                        break;
                    }
                    Parameter pa1 = i.LookupParameter(source);
                    if (pa1 != null)
                    {
                        ElementtransformToCopy tr = new ElementtransformToCopy();
                        FamilyInstance flat = tr.GetFlat(doc, i);
                        if(flat!=null)
                        {
                            FamilyInstance wapred = tr.GetWarped(doc, i);
                            Parameter Fpa1 = flat.LookupParameter(source);
                            Fpa1.Set(pa1.AsString());
                            Parameter Wpa1 = wapred.LookupParameter(source);
                            Wpa1.Set(pa1.AsString());
                        }
                    }
                }
                progressBarform.Close();
                tran.Commit();
            }
        }
    }
}
