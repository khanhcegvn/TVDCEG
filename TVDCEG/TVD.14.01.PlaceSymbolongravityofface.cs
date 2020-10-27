#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TVDCEG.LBR;
using TVDCEG.Ultis;
#endregion

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class PlaceSymbolongravityFacecmd : IExternalCommand
    {
        public List<FamilySymbol> listsymbol = new List<FamilySymbol>();
        public IList<CurveLoop> curves = new List<CurveLoop>();
        public Settingplacesymbolongaravity Setting;
        public XYZ center;
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
            listsymbol = ListSymbol(doc);
            Setting = Settingplacesymbolongaravity.Instance.GetSetting();
            center = Doing(doc);
            Reference rf = sel.PickObject(ObjectType.Face);
            curves = GetCurveLoops(doc, rf);
            var form = new FrmPlaceSymbolGravity(this, doc);
            form.ShowDialog();

            return Result.Succeeded;
        }
        public void Placesymbol(Document doc, IList<CurveLoop> curveloop, FamilySymbol symbol)
        {
            XYZ point = Createsolid(doc, curveloop);
            using (Transaction tr = new Transaction(doc, "Place Symbol"))
            {
                tr.Start();
                doc.Create.NewFamilyInstance(point, symbol, doc.ActiveView);
                tr.Commit();
            }
        }
        public void Placesymbol(Document doc, XYZ xYZ, FamilySymbol familySymbol)
        {
            using (Transaction tr = new Transaction(doc, "Place Symbol"))
            {
                tr.Start();
                doc.Create.NewFamilyInstance(xYZ, familySymbol, doc.ActiveView);
                tr.Commit();
            }
        }
        IList<CurveLoop> GetCurveLoops(Document doc, Reference rf)
        {
            IList<CurveLoop> curveloop = new List<CurveLoop>();
            GeometryObject geometryObject = doc.GetElement(rf).GetGeometryObjectFromReference(rf);
            try
            {
                PlanarFace planarFace = geometryObject as PlanarFace;
                curveloop = planarFace.GetEdgesAsCurveLoops();
            }
            catch
            {
                HermiteFace hermiteFace = geometryObject as HermiteFace;
                if (hermiteFace == null)
                {
                    RuledFace ruledFace = geometryObject as RuledFace;
                    curveloop = ruledFace.GetEdgesAsCurveLoops();
                }
                else
                {
                    curveloop = hermiteFace.GetEdgesAsCurveLoops();
                }
            }
            return curveloop;
        }
        List<FamilySymbol> ListSymbol(Document doc)
        {
            var symbol = (from FamilySymbol x in new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_GenericAnnotation).OfClass(typeof(FamilySymbol)).Cast<FamilySymbol>() where x.Category.CategoryType == CategoryType.Annotation select x).ToList();
            return symbol;
        }
        XYZ Createsolid(Document doc, IList<CurveLoop> looplist)
        {
            View view = doc.ActiveView;
            XYZ point = view.ViewDirection;
            Solid solid = GeometryCreationUtilities.CreateExtrusionGeometry(looplist, point, 0.1);
            XYZ center = solid.ComputeCentroid();
            return center;
        }
        public XYZ Doing(Document doc)
        {
            var struc = Getstructuralframming(doc);
            List<Solid> solids = Solidhelper.AllSolids(struc);
            return GetCenterSolid(solids);
        }
        public FamilyInstance Getstructuralframming(Document doc)
        {
            AssemblyInstance instance = doc.GetElement(doc.ActiveView.AssociatedAssemblyInstanceId) as AssemblyInstance;
            ICollection<ElementId> member = instance.GetMemberIds();
            FamilyInstance familyInstance = doc.GetElement((from x in member where doc.GetElement(x).Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming select x).First()) as FamilyInstance;
            return familyInstance;
        }
        public XYZ GetCenterSolid(List<Solid> solids)
        {
            List<XYZ> list = new List<XYZ>();
            foreach (var item in solids)
            {
                list.Add(item.ComputeCentroid());
            }
            double Volumncount = 0;
            double x = 0;
            double y = 0;
            double z = 0;
            for (int i = 0; i < solids.Count; i++)
            {
                if (solids[i] != null)
                {
                    Volumncount = Volumncount + solids[i].Volume;
                }
            }
            for (int i = 0; i < solids.Count; i++)
            {
                if (solids[i] != null)
                {
                    x = x + solids[i].ComputeCentroid().X * solids[i].Volume / Volumncount;
                    y = y + solids[i].ComputeCentroid().Y * solids[i].Volume / Volumncount;
                    z = z + solids[i].ComputeCentroid().Z * solids[i].Volume / Volumncount;
                }
            }
            return new XYZ(x, y, z);
        }
        public Solid Unionsolid(List<Solid> solids)
        {
            List<Solid> MB = new List<Solid> { solids[1], solids[2] };
            Solid result = null;
            foreach (Solid item in solids)
            {
                if (result == null)
                {
                    result = item;
                }
                else
                {
                    try
                    {
                        var t = BooleanOperationsUtils.ExecuteBooleanOperation(result, item, BooleanOperationsType.Intersect);
                        if (t.Volume != 0)
                        {
                            result = BooleanOperationsUtils.ExecuteBooleanOperation(result, item, BooleanOperationsType.Union);
                        }
                    }
                    catch
                    {

                    }
                }
            }
            return result;
        }
    }
    public class Settingplacesymbolongaravity
    {
        private static Settingplacesymbolongaravity _intance;
        private Settingplacesymbolongaravity()
        {

        }
        public static Settingplacesymbolongaravity Instance => _intance ?? (_intance = new Settingplacesymbolongaravity());
        public string Symbolongravity { get; set; }
        public string GetFolderPath()
        {
            string folderPath = SettingExtension.GetSettingPath() + "\\TVD.14.Settingplacesymbolongravity";
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
            return folderPath;
        }

        public string GetFileName()
        {
            return "Settingplacesymbolongravity.json";
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

        public Settingplacesymbolongaravity GetSetting()
        {
            Settingplacesymbolongaravity setting = SettingExtension.GetSetting<Settingplacesymbolongaravity>(GetFullFileName());
            if (setting == null) setting = new Settingplacesymbolongaravity();
            return setting;
        }
    }
}
