#region Namespaces
using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace TVDCEG
{
    public class ChangeCategoryfamily
    {
        private static ChangeCategoryfamily _instance;
        private ChangeCategoryfamily()
        {

        }
        public static ChangeCategoryfamily Instance => _instance ?? (_instance = new ChangeCategoryfamily());

        public Dictionary<string, List<Category>> Getallcategory(Document familydoc)
        {
            Dictionary<string, List<Category>> dic = new Dictionary<string, List<Category>>();
            using (Transaction t = new Transaction(familydoc, "Modify"))
            {
                t.Start();
                Family f = familydoc.OwnerFamily;
                var gh = familydoc.Settings.Categories;
                foreach (Category i in gh)
                {
                    if (dic.ContainsKey(i.CategoryType.ToString()))
                    {
                        dic[i.CategoryType.ToString()].Add(i);
                    }
                    else
                    {
                        dic.Add(i.CategoryType.ToString(), new List<Category> { i });
                    }
                }
                Category c = f.FamilyCategory;
                t.Commit();
            }
            return dic;
        }
        public Dictionary<string, Category> TypeCategory(Document familydoc)
        {
            var gh = familydoc.Settings.Categories.Cast<Category>().ToList();
            List<Category> value = new List<Category>();
            List<string> list = new List<string> { "Specialty Equipment", "Structural Framing", "Structural Columns", "Generic Models" };
            Dictionary<string, Category> dic = new Dictionary<string, Category>();
            list.ForEach(x => dic.Add(x, (from y in gh where y.Name == x select y).First()));
            return dic;
        }
        public List<FamilySymbol> Getallfamily(Document doc)
        {
            var col = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Cast<FamilySymbol>().ToList();
            return col;
        }
        public void EditandloadFamily(Document doc, List<FamilySymbol> listsymbol)
        {
            string path = System.IO.Path.GetTempPath();
            string fordelsave = path + "\\" + "AFamilyrevit";
            if (!System.IO.Directory.Exists(fordelsave))
            {
                System.IO.Directory.CreateDirectory(fordelsave);
            }
            foreach (FamilySymbol symbol in listsymbol)
            {
                Family family = symbol.Family;
                Document familydoc = doc.EditFamily(family);
                SaveAsOptions saveop = new SaveAsOptions();
                saveop.OverwriteExistingFile = true;
            }
        }

    }
    public class CustomFamilyLoadOption : IFamilyLoadOptions
    {
        public bool OnFamilyFound(bool familyInUse, out bool overwriteParameterValues)
        {
            overwriteParameterValues = true;
            return true;
        }

        public bool OnSharedFamilyFound(Family sharedFamily, bool familyInUse, out FamilySource source, out bool overwriteParameterValues)
        {
            source = FamilySource.Family;
            overwriteParameterValues = true;
            return true;
        }
    }
}
