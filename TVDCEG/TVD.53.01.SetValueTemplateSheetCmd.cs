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
using TVDCEG.Extension;
using TVDCEG.LBR;
using TVDCEG.Ultis;

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class SetValueTemplateSheetCmd : IExternalCommand
    {
        public SettingSetValueSheet Setting;
        public CegParameterSet dic = new CegParameterSet();
        [Obsolete]
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication application = commandData.Application;
            UIDocument activeUIDocument = application.ActiveUIDocument;
            Application application2 = application.Application;
            Document doc = activeUIDocument.Document;
            Selection sel = activeUIDocument.Selection;
            if(doc.ActiveView.ViewType==ViewType.DrawingSheet)
            {
                Setting = SettingSetValueSheet.Instance.GetSetting("");
                FamilyInstance Titleblockc = SetValueTemplateSheet.Instance.Gettileblock(doc);
                dic = SetValueTemplateSheet.Instance.GetParameter(doc, Titleblockc);
                var form = new FrmSetValueTemplateSheet(this, doc);
                form.ShowDialog();
                return Result.Succeeded;
            }
            else
            {
                TaskDialog.Show("Error", "Please go to Sheet");
                return Result.Cancelled;
            }
           
        }
    }
}
