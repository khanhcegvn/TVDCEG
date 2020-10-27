#region Namespaces
#endregion

namespace TVDCEG
{
    public class AutodimElementEvent : EventRegisterHandler
    {
        private AutodimElement _extension;
        private AutodimElementcmd _data;
        private FrmAutodimelement _form;
        public AutodimElementEvent(AutodimElementcmd data, AutodimElement extension, FrmAutodimelement form)
        {
            _data = data;
            _extension = extension;
            _form = form;
        }
        public override void DoingSomething()
        {
            _extension.Dimwall(_data.doc, _data.sel, _form.DimensionType);
        }
    }
}
