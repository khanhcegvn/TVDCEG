using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using TVDCEG.CEG_INFOR;
using TVDCEG.Extension;
using TVDCEG.LBR;
using TVDCEG.Ultis;

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class CheckElementWorkshareCmd : IExternalCommand
    {
        public UIDocument uidoc;
        public List<CEG_Element> listceg = new List<CEG_Element>();
        [Obsolete]
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication application = commandData.Application;
            uidoc = application.ActiveUIDocument;
            Application application2 = application.Application;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            ICollection<ElementId> Ids = sel.GetElementIds();
            listceg = Getelement(doc, Ids);
            var form = new FrmCheckElementWorkshare(this);
            form.ShowDialog();
            return Result.Succeeded;
        }
        public List<CEG_Element> Getelement(Document doc, ICollection<ElementId> elementIds)
        {
            List<CEG_Element> list = new List<CEG_Element>();
            foreach (ElementId item in elementIds)
            {
                Element ele = doc.GetElement(item);
                if(ele.Category.ToBuiltinCategory()==BuiltInCategory.OST_Viewports)
                {
                    Viewport vp = ele as Viewport;
                    Element pele = doc.GetElement(vp.ViewId);
                    CEG_Element _Element = new CEG_Element(doc, pele);
                    list.Add(_Element);
                }
                else
                {
                    CEG_Element _Element = new CEG_Element(doc, ele);
                    list.Add(_Element);
                }
            }
            return list;
        }
    }
}
