using Autodesk.Revit.DB;

namespace TVDCEG
{
    public class DimBrickEvent : EventRegisterHandler
    {
        private DimBrickcmd _data;
        private DimBrick _extension;
        private Document _doc;
        private FrmBrickDim _form;
        public DimBrickEvent(DimBrickcmd data, DimBrick extension, Document doc, FrmBrickDim form)
        {
            this._data = data;
            this._extension = extension;
            this._form = form;
            _doc = doc;
        }
        public override void DoingSomething()
        {
            if (_form.Checktype == 1)
            {
                _extension.CreateDimBrickVertical(_doc, _data.sel, _form.Dimvertical);
            }
            if (_form.Checktype == 2)
            {
                _extension.CreateDimBrickHolizontal(_doc, _data.sel, _form.space, _form.Dimholizontal1, _form.Dimholizontal2);
            }
            if (_form.Checktype == 3)
            {
                _extension.Showbrickdim(_doc);
            }
            if (_form.Checktype == 4)
            {
                _extension.Deletetexnote(_doc);
            }
            if (_form.Checktype == 5)
            {
                _extension.CreateDimBrickVertical2(_doc, _data.sel, _form.Dimvertical);
            }
            if (_form.Checktype == 6)
            {
                _extension.CreateDimBrickHolizontal2(_doc, _data.sel, _form.space, _form.Dimholizontal1, _form.Dimholizontal2);
            }
        }
    }
}
