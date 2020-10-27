using System;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace TVDCEG
{
	[Transaction(TransactionMode.Manual)]
	public class RebarSymbolCreatorCmd : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			UIApplication application = commandData.Application;
			UIDocument activeUIDocument = application.ActiveUIDocument;
			Application application2 = application.Application;
			Document document = activeUIDocument.Document;
			Selection selection = activeUIDocument.Selection;
			RebarSymbolCreatorController rebarSymbolCreatorController = new RebarSymbolCreatorController(application);
			rebarSymbolCreatorController.ReadData();
			using (Transaction transaction = new Transaction(document))
			{
				transaction.Start("Invention EXT: Rebar Symbol");
				rebarSymbolCreatorController.Execute();
				transaction.Commit();
			}
			return Result.Succeeded;
		}
	}
}
