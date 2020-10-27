using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TVDCEG.CEG_INFOR
{
   public class CEG_Element
    {
        public string WORKSET { get; set; }
        public int Id { get; set; }
        public string LastChangeBy { get; set; }
        public string Owner { get; set; }
        public string Creater { get; set; }
        public string Name { get; set; }
        public CEG_Element(Document doc, Element ele)
        {
            Name = ele.Name;
            Id = ele.Id.IntegerValue;
            WorksharingTooltipInfo worksharingTooltipInfo = WorksharingUtils.GetWorksharingTooltipInfo(doc, ele.Id);
            LastChangeBy = worksharingTooltipInfo.LastChangedBy;
            Owner = worksharingTooltipInfo.Owner;
            Creater = worksharingTooltipInfo.Creator;
        }
    }
}
