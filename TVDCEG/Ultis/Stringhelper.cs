using System.Collections.Generic;
using System.Linq;

namespace TVDCEG.LBR
{
    public static class Stringhelper
    {
        public static bool Isonstring(this string s, List<string> list)
        {
            var y = list.Where(x => x == s).ToList();
            if (y.Count != 0) return true;
            else return false;
        }
    }
}
