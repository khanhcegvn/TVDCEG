using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Linq;
using System.IO;

namespace TVDCEG.Extension
{
    [Transaction(TransactionMode.Manual)]
    public class FixduplicateControlNumbercmd : IExternalCommand
    {
        public Document Doc { get; set; }
        public Selection Sel { get; set; }
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Sel = uidoc.Selection;
            Doc = uidoc.Document;
            ICollection<ElementId> list = Sel.GetElementIds();
            List<objectcheck> objects = new List<objectcheck>();
            foreach (var item in list)
            {
                FamilyInstance familyInstance = Doc.GetElement(item) as FamilyInstance;
                objectcheck objectcheck = new objectcheck(familyInstance);
                objects.Add(objectcheck);
            }
            Sortnumber(objects);
            Transaction tran = new Transaction(Doc, "renumber");
            tran.Start();
            foreach (var item in objects)
            {
                FamilyInstance familyInstance = item.FamilyInstance;
                double controlnumber = Convert.ToDouble(familyInstance.LookupParameter("CONTROL_NUMBER").AsString());
                double NEWCTR = controlnumber - 1;
                Parameter pa = familyInstance.LookupParameter("CONTROL_NUMBER");
                string stringctrnumber = NEWCTR.ToString();
                if(stringctrnumber.Length==3)
                {
                    stringctrnumber = "0" + stringctrnumber;
                    pa.Set(stringctrnumber.ToString());
                }
                pa.Set(stringctrnumber);
            }
            tran.Commit();
            return Result.Succeeded;
        }
        public void Sortnumber(List<objectcheck> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < list.Count; j++)
                {
                    if ((list[i].controlnumber) < (list[j].controlnumber))
                    {
                        var temp = list[i];
                        list[i] = list[j];
                        list[j] = temp;
                    }
                }
            }
        }
    }
    public class objectcheck
    {
        public double controlnumber { get; set; }
        public FamilyInstance FamilyInstance { get; set; }
        public objectcheck(FamilyInstance familyInstance)
        {
            FamilyInstance = familyInstance;
            Parameter pa = familyInstance.LookupParameter("CONTROL_NUMBER");
            if(pa!=null)
            {
                string value = pa.AsString();
                if(!string.IsNullOrEmpty(value))
                {
                    controlnumber = Convert.ToDouble(value);
                }
            }
        }
    }
}
