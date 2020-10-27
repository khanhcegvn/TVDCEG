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
    public class MergeTextcmd : IExternalCommand
    {
        public Document doc;
        public Application app;
        public UIDocument uidoc;
        public List<Reference> listrf = new List<Reference>();
        public bool btn_pick;
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            uidoc = uiapp.ActiveUIDocument;
            app = uiapp.Application;
            doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            bool flag = true;
            List<Reference> list = new List<Reference>();
            while (flag)
            {
                try
                {
                    Reference pick = sel.PickObject(ObjectType.Element, new CEGFilterTextnote(), "Select text note");
                    list.Add(pick);
                }
                catch
                {
                    flag = false;
                    MergeText.Instance.CreateTextNote(doc, list, sel);
                    //list.Clear();
                    //return list;
                }
            }
            //var from = new FrmMergeText(this);
            //from.Show();
            //this._exEvent = ExternalEvent.Create((IExternalEventHandler)new MergeTextEvent(doc, this, MergeText.Instance));
            //from.ExEvent = this._exEvent;
            return Result.Succeeded;
        }
    }
    public class CEGFilterTextnote : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem.Category.Name.Contains("Text Notes")) return true;
            else return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
