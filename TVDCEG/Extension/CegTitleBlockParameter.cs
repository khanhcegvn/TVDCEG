using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TVDCEG.Extension
{
	public class CegTitleBlockParameter
	{
		private CegTitleBlockParameter()
		{
		}
		public static CegTitleBlockParameter Instance => _instance ?? (_instance = new CegTitleBlockParameter());
		public Dictionary<string, List<ViewSheet>> AssemblySheetDictionary(Document doc)
		{
			Dictionary<string, List<ViewSheet>> dictionary = new Dictionary<string, List<ViewSheet>>();
			FilteredElementCollector filteredElementCollector = new FilteredElementCollector(doc);
			IList<Element> list = filteredElementCollector.OfClass(typeof(ViewSheet)).ToElements();
			foreach (Element element in list)
			{
				ViewSheet viewSheet = element as ViewSheet;
				bool flag = viewSheet == null;
				if (!flag)
				{
					AssemblyInstance assemblyInstance = doc.GetElement(viewSheet.AssociatedAssemblyInstanceId) as AssemblyInstance;
					bool flag2 = assemblyInstance == null;
					if (!flag2)
					{
						string assemblyTypeName = assemblyInstance.AssemblyTypeName;
						bool flag3 = dictionary.ContainsKey(assemblyTypeName);
						if (flag3)
						{
							bool flag4 = !dictionary[assemblyTypeName].Contains(viewSheet);
							if (flag4)
							{
								dictionary[assemblyTypeName].Add(viewSheet);
							}
						}
						bool flag5 = !dictionary.ContainsKey(assemblyTypeName);
						if (flag5)
						{
							dictionary[assemblyTypeName] = new List<ViewSheet>
							{
								viewSheet
							};
						}
					}
				}
			}
			List<string> list2 = dictionary.Keys.ToList<string>();
			list2.Sort();
			Dictionary<string, List<ViewSheet>> dictionary2 = new Dictionary<string, List<ViewSheet>>();
			foreach (string key in list2)
			{
				dictionary2[key] = dictionary[key];
			}
			return dictionary2;
		}
		public Dictionary<string, FamilyInstance> GetSheetTitleBlock(Document doc, out Dictionary<string, CegParameterSet> sheetParameterDict, out Dictionary<string, CegParameterSet> titleBlockParameterDict, out Dictionary<string, ViewSheet> viewSheetDict, out List<string> sheetList)
		{
			sheetParameterDict = new Dictionary<string, CegParameterSet>();
			titleBlockParameterDict = new Dictionary<string, CegParameterSet>();
			viewSheetDict = new Dictionary<string, ViewSheet>();
			sheetList = new List<string>();
			Dictionary<string, FamilyInstance> dictionary = new Dictionary<string, FamilyInstance>();
			FilteredElementCollector filteredElementCollector = new FilteredElementCollector(doc);
			IList<Element> list = filteredElementCollector.OfClass(typeof(FamilyInstance)).OfCategory(BuiltInCategory.OST_TitleBlocks).ToElements();
			foreach (Element element in list)
			{
				FamilyInstance familyInstance = element as FamilyInstance;
				ViewSheet viewSheet = doc.GetElement((familyInstance != null) ? familyInstance.OwnerViewId : null) as ViewSheet;
				bool flag = viewSheet == null;
				if (!flag)
				{
					string sheetName = GetSheetName(viewSheet);
					dictionary[sheetName] = familyInstance;
					this.GetParameter(viewSheet, familyInstance, ref titleBlockParameterDict, ref sheetParameterDict);
					bool flag2 = !viewSheet.IsAssemblyView && !sheetList.Contains(sheetName);
					if (flag2)
					{
						sheetList.Add(sheetName);
					}
					viewSheetDict[sheetName] = viewSheet;
				}
			}
			sheetList.Sort();
			return dictionary;
		}
		public void GetParameter(ViewSheet viewSheet, FamilyInstance instance, ref Dictionary<string, CegParameterSet> titleBlockParameter, ref Dictionary<string, CegParameterSet> sheetParameter)
		{
			CegParameterSet cegParameterSet = new CegParameterSet();
			foreach (object obj in instance.Parameters)
			{
				Parameter parameter = (Parameter)obj;
				bool isReadOnly = parameter.IsReadOnly;
				if (!isReadOnly)
				{
					string name = parameter.Definition.Name;
					CegParameterInfo value = new CegParameterInfo(parameter);
					cegParameterSet.Parameters[name] = value;
				}
			}
			string sheetName = GetSheetName(viewSheet);
			titleBlockParameter[sheetName] = cegParameterSet;
			CegParameterSet cegParameterSet2 = new CegParameterSet();
			foreach (object obj2 in viewSheet.Parameters)
			{
				Parameter parameter2 = (Parameter)obj2;
				bool isReadOnly2 = parameter2.IsReadOnly;
				if (!isReadOnly2)
				{
					string name2 = parameter2.Definition.Name;
					CegParameterInfo value2 = new CegParameterInfo(parameter2);
					cegParameterSet2.Parameters[name2] = value2;
				}
			}
			sheetParameter[sheetName] = cegParameterSet2;
		}
		public string GetSheetName(ViewSheet viewSheet)
		{
			Document document = viewSheet.Document;
			string text = viewSheet.SheetNumber + "-" + viewSheet.Name;
			bool isAssemblyView = viewSheet.IsAssemblyView;
			if (isAssemblyView)
			{
				AssemblyInstance assemblyInstance = document.GetElement(viewSheet.AssociatedAssemblyInstanceId) as AssemblyInstance;
				bool flag = assemblyInstance != null;
				if (flag)
				{
					text = text + "-" + assemblyInstance.AssemblyTypeName;
				}
			}
			return text;
		}
		private static CegTitleBlockParameter _instance;
	}
}
