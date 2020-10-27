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
    public class Gridandlevelbuble
    {
        private static Gridandlevelbuble _instance;
        private Gridandlevelbuble()
        {

        }
        public static Gridandlevelbuble Instance => _instance ?? (_instance = new Gridandlevelbuble());

        public void Doing(Document doc, View view, BubbleEnds bubbleEnds, List<CEG_Grid> list)
        {
            using (Transaction tran = new Transaction(doc, "Ivention Tool: Show/ Hide Bubbles"))
            {
                tran.Start();
                foreach (var gridBb in list)
                {
                    if (gridBb.Direction == Direction.Horizontal)
                    {
                        if (bubbleEnds.Left == true)
                        {
                            gridBb.RGrid.ShowBubbleInView(gridBb.Left, view);
                        }
                        if (bubbleEnds.Left == false)
                        {
                            gridBb.RGrid.HideBubbleInView(gridBb.Left, view);
                        }

                        if (bubbleEnds.Right == true)
                        {
                            gridBb.RGrid.ShowBubbleInView(gridBb.Right, view);
                        }
                        if (bubbleEnds.Right == false)
                        {
                            gridBb.RGrid.HideBubbleInView(gridBb.Right, view);
                        }
                    }

                    if (gridBb.Direction == Direction.Vertical)
                    {
                        if (bubbleEnds.Bottom == true)
                        {
                            gridBb.RGrid.ShowBubbleInView(gridBb.Bottom, view);
                        }
                        if (bubbleEnds.Bottom == false)
                        {
                            gridBb.RGrid.HideBubbleInView(gridBb.Bottom, view);
                        }

                        if (bubbleEnds.Top == true)
                        {
                            gridBb.RGrid.ShowBubbleInView(gridBb.Top, view);
                        }
                        if (bubbleEnds.Top == false)
                        {
                            gridBb.RGrid.HideBubbleInView(gridBb.Top, view);
                        }
                    }
                }
                tran.Commit();
            }

        }
        public void Doinglevel(Document doc, View view, BubbleEnds bubbleEnds, List<CEG_level> list)
        {
            using (Transaction tran = new Transaction(doc, "Ivention Tool: Show/ Hide Bubbles"))
            {
                tran.Start();
                foreach (var gridBb in list)
                {
                    if (gridBb.Direction == Direction.Horizontal)
                    {
                        if (bubbleEnds.Left == true)
                        {
                            gridBb.RLevel.ShowBubbleInView(gridBb.Left, view);
                        }
                        if (bubbleEnds.Left == false)
                        {
                            gridBb.RLevel.HideBubbleInView(gridBb.Left, view);
                        }

                        if (bubbleEnds.Right == true)
                        {
                            gridBb.RLevel.ShowBubbleInView(gridBb.Right, view);
                        }
                        if (bubbleEnds.Right == false)
                        {
                            gridBb.RLevel.HideBubbleInView(gridBb.Right, view);
                        }
                    }
                }
                tran.Commit();
            }
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
        public Line GetGridLine(View view, Grid grid)
        {
            Line _line = null;
            GeometryElement geometryElement = grid.get_Geometry(new Options
            {
                ComputeReferences = true,
                IncludeNonVisibleObjects = true,
                View = view
            });
            bool flag = geometryElement == null;
            Line result;
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
                        _line = line;
                    }
                }
                result = _line;
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
    public class CEG_level
    {
        public Direction Direction { get; set; }
        public DatumEnds Left { get; set; }
        public DatumEnds Right { get; set; }
        public Level RLevel
        {
            get;
            set;
        }
        Direction GetDirection(View view, Level level)
        {
            var result = Direction.Undefine;
            XYZ direc = GetGridDirection(view, level);
            var gh = direc.Normalize();
            if (gh.DotProduct(view.RightDirection) != 0)
            {
                result = Direction.Horizontal;
            }
            else
            {
                result = Direction.Vertical;
            }
            return result;
        }
        private XYZ GetGridDirection(View view, Level level)
        {
            XYZ result = new XYZ();
            Curve curve = null;
            double num = double.MinValue;
            foreach (Curve curve2 in level.GetCurvesInView(DatumExtentType.Model, view))
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
        public Curve Getcurvelevel(View view, Level level)
        {
            Curve curve = null;
            var list = level.GetCurvesInView(DatumExtentType.Model, view);
            curve = list.First();
            foreach (var item in list)
            {
                if (curve.ApproximateLength < item.ApproximateLength)
                {
                    curve = item;
                }
            }
            return curve;
        }
        public CEG_level(View view, Level level)
        {
            RLevel = level;
            Direction = GetDirection(view, level);
            if (Direction == Direction.Horizontal)
            {
                var curve = Getcurvelevel(view, level);
                var end0 = curve.GetEndPoint(0);
                var end1 = curve.GetEndPoint(1);
                var center = (end0 + end1) / 2;
                var flag = center - end0;
                if (view.UpDirection.Z != 0)
                {
                    if (!Util.Nguochuong(flag.Normalize(), view.RightDirection))
                    {
                        Left = DatumEnds.End0;
                        Right = DatumEnds.End1;
                    }
                    else
                    {
                        Right = DatumEnds.End0;
                        Left = DatumEnds.End1;
                    }
                }
                else
                {
                    if (Util.Nguochuong(flag.Normalize(), view.RightDirection))
                    {
                        Left = DatumEnds.End0;
                        Right = DatumEnds.End1;
                    }
                    else
                    {
                        Right = DatumEnds.End0;
                        Left = DatumEnds.End1;
                    }
                }
            }
        }
    }
    public class CEG_Grid
    {
        public Direction Direction { get; set; }
        public DatumEnds Left { get; set; }
        public DatumEnds Right { get; set; }
        public DatumEnds Top { get; set; }
        public DatumEnds Bottom { get; set; }
        public Grid RGrid
        {
            get;
            set;
        }
        Direction GetDirection(View view, Grid grid)
        {
            var result = Direction.Undefine;
            XYZ direc = GetGridDirection(view, grid);
            if (direc.DotProduct(view.UpDirection) == 0)
            {
                result = Direction.Horizontal;
            }
            else
            {
                result = Direction.Vertical;
            }
            return result;
        }
        private XYZ GetGridDirection(View view, Grid grid)
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
        public CEG_Grid(View view, Grid grid)
        {
            RGrid = grid;
            Direction = GetDirection(view, grid);
            if (Direction == Direction.Horizontal)
            {
                var curve = grid.Curve;
                var end0 = curve.GetEndPoint(0);
                var end1 = curve.GetEndPoint(1);
                var center = (end0 + end1) / 2;
                var flag = center - end0;
                if (view.UpDirection.Z != 0)
                {
                    if (!Util.Nguochuong(flag, view.RightDirection))
                    {
                        Left = DatumEnds.End0;
                        Right = DatumEnds.End1;
                    }
                    else
                    {
                        Right = DatumEnds.End0;
                        Left = DatumEnds.End1;
                    }
                }
                else
                {
                    if (Util.Nguochuong(flag, view.RightDirection))
                    {
                        Left = DatumEnds.End0;
                        Right = DatumEnds.End1;
                    }
                    else
                    {
                        Right = DatumEnds.End0;
                        Left = DatumEnds.End1;
                    }
                }
            }
            if (Direction == Direction.Vertical)
            {
                var curve = grid.Curve;
                var end0 = curve.GetEndPoint(0);
                var end1 = curve.GetEndPoint(1);
                var center = (end0 + end1) / 2;
                var flag = center - end0;
                if (view.UpDirection.Z != 0)
                {
                    if (!Util.Nguochuong(flag, view.UpDirection))
                    {
                        Bottom = DatumEnds.End0;
                        Top = DatumEnds.End1;
                    }
                    else
                    {
                        Top = DatumEnds.End0;
                        Bottom = DatumEnds.End1;
                    }
                }
                else
                {
                    if (Util.Nguochuong(flag, view.UpDirection))
                    {
                        Bottom = DatumEnds.End0;
                        Top = DatumEnds.End1;
                    }
                    else
                    {
                        Top = DatumEnds.End0;
                        Bottom = DatumEnds.End1;
                    }
                }
            }
        }
    }
    public class BubbleEnds
    {
        public bool? Left { get; set; }
        public bool? Right { get; set; }
        public bool? Top { get; set; }
        public bool? Bottom { get; set; }
    }
}
