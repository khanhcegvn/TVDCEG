using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;

namespace TVDCEG
{
    public class ElementtransformforGrand
    {
        public static ElementtransformforGrand _instance;
        private ElementtransformforGrand()
        {

        }
        public static ElementtransformforGrand Instance => _instance ?? (_instance = new ElementtransformforGrand());
        public Transform TransformCopyForGrand(Document doc, FamilyInstance instance1, FamilyInstance instance2)
        {
            Transform Newtransform = null;
            Transform transform1 = instance1.GetTransform();
            Transform transform2 = instance2.GetTransform();
            Newtransform = transform1.Multiply(transform2.Inverse);
            return Newtransform;
        }
        //public static bool OriginEqual(this XYZ x, XYZ xYZ)
        // {
        //     if (x.X == xYZ.X && x.Y == xYZ.Y && x.Z == xYZ.Z) return true;
        //     else return false;
        // }
        public void CopyElementsForGrand(Document doc, FamilyInstance familyInstance, List<FamilyInstance> listinstance, ICollection<ElementId> elementIds)
        {
            ICollection<ElementId> newlist = new List<ElementId>();
            CopyPasteOptions option = new CopyPasteOptions();
            ProgressBarform progressBarform = new ProgressBarform(listinstance.Count, "Loading...");
            progressBarform.Show();
            foreach (FamilyInstance source in listinstance)
            {
                progressBarform.giatri();
                if (progressBarform.iscontinue == false)
                {
                    break;
                }
                Transform transform = TransformCopyForGrand(doc, source, familyInstance);
                using (Transaction tran = new Transaction(doc, "copy"))
                {
                    tran.Start();
                    FailureHandlingOptions options = tran.GetFailureHandlingOptions();
                    IgnoreProcess ignoreProcess = new IgnoreProcess();
                    options.SetClearAfterRollback(true);
                    options.SetFailuresPreprocessor(ignoreProcess);
                    tran.SetFailureHandlingOptions(options);
                    try
                    {
                        newlist = ElementTransformUtils.CopyElements(doc, elementIds, doc, transform, option);
                        Remove_product(doc, newlist);
                    }
                    catch (Exception)
                    {

                    }
                    tran.Commit();
                }
            }
            progressBarform.Close();
        }
        public void Remove_product(Document doc, ICollection<ElementId> elementIds)
        {
            foreach (ElementId i in elementIds)
            {
                Element ele = doc.GetElement(i);
                Parameter pa1 = ele.LookupParameter("CONSTRUCTION_PRODUCT_HOST");
                Parameter pa2 = ele.LookupParameter("BOM_PRODUCT_HOST");
                if (pa1 != null)
                {
                    pa1.Set("");
                }
                if (pa2 != null)
                {
                    pa2.Set("");
                }
            }
        }

        public static FamilyInstance Elementcopy(Document doc, AssemblyInstance assembly)
        {
            ICollection<ElementId> Memberid = assembly.GetMemberIds();
            FamilyInstance instance = null;
            foreach (ElementId i in Memberid)
            {
                FamilyInstance familyInstance = doc.GetElement(i) as FamilyInstance;
                if (familyInstance != null)
                {
                    string familyName = familyInstance.Symbol.Category.Name;
                    if (familyName.Equals("Generic Models"))
                    {
                        instance = familyInstance;
                    }
                }
            }
            return instance;
        }
    }
}
