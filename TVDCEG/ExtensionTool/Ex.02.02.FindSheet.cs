using System;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace TVDCEG.ExtensionTool
{
	[Transaction(TransactionMode.Manual)]
	public class FindSheet : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			UIApplication application = commandData.Application;
			UIDocument activeUIDocument = application.ActiveUIDocument;
			Application application2 = application.Application;
			Document document = activeUIDocument.Document;
			DataSource dataSource = new DataSource(document);
			Selection selection = activeUIDocument.Selection;
			dataSource.GetSelection(selection, document);
			frmFindSheetContainLegend frmFindSheetContainLegend = new frmFindSheetContainLegend(dataSource);
			frmFindSheetContainLegend.ShowDialog();
			bool isTrue = frmFindSheetContainLegend.isTrue;
			if (isTrue)
			{
				ElementId elementId = new ElementId(frmFindSheetContainLegend.idToGo);
				ViewSheet activeView = document.GetElement(elementId) as ViewSheet;
				activeUIDocument.ActiveView = activeView;
			}
			return Result.Succeeded;
		}
	}
}
