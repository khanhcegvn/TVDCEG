#region Namespaces
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#endregion

namespace TVDCEG
{
    public class DetailRebar
    {
        private static DetailRebar _instance;
        private DetailRebar()
        {

        }
        public static DetailRebar Instance => _instance ?? (_instance = new DetailRebar());
        public Dictionary<string, List<Face>> GetTypeFace(FamilyInstance familyInstance)
        {
            Dictionary<string, List<Face>> dic = new Dictionary<string, List<Face>>();
            Options option = new Options();
            option.ComputeReferences = true;
            option.IncludeNonVisibleObjects = false;
            if (familyInstance != null)
            {
                GeometryElement geoElement = familyInstance.get_Geometry(option);
                foreach (GeometryObject geoObject in geoElement)
                {
                    GeometryInstance instance = geoObject as GeometryInstance;
                    if (null != instance)
                    {
                        GeometryElement instanceGeometryElement = instance.GetSymbolGeometry();
                        foreach (GeometryObject o in instanceGeometryElement)
                        {
                            Solid solid = o as Solid;
                            if (solid != null)
                            {
                                FaceArray kl = solid.Faces;
                                foreach (var i in kl)
                                {
                                    if (dic.ContainsKey(i.GetType().Name))
                                    {
                                        dic[i.GetType().Name].Add(i as Face);
                                    }
                                    else
                                    {
                                        dic.Add(i.GetType().Name, new List<Face> { i as Face });
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return dic;
        }
        public void Createline(Document doc, FamilyInstance familyInstance, XYZ point)
        {
            var dic = GetTypeFace(familyInstance);

            using (Transaction t = new Transaction(doc, "Create detail line"))
            {
                t.Start();
                var list = dic.Keys.ToList();
                foreach (var item in list)
                {
                    foreach (var i in dic[item])
                    {
                        EdgeArrayArray edgeArrayArray = i.EdgeLoops;
                        foreach (EdgeArray j in edgeArrayArray)
                        {
                            foreach (Edge k in j)
                            {
                                try
                                {
                                    Curve curve = k.AsCurve();
                                    View view = doc.ActiveView;
                                    var detailCurve = doc.Create.NewDetailCurve(view, curve);
                                    ElementTransformUtils.MoveElement(doc, detailCurve.Id, point);
                                }
                                catch
                                {

                                }
                            }
                        }
                    }
                }
                t.Commit();
            }
        }
        public void Createdrangting(Document doc, Selection sel, UIDocument uidoc)
        {
            var list = (from x in sel.PickObjects(ObjectType.Element) select x.ElementId).ToList();
            Createdrafting(doc, sel, uidoc, list);
        }

        List<ElementId> HideIsolate(Document doc, List<ElementId> elementIds)
        {
            List<ElementId> listhide = new List<ElementId>();
            var col = new FilteredElementCollector(doc, doc.ActiveView.Id).WhereElementIsNotElementType().OfCategory(BuiltInCategory.INVALID).Cast<Element>().ToList();
            var ml = (from x in col select x.Id).ToList();
            var query = ml.Except(elementIds).ToList();
            var g = (from x in query where doc.GetElement(x).IsHidden(doc.ActiveView) == false select x).ToList();
            using (Transaction t = new Transaction(doc, "Hide element"))
            {
                t.Start();
                foreach (var i in g)
                {
                    if (doc.GetElement(i).CanBeHidden(doc.ActiveView))
                    {
                        doc.ActiveView.HideElements(new List<ElementId> { i });
                        listhide.Add(i);
                    }
                }
                t.Commit();
            }
            return listhide;
        }
        void Createdrafting(Document doc, Selection sel, UIDocument uidoc, List<ElementId> list)
        {
            var list1 = HideIsolate(doc, list);
            uidoc.RefreshActiveView();
            string file = null;
            string file2 = null;
            try
            {
                using (Transaction tr = new Transaction(doc, "Delete"))
                {
                    tr.Start();
                    bool exported = false;
                    ElementId outid = ElementId.InvalidElementId;
                    DWGExportOptions dwgOptions = new DWGExportOptions();
                    dwgOptions.FileVersion = ACADVersion.R2007;
                    View v = null;
                    var option = new CopyPasteOptions();
                    option.SetDuplicateTypeNamesHandler(new CopyHandler());
                    ICollection<ElementId> views = new List<ElementId>();
                    views.Add(doc.ActiveView.Id);
                    var fd = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    exported = doc.Export(fd, "234567", views, dwgOptions);
                    file = Path.Combine(fd, "234567" + ".dwg");
                    file2 = Path.Combine(fd, "234567" + ".PCP");
                    var dwgimport = new DWGImportOptions();
                    dwgimport.ColorMode = ImportColorMode.BlackAndWhite;
                    if (exported)
                    {
                        v = CreateDrafting(doc);
                        doc.Import(file, dwgimport, v, out outid);
                        File.Delete(file);
                        File.Delete(file2);
                    }
                    if (doc.GetElement(outid).Pinned)
                    {
                        doc.GetElement(outid).Pinned = false;
                    }
                    ElementTransformUtils.CopyElements(v, new List<ElementId> { outid }, doc.ActiveView, Transform.Identity, option);
                    doc.ActiveView.UnhideElements(list1);
                    doc.Delete(v.Id);
                    tr.Commit();
                }
            }
            catch
            {
                File.Delete(file);
                File.Delete(file2);
            }

        }
        View CreateDrafting(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(ViewFamilyType));
            ViewFamilyType viewFamilyType = collector.Cast<ViewFamilyType>().First(vft => vft.ViewFamily == ViewFamily.Drafting);
            ViewDrafting drafting = ViewDrafting.Create(doc, viewFamilyType.Id);
            return (drafting as View);
        }
    }
}
public class CopyHandler : IDuplicateTypeNamesHandler
{
    public DuplicateTypeAction OnDuplicateTypeNamesFound(DuplicateTypeNamesHandlerArgs args)
    {
        return DuplicateTypeAction.UseDestinationTypes;
    }
}
