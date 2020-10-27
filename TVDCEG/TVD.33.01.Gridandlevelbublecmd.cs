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
    public class Gridandlevelbublecmd : IExternalCommand
    {
        public Document doc;
        public ExternalEvent _event;
        public UIDocument uidoc;
        public Selection sel;
        public BubbleEnds Databubble;
        public List<CEG_Grid> CEG_Grids = new List<CEG_Grid>();
        public List<CEG_level> CEG_Levels = new List<CEG_level>();
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
            Databubble = new BubbleEnds();
            CEG_Grids = (from x in new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategory(BuiltInCategory.OST_Grids).OfClass(typeof(Grid)).Cast<Grid>() select new CEG_Grid(doc.ActiveView, x)).ToList();
            CEG_Levels = (from x in new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategory(BuiltInCategory.OST_Levels).OfClass(typeof(Level)).Cast<Level>() select new CEG_level(doc.ActiveView, x)).ToList();
            using (var form = new FrmGridandlevelbuble(this, doc))
            {
                if (form.ShowDialog() != null)
                {
                    Gridandlevelbuble.Instance.Doing(doc, doc.ActiveView, Databubble, CEG_Grids);
                    if (CEG_Levels.Count != 0)
                    {
                        Gridandlevelbuble.Instance.Doinglevel(doc, doc.ActiveView, Databubble, CEG_Levels);
                    }
                }
            }
            return Result.Succeeded;
        }
    }
}
