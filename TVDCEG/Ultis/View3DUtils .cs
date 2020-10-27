using Autodesk.Revit.DB;
using System.Linq;

namespace TVDCEG.LBR
{
    public static class View3DUtils
    {
        public static bool IsAssemblyview(this View3D view3d)
        {
            if (view3d.AssociatedAssemblyInstanceId.IntegerValue != -1) return true;
            else return false;
        }
    }
}
