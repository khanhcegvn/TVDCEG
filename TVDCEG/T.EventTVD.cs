#region Namespaces
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using System.Collections.Generic;
#endregion

namespace TVDCEG
{
    public class EventchangeTVD
    {
        public static EventchangeTVD _instance;
        private EventchangeTVD()
        {

        }
        public static EventchangeTVD Instance => _instance ?? (_instance = new EventchangeTVD());
        public void GetElementAdded(object sender, DocumentChangedEventArgs args)
        {
            ICollection<ElementId> elementIds = args.GetAddedElementIds();
            TaskDialog.Show("so luong element duoc add them", elementIds.Count.ToString());
        }
        public void GetElementDelete(object sender, DocumentChangedEventArgs args)
        {
            ICollection<ElementId> elementIds = args.GetDeletedElementIds();
            TaskDialog.Show("so luong element duoc add them", elementIds.Count.ToString());
        }
    }
}
