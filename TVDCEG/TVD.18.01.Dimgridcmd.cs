#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.IO;
using System.Linq;
using TVDCEG.LBR;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using TVDCEG.Ultis;
using System;
#endregion

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class Dimgridcmd : IExternalCommand
    {
        public Document doc;
        public ExternalEvent _event;
        public Selection sel;
        public SettingDimgrid Setting;
        public List<DimensionType> dimensionTypes = new List<DimensionType>();
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            doc = uidoc.Document;
            sel = uidoc.Selection;
            Setting = SettingDimgrid.Instance.GetSetting();
            dimensionTypes = DimBrick.Instance.GetDimensions(doc);
            var form = new FrmDimgrid(this, doc);
            form.ShowDialog();
            //XYZ point = sel.PickPoint();
            //Dimgrid(rf, point);
            return Result.Succeeded;
        }
        public void Dimgrid(IList<Reference> references, XYZ point)
        {
            List<Grid> grids = new List<Grid>();
            ReferenceArray referenceArray = new ReferenceArray();
            foreach (var item in references)
            {
                Grid grid = (Grid)doc.GetElement(item);
                grids.Add(grid);
            }
            foreach (var item in grids)
            {
                referenceArray.Append(GetGridReference(doc.ActiveView, item));
            }
            var p1 = GetGridDirection(doc.ActiveView, grids.First());
            var r1 = p1.CrossProduct(doc.ActiveView.ViewDirection);
            var enp = r1 + point;
            Line line = Line.CreateBound(point, enp);
            using (Transaction tran = new Transaction(doc, "Create dimesion"))
            {
                tran.Start();
                doc.Create.NewDimension(doc.ActiveView, line, referenceArray);
                tran.Commit();
            }
        }
        public void Dimelement(Document doc, DimensionType dimensionType, View view)
        {
            Dictionary<string, List<Grid>> dic = GridCollection(doc, view);
            foreach (var item in dic.Keys)
            {
                ReferenceArray referenceArray = new ReferenceArray();
                ReferenceArray referenceArray2 = new ReferenceArray();
                foreach (var item2 in dic[item])
                {
                    referenceArray.Append(GetGridReference(doc.ActiveView, item2));
                }
                referenceArray2.Append(GetGridReference(doc.ActiveView, dic[item].First()));
                referenceArray2.Append(GetGridReference(doc.ActiveView, dic[item].Last()));
                var p1 = GetGridDirection(doc.ActiveView, dic[item].First());
                var p2 = GetGridDirection(doc.ActiveView, dic[item].Last());
                var r1 = p1.CrossProduct(doc.ActiveView.ViewDirection);
                var curve = dic[item].First().Curve;
                var curve2 = dic[item].Last().Curve;
                Line line1 = curve as Line;
                if (line1.Direction.Equalpoint(doc.ActiveView.RightDirection))
                {
                    XYZ point = Findpointdimension(curve.GetEndPoint(0), p1, 450, view);
                    XYZ point2 = Findpointdimension(curve2.GetEndPoint(0), p2, 450, view);
                    XYZ point3 = Findpointdimension(curve.GetEndPoint(0), p1, 200, view);
                    XYZ point4 = Findpointdimension(curve2.GetEndPoint(0), p2, 200, view);
                    Line line = Line.CreateBound(point, point2);
                    Line line2 = Line.CreateBound(point3, point4);
                    using (Transaction tran = new Transaction(doc, "Create dimesion"))
                    {
                        tran.Start();
                        doc.Create.NewDimension(doc.ActiveView, line, referenceArray, dimensionType);
                        doc.Create.NewDimension(doc.ActiveView, line2, referenceArray2, dimensionType);
                        tran.Commit();
                    }
                }
                if (line1.Direction.Equalpoint(doc.ActiveView.UpDirection))
                {
                    XYZ point = Findpointdimension(curve.GetEndPoint(0), p1, 450, view);
                    XYZ point2 = Findpointdimension(curve2.GetEndPoint(0), p2, 450, view);
                    XYZ point3 = Findpointdimension(curve.GetEndPoint(0), p1, 200, view);
                    XYZ point4 = Findpointdimension(curve2.GetEndPoint(0), p2, 200, view);
                    Line line = Line.CreateBound(point, point2);
                    Line line2 = Line.CreateBound(point3, point4);
                    using (Transaction tran = new Transaction(doc, "Create dimesion"))
                    {
                        tran.Start();
                        doc.Create.NewDimension(doc.ActiveView, line, referenceArray, dimensionType);
                        doc.Create.NewDimension(doc.ActiveView, line2, referenceArray2, dimensionType);
                        tran.Commit();
                    }
                }
            }
        }
        public XYZ Findpointdimension(XYZ A, XYZ V, double vm, View view)
        {
            XYZ diem = null;
            var val = vm /*/ Convert.ToDouble(view.Scale)*/;
            var tong = Math.Pow(V.X, 2) + Math.Pow(V.Y, 2) + Math.Pow(V.Z, 2);
            var x1 = A.X + ((V.X) * val / (tong));
            var y1 = A.Y + ((V.Y) * val / (tong));
            var z1 = A.Z + ((V.Z) * val / (tong));
            var x2 = A.X - ((V.X) * val / (tong));
            var y2 = A.Y - ((V.Y) * val / (tong));
            var z2 = A.Z - ((V.Z) * val / (tong));
            XYZ diem1 = new XYZ(x1, y1, z1);
            XYZ diem2 = new XYZ(x2, y2, z2);
            XYZ Adiem1 = new XYZ(x1 - A.X, y1 - A.Y, z1 - A.Z);
            XYZ Adiem2 = new XYZ(x2 - A.X, y2 - A.Y, z2 - A.Z);
            if (Util.Nguochuong(Adiem1, V))
            {
                diem = diem1;
            }
            if (Util.Nguochuong(Adiem2, V))
            {
                diem = diem2;
            }
            return diem;
        }
        Dictionary<string, List<Grid>> GridCollection(Document doc, View view)
        {
            var col = new FilteredElementCollector(doc).WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_Grids).OfClass(typeof(Grid)).Cast<Grid>().ToList();
            Dictionary<string, List<Grid>> dic = new Dictionary<string, List<Grid>>();
            foreach (var item in col)
            {
                if (dic.ContainsKey(Dicrectiline(item)))
                {
                    dic[(Dicrectiline(item))].Add(item);
                }
                else
                {
                    dic.Add((Dicrectiline(item)), new List<Grid> { item });
                }
            }
            return dic;
        }
        public string Dicrectiline(Grid grid)
        {
            Line line = grid.Curve as Line;
            return line.Direction.ToString();
        }
        public Reference GetGridReference(View view, Grid grid)
        {
            Reference reference = null;
            GeometryElement geometryElement = grid.get_Geometry(new Options
            {
                ComputeReferences = true,
                IncludeNonVisibleObjects = true,
                View = view
            });
            bool flag = geometryElement == null;
            Reference result;
            if (flag)
            {
                result = null;
            }
            else
            {
                foreach (GeometryObject geometryObject in geometryElement)
                {
                    bool flag2 = geometryObject is Line;
                    if (flag2)
                    {
                        Line line = geometryObject as Line;
                        reference = line.Reference;
                    }
                }
                result = reference;
            }
            return result;
        }
        public XYZ GetGridDirection(View view, Grid grid)
        {
            XYZ result = new XYZ();
            Curve curve = null;
            double num = double.MinValue;
            foreach (Curve curve2 in grid.GetCurvesInView(DatumExtentType.Model, view))
            {
                double length = curve2.Length;
                bool flag = length > num;
                if (flag)
                {
                    curve = curve2;
                    num = length;
                }
            }
            bool flag2 = curve != null;
            if (flag2)
            {
                result = curve.GetEndPoint(1) - curve.GetEndPoint(0);
            }
            return result;
        }
    }
    public class SettingDimgrid
    {
        private static SettingDimgrid _intance;
        private SettingDimgrid()
        {

        }
        public static SettingDimgrid Instance => _intance ?? (_intance = new SettingDimgrid());
        public string Dimtype { get; set; }
        public string GetFolderPath()
        {
            string folderPath = SettingExtension.GetSettingPath() + "\\TVD.18.SettingDimgrid";
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
            return folderPath;
        }

        public string GetFileName()
        {
            return "SettingDimgrid.json";
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
        public SettingDimgrid GetSetting()
        {
            SettingDimgrid setting = SettingExtension.GetSetting<SettingDimgrid>(GetFullFileName());
            if (setting == null) setting = new SettingDimgrid();
            return setting;
        }
    }
}
