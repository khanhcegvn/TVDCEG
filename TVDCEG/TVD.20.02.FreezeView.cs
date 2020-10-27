#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
#endregion

namespace TVDCEG
{
    public class FreezeView
    {
        private static FreezeView _instance;
        private FreezeView()
        {

        }
        public static FreezeView Instance => _instance ?? (_instance = new FreezeView());
    }
    internal class ATexts
    {
        internal static ResourceManager ResourceManager
        {
            get
            {
                bool flag = ATexts.resourceMan == null;
                if (flag)
                {
                    ResourceManager temp = new ResourceManager("REX.DRevitFreezeDrawing.Resources.Strings.Texts", typeof(ATexts).Assembly);
                    ATexts.resourceMan = temp;
                }
                return ATexts.resourceMan;
            }
        }
        internal static string Freezed
        {
            get
            {
                return ATexts.ResourceManager.GetString("Freezed", ATexts.resourceCulture);
            }
        }
        private static ResourceManager resourceMan;
        private static CultureInfo resourceCulture;
    }
}
