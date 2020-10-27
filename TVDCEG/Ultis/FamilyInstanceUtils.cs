using Autodesk.Revit.DB;

namespace TVDCEG.LBR
{
    public static class FamilyInstanceUtils
    {
        public static bool FlipWorkPlane(this FamilyInstance fi)
        {
            var flag = false;
            if (fi.CanFlipWorkPlane)
            {
                flag = true;
                if (fi.IsWorkPlaneFlipped)
                {
                    fi.IsWorkPlaneFlipped = false;
                }
                else
                {
                    fi.IsWorkPlaneFlipped = true;
                }
            }
            return flag;
        }
        public static XYZ Getlocationofinstacne(this FamilyInstance familyInstance)
        {
            LocationPoint locationPoint = familyInstance.Location as LocationPoint;
            if(locationPoint!=null)
            {
                return locationPoint.Point;
            }
            else
            {
                LocationCurve locationCurve = familyInstance.Location as LocationCurve;
                return (locationCurve.Curve as Line).Origin;
            }
        }
        public static FamilyInstance GetSuperInstances(this FamilyInstance familyInstance)
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
