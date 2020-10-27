#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using TVDCEG.WPF;
#endregion

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class Respacedim : IExternalCommand
    {
        public Document doc;
        public List<Grid> listgrid = new List<Grid>();
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            var form = new FrmRespacedim();
            if (form.ShowDialog() == true)
            {
                IList<Reference> listrf = sel.PickObjects(ObjectType.Element, new Filterdimention());
                //Getlinedimention(listrf, form.space);
                //Getlinedimention2(listrf, form.space);
                if (form.radiobutoncheck == true)
                {
                    Getlinedimention(listrf, form.space);
                }
                else
                {
                    Getlinedimention2(listrf, form.space);
                }
            }
            return Result.Succeeded;
        }
        public void Getlinedimention(IList<Reference> references, double x)
        {
            View view = doc.ActiveView;
            XYZ directview = view.ViewDirection;
            XYZ up = view.UpDirection;
            XYZ right = view.RightDirection;
            List<Dimension> listdim = new List<Dimension>();
            foreach (var i in references)
            {
                Dimension dim = doc.GetElement(i) as Dimension;
                listdim.Add(dim);
            }
            var line = listdim.First().Curve as Line;
            XYZ directline = line.Direction;
            Sortdimensionlistrightdirect(listdim);
            for (int i = 1; i < listdim.Count; i++)
            {
                var dim1 = listdim[i];
                var dim2 = listdim[i - 1];
                var line1 = dim1.Curve as Line;
                var line2 = dim2.Curve as Line;
                var org1 = line1.Origin;
                var org2 = line2.Origin;
                double kc = line1.Distance(org2);
                var bm = Math.Round(right.DotProduct(directline), 3);
                using (Transaction t = new Transaction(doc, "move"))
                {
                    t.Start();
                    if (bm != 0)
                    {
                        if (up.X > 0 || up.Y > 0 || up.Z > 0)
                        {
                            ICollection<ElementId> twodim = new List<ElementId>();
                            for (int j = i + 1; j < listdim.Count; j++)
                            {
                                var dim3 = listdim[j];
                                twodim.Add(dim3.Id);
                            }
                            ElementTransformUtils.MoveElement(doc, dim1.Id, -up * kc);
                            ElementTransformUtils.MoveElement(doc, dim1.Id, up * x);
                            if (twodim.Count != 0)
                            {
                                ElementTransformUtils.MoveElements(doc, twodim, up * x);
                            }
                        }
                        if (up.X < 0 || up.Y < 0 || up.Z < 0)
                        {
                            ICollection<ElementId> twodim = new List<ElementId>();
                            for (int j = i + 1; j < listdim.Count; j++)
                            {
                                var dim3 = listdim[j];
                                twodim.Add(dim3.Id);
                            }
                            ElementTransformUtils.MoveElement(doc, dim1.Id, up * kc);
                            ElementTransformUtils.MoveElement(doc, dim1.Id, -up * x);
                            if (twodim.Count != 0)
                            {
                                ElementTransformUtils.MoveElements(doc, twodim, -up * x);
                            }
                        }
                    }
                    else
                    {
                        if (right.X > 0 || right.Y > 0 || right.Z > 0)
                        {
                            ICollection<ElementId> twodim = new List<ElementId>();
                            for (int j = i + 1; j < listdim.Count; j++)
                            {
                                var dim3 = listdim[j];
                                twodim.Add(dim3.Id);
                            }
                            ElementTransformUtils.MoveElement(doc, dim1.Id, -right * kc);
                            ElementTransformUtils.MoveElement(doc, dim1.Id, right * x);
                            if (twodim.Count != 0)
                            {
                                ElementTransformUtils.MoveElements(doc, twodim, right * x);
                            }
                        }
                        if (right.X < 0 || right.Y < 0 || right.Z < 0)
                        {
                            ICollection<ElementId> twodim = new List<ElementId>();
                            for (int j = i + 1; j < listdim.Count; j++)
                            {
                                var dim3 = listdim[j];
                                twodim.Add(dim3.Id);
                            }
                            ElementTransformUtils.MoveElement(doc, dim1.Id, right * kc);
                            ElementTransformUtils.MoveElement(doc, dim1.Id, -right * x);
                            if (twodim.Count != 0)
                            {
                                ElementTransformUtils.MoveElements(doc, twodim, -right * x);
                            }
                        }
                    }
                    t.Commit();
                }
            }
        }
        public void Getlinedimention2(IList<Reference> references, double x)
        {
            View view = doc.ActiveView;
            XYZ directview = view.ViewDirection;
            XYZ up = view.UpDirection;
            XYZ right = view.RightDirection;
            List<Dimension> listdim = new List<Dimension>();
            foreach (var i in references)
            {
                Dimension dim = doc.GetElement(i) as Dimension;
                listdim.Add(dim);
            }
            var line = listdim.First().Curve as Line;
            XYZ directline = line.Direction;
            double gv = up.DotProduct(directline);
            double gg = right.DotProduct(directline);
            Sortdimensionlistrightdirect2(listdim);
            for (int i = 1; i < listdim.Count; i++)
            {
                var dim1 = listdim[i];
                var dim2 = listdim[i - 1];
                var line1 = dim1.Curve as Line;
                var line2 = dim2.Curve as Line;
                var org1 = line1.Origin;
                var org2 = line2.Origin;
                double kc = line1.Distance(org2);
                using (Transaction t = new Transaction(doc, "move 2"))
                {
                    t.Start();
                    if (right.DotProduct(directline) != 0)
                    {
                        if (up.X > 0 || up.Y > 0 || up.Z > 0)
                        {
                            ICollection<ElementId> twodim = new List<ElementId>();
                            for (int j = i + 1; j < listdim.Count; j++)
                            {
                                var dim3 = listdim[j];
                                twodim.Add(dim3.Id);
                            }
                            ElementTransformUtils.MoveElement(doc, dim1.Id, up * kc);
                            ElementTransformUtils.MoveElement(doc, dim1.Id, -up * x);
                            if (twodim.Count != 0)
                            {
                                ElementTransformUtils.MoveElements(doc, twodim, -up * x);
                            }
                        }
                        if (up.X < 0 || up.Y < 0 || up.Z < 0)
                        {
                            ICollection<ElementId> twodim = new List<ElementId>();
                            for (int j = i + 1; j < listdim.Count; j++)
                            {
                                var dim3 = listdim[j];
                                twodim.Add(dim3.Id);
                            }
                            ElementTransformUtils.MoveElement(doc, dim1.Id, -up * kc);
                            ElementTransformUtils.MoveElement(doc, dim1.Id, up * x);
                            if (twodim.Count != 0)
                            {
                                ElementTransformUtils.MoveElements(doc, twodim, up * x);
                            }
                        }
                    }
                    else
                    {
                        if (right.X > 0 || right.Y > 0 || right.Z > 0)
                        {
                            ICollection<ElementId> twodim = new List<ElementId>();
                            for (int j = i + 1; j < listdim.Count; j++)
                            {
                                var dim3 = listdim[j];
                                twodim.Add(dim3.Id);
                            }
                            ElementTransformUtils.MoveElement(doc, dim1.Id, right * kc);
                            ElementTransformUtils.MoveElement(doc, dim1.Id, -right * x);
                            if (twodim.Count != 0)
                            {
                                ElementTransformUtils.MoveElements(doc, twodim, -right * x);
                            }
                        }
                        if (right.X < 0 || right.Y < 0 || right.Z < 0)
                        {
                            ICollection<ElementId> twodim = new List<ElementId>();
                            for (int j = i + 1; j < listdim.Count; j++)
                            {
                                var dim3 = listdim[j];
                                twodim.Add(dim3.Id);
                            }
                            ElementTransformUtils.MoveElement(doc, dim1.Id, -right * kc);
                            ElementTransformUtils.MoveElement(doc, dim1.Id, right * x);
                            if (twodim.Count != 0)
                            {
                                ElementTransformUtils.MoveElements(doc, twodim, right * x);
                            }
                        }
                    }
                    t.Commit();
                }
            }
        }
        public void Sortdimensionlistz(List<Dimension> dimensions)
        {
            for (int i = 0; i < dimensions.Count; i++)
            {
                for (int j = 0; j < dimensions.Count; j++)
                {
                    if ((dimensions[i].Curve as Line).Origin.Z < (dimensions[j].Curve as Line).Origin.Z)
                    {
                        var temp = dimensions[i];
                        dimensions[i] = dimensions[j];
                        dimensions[j] = temp;
                    }
                }
            }
        }
        public void Sortdimensionlistrightdirect(List<Dimension> dimensions)
        {
            var dim1 = dimensions[0];
            var dim2 = dimensions[1];
            var line1 = dim1.Curve as Line;
            var line2 = dim2.Curve as Line;
            var org1 = line1.Origin;
            var org2 = line2.Origin;
            var kc = Math.Round(line1.Distance(line2.Origin),2);
            if (Math.Round(Math.Abs(org1.Z - org2.Z),2) == kc)
            {
                for (int i = 0; i < dimensions.Count; i++)
                {
                    for (int j = 0; j < dimensions.Count; j++)
                    {
                        if ((dimensions[i].Curve as Line).Origin.Z < (dimensions[j].Curve as Line).Origin.Z)
                        {
                            var temp = dimensions[i];
                            dimensions[i] = dimensions[j];
                            dimensions[j] = temp;
                        }
                    }
                }
            }
            if (Math.Round(Math.Abs(org1.X - org2.X),2) == kc)
            {
                for (int i = 0; i < dimensions.Count; i++)
                {
                    for (int j = 0; j < dimensions.Count; j++)
                    {
                        if ((dimensions[i].Curve as Line).Origin.X < (dimensions[j].Curve as Line).Origin.X)
                        {
                            var temp = dimensions[i];
                            dimensions[i] = dimensions[j];
                            dimensions[j] = temp;
                        }
                    }
                }
            }
            if (Math.Round(Math.Abs(org1.Y - org2.Y),2) == kc)
            {
                for (int i = 0; i < dimensions.Count; i++)
                {
                    for (int j = 0; j < dimensions.Count; j++)
                    {
                        if ((dimensions[i].Curve as Line).Origin.Y < (dimensions[j].Curve as Line).Origin.Y)
                        {
                            var temp = dimensions[i];
                            dimensions[i] = dimensions[j];
                            dimensions[j] = temp;
                        }
                    }
                }
            }
        }
        public void Sortdimensionlistrightdirect2(List<Dimension> dimensions)
        {
            var dim1 = dimensions[0];
            var dim2 = dimensions[1];
            var line1 = dim1.Curve as Line;
            var line2 = dim2.Curve as Line;
            var org1 = line1.Origin;
            var org2 = line2.Origin;
            var kc = line1.Distance(line2.Origin);
            if (Math.Abs(org1.Z - org2.Z) == kc)
            {
                for (int i = 0; i < dimensions.Count; i++)
                {
                    for (int j = 0; j < dimensions.Count; j++)
                    {
                        if ((dimensions[i].Curve as Line).Origin.Z > (dimensions[j].Curve as Line).Origin.Z)
                        {
                            var temp = dimensions[i];
                            dimensions[i] = dimensions[j];
                            dimensions[j] = temp;
                        }
                    }
                }
            }
            if (Math.Abs(org1.X - org2.X) == kc)
            {
                for (int i = 0; i < dimensions.Count; i++)
                {
                    for (int j = 0; j < dimensions.Count; j++)
                    {
                        if ((dimensions[i].Curve as Line).Origin.X > (dimensions[j].Curve as Line).Origin.X)
                        {
                            var temp = dimensions[i];
                            dimensions[i] = dimensions[j];
                            dimensions[j] = temp;
                        }
                    }
                }
            }
            if (Math.Abs(org1.Y - org2.Y) == kc)
            {
                for (int i = 0; i < dimensions.Count; i++)
                {
                    for (int j = 0; j < dimensions.Count; j++)
                    {
                        if ((dimensions[i].Curve as Line).Origin.Y > (dimensions[j].Curve as Line).Origin.Y)
                        {
                            var temp = dimensions[i];
                            dimensions[i] = dimensions[j];
                            dimensions[j] = temp;
                        }
                    }
                }
            }
        }
    }
}
