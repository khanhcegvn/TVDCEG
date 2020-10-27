using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace TVDCEG.LBR
{
    public static class Getmodelelement
    {
        public static List<TextNoteType> GetTextNoteTypes(Document doc)
        {
            return new FilteredElementCollector(doc).OfClass(typeof(TextNoteType)).Cast<TextNoteType>().ToList();
        }
        public static List<Grid> GetAllGridinView(View view, Document doc)
        {
            return new FilteredElementCollector(doc, view.Id).OfCategory(BuiltInCategory.OST_Grids).OfClass(typeof(Grid)).Cast<Grid>().ToList();
        }
        public static List<Grid> GetAllGrid(Document doc)
        {
            return new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Grids).OfClass(typeof(Grid)).Cast<Grid>().ToList();
        }
        public static List<ViewSheet> GetallViewsheet(Document doc)
        {
            var col = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Sheets).OfClass(typeof(ViewSheet)).Cast<ViewSheet>().ToList();
            return col;
        }
        public static List<View> GetallViewTeamplate(Document doc)
        {
            var col = (from x in new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(View)).Cast<View>() where x.IsTemplate select x).ToList();
            return col;
        }
        public static List<View> GetAllViews(Document doc)
        {
            List<View> list = new List<View>();
            var views = new FilteredElementCollector(doc).WhereElementIsNotElementType().OfClass(typeof(View)).ToElements().Cast<View>().ToList();
            foreach (var view in views)
            {
                if (view.ViewType == ViewType.FloorPlan ||
                    //view.ViewType == ViewType.CeilingPlan ||
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
                    list.Add(view);
                }
            }
            return list;
        }
        public static List<View> Getlegends(Document doc)
        {
            var col = (from x in new FilteredElementCollector(doc).WhereElementIsNotElementType().OfClass(typeof(View)).ToElements().Cast<View>() where x.ViewType == ViewType.Legend select x).ToList();
            return (from x in col where !x.IsTemplate select x).ToList();
        }
        public static List<FamilySymbol> GetallfamilySymbol(Document doc)
        {
            var col = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Cast<FamilySymbol>().ToList();
            return col;
        }
        public static List<View> GetallFloorplan(Document doc)
        {
            var col = (from x in new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(View)).Cast<View>() where x.ViewType == ViewType.FloorPlan select x).ToList();
            return col;
        }
        public static Dictionary<string, List<FamilyInstance>> Findsymboltag(Document doc)
        {
            Dictionary<string, List<FamilyInstance>> dic = new Dictionary<string, List<FamilyInstance>>();
            var col = new FilteredElementCollector(doc).WhereElementIsNotElementType().OfClass(typeof(IndependentTag)).Where(x => x.Category.Name == "Multi-Category Tags" || x.Category.Name == "Structural Framing Tags").Cast<IndependentTag>().ToList();
            foreach (var tag in col)
            {
                FamilyInstance instancesource = doc.GetElement(tag.TaggedLocalElementId) as FamilyInstance;
                if (instancesource.Category.Name != null)
                {
                    if (instancesource.Category.Name.Contains("Structural Framing"))
                    {
                        if (dic.ContainsKey(tag.Name))
                        {
                            dic[tag.Name].Add(instancesource);
                        }
                        else
                        {
                            dic.Add(tag.Name, new List<FamilyInstance> { instancesource });
                        }
                    }
                }
            }
            return dic;
        }
    }
}
