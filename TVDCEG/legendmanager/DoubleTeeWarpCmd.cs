using System;
using System.Windows.Forms;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace TVDCEG
{
	[Transaction(TransactionMode.Manual)]
	public class DoubleTeeWarpCmd : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			UIApplication application = commandData.Application;
			UIDocument activeUIDocument = application.ActiveUIDocument;
			Application application2 = application.Application;
			Document document = activeUIDocument.Document;
			try
			{
				Selection selection = activeUIDocument.Selection;
				DoubleTeeWarpController doubleTeeWarpController = new DoubleTeeWarpController(document);
				TeeWarpForm teeWarpForm = new TeeWarpForm(doubleTeeWarpController);
				teeWarpForm.ShowDialog();
				doubleTeeWarpController.ReadData();
				using (Transaction transaction = new Transaction(document))
				{
					transaction.Start("Warping For Tee");
					doubleTeeWarpController.Execute();
					transaction.Commit();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				return Result.Cancelled;
			}
			return Result.Succeeded;
		}
	}
}
