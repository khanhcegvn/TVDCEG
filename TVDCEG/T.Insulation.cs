#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
#endregion

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class Insulationcmd : IExternalCommand
    {
        public Document doc;
        List<XYZ> listpoint = new List<XYZ>();
        public List<Element> listelement = new List<Element>();
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
            listpoint.Clear();
            Insulationlbr insulationlbr = Insulationlbr.Instance;
            InsulationSupport insulationSupport = new InsulationSupport();
            FamilyInstance WALLINSTANCE = null;
            List<FamilySymbol> listsym = insulationlbr.GetFamilySymbols(doc);
            List<Rectangleslbr> soluong = new List<Rectangleslbr>();
            AssemblyInstance instance = null;
            FamilyInstance insu = null;
            Rectangleslbr rec = null;
            ICollection<ElementId> copyid = new List<ElementId>();
            //Reference rf = sel.PickObject(ObjectType.Element, new AssemblySelectionfilter(), "Assembly");
            //Element ele = doc.GetElement(rf);
            //AssemblyInstance instance = ele as Assembly+Instance;
            ProgressbarWPF progressbarWPF = new ProgressbarWPF(4, "Loading...");
            progressbarWPF.Show();
            for (int i = 1; i < 4; i++)
            {
                progressbarWPF.Giatri();
                if (progressbarWPF.iscontinue == false)
                {
                    break;
                }
                if (i == 1)
                {
                    if (doc.ActiveView.IsAssemblyView)
                    {
                        instance = doc.GetElement(doc.ActiveView.AssociatedAssemblyInstanceId) as AssemblyInstance;
                    }
                    insu = insulationlbr.Getinsulation(doc, instance);
                    WALLINSTANCE = insulationlbr.GetWall(doc, instance);
                    //var topface = insulationSupport.FindTopWallother(insu);
                    rec = insulationlbr.DrawingFaceTop(doc, instance);
                }
                if (i == 2)
                {
                    var right = doc.ActiveView.RightDirection;
                    PlanarFace topface = insulationSupport.FindTopWallother(insu);
                    IList<CurveLoop> loops = topface.GetEdgesAsCurveLoops();
                    if (loops.Count != 1)
                    {
                        soluong = insulationSupport.DrawBlockOut2(doc, instance, insu, WALLINSTANCE, rec, 0.125, 8);
                        List<List<Rectangleslbr>> listrec = new List<List<Rectangleslbr>>();
                        listrec = Rectangleslbr.ListRecVert(soluong);
                        insulationSupport.SortRectangleslist(listrec);
                        if (Math.Floor(right.X) != 0)
                        {
                            copyid = Rectangleslbr.LayoutInsulationvert(doc, listrec);
                        }
                        else
                        {
                            copyid = Rectangleslbr.LayoutInsulationvert2(doc, listrec);
                        }
                    }
                    else
                    {
                        soluong = insulationSupport.DrawBlockOut1(doc, instance, insu, WALLINSTANCE, rec, 0.125, 8);
                        List<List<Rectangleslbr>> listrec = new List<List<Rectangleslbr>>();
                        listrec = Rectangleslbr.ListRecVert(soluong);
                        insulationSupport.SortRectangleslist(listrec);
                        if (Math.Floor(right.X) != 0)
                        {
                            copyid = Rectangleslbr.LayoutInsulationvert(doc, listrec);
                        }
                        else
                        {
                            copyid = Rectangleslbr.LayoutInsulationvert2(doc, listrec);
                        }
                    }
                }
                if (i == 3)
                {
                    View viewcopy = insulationlbr.CreateDraftingView(doc);
                    using (Transaction tran = new Transaction(doc, "Copy"))
                    {
                        tran.Start();
                        ElementTransformUtils.CopyElements(doc.ActiveView, copyid, viewcopy, null, new CopyPasteOptions());
                        doc.Delete(copyid);
                        tran.Commit();
                    }
                    TaskDialog.Show("Insulation lay out", "Finish View" + " " + viewcopy.Name);
                }
            }
            progressbarWPF.Close();

            /////////
            //IList<Element> rf = sel.PickElementsByRectangle(new Filterdetailline(), "Detailline");
            //ICollection<ElementId> listd = new List<ElementId>();
            //foreach (Element e in rf)
            //{
            //    listd.Add(e.Id);
            //}
            //List<XYZ> list = insulationlbr.GetIntersectXYZoncurve(doc, listd);
            //List<FamilySymbol> listsym = insulationlbr.GetFamilySymbols(doc);
            //List<Rectangleslbr> soluong = Rectangleslbr.CreateRectangle(doc, list);
            ////insulationlbr.PlaceSymmbolonRectangles(doc, listsym, soluong);
            //List<List<Rectangleslbr>> listrec = new List<List<Rectangleslbr>>();
            //listrec = Rectangleslbr.ListRecVert(soluong);
            //TaskDialog.Show("SSS", soluong.Count.ToString());
            ////insulationlbr.PlaceSymmbolonPoint(doc, listsym, list);
            //insulationlbr.PlaceSymmbolonRectangles(doc, listsym, soluong);
            return Result.Succeeded;
        }
    }
}
