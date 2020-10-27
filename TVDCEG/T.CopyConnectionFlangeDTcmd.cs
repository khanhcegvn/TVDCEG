﻿#region Namespaces
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
    public class CopyConnectionFlangeDTcmd : IExternalCommand
    {
        public Document doc;
        public List<FamilyInstance> listtarget = new List<FamilyInstance>();
        public FamilyInstance elesource = null;
        private UIDocument uidoc;
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            Reference ef = sel.PickObject(ObjectType.Element, new Filterstructuralframing(), "Selection element source");
            elesource = doc.GetElement(ef) as FamilyInstance;
            ICollection<ElementId> elementIds = Elementlistcopy(doc);
            listtarget = ElementTarget(doc);
            ElementtransformToCopy tr = new ElementtransformToCopy();
            using (FrmCopyConn_Dt_To_Dt form = new FrmCopyConn_Dt_To_Dt(this))
            {
                form.ShowDialog();
                if (form.checkbutton == true)
                {
                    tr.CopyElementsConnFlangeDtee(doc, elesource, listtarget, elementIds, form.Checkfl);
                }
            }
            return Result.Succeeded;
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
        public ICollection<ElementId> Elementlistcopy(Document doc)
        {
            ICollection<ElementId> listconnection = new List<ElementId>();
            Selection sel = uidoc.Selection;
            IList<Reference> gh = sel.PickObjects(ObjectType.Element, new FilterSpecialEquipment(), "Selection");
            foreach (var j in gh)
            {
                Element ee = doc.GetElement(j);
                listconnection.Add(ee.Id);
            }
            return listconnection;
        }
        public List<FamilyInstance> ElementTarget(Document doc)
        {
            Selection sel = uidoc.Selection;
            IList<Reference> gh = sel.PickObjects(ObjectType.Element, new Filterstructuralframing(), "Selection");
            List<FamilyInstance> instances = new List<FamilyInstance>();
            foreach (var t in gh)
            {
                FamilyInstance op = doc.GetElement(t) as FamilyInstance;
                instances.Add(op);
            }
            return instances;
        }
    }
}
