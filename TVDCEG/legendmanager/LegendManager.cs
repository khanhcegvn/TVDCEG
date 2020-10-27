using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace TVDCEG
{
	public class LegendManager
	{
		public Dictionary<int, LegendExtension> Dic { get; set; } = new Dictionary<int, LegendExtension>();
		public Document Doc { get; set; }
		private void LoadData(Document doc)
		{
			this.Doc = doc;
			List<ViewSheet> list = this.AllViewSheets(doc);
			List<View> list2 = new List<View>();
			foreach (ViewSheet viewSheet in list)
			{
				List<View> list3 = this.AllLegendInSheet(viewSheet, doc);
				bool flag = list3.Count < 1;
				if (!flag)
				{
					foreach (View view in list3)
					{
						bool flag2 = this.Dic.ContainsKey(view.Id.IntegerValue);
						if (flag2)
						{
							string str = string.Concat(new string[]
							{
								"  + [",
								viewSheet.SheetNumber,
								"_",
								viewSheet.Name,
								"]"
							});
							LegendExtension legendExtension = this.Dic[view.Id.IntegerValue];
							legendExtension.ListSheets += str;
						}
						else
						{
							LegendExtension legendExtension2 = new LegendExtension();
							WorksharingTooltipInfo worksharingTooltipInfo = WorksharingUtils.GetWorksharingTooltipInfo(doc, view.Id);
							legendExtension2.Name = view.Name;
							legendExtension2.Id = view.Id.IntegerValue;
							legendExtension2.Creator = worksharingTooltipInfo.Creator;
							legendExtension2.LastChangedBy = worksharingTooltipInfo.LastChangedBy;
							legendExtension2.ListSheets = string.Concat(new string[]
							{
								"[",
								viewSheet.SheetNumber,
								"_",
								viewSheet.Name,
								"]"
							});
							list2.Add(view);
							this.Dic.Add(view.Id.IntegerValue, legendExtension2);
						}
					}
				}
			}
			foreach (View view2 in this.AllLegends(doc))
			{
				foreach (View view3 in list2)
				{
					bool flag3 = view2.Id.IntegerValue == view3.Id.IntegerValue;
					if (!flag3)
					{
						bool flag4 = this.Dic.ContainsKey(view2.Id.IntegerValue);
						if (!flag4)
						{
							LegendExtension legendExtension3 = new LegendExtension();
							WorksharingTooltipInfo worksharingTooltipInfo2 = WorksharingUtils.GetWorksharingTooltipInfo(doc, view2.Id);
							legendExtension3.Name = view2.Name;
							legendExtension3.Id = view2.Id.IntegerValue;
							legendExtension3.Creator = worksharingTooltipInfo2.Creator;
							legendExtension3.LastChangedBy = worksharingTooltipInfo2.LastChangedBy;
							legendExtension3.ListSheets = "";
							this.Dic.Add(view2.Id.IntegerValue, legendExtension3);
						}
					}
				}
			}
		}
		public LegendManager(Document doc)
		{
			this.LoadData(doc);
		}
		private List<View> AllLegends(Document doc)
		{
			IList<Element> list = new FilteredElementCollector(doc).WhereElementIsNotElementType().OfClass(typeof(View)).ToElements();
			List<View> list2 = new List<View>();
			foreach (Element element in list)
			{
				View view = element as View;
				bool flag = view == null;
				if (!flag)
				{
					bool flag2 = view.ViewType == ViewType.Legend;
					if (flag2)
					{
						list2.Add(view);
					}
				}
			}
			return list2;
		}
		private List<ViewSheet> AllViewSheets(Document doc)
		{
			return new FilteredElementCollector(doc).WhereElementIsNotElementType().OfClass(typeof(ViewSheet)).ToElements().Cast<ViewSheet>().ToList<ViewSheet>();
		}
		private List<View> AllLegendInSheet(ViewSheet viewSheet, Document doc)
		{
			List<View> list = new List<View>();
			ISet<ElementId> allPlacedViews = viewSheet.GetAllPlacedViews();
			foreach (ElementId elementId in allPlacedViews)
			{
				View view = doc.GetElement(elementId) as View;
				bool flag = view == null;
				if (!flag)
				{
					bool flag2 = view.ViewType == ViewType.Legend;
					if (flag2)
					{
						list.Add(view);
					}
				}
			}
			return list;
		}
	}
}
