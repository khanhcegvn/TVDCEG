#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Linq;
using System.IO;
#endregion

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class AddViewsToSheetCmd : IExternalCommand
    {
        public Document Doc { get; set; }
        public Selection Sel { get; set; }
        private ExternalEvent _exEvent;
        public AddViewToSheetData Data { get; set; }
        public View ActiveView => Doc.ActiveView;
        public List<ViewObject> viewObjects = new List<ViewObject>();
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Sel = uidoc.Selection;
            Doc = uidoc.Document;
            Data = new AddViewToSheetData();
            GetAllViews();
            viewObjects = GetData(Doc);
            var ac = ActiveView as ViewSheet;
            if (ac == null)
            {
                TaskDialog.Show("Add View", "Please go to the ViewSheet to Add the Views");
                return Result.Cancelled;
            }
            var form = new AddViewsToSheetForm(this, Doc);
            this._exEvent = ExternalEvent.Create((IExternalEventHandler)new EventAddview(this, Doc));
            form.Show();
            form.ExEvent = this._exEvent;
            return Result.Succeeded;
        }

        private List<ViewObject> GetData(Document doc)
        {
            List<ViewObject> list = new List<ViewObject>();
            FilteredElementCollector filteredElementCollector = new FilteredElementCollector(doc).WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_Sheets).OfClass(typeof(ViewSheet));
            foreach (Element element in filteredElementCollector)
            {
                bool flag = element is ViewSheet;
                if (flag)
                {
                    ViewSheet viewSheet = element as ViewSheet;
                    ICollection<ElementId> allPlacedViews = viewSheet.GetAllPlacedViews();
                    foreach (ElementId elementId in allPlacedViews)
                    {
                        View view = doc.GetElement(elementId) as View;
                        ViewObject item = new ViewObject(view.Name, viewSheet.Name, viewSheet.SheetNumber, view.Id.IntegerValue, viewSheet.Id.IntegerValue);
                        list.Add(item);
                    }
                }
            }
            return list;
        }
        private void GetAllViews()
        {
            var views = new FilteredElementCollector(Doc).WhereElementIsNotElementType().OfClass(typeof(View)).ToElements().Cast<View>().ToList();
            foreach (var view in views)
            {
                if (view.ViewType == ViewType.FloorPlan ||
                    view.ViewType == ViewType.CeilingPlan ||
                    view.ViewType == ViewType.Elevation ||
                    view.ViewType == ViewType.ThreeD ||
                    view.ViewType == ViewType.Schedule ||
                    view.ViewType == ViewType.DraftingView ||
                    view.ViewType == ViewType.Legend ||
                    view.ViewType == ViewType.Section ||
                    view.ViewType == ViewType.EngineeringPlan ||
                    view.ViewType == ViewType.Detail)
                {
                    if (view.IsTemplate) continue;
                    if (view.ViewType == ViewType.Schedule && view.Name.Contains("Revision Schedule"))
                    {
                        continue;
                    }
                    Data.AllViews.Add(view);
                }
            }
            Data.AllViews = Data.AllViews.OrderBy(x => x.Name).ToList();
        }
        public class AddViewToSheetData
        {
            public List<View> AllViews { get; set; } = new List<View>();
            public List<View> Views { get; set; } = new List<View>();
            public ElementId ViewPortTypeId { get; set; } = ElementId.InvalidElementId;
            public bool IsDuplicate { get; set; } = false;
            public ViewDuplicateOption Option { get; set; }
        }
        public class EventAddview : EventRegisterHandler
        {
            private AddViewsToSheetCmd _data;
            private Document Doc;
            public EventAddview(AddViewsToSheetCmd data, Document doc)
            {
                _data = data;
                Doc = doc;
            }
            public override void DoingSomething()
            {
                if (_data.Data.Views.Count != 0)
                {
                    using (Transaction tx = new Transaction(Doc))
                    {
                        tx.Start("Ivention tool: Add Views To Sheet");
                        if (_data.Data.Views.Count != 0)
                        {
                            AddViewsToSheet(_data.Data.Views, _data.Data.ViewPortTypeId, _data.Data.IsDuplicate, _data.Data.Option);
                        }
                        tx.Commit();
                    }
                }
                else
                {
                    TaskDialog.Show("Error", "Select a view");
                }
            }
            public void AddViewsToSheet(List<View> views, ElementId viewPortTypeId, bool isDuplicate, ViewDuplicateOption viewDuplicateOption)
            {
                foreach (var v in views)
                {
                    var view = v;
                    if (isDuplicate)
                    {
                        try
                        {
                            if (view.ViewType == ViewType.Schedule)
                            {
                                var viewid = v.Duplicate(ViewDuplicateOption.Duplicate);
                                view = Doc.GetElement(viewid) as View;
                            }
                            else
                            {
                                var viewid = v.Duplicate(viewDuplicateOption);
                                view = Doc.GetElement(viewid) as View;
                            }
                        }
                        catch
                        {
                            var viewid = v.Duplicate(ViewDuplicateOption.Duplicate);
                            view = Doc.GetElement(viewid) as View;
                        }
                    }
                    try
                    {
                        var viewname = view.Name;
                        if (view.ViewType != ViewType.Schedule)
                        {
                            var point = _data.Sel.PickPoint("Pick a Point to Place View:" + viewname);
                            var vp = Viewport.Create(Doc, Doc.ActiveView.Id, view.Id, point);
                            vp.ChangeTypeId(viewPortTypeId);
                        }
                        else
                        {
                            var point = _data.Sel.PickPoint("Pick a Point to Place View:" + viewname);
                            ScheduleSheetInstance.Create(Doc, Doc.ActiveView.Id, view.Id, point);
                        }
                    }
                    catch
                    {
                        TaskDialog.Show("Iven EXT:", v.Name + " only can added in one Sheet.");
                    }
                }
            }
        }
    }
}
