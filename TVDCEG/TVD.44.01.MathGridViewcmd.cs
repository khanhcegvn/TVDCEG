#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using TVDCEG.LBR;
using TVDCEG.Ultis;
#endregion

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class MathGridViewcmd : IExternalCommand
    {
        public Document doc;
        public ExternalEvent _event;
        public UIDocument uidoc;
        public Selection sel;
        public SettingTopDT Setting;
        public bool iscontinue = true;
        public Dictionary<string, ViewSheet> dic_sheet = new Dictionary<string, ViewSheet>();
        public List<TextNoteType> listTextnotes = new List<TextNoteType>();
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
            var listgrid = Getmodelelement.GetAllGridinView(doc.ActiveView, doc);
            using (var form = new FrmMathGridView(doc))
            {
                if (form.ShowDialog() != true)
                {
                    using (Transaction tran = new Transaction(doc, "Math Grid"))
                    {
                        tran.Start();
                        foreach (var item in form.listview)
                        {
                            Doing(listgrid, doc.ActiveView, item);
                        }
                        tran.Commit();
                    }
                }
            }
            return Result.Succeeded;
        }
        public void Doing(List<Grid> grids, View source, View view)
        {
            ISet<ElementId> ids = new HashSet<ElementId>();
            ids.Add(view.Id);
            foreach (var grid in grids)
            {
                if (!grid.IsHidden(view))
                {
                    grid.PropagateToViews(source, ids);
                }
            }
        }
    }
}
