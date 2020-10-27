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
#endregion

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class Copyvaluedimesioncmd : IExternalCommand
    {
        public Document doc;
        public ExternalEvent _event;
        public UIDocument uidoc;
        public Selection sel;
        string Above;
        string Prefix;
        string Suffix;
        string Below;
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
            bool iscontinue = true;
            Reference reference = sel.PickObject(ObjectType.PointOnElement);
            XYZ globalPoint = reference.GlobalPoint;
            Element element = doc.GetElement(reference);
            Dimension dimension = element as Dimension;
            DimensionSegment dimensionSegment = null;
            double num = double.MaxValue;
            foreach (object obj in dimension.Segments)
            {
                DimensionSegment dimensionSegment2 = (DimensionSegment)obj;
                XYZ origin = dimensionSegment2.Origin;
                double num2 = origin.DistanceTo(globalPoint);
                bool flag = num > num2;
                if (flag)
                {
                    dimensionSegment = dimensionSegment2;
                    num = num2;
                }
            }
            bool flag2 = dimensionSegment != null;
            if (flag2)
            {
                Above = dimensionSegment.Above;
                Prefix = dimensionSegment.Prefix;
                Suffix = dimensionSegment.Suffix;
                Below = dimensionSegment.Below;
            }
            try
            {
                while (iscontinue)
                {
                    using (Transaction tran = new Transaction(doc,"Paste"))
                    {
                        tran.Start();
                        Reference reference2 = sel.PickObject(ObjectType.PointOnElement);
                        XYZ globalPoint2 = reference2.GlobalPoint;
                        Element element2 = doc.GetElement(reference);
                        bool flag = element2 != null;
                        if (flag)
                        {
                            Dimension dimension2 = element2 as Dimension;
                            bool flag3 = dimension2 == null;
                            if (flag3)
                            {
                                continue;
                            }
                            DimAddAnnotation(dimension2, globalPoint2, Above, Prefix, Suffix, Below);
                        }
                        tran.Commit();
                    }
                }
            }
            catch
            {
                iscontinue = false;
                return Result.Succeeded;
            }

            return Result.Succeeded;
        }
        public void DimAddAnnotation(Dimension dim, XYZ globalPoint, string above, string prefix, string suffix, string below)
        {
            DimensionSegment dimensionSegment = null;
            double num = double.MaxValue;
            foreach (object obj in dim.Segments)
            {
                DimensionSegment dimensionSegment2 = (DimensionSegment)obj;
                XYZ origin = dimensionSegment2.Origin;
                double num2 = origin.DistanceTo(globalPoint);
                bool flag = num > num2;
                if (flag)
                {
                    dimensionSegment = dimensionSegment2;
                    num = num2;
                }
            }
            bool flag2 = dimensionSegment != null;
            if (flag2)
            {
                dimensionSegment.Above = above;
                dimensionSegment.Prefix = prefix;
                dimensionSegment.Suffix = suffix;
                dimensionSegment.Below = below;
            }
        }

    }
}
