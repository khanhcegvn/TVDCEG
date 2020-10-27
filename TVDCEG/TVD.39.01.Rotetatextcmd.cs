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
    public class Rotetatextcmd : IExternalCommand
    {
        public Document doc;
        public ExternalEvent _event;
        public UIDocument uidoc;
        public Selection sel;
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
            try
            {
                while (iscontinue)
                {
                    Reference reference = sel.PickObject(ObjectType.Element, new CEGFilterTextnote(), "Select text");
                    TextElement textElement = doc.GetElement(reference) as TextElement;
                    Reference rf = sel.PickObject(ObjectType.Element, new Filterdimention(), "Select text");
                    Dimension dimension = doc.GetElement(rf) as Dimension;
                    RotateText(doc, textElement, dimension);
                }
            }
            catch
            {
                iscontinue = false;
                return Result.Succeeded;
            }

            //Reference reference = sel.PickObject(ObjectType.Element, /*new CEGFilterTextnote(),*/ "Select text");
            //TextElement textElement = doc.GetElement(reference) as TextElement;
            //Reference rf = sel.PickObject(ObjectType.Element, /*new Filterdimention(),*/ "Select text");
            //Dimension dimension = doc.GetElement(rf) as Dimension;
            //RotateText(doc, textElement, dimension);



            return Result.Succeeded;
        }
        public XYZ Getdirectiondim(Dimension dimension)
        {
            Curve curve = dimension.Curve;
            Line line = curve as Line;
            return line.Direction;
        }
        public XYZ Getupdirectionoftext(TextElement textElement)
        {
            return textElement.BaseDirection;
        }
        public void RotateText(Document doc, TextElement textElement, Dimension dimension)
        {
            XYZ updirectiontext = Getupdirectionoftext(textElement);
            XYZ directionofdim = Getdirectiondim(dimension);
            double angle = directionofdim.AngleTo(updirectiontext);
            double Convertrado = UnitUtils.Convert(angle, DisplayUnitType.DUT_RADIANS, DisplayUnitType.DUT_DECIMAL_DEGREES);
            using (Transaction tran = new Transaction(doc, "Rotate Text"))
            {
                tran.Start();
                if (Math.Round(Convertrado, 0) < 90)
                {
                    double gocquay = 180 - Convertrado;
                    var gh = UnitUtils.Convert(gocquay, DisplayUnitType.DUT_DECIMAL_DEGREES, DisplayUnitType.DUT_RADIANS);
                    BoundingBoxXYZ boxXYZ = textElement.get_BoundingBox(doc.ActiveView);
                    XYZ p1 = (boxXYZ.Max + boxXYZ.Min) / 2;
                    XYZ p2 = p1 + 2 * doc.ActiveView.ViewDirection;
                    Line line = Line.CreateBound(p1, p2);
                    ElementTransformUtils.RotateElement(doc, textElement.Id, line, gh);
                    var sau = Getupdirectionoftext(textElement);
                    if(!sau.IsParallel(directionofdim))
                    {
                        ElementTransformUtils.RotateElement(doc, textElement.Id, line, -2*gh);
                    }
                }
                else if (Math.Round(Convertrado, 0) > 90 || Math.Round(Convertrado, 0) == 90)
                {
                    double gocquay = 180 - Convertrado;
                    var gh = UnitUtils.Convert(gocquay, DisplayUnitType.DUT_DECIMAL_DEGREES, DisplayUnitType.DUT_RADIANS);
                    BoundingBoxXYZ boxXYZ = textElement.get_BoundingBox(doc.ActiveView);
                    XYZ p1 = (boxXYZ.Max + boxXYZ.Min) / 2;
                    XYZ p2 = p1 + 2 * doc.ActiveView.ViewDirection;
                    Line line = Line.CreateBound(p1, p2);
                    ElementTransformUtils.RotateElement(doc, textElement.Id, line, gh);
                    var sau = Getupdirectionoftext(textElement);
                    if (!sau.IsParallel(directionofdim))
                    {
                        ElementTransformUtils.RotateElement(doc, textElement.Id, line, -2 * gh);
                    }
                }
                tran.Commit();
            }
        }
    }
}
