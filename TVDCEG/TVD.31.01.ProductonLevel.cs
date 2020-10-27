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
using TVDCEG.Ultis;
#endregion

namespace TVDCEG
{
    public class ProductonLevel
    {
        private static ProductonLevel _instance;
        private ProductonLevel()
        {

        }
        public static ProductonLevel Instance => _instance ?? (_instance = new ProductonLevel());
        public Dictionary<string, List<ProductCEG>> Getallproductbycontrolmark(Document doc)
        {
            Dictionary<string, List<ProductCEG>> dic = new Dictionary<string, List<ProductCEG>>();
            var col = (from x in new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralFraming).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>() where x.GetSubComponentIds().Count != 0 select x).ToList();
            foreach (var item in col)
            {
                ProductCEG productCEG = new ProductCEG(item);
                if (dic.ContainsKey(productCEG.Controlmark + "-" + productCEG.Workplane))
                {
                    dic[productCEG.Controlmark + "-" + productCEG.Workplane].Add(productCEG);
                }
                else
                {
                    dic.Add(productCEG.Controlmark + "-" + productCEG.Workplane, new List<ProductCEG> { productCEG });
                }
            }
            return dic;
        }
        public Dictionary<string, List<string>> Caculator(Dictionary<string, List<ProductCEG>> dic)
        {
            Dictionary<string, List<string>> dic1 = new Dictionary<string, List<string>>();
            foreach (var text in dic.Keys)
            {
                var val1 = text.Split('-').First();
                var val2 = text.Split('-').Last();
                var val3 = text.Split(':').Last();
                var val4 = val3.Split(' ').Last();
                if (dic1.ContainsKey(val1))
                {
                    dic1[val1].Add(dic[text].Count + " " + "(" + val4 + ")");
                }
                else
                {
                    dic1.Add(val1, new List<string> { dic[text].Count + " " + "(" + val4 + ")" });
                }
            }
            return dic1;
        }
        public List<TextNoteType> GetalltypeText(Document doc)
        {
            var colec = new FilteredElementCollector(doc).OfClass(typeof(TextNoteType)).Cast<TextNoteType>().ToList();
            return colec;
        }
        public void CreateTextNote(Document doc, Dictionary<string, List<string>> dic, TextNoteType textNoteType, Selection sel)
        {
            var point = sel.PickPoint();
            if (doc.ActiveView.IsAssemblyView)
            {
                XYZ direction = doc.ActiveView.UpDirection;
                string assemblyname = (doc.GetElement(doc.ActiveView.AssociatedAssemblyInstanceId) as AssemblyInstance).Name;
                var list = dic[assemblyname].ToList();
                list.Sort();
                Sortliststring(list);
                if (direction.X != 0)
                {
                    using (Transaction tran = new Transaction(doc, "Create new type text"))
                    {
                        XYZ xYZ = XYZ.Zero;
                        tran.Start();
                        for (int i = 0, j = 0; i < list.Count; i++, j += 1)
                        {
                            if (i == 0)
                            {
                                var p2 = new XYZ(point.X, point.Y, point.Z + 0.016);
                                xYZ = p2;
                                TextNote textNote = TextNote.Create(doc, doc.ActiveView.Id, xYZ, list[i], textNoteType.Id);
                            }
                            else
                            {
                                var p2 = new XYZ(xYZ.X, xYZ.Y, xYZ.Z + 0.016);
                                xYZ = p2;
                                TextNote textNote = TextNote.Create(doc, doc.ActiveView.Id, xYZ, list[i], textNoteType.Id);
                            }
                        }
                        tran.Commit();
                    }
                }
                if (direction.Y != 0)
                {
                    using (Transaction tran = new Transaction(doc, "Create new type text"))
                    {
                        XYZ xYZ = XYZ.Zero;
                        tran.Start();
                        for (int i = 0, j = 0; i < list.Count; i++, j += 1)
                        {
                            if (i == 0)
                            {
                                var p2 = new XYZ(point.X, point.Y + 0.016, point.Z);
                                xYZ = p2;
                                TextNote textNote = TextNote.Create(doc, doc.ActiveView.Id, xYZ, list[i], textNoteType.Id);
                            }
                            else
                            {
                                var p2 = new XYZ(xYZ.X, xYZ.Y + 0.016, xYZ.Z);
                                xYZ = p2;
                                TextNote textNote = TextNote.Create(doc, doc.ActiveView.Id, xYZ, list[i], textNoteType.Id);
                            }
                        }
                        tran.Commit();
                    }
                }
                if (direction.Z != 0)
                {
                    using (Transaction tran = new Transaction(doc, "Create new type text"))
                    {
                        XYZ xYZ = XYZ.Zero;
                        tran.Start();
                        for (int i = 0, j = 0; i < list.Count; i++, j += 1)
                        {
                            if (i == 0)
                            {
                                var p2 = new XYZ(point.X, point.Y, point.Z + 0.016);
                                xYZ = p2;
                                TextNote textNote = TextNote.Create(doc, doc.ActiveView.Id, xYZ, list[i], textNoteType.Id);
                            }
                            else
                            {
                                var p2 = new XYZ(xYZ.X, xYZ.Y, xYZ.Z + 0.016);
                                xYZ = p2;
                                TextNote textNote = TextNote.Create(doc, doc.ActiveView.Id, xYZ, list[i], textNoteType.Id);
                            }
                        }
                        tran.Commit();
                    }
                }
            }

        }
        public void Sortliststring(List<string> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < list.Count; j++)
                {
                    var val1 = Convertstringtodouble(list[i]);
                    var val2 = Convertstringtodouble(list[j]);
                    if(val1>val2)
                    {
                        var temp = list[i];
                        list[i] = list[j];
                        list[j] = temp;
                    }
                }
            }
        }
        public double Convertstringtodouble(string text)
        {
            var val1 = text.Split(' ').Last();
            var val2 = val1.Split('L').Last();
            val2 = val2.Replace(")", "");
            double x = Convert.ToDouble(val2);
            return x;
        }
    }
    public class ProductCEG
    {
        public string Controlmark { get; set; }
        public string Controlnumber { get; set; }
        public string Workplane { get; set; }
        public FamilyInstance instance;
        public string MEMBER_WEIGHT_CAST { get; set; }
        public ProductCEG(FamilyInstance familyInstance)
        {
            Autodesk.Revit.DB.Parameter parameter = familyInstance.get_Parameter(BuiltInParameter.SKETCH_PLANE_PARAM);
            if (parameter != null)
            {
                var value = parameter.AsString();
                if (value != null)
                {
                    Workplane = value;
                }
                else
                {
                    Workplane = "";
                }
            }
            instance = familyInstance;
            Parameter pa1 = familyInstance.LookupParameter("CONTROL_MARK");
            Parameter pa2 = familyInstance.LookupParameter("CONTROL_NUMBER");
            Parameter pa3 = familyInstance.LookupParameter("MEMBER_WEIGHT_CAST");
            if (pa1 != null)
            {
                Controlmark = pa1.AsString();
            }
            else
            {
                Controlmark = "";
            }
            if (pa2 != null)
            {
                Controlnumber = pa2.AsString();
            }
            else
            {
                Controlnumber = "";
            }
            if(pa3!=null)
            {
                MEMBER_WEIGHT_CAST = pa3.AsString();
            }
            else
            {
                MEMBER_WEIGHT_CAST = "";
            }
        }
    }
    public class SettingProductonlevel
    {
        private static SettingProductonlevel _instance;
        private SettingProductonlevel()
        {

        }
        public static SettingProductonlevel Instance => _instance ?? (_instance = new SettingProductonlevel());
        public string TypeText { get; set; }
        public string GetFolderPath()
        {
            string folderPath = SettingExtension.GetSettingPath() + "\\TVD.11.SettingBrick";
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
            return folderPath;
        }

        public string GetFileName()
        {
            return "SettingProductonlevel.json";
        }

        public string GetFullFileName()
        {
            return GetFolderPath() + "\\" + GetFileName();
        }

        public void SaveSetting()
        {
            var gh = GetFullFileName();
            SettingExtension.SaveSetting(this, GetFullFileName());
        }

        public SettingProductonlevel GetSetting()
        {
            SettingProductonlevel setting = SettingExtension.GetSetting<SettingProductonlevel>(GetFullFileName());
            if (setting == null) setting = new SettingProductonlevel();
            return setting;
        }
    }
}
