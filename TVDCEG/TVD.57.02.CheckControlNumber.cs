using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Shapes;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Newtonsoft.Json;
using TVDCEG.CEG_INFOR;
using TVDCEG.Extension;
using TVDCEG.LBR;
using TVDCEG.Ultis;
using Line = Autodesk.Revit.DB.Line;

namespace TVDCEG
{
    public class CheckControlNumber : ConstructorSingleton<CheckControlNumber>
    {
        public List<CEG_Product> GetallProducts(Document doc)
        {
            List<CEG_Product> list = new List<CEG_Product>();
            List<FamilyInstance> Col1 =(from x in new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralFraming).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>() where !x.Name.Contains("DO NOT USE") select x).ToList();
            foreach (var item in Col1)
            {
                bool flag = item.LookupParameter("CONTROL_MARK") != null;
                if (flag)
                {
                    CEG_Product _Product = new CEG_Product(doc,item);
                    list.Add(_Product);
                }
            }
            Sortlistproduct(list);
            return list;
        }
        public void Showview(Document doc, UIDocument uidoc, ICollection<ElementId> elementIds, string viewname)
        {
            View3D view3D = null;
            SetBoundingBox(doc, elementIds,ref view3D);
            uidoc.Selection.SetElementIds(elementIds);
            uidoc.ActiveView = view3D;
            
        }
        public View3D Get3Dview(Document doc)
        {
            View3D view3D = null;
            List<View3D> view3Ds = new List<View3D>();
            var col = new FilteredElementCollector(doc).OfClass(typeof(View3D)).WhereElementIsNotElementType().Cast<View3D>().ToList();
            foreach (var i in col)
            {
                if (i.IsTemplate) continue;
                view3Ds.Add(i);
            }
            view3D = view3Ds.First();
            return view3D;
        }
        public void Renumbercontrolnumber(Document doc,List<CEG_Product> list,int increase)
        {
            Transaction tran = new Transaction(doc, "EXT: Control number");
            tran.Start();
            for (int i =0; i < list.Count; i++)
            {
                double controlnumber = Convert.ToDouble(list[i].CONTROL_NUMBER);
                double NEWCTR = controlnumber + increase;
                Parameter pa = list[i].FamilyInstance.LookupParameter("CONTROL_NUMBER");
                var value = list[i].CONTROL_NUMBER.Length;
                string taget = string.Empty;
                switch (NEWCTR.ToString().Length)
                {
                    case 2:
                        list[i].CONTROL_NUMBER = string.Concat("0","0",NEWCTR.ToString());
                        taget = list[i].CONTROL_NUMBER;
                        break;
                    case 3:
                        list[i].CONTROL_NUMBER = string.Concat("0", NEWCTR.ToString());
                        taget = list[i].CONTROL_NUMBER;
                        break;
                    default:
                        break;
                }
                pa.Set(taget);
            }
            tran.Commit();
        }
        public void SetBoundingBox(Document doc, ICollection<ElementId> ids,ref View3D view3D)
        {
            try
            {
                view3D = Get3Dview(doc);
                bool flag = view3D == null;
                if (flag)
                {
                    TaskDialog.Show("Error", "Please go to 3D view");
                }
                else
                {
                    XYZ xyz = XYZ.Zero;
                    XYZ xyz2 = XYZ.Zero;
                    foreach (ElementId id in ids)
                    {
                        FamilyInstance familyInstance = doc.GetElement(id) as FamilyInstance;
                        bool flag2 = familyInstance == null;
                        if (!flag2)
                        {
                            BoundingBoxXYZ boundingBoxXYZ = familyInstance.get_BoundingBox(view3D);
                            XYZ min = boundingBoxXYZ.Min;
                            XYZ max = boundingBoxXYZ.Max;
                            bool flag3 = xyz.IsAlmostEqualTo(XYZ.Zero);
                            if (flag3)
                            {
                                xyz = min;
                            }
                            else
                            {
                                xyz = new XYZ(Math.Min(xyz.X, min.X), Math.Min(xyz.Y, min.Y), Math.Min(xyz.Z, min.Z));
                            }
                            bool flag4 = xyz2.IsAlmostEqualTo(XYZ.Zero);
                            if (flag4)
                            {
                                xyz2 = max;
                            }
                            else
                            {
                                xyz2 = new XYZ(Math.Max(xyz2.X, max.X), Math.Max(xyz2.Y, max.Y), Math.Max(xyz2.Z, max.Z));
                            }
                        }
                    }
                    Transaction transaction = new Transaction(doc, "SetBoundingBox");
                    transaction.Start();
                    BoundingBoxXYZ boundingBoxXYZ2 = new BoundingBoxXYZ();
                    boundingBoxXYZ2.Min = xyz - new XYZ(1.0, 1.0, 1.0);
                    boundingBoxXYZ2.Max = xyz2 + new XYZ(1.0, 1.0, 1.0);
                    view3D.IsSectionBoxActive = true;
                    view3D.SetSectionBox(boundingBoxXYZ2);
                    transaction.Commit();
                }
            }
            catch
            {
            }
        }
        public void Sortlistproduct(List<CEG_Product> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < list.Count; j++)
                {
                    if (!string.IsNullOrEmpty(list[i].CONTROL_NUMBER) && !string.IsNullOrEmpty(list[j].CONTROL_NUMBER))
                    {
                        if (Convert.ToDouble(list[i].CONTROL_NUMBER) < Convert.ToDouble(list[j].CONTROL_NUMBER))
                        {
                            var temp = list[i];
                            list[i] = list[j];
                            list[j] = temp;
                        }
                    }
                }
            }
        }
    }
}
