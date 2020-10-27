#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]

    public class Transfer_view : IExternalCommand
    {
        public List<View> listview = new List<View>();
        public List<Document> listdoc = new List<Document>();
        public Document doc;
        public UIDocument uidoc;
        public int versionnumber;
        [Obsolete]
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            versionnumber = int.Parse(app.VersionNumber);
            doc = uidoc.Document;
            listdoc = GetDocuments(app);
            TransferView form = new TransferView(this, doc);
            form.ShowDialog();
            return Result.Succeeded;
        }

        [Obsolete]
        public void TransferViewproject(Document source, Document targets, ICollection<ElementId> elementIds, string Title_name, int versionnumber)
        {
            Transaction newtran = new Transaction(targets, "Transfer");
            newtran.Start();
            CopyPasteOptions options = new CopyPasteOptions();
            options.SetDuplicateTypeNamesHandler(new CopyHandler());
            foreach (var i in elementIds)
            {
                View view = source.GetElement(i) as View;
                if (view.ViewType == ViewType.Legend)
                {
                    IList<ElementId> list1 = (from View i1 in new FilteredElementCollector(targets).OfClass(typeof(View)).WhereElementIsNotElementType()
                                              where i1.ViewType == ViewType.Legend
                                              select i1.Id).ToList();

                    ICollection<ElementId> collection2 = new List<ElementId>();
                    collection2.Add(i);
                    ICollection<ElementId> litstrans = ElementTransformUtils.CopyElements(source, collection2, targets, null, options);
                    foreach (var m in litstrans)
                    {
                        View o = targets.GetElement(m) as View;
                        o.Name = o.Name + "(" + Title_name + ")";
                    }
                    if (versionnumber <= 2019)
                    {
                        View val4 = (from View i1 in new FilteredElementCollector(source).OfClass(typeof(View)).WhereElementIsNotElementType()
                                     where i1.Id == i
                                     select i1).FirstOrDefault();
                        FilteredElementCollector val5 = new FilteredElementCollector(targets).OfClass(typeof(View));
                        if (list1.Count > 0)
                        {
                            var Y = val5.Excluding(list1);
                        }
                        View val6 = (from View i1 in val5.WhereElementIsNotElementType()
                                     where (int)i1.ViewType == 11
                                     select i1).FirstOrDefault();
                        ElementOwnerViewFilter val7 = new ElementOwnerViewFilter(val4.Id);
                        ElementCategoryFilter val8 = new ElementCategoryFilter(BuiltInCategory.OST_IOSSketchGrid, true);
                        ElementCategoryFilter val9 = new ElementCategoryFilter(BuiltInCategory.OST_CLines, true);
                        FilteredElementCollector val10 = new FilteredElementCollector(source);
                        val10.WherePasses(val7);
                        val10.WherePasses(val8);
                        val10.WherePasses(val9);
                        IList<ElementId> list2 = (from Element i1 in val10
                                                  where i1.Category != null
                                                  select i1.Id).ToList();
                        if (list2.Count > 0)
                        {
                            ElementTransformUtils.CopyElements(val4, list2, val6, null, options);
                        }
                    }
                }
                if (view.ViewType == ViewType.DraftingView)
                {
                    IList<ElementId> list1 = (from View i1 in new FilteredElementCollector(targets).OfClass(typeof(View)).WhereElementIsNotElementType()
                                              where i1.ViewType == ViewType.DraftingView
                                              select i1.Id).ToList();
                    ICollection<ElementId> collection2 = new List<ElementId>();
                    collection2.Add(i);
                    ICollection<ElementId> litstrans = ElementTransformUtils.CopyElements(source, collection2, targets, null, options);
                    foreach (var m in litstrans)
                    {
                        View o = targets.GetElement(m) as View;
                        o.Name = o.Name + "(" + Title_name + ")";
                    }
                    View val4 = (from View i1 in new FilteredElementCollector(source).OfClass(typeof(View)).WhereElementIsNotElementType()
                                 where i1.Id == i
                                 select i1).FirstOrDefault();
                    FilteredElementCollector val5 = new FilteredElementCollector(targets).OfClass(typeof(View));
                    if (list1.Count > 0)
                    {
                        var Y = val5.Excluding(list1);
                    }
                    View val6 = (from View i1 in val5.WhereElementIsNotElementType()
                                 where i1.ViewType == ViewType.DraftingView
                                 select i1).FirstOrDefault();
                    ElementOwnerViewFilter val7 = new ElementOwnerViewFilter(val4.Id);
                    ElementCategoryFilter val8 = new ElementCategoryFilter(BuiltInCategory.OST_IOSSketchGrid, true);
                    ElementCategoryFilter val9 = new ElementCategoryFilter(BuiltInCategory.OST_CLines, true);
                    FilteredElementCollector val10 = new FilteredElementCollector(source);
                    val10.WherePasses(val7);
                    val10.WherePasses(val8);
                    val10.WherePasses(val9);
                    IList<ElementId> list2 = (from Element i1 in val10
                                              where i1.Category != null
                                              select i1.Id).ToList();
                    IList<ElementId> list3 = new List<ElementId>();
                    foreach (var u in list2)
                    {
                        Element po = source.GetElement(u);
                        var ty = po.Category.Name.ToString();
                        if (ty != "Views")
                        {
                            list3.Add(u);
                        }
                    }
                    if (list3.Count > 0)
                    {
                        ElementTransformUtils.CopyElements(val4, list3, val6, null, options);
                    }
                }
                if (view.ViewType == ViewType.Schedule)
                {
                    IList<ElementId> list1 = (from View i1 in new FilteredElementCollector(targets).OfClass(typeof(View)).WhereElementIsNotElementType()
                                              where i1.ViewType == ViewType.Schedule
                                              select i1.Id).ToList();

                    ICollection<ElementId> collection2 = new List<ElementId>();
                    collection2.Add(i);
                    ICollection<ElementId> litstrans = ElementTransformUtils.CopyElements(source, collection2, targets, null, options);
                    foreach (var m in litstrans)
                    {
                        View o = targets.GetElement(m) as View;
                        o.Name = o.Name + "(" + Title_name + ")";
                    }
                }
            }
            newtran.Commit();
        }

        public List<Document> GetDocuments(Application app)
        {
            List<Document> list = new List<Document>();
            var Alldoc = app.Documents;
            foreach (Document i in Alldoc)
            {
                list.Add(i);
            }
            return list;
        }
    }
    public class CopyHandler : IDuplicateTypeNamesHandler
    {
        public DuplicateTypeAction OnDuplicateTypeNamesFound(DuplicateTypeNamesHandlerArgs args)
        {
            return DuplicateTypeAction.UseDestinationTypes;
        }
    }

}
