#region Namespaces
using Autodesk.Revit.DB;
#endregion

namespace TVDCEG
{
    public class CheckClashProductEvent : EventRegisterHandler
    {
        private CheckClashProductcmd _data;
        private Document _doc;
        public CheckClashProductEvent(Document doc,CheckClashProductcmd data)
        {
            _data = data;
            _doc = doc;
        }
        public override void DoingSomething()
        {
            CheckClashProduct.Instance.SelectElement(_doc,_data.uidoc ,_data.listshow);
        }
    }
}
