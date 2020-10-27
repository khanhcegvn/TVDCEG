using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;

namespace TVDCEG.LBR
{
    public class RvtController : IController
    {
        public UIApplication UiApp
        {
            get;
            set;
        }

        public UIDocument UiDoc
        {
            get;
            set;
        }

        public Document Doc
        {
            get;
            set;
        }

        public View ActiveView
        {
            get;
            set;
        }
        public XYZ RightDirection { get; set; }
        public XYZ Updirection { get; set; }

        public Selection RSelection
        {
            get;
            set;
        }

        public List<Workset> Worksets
        {
            get;
            set;
        }

        public bool IsValid
        {
            get;
            set;
        }

        public RvtController(UIApplication uiapp)
        {
            UiApp = uiapp;
            UiDoc = UiApp.ActiveUIDocument;
            RSelection = UiApp.ActiveUIDocument.Selection;
            Doc = UiDoc.Document;
            ActiveView = Doc.ActiveView;
            IsValid = true;
            RightDirection = ActiveView.RightDirection;
            Updirection = ActiveView.UpDirection;
        }

        public RvtController(Document doc)
        {

            UiApp = new UIApplication(doc.Application);
            UiDoc = new UIDocument(doc);
            RSelection = UiDoc.Selection;
            Doc = doc;
            ActiveView = Doc.ActiveView;
            RightDirection = ActiveView.RightDirection;
            Updirection = ActiveView.UpDirection;
        }

        public virtual void LoadSettings()
        {
        }

        public virtual void SaveSettings()
        {
        }

        public virtual void Execute()
        {
        }

        public virtual void ReadData()
        {
        }

        public virtual void SaveData()
        {
        }

        public virtual void FilterData(string filter)
        {
        }

        public virtual void CheckedToSelected(int i)
        {
        }

        //public void Cleanup()
        //{
        //    if (Pud != null)
        //    {
        //        Pud.Cleanup();
        //    }
        //}
    }

}
