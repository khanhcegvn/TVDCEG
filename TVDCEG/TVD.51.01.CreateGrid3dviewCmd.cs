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
using TVDCEG.LBR;
using TVDCEG.Ultis;

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class CreateGrid3dviewCmd : IExternalCommand
    {
        SettingCreateGrid3d Setting;

        [Obsolete]
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication application = commandData.Application;
            UIDocument activeUIDocument = application.ActiveUIDocument;
            Application application2 = application.Application;
            Document doc = activeUIDocument.Document;
            Selection sel = activeUIDocument.Selection;
            Setting = SettingCreateGrid3d.Instance.GetSetting();
            FamilySymbol symbol = CreateGrid3dview.Instance.GetSymbol(doc, Setting);
            CreateGrid3dview.Instance.Deleteolddata(doc);
            using (Transaction tran = new Transaction(doc, "Create grid"))
            {
                tran.Start();
                CreateGrid3dview.Instance.Create3dGrid(doc, symbol);
                tran.Commit();
            }
            return Result.Succeeded;
        }

    }
}
