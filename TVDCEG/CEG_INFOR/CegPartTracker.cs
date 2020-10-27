using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TVDCEG.LBR;
using TVDCEG.Ultis;

namespace TVDCEG.CEG_INFOR
{
	public class CegPartTracker:ConstructorSingleton<CegPartTracker>
	{
		public Dictionary<string, List<string>> GetPartConnectionDict(Document doc, out Dictionary<string, List<ElementId>> idDict, out Dictionary<string, int> partCountDict)
		{
			partCountDict = new Dictionary<string, int>();
			idDict = new Dictionary<string, List<ElementId>>();
			Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
			FilteredElementCollector filteredElementCollector = new FilteredElementCollector(doc);
			IList<Element> list = filteredElementCollector.OfClass(typeof(FamilyInstance)).OfCategory(BuiltInCategory.OST_SpecialityEquipment).ToElements();
			foreach (Element element in list)
			{
				FamilyInstance familyInstance = element as FamilyInstance;
				bool flag = familyInstance == null;
				if (!flag)
				{
					Parameter parameter = familyInstance.Symbol.LookupParameter("CONTROL_MARK");
					string text = (parameter != null) ? parameter.AsString() : null;
					bool flag2 = string.IsNullOrEmpty(text);
					if (flag2)
					{
						Parameter parameter2 = familyInstance.LookupParameter("CONTROL_MARK");
						text = ((parameter2 != null) ? parameter2.AsString() : null);
					}
					bool flag3 = string.IsNullOrEmpty(text);
					if (!flag3)
					{
						FamilyInstance superInstance = FamilyInstanceUtils.GetSuperInstances(familyInstance);
						bool flag4 = dictionary.ContainsKey(text);
						if (flag4)
						{
							bool flag5 = !dictionary[text].Contains(superInstance.Symbol.Name);
							if (flag5)
							{
								dictionary[text].Add(superInstance.Symbol.Name);
							}
						}
						else
						{
							dictionary[text] = new List<string>
							{
								superInstance.Symbol.Name
							};
						}
						bool flag6 = idDict.ContainsKey(text);
						if (flag6)
						{
							bool flag7 = !idDict[text].Contains(familyInstance.Id);
							if (flag7)
							{
								idDict[text].Add(familyInstance.Id);
							}
						}
						else
						{
							idDict[text] = new List<ElementId>
							{
								familyInstance.Id
							};
						}
						bool flag8 = partCountDict.ContainsKey(text);
						if (flag8)
						{
							Dictionary<string, int> dictionary2 = partCountDict;
							string key = text;
							int num = dictionary2[key];
							dictionary2[key] = num + 1;
						}
						else
						{
							partCountDict[text] = 1;
						}
					}
				}
			}
			return dictionary;
		}

		public string ListToString(List<string> stringList)
		{
			string text = "";
			foreach (string str in stringList)
			{
				text = text + str + ";";
			}
			bool flag = text.Length > 0;
			if (flag)
			{
				text.Remove(text.Length - 1, 1);
			}
			return text;
		}

		public List<int> SortingOrderValues()
		{
			return new List<int>
			{
				100,
				101,
				102,
				104,
				200,
				201,
				202,
				300,
				301,
				302
			};
		}
	}
}
