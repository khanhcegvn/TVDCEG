using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;

namespace TVDCEG
{
    public class MergeTextEvent : EventRegisterHandler
    {
        private MergeTextcmd _data;
        private Document _doc;
        private MergeText _source;
        private List<Reference> list = new List<Reference>();
        private bool flag;
        public MergeTextEvent(Document doc, MergeTextcmd data, MergeText source)
        {
            this._source = source;
            this._data = data;
            this._doc = doc;
        }
        public override void DoingSomething()
        {
            if (_data.btn_pick)
            {
                flag = true;
                _data.listrf = Pickelement(_doc, _data.uidoc.Selection);
            }
        }
        public List<Reference> Pickelement(Document doc, Selection sel)
        {
            List<Reference> list = new List<Reference>();
            while (flag)
            {
                try
                {
                    Reference pick = sel.PickObject(ObjectType.Element, new CEGFilterTextnote(), "Select text note");
                    list.Add(pick);
                }
                catch
                {
                    flag = false;
                    _source.CreateTextNote(_doc, list, _data.uidoc.Selection);
                    list.Clear();
                    return list;
                }
            }
            return list;
        }
    }
}
