using Autodesk.Revit.UI;

namespace TVDCEG
{
    public abstract class EventRegisterHandler : IExternalEventHandler
    {
        protected UIApplication _app;
        public bool EventRegistered { get; set; }
        public void Execute(UIApplication app)
        {
            _app = app;
            DoingSomething();
        }

        public string GetName()
        {
            return nameof(EventRegisterHandler);
        }
        public abstract void DoingSomething();
    }
}
