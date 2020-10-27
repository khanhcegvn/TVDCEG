using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TVDCEG.CEG_INFOR
{
   public class CEG_Product
    {
        public string CONTROL_MARK { get; set; }
        public string CONTROL_NUMBER { get; set; }
        public string DIM_LENGTH { get; set; }
        public string DIM_WIDTH { get; set; }
        public string WORKSET { get; set; }
        public string Creater { get; set; }
        public string LastChangeBy { get; set; }
        public string Owner { get; set; }
        public int Id { get; set; }
        public FamilyInstance FamilyInstance;

        public CEG_Product (Document doc,FamilyInstance familyInstance)
        {
            FamilyInstance = familyInstance;
            Id = familyInstance.Id.IntegerValue;
            Parameter PA_CONTROL_MARK = familyInstance.LookupParameter("CONTROL_MARK");
            if (PA_CONTROL_MARK != null)
            {
                CONTROL_MARK = PA_CONTROL_MARK.AsString();
            }
            Parameter PA_CONTROL_NUMBER = familyInstance.LookupParameter("CONTROL_NUMBER");
            if (PA_CONTROL_NUMBER != null)
            {
                CONTROL_NUMBER = PA_CONTROL_NUMBER.AsString();
            }
            Parameter PA_WORKSET = familyInstance.LookupParameter("Workset");
            if (PA_WORKSET != null)
            {
                WORKSET = PA_WORKSET.AsValueString();
            }
            Parameter PA_DIM_LENGTH = familyInstance.LookupParameter("DIM_LENGTH");
            if(PA_DIM_LENGTH!=null)
            {
                DIM_LENGTH = PA_DIM_LENGTH.AsValueString();
            }
            Parameter PA_DIM_WIDTH = familyInstance.LookupParameter("DIM_WIDTH");
            if (PA_DIM_WIDTH != null)
            {
                DIM_WIDTH = PA_DIM_WIDTH.AsValueString();
            }
            WorksharingTooltipInfo worksharingTooltipInfo = WorksharingUtils.GetWorksharingTooltipInfo(doc, familyInstance.Id);
            LastChangeBy = worksharingTooltipInfo.LastChangedBy;
            Owner = worksharingTooltipInfo.Owner;
            Creater = worksharingTooltipInfo.Creator;
        }
    }
}
