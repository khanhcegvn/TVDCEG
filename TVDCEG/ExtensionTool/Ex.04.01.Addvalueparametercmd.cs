using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace TVDCEG.ExtensionTool
{
    [Transaction(TransactionMode.Manual)]
    public class Addvalueparametercmd : IExternalCommand
    {
        public Dictionary<string, Parameter> dicpa = new Dictionary<string, Parameter>();
        public Dictionary<string, List<FamilyInstance>> dic = new Dictionary<string, List<FamilyInstance>>();
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication application = commandData.Application;
            UIDocument activeUIDocument = application.ActiveUIDocument;
            Application application2 = application.Application;
            Document doc = activeUIDocument.Document;
            dicpa = GetParameterelements(doc);
            dic = Getallframing(doc);
            Transaction transaction = new Transaction(doc, "Add value parameter");
            transaction.Start();
            //using (FrmAddvalueParameter frmAddvalueParameter = new FrmAddvalueParameter(this))
            //{
            //    bool flag = frmAddvalueParameter.ShowDialog() != System.Windows.Forms.DialogResult.OK;
            //    if (flag)
            //    {
            //        transaction.RollBack();
            //        return Result.Cancelled;
            //    }
            //}
           
            List<FamilyInstance> list = new List<FamilyInstance>();
            foreach (KeyValuePair<string,List<FamilyInstance>> item in dic)
            {
                foreach (var item2 in item.Value)
                {
                    list.Add(item2);
                }
            }
            Addvalue(doc, list, "WEIGHT_PER_UNIT", "150");
            transaction.Commit();
            return Result.Succeeded;
        }
        public void Addvalue(Document doc, List<FamilyInstance> list, string pa,string value)
        {
            ProgressBarform progressBarform = new ProgressBarform(list.Count, "Running...");
            progressBarform.Show();
            foreach (var item in list)
            {
                progressBarform.giatri();
                if (progressBarform.iscontinue == false)
                {
                    break;
                }
                Parameter pa1 = item.LookupParameter(pa);
                if(pa1!=null)
                {
                    switch (pa1.StorageType)
                    {
                        case StorageType.String:
                            pa1.Set(value);
                            break;
                        case StorageType.Double:
                            pa1.Set(Convert.ToDouble(value));
                            break;
                        case StorageType.Integer:
                            pa1.Set(Convert.ToInt32(value));
                            break;
                        default:
                            break;
                    }
                }
            }
            progressBarform.Close();
        }
        public Dictionary<string, List<FamilyInstance>> Getallframing(Document doc)
        {
            Dictionary<string, List<FamilyInstance>> dic = new Dictionary<string, List<FamilyInstance>>();
            var structural = new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategory(BuiltInCategory.OST_StructuralFraming).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().ToList();
            foreach (var item in structural)
            {
                if (dic.ContainsKey(item.Name))
                {
                    dic[item.Name].Add(item);
                }
                else
                {
                    dic.Add(item.Name, new List<FamilyInstance> { item });
                }
            }
            dic.OrderByDescending(x => x.Key).ToList();
            return dic;
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
    }
}
