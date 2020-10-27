using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;


namespace TVDCEG.ExtensionTool
{
	public class DataSource
	{
		public IList<LSObject> getObjecs
		{
			get
			{
				return this.LstObjects;
			}
		}

		public View GetSelection(Selection sel, Document doc)
		{
			bool flag = sel.GetElementIds().Count<ElementId>() > 0;
			if (flag)
			{
				this.legendVP = (doc.GetElement((doc.GetElement(sel.GetElementIds().First<ElementId>()) as Viewport).ViewId) as View);
			}
			return this.legendVP;
		}

		public DataSource(Document doc)
		{
			this.AddObject(doc);
		}

		public IList<LSObject> AddObject(Document doc)
		{
			FilteredElementCollector filteredElementCollector = new FilteredElementCollector(doc).WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_Sheets).OfClass(typeof(ViewSheet));
			foreach (Element element in filteredElementCollector)
			{
				bool flag = element is ViewSheet;
				if (flag)
				{
					ViewSheet viewSheet = element as ViewSheet;
					ICollection<ElementId> allPlacedViews = viewSheet.GetAllPlacedViews();
					foreach (ElementId elementId in allPlacedViews)
					{
						View view = doc.GetElement(elementId) as View;
						LSObject item = new LSObject(view.Name, viewSheet.Name, viewSheet.SheetNumber, view.Id.IntegerValue, viewSheet.Id.IntegerValue);
						this.LstObjects.Add(item);
					}
				}
			}
			return this.LstObjects;
		}

		private IList<LSObject> LstObjects = new List<LSObject>();

		public View legendVP = null;
	}
	public class LSObject
	{
		public string LegendName { get; set; }
		public string SheetName { get; set; }
		public string SheetNumber { get; set; }
		public int IdLegend { get; set; }
		public int IdViewSheet { get; set; }
		public LSObject(string x1, string x2, string x3, int x4, int x5)
		{
			this.LegendName = x1;
			this.SheetName = x2;
			this.SheetNumber = x3;
			this.IdLegend = x4;
			this.IdViewSheet = x5;
		}
	}
}
