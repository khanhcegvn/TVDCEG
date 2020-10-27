#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
#endregion

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class CopyReforcementGrandcmd : IExternalCommand
    {
        public Document doc;
        public List<FamilyInstance> listtarget = new List<FamilyInstance>();
        public FamilyInstance elesource = null;
        public ICollection<ElementId> listid = null;
        public Dictionary<string, List<FamilyInstance>> dic_element = new Dictionary<string, List<FamilyInstance>>();
        public Dictionary<string, List<FamilyInstance>> dic_connection = new Dictionary<string, List<FamilyInstance>>();
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            Reference ef = sel.PickObject(ObjectType.Element, new AssemblySelectionfilter(), "Select Assembly");
            Element ele = doc.GetElement(ef);
            AssemblyInstance instance = ele as AssemblyInstance;
            elesource = ElementtransformforGrand.Elementcopy(doc, instance);
            dic_element = GetElements(doc, instance);
            IList<Reference> rt = sel.PickObjects(ObjectType.Element, "Select element");
            foreach (Reference k in rt)
            {
                Element it = doc.GetElement(k);
                if (it.Category.Name == "Assemblies")
                {
                    FamilyInstance elementins = ElementtransformToCopy.Elementcopy(doc, it as AssemblyInstance);
                    listtarget.Add(elementins);
                }
                else
                {
                    FamilyInstance ol = it as FamilyInstance;
                    listtarget.Add(ol);
                }
            }

            using (FrmCopyReforcementGrand form = new FrmCopyReforcementGrand(this, doc))
            {
                form.ShowDialog();
                if (form.listcopy.Count != 0)
                {
                    ElementtransformforGrand.Instance.CopyElementsForGrand(doc, elesource, listtarget, form.listcopy);
                }
            }
            return Result.Succeeded;
        }
        public Dictionary<string, List<FamilyInstance>> GetElements(Document doc, AssemblyInstance assembly)
        {
            ICollection<ElementId> Memberid = assembly.GetMemberIds();
            List<FamilyInstance> listinstance = new List<FamilyInstance>();
            foreach (ElementId li in Memberid)
            {
                FamilyInstance yu = doc.GetElement(li) as FamilyInstance;
                listinstance.Add(yu);
            }
            List<FamilyInstance> listBehind = new List<FamilyInstance>();
            foreach (var y in listinstance)
            {
                FamilyInstance instance = GetSuperInstances(y);
                if (!instance.Symbol.Category.Name.Contains("Framing"))
                {
                    listBehind.Add(instance);
                }
            }
            var dic = new Dictionary<string, List<FamilyInstance>>();
            foreach (FamilyInstance inst in listBehind)
            {
                ElementId faid = inst.GetTypeId();
                Element elemtype = doc.GetElement(faid);
                Parameter pa = elemtype.LookupParameter("MANUFACTURE_COMPONENT");
                Parameter po = inst.LookupParameter("MANUFACTURE_COMPONENT");
                if (pa != null)
                {
                    string pa1 = pa.AsString();
                    if (pa1 != null)
                    {
                        if (inst != null)
                        {
                            string categoryName = inst.Symbol.Category.Name;

                            var Familyname = inst.Symbol.Name;
                            if (pa1.Contains("CONNECTION") || pa1.Contains("EMBED"))
                            {
                                if (dic_connection.ContainsKey(Familyname))
                                {
                                    dic_connection[Familyname].Add(inst);
                                }
                                else
                                {
                                    dic_connection.Add(Familyname, new List<FamilyInstance> { inst });
                                }
                            }
                            else
                            {
                                if (dic.ContainsKey(Familyname))
                                {
                                    dic[Familyname].Add(inst);
                                }
                                else
                                {
                                    dic.Add(Familyname, new List<FamilyInstance> { inst });
                                }
                            }

                        }
                    }
                }
                if (po != null)
                {
                    string po1 = po.AsString();
                    if (po1 != null)
                    {
                        if (inst != null)
                        {
                            string categoryName = inst.Symbol.Category.Name;
                            var Familyname = inst.Symbol.Name;
                            if (po1.Contains("CONNECTION") || po1.Contains("EMBED")) continue;
                            if (po1.Contains("REBAR"))
                            {
                                if (dic.ContainsKey(Familyname))
                                {
                                    dic[Familyname].Add(inst);
                                }
                                else
                                {
                                    dic.Add(Familyname, new List<FamilyInstance> { inst });
                                }
                            }
                        }
                    }
                }

            }
            return dic;
        }
        public FamilyInstance GetSuperInstances(FamilyInstance familyInstance)
        {
            FamilyInstance super = null;
            var superinstance = familyInstance.SuperComponent as FamilyInstance;

            if (superinstance == null)
            {
                super = familyInstance;
            }
            else
            {
                super = superinstance;
            }
            return super;
        }
    }
}
