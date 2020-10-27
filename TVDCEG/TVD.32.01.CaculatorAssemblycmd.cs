#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Linq;
using System.Collections.Generic;
#endregion

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class CaculatorAssemblycmd : IExternalCommand
    {
        public Dictionary<string, List<FamilyInstance>> independentTags = new Dictionary<string, List<FamilyInstance>>();
        public Dictionary<string, Parameter> dicpa = new Dictionary<string, Parameter>();
        public Dictionary<string, List<ProductCEG>> dic = new Dictionary<string, List<ProductCEG>>();
        public List<AssemblyInstance> list = new List<AssemblyInstance>();
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
            dicpa = GetParameterelements(doc);
            dic = Getallproductbycontrolmark(doc);
            independentTags = Findsymboltag(doc);
            list = GetAssemblyInstances(doc);
            using (var form = new FrmAddValueAssembly(this))
            {
                if (form.ShowDialog() != true)
                {
                    if (form.check && form.All)
                    {
                        Setvalue(doc, form.tong, form.Parametertag);
                    }
                    if (form.check && !form.All)
                    {
                        Setvalue(doc, form.listinstance, form.Parametertag);
                    }
                }
            }
            return Result.Succeeded;
        }
        public static Dictionary<string, List<FamilyInstance>> Findsymboltag(Document doc)
        {
            Dictionary<string, List<FamilyInstance>> dic = new Dictionary<string, List<FamilyInstance>>();
            var col = new FilteredElementCollector(doc).WhereElementIsNotElementType().OfClass(typeof(IndependentTag)).Where(x => x.Category.Name == "Multi-Category Tags" || x.Category.Name == "Structural Framing Tags").Cast<IndependentTag>().ToList();
            foreach (var tag in col)
            {
                FamilyInstance instancesource = doc.GetElement(tag.TaggedLocalElementId) as FamilyInstance;
                if (instancesource.Category.Name != null)
                {
                    if (instancesource.Category.Name.Contains("Structural Framing"))
                    {
                        if (dic.ContainsKey(tag.Name))
                        {
                            dic[tag.Name].Add(instancesource);
                        }
                        else
                        {
                            dic.Add(tag.Name, new List<FamilyInstance> { instancesource });
                        }
                    }
                }
            }
            return dic;
        }
        public Dictionary<string, List<ProductCEG>> Getallproductbycontrolmark(Document doc)
        {
            Dictionary<string, List<ProductCEG>> dic = new Dictionary<string, List<ProductCEG>>();
            var col = (from x in new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralFraming).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>() where x.GetSubComponentIds().Count != 0 select x).ToList();
            foreach (var item in col)
            {
                ProductCEG productCEG = new ProductCEG(item);
                if (dic.ContainsKey(productCEG.Controlmark))
                {
                    dic[productCEG.Controlmark].Add(productCEG);
                }
                else
                {
                    dic.Add(productCEG.Controlmark, new List<ProductCEG> { productCEG });
                }
            }
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
        public List<AssemblyInstance> GetAssemblyInstances(Document doc)
        {
            var col = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Assemblies).OfClass(typeof(AssemblyInstance)).Cast<AssemblyInstance>().ToList();
            return col;
        }
        public FamilyInstance GetVolumnAssembly(Document doc, AssemblyInstance assemblyInstance, out string Val)
        {
            FamilyInstance familyInstance = null;
            List<FamilyInstance> list = new List<FamilyInstance>();
            var member = assemblyInstance.GetMemberIds().ToList();
            foreach (var item in member)
            {
                var e = doc.GetElement(item);
                if (e.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming)
                {
                    list.Add(e as FamilyInstance);
                    familyInstance = e as FamilyInstance;
                }
                if (e.Name.Contains("CORBEL"))
                {
                    list.Add(e as FamilyInstance);
                }
            }
            double Giatri = 0;
            foreach (var item in list)
            {
                Parameter volume = item.LookupParameter("Volume");
                double Value = volume.AsDouble();
                Giatri = Giatri + Value;
            }
            double kl = Giatri * 150;
            double k1 = Math.Round(kl / 1000, 1);
            double k2 = k1 * 1000;
            double k3 = Math.Ceiling(k2) + 50;
            Val = k3.ToString();
            return familyInstance;
        }
        public void Set(Document doc, List<AssemblyInstance> assemblyInstances, string value)
        {
            ProgressbarWPF progressBarform = new ProgressbarWPF(assemblyInstances.Count, "Loading...");
            progressBarform.Show();
            foreach (var item in assemblyInstances)
            {
                progressBarform.Giatri();
                if (progressBarform.iscontinue == false)
                {
                    break;
                }
                string giatri;
                var family = GetVolumnAssembly(doc, item, out giatri);
                Parameter commentdata = family.LookupParameter(value);
                using (Transaction tran = new Transaction(doc, "Set"))
                {
                    tran.Start();
                    commentdata.Set(giatri.ToString());
                    tran.Commit();
                }
            }
            progressBarform.Close();
        }
        public void Setvalue(Document doc, List<ProductCEG> list, string value)
        {
            ProgressbarWPF progressBarform = new ProgressbarWPF(list.Count, "Loading...");
            progressBarform.Show();
            foreach (var i in list)
            {
                progressBarform.Giatri();
                if (progressBarform.iscontinue == false)
                {
                    break;
                }
                Parameter commentdata = i.instance.LookupParameter(value);
                if (commentdata != null)
                {
                    string hj = commentdata.AsString();
                    using (Transaction tran = new Transaction(doc, "Set"))
                    {
                        tran.Start();
                        FamilyInstance flat = Flat(doc, i.instance);
                        if (flat != null)
                        {
                            Parameter fldata = flat.LookupParameter(value);
                            if (fldata != null)
                            {
                                Parameter volume = flat.LookupParameter("Volume");
                                double Value = volume.AsDouble();
                                double kl = Value * 150;
                                double k1 = Math.Round(kl / 1000, 1);
                                double k2 = k1 * 1000;
                                double k3 = Math.Ceiling(k2) + 50;
                                commentdata.Set(k3.ToString());
                                fldata.Set(k3.ToString());
                                string gh = commentdata.AsString();
                            }
                        }
                        else
                        {
                            Parameter volume = i.instance.LookupParameter("Volume");
                            double Value = volume.AsDouble();
                            double kl = Value * 150;
                            double k1 = Math.Round(kl / 1000, 1);
                            double k2 = k1 * 1000;
                            double k3 = Math.Ceiling(k2) + 50;
                            commentdata.Set(k3.ToString());
                            string gh = commentdata.AsString();
                        }
                        tran.Commit();
                    }
                }
            }
            progressBarform.Close();
        }
        public FamilyInstance Flat(Document doc, FamilyInstance familyInstance)
        {
            var list = familyInstance.GetSubComponentIds();
            FamilyInstance flat = null;
            foreach (var i in list)
            {
                Element ele = doc.GetElement(i);
                if (ele.Name.Contains("FLAT"))
                {
                    flat = ele as FamilyInstance;
                }
            }
            return flat;
        }
    }
}
