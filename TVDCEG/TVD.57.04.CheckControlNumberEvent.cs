#region Namespaces
using Autodesk.Revit.DB;
#endregion

namespace TVDCEG
{
    public class CheckControlNumberEvent : EventRegisterHandler
    {
        private CheckControlNumberCmd _data;
        private Document _doc;
        public CheckControlNumberEvent(CheckControlNumberCmd data, Document doc)
        {
            this._data = data;
            _doc = doc;
        }
        public override void DoingSomething()
        {
            if (_data.flag == 0)
            {
                CheckControlNumber.Instance.Showview(_doc, _data.uidoc, _data.listproductid, _doc.ActiveView.Name);
            }
            if (_data.flag == 1)
            {
                CheckControlNumber.Instance.Renumbercontrolnumber(_doc, _data.listRenumber, _data.increase);
            }
        }
    }
}