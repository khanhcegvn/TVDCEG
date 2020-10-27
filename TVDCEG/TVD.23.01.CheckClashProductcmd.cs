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
#endregion

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class CheckClashProductcmd : IExternalCommand
    {
        public Document doc;
        public ExternalEvent _event;
        public UIDocument uidoc;
        public Selection sel;
        public Dictionary<string, string> dic = new Dictionary<string, string>();
        public List<ElementId> listshow = new List<ElementId>();
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
            using (var form = new FrmCheckClashProduct_1(doc))
            {
                if (form.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    if (form.check == true)
                    {
                        var list = form.list;
                        List<ElementId> ids = new List<ElementId>();
                        list.ForEach(x => ids.Add(x.Id));
                        dic = CheckClashProduct.Instance.Excute(doc, ids);
                        Showform();
                    }
                }
                else
                {
                    return Result.Cancelled;
                }
            }
            return Result.Succeeded;
        }
        public void Showform()
        {
            var form2 = new FrmReport(this);
            this._event = ExternalEvent.Create((IExternalEventHandler)new CheckClashProductEvent(doc, this));
            form2.Show();
            form2.ExEvent = this._event;
        }
    }
}
