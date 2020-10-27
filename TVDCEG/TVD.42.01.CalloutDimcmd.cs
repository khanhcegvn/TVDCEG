#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using TVDCEG.LBR;
using TVDCEG.Ultis;
#endregion

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class CalloutDimcmd : IExternalCommand
    {
        public Document doc;
        public ExternalEvent _event;
        public UIDocument uidoc;
        public Selection sel;
        public bool iscontinue = true;
        public Dictionary<string, ViewSheet> dic_sheet = new Dictionary<string, ViewSheet>();
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            doc = uidoc.Document;
            sel = uidoc.Selection;
            FamilySymbol symbolembed1 = null;
            FamilySymbol symbolembed2 = null;
            FamilySymbol symbolrebar1 = null;
            FamilySymbol symbolrebar2 = null;
            Findfamilysymbol(doc, ref symbolembed1, ref symbolembed2, ref symbolrebar1, ref symbolrebar2);
            while (iscontinue)
            {
                Transaction tran = new Transaction(doc, "Callout dimension");
                tran.Start();
                try
                {
                    Reference reference = sel.PickObject(ObjectType.Element,/* new Filterdimention(),*/ "Select Dimension");
                    Dimension dimension = doc.GetElement(reference) as Dimension;
                    CSAZDimCallout dim = new CSAZDimCallout(doc, dimension, new XYZ(1, 0, 0));
                    if (dim.Type == "EMBED")
                    {
                        Direction direction = dim.Direction;
                        switch (direction)
                        {
                            case Direction.Horizontal:
                                PasteSymbolEmbed(doc, symbolembed1, sel, dim);
                                break;
                            case Direction.Vertical:
                                PasteSymbolEmbed(doc, symbolembed2, sel, dim);
                                break;
                            case Direction.Undefine:
                                PasteSymbolEmbed(doc, symbolembed1, sel, dim);
                                break;
                            default:
                                break;
                        }
                    }
                    if (dim.Type == "REBAR")
                    {
                        Direction direction = dim.Direction;
                        switch (direction)
                        {
                            case Direction.Horizontal:
                                PasteSymbolRebar(doc, symbolrebar1, sel, dim);
                                break;
                            case Direction.Vertical:
                                PasteSymbolRebar(doc, symbolrebar2, sel, dim);
                                break;
                            case Direction.Undefine:
                                PasteSymbolRebar(doc, symbolrebar1, sel, dim);
                                break;
                            default:
                                break;
                        }
                    }
                }
                catch (Exception)
                {
                    this.iscontinue = false;
                }

                tran.Commit();
            }
            return Result.Succeeded;

        }
        public FamilyInstance Findproduct(Document doc)
        {
            FamilyInstance familyInstance = null;
            if (doc.ActiveView.IsAssemblyView)
            {
                AssemblyInstance assemblyInstance = doc.GetElement(doc.ActiveView.AssociatedAssemblyInstanceId) as AssemblyInstance;
                var col = new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategory(BuiltInCategory.OST_StructuralFraming).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().ToList();
                foreach (var item in col)
                {
                    Parameter pa = item.LookupParameter("CONTROL_MARK");
                    if (pa != null)
                    {
                        var val = pa.AsString();
                        if (val.Equals(assemblyInstance.Name))
                        {
                            familyInstance = item as FamilyInstance;
                        }
                    }
                }
            }
            return familyInstance;
        }
        public void PasteSymbolEmbed(Document doc, FamilySymbol familySymbol, Selection sel, CSAZDimCallout dim)
        {
            XYZ p = null;
            XYZ point = sel.PickPoint();
            PLane3D pLane3D = new PLane3D(doc.ActiveView.Origin, doc.ActiveView.ViewDirection);
            XYZ vp = pLane3D.ProjectPointOnPlane(point);
            if (vp.DistanceTo(dim.Endpoint) > vp.DistanceTo(dim.Startpoint))
            {
                p = dim.Startpoint - dim.Line.Direction * 3.8;
            }
            else
            {
                p = dim.Endpoint + dim.Line.Direction * 2;
            }
            //p = dim.Startpoint - dim.Line.Direction * 3.8;
            FamilyInstance element = doc.Create.NewFamilyInstance(p, familySymbol, doc.ActiveView);
            Parameter p1 = element.LookupParameter("Part No.");
            p1.Set(dim.ControlMark);
            Parameter p2 = element.LookupParameter("TEXT 2");
            p2.Set(dim.Side);
            Parameter p3 = element.LookupParameter("TEXT 3");
            p3.Set(dim.Numbersegment);
        }
        public void PasteSymbolRebar(Document doc, FamilySymbol familySymbol, Selection sel, CSAZDimCallout dim)
        {
            //XYZ p = dim.Endpoint - dim.Line.Direction * 3.8;
            XYZ p = null;
            XYZ point = sel.PickPoint();
            PLane3D pLane3D = new PLane3D(doc.ActiveView.Origin, doc.ActiveView.ViewDirection);
            XYZ vp = pLane3D.ProjectPointOnPlane(point);
            if (vp.DistanceTo(dim.Endpoint) > vp.DistanceTo(dim.Startpoint))
            {
                p = dim.Startpoint - dim.Line.Direction * 3;
            }
            else
            {
                p = dim.Endpoint + dim.Line.Direction * 2;
            }
            FamilyInstance element = doc.Create.NewFamilyInstance(p, familySymbol, doc.ActiveView);
            Parameter p1 = element.LookupParameter("Part No.");
            p1.Set(dim.ControlMark);
            Parameter p2 = element.LookupParameter("TEXT 2");
            p2.Set(dim.Side);
            Parameter p3 = element.LookupParameter("TEXT 3");
            var ty = string.Concat("CL", " ", "(", dim.Quality, ")");
            p3.Set(ty);
        }
        public void Findfamilysymbol(Document doc, ref FamilySymbol symbol1, ref FamilySymbol symbol2, ref FamilySymbol symbol3, ref FamilySymbol symbol4)
        {
            var col = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_GenericAnnotation).OfClass(typeof(FamilySymbol)).Cast<FamilySymbol>().ToList();
            foreach (var item in col)
            {
                string _Name = item.FamilyName;
                switch (_Name)
                {
                    case ("ARI1"):
                        symbol1 = item;
                        break;
                    case ("ARI2"):
                        symbol2 = item;
                        break;
                    case ("AKSDADA1"):
                        symbol3 = item;
                        break;
                    case ("AKSDADA2"):
                        symbol4 = item;
                        break;
                    default:
                        break;
                }
            }
        }
        public XYZ FindCenterproduct(FamilyInstance familyInstance)
        {
            var list = Solidhelper.AllSolids(familyInstance);
            Solid solid = list.First();
            foreach (var item in list)
            {
                if (solid.Volume < item.Volume)
                {
                    solid = item;
                }
            }
            return solid.ComputeCentroid();
        }
    }
    public class CSAZDimCallout
    {
        private string _numbersegment;
        private int _quality;
        private string _side;
        private string _controlmark;
        private string _type;
        private XYZ _startpoint;
        private XYZ _endpoint;
        private Line _line;
        public XYZ VectorDirection { get; set; }
        public Direction Direction { get; set; }
        public string Numbersegment
        {
            get
            {
                return this._numbersegment;
            }
            set
            {
                this._numbersegment = value;
            }
        }
        public Line Line
        {
            get
            {
                return this._line;
            }
            set
            {
                this._line = value;
            }
        }
        public int Quality
        {
            get
            {
                return this._quality;
            }
            set
            {
                this._quality = value;
            }
        }
        public XYZ Startpoint
        {
            get
            {
                return this._startpoint;
            }
            set
            {
                this._startpoint = value;
            }
        }
        public XYZ Endpoint
        {
            get
            {
                return this._endpoint;
            }
            set
            {
                this._endpoint = value;
            }
        }
        public string Type
        {
            get
            {
                return this._type;
            }
            set
            {
                this._type = value;
            }
        }
        public string Side
        {
            get
            {
                return this._side;
            }
            set
            {
                this._side = value;
            }
        }
        public string ControlMark
        {
            get
            {
                return this._controlmark;
            }
            set
            {
                this._controlmark = value;
            }
        }
        public CSAZDimCallout(Document doc, Dimension dimension, XYZ point)
        {
            var segment = dimension.NumberOfSegments - 1;
            this._numbersegment = string.Concat("CL", " ", "(", segment, ")");
            Member(doc, dimension, ref this._controlmark, ref this._type);
            var dic = Getelementview(doc, doc.ActiveView);
            foreach (KeyValuePair<string, List<FamilyInstance>> item in dic)
            {
                if (_controlmark.Equals(item.Key))
                {
                    this._quality = item.Value.Count;
                }
            }
            Line line = dimension.Curve as Line;
            XYZ center1 = line.Origin;
            PLane3D pLane3D = new PLane3D(doc.ActiveView.Origin, doc.ActiveView.ViewDirection);
            var center = pLane3D.ProjectPointOnPlane(center1);
            if (center.Y > point.Y)
            {
                this._side = "SIDE 'A'";
            }
            else
            {
                this._side = "SIDE 'B'";
            }
            XYZ p1 = GetDimensionStartPoint(dimension);
            List<XYZ> pts1 = GetDimensionPoints(dimension, p1);
            _line = dimension.Curve as Line;
            _line.MakeBound(0, 1);
            double length = 0;
            DimensionSegmentArray dimensionSegmentarr = dimension.Segments;
            for (int i = 0; i < dimensionSegmentarr.Size; i++)
            {
                double val = dimensionSegmentarr.get_Item(i).Value.Value;
                length = length + val;
            }
            //_startpoint = pLane3D.ProjectPointOnPlane(_line.GetEndPoint(0));
            //_endpoint = _startpoint + line.Direction * length;
            _startpoint = pts1.First();
            _endpoint = pts1.Last();
            GetDimDirection(dimension);
            Direction = GetDirection(doc.ActiveView, dimension);
            VectorDirection = (_startpoint - _endpoint).Normalize();
        }
        List<XYZ> GetDimensionPoints(Dimension dim, XYZ pStart)
        {
            Line dimLine = dim.Curve as Line;
            if (dimLine == null) return null;
            List<XYZ> pts = new List<XYZ>();

            dimLine.MakeBound(0, 1);
            XYZ pt1 = dimLine.GetEndPoint(0);
            XYZ pt2 = dimLine.GetEndPoint(1);
            XYZ direction = pt2.Subtract(pt1).Normalize();

            if (0 == dim.Segments.Size)
            {
                XYZ v = 0.5 * (double)dim.Value * direction;
                pts.Add(pStart - v);
                pts.Add(pStart + v);
            }
            else
            {
                XYZ p = pStart;
                foreach (DimensionSegment seg in dim.Segments)
                {
                    XYZ v = (double)seg.Value * direction;
                    if (0 == pts.Count)
                    {
                        pts.Add(p = (pStart - 0.5 * v));
                    }
                    pts.Add(p = p.Add(v));
                }
            }
            return pts;
        }
        XYZ GetDimensionStartPoint(Dimension dim)
        {
            XYZ p = null;

            try
            {
                p = dim.Origin;
            }
            catch (Autodesk.Revit.Exceptions.ApplicationException ex)
            {
                Debug.Assert(ex.Message.Equals("Cannot access this method if this dimension has more than one segment."));

                foreach (DimensionSegment seg in dim.Segments)
                {
                    p = seg.Origin;
                    break;
                }
            }
            return p;
        }
        Direction GetDirection(View view, Dimension dimension)
        {
            var result = Direction.Undefine;
            XYZ direc = GetDimDirection(dimension);
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
        private XYZ GetDimDirection(Dimension dimension)
        {
            XYZ result = new XYZ();
            Curve curve = dimension.Curve;
            curve.MakeBound(0, 1);
            result = curve.GetEndPoint(1) - curve.GetEndPoint(0);
            return result;
        }
        public Dictionary<string, List<FamilyInstance>> Getelementview(Document doc, View view)
        {
            Dictionary<string, List<FamilyInstance>> dic = new Dictionary<string, List<FamilyInstance>>();
            var col = (from FamilyInstance x in new FilteredElementCollector(doc, view.Id).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().ToList() where x.HaveParameterInTypeorRebar(doc, "CONTROL_MARK") select x).ToList();
            try
            {
                foreach (var element in col)
                {
                    ElementId elementId = element.GetTypeId();
                    Element eletype = doc.GetElement(elementId);
                    int sorting_order = eletype.LookupParameter("SORTING_ORDER").AsInteger();
                    string val = string.Empty;
                    if (sorting_order == 405)
                    {
                        Parameter Fy = element.LookupParameter("CONTROL_MARK");
                        if (Fy != null)
                        {
                            val = Fy.AsString();
                            if (dic.ContainsKey(val))
                            {
                                dic[val].Add(element);
                            }
                            else
                            {
                                dic.Add(val, new List<FamilyInstance> { element });
                            }
                        }
                    }
                    else
                    {
                        val = eletype.LookupParameter("CONTROL_MARK").AsString();
                        if (dic.ContainsKey(val))
                        {
                            dic[val].Add(element);
                        }
                        else
                        {
                            dic.Add(val, new List<FamilyInstance> { element });
                        }
                    }
                }
            }
            catch
            {

            }
            return dic;
        }
        private void Member(Document doc, Dimension dimension, ref string hh, ref string type)
        {
            ReferenceArray referenceArray = dimension.References;
            List<string> list = new List<string>();
            for (int i = 1; i < referenceArray.Size - 1; i++)
            {
                Reference reference = referenceArray.get_Item(i);
                Element element = doc.GetElement(reference);

                if (element.Category.Id.IntegerValue != (int)BuiltInCategory.OST_StructuralFraming)
                {
                    ElementId elementId = element.GetTypeId();
                    Element eletype = doc.GetElement(elementId);
                    Parameter sorting_order = eletype.LookupParameter("SORTING_ORDER");
                    string val = string.Empty;
                    if (sorting_order != null)
                    {
                        if (sorting_order.AsInteger() == 405)
                        {
                            type = "REBAR";
                            val = element.LookupParameter("CONTROL_MARK").AsString();
                        }
                        else
                        {
                            type = "EMBED";
                            val = eletype.LookupParameter("CONTROL_MARK").AsString();
                        }
                    }
                    else
                    {
                        type = "EMBED";
                        val = element.LookupParameter("CONTROL_MARK").AsString();
                    }
                    list.Add(val);
                }
            }
            Removeduplicatestring(list);
            for (int i = 0; i < list.Count; i++)
            {
                if (i == 0)
                {
                    hh = list[i];
                }
                else
                {
                    hh = hh + "," + list[i];
                }
            }
        }
        private void Removeduplicatestring(List<string> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = i + 1; j < list.Count; j++)
                {
                    string s1 = list[i];
                    string s2 = list[j];
                    if (s1.Equals(s2))
                    {
                        list.RemoveAt(j);
                        j--;
                    }
                }
            }
        }
    }
}
