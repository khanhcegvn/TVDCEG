#region Namespaces
using Autodesk.Revit.UI;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using System.Reflection;
using Autodesk.Revit.ApplicationServices;
using System.Windows.Controls;
using System.Windows.Media;
using System.DirectoryServices.ActiveDirectory;
using Autodesk.Revit.DB;
using System;
#endregion

namespace TVDCEG
{
    class App : IExternalApplication
    {
        public Dictionary<string, RibbonItem> ButtonsDictionary;
        internal static App _app = null;

        public static App Instance => _app;
        public Result OnStartup(UIControlledApplication a)
        {
            return Result.Succeeded;
        }

        public void op()
        {
            ButtonsDictionary = _app.ButtonsDictionary;
        }
        public Result OnShutdown(UIControlledApplication a)
        {
          
            return Result.Succeeded;
        }
    }
}
