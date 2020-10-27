#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
#endregion

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class Switchgrid2d3dcmd : IExternalCommand
    {
        public Document doc;
        public List<Grid> listgrid = new List<Grid>();
        public List<Level> listlevels = new List<Level>();
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
            listgrid = GetGrids(doc);
            listlevels = GetLevels(doc);
            ChangeSwitch2d_3d(doc, listgrid, listlevels);
            return Result.Succeeded;
        }
        public List<Grid> GetGrids(Document doc)
        {
            List<Grid> list = new List<Grid>();
            FilteredElementCollector col = new FilteredElementCollector(doc, doc.ActiveView.Id).WhereElementIsNotElementType().OfClass(typeof(Grid));
            var tui = col.ToElements();
            foreach (var i in tui)
            {
                list.Add(i as Grid);
            }
            return list;
        }
        public List<Level> GetLevels(Document doc)
        {
            List<Level> list = new List<Level>();
            FilteredElementCollector col = new FilteredElementCollector(doc, doc.ActiveView.Id).WhereElementIsNotElementType().OfClass(typeof(Level));
            var tui = col.ToElements();
            foreach (var i in tui)
            {
                list.Add(i as Level);
            }
            return list;
        }
        public void ChangeSwitch2d_3d(Document doc, List<Grid> grids, List<Level> levels)
        {
            using (Transaction tran = new Transaction(doc, "Set"))
            {
                tran.Start();
                if (grids.Count != 0)
                {
                    foreach (Grid i in grids)
                    {
                        var start = i.GetDatumExtentTypeInView(DatumEnds.End0, doc.ActiveView);
                        var end = i.GetDatumExtentTypeInView(DatumEnds.End1, doc.ActiveView);
                        if (start == DatumExtentType.Model)
                        {
                            i.SetDatumExtentType(DatumEnds.End0, doc.ActiveView, DatumExtentType.ViewSpecific);
                            Switchlevel(DatumExtentType.ViewSpecific, levels);
                        }
                        else if (start == DatumExtentType.ViewSpecific)
                        {
                            i.SetDatumExtentType(DatumEnds.End0, doc.ActiveView, DatumExtentType.Model);
                            Switchlevel(DatumExtentType.Model, levels);
                        }
                        if (end == DatumExtentType.Model)
                        {
                            i.SetDatumExtentType(DatumEnds.End1, doc.ActiveView, DatumExtentType.ViewSpecific);
                        }
                        else if (end == DatumExtentType.ViewSpecific)
                        {
                            i.SetDatumExtentType(DatumEnds.End1, doc.ActiveView, DatumExtentType.Model);
                        }
                    }
                }
                else
                {
                    foreach (Level level in levels)
                    {
                        var start = level.GetDatumExtentTypeInView(DatumEnds.End0, doc.ActiveView);
                        var end = level.GetDatumExtentTypeInView(DatumEnds.End1, doc.ActiveView);
                        if (start == DatumExtentType.Model) level.SetDatumExtentType(DatumEnds.End0, doc.ActiveView, DatumExtentType.ViewSpecific);
                        else if(start == DatumExtentType.ViewSpecific) level.SetDatumExtentType(DatumEnds.End0, doc.ActiveView, DatumExtentType.ViewSpecific);
                        if(end == DatumExtentType.Model) level.SetDatumExtentType(DatumEnds.End1, doc.ActiveView, DatumExtentType.ViewSpecific);
                        else if(end==DatumExtentType.ViewSpecific) level.SetDatumExtentType(DatumEnds.End1, doc.ActiveView, DatumExtentType.Model);
                    }
                }
                tran.Commit();
            }
        }
        public void Switchlevel(DatumExtentType dantumtype, List<Level> levels)
        {
            if (levels.Count != 0)
            {
                foreach (Level i in levels)
                {
                    var start = i.GetDatumExtentTypeInView(DatumEnds.End0, doc.ActiveView);
                    var end = i.GetDatumExtentTypeInView(DatumEnds.End1, doc.ActiveView);
                    if (dantumtype == DatumExtentType.Model)
                    {
                        if (start == DatumExtentType.Model) continue;
                        else if (start == DatumExtentType.ViewSpecific) i.SetDatumExtentType(DatumEnds.End0, doc.ActiveView, DatumExtentType.Model);
                        if (end == DatumExtentType.Model) continue;
                        else if (end == DatumExtentType.ViewSpecific) i.SetDatumExtentType(DatumEnds.End1, doc.ActiveView, DatumExtentType.Model);
                    }
                    if (dantumtype == DatumExtentType.ViewSpecific)
                    {
                        if (start == DatumExtentType.Model) i.SetDatumExtentType(DatumEnds.End0, doc.ActiveView, DatumExtentType.ViewSpecific);
                        else if (start == DatumExtentType.ViewSpecific) continue;
                        if (end == DatumExtentType.Model) i.SetDatumExtentType(DatumEnds.End1, doc.ActiveView, DatumExtentType.ViewSpecific);
                        else if (end == DatumExtentType.ViewSpecific) continue;
                    }
                }
            }
        }
    }
}
