#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TVDCEG.Ultis;
#endregion

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class FreezeViewcmd : IExternalCommand
    {
        public Document doc;
        public View3D tmp3D = null;
        private List<View> m_ViewList = new List<View>();
        private List<string> m_ViewListStr = new List<string>();
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
            View viewexpr = null;
            string file;
            using (Transaction T = new Transaction(doc, "Create View"))
            {
                T.Start();
                viewexpr = ExportDWG(doc, doc.ActiveView, out file);
                if (viewexpr != null)
                {
                    T.Commit();
                }
                else
                {
                    T.RollBack();
                }
            }
            if (viewexpr != null)
            {
                uidoc.ActiveView = viewexpr;
            }
            return Result.Succeeded;
        }
        public View ExportDWG(Document document, View view, out string file)
        {
            bool exported = false;
            ElementId outid = ElementId.InvalidElementId;
            DWGExportOptions dwgOptions = new DWGExportOptions();
            dwgOptions.FileVersion = ACADVersion.R2007;
            dwgOptions.ExportOfSolids = SolidGeometry.Polymesh;
            dwgOptions.ACAPreference = ACAObjectPreference.Geometry;
            dwgOptions.MergedViews = true;
            dwgOptions.PropOverrides = PropOverrideMode.ByEntity;
            View v = null;
            ICollection<ElementId> views = new List<ElementId>();
            views.Add(view.Id);
            var pathfile = SettingFreeze.Instance.GetFolderPath();
            if (view is View3D)
            {
                List<ElementId> vSet = new List<ElementId>();
                bool flag = view != null;
                if (flag)
                {
                    View3D tmp3D = null;
                    ViewSheet viewSheet = null;
                    bool proceed = false;
                    try
                    {
                        FilteredElementCollector filteredElementCollector = new FilteredElementCollector(doc);
                        filteredElementCollector.OfClass(typeof(FamilySymbol));
                        filteredElementCollector.OfCategory(BuiltInCategory.OST_TitleBlocks);
                        IList<Element> titleBlocks = filteredElementCollector.ToElements();
                        List<FamilySymbol> familySymbols = new List<FamilySymbol>();
                        foreach (Element element in titleBlocks)
                        {
                            FamilySymbol f = element as FamilySymbol;
                            bool flag3 = f != null;
                            if (flag3)
                            {
                                familySymbols.Add(f);
                            }
                        }
                        bool flag4 = titleBlocks.Count != 0;
                        if (flag4)
                        {
                            FamilySymbol fs = null;
                            foreach (FamilySymbol f2 in familySymbols)
                            {
                                bool flag5 = f2 != null;
                                if (flag5)
                                {
                                    fs = f2;
                                    break;
                                }
                            }
                            viewSheet = ViewSheet.Create(doc, fs.Id);
                            bool flag6 = viewSheet != null;
                            if (flag6)
                            {
                                UV location = new UV((viewSheet.Outline.Max.U - viewSheet.Outline.Min.U) / 2.0, (viewSheet.Outline.Max.V - viewSheet.Outline.Min.V) / 2.0);
                                try
                                {
                                    Viewport.Create(doc, viewSheet.Id, view.Id, new XYZ(location.U, location.V, 0.0));
                                }
                                catch
                                {
                                    try
                                    {
                                        XYZ tmpXYZ = new XYZ(-view.ViewDirection.X, -view.ViewDirection.Y, -view.ViewDirection.Z);
                                        BoundingBoxXYZ tmpBoundingBox = view.CropBox;
                                        bool tmpCropBoxActive = view.CropBoxActive;
                                        IList<Element> viewTypes = new FilteredElementCollector(doc).OfClass(typeof(ViewFamilyType)).ToElements();
                                        ElementId viewTypeid = null;
                                        foreach (Element viewType in viewTypes)
                                        {
                                            ViewFamilyType famType = viewType as ViewFamilyType;
                                            bool flag7 = famType != null && famType.ViewFamily == ViewFamily.ThreeDimensional;
                                            if (flag7)
                                            {
                                                viewTypeid = famType.Id;
                                                break;
                                            }
                                        }
                                        bool flag8 = viewTypeid != null;
                                        if (flag8)
                                        {
                                            tmp3D = View3D.CreateIsometric(doc, viewTypeid);
                                            tmp3D.ApplyViewTemplateParameters(view);
                                            tmp3D.CropBox = tmpBoundingBox;
                                            Viewport.Create(doc, viewSheet.Id, tmp3D.Id, new XYZ(location.U, location.V, 0.0));
                                        }
                                    }
                                    catch
                                    {
                                    }
                                }
                                vSet.Add(viewSheet.Id);
                                proceed = true;
                            }
                        }
                    }
                    catch
                    {
                    }
                    bool flag9 = proceed;
                    if (flag9)
                    {
                        exported = document.Export(pathfile, view.Name, vSet, dwgOptions);
                        bool flag10 = viewSheet != null;
                        if (flag10)
                        {
                            ElementId elementId = viewSheet.Id;
                            doc.Delete(elementId);
                        }
                        bool flag11 = tmp3D != null;
                        if (flag11)
                        {
                            ElementId elementId2 = tmp3D.Id;
                            doc.Delete(elementId2);
                        }
                    }
                }
            }
            else
            {
                exported = document.Export(pathfile, view.Name, views, dwgOptions);
            }
            if (tmp3D != null)
            {
                file = Path.Combine(pathfile, tmp3D.Name + ".dwg");
            }
            else
            {
                file = Path.Combine(pathfile, view.Name + ".dwg");
            }
            if (exported)
            {
                TaskDialog taskDialog = new TaskDialog("Freeze View");
                taskDialog.Id = "Freeze";
                taskDialog.Title = "Freeze Drawing";
                taskDialog.TitleAutoPrefix = true;
                taskDialog.MainIcon = TaskDialogIcon.TaskDialogIconInformation;
                taskDialog.AllowCancellation = true;
                taskDialog.MainInstruction = ("Select View Type :");
                taskDialog.AddCommandLink((TaskDialogCommandLinkId)1001, "Drafting View");
                taskDialog.AddCommandLink((TaskDialogCommandLinkId)1002, "Legend");
                taskDialog.CommonButtons = TaskDialogCommonButtons.Cancel;
                taskDialog.DefaultButton = ((TaskDialogResult)2);
                TaskDialogResult taskDialogResult = taskDialog.Show();
                var dwgimport = new DWGImportOptions();
                dwgimport.ColorMode = ImportColorMode.BlackAndWhite;
                dwgimport.ThisViewOnly = true;
                dwgimport.OrientToView = true;
                dwgimport.Placement = ImportPlacement.Origin;
                if (taskDialogResult == TaskDialogResult.CommandLink2)
                {
                    v = GetLegend(document);
                }
                else if (taskDialogResult == TaskDialogResult.CommandLink1)
                {
                    v = CreateDrafting(document);
                }
                if (v != null)
                {
                    document.Import(file, dwgimport, v, out outid);
                }
                string strPost = "(Forzen)";
                string newname = this.ReplaceForbiddenSigns(doc.ActiveView.Name);
                string tempName = newname;
                if (v != null)
                {
                    int j = 1;
                    for (; ; )
                    {
                        try
                        {
                            v.Name = newname + strPost;
                            break;
                        }
                        catch
                        {
                            bool flag2 = j > 10;
                            if (flag2)
                            {
                                try
                                {
                                    v.Name += strPost;
                                }
                                catch
                                {
                                }
                                break;
                            }
                            newname = tempName + "-" + j.ToString();
                            j++;
                        }
                    }
                }
            }
            return v;
        }
        public void CheckCompare(Document doc)
        {
            FilteredElementCollector val = new FilteredElementCollector(doc);
            IList<Element> list = val.OfClass(typeof(View)).ToElements();
            foreach (Element item in list)
            {
                View val2 = item as View;
                if (val2 != null && val2.CanBePrinted && (int)val2.ViewType != 6)
                {
                    m_ViewList.Add(val2);
                    m_ViewListStr.Add(val2.Name);
                }
            }
            m_ViewListStr.Sort();
            m_ViewList.Sort(CompareViewsByName);
        }
        public ViewDrafting FindDraftingview(Document doc)
        {
            var val = new FilteredElementCollector(doc).OfClass(typeof(View)).Cast<View>().FirstOrDefault(x => x.ViewType == ViewType.DraftingView);
            return val as ViewDrafting;
        }
        private static int CompareViewsByName(View arg1, View arg2)
        {
            return arg1.Name.CompareTo(arg2.Name);
        }

        View GetLegend(Document doc)
        {
            var legend = new FilteredElementCollector(doc).OfClass(typeof(View)).Cast<View>().FirstOrDefault(x => x.ViewType == ViewType.Legend);
            return doc.GetElement(legend.Duplicate(ViewDuplicateOption.Duplicate)) as View;
        }
        View CreateDrafting(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(ViewFamilyType));
            ViewFamilyType viewFamilyType = collector.Cast<ViewFamilyType>().First(vft => vft.ViewFamily == ViewFamily.Drafting);
            ViewDrafting drafting = ViewDrafting.Create(doc, viewFamilyType.Id);
            return (drafting as View);
        }
        List<View> GetAllView(Document doc)
        {
            var list1 = (from View x in new FilteredElementCollector(doc).OfClass(typeof(View)).Cast<View>() where x.ViewType == ViewType.Legend select x).ToList();
            var list2 = (from View x in new FilteredElementCollector(doc).OfClass(typeof(View)).Cast<View>() where x.ViewType == ViewType.DraftingView select x).ToList();
            list2.ForEach(x => list1.Add(x));
            return list2;
        }

        public View ReferenceViews()
        {
            View viewrf = null;
            View view = doc.ActiveView;
            FilteredElementCollector viewersCollector = new FilteredElementCollector(doc);
            viewersCollector.OfCategory(BuiltInCategory.OST_Viewers);
            Element elem = null;
            foreach (Element e in viewersCollector)
            {
                if (e.Name == view.Name)
                    elem = e;
            }

            if (elem != null)
            {
                DateTime start = DateTime.Now;
                FilterableValueProvider provider = new ParameterValueProvider(new ElementId(BuiltInParameter.VIEWPORT_SHEET_NUMBER));
                FilterRule rule = new FilterStringRule(provider, new FilterStringGreater(), string.Empty, false);
                ElementParameterFilter epf = new ElementParameterFilter(rule, false);
                FilteredElementCollector viewCol = new FilteredElementCollector(doc).WherePasses(epf);
                viewCol.OfClass(typeof(View));

                StringBuilder sb = new StringBuilder();
                foreach (View v in viewCol)
                {
                    if (v.Id.IntegerValue == view.Id.IntegerValue || v.IsTemplate || v.ViewType == ViewType.DrawingSheet || v.ViewType == ViewType.ColumnSchedule)
                        continue;
                    try
                    {
                        ICollection<ElementId> col = new FilteredElementCollector(doc, v.Id).ToElementIds();
                        if (col.Contains(elem.Id))
                            sb.AppendLine("View: " + v.Name);
                        viewrf = v;
                    }
                    catch { }
                }
            }
            return viewrf;
        }
        private string ReplaceForbiddenSigns(string name)
        {
            name = name.Replace("[", "");
            name = name.Replace("]", "");
            name = name.Replace("}", "");
            name = name.Replace("{", "");
            name = name.Replace("|", "");
            name = name.Replace("?", "");
            name = name.Replace("'", "");
            name = name.Replace(":", "");
            name = name.Replace("\\", "");
            name = name.Replace("~", "");
            name = name.Replace(">", "");
            name = name.Replace("<", "");
            name = name.Replace(";", "");
            return name;
        }
        public ViewSheet ReferenceSheet(Document doc, View view)
        {
            ViewSheet viewhseetrf = null;
            DateTime start = DateTime.Now;
            FilterableValueProvider provider = new ParameterValueProvider(new ElementId(BuiltInParameter.VIEW_SHEET_VIEWPORT_INFO));
            FilterRule rule = new FilterStringRule(provider, new FilterStringGreater(), string.Empty, false);
            ElementParameterFilter epf = new ElementParameterFilter(rule, false);
            FilteredElementCollector viewCol = new FilteredElementCollector(doc).WherePasses(epf);
            viewCol.OfClass(typeof(ViewSheet));
            StringBuilder sb = new StringBuilder();
            foreach (ViewSheet v in viewCol)
            {
                try
                {
                    ISet<ElementId> col = v.GetAllPlacedViews();
                    foreach (var fg in col)
                    {
                        if (fg == view.Id)
                        {
                            sb.AppendLine("View: " + v.Name);
                            viewhseetrf = v;
                        }
                    }

                }
                catch { }
            }
            return viewhseetrf;
        }
        public void PlaceViewToSheet(Document doc, View view, ViewSheet sheet)
        {
            if (view != null)
            {
                XYZ point = new XYZ(0, 0, 0);
                Viewport vp = Viewport.Create(doc, sheet.Id, view.Id, point);
                Outline outline1 = vp.GetBoxOutline();
                XYZ boxCenter = vp.GetBoxCenter();
                vp.SetBoxCenter(outline1.MaximumPoint);
                doc.Regenerate();
            }
        }
    }
    public class SettingFreeze
    {
        private static SettingFreeze _instance;
        private SettingFreeze()
        {

        }
        public static SettingFreeze Instance => _instance ?? (_instance = new SettingFreeze());
        public string TypeText { get; set; }
        public string GetFolderPath()
        {
            string folderPath = SettingExtension.GetSettingPath() + "\\TVD.Freeze";
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
            return folderPath;
        }
    }
}
