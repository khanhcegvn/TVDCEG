#region Namespaces
using Autodesk.Revit.DB;
#endregion

namespace TVDCEG
{
    public class Intersect_event : EventRegisterHandler
    {
        private CheckIntersectcmd _data;
        private Interference_Report _form;
        private FrmCheck _frmCheck;
        private Document _doc;
        public Intersect_event(CheckIntersectcmd data, Document doc,Interference_Report form,FrmCheck frmCheck)
        {
            this._data = data;
            this._frmCheck = frmCheck;
            _doc = doc;
            this._form = form;
        }
        public override void DoingSomething()
        {
            if(_form.s1==1)
            {
                _data.SelectElement(_doc, _data.listshow);
            }
            if(_form.s1==2)
            {
                _data.Showform(_doc, _data.ids,ref _data.dic,_frmCheck);
            }
        }
    }
}