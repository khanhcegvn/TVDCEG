#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class DuplicateViewSheet : IExternalCommand
    {
        public List<Element> listelement = new List<Element>();
        public Document doc;
        public List<ViewSheet> ListSheet = new List<ViewSheet>();
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
            ListSheet = GetViewSheets(doc);
            DuplicateSheet(doc, ListSheet);
            return Result.Succeeded;
        }
        public List<View> GetAllViewOfSheet(Document doc, List<ViewSheet> viewSheets)
        {
            List<View> AllView = new List<View>();
            return AllView;
        }
        public List<ViewSheet> GetViewSheets(Document doc)
        {
            List<ViewSheet> viewSheets = new List<ViewSheet>();
            FilteredElementCollector col = new FilteredElementCollector(doc).WhereElementIsNotElementType().OfClass(typeof(ViewSheet));
            var bn = col.ToElements();
            foreach (var i in bn)
            {
                ViewSheet sheet = i as ViewSheet;
                if (sheet.SheetNumber.Contains("PC4"))
                {
                    viewSheets.Add(sheet);
                }
            }
            return viewSheets;
        }
        public void DuplicateSheet(Document doc, List<ViewSheet> viewSheets)
        {
            ProgressBarform progressBarform = new ProgressBarform(viewSheets.Count, "Loading...");
            progressBarform.Show();
            foreach (ViewSheet sheet in viewSheets)
            {

                progressBarform.giatri();
                if (progressBarform.iscontinue == false)
                {
                    break;
                }
                FamilyInstance Tileblock = GetTileBlock(doc, sheet);
                using (Transaction tran = new Transaction(doc, "Dup"))
                {
                    tran.Start();
                    ViewSheet viewSheet = ViewSheet.Create(doc, Tileblock.GetTypeId());
                    viewSheet.Name = sheet.Name;
                    CopyViews(sheet, viewSheet);
                    CopyParameterValue(sheet, viewSheet);
                    tran.Commit();
                }
            }
            progressBarform.Close();
        }
        public FamilyInstance GetTileBlock(Document doc, ViewSheet sheet)
        {
            FamilyInstance col = new FilteredElementCollector(doc, sheet.Id).WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_TitleBlocks)
                .OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().First();
            return col;
        }
        public static void SetTitleBlockParameters(Element existingTitleBlock, Element newTitleBlock)
        {
            try
            {
                Autodesk.Revit.DB.FamilyInstance familyInstance1 = existingTitleBlock as Autodesk.Revit.DB.FamilyInstance;
                Autodesk.Revit.DB.FamilyInstance familyInstance2 = newTitleBlock as Autodesk.Revit.DB.FamilyInstance;
                IList<Parameter> orderedParameters = familyInstance1.GetOrderedParameters();
                familyInstance2.GetOrderedParameters();
                foreach (Parameter parameter1 in (IEnumerable<Parameter>)orderedParameters)
                {
                    string name = parameter1.Definition.Name;
                    Parameter parameter2 = familyInstance1.LookupParameter(name);
                    Parameter parameter3 = familyInstance2.LookupParameter(name);
                    if (parameter3 != null || parameter2 != null)
                    {
                        BuiltInParameter parameterId1 = BuiltInParameter.SHEET_NUMBER;
                        Parameter parameter4 = familyInstance2.get_Parameter(parameterId1);
                        BuiltInParameter parameterId2 = BuiltInParameter.SHEET_NAME;
                        Parameter parameter5 = familyInstance2.get_Parameter(parameterId2);
                        if (!(parameter3.Definition.Name == parameter4.Definition.Name) && !(parameter3.Definition.Name == parameter5.Definition.Name))
                        {
                            try
                            {
                                if (parameter3.StorageType == StorageType.Double)
                                    parameter3.Set(parameter2.AsDouble());
                                else if (parameter3.StorageType == StorageType.ElementId)
                                    parameter3.Set(parameter2.AsElementId());
                                else if (parameter3.StorageType == StorageType.Integer)
                                    parameter3.Set(parameter2.AsInteger());
                                else if (parameter3.StorageType == StorageType.String)
                                    parameter3.Set(parameter2.AsString());
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }
        public static void SetSheetParameters(Element existingSheet, Element newSheet)
        {
            try
            {
                IList<Parameter> orderedParameters = existingSheet.GetOrderedParameters();
                newSheet.GetOrderedParameters();
                foreach (Parameter parameter1 in orderedParameters)
                {
                    string name = parameter1.Definition.Name;
                    Parameter parameter2 = existingSheet.LookupParameter(name);
                    Parameter parameter3 = newSheet.LookupParameter(name);
                    if (parameter3 != null || parameter2 != null)
                    {
                        BuiltInParameter parameterId1 = BuiltInParameter.SHEET_NUMBER;
                        Parameter parameter4 = newSheet.get_Parameter(parameterId1);
                        BuiltInParameter parameterId2 = BuiltInParameter.SHEET_NAME;
                        Parameter parameter5 = newSheet.get_Parameter(parameterId2);
                        if (!(parameter3.Definition.Name == parameter4.Definition.Name) && !(parameter3.Definition.Name == parameter5.Definition.Name))
                        {
                            try
                            {
                                if (parameter3.StorageType == StorageType.Double)
                                    parameter3.Set(parameter2.AsDouble());
                                else if (parameter3.StorageType == StorageType.ElementId)
                                    parameter3.Set(parameter2.AsElementId());
                                else if (parameter3.StorageType == StorageType.Integer)
                                    parameter3.Set(parameter2.AsInteger());
                                else if (parameter3.StorageType == StorageType.String)
                                    parameter3.Set(parameter2.AsString());
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }
        void CopyViews(ViewSheet source, ViewSheet newSheet)
        {
            var viewportIds = source.GetAllViewports();
            var viewports = viewportIds.Select(x => doc.GetElement(x) as Viewport);
            foreach (var vp in viewports)
            {
                CopyView(vp, newSheet);
            }
        }
        void CopyView(Viewport viewport, ViewSheet newSheet)
        {
            var viewId = viewport.ViewId;
            var view = doc.GetElement(viewport.ViewId) as View;
            var point = viewport.GetBoxCenter();
            if (view.ViewType == ViewType.Legend)
            {
                Viewport.Create(doc, newSheet.Id, viewId, point);
                return;
            }
            else
            {
                var duplicatedViewId = view.Duplicate(ViewDuplicateOption.WithDetailing);
                Viewport.Create(doc, newSheet.Id, duplicatedViewId, point);
            }
        }
        void CopyParameterValue(ViewSheet source, ViewSheet newSheet)
        {
            var sourceParamters = source.GetOrderedParameters();
            SetSheetParameters(source, newSheet);
            var tb1 = GetTileBlock(doc, source);
            var tb2 = GetTileBlock(doc, newSheet);
            var translation = (tb1.Location as LocationPoint).Point;
            tb2.Location.Move(translation);
            if (tb1 != null)
            {
                SetTitleBlockParameters(tb1, tb2);
            }
        }
    }
}
