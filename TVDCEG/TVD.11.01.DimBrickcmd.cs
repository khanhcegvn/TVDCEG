#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.IO;
using TVDCEG.Ultis;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
#endregion

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class DimBrickcmd : IExternalCommand
    {
        public Document doc;
        public List<Grid> listgrid = new List<Grid>();
        public ExternalEvent _event;
        public List<DimensionType> dimensionTypes = new List<DimensionType>();
        public SettingBrick Setting;
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
            DimBrick.Instance.CreateTypetextnote(doc);
            Setting = SettingBrick.Instance.GetSetting();
            dimensionTypes = DimBrick.Instance.GetDimensions(doc);
            var form = new FrmBrickDim(this,doc);
            this._event = ExternalEvent.Create((IExternalEventHandler)new DimBrickEvent(this, DimBrick.Instance, doc, form));
            form.Show();
            form.ExEvent = this._event;
            return Result.Succeeded;
        }
    }

}
