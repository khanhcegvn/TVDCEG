using System;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace TVDCEG
{
	[Transaction(TransactionMode.Manual)]
	public class LegendManagerCmd : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			UIApplication application = commandData.Application;
			UIDocument activeUIDocument = application.ActiveUIDocument;
			Application application2 = application.Application;
			Document document = activeUIDocument.Document;
			Selection selection = activeUIDocument.Selection;
			LegendManager data = new LegendManager(document);
			using (LegendManagerForm legendManagerForm = new LegendManagerForm(data))
			{
				legendManagerForm.ShowDialog();
			}
			return Result.Succeeded;
		}
	}
}
