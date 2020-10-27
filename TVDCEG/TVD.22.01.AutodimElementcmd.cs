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
    public class AutodimElementcmd : IExternalCommand
    {
        public Document doc;
        public ExternalEvent _event;
        public SettingAdutodimelement Setting;
        public List<DimensionType> dimensionTypes = new List<DimensionType>();
        public Selection sel;
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            doc = uidoc.Document;
            sel = uidoc.Selection;
            Setting = SettingAdutodimelement.Instance.GetSetting();
            dimensionTypes = AutodimElement.Instance.GetDimensions(doc);

            var form = new FrmAutodimelement(this);
            this._event = ExternalEvent.Create((IExternalEventHandler)new AutodimElementEvent(this, AutodimElement.Instance, form));
            form.Show();
            form.ExEvent = this._event;
            return Result.Succeeded;
        }
    }
}
