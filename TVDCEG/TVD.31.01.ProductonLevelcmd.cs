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
    public class ProductonLevelcmd : IExternalCommand
    {
        public Document doc;
        public ExternalEvent _event;
        public UIDocument uidoc;
        public Selection sel;
        public SettingProductonlevel Setting;
        public Dictionary<string, List<string>> dic2 = new Dictionary<string, List<string>>();
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
            Setting = SettingProductonlevel.Instance.GetSetting();
            var dic = ProductonLevel.Instance.Getallproductbycontrolmark(doc);
            dic2 = ProductonLevel.Instance.Caculator(dic);
            using (var form = new FrmProductonlevel(this, doc))
            {
                if (form.ShowDialog()==false)
                {
                    ProductonLevel.Instance.CreateTextNote(doc, dic2, form.textnotetype, sel);
                }
            }
            return Result.Succeeded;
        }
    }
}
