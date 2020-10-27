#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using TVDCEG.LBR;
using TVDCEG.Ultis;
#endregion

namespace TVDCEG
{
    public class ManagerSectionEvent : EventRegisterHandler
    {
        private ManagerSectioncmd _data;
        private Document _doc;
        public ManagerSectionEvent(ManagerSectioncmd data, Document doc)
        {
            this._data = data;
            _doc = doc;
        }
        public override void DoingSomething()
        {
            _data.Run(_doc);
        }
    }
}
