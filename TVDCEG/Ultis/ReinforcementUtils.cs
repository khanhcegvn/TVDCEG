using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System.Collections.Generic;
using System.Linq;

namespace TVDCEG.LBR
{
    public static class ReinforcementUtils
    {
        public static IList<Element> GetAllReinforcements(this Document rvtDoc)
        {
            return GetAllRebars(rvtDoc)
            .Concat(GetAllWWMs(rvtDoc))
            .Concat(GetAllRebarInSystemsReinforcements(rvtDoc))
            .Concat(GetAllRebarContainers(rvtDoc)).ToList();
        }
        //----------------------------------------------------
        public static IList<Element> GetAllRebars(Document rvtDoc)
        {
            return new FilteredElementCollector(rvtDoc).OfClass(typeof(Rebar)).ToElements();
        }
        //----------------------------------------------------
        public static IList<Element> GetAllWWMs(Document rvtDoc)
        {
            return new FilteredElementCollector(rvtDoc).OfClass(typeof(FabricSheet)).ToElements();
        }
        //----------------------------------------------------
        public static IList<Element> GetAllRebarInSystemsReinforcements(Document rvtDoc)
        {
            return new FilteredElementCollector(rvtDoc).OfClass(typeof(RebarInSystem)).ToElements();
        }
        //----------------------------------------------------
        public static IList<Element> GetAllRebarContainers(Document rvtDoc)
        {
            return new FilteredElementCollector(rvtDoc).OfClass(typeof(RebarContainer)).ToElements();
        }
    }

}
