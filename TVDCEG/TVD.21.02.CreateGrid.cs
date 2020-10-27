#region Namespaces
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System;
using System.Linq;
using System.Collections.Generic;
using TVDCEG.LBR;
#endregion

namespace TVDCEG
{
    public class CreateGrid
    {
        private static CreateGrid _instance;
        private CreateGrid()
        {

        }
        public static CreateGrid Instance => _instance ?? (_instance = new CreateGrid());
        public void NewGridX(Document doc, XYZ point, string tengrid, List<Layoutdatacreategrid> layoutdatacreategrids, List<Layoutdatacreategrid> DataY, ref List<Grid> GridX)
        {
            double kccan = 0;
            foreach (var item in DataY)
            {
                kccan = kccan + Convert.ToDouble(item.space) * Convert.ToDouble(item.number) + 15;
            }
            XYZ rigthdirection = doc.ActiveView.RightDirection;
            XYZ updirection = doc.ActiveView.UpDirection;
            XYZ p1 = FindTaghead(point, -updirection, 10, doc.ActiveView);
            XYZ newpoint = FindTaghead(p1, updirection, kccan, doc.ActiveView);
            Line line = Line.CreateBound(p1, newpoint);
            List<XYZ> list = new List<XYZ>();
            list.Add(p1);
            using (Transaction tran = new Transaction(doc, "Create Grid"))
            {
                tran.Start();
                var gridtypical = Grid.Create(doc, line);
                gridtypical.Name = tengrid;
                GridX.Add(gridtypical);
                foreach (var item in layoutdatacreategrids)
                {
                    for (int i = 0; i < Convert.ToDouble(item.number); i++)
                    {
                        XYZ xYZ = list.Last();
                        try
                        {
                            var grid = CreatenewGridX(doc, rigthdirection, updirection, Convert.ToDouble(item.space), kccan, ref list);
                            GridX.Add(grid);
                        }
                        catch
                        {

                        }
                    }
                }
                tran.Commit();
            }
        }
        public void NewGridY(Document doc, XYZ point, string tengrid, List<Layoutdatacreategrid> layoutdatacreategrids, List<Layoutdatacreategrid> DataX, ref List<Grid> GridY)
        {
            double kccan = 0;
            foreach (var item in DataX)
            {
                kccan = kccan + Convert.ToDouble(item.space) * Convert.ToDouble(item.number) + 10;
            }
            XYZ rigthdirection = doc.ActiveView.RightDirection;
            XYZ updirection = doc.ActiveView.UpDirection;
            XYZ p1 = FindTaghead(point, -rigthdirection, 10, doc.ActiveView);
            XYZ newpoint = FindTaghead(p1, rigthdirection, kccan, doc.ActiveView);
            Line line = Line.CreateBound(p1, newpoint);
            List<XYZ> list = new List<XYZ>();
            list.Add(p1);
            using (Transaction tran = new Transaction(doc, "Create Grid"))
            {
                tran.Start();
                var gridtypical = Grid.Create(doc, line);
                gridtypical.Name = tengrid;
                GridY.Add(gridtypical);
                foreach (var item in layoutdatacreategrids)
                {
                    for (int i = 0; i < Convert.ToDouble(item.number); i++)
                    {
                        XYZ xYZ = list.Last();
                        try
                        {
                            var grid = CreatenewGridY(doc, updirection, rigthdirection, Convert.ToDouble(item.space), kccan, ref list);
                            GridY.Add(gridtypical);
                        }
                        catch
                        {

                        }
                    }
                }
                tran.Commit();
            }
        }
        public string Dicrectiline(Grid grid)
        {
            Line line = grid.Curve as Line;
            return line.Direction.ToString();
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
        public void DimGrids(Document doc, View view, double kctoidaugrid, double kc2dim)
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
                var p1 = Directorline(dic[item].First());
                var p2 = Directorline(dic[item].Last());
                var curve = dic[item].First().Curve;
                var curve2 = dic[item].Last().Curve;
                XYZ starpoint1 = null;
                XYZ enpoint1 = null;
                XYZ startpoint2 = null;
                XYZ endpoint2 = null;
                Get2pointofgridingview(view, dic[item].First(), ref starpoint1, ref enpoint1);
                Get2pointofgridingview(view, dic[item].Last(), ref startpoint2, ref endpoint2);
                Line line1 = curve as Line;
                XYZ point = Findpointdimension(starpoint1, p1, UnitUtils.Convert(kctoidaugrid + kc2dim, DisplayUnitType.DUT_MILLIMETERS, DisplayUnitType.DUT_DECIMAL_FEET), view);
                XYZ point2 = Findpointdimension(startpoint2, p2, UnitUtils.Convert(kctoidaugrid + kc2dim, DisplayUnitType.DUT_MILLIMETERS, DisplayUnitType.DUT_DECIMAL_FEET), view);
                XYZ point3 = Findpointdimension(starpoint1, p1, UnitUtils.Convert(kctoidaugrid, DisplayUnitType.DUT_MILLIMETERS, DisplayUnitType.DUT_DECIMAL_FEET), view);
                XYZ point4 = Findpointdimension(startpoint2, p2, UnitUtils.Convert(kctoidaugrid, DisplayUnitType.DUT_MILLIMETERS, DisplayUnitType.DUT_DECIMAL_FEET), view);
                XYZ point5 = Findpointdimension(enpoint1, -p1, UnitUtils.Convert(kctoidaugrid + kc2dim, DisplayUnitType.DUT_MILLIMETERS, DisplayUnitType.DUT_DECIMAL_FEET), view);
                XYZ point6 = Findpointdimension(endpoint2, -p2, UnitUtils.Convert(kctoidaugrid + kc2dim, DisplayUnitType.DUT_MILLIMETERS, DisplayUnitType.DUT_DECIMAL_FEET), view);
                XYZ point7 = Findpointdimension(enpoint1, -p1, UnitUtils.Convert(kctoidaugrid, DisplayUnitType.DUT_MILLIMETERS, DisplayUnitType.DUT_DECIMAL_FEET), view);
                XYZ point8 = Findpointdimension(endpoint2, -p2, UnitUtils.Convert(kctoidaugrid, DisplayUnitType.DUT_MILLIMETERS, DisplayUnitType.DUT_DECIMAL_FEET), view);
                Line line = Line.CreateBound(point, point2);
                Line line2 = Line.CreateBound(point3, point4);
                Line line3 = Line.CreateBound(point5, point6);
                Line line4 = Line.CreateBound(point7, point8);
                using (Transaction tran = new Transaction(doc, "Create dimesion"))
                {
                    tran.Start();
                    //doc.Create.NewDimension(doc.ActiveView, line, referenceArray);
                    //doc.Create.NewDimension(doc.ActiveView, line2, referenceArray2);
                    doc.Create.NewDimension(doc.ActiveView, line3, referenceArray);
                    doc.Create.NewDimension(doc.ActiveView, line4, referenceArray2);
                    tran.Commit();
                }
            }
        }
        public Grid CreatenewGridX(Document doc, XYZ rightdirection, XYZ updirection, double kc, double kccan, ref List<XYZ> listpoint)
        {
            XYZ newpoint = FindTaghead(listpoint.Last(), rightdirection, kc, doc.ActiveView);
            listpoint.Add(newpoint);
            XYZ p2 = FindTaghead(newpoint, updirection, kccan, doc.ActiveView);
            Line line = Line.CreateBound(newpoint, p2);
            Grid grid = Grid.Create(doc, line);
            return grid;
        }
        public Grid CreatenewGridY(Document doc, XYZ rightdirection, XYZ updirection, double kc, double kccan, ref List<XYZ> listpoint)
        {
            XYZ newpoint = FindTaghead(listpoint.Last(), rightdirection, kc, doc.ActiveView);
            listpoint.Add(newpoint);
            XYZ p2 = FindTaghead(newpoint, updirection, kccan, doc.ActiveView);
            Line line = Line.CreateBound(newpoint, p2);
            Grid grid = Grid.Create(doc, line);
            return grid;
        }
        // set lai chieu dai grid
        public void SetCurveGridsX(View view, Grid gridX, Grid gridY, double extend)
        {
            XYZ directorX = Directorline(gridX);
            XYZ directorY = Directorline(gridY);
            XYZ startpointX = new XYZ();
            XYZ endpointX = new XYZ();
            XYZ startpointY = new XYZ();
            XYZ endpointY = new XYZ();
            Get2pointofgridingview(view, gridX, ref startpointX, ref endpointX);
            Get2pointofgridingview(view, gridY, ref startpointY, ref endpointY);
            var t = (startpointY.X - startpointX.X) / (directorX.X - directorY.X);
            var x = startpointX.X + (directorX.X * t);
            var y = startpointX.Y + (directorX.Y * t);
            var z = startpointX.Z + (directorX.Z * t);
            XYZ point = new XYZ(x, y, z);
            XYZ newpoint = Findpointdimension(point, directorX, extend, view);
            Line newline = Line.CreateBound(startpointX, point);
            gridX.SetCurveInView(DatumExtentType.Model, view, newline);
        }
        public void Get2pointofgridingview(View view, Grid grid, ref XYZ startpoint, ref XYZ endpoint)
        {
            GeometryElement geometryElement = grid.get_Geometry(new Options
            {
                ComputeReferences = true,
                IncludeNonVisibleObjects = true,
                View = view
            });
            foreach (GeometryObject geometryObject in geometryElement)
            {
                bool flag2 = geometryObject is Line;
                if (flag2)
                {
                    Line line = geometryObject as Line;
                    startpoint = line.GetEndPoint(0);
                    endpoint = line.GetEndPoint(1);
                }
            }
        }
        public XYZ Directorline(Grid grid)
        {
            Line line = grid.Curve as Line;
            return line.Direction;
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
        public XYZ FindTaghead(XYZ A, XYZ V, double vm, View view)
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
            if (!Util.Nguochuong(Adiem1, V))
            {
                diem = diem1;
            }
            if (!Util.Nguochuong(Adiem2, V))
            {
                diem = diem2;
            }
            return diem;
        }
        public XYZ Findpointdimension(XYZ A, XYZ V, double vm, View view)
        {
            XYZ diem = null;
            var val = vm;
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
    }
    public class Layoutdatacreategrid
    {
        public string number { get; set; }
        public string space { get; set; }
    }
}
