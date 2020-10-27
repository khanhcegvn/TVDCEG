#region Namespaces
using Autodesk.Revit.DB;
#endregion

namespace TVDCEG
{
    public class HideElementbyworkplaneandlevelEvent : EventRegisterHandler
    {
        private HideElementbyworkplaneandlevel _data;
        private Document _doc;
        private FrmHideElementbyworkplaneandlevel _form;
        public HideElementbyworkplaneandlevelEvent(Document doc, HideElementbyworkplaneandlevel data,FrmHideElementbyworkplaneandlevel form)
        {
            this._data = data;
            this._form = form;
            _doc = doc;
        }
        public override void DoingSomething()
        {
            _data.Hidelementinview(_doc, _doc.ActiveView, _form.ids);
        }
    }
}